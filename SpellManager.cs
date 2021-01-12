using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using ch.sycoforge.Decal;

public class SpellManager : MonoBehaviour
{
    [SerializeField] private GameObject playerTarget;
    [SerializeField] private GameObject spellProjectile;
    [SerializeField] private GameObject FireballEffect;
    [SerializeField] private GameObject ExplosionSpellEffect;

    private GameObject projectile;
    public DuloGames.UI.UISpellSlot currentCastedSlot;

    private Dictionary<DuloGames.UI.UISpellInfo, GameObject> activeSpellObjects;

    private float GCD = 1.0f;

    private string lastCastedSpell;

    private DuloGames.UI.UISpellSlot[] slots;

    public DuloGames.UI.UISpellSlot slotToReset;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (PlayerController.instance.GetComponent<PlayerManager>().hasTarget)
        {
            playerTarget = PlayerController.instance.GetComponent<PlayerManager>().playerTarget;
            spellProjectile.GetComponent<ProjectileController>().setTarget(PlayerController.instance.GetComponent<PlayerManager>().playerTarget);
        }
        else
        {
            playerTarget = null;
        }

    }

    private IEnumerator Countdown(float time)
    {
        float duration = time;
        float normalizedTime = 0;
        while (normalizedTime <= 1f)
        {
            normalizedTime += Time.deltaTime / duration;
            yield return null;
        }

        yield return 1;
    }



    public bool DeathKiss(DuloGames.UI.UISpellInfo spellInfo)
    {
        float dmg = UnityEngine.Random.Range(spellInfo.EffectAmountMin, spellInfo.EffectAmountMax);
        if (UnityEngine.Random.Range(0f, 1.0f) <= PlayerController.instance.GetComponent<PlayerManager>().playerSpellCrit)
        {
            // CRIT
            dmg *= 1.5f;
        }

        Destroy(Instantiate(ExplosionSpellEffect, PlayerController.instance.transform.position, PlayerController.instance.transform.rotation), 2f);
        Collider[] hitColliders = Physics.OverlapSphere(PlayerController.instance.transform.position, 5f);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.tag == "HostileNPC")
            {
                hitCollider.gameObject.GetComponent<NPCManager>().aoeDamageNPC(dmg, spellInfo);
            }
        }
        return true;
    }

    public bool Fireball(DuloGames.UI.UISpellInfo spellInfo)
    {
        Debug.Log("CASTING FIREBALL");

        float dmg = UnityEngine.Random.Range(spellInfo.EffectAmountMin, spellInfo.EffectAmountMax) + (PlayerController.instance.GetComponent<PlayerManager>().playerSpellPower * spellInfo.EffectAmountModifier);
        if (UnityEngine.Random.Range(0f, 1.0f) <= PlayerController.instance.GetComponent<PlayerManager>().playerSpellCrit)
        {
            // CRIT
            dmg *= 1.5f;
        }

        if (playerTarget != null)
        {
            if (PlayerController.instance.GetComponent<PlayerManager>().playerTarget.GetComponent<NPCManager>().isAttackable && PlayerController.instance.GetComponent<PlayerManager>().playerTarget.GetComponent<NPCManager>().isAlive)
            {
                GameObject go = Instantiate(spellProjectile, PlayerController.instance.transform.position, PlayerController.instance.transform.rotation);
                go.GetComponent<ProjectileController>().setTarget(PlayerController.instance.GetComponent<PlayerManager>().playerTarget);
                go.GetComponent<ProjectileController>().spellInfo = spellInfo;
                go.GetComponent<ProjectileController>().spellDamage = dmg; // WILL NEED THE SPELLINFO SCRIPT AND DATABASE TO HAVE SPELL DAMAGE INSTEAD OF STATIC HARD CODE
                go.GetComponent<ProjectileController>().spellEffect = spellInfo.spellEffect;
                //Instantiate(spellProjectile, PlayerController.instance.transform.position, PlayerController.instance.transform.rotation);
                return true;
            }
        }

        return false;
    }


    public void toggleStealth()
    {
        if (PlayerController.instance.GetComponent<PlayerManager>().isPlayerStealth() == false)
        {
            PlayerController.instance.GetComponent<PlayerManager>().charMesh.GetComponent<Renderer>().material = PlayerController.instance.GetComponent<PlayerManager>().transMat;
            PlayerController.instance.GetComponent<PlayerManager>().currentMat = PlayerController.instance.GetComponent<PlayerManager>().charMesh.GetComponent<Renderer>().material;
            PlayerController.instance.GetComponent<PlayerManager>().ChangeAlpha(0.1f);
            PlayerController.instance.GetComponent<PlayerManager>().isStealth = true;
        }
        else
        {
            PlayerController.instance.GetComponent<PlayerManager>().charMesh.GetComponent<Renderer>().material = PlayerController.instance.GetComponent<PlayerManager>().opaqueMat;
            PlayerController.instance.GetComponent<PlayerManager>().currentMat = PlayerController.instance.GetComponent<PlayerManager>().charMesh.GetComponent<Renderer>().material;
            PlayerController.instance.GetComponent<PlayerManager>().ChangeAlpha(1.0f);
            PlayerController.instance.GetComponent<PlayerManager>().isStealth = false;
        }
    }

    public void Stealth()
    {
        if (PlayerController.instance.GetComponent<PlayerManager>().isPlayerStealth() == false)
        {
            if (!PlayerController.instance.GetComponent<PlayerManager>().inCombat)
            {
                toggleStealth();
            }

        }
        else
        {
            toggleStealth();
        }
    }


    public bool FinalReclamation(DuloGames.UI.UISpellInfo spellInfo)
    {
        Destroy(Instantiate(spellInfo.spellEffect, this.transform.position, this.transform.rotation), 2f);
        Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, 10f); // need to verify the radius and what that looks like in game to match it with the scale of the effect
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.tag.Contains("NPC") && this.gameObject.tag.Contains("Player"))
            {
                hitCollider.gameObject.GetComponent<NPCManager>().aoeDamageNPC(99999f, spellInfo);
            }
        }
        return true;

    }



    //////////////////////////////////////////////////
    ///                 THAUMATURGE                ///
    //////////////////////////////////////////////////
    
    public bool Plasmaball(DuloGames.UI.UISpellInfo spellInfo)
    {
        float dmg = UnityEngine.Random.Range(spellInfo.EffectAmountMin, spellInfo.EffectAmountMax) + (PlayerController.instance.GetComponent<PlayerManager>().playerSpellPower * spellInfo.EffectAmountModifier);
        if (UnityEngine.Random.Range(0f, 1.0f) <= PlayerController.instance.GetComponent<PlayerManager>().playerSpellCrit)
        {
            // CRIT
            dmg *= 1.5f;
        }

        if (playerTarget != null)
        {
            if (PlayerController.instance.GetComponent<PlayerManager>().playerTarget.GetComponent<NPCManager>().isAttackable && PlayerController.instance.GetComponent<PlayerManager>().playerTarget.GetComponent<NPCManager>().isAlive)
            {
                GameObject go = Instantiate(spellProjectile, PlayerController.instance.transform.position, PlayerController.instance.transform.rotation);
                go.GetComponent<ProjectileController>().setTarget(PlayerController.instance.GetComponent<PlayerManager>().playerTarget);
                go.GetComponent<ProjectileController>().spellInfo = spellInfo;
                go.GetComponent<ProjectileController>().spellDamage = dmg; // WILL NEED THE SPELLINFO SCRIPT AND DATABASE TO HAVE SPELL DAMAGE INSTEAD OF STATIC HARD CODE
                go.GetComponent<ProjectileController>().spellEffect = spellInfo.spellEffect;
                return true;
            }
        }
        return false;
    }

    public bool QuantumShift(DuloGames.UI.UISpellInfo spellInfo)
    {
        RaycastHit solid;
        Vector3 destination = transform.position + transform.forward * 20f;
        //transform.position = destination;
        
        if (Physics.Linecast(transform.position, destination, out solid))
        {
            destination = transform.position + transform.forward * (solid.distance - 1f);
        }

        if (Physics.Raycast(destination, -Vector3.up, out solid))
        {
            destination.y = 0.5f;
            destination = solid.point;
            transform.position = destination;
        }

        // PASSIVE FOR QUANTUM SHIFT
        foreach (var item in GameManager.instance.getPlayerInfo().learnedSpells)
        {
            if (item.Name == "Catch Me If You Can")
            {
                if (UnityEngine.Random.Range(0f, 1.0f) <= 0.1f)
                {
                    ResetSpell("Quantum Shift");
                }
                break;
            }
            
        }

        return true;
    }

    public void GravityFieldEffect(Collider[] hitColliders, DuloGames.UI.UISpellInfo spellInfo)
    {
        float dmg = UnityEngine.Random.Range(spellInfo.EffectAmountMin, spellInfo.EffectAmountMax) + (PlayerController.instance.GetComponent<PlayerManager>().playerSpellPower * GameManager.instance.SPELLPOWER_DAMAGE_MULTIPLIER);
        if (UnityEngine.Random.Range(0f, 1.0f) <= PlayerController.instance.GetComponent<PlayerManager>().playerSpellCrit)
        {
            // CRIT
            dmg *= 1.5f;
        }

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.tag == "HostileNPC")
            {
                //hitCollider.transform.position = hitColliders.transform.position;
                hitCollider.gameObject.GetComponent<NPCManager>().aoeDamageNPC(dmg, spellInfo);

            }
        }
    }

    public bool GravityField(DuloGames.UI.UISpellInfo spellInfo)
    {
        spellInfo.spellEffect.transform.localScale = new Vector3(spellInfo.AOERange, spellInfo.AOERange, 0f);
        GameObject go = Instantiate(spellInfo.spellEffect, DuloGames.UI.Demo_CastManager.instance.lastTargetPosition, spellInfo.spellEffect.transform.rotation);
        go.AddComponent<SpellController>();
        go.GetComponent<SpellController>().spellInfo = spellInfo;
        go.GetComponent<SpellController>().spellDuration = spellInfo.Duration;
        return true;
    }

    public bool QuantumDischarge(DuloGames.UI.UISpellInfo spellInfo)
    {
        float dmg = UnityEngine.Random.Range(spellInfo.EffectAmountMin, spellInfo.EffectAmountMax) + (PlayerController.instance.GetComponent<PlayerManager>().playerSpellPower * GameManager.instance.SPELLPOWER_DAMAGE_MULTIPLIER);
        if (UnityEngine.Random.Range(0f, 1.0f) <= PlayerController.instance.GetComponent<PlayerManager>().playerSpellCrit)
        {
            // CRIT
            dmg *= 1.5f;
        }

        spellInfo.spellEffect.transform.localScale = new Vector3(spellInfo.AOERange, spellInfo.AOERange, 0f);
        Destroy(Instantiate(spellInfo.spellEffect, DuloGames.UI.Demo_CastManager.instance.lastTargetPosition, spellInfo.spellEffect.transform.rotation), 2f);
        Collider[] hitColliders = Physics.OverlapSphere(DuloGames.UI.Demo_CastManager.instance.lastTargetPosition, spellInfo.AOERange);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.tag == "HostileNPC")
            {
                hitCollider.gameObject.GetComponent<NPCManager>().aoeDamageNPC(dmg, spellInfo);
            }
        }
        return true;
    }


    //////////////////////////////////////////////////
    ///                VOID STALKER                ///
    //////////////////////////////////////////////////

    public Dictionary<bool, float> VoidAssault(DuloGames.UI.UISpellInfo spellInfo)
    {
        Dictionary<bool, float> dict = new Dictionary<bool, float>();

        float dmg = UnityEngine.Random.Range(spellInfo.EffectAmountMin, spellInfo.EffectAmountMax) + (PlayerController.instance.GetComponent<PlayerManager>().playerAttackPower * GameManager.instance.ATTACKPOWER_DAMAGE_MULTIPLIER);

        // chance to crit
        if (UnityEngine.Random.Range(0f, 1.0f) <= PlayerController.instance.GetComponent<PlayerManager>().playerAttackCrit)
        {
            // CRIT
            dmg *= 1.5f;
        }

        PlayerController.instance.anim.SetTrigger("SlashAttack");
        Destroy(Instantiate(spellInfo.spellEffect, playerTarget.transform.position + new Vector3(0f, 1f, 0f), playerTarget.transform.rotation), 1.5f);
        playerTarget.GetComponent<NPCManager>().damageTargetNPC(dmg, spellInfo);

        dict.Add(true, dmg);

        return dict;
    }

    public bool ShadowSlice(DuloGames.UI.UISpellInfo spellInfo)
    {
        float dmg = UnityEngine.Random.Range(spellInfo.EffectAmountMin, spellInfo.EffectAmountMax) + (PlayerController.instance.GetComponent<PlayerManager>().playerAttackPower * GameManager.instance.ATTACKPOWER_DAMAGE_MULTIPLIER);

        // chance to crit
        if (UnityEngine.Random.Range(0f, 1.0f) <= PlayerController.instance.GetComponent<PlayerManager>().playerAttackCrit)
        {
            // CRIT
            dmg *= 1.5f;
        }

        if (playerTarget != null)
        {
            if (playerTarget.GetComponent<NPCManager>().isAttackable && playerTarget.GetComponent<NPCManager>().isAlive)
            {
                if (Vector3.Distance(playerTarget.transform.position, PlayerController.instance.transform.position) <= spellInfo.Range)
                {
                    PlayerController.instance.anim.SetTrigger("SlashAttack");
                    Destroy(Instantiate(spellInfo.spellEffect, playerTarget.transform.position + new Vector3(0f, 1f, 0f), PlayerController.instance.transform.rotation), 1.5f);
                    playerTarget.GetComponent<NPCManager>().damageTargetNPC(dmg, spellInfo);
                    return true;
                }
            }
        }

        return false;
    }

    public bool VoidNova(DuloGames.UI.UISpellInfo spellInfo)
    {
        float dmg = UnityEngine.Random.Range(spellInfo.EffectAmountMin, spellInfo.EffectAmountMax) + (PlayerController.instance.GetComponent<PlayerManager>().playerAttackPower * GameManager.instance.ATTACKPOWER_DAMAGE_MULTIPLIER);
        if (UnityEngine.Random.Range(0f, 1.0f) <= PlayerController.instance.GetComponent<PlayerManager>().playerAttackCrit)
        {
            // CRIT
            dmg *= 1.5f;
        }

        spellInfo.spellEffect.transform.localScale = new Vector3(spellInfo.AOERange, spellInfo.AOERange, 0f);
        Destroy(Instantiate(spellInfo.spellEffect, PlayerController.instance.transform.position, spellInfo.spellEffect.transform.rotation), 3f);
        Collider[] hitColliders = Physics.OverlapSphere(PlayerController.instance.transform.position, spellInfo.AOERange);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.tag == "HostileNPC")
            {
                hitCollider.gameObject.GetComponent<NPCManager>().aoeDamageNPC(dmg, spellInfo);
                PlayerController.instance.GetComponent<PlayerManager>().playerCurrentHealth += (dmg * 0.35f);
                Debug.Log("Void Nova healed you for " + (dmg * 0.35f).ToString() + ".");
            }
        }
        return true;
    }

    public void GroundedDarknessEffect(Collider[] hitColliders, DuloGames.UI.UISpellInfo spellInfo)
    {
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.tag == "HostileNPC")
            {
                hitCollider.gameObject.GetComponent<NPCManager>().aoeDamageNPC(UnityEngine.Random.Range(spellInfo.EffectAmountMin, spellInfo.EffectAmountMax) + (PlayerController.instance.GetComponent<PlayerManager>().playerAttackPower * GameManager.instance.ATTACKPOWER_DAMAGE_MULTIPLIER), spellInfo);
                hitCollider.gameObject.GetComponent<EnemyController>().agent.speed *= 0.8f;

            }
            else if (hitCollider.gameObject.tag == "Player")
            {
                Debug.Log("Grounded Darkness healed you for " + ((PlayerController.instance.GetComponent<PlayerManager>().playerCurrentHealth * 1.02f) - PlayerController.instance.GetComponent<PlayerManager>().playerCurrentHealth).ToString());
                PlayerController.instance.GetComponent<PlayerManager>().playerCurrentHealth *= 1.02f;
            }
        }
    }

    public bool GroundedDarkness(DuloGames.UI.UISpellInfo spellInfo)
    {
        spellInfo.spellEffect.transform.localScale = new Vector3(spellInfo.AOERange, spellInfo.AOERange, 0f);
        GameObject go = Instantiate(spellInfo.spellEffect, PlayerController.instance.transform.position - new Vector3(0f, 0.5f, 0f), spellInfo.spellEffect.transform.rotation);
        go.AddComponent<SpellController>();
        go.GetComponent<SpellController>().spellInfo = spellInfo;
        go.GetComponent<SpellController>().spellDuration = spellInfo.Duration;

        return true;
    }

    public bool VoidStep(DuloGames.UI.UISpellInfo spellInfo)
    {
        float dmg = UnityEngine.Random.Range(spellInfo.EffectAmountMin, spellInfo.EffectAmountMax) + (PlayerController.instance.GetComponent<PlayerManager>().playerAttackPower * GameManager.instance.ATTACKPOWER_DAMAGE_MULTIPLIER);

        // 50% chance to crit
        if (UnityEngine.Random.Range(0f, 1.0f) <= 0.5f)
        {
            // CRIT
            dmg *= 1.5f;
        }

        if (playerTarget != null)
        {
            if (playerTarget.GetComponent<NPCManager>().isAttackable && playerTarget.GetComponent<NPCManager>().isAlive)
            {
                PlayerController.instance.charCon.enabled = false;
                PlayerController.instance.transform.position = playerTarget.transform.position;
                PlayerController.instance.charCon.enabled = true;
                playerTarget.GetComponent<NPCManager>().damageTargetNPC(dmg, spellInfo);
                return true;
            }
        }

        return false;
    }
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



    // GCD is being called from UICastBar.cs -- GCD starts on cast start like in WoW
    public void globalCooldown()
    {
        // Start the cooldown on all the slots with the specified spell id
        foreach (DuloGames.UI.UISpellSlot s in DuloGames.UI.UISpellSlot.GetSlots())
        {
            if (s.IsAssigned() && s.GetSpellInfo() != null && s.cooldownComponent != null)
            {
                DuloGames.UI.UISpellInfo info = s.GetSpellInfo();

                // Start the global cooldown if spell is not on cooldown or if it has less than GCD time on current cooldown
                if (info.hasGCD)
                {
                    if (!s.cooldownComponent.IsOnCooldown)
                    {
                        s.cooldownComponent.GlobalCooldown(info.ID, GCD);
                        s.cooldownComponent.setGCDTrue();
                    }
                    else if (s.cooldownComponent.IsOnCooldown && ((s.cooldownComponent.getSpellCooldowns()[info.ID].endTime - Time.time) < GCD))
                    {
                        s.cooldownComponent.GlobalCooldown(info.ID, GCD);
                        s.cooldownComponent.setGCDTrue();
                    }
                }
            }
        }
    }


    public void handleCooldown(DuloGames.UI.UISpellInfo spellInfo)
    {
        // Handle cooldown just for the demonstration
        if (currentCastedSlot.cooldownComponent != null && spellInfo.Cooldown > 0f)
        {
            // Start the cooldown on all the slots with the specified spell id
            foreach (DuloGames.UI.UISpellSlot s in DuloGames.UI.UISpellSlot.GetSlots())
            {
                if (s.IsAssigned() && s.GetSpellInfo() != null && s.cooldownComponent != null)
                {
                    // If the slot IDs match
                    if (s.GetSpellInfo().ID == spellInfo.ID)
                    {
                        // Start the cooldown
                        s.cooldownComponent.StartCooldown(spellInfo.ID, spellInfo.Cooldown);
                    }
                }
            }
        }
    }

    public void ResetSpell(string spellName)
    {
        foreach (Transform t in DuloGames.UI.Demo_CastManager.instance.m_SlotContainers)
        {
            slots = t.GetComponentsInChildren<DuloGames.UI.UISpellSlot>();

            foreach (DuloGames.UI.UISpellSlot s in slots)
            {
                // Get the spell info from the slot
                DuloGames.UI.UISpellInfo spellInfo = s.GetSpellInfo();

                // Make sure we have spell info
                if (spellInfo != null)
                {
                    // If the slot spell names match
                    if (s.GetSpellInfo().Name == spellName)
                    {
                        Debug.Log(spellName + " found -- resetting cooldown.");

                        slotToReset = s;
                        
                        return;
                    }
                }
                else
                {
                    Debug.Log("Spell info was null");
                }
            }
        }
    }


    public void cast(DuloGames.UI.UISpellInfo spellInfo)
    {
        if ((spellInfo.requiresTarget && Vector3.Distance(playerTarget.transform.position, PlayerController.instance.transform.position) <= spellInfo.Range) || !spellInfo.requiresTarget)
        {
            if (spellInfo.Name == "Death Kiss")
            {
                if (PlayerController.instance.GetComponent<PlayerManager>().getPlayerCurrentResource() >= spellInfo.PowerCost)
                {
                    bool success = DeathKiss(spellInfo);
                    if (success)
                    {
                        handleCooldown(spellInfo);
                        PlayerController.instance.GetComponent<PlayerManager>().reduceResource(spellInfo.PowerCost);
                    }
                }
                else
                {
                    Debug.Log("Not enough resource for " + spellInfo.Name);
                }
            }

            if (spellInfo.Name == "Fireball")
            {
                if (playerTarget != null)
                {
                    if (PlayerController.instance.GetComponent<PlayerManager>().getPlayerCurrentResource() >= spellInfo.PowerCost)
                    {
                        bool success = Fireball(spellInfo);
                        if (success)
                        {
                            handleCooldown(spellInfo);
                            PlayerController.instance.GetComponent<PlayerManager>().reduceResource(spellInfo.PowerCost);
                        }
                    }
                    else
                    {
                        Debug.Log("Not enough resource for " + spellInfo.Name);
                    }
                }
            }

            if (spellInfo.Name == "Void Step")
            {
                if (playerTarget != null)
                {
                    bool success = VoidStep(spellInfo);
                    if (success)
                    {
                        handleCooldown(spellInfo);
                        lastCastedSpell = "Void Step";
                    }
                }
            }

            if (spellInfo.Name == "Stealth")
            {
                Stealth();
            }

            if (spellInfo.Name == "Shadow Slice")
            {
                if (playerTarget != null)
                {
                    bool success = ShadowSlice(spellInfo);
                    if (success)
                    {
                        handleCooldown(spellInfo);
                        lastCastedSpell = "Shadow Slice";
                    }
                }
            }

            if (spellInfo.Name == "Final Reclamation")
            {
                bool success = FinalReclamation(spellInfo);
                if (success)
                {
                    handleCooldown(spellInfo);
                }
            }

            if (spellInfo.Name == "Void Assault")
            {
                Dictionary<bool, float> success = VoidAssault(spellInfo);
                if (success.ContainsKey(true))
                {
                    handleCooldown(spellInfo);
                    lastCastedSpell = "Void Assault";
                    while (true)
                    {
                        if (UnityEngine.Random.Range(0f, 1.0f) <= 0.50f)
                        {
                            float dmg = success[true];
                            dmg *= 0.25f;

                            // chance to crit
                            if (UnityEngine.Random.Range(0f, 1.0f) <= PlayerController.instance.GetComponent<PlayerManager>().playerAttackCrit)
                            {
                                // CRIT
                                dmg *= 1.5f;
                            }

                            playerTarget.GetComponent<NPCManager>().damageTargetNPC(dmg, spellInfo);

                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }

            if (spellInfo.Name == "Void Nova")
            {
                bool success = VoidNova(spellInfo);
                if (success)
                {
                    handleCooldown(spellInfo);
                    lastCastedSpell = "Void Nova";
                }
            }

            if (spellInfo.Name == "Grounded Darkness")
            {
                bool success = GroundedDarkness(spellInfo);
                if (success)
                {
                    handleCooldown(spellInfo);
                    lastCastedSpell = "Grounded Darkness";
                }
            }

            if (spellInfo.Name == "Plasmaball")
            {
                bool success = Plasmaball(spellInfo);
                if (success)
                {
                    lastCastedSpell = "Plasmaball";
                }
            }

            if (spellInfo.Name == "Quantum Shift")
            {
                bool success = QuantumShift(spellInfo);
                if (success)
                {
                    handleCooldown(spellInfo);
                    lastCastedSpell = "Quantum Shift";
                }
            }

            if (spellInfo.Name == "Gravity Field")
            {
                bool success = GravityField(spellInfo);
                if (success)
                {
                    handleCooldown(spellInfo);
                    lastCastedSpell = "Gravity Field";
                }
            }

            if (spellInfo.Name == "Quantum Discharge")
            {
                bool success = QuantumDischarge(spellInfo);
                if (success)
                {
                    handleCooldown(spellInfo);
                    lastCastedSpell = "Quantum Discharge";
                }
            }
        }
    }

    public void destroyProjectile()
    {
        Destroy(projectile);
    }

    public void setCurrentCastedSlot(DuloGames.UI.UISpellSlot slot)
    {
        this.currentCastedSlot = slot;
    }
}

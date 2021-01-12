using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    //public static NPCManager instance;

    public GameObject npcCharacter;
    public GameObject npcHighlight;
    public string npcName;
    public int npcLevel;
    public float npcMaxHealth;
    public float npcCurrentHealth;
    public float npcMaxResource;
    public float npcCurrentResource;
    public string npcResourceType;
    public float npcAgroRange;
    public float npcStealthAgroRange;
    public float npcMeleeRange;
    public float npcMeleeAttackSpeed;
    public float npcMeleeAttackDamageMax;
    public float npcMeleeAttackDamageMin;
    public float npcMeleeCritChance;

    public bool isElite;

    public float npcExperienceRewardOnKill;
    public float npcRespawnTime;
    public float npcDespawnTime;
    private float npcDespawnCounter;

    public bool isAttackable;
    public bool isTargeted;

    private float healthBarFillPercent;
    private float resourceBarFillPercent;

    public bool isAlive = true;

    public Dictionary<DuloGames.UI.UISpellInfo, float> activeDebuffs;

    public GameObject FloatingTextPrefab; // to display damage done by player above the npc

    private void Awake()
    {
        //instance = this;
        this.GetComponent<NPCManager>().npcDespawnCounter = this.GetComponent<NPCManager>().npcDespawnTime;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (this.GetComponent<NPCManager>().isAlive == false)
        {
            if (this.GetComponent<NPCManager>().npcDespawnCounter >= 0f)
            {
                this.GetComponent<NPCManager>().npcDespawnCounter -= Time.deltaTime;
            } else
            {
                despawnNPC();
            }
        }

        if (PlayerController.instance.GetComponent<PlayerManager>().playerTarget)
        {
            // Always need to be updating health and resource
            PlayerController.instance.GetComponent<PlayerManager>().playerTarget.GetComponent<NPCManager>().healthBarFillPercent = PlayerController.instance.GetComponent<PlayerManager>().playerTarget.GetComponent<NPCManager>().npcCurrentHealth / PlayerController.instance.GetComponent<PlayerManager>().playerTarget.GetComponent<NPCManager>().npcMaxHealth;
            PlayerController.instance.GetComponent<PlayerManager>().playerTarget.GetComponent<NPCManager>().resourceBarFillPercent = PlayerController.instance.GetComponent<PlayerManager>().playerTarget.GetComponent<NPCManager>().npcCurrentResource / PlayerController.instance.GetComponent<PlayerManager>().playerTarget.GetComponent<NPCManager>().npcMaxResource;

            if (PlayerController.instance.GetComponent<PlayerManager>().playerTarget.GetComponent<NPCManager>().isTargeted)
            {
                //Debug.Log(PlayerController.target.GetComponent<NPCManager>().npcName);
                UIController.instance.targetHealthSlider.GetComponent<DuloGames.UI.UIProgressBar>().fillAmount = PlayerController.instance.GetComponent<PlayerManager>().playerTarget.GetComponent<NPCManager>().healthBarFillPercent;
                UIController.instance.targetHealthText.text = Math.Round(PlayerController.instance.GetComponent<PlayerManager>().playerTarget.GetComponent<NPCManager>().npcCurrentHealth).ToString() + " / " + Math.Round(PlayerController.instance.GetComponent<PlayerManager>().playerTarget.GetComponent<NPCManager>().npcMaxHealth).ToString() + " (" + Math.Round((PlayerController.instance.GetComponent<PlayerManager>().playerTarget.GetComponent<NPCManager>().healthBarFillPercent * 100f)).ToString() + "%)";

                if (PlayerController.instance.GetComponent<PlayerManager>().playerTarget.GetComponent<NPCManager>().npcResourceType == "None")
                {
                    UIController.instance.targetResourceSlider.GetComponent<DuloGames.UI.UIProgressBar>().fillAmount = 0f;
                    UIController.instance.targetResourceText.text = "";
                }
                else
                {
                    UIController.instance.targetResourceSlider.GetComponent<DuloGames.UI.UIProgressBar>().fillAmount = PlayerController.instance.GetComponent<PlayerManager>().playerTarget.GetComponent<NPCManager>().resourceBarFillPercent;
                    UIController.instance.targetResourceText.text = Math.Round((PlayerController.instance.GetComponent<PlayerManager>().playerTarget.GetComponent<NPCManager>().resourceBarFillPercent * 100f)).ToString() + "%";
                }
            }
        }
    }

    // OVERLOAD for autoattack damage, no spell info
    public void damageTargetNPC(float amount)
    {
        if (!this.GetComponent<EnemyController>().isEvading && this.GetComponent<NPCManager>().isAlive == true)
        {
            // Agro the npc from hitting it if not already agro'd
            if (!this.GetComponent<EnemyController>().inCombat)
            {
                this.GetComponent<EnemyController>().agroFromPlayerAttack();
            }

            PlayerController.instance.GetComponent<PlayerManager>().playerTarget.GetComponent<EnemyController>().resetLastHit();
            PlayerController.instance.GetComponent<PlayerManager>().playerTarget.GetComponent<NPCManager>().npcCurrentHealth -= amount;

            // Trigger floating text for damage
            if (FloatingTextPrefab && PlayerController.instance.GetComponent<PlayerManager>().playerTarget.GetComponent<NPCManager>().npcCurrentHealth > 0)
            {
                showFloatingText(amount);
            }

            UIController.instance.chatWindow.GetComponent<DuloGames.UI.Demo_Chat>().ReceiveChatMessage(2, PlayerController.instance.GetComponent<PlayerManager>().getPlayerName() + "'s autoattack hit " + this.GetComponent<NPCManager>().npcName + " for " + amount.ToString() + ".");
            if (PlayerController.instance.GetComponent<PlayerManager>().playerTarget.GetComponent<NPCManager>().npcCurrentHealth <= 0)
            {
                if (PlayerController.instance.GetComponent<PlayerManager>().playerTarget.GetComponent<NPCManager>().npcCurrentHealth < 0)
                {
                    PlayerController.instance.GetComponent<PlayerManager>().playerTarget.GetComponent<NPCManager>().npcCurrentHealth = 0;
                }

                // NPC DIES
                //PlayerController.instance.GetComponent<PlayerManager>().removeFromCombatList(this.GetComponent<NPCManager>().npcCharacter);
                //PlayerController.instance.GetComponent<PlayerManager>().playerTarget.GetComponent<NPCManager>().isAlive = false;
                //PlayerController.instance.GetComponent<PlayerManager>().playerTarget.GetComponent<NPCManager>().npcCharacter.transform.gameObject.active = false;
                //rewardPlayerExperience();
                die();

            }
        }
        else
        {
            Debug.Log("Enemy is evading or dead -- cannot hit.");
        }
    }

    public void damageTargetNPC(float amount, DuloGames.UI.UISpellInfo spellInfo)
    {
        if (!this.GetComponent<EnemyController>().isEvading && this.GetComponent<NPCManager>().isAlive == true)
        {
            // Agro the npc from hitting it if not already agro'd
            if (!this.GetComponent<EnemyController>().inCombat)
            {
                this.GetComponent<EnemyController>().agroFromPlayerAttack();
            }

            // Trigger floating text for damage
            if (FloatingTextPrefab && PlayerController.instance.GetComponent<PlayerManager>().playerTarget.GetComponent<NPCManager>().npcCurrentHealth > 0)
            {
                showFloatingText(amount);
            }

            PlayerController.instance.GetComponent<PlayerManager>().playerTarget.GetComponent<EnemyController>().resetLastHit();
            PlayerController.instance.GetComponent<PlayerManager>().playerTarget.GetComponent<NPCManager>().npcCurrentHealth -= amount;

            UIController.instance.chatWindow.GetComponent<DuloGames.UI.Demo_Chat>().ReceiveChatMessage(2, PlayerController.instance.GetComponent<PlayerManager>().getPlayerName() + "'s " + spellInfo.Name + " hit " + this.GetComponent<NPCManager>().npcName + " for " + Math.Round(amount).ToString() + ".");
            if (PlayerController.instance.GetComponent<PlayerManager>().playerTarget.GetComponent<NPCManager>().npcCurrentHealth <= 0)
            {
                if (PlayerController.instance.GetComponent<PlayerManager>().playerTarget.GetComponent<NPCManager>().npcCurrentHealth < 0)
                {
                    PlayerController.instance.GetComponent<PlayerManager>().playerTarget.GetComponent<NPCManager>().npcCurrentHealth = 0;
                }

                // NPC DIES
                //PlayerController.instance.GetComponent<PlayerManager>().removeFromCombatList(this.GetComponent<NPCManager>().npcCharacter);
                //PlayerController.instance.GetComponent<PlayerManager>().playerTarget.GetComponent<NPCManager>().isAlive = false;
                //PlayerController.instance.GetComponent<PlayerManager>().playerTarget.GetComponent<NPCManager>().npcCharacter.transform.gameObject.active = false;
                //rewardPlayerExperience();
                die();

            }
        } else
        {
            Debug.Log("Enemy is evading or dead -- cannot hit.");
        }
    }

    public void aoeDamageNPC(float amount, DuloGames.UI.UISpellInfo spellInfo)
    {
        if (!this.GetComponent<EnemyController>().isEvading && this.GetComponent<NPCManager>().isAlive == true)
        {
            // Agro the npc from hitting it if not already agro'd
            if (!this.GetComponent<EnemyController>().inCombat)
            {
                this.GetComponent<EnemyController>().agroFromPlayerAttack();
            }

            // Trigger floating text for damage
            if (FloatingTextPrefab && this.GetComponent<NPCManager>().npcCurrentHealth > 0)
            {
                showFloatingText(amount);
            }

            this.GetComponent<EnemyController>().resetLastHit();
            this.GetComponent<NPCManager>().npcCurrentHealth -= amount;

            UIController.instance.chatWindow.GetComponent<DuloGames.UI.Demo_Chat>().ReceiveChatMessage(2, PlayerController.instance.GetComponent<PlayerManager>().getPlayerName() + "'s " + spellInfo.Name + " hit " + this.GetComponent<NPCManager>().npcName + " for " + Math.Round(amount).ToString() + ".");
            if (this.GetComponent<NPCManager>().npcCurrentHealth <= 0)
            {
                if (this.GetComponent<NPCManager>().npcCurrentHealth < 0)
                {
                    this.GetComponent<NPCManager>().npcCurrentHealth = 0;
                }

                // NPC DIES
                die();

            }
        } else
        {
            Debug.Log("Enemy evading or dead and was not hit.");
        }
        
    }

    public void showFloatingText(float dmg)
    {
        var go = Instantiate(FloatingTextPrefab, this.transform.position, Quaternion.identity, this.transform);
        go.GetComponent<TextMesh>().text = Math.Round(dmg).ToString();
    }

    public void rewardPlayerExperience()
    {
        int level = PlayerController.instance.GetComponent<PlayerManager>().getPlayerLevel();
        if (level <= this.GetComponent<NPCManager>().npcLevel)
        {
            if (this.GetComponent<NPCManager>().npcLevel - level <= 5)
            {
                this.GetComponent<NPCManager>().npcExperienceRewardOnKill *= 1;
            } else
            {
                this.GetComponent<NPCManager>().npcExperienceRewardOnKill *= 0f;
            }
            
        } else if (level - this.GetComponent<NPCManager>().npcLevel <= 5 && level - this.GetComponent<NPCManager>().npcLevel > 0)
        {
            if (level - this.GetComponent<NPCManager>().npcLevel == 1)
            {
                this.GetComponent<NPCManager>().npcExperienceRewardOnKill *= 0.90f;
            }
            else if (level - this.GetComponent<NPCManager>().npcLevel == 2)
            {
                this.GetComponent<NPCManager>().npcExperienceRewardOnKill *= 0.80f;
            }
            else if (level - this.GetComponent<NPCManager>().npcLevel == 3)
            {
                this.GetComponent<NPCManager>().npcExperienceRewardOnKill *= 0.70f;
            }
            else if (level - this.GetComponent<NPCManager>().npcLevel == 4)
            {
                this.GetComponent<NPCManager>().npcExperienceRewardOnKill *= 0.60f;
            }
            else if (level - this.GetComponent<NPCManager>().npcLevel == 5)
            {
                this.GetComponent<NPCManager>().npcExperienceRewardOnKill *= 0.50f;
            }
        } else if (level - this.GetComponent<NPCManager>().npcLevel > 5)
        {
            this.GetComponent<NPCManager>().npcExperienceRewardOnKill *= 0;
        }
        
        PlayerController.instance.GetComponent<PlayerManager>().addExperience((int)Math.Round(this.GetComponent<NPCManager>().npcExperienceRewardOnKill)); // will need to later check to only give experience to player that this npc is in combat with
        UIController.instance.chatWindow.GetComponent<DuloGames.UI.Demo_Chat>().ReceiveChatMessage(2, "You gained " + ((int)Math.Round(this.GetComponent<NPCManager>().npcExperienceRewardOnKill)).ToString() + " experience.");
    }

    public void die()
    {
        if (PlayerController.instance.GetComponent<PlayerManager>().combatList.Contains(this.GetComponent<NPCManager>().npcCharacter))
        {
            PlayerController.instance.GetComponent<PlayerManager>().removeFromCombatList(this.GetComponent<NPCManager>().npcCharacter);
        }
        
        this.GetComponent<NPCManager>().isAlive = false;
        this.GetComponent<EnemyController>().anim.SetTrigger("Dead");
        //this.GetComponent<NPCManager>().npcCharacter.transform.gameObject.active = false;
        rewardPlayerExperience();
    }

    public void despawnNPC()
    {
        if (PlayerController.instance.GetComponent<PlayerManager>().playerTarget == this.gameObject)
        {
            UIController.instance.targetUnitFrame.active = false;
            PlayerController.instance.GetComponent<PlayerManager>().playerTarget = null;
            PlayerController.instance.GetComponent<PlayerManager>().hasTarget = false;
        }
        Destroy(this.gameObject);
    }

}

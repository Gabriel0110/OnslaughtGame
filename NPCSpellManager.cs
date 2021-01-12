using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSpellManager : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] private string npcName;
    [SerializeField] private GameObject spellProjectile;
    [SerializeField] public GameObject npcAoeSpellArea;
    [SerializeField] private GameObject TestFinalRecEffect; // WILL WANT TO ADD THE EFFECT TO THE SPELL INTHE DATABASE AND GET FROM THERE

    [SerializeField] private string[] m_Spells;

    private GameObject projectile;

    public float timeUntilSpell;
    private float spellCountdown;
    public bool isCasting = false;

    // Start is called before the first frame update
    void Start()
    {
        spellCountdown = timeUntilSpell;
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            if (target.GetComponent<PlayerManager>().isPlayerAlive())
            {
                spellProjectile.GetComponent<ProjectileController>().target = target;
            }
        }

        if (this.GetComponent<EnemyController>().inCombat && spellCountdown > 0f)
        {
            spellCountdown -= Time.deltaTime;
        }

        if (spellCountdown <= 0f && isCasting == false)
        {
            isCasting = true;
            if (m_Spells.Contains("Final Reclamation"))
            {
                this.GetComponent<DuloGames.UI.Demo_CastManager>().NPCSpellCast("Final Reclamation");
            }
        }
    }


    public void FinalReclamation(DuloGames.UI.UISpellInfo spellInfo)
    {
        isCasting = false;
        spellCountdown = timeUntilSpell;
        Destroy(Instantiate(TestFinalRecEffect, this.transform.position, this.transform.rotation), 2f);
        Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, 10f); // need to verify the radius and what that looks like in game to match it with the scale of the effect
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.tag == "Player" && this.gameObject.tag.Contains("NPC"))
            {
                hitCollider.gameObject.GetComponent<PlayerManager>().damagePlayer(this.GetComponent<NPCManager>().npcCharacter, 99999f, "Spell");
            }
        }
        
    }


    public void cast(DuloGames.UI.UISpellInfo spellInfo)
    {
        if (spellInfo.Name == "Final Reclamation")
        {
            FinalReclamation(spellInfo);
        }
    }

    public void destroyProjectile()
    {
        Destroy(projectile);
    }

}

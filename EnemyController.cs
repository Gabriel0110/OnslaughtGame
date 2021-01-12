using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    private bool chasing;
    public float distanceToChase = 10f; // if player within 10 units, chase them
    public float distanceToLose = 50f; // player needs to get 20 units away from the enemy start point before they stop chasing
    public float distanceToStop = 2f; // distance for enemy to stop moving when at the player
    public float chaseCounter = 7f; // how long should the enemy chase for if not hit
    public float keepChasingTime = 5f;
    public float meleeAttackSpeed;
    private float attackWaitCounter;

    private Vector3 targetPoint;
    private Vector3 startPoint; // to have the enemy go back to where it was when it stops chasing
    private Quaternion startRotation;

    private float lastHit = 0f; // when was the enemy last hit by player

    public NavMeshAgent agent;
    public float normalMoveSpeed;

    public Animator anim;

    public bool isEvading;
    public bool cannotChase;
    public bool inCombat;

    // Start is called before the first frame update
    void Start()
    {
        startPoint = this.transform.position;
        startRotation = this.transform.rotation;
        distanceToStop = this.GetComponent<NPCManager>().npcMeleeRange;
        distanceToChase = this.GetComponent<NPCManager>().npcAgroRange;
        meleeAttackSpeed = this.GetComponent<NPCManager>().npcMeleeAttackSpeed;
        attackWaitCounter = 0;

        normalMoveSpeed = agent.speed;
    }

    // Update is called once per frame
    void Update()
    {
        if (this.GetComponent<NPCManager>().isAlive)
        {
            if (this.GetComponent<EnemyController>().inCombat)
            {
                //transform.LookAt(targetPoint);
                lastHit += Time.deltaTime;
                if (attackWaitCounter > 0)
                {
                    attackWaitCounter -= Time.deltaTime;
                }

            }

            targetPoint = PlayerController.instance.transform.position;
            targetPoint.y = transform.position.y;

            if (!chasing)
            {
                if (isEvading && cannotChase)
                {
                    this.GetComponent<NPCManager>().npcCurrentHealth = this.GetComponent<NPCManager>().npcMaxHealth;
                    inCombat = false;
                    attackWaitCounter = 0;
                    if (Vector3.Distance(transform.position, startPoint) <= 0.2)
                    {
                        anim.SetBool("isMoving", false);
                        isEvading = false;
                        cannotChase = false;
                        transform.rotation = startRotation;
                    }
                }

                if (PlayerController.instance.GetComponent<PlayerManager>().isPlayerAlive()) // check if player is alive first
                {
                    if (PlayerController.instance.GetComponent<PlayerManager>().isPlayerStealth()) // reduced agro range if player is stealth
                    {
                        if (Vector3.Distance(transform.position, targetPoint) < this.GetComponent<NPCManager>().npcStealthAgroRange && cannotChase == false) // if player in range, engage
                        {
                            chasing = true;
                            inCombat = true;
                            PlayerController.instance.GetComponent<PlayerManager>().addToCombatList(this.GetComponent<NPCManager>().npcCharacter);
                        }
                    }
                    else // not stealth = regular agro range
                    {
                        if (Vector3.Distance(transform.position, targetPoint) < this.GetComponent<NPCManager>().npcAgroRange && cannotChase == false) // if player in range, engage
                        {
                            chasing = true;
                            inCombat = true;
                            PlayerController.instance.GetComponent<PlayerManager>().addToCombatList(this.GetComponent<NPCManager>().npcCharacter); // MIGHT NEED TO CHECK IF ALREADY IN COMBAT WITH NPC BEFORE ADDING
                        }
                    }


                    if (agent.remainingDistance < 0.25f)
                    {
                        anim.SetBool("isMoving", false);
                    }
                    else
                    {
                        anim.SetBool("isMoving", true);
                    }
                }

            }
            else
            {
                // if player is dead, reset
                if (PlayerController.instance.GetComponent<PlayerManager>().isPlayerAlive() == false)
                {
                    Debug.Log("PLAYER DEAD -- ENEMY RESETTING");
                    anim.SetBool("isMoving", true);
                    chasing = false;
                    isEvading = true;
                    cannotChase = true;
                    inCombat = false;
                    agent.destination = startPoint;
                    return;
                }

                transform.LookAt(targetPoint);
                if (Vector3.Distance(transform.position, targetPoint) > this.GetComponent<NPCManager>().npcMeleeRange)
                {
                    if (this.GetComponent<NPCSpellManager>() == null || (this.GetComponent<NPCSpellManager>() != null && this.GetComponent<NPCSpellManager>().isCasting == false))
                    {
                        agent.destination = targetPoint;
                        anim.SetBool("isMoving", true);
                    }
                }
                else
                {
                    anim.SetBool("isMoving", false);
                    agent.destination = transform.position;

                    if (this.GetComponent<NPCSpellManager>() == null || (this.GetComponent<NPCSpellManager>() != null && this.GetComponent<NPCSpellManager>().isCasting == false))
                    {
                        attack();
                    }
                }

                if (Vector3.Distance(targetPoint, startPoint) > distanceToLose && lastHit >= 7.0f)
                {
                    Debug.Log("ENEMY RESETTING");
                    PlayerController.instance.GetComponent<PlayerManager>().removeFromCombatList(this.GetComponent<NPCManager>().npcCharacter);
                    chasing = false;
                    isEvading = true;
                    cannotChase = true;
                    inCombat = false;
                    agent.destination = startPoint;
                }
            }
        } else
        {
            agent.destination = this.transform.position;
        }
    }

    public void agroFromPlayerAttack()
    {
        chasing = true;
        inCombat = true;
        PlayerController.instance.GetComponent<PlayerManager>().addToCombatList(this.GetComponent<NPCManager>().npcCharacter);
    }

    public void attack()
    {
        if (attackWaitCounter > 0f)
        {
            //anim.SetBool("isAttacking", false);

        } else
        {
            if (PlayerController.instance.GetComponent<PlayerManager>().isPlayerAlive())
            {
                anim.SetTrigger("Attack");
                float dmg = calcDamage(this.GetComponent<NPCManager>().npcMeleeAttackDamageMin, this.GetComponent<NPCManager>().npcMeleeAttackDamageMax);

                PlayerController.instance.GetComponent<PlayerManager>().damagePlayer(this.GetComponent<NPCManager>().npcCharacter, dmg, "Melee");
                attackWaitCounter = meleeAttackSpeed;

                //if (PlayerController.instance.GetComponent<PlayerManager>().hasTarget == false)
                //{
                //    PlayerController.instsance.GetComponent<PlayerManager>().playerTarget = this.GetComponent<NPCManager>().npcCharacter;
                //}

            } else
            {
                Debug.Log("PLAYER DEAD -- ENEMY RESETTING");
                chasing = false;
                isEvading = true;
                cannotChase = true;
                inCombat = false;
                agent.destination = startPoint;
            }
        }
    }

    public float calcDamage(float min, float max)
    {
        float dmg = UnityEngine.Random.Range(min, max);

        // Check for crit
        if (UnityEngine.Random.Range(0f, 1f) <= this.GetComponent<NPCManager>().npcMeleeCritChance)
        {
            dmg *= 1.5f;
            Debug.Log("Enemy crit player for " + Math.Round(dmg).ToString());
        } else
        {
            Debug.Log("Enemy hit player for " + Math.Round(dmg).ToString());
        }

        return (float)Math.Round(dmg);
    }

    public void resetLastHit()
    {
        lastHit = 0f;
        Debug.Log("LASTHIT: " + lastHit.ToString());
    }
}

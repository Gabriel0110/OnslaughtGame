using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DuloGames.UI;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;
    public GameObject player;
    private UICharacterInfo playerInfo;

    public bool isGameMaster; // ALLOWS FOR GAMEMASTER COMMANDS IN-GAME -- SET OFF IF NO LONGER NEEDED
    private static string playerName; // DEFAULT -- REMOVE ASSIGNMENT WHEN READY
    public string playerClass;
    public int playerCurrentLevel; // DEFAULT -- REMOVE ASSIGNMENT WHEN READY
    public int playerMaxLevel = 50;
    public int playerStrength;
    public int playerStamina;
    public int playerAgility;
    public int playerIntellect;
    public int playerArmor;
    public float armorMitigation;
    public float playerAttackCrit;
    public float playerSpellCrit;
    public float playerAttackPower;
    public float playerSpellPower;
    public int playerCurrentExperience;
    public int playerMaxExperience;
    public float playerCurrentHealth;
    public float playerMaxHealth;
    public float playerCurrentResource;
    public float playerMaxResource;
    public string playerResourceType;
    public int playerCurrentPvpRank;
    public int playerCurrentHordeRound;

    public int playerAvailableTalentPoints;
    public int playerTotalTalentPoints;

    public Vector3 playerLastPosition;
    public Quaternion playerLastRotation;

    public float playerMeleeDamageMin; // NEEDS TO BE SET TO WEAPON MINIMUM DAMAGE (WIP)
    public float playerMeleeDamageMax; // NEEDS TO BE SET TO WEAPON MAXIMUM DAMAGE (WIP)
    public float playerMeleeAttackRange;

    private bool noWeaponEquipped = true;

    public float playerUnarmedAttackSpeed = 2f;
    private float attackWaitCounter;

    private float healthBarFillPercent;
    private float resourceBarFillPercent;

    public bool hasTarget;  // check if player has something targeted
    public GameObject playerTarget; // holds the object that the player is targeting (NPC/enemy)
    private Vector3 targetPos;

    private bool attackReady;

    private bool isAlive = true;
    public bool inCombat = false;
    public bool canMove = true;
    private bool isStunned = false;

    public List<GameObject> combatList;
    public Dictionary<DuloGames.UI.UISpellInfo, float> activeDebuffs;

    public GameObject charMesh;
    public Material opaqueMat;
    public Material transMat;
    public Material currentMat; // for stealth
    public bool isStealth = false;

    public GameObject aoeSpellEffect;


    private void Awake()
    {
        instance = this;

    }

    // Start is called before the first frame update
    void Start()
    {
        playerName = GameManager.instance.getCurrentCharacterName();
        playerInfo = UICharacterDatabase.Instance.GetByName(playerName);
        playerClass = playerInfo.playerClass;
        isGameMaster = playerInfo.isGameMaster;

        playerCurrentLevel = playerInfo.playerCurrentLevel;

        playerStrength = playerInfo.playerStrength;
        playerStamina = playerInfo.playerStamina;
        playerIntellect = playerInfo.playerIntellect;
        playerAgility = playerInfo.playerAgility;
        playerArmor = playerInfo.playerArmor;

        updateStats();

        playerCurrentExperience = playerInfo.playerCurrentExperience;
        playerMaxExperience = GameManager.instance.expReqDict[playerCurrentLevel];

        playerAvailableTalentPoints = playerInfo.playerAvailableTalentPoints;
        playerTotalTalentPoints = playerInfo.playerTotalTalentPoints;


        hasTarget = false;

        UIController.instance.playerNameText.text = playerName;
        currentMat = charMesh.GetComponent<Renderer>().material;

        attackWaitCounter = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (combatList.Count > 0)
        {
            inCombat = true;
            if (isStealth)
            {
                PlayerController.instance.GetComponent<SpellManager>().toggleStealth();
            }

            if (attackWaitCounter > 0)
            {
                attackWaitCounter -= Time.deltaTime;
            }

        } else
        {
            inCombat = false;
        }


        // Regenerate health out of combat
        if (!inCombat && isAlive && playerCurrentHealth < playerMaxHealth)
        {
            playerCurrentHealth *= 1.00009f;
        }

        // Regenerate mana out of combat
        if (!inCombat && isAlive && playerCurrentResource < playerMaxResource)
        {
            playerCurrentResource *= 1.0001f;
        }

        // Always need to be updating player health and resource
        healthBarFillPercent = playerCurrentHealth / playerMaxHealth;
        resourceBarFillPercent = playerCurrentResource / playerMaxResource;
        UIController.instance.healthSlider.GetComponent<DuloGames.UI.UIProgressBar>().fillAmount = healthBarFillPercent;
        UIController.instance.resourceSlider.GetComponent<DuloGames.UI.UIProgressBar>().fillAmount = resourceBarFillPercent;
        UIController.instance.healthText.text = Math.Round(playerCurrentHealth).ToString() + " / " + Math.Round(playerMaxHealth).ToString() + " (" + Math.Round((healthBarFillPercent * 100f)).ToString() + "%)";
        UIController.instance.resourceText.text = Math.Round(playerCurrentResource).ToString() + " / " + Math.Round(playerMaxResource).ToString() + " (" + Math.Round((resourceBarFillPercent * 100f)).ToString() + "%)";

        // Always need to be updating player level and experience
        UIController.instance.playerLevelText.text = getPlayerLevel().ToString();
        if (getPlayerLevel() < 50)
        {
            UIController.instance.expText.text = getPlayerExperience() + " / " + getPlayerMaxExperience() + " (" + Convert.ToInt32(((float)((float)(playerCurrentExperience) / (float)(playerMaxExperience))) * 100f) + "%)";
            UIController.instance.expSlider.GetComponent<DuloGames.UI.UIProgressBar>().fillAmount = (float)((float)(playerCurrentExperience) / (float)(playerMaxExperience));
        } else
        {
            UIController.instance.expText.text = "";
            UIController.instance.expSlider.GetComponent<DuloGames.UI.UIProgressBar>().fillAmount = 1;
        }


        // Control player autoattack
        if (playerTarget != null)
        {
            hasTarget = true;

            if (playerTarget.GetComponent<NPCManager>().isAlive)
            {
                if (attackReady)
                {
                    //Debug.Log("ATTACK READY -- TARGET: " + PlayerController.target.GetComponent<NPCManager>().npcName);

                    // START AUTOATTACKING (set an autoattacking variable to true and activate attacking animation
                    // Constantly check for range to the target -- if out of range, stop attacking animation but keep attackReady true until target removed or player stops attack with keybind like WoW's T

                    targetPos = PlayerController.target.transform.position;
                    if (Vector3.Distance(PlayerController.instance.transform.position, targetPos) < playerMeleeAttackRange)
                    {
                        // ATTACK STUFF
                        autoAttack();
                    }
                    else
                    {
                        Debug.Log("TOO FAR AWAY FROM TARGET TO ATTACK");
                    }
                }
            }
        } else
        {
            hasTarget = false;
            attackReady = false;
        }
        

    }

    void LateUpdate()
    {
        // update armor mitigation per amount of armor
        armorMitigation = (playerArmor * 0.01f) / 100f;

        playerLastPosition = PlayerController.instance.GetComponent<PlayerManager>().player.transform.position;
        playerLastRotation = PlayerController.instance.GetComponent<PlayerManager>().player.transform.rotation;
    }


    // FOR TESTING STEALTH
    public void ChangeAlpha(float alphaVal)
    {
        Color oldColor = currentMat.color;
        Color newColor = new Color(oldColor.r, oldColor.g, oldColor.b, alphaVal);
        currentMat.SetColor("_Color", newColor);

    }

    public void autoAttack()
    {
        if (attackWaitCounter > 0f)
        {
            //anim.SetBool("isAttacking", false);

        }
        else
        {
            if (PlayerController.instance.GetComponent<PlayerManager>().playerTarget.GetComponent<NPCManager>().isAlive)
            {
                PlayerController.instance.anim.SetTrigger("AttackTrigger");
                float dmg = calcDamage(playerMeleeDamageMin, playerMeleeDamageMax);

                PlayerController.instance.GetComponent<PlayerManager>().playerTarget.GetComponent<NPCManager>().damageTargetNPC(dmg);

                if (noWeaponEquipped)
                {
                    attackWaitCounter = playerUnarmedAttackSpeed;
                }
                

                //if (PlayerController.instance.GetComponent<PlayerManager>().hasTarget == false)
                //{
                //    PlayerController.instsance.GetComponent<PlayerManager>().playerTarget = GetComponent<NPCManager>().npcCharacter;
                //}

            }
            else
            {
                Debug.Log("TARGET DEAD - CAN'T ATTACK");
            }
        }
    }


    public float calcDamage(float min, float max)
    {
        float dmg = UnityEngine.Random.Range(min, max) + (playerAttackPower * 0.25f);

        // Check for crit
        if (UnityEngine.Random.Range(0f, 1f) <= playerAttackCrit)
        {
            dmg *= 1.5f;
        }

        return (float)Math.Round(dmg);
    }


    public void updateStats()
    {
        playerMaxHealth = playerStamina * GameManager.instance.STAMINA_HEALTH_MULTIPLIER;
        playerCurrentHealth = playerMaxHealth;

        playerMaxResource = playerIntellect * GameManager.instance.INTELLECT_MANA_MULTIPLIER;
        playerCurrentResource = playerMaxResource;

        playerAttackCrit = 0.05f + playerAgility * GameManager.instance.AGILITY_CRIT_MULTIPLIER;
        playerSpellCrit = 0.05f + playerIntellect * GameManager.instance.INTELLECT_CRIT_MULTIPLIER;

        playerAttackPower = playerAgility * GameManager.instance.AGILITY_ATTACKPOWER_MULTIPLIER + playerStrength * GameManager.instance.STRENGTH_ATTACKPOWER_MULTIPLIER;
        playerSpellPower = playerIntellect * GameManager.instance.INTELLECT_SPELLPOWER_MULTIPLIER;
    }


    public bool isPlayerStealth()
    {
        return isStealth;
    }

    public bool isPlayerAlive()
    {
        return isAlive;
    }

    public bool isPlayerStunned()
    {
        return isStunned;
    }

    public void setAttackReady(bool isReady)
    {
        attackReady = isReady;
    }

    public bool getAttackReady()
    {
        return attackReady;
    }

    public void reduceResource(float amount)
    {
        playerCurrentResource -= amount;
        if (playerCurrentResource <= 0)
        {
            playerCurrentResource = 0;
        }
    }

    public float getPlayerCurrentResource()
    {
        return playerCurrentResource;
    }

    public string getPlayerName()
    {
        return playerName;
    }

    public int getPlayerExperience()
    {
        return playerCurrentExperience;
    }

    public void setPlayerExperience(int amount)
    {
        playerCurrentExperience = amount;
    }

    public int getPlayerMaxExperience()
    {
        return playerMaxExperience;
    }

    public void setPlayerMaxExperience(int amount)
    {
        playerMaxExperience = amount;
    }

    public int getPlayerLevel()
    {
        return playerCurrentLevel;
    }

    public void incrementPlayerLevel()
    {
        playerCurrentLevel++;
    }

    public void decrementPlayerLevel()
    {
        playerCurrentLevel--;
    }

    public void levelUp()
    {
        // LEVEL-UP
        incrementPlayerLevel();
        setPlayerExperience(playerCurrentExperience % playerMaxExperience);
        int level = getPlayerLevel();
        if (level < playerMaxLevel)
        {
            setPlayerMaxExperience(GameManager.instance.expReqDict[level]);
        }

        // start lvl notification time, show it, wait for 4 second difference in Update(), then turn it off and set the notification time back to default
        UIController.instance.levelNotificationText.text = getPlayerLevel().ToString();
        UIController.instance.levelNotification.active = true;
        UIController.instance.levelNotificationStartTime = Time.time;

        // boost stats, etc
        playerStamina += 6;
        playerStrength += 2;
        playerAgility += 2;
        playerIntellect += 3;
        playerAvailableTalentPoints += 1;
        playerTotalTalentPoints += 1;
        updateStats();

        // set health and resource
        playerCurrentHealth = playerMaxHealth;
        playerCurrentResource = playerMaxResource;
    }

    public void levelDown()
    {
        decrementPlayerLevel();
        setPlayerExperience(0);
        int level = getPlayerLevel();
        if (level < playerMaxLevel)
        {
            setPlayerMaxExperience(GameManager.instance.expReqDict[level]);
        }

        UIController.instance.levelNotificationText.text = getPlayerLevel().ToString();
        UIController.instance.levelNotification.active = true;
        UIController.instance.levelNotificationStartTime = Time.time;

        playerStamina -= 6;
        playerStrength -= 2;
        playerAgility -= 2;
        playerIntellect -= 3;
        playerAvailableTalentPoints -= 1;
        playerTotalTalentPoints -= 1;
        updateStats();

        // set health and resource
        playerCurrentHealth = playerMaxHealth;
        playerCurrentResource = playerMaxResource;
    }

    public void addExperience(int amount)
    {
        // This function should be called with the experience amount whenever player should be given experience for some reason (e.g. kills, quest completions, etc)
        if (getPlayerLevel() < playerMaxLevel)
        {
            setPlayerExperience(getPlayerExperience() + amount);
        }
        
        if (playerCurrentExperience >= playerMaxExperience && (getPlayerLevel() != playerMaxLevel))
        {
            levelUp();
        }
    }

    public void damagePlayer(GameObject source, float amount, string dmgType)
    {
        // only deal damage to player if they are not set to game master
        if (isGameMaster == false)
        {
            // MITIGATE MELEE DAMAGE FROM ARMOR -- NOT SPELL DAMAGE
            if (dmgType == "Melee")
            {
                playerCurrentHealth -= (amount - (amount * armorMitigation));
            } else if (dmgType == "Spell")
            {
                playerCurrentHealth -= amount;
            }
        }

        UIController.instance.chatWindow.GetComponent<DuloGames.UI.Demo_Chat>().ReceiveChatMessage(2, source.GetComponent<NPCManager>().npcName + " hit you for " + amount.ToString() + ".");

        if (playerCurrentHealth <= 0)
        {
            if (playerCurrentHealth < 0)
            {
                playerCurrentHealth = 0;
            }

            // PLAYER DIES
            die();
        }
    }

    public void addToCombatList(GameObject go)
    {
        combatList.Add(go);
    }

    public void removeFromCombatList(GameObject go)
    {
        combatList.Remove(go);
    }

    public void clearCombatList()
    {
        combatList.Clear();
    }

    public void die()
    {
        clearCombatList();
        isAlive = false;
        inCombat = false;
        canMove = false;
        playerCurrentHealth = 0;
        playerCurrentResource = 0;
        PlayerController.instance.anim.SetTrigger("Dead");
        //PlayerController.instance.anim.ResetTrigger("Revived");
        PlayerController.instance.anim.SetBool("MovingForward", false);
        PlayerController.instance.anim.SetBool("MovingBackward", false);
        PlayerController.instance.anim.SetBool("StrafingLeft", false);
        PlayerController.instance.anim.SetBool("StrafingRight", false);
        PlayerController.instance.anim.SetBool("Jumping", false);
    }

    public void resurrect()
    {
        // WIP -- when player runs back to body to rez
        //PlayerController.instance.anim.ResetTrigger("Dead");
        PlayerController.instance.anim.SetTrigger("Revived");
        isAlive = true;
        canMove = true;
        playerCurrentHealth = playerMaxHealth * 0.2f;
        playerCurrentResource = playerMaxResource * 0.2f;
        //PlayerController.instance.anim.ResetTrigger("Revived");
    }
}

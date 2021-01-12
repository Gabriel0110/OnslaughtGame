using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject playerPrefab;
    public GameObject currentPlayerObject;

    public GameObject UIPrefab;

    public static string currentCharacterName; // STATICALLY SET UNTIL LOGIN SCREEN/CHAR SELECT SCREENS ARE WORKING
    public static string currentCharacterClass; // STATICALLY SET UNTIL LOGIN SCREEN/CHAR SELECT SCREENS ARE WORKING
    public static bool isCurrentCharacterGameMaster;

    public Vector3 playerLastPosition;
    public Quaternion playerLastRotation;

    public float STAMINA_HEALTH_MULTIPLIER;
    public float INTELLECT_MANA_MULTIPLIER;
    public float AGILITY_CRIT_MULTIPLIER;
    public float INTELLECT_CRIT_MULTIPLIER;
    public float AGILITY_ATTACKPOWER_MULTIPLIER;
    public float STRENGTH_ATTACKPOWER_MULTIPLIER;
    public float ATTACKPOWER_DAMAGE_MULTIPLIER;
    public float INTELLECT_SPELLPOWER_MULTIPLIER;
    public float SPELLPOWER_DAMAGE_MULTIPLIER;

    public int levelReq;
    public Dictionary<int, int> expReqDict;

    private DuloGames.UI.UICharacterInfo playerInfo;

    public Transform[] spellSlots;
    public Transform[] keybindSlots;

    void Awake()
    {
        instance = this;
        levelReq = 2000;
        expReqDict = new Dictionary<int, int>();

        setLevelRequirements();
    }

    // Start is called before the first frame update
    void Start()
    {
        STAMINA_HEALTH_MULTIPLIER = 12f; // each stamina point gives 12 hp
        INTELLECT_MANA_MULTIPLIER = 10f; // each intellect point gives 10 mana
        AGILITY_CRIT_MULTIPLIER = 0.0005f; // each agility point gives 0.0005% melee/range crit chance
        INTELLECT_CRIT_MULTIPLIER = 0.00045f; // each intellect point gives 0.0005% SPELL crit chance
        AGILITY_ATTACKPOWER_MULTIPLIER = 0.5f; // each point of agility gives 0.5 attack power
        STRENGTH_ATTACKPOWER_MULTIPLIER = 1f; // each point of strength gives 1 attack power
        ATTACKPOWER_DAMAGE_MULTIPLIER = 0.575f; // each point of attack power increases melee/ranged damage by 1.5  (WILL NEED TO MULTIPLY THIS TO ABILITY DAMAGE)
        INTELLECT_SPELLPOWER_MULTIPLIER = 1.5f; // each point of intellect gives 1 spell power
        SPELLPOWER_DAMAGE_MULTIPLIER = 0.575f; // each point of spell power inceases spell damage by 1.5  (WILL NEED TO MULTIPLY THIS TO ABILITY DAMAGE)

        Debug.Log(DuloGames.UI.UILoadScene.currentLoadedCharacter);
        loadSelectedChar(DuloGames.UI.UILoadScene.currentLoadedCharacter);
        GameManager.isCurrentCharacterGameMaster = DuloGames.UI.UICharacterDatabase.Instance.GetByName(DuloGames.UI.UILoadScene.currentLoadedCharacter).isGameMaster;

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void setLevelRequirements()
    {
        // Setup level exp requirements
        for (int i = 1; i <= 50; i++)
        {
            if (i == 1)
            {
                expReqDict.Add(i, levelReq);
            }
            else if (i > 1 && i < 20)
            {
                levelReq = (int)(Math.Round(levelReq * 1.3));
                expReqDict.Add(i, levelReq);
            }
            else if (i >= 20 && i < 30)
            {
                levelReq = (int)(Math.Round(levelReq * 1.125));
                expReqDict.Add(i, levelReq);
            }
            else if (i >= 30)
            {
                levelReq = (int)(Math.Round(levelReq * 1.05));
                expReqDict.Add(i, levelReq);
            }
        }
    }

    public string getCurrentCharacterName()
    {
        return currentCharacterName;
    }

    public string getCurrentCharacterClass()
    {
        return currentCharacterClass;
    }

    public DuloGames.UI.UICharacterInfo getCharacterInfo(string name)
    {
        List<DuloGames.UI.UICharacterInfo> info = DuloGames.UI.UICharacterDatabase.Instance.GetCharacters();
        for (int i = 0; i < info.Count; i++)
        {
            if (info[i].playerName == name)
            {
                return info[i];
            }
        }
        return null;
    }

    public DuloGames.UI.UICharacterInfo getPlayerInfo()
    {
        return playerInfo;
    }

    public void logout()
    {
        // SAVE ALL LOCAL CHARACTER AND GAME STUFF THAT SHOULD BE SAVED, THEN LOGOUT
        playerInfo = DuloGames.UI.UICharacterDatabase.Instance.GetByName(currentCharacterName);
        playerInfo.playerCurrentLevel = PlayerController.instance.GetComponent<PlayerManager>().playerCurrentLevel;
        playerInfo.playerStamina = PlayerController.instance.GetComponent<PlayerManager>().playerStamina;
        playerInfo.playerStrength = PlayerController.instance.GetComponent<PlayerManager>().playerStrength;
        playerInfo.playerAgility = PlayerController.instance.GetComponent<PlayerManager>().playerAgility;
        playerInfo.playerIntellect = PlayerController.instance.GetComponent<PlayerManager>().playerIntellect;
        playerInfo.playerArmor = PlayerController.instance.GetComponent<PlayerManager>().playerArmor;
        playerInfo.playerMoveSpeed = PlayerController.instance.moveSpeed;
        playerInfo.playerCurrentExperience = PlayerController.instance.GetComponent<PlayerManager>().playerCurrentExperience;
        playerInfo.playerAvailableTalentPoints = PlayerController.instance.GetComponent<PlayerManager>().playerAvailableTalentPoints;
        playerInfo.playerTotalTalentPoints = PlayerController.instance.GetComponent<PlayerManager>().playerTotalTalentPoints;
        playerInfo.playerCurrentPvpRank = PlayerController.instance.GetComponent<PlayerManager>().playerCurrentPvpRank;
        playerInfo.playerCurrentHordeRound = PlayerController.instance.GetComponent<PlayerManager>().playerCurrentHordeRound;
        playerInfo.playerLastPosition = PlayerController.instance.GetComponent<PlayerManager>().playerLastPosition;
        playerInfo.playerLastRotation = PlayerController.instance.GetComponent<PlayerManager>().playerLastRotation;
        DuloGames.UI.UICharacterDatabase.Instance.SaveCharacterInfo(playerInfo);

        currentCharacterName = "";
        currentCharacterClass = "";
        UIController.instance.LogoutButton.GetComponent<DuloGames.UI.UILoadScene>().Logout();
    }

    public void loadSelectedChar(string charName)
    {
        // INSTANTIATE THE UI AND POPULATE W/E YOU HAVE TO ON THE UI, THEN INSTANTIATE PLAYER AND POPULATE WHATEVER YOU HAVE TO ON THE PLAYER FROM THE UI
        GameObject UI = Instantiate(UIPrefab);
        UI.SetActive(true);

        playerInfo = DuloGames.UI.UICharacterDatabase.Instance.GetByName(charName);

        currentCharacterClass = char.ToUpper(playerInfo.playerClass[0]) + playerInfo.playerClass.Substring(1);
        currentCharacterName = char.ToUpper(playerInfo.playerName[0]) + playerInfo.playerName.Substring(1);
        GameObject player = Instantiate(playerPrefab, playerInfo.playerLastPosition, playerInfo.playerLastRotation);
        player.SetActive(true);
        currentPlayerObject = player;

        setupPlayerUIComponents();
    }

    public void setupPlayerUIComponents()
    {
        currentPlayerObject.GetComponent<DuloGames.UI.Demo_CastManager>().m_CastBar = UIController.instance.playerCastBar;

        spellSlots = new Transform[12]{ UIController.instance.ActionBarSlot1, UIController.instance.ActionBarSlot2, UIController.instance.ActionBarSlot3, UIController.instance.ActionBarSlot4, UIController.instance.ActionBarSlot5,
            UIController.instance.ActionBarSlot6, UIController.instance.ActionBarSlot7, UIController.instance.ActionBarSlot8, UIController.instance.ActionBarSlot9, UIController.instance.ActionBarSlot10, UIController.instance.ActionBarSlot11,
            UIController.instance.ActionBarSlot12 };

        spellSlots.CopyTo(currentPlayerObject.GetComponent<DuloGames.UI.Demo_CastManager>().m_SlotContainers, 0);

        // keybind references are reverse of castbar slots
        keybindSlots = new Transform[12]{ UIController.instance.ActionBarSlot12, UIController.instance.ActionBarSlot11, UIController.instance.ActionBarSlot10, UIController.instance.ActionBarSlot9, UIController.instance.ActionBarSlot8,
            UIController.instance.ActionBarSlot7, UIController.instance.ActionBarSlot6, UIController.instance.ActionBarSlot5, UIController.instance.ActionBarSlot4, UIController.instance.ActionBarSlot3, UIController.instance.ActionBarSlot2,
            UIController.instance.ActionBarSlot1 };

        keybindSlots.CopyTo(currentPlayerObject.GetComponent<DuloGames.UI.KeybindManager>().m_SlotContainers, 0);
    }
}

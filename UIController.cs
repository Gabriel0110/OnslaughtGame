using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController instance; // only one UI in our game, so we want to be able to access that from scripts

    public GameObject CharacterWindow;
    public GameObject InventoryWindow;
    public GameObject SpellBookWindow;
    public GameObject QuestLogWindow;

    public Text playerNameText;

    public GameObject expSlider;
    public Text expText;

    public Text playerLevelText;

    public GameObject healthSlider;
    public GameObject resourceSlider;
    public Text healthText;
    public Text resourceText;

    public GameObject chatWindow;
    public InputField chatInputField;
    public Text chatText;
    private bool chatInputActive;

    public DuloGames.UI.UICastBar targetCastBar;
    public GameObject targetUnitFrame; // to activate target's unitframe on screen
    public GameObject targetHealthSlider;
    public GameObject targetResourceSlider;
    public Text targetName; // on unit frame
    public Text targetLevel; // on unit frame
    public Text targetHealthText; // on unit frame
    public Text targetResourceText; // on unit frame

    public GameObject dragonBorder;

    public GameObject levelNotification;
    public Text levelNotificationText;
    public float levelNotificationStartTime;

    public Text CharWindowHealthText;
    public Text CharWindowResourceText;
    public Text CharWindowStaminaText;
    public Text CharWindowStrengthText;
    public Text CharWindowAgilityText;
    public Text CharWindowIntellectText;
    public Text CharWindowAttackPowerText;
    public Text CharWindowSpellPowerText;
    public Text CharWindowDamageText;
    public Text CharWindowAttackSpeedText;
    public Text CharWindowAverageDPSText;
    public Text CharWindowAttackCritText;
    public Text CharWindowSpellCritText;
    public Text CharWindowArmorText;
    public Text CharWindowArmorMitigationText;
    public Text CharWindowMoveSpeedText;
    public Text CharWindowLevelText;
    public Text CharWindowTotalTalentPointsText;
    public Text CharWindowExperienceText;
    public Text CharWindowGearScoreText;

    public Button LogoutButton;

    public DuloGames.UI.UICastBar playerCastBar;
    public Transform ActionBarSlot1;
    public Transform ActionBarSlot2;
    public Transform ActionBarSlot3;
    public Transform ActionBarSlot4;
    public Transform ActionBarSlot5;
    public Transform ActionBarSlot6;
    public Transform ActionBarSlot7;
    public Transform ActionBarSlot8;
    public Transform ActionBarSlot9;
    public Transform ActionBarSlot10;
    public Transform ActionBarSlot11;
    public Transform ActionBarSlot12;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (levelNotification.active)
        {
            if (Time.time - levelNotificationStartTime >= 4f)
            {
                levelNotification.active = false;
            }
        }

        if (PlayerController.instance.GetComponent<PlayerManager>().hasTarget)
        {
            if (PlayerController.instance.GetComponent<PlayerManager>().playerTarget.GetComponent<NPCManager>().isElite)
            {
                dragonBorder.active = true;
            } else
            {
                dragonBorder.active = false;
            }
        }

        // Always update character screen stats
        CharWindowHealthText.text = PlayerController.instance.GetComponent<PlayerManager>().playerMaxHealth.ToString();
        CharWindowResourceText.text = PlayerController.instance.GetComponent<PlayerManager>().playerMaxResource.ToString();
        CharWindowStaminaText.text = PlayerController.instance.GetComponent<PlayerManager>().playerStamina.ToString();
        CharWindowStrengthText.text = PlayerController.instance.GetComponent<PlayerManager>().playerStrength.ToString();
        CharWindowAgilityText.text = PlayerController.instance.GetComponent<PlayerManager>().playerAgility.ToString();
        CharWindowIntellectText.text = PlayerController.instance.GetComponent<PlayerManager>().playerIntellect.ToString();
        CharWindowAttackPowerText.text = PlayerController.instance.GetComponent<PlayerManager>().playerAttackPower.ToString();
        CharWindowSpellPowerText.text = PlayerController.instance.GetComponent<PlayerManager>().playerSpellPower.ToString();
        CharWindowDamageText.text = Math.Round(PlayerController.instance.GetComponent<PlayerManager>().playerMeleeDamageMin + (PlayerController.instance.GetComponent<PlayerManager>().playerAttackPower * 0.25f)).ToString() + " - " + Math.Round(PlayerController.instance.GetComponent<PlayerManager>().playerMeleeDamageMax + (PlayerController.instance.GetComponent<PlayerManager>().playerAttackPower * 0.25f)).ToString();
        CharWindowAttackSpeedText.text = PlayerController.instance.GetComponent<PlayerManager>().playerUnarmedAttackSpeed.ToString(); // WILL NEED TO UPDATE TO WEAPON ATTACK SPEED
        CharWindowAverageDPSText.text = Math.Round((((PlayerController.instance.GetComponent<PlayerManager>().playerMeleeDamageMin + (PlayerController.instance.GetComponent<PlayerManager>().playerAttackPower * 0.25f)) + (PlayerController.instance.GetComponent<PlayerManager>().playerMeleeDamageMax + (PlayerController.instance.GetComponent<PlayerManager>().playerAttackPower * 0.25f))) / 2) / PlayerController.instance.GetComponent<PlayerManager>().playerUnarmedAttackSpeed).ToString();
        CharWindowAttackCritText.text = (PlayerController.instance.GetComponent<PlayerManager>().playerAttackCrit * 100f).ToString() + "%";
        CharWindowSpellCritText.text = (PlayerController.instance.GetComponent<PlayerManager>().playerSpellCrit * 100f).ToString() + "%";
        CharWindowArmorText.text = PlayerController.instance.GetComponent<PlayerManager>().playerArmor.ToString();
        CharWindowArmorMitigationText.text = (PlayerController.instance.GetComponent<PlayerManager>().armorMitigation * 100f).ToString() + "%";
        CharWindowMoveSpeedText.text = PlayerController.instance.moveSpeed.ToString();
        CharWindowLevelText.text = PlayerController.instance.GetComponent<PlayerManager>().playerCurrentLevel.ToString();
        CharWindowTotalTalentPointsText.text = PlayerController.instance.GetComponent<PlayerManager>().playerTotalTalentPoints.ToString();
        CharWindowExperienceText.text = PlayerController.instance.GetComponent<PlayerManager>().playerCurrentExperience.ToString();
        CharWindowGearScoreText.text = "0";
    }

    public void ToggleCharacterWindow()
    {
        if (CharacterWindow.GetComponent<DuloGames.UI.UIWindow>().IsOpen)
        {
            CharacterWindow.GetComponent<DuloGames.UI.UIWindow>().Hide();
        } else
        {
            //if (SpellBookWindow.GetComponent<DuloGames.UI.UIWindow>().IsOpen)
            //{
            //    SpellBookWindow.GetComponent<DuloGames.UI.UIWindow>().Hide();
            //}
            CharacterWindow.GetComponent<DuloGames.UI.UIWindow>().Show();
        }
    }

    public void ToggleInventoryWindow()
    {
        if (InventoryWindow.GetComponent<DuloGames.UI.UIWindow>().IsOpen)
        {
            InventoryWindow.GetComponent<DuloGames.UI.UIWindow>().Hide();
        }
        else
        {
            InventoryWindow.GetComponent<DuloGames.UI.UIWindow>().Show();
        }
    }

    public void ToggleSpellBookWindow()
    {
        if (SpellBookWindow.GetComponent<DuloGames.UI.UIWindow>().IsOpen)
        {
            SpellBookWindow.GetComponent<DuloGames.UI.UIWindow>().Hide();
        }
        else
        {
            //if (CharacterWindow.GetComponent<DuloGames.UI.UIWindow>().IsOpen)
            //{
            //    CharacterWindow.GetComponent<DuloGames.UI.UIWindow>().Hide();
            //}
            SpellBookWindow.GetComponent<DuloGames.UI.UIWindow>().Show();
        }
    }

    public void ToggleQuestLogWindow()
    {
        if (QuestLogWindow.GetComponent<DuloGames.UI.UIWindow>().IsOpen)
        {
            QuestLogWindow.GetComponent<DuloGames.UI.UIWindow>().Hide();
        }
        else
        {
            QuestLogWindow.GetComponent<DuloGames.UI.UIWindow>().Show();
        }
    }

    public bool isChatActive()
    {
        return chatInputActive;
    }

    public void setChatActive(bool isActive)
    {
        chatInputActive = isActive;
    }
}

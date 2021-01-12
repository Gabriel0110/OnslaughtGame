using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * THIS SCRIPT SHOULD BE USED TO HELP MANAGE CHAT FUNCTIONS ALONGSIDE 'Demo_Chat' SCRIPT.  It is currently attached to the Chat prefab with the 'Demo_Chat' script.
 * Example use case: managing when someone clicks on a players name in order to initiate a whisper, handling checking for when chat commands that are input, etc.
*/

// WE NEED TO EVENTUALLY (FOR MULTIPLAYER) MOVE AWAY FROM ACCESSING THE PLAYER CONTROLLER INSTANCE AND GETTING THE PLAYER COMPONENT VIA THE GAMEOBJECT

public class ChatManager : MonoBehaviour
{
    public static ChatManager instance;

    public GameObject chatWindow;

    public GameObject messageOwner;
    public string messageOwnerName;
    public string lastChatMessage;

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
        // Check for a GM command in chat
        checkForGMCommand();
        if (messageOwner != GameManager.instance.currentPlayerObject)
        {
            messageOwner = GameManager.instance.currentPlayerObject;
            messageOwnerName = GameManager.currentCharacterName;
        }

        //Debug.Log(messageOwner.GetComponent<PlayerManager>().getPlayerName());
        

    }

    public void checkForGMCommand()
    {
        if (lastChatMessage.Length > 0)
        {
            if (lastChatMessage[0] == '.')
            {
                if (DuloGames.UI.UIAccountDatabase.Instance.GetByID(LoginManager.currentAccountID).isGameMaster)
                {
                    if (lastChatMessage.Substring(1, lastChatMessage.Length - 1) == "kill")
                    {
                        if (PlayerController.instance.GetComponent<PlayerManager>().hasTarget)
                        {
                            PlayerController.instance.GetComponent<PlayerManager>().playerTarget.GetComponent<NPCManager>().npcCurrentHealth = 0f;
                            PlayerController.instance.GetComponent<PlayerManager>().playerTarget.GetComponent<NPCManager>().die();
                        }
                    }

                    if (lastChatMessage.Substring(1, lastChatMessage.Length - 1) == "levelup")
                    {
                        PlayerController.instance.GetComponent<PlayerManager>().levelUp();
                    }

                    if (lastChatMessage.Substring(1, 3) == "set")
                    {
                        Debug.Log(lastChatMessage);
                        if (lastChatMessage.Contains("movespeed"))
                        {
                            if (lastChatMessage.Substring(5, 9) == "movespeed")
                            {
                                float speed = float.Parse(lastChatMessage.Substring(15, 3));
                                if (speed <= 2.5f && speed >= 0.1f)
                                {
                                    PlayerController.instance.normalSpeed = speed * 10f;
                                    PlayerController.instance.moveSpeed = speed * 10f;
                                    Debug.Log("Movespeed set to " + (speed * 10f).ToString() + ". Default player movespeed is 6 (0.6).");
                                }
                                else
                                {
                                    Debug.Log("Desired movespeed value must be between 0.1 and 2.5, inclusive.");
                                }
                            }
                        } else if (lastChatMessage.Contains("level") && !lastChatMessage.Contains("levelup"))
                        {
                            if (lastChatMessage.Substring(5, 5) == "level")
                            {
                                int level = Convert.ToInt32(lastChatMessage.Substring(11, lastChatMessage.Length - 11));

                                if (level > PlayerController.instance.GetComponent<PlayerManager>().playerCurrentLevel && level <= PlayerController.instance.GetComponent<PlayerManager>().playerMaxLevel)
                                {
                                    for (int i = 1; i < level; i++)
                                    {
                                        if (PlayerController.instance.GetComponent<PlayerManager>().playerCurrentLevel < PlayerController.instance.GetComponent<PlayerManager>().playerMaxLevel)
                                        {
                                            PlayerController.instance.GetComponent<PlayerManager>().levelUp();
                                        }
                                    }
                                } else if (level < PlayerController.instance.GetComponent<PlayerManager>().playerCurrentLevel && level >= 1)
                                {
                                    for (int i = messageOwner.GetComponent<PlayerManager>().playerCurrentLevel; i > level; i--)
                                    {
                                        if (messageOwner.GetComponent<PlayerManager>().playerCurrentLevel > 1)
                                        {
                                            PlayerController.instance.GetComponent<PlayerManager>().levelDown();
                                        }
                                    }
                                }
                            }
                        } else if (lastChatMessage.Contains("agility") || lastChatMessage.Contains("strength") || lastChatMessage.Contains("stamina") || lastChatMessage.Contains("intellect"))
                        {
                            if (lastChatMessage.Substring(5, 3) == "agi")
                            {
                                int agility = Convert.ToInt32(lastChatMessage.Substring(13, lastChatMessage.Length - 13));
                                PlayerController.instance.GetComponent<PlayerManager>().playerAgility = agility;
                                PlayerController.instance.GetComponent<PlayerManager>().updateStats();
                            } else if (lastChatMessage.Substring(5, 3) == "str")
                            {
                                int strength = Convert.ToInt32(lastChatMessage.Substring(14, lastChatMessage.Length - 14));
                                PlayerController.instance.GetComponent<PlayerManager>().playerStrength = strength;
                                PlayerController.instance.GetComponent<PlayerManager>().updateStats();
                            }
                        }
                    }

                    if (lastChatMessage.Substring(1, lastChatMessage.Length - 1) == "revive")
                    {
                        PlayerController.instance.GetComponent<PlayerManager>().resurrect();
                    }

                    if (lastChatMessage.Substring(1, lastChatMessage.Length - 1) == "suicide")
                    {
                        PlayerController.instance.GetComponent<PlayerManager>().die();
                    }

                    if (lastChatMessage.Substring(1, lastChatMessage.Length - 1) == "gm")
                    {
                        if (lastChatMessage == "gm on")
                        {
                            PlayerController.instance.GetComponent<PlayerManager>().isGameMaster = true;
                        } else if (lastChatMessage == "gm off")
                        {
                            PlayerController.instance.GetComponent<PlayerManager>().isGameMaster = false;
                        }
                    }
                }
            }
        }

        lastChatMessage = "";
    }
}

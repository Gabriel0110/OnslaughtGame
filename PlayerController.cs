using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance; // singleton instance of PlayerController to more easily have enemies chase the player
    public CharacterController charCon;

    private Vector3 moveInput;

    public float moveSpeed, gravityModifier, jumpPower, stealthMoveSpeed;
    public float normalSpeed;

    private bool isAutorunning = false;

    private bool canJump;
    public Transform groundCheckPoint;
    public LayerMask whatIsWalkable;

    public Animator anim; // to access the animator booleans to control animations

    private bool isCasting;

    public static Transform target;

    private float rightClickStart;
    private float leftClickStart;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        normalSpeed = moveSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (DuloGames.UI.Demo_CastManager.instance.m_CastBar != null)
        {
            if (DuloGames.UI.Demo_CastManager.instance.m_CastBar.IsCasting)
            {
                isCasting = true;
            }
            else
            {
                isCasting = false;
            }
        }
        

        if (PlayerController.instance.GetComponent<PlayerManager>().isPlayerStealth())
        {
            moveSpeed = stealthMoveSpeed;

        } else
        {
            moveSpeed = normalSpeed;
        }

        // Store y velocity at start of frame
        float yStore = moveInput.y;

        if (UIController.instance.isChatActive() == false || (isAutorunning == true && UIController.instance.isChatActive() == true))
        {
            Vector3 vertMove;
            Vector3 horiMove;
            if (isAutorunning == false)
            {
                vertMove = transform.forward * Input.GetAxisRaw("Vertical");
                horiMove = transform.right * Input.GetAxisRaw("Horizontal");
                moveInput = horiMove + vertMove;
            } else
            {
                vertMove = transform.forward;
                moveInput = vertMove;
            }
            
            moveInput.Normalize(); // fixes the super speed when moving diagonally

            moveInput *= moveSpeed;

            moveInput.y = yStore;
        }

        // Implement gravity for falling
        moveInput.y += Physics.gravity.y * gravityModifier * Time.deltaTime;

        if (charCon.isGrounded)
        {
            moveInput.y = Physics.gravity.y * gravityModifier * Time.deltaTime;
        }



        // MOVEMENT & JUMPING -- check if player can move first (in PlayerManager)
        if (PlayerManager.instance.canMove && !PlayerManager.instance.isPlayerStunned())
        {
            // Handle Jumping
            canJump = Physics.OverlapSphere(groundCheckPoint.position, .15f, whatIsWalkable).Length > 0;

            if (anim.GetBool("Jumping") == true)
            {
                if (charCon.isGrounded)
                {
                    anim.SetBool("Jumping", false);
                }
            }

            if (Input.GetKeyDown(KeyCode.Space) && canJump && UIController.instance.isChatActive() == false)
            {
                if (isCasting)
                {
                    DuloGames.UI.Demo_CastManager.instance.m_CastBar.Interrupt();
                }
                moveInput.y = jumpPower;
                anim.SetBool("Jumping", true);
            }

            // Move player using moveInput from above
            if (isAutorunning == true)
            {
                charCon.Move(moveInput * Time.deltaTime);
                anim.SetBool("MovingForward", true);
            }
            else
            {
                if (UIController.instance.isChatActive() == false || (UIController.instance.isChatActive() == true && anim.GetBool("Jumping") == true))
                {
                    charCon.Move(moveInput * Time.deltaTime);
                }
            }


            // Handle animation activations
            if ((Input.GetKeyDown(KeyCode.W) && UIController.instance.isChatActive() == false))
            {
                if (isCasting)
                {
                    DuloGames.UI.Demo_CastManager.instance.m_CastBar.Interrupt();
                }
                isAutorunning = false;
                anim.SetBool("MovingForward", true);

            }
            else if (Input.GetKeyUp(KeyCode.W) && UIController.instance.isChatActive() == false)
            {
                anim.SetBool("MovingForward", false);
            }

            if (Input.GetKeyDown(KeyCode.S) && UIController.instance.isChatActive() == false)
            {
                if (isCasting)
                {
                    DuloGames.UI.Demo_CastManager.instance.m_CastBar.Interrupt();
                }

                if (isAutorunning == true)
                {
                    isAutorunning = false;
                    anim.SetBool("MovingForward", false);
                }
                anim.SetBool("MovingBackward", true);
            }
            else if (Input.GetKeyUp(KeyCode.S))
            {
                anim.SetBool("MovingBackward", false);
            }

            if (Input.GetKeyDown(KeyCode.A) && UIController.instance.isChatActive() == false)
            {
                if (isCasting)
                {
                    DuloGames.UI.Demo_CastManager.instance.m_CastBar.Interrupt();
                }

                if (isAutorunning == true)
                {
                    isAutorunning = false;
                    anim.SetBool("MovingForward", false);
                }
                anim.SetBool("StrafingLeft", true);
            }
            else if (Input.GetKeyUp(KeyCode.A))
            {
                anim.SetBool("StrafingLeft", false);
            }

            if (Input.GetKeyDown(KeyCode.D) && UIController.instance.isChatActive() == false)
            {
                if (isCasting)
                {
                    DuloGames.UI.Demo_CastManager.instance.m_CastBar.Interrupt();
                }

                if (isAutorunning == true)
                {
                    isAutorunning = false;
                    anim.SetBool("MovingForward", false);
                }
                anim.SetBool("StrafingRight", true);
            }
            else if (Input.GetKeyUp(KeyCode.D))
            {
                anim.SetBool("StrafingRight", false);
            }

            // If chat is active, stop all movement animations UNLESS AUTORUNNING (not implemented yet)
            if (UIController.instance.isChatActive() == true)
            {
                if (isAutorunning == false)
                {
                    anim.SetBool("MovingForward", false);
                    anim.SetBool("MovingBackward", false);
                    anim.SetBool("StrafingLeft", false);
                    anim.SetBool("StrafingRight", false);
                }

            }
        }
        


        // Check for mouse clicks on NPCs and handle TARGETING  (DOES NOT HANDLE CLICKING ON PLAYERS)
        if (Input.GetMouseButtonDown(1))
        {
            rightClickStart = Time.time;
        } else if (Input.GetMouseButtonDown(0))
        {
            leftClickStart = Time.time;
        }

        if (Input.GetMouseButtonUp(0))
        {
            handleTargeting("left");

        } else if (Input.GetMouseButtonUp(1))
        {
            handleTargeting("right");
        }



        // Handle in-game Window keybinds
        if (Input.GetKeyDown(KeyCode.C) && UIController.instance.isChatActive() == false)
        {
            UIController.instance.ToggleCharacterWindow();
        }

        if (Input.GetKeyDown(KeyCode.B) && UIController.instance.isChatActive() == false)
        {
            UIController.instance.ToggleInventoryWindow();
        }

        if (Input.GetKeyDown(KeyCode.P) && UIController.instance.isChatActive() == false)
        {
            UIController.instance.ToggleSpellBookWindow();
        }

        if (Input.GetKeyDown(KeyCode.L) && UIController.instance.isChatActive() == false)
        {
            UIController.instance.ToggleQuestLogWindow();
        }

        
        // Press enter to activate chat input
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (UIController.instance.isChatActive() == false)
            {
                UIController.instance.chatInputField.ActivateInputField();
                UIController.instance.setChatActive(true);
            } else
            {
                UIController.instance.setChatActive(false);
            }
            
        }


        // Press num-lock to auto-run
        if (Input.GetKeyDown(KeyCode.Numlock) && isAutorunning == false)
        {
            isAutorunning = true;
        } else if (Input.GetKeyDown(KeyCode.Numlock) && isAutorunning == true)
        {
            isAutorunning = false;
            anim.SetBool("MovingForward", false);
        }
        
    }

    public void handleTargeting(string mouseButton)
    {
        RaycastHit hit;

        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
        {
            if (hit.transform.tag == "NonHostileNPC" || hit.transform.tag == "HostileNPC")
            {
                Debug.Log("NPC CLICKED");
                if (mouseButton == "left" || (mouseButton == "right" && ((Time.time - rightClickStart) < 0.2)))
                {
                    if (target == null)
                    {
                        target = hit.transform;
                        target.GetComponent<NPCManager>().isTargeted = true;
                    }
                    else
                    {
                        target.GetComponent<NPCManager>().npcHighlight.active = false;
                        target.GetComponent<NPCManager>().isTargeted = false;
                        target = hit.transform;
                        target.GetComponent<NPCManager>().isTargeted = true;
                    }
                }

                if (hit.transform.GetComponent<NPCManager>().npcHighlight.active == false)
                {
                    if (mouseButton == "left" || (mouseButton == "right" && ((Time.time - rightClickStart) < 0.2)))
                    {
                        this.GetComponent<PlayerManager>().playerTarget = target.gameObject;
                        UIController.instance.targetUnitFrame.active = true;
                        UIController.instance.targetName.text = target.GetComponent<NPCManager>().npcName;

                        if (target.GetComponent<NPCManager>().npcLevel - PlayerManager.instance.getPlayerLevel() >= 10) // if target is 10 or more levels higher, target lvl will be '?'
                        {
                            UIController.instance.targetLevel.text = "??";
                        } else
                        {
                            UIController.instance.targetLevel.text = target.GetComponent<NPCManager>().npcLevel.ToString();
                        }
                        
                        target.GetComponent<NPCManager>().npcHighlight.active = true;

                        if (mouseButton == "right" && target.GetComponent<NPCManager>().isAttackable)
                        {
                            this.GetComponent<PlayerManager>().setAttackReady(true);
                        } else
                        {
                            this.GetComponent<PlayerManager>().setAttackReady(false);
                        }
                    }
                    
                }
            }
            else if (hit.transform.tag != "NonHostileNPC" && hit.transform.tag != "HostileNPC")
            {
                Debug.Log("NO NPC CLICKED");
                if (target != null && mouseButton == "left" && ((Time.time - leftClickStart) < 0.2))
                {
                    if (UIController.instance.GetComponent<CheckClicks>().getRaycastResults().Count == 0)
                    {
                        this.GetComponent<PlayerManager>().setAttackReady(false);
                        this.GetComponent<PlayerManager>().playerTarget = null;
                        UIController.instance.targetUnitFrame.active = false;
                        target.GetComponent<NPCManager>().npcHighlight.active = false;
                        target.GetComponent<NPCManager>().isTargeted = false;
                        target = null;
                    }
                }
            }
        }
        else
        {
            Debug.Log("NO COLLIDER CLICKED");
            if (target != null && mouseButton == "left" && ((Time.time - leftClickStart) < 0.2))
            {
                if (UIController.instance.GetComponent<CheckClicks>().getRaycastResults().Count == 0)
                {
                    this.GetComponent<PlayerManager>().setAttackReady(false);
                    this.GetComponent<PlayerManager>().playerTarget = null;
                    UIController.instance.targetUnitFrame.active = false;
                    target.GetComponent<NPCManager>().npcHighlight.active = false;
                    target.GetComponent<NPCManager>().isTargeted = false;
                    target = null;
                }
            }
        }
    }

    public bool canCast()
    {
        // TURNED OFF CHECK FOR GROUNDING -- IT"S CAUSING DELAYS IN REGISTERING CAST

        if (isAutorunning)
        {
            return false;
        }

        if ((anim.GetBool("Jumping") == true) || (anim.GetBool("MovingForward") == true) || (anim.GetBool("MovingBackward") == true) || (anim.GetBool("StrafingLeft") == true) || (anim.GetBool("StrafingRight") == true))
        {
            return false;
        }

        return true;
    }
}

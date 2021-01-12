using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DuloGames.UI
{
    public class KeybindManager : MonoBehaviour
    {
        #pragma warning disable 0649
        [SerializeField] public Transform[] m_SlotContainers;
        #pragma warning restore 0649

        private bool MainSpellSlot0;
        private bool MainSpellSlot1;
        private bool MainSpellSlot2;
        private bool MainSpellSlot3;
        private bool MainSpellSlot4;
        private bool MainSpellSlot5;
        private bool MainSpellSlot6;
        private bool MainSpellSlot7;
        private bool MainSpellSlot8;
        private bool MainSpellSlot9;
        private bool MainSpellSlot10;
        private bool MainSpellSlot11;
        private UISpellSlot[] mainSpellSlots;

        private void Awake()
        {
            //mainSpellSlots = Demo_CastManager.instance.getSpellSlots();
        }

        // Start is called before the first frame update
        void Start()
        {
            if (this.m_SlotContainers != null && this.m_SlotContainers.Length > 0)
            {
                int i = 0;
                foreach (Transform t in this.m_SlotContainers)
                {
                    //mainSpellSlots = t.GetComponentsInChildren<UISpellSlot>();
                    //mainSpellSlots.SetValue(t, i);
                    //mainSpellSlots[i] = t.GetComponentsInChildren<UISpellSlot>();
                    i++;
                }
            
            }

            //Debug.Log(mainSpellSlots.Length);

            MainSpellSlot0 = Input.GetKeyDown(KeyCode.E);
            MainSpellSlot1 = Input.GetKeyDown(KeyCode.R);
            MainSpellSlot2 = Input.GetKeyDown(KeyCode.F);
            MainSpellSlot3 = Input.GetKeyDown(KeyCode.C);
            MainSpellSlot4 = Input.GetKeyDown(KeyCode.X);
            MainSpellSlot5 = Input.GetKeyDown(KeyCode.Alpha2);
            MainSpellSlot6 = Input.GetKeyDown(KeyCode.Alpha3);
            MainSpellSlot7 = Input.GetKeyDown(KeyCode.Alpha4);
            MainSpellSlot8 = (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.E));
            MainSpellSlot9 = (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.R));
            MainSpellSlot10 = (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.F));
            MainSpellSlot11 = Input.GetKeyDown(KeyCode.Mouse2);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.E) && UIController.instance.chatInputField.isFocused == false)
            {
                Demo_CastManager.instance.CastKeyboundSpell(this.m_SlotContainers[11].GetComponentsInChildren<UISpellSlot>()[0]);
            }
            if (Input.GetKeyDown(KeyCode.R) && UIController.instance.chatInputField.isFocused == false)
            {
                Demo_CastManager.instance.CastKeyboundSpell(this.m_SlotContainers[10].GetComponentsInChildren<UISpellSlot>()[0]);
            }
            if (Input.GetKeyDown(KeyCode.F) && UIController.instance.chatInputField.isFocused == false)
            {
                Demo_CastManager.instance.CastKeyboundSpell(this.m_SlotContainers[9].GetComponentsInChildren<UISpellSlot>()[0]);
            }
            if (Input.GetKeyDown(KeyCode.C) && UIController.instance.chatInputField.isFocused == false)
            {
                Demo_CastManager.instance.CastKeyboundSpell(this.m_SlotContainers[8].GetComponentsInChildren<UISpellSlot>()[0]);
            }
            if (Input.GetKeyDown(KeyCode.X) && UIController.instance.chatInputField.isFocused == false)
            {
                Demo_CastManager.instance.CastKeyboundSpell(this.m_SlotContainers[7].GetComponentsInChildren<UISpellSlot>()[0]);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2) && UIController.instance.chatInputField.isFocused == false)
            {
                Demo_CastManager.instance.CastKeyboundSpell(this.m_SlotContainers[6].GetComponentsInChildren<UISpellSlot>()[0]);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3) && UIController.instance.chatInputField.isFocused == false)
            {
                Demo_CastManager.instance.CastKeyboundSpell(this.m_SlotContainers[5].GetComponentsInChildren<UISpellSlot>()[0]);
            }
            if (Input.GetKeyDown(KeyCode.Alpha4) && UIController.instance.chatInputField.isFocused == false)
            {
                Demo_CastManager.instance.CastKeyboundSpell(this.m_SlotContainers[4].GetComponentsInChildren<UISpellSlot>()[0]);
            }
            if ((Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.E)) && UIController.instance.chatInputField.isFocused == false)
            {
                Demo_CastManager.instance.CastKeyboundSpell(this.m_SlotContainers[3].GetComponentsInChildren<UISpellSlot>()[0]);
            }
            if ((Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.R)) && UIController.instance.chatInputField.isFocused == false)
            {
                Demo_CastManager.instance.CastKeyboundSpell(this.m_SlotContainers[2].GetComponentsInChildren<UISpellSlot>()[0]);
            }
            if ((Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.F)) && UIController.instance.chatInputField.isFocused == false)
            {
                Demo_CastManager.instance.CastKeyboundSpell(this.m_SlotContainers[1].GetComponentsInChildren<UISpellSlot>()[0]);
            }
            if (Input.GetKeyDown(KeyCode.Mouse2) && UIController.instance.chatInputField.isFocused == false)
            {
                Demo_CastManager.instance.CastKeyboundSpell(this.m_SlotContainers[0].GetComponentsInChildren<UISpellSlot>()[0]);
            }
        }

    }
}

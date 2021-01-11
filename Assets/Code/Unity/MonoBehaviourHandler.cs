using System;
using JoyLib.Code.Entities;
using JoyLib.Code.Graphics;
using JoyLib.Code.Helpers;
using JoyLib.Code.Unity.GUI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using ContextMenu = JoyLib.Code.Unity.GUI.ContextMenu;

namespace JoyLib.Code.Unity
{
    public class MonoBehaviourHandler : ManagedSprite, IPointerEnterHandler, IPointerExitHandler
    {
        public IJoyObject JoyObject { get; protected set; }
        protected ManagedSprite SpeechBubble { get; set; }
        protected bool PointerOver { get; set; }
        
        protected static IGUIManager GUIManager { get; set; }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (this.JoyObject == null)
            {
                return;
            }
            
            this.JoyObject.Update();
            this.transform.position = new Vector3(this.JoyObject.WorldPosition.x, this.JoyObject.WorldPosition.y);
        }

        public void Start()
        {
            InputSystem.onActionChange -= this.HandleInput;
            InputSystem.onActionChange += this.HandleInput;
        }

        public override void SetSpriteLayer(string layerName)
        {
            base.SetSpriteLayer(layerName);
            if (this.SpeechBubble is null == false)
            {
                this.SpeechBubble.SetSpriteLayer(layerName);
            }
        }

        public virtual void AttachJoyObject(IJoyObject joyObject)
        {
            if (GUIManager is null)
            {
                GUIManager = GlobalConstants.GameManager.GUIManager;
            }
            
            this.JoyObject = joyObject;
            this.JoyObject.AttachMonoBehaviourHandler(this);
            Transform transform = this.transform.Find("Speech Bubble");
            if (transform is null == false)
            {
                this.SpeechBubble = transform.GetComponent<ManagedSprite>();
            }
            this.name = this.JoyObject.JoyName + ":" + this.JoyObject.GUID;
            this.transform.position = new Vector3(this.JoyObject.WorldPosition.x, this.JoyObject.WorldPosition.y, 0.0f);
            this.SetSpeechBubble(false);
        }

        public void SetSpeechBubble(bool on, ISpriteState need = null)
        {
            if (this.SpeechBubble is null)
            {
                return;
            }
            
            this.SpeechBubble.gameObject.SetActive(on);
            if (on)
            {
                this.SpeechBubble.Clear();
                this.SpeechBubble.AddSpriteState(need, true);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            this.PointerOver = true;
            if (GUIManager.IsActive(GUINames.CONTEXT_MENU) == false
                && GlobalConstants.GameManager.Player.VisionProvider.CanSee(
                    GlobalConstants.GameManager.Player,
                    this.JoyObject.MyWorld,
                    this.JoyObject.WorldPosition))
            {
                GUIManager.OpenGUI(GUINames.TOOLTIP).GetComponent<Tooltip>().Show(
                    this.JoyObject.JoyName,
                    null,
                    this.CurrentSpriteState,
                    this.JoyObject.Tooltip);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            this.PointerOver = false;
            GUIManager.CloseGUI(GUINames.TOOLTIP);
        }

        protected virtual void OpenContextMenu()
        {
            ContextMenu contextMenu = GUIManager.GetGUI(GUINames.CONTEXT_MENU).GetComponent<ContextMenu>();
            contextMenu.Clear();

            if (this.JoyObject.Equals(GlobalConstants.GameManager.Player) == false
                && GlobalConstants.GameManager.Player.VisionProvider.CanSee(
                    GlobalConstants.GameManager.Player,
                    this.JoyObject.MyWorld,
                    this.JoyObject.WorldPosition)
                && this.JoyObject is IEntity)
            {
                if (AdjacencyHelper.IsAdjacent(
                    this.JoyObject.WorldPosition, 
                    GlobalConstants.GameManager.Player.WorldPosition))
                {
                    contextMenu.AddMenuItem("Talk", this.TalkToPlayer);
                }
                else
                {
                    contextMenu.AddMenuItem("Call Over", this.CallOver);
                }
            }
            else
            {
                contextMenu.AddMenuItem("Open Inventory", this.OpenInventory);
            }

            if (contextMenu.GetComponentsInChildren<MenuItem>().Length > 0)
            {
                GUIManager.OpenGUI(GUINames.CONTEXT_MENU);
                contextMenu.Show();
            }
        }

        protected virtual void HandleInput(object data, InputActionChange change)
        {
            if (this.PointerOver == false)
            {
                return;
            }
        
            if (change != InputActionChange.ActionPerformed)
            {
                return;
            }

            if (!(data is InputAction action))
            {
                return;
            }
            
            if (action.name.Equals("open context menu", StringComparison.OrdinalIgnoreCase))
            {
                this.OpenContextMenu();
            }
        }

        protected void OpenInventory()
        {
            GUIManager.OpenGUI(GUINames.INVENTORY);
        }

        protected void TalkToPlayer()
        {
            GUIManager.CloseGUI(GUINames.CONTEXT_MENU);
            GUIManager.OpenGUI(GUINames.CONVERSATION);
            GlobalConstants.GameManager.ConversationEngine.SetActors(
                GlobalConstants.GameManager.Player, 
                this.JoyObject as IEntity);
            
            GlobalConstants.GameManager.ConversationEngine.Converse();
        }

        protected void CallOver()
        {
            GUIManager.CloseGUI(GUINames.CONTEXT_MENU);
            this.JoyObject.FetchAction("seekaction")
                .Execute(
                    new IJoyObject[] {this.JoyObject, GlobalConstants.GameManager.Player},
                    new[] {"call over"},
                    new object[] {"friendship"});
        }
    }
}

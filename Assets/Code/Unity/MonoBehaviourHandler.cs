using System;
using JoyLib.Code.Entities;
using JoyLib.Code.Helpers;
using JoyLib.Code.Unity.GUI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using ContextMenu = JoyLib.Code.Unity.GUI.ContextMenu;

namespace JoyLib.Code.Unity
{
    public class MonoBehaviourHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public IJoyObject JoyObject { get; protected set; }
        protected SpriteRenderer SpriteRenderer { get; set; }
        protected SpriteRenderer SpeechBubble { get; set; }
        protected bool PointerOver { get; set; }
        
        protected static IGUIManager GUIManager { get; set; }

        public void Update()
        {
            if (this.JoyObject == null)
            {
                return;
            }
            
            this.JoyObject.Update();
            this.SpriteRenderer.sprite = this.JoyObject.Sprite;
            this.transform.position = new Vector3(this.JoyObject.WorldPosition.x, this.JoyObject.WorldPosition.y);
        }

        public void Start()
        {
            InputSystem.onActionChange -= this.HandleInput;
            InputSystem.onActionChange += this.HandleInput;
        }

        public virtual void AttachJoyObject(IJoyObject joyObject)
        {
            if (GUIManager is null)
            {
                GUIManager = GlobalConstants.GameManager.GUIManager;
            }
            
            this.JoyObject = joyObject;
            this.JoyObject.AttachMonoBehaviourHandler(this);
            this.SpriteRenderer = this.GetComponent<SpriteRenderer>();
            Transform transform = this.transform.Find("Speech Bubble");
            if (transform is null == false)
            {
                this.SpeechBubble = transform.GetComponent<SpriteRenderer>();
                this.SpeechBubble.gameObject.SetActive(false);
            }

            /*
            if(this.JoyObject.JoyName.StartsWith("Downstairs") || this.JoyObject.JoyName.StartsWith("Upstairs"))
            {
                this.SpriteRenderer.sortingLayerName = "Walls";
            }
            else if (this.JoyObject.GetType() == typeof(JoyObject))
            {
                if(this.JoyObject.IsWall)
                {
                    this.SpriteRenderer.sortingLayerName = "Walls";
                }
                else
                {
                    this.SpriteRenderer.sortingLayerName = "Terrain";
                }
            }
            else
            {
                if(this.JoyObject is ItemInstance)
                {
                    this.SpriteRenderer.sortingLayerName = "Objects";
                }
                else
                {
                    this.SpriteRenderer.sortingLayerName = "Entities";
                }
            }
            */
            this.name = this.JoyObject.JoyName + ":" + this.JoyObject.GUID;
            this.transform.position = new Vector3(this.JoyObject.WorldPosition.x, this.JoyObject.WorldPosition.y, 0.0f);
            this.SpriteRenderer.sprite = joyObject.Sprite;
            this.SetSpeechBubble(false);
        }

        public void SetSpeechBubble(bool on, Sprite need = null)
        {
            this.SpeechBubble.gameObject.SetActive(on);
            if (on)
            {
                this.SpeechBubble.sprite = need;
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            this.PointerOver = true;
            if (GUIManager.IsActive(GUINames.CONTEXT_MENU) == false
                /*&& GlobalConstants.GameManager.Player.VisionProvider.CanSee(
                    GlobalConstants.GameManager.Player,
                    this.JoyObject.MyWorld,
                    this.JoyObject.WorldPosition)*/)
            {
                GUIManager.OpenGUI(GUINames.TOOLTIP).GetComponent<Tooltip>().Show(
                    this.JoyObject.JoyName,
                    null,
                    this.JoyObject.Sprite,
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

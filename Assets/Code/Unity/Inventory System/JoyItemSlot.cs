using JoyLib.Code.Conversation;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Items;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace JoyLib.Code.Unity.GUI
{
    public class JoyItemSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerDownHandler,
        IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] protected KeyCode m_UseKey;
        
        [SerializeField]
        protected Image m_CooldownOverlay;
        
        [SerializeField]
        protected Text m_Cooldown;
        
        [SerializeField]
        protected Text m_ItemName;
        
        [SerializeField]
        protected bool m_UseRarityColor = false;
        
        [SerializeField]
        protected Image m_Icon;
        
        [SerializeField]
        protected Text m_Stack;

        public string m_Slot;
        
        public ItemContainer Container { get; set; }

        protected IItemInstance m_Item;

        public IItemInstance Item
        {
            get
            {
                return this.m_Item;
            }
            set
            {
                this.m_Item = value;
                this.Repaint();
            }
        }

        public bool IsEmpty => this.Item is null;

        [SerializeField] protected string m_Name;

        public string Name
        {
            get => this.m_Name;
            protected set => this.m_Name = value;
        }


        /// <summary>
        /// Gets a value indicating whether this slot is in cooldown.
        /// </summary>
        /// <value><c>true</c> if this slot is in cooldown; otherwise, <c>false</c>.</value>
        public bool IsCooldown
        {
            get;
        }
        
        
        public static IConversationEngine ConversationEngine { get; set; }

        public static IGUIManager GUIManager { get; set; }

        public static ILiveEntityHandler EntityHandler { get; set; }

        protected static IEntity Player { get; set; }

        public void Awake()
        {
            this.GetBits();
        }

        protected void GetBits()
        {
            if (GlobalConstants.GameManager is null || GUIManager is null == false)
            {
                return;
            }

            ConversationEngine = GlobalConstants.GameManager.ConversationEngine;
            GUIManager = GlobalConstants.GameManager.GUIManager;
            EntityHandler = GlobalConstants.GameManager.EntityHandler;
        }
        
        public virtual void Repaint()
        {
            if (this.m_ItemName != null)
            {
                this.m_ItemName.text = this.IsEmpty ? string.Empty : this.Item.JoyName;
            }

            if (this.m_Icon is null == false)
            {
                if (!this.IsEmpty)
                {
                    this.m_Icon.overrideSprite = this.Item.Sprite;
                    this.m_Icon.enabled = true;
                }
                else 
                {
                    this.m_Icon.enabled = false;
                }
            }

            //TODO: Add item stacking
            /*
            if (this.m_Stack != null) 
            {
                if (!IsEmpty && ObservedItem.MaxStack > 1 )
                {
                    //Updates the stack and enables it.
                    this.m_Stack.text = ObservedItem.Stack.ToString();
                    this.m_Stack.enabled = true;
                }
                else
                {
                    //If there is no item in this slot, disable stack field
                    this.m_Stack.enabled = false;
                }
            }
            */
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.dragging)
            {
                return;
            }

            ContextMenu menu = GUIManager.GetGUI(GUINames.CONTEXT_MENU).GetComponent<ContextMenu>();
            if (menu is null || this.Container.UseContextMenu == false)
            {
                return;
            }

            this.GetBits();

            if (this.Item is null == false)
            {
                if (this.Item.HasTag("container"))
                {
                    menu.AddMenuItem("Open", this.OpenContainer);
                    menu.Show();
                }
            }
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            this.ShowTooltip();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            this.CloseTooltip();
        }
        
        protected virtual void ShowTooltip() 
        {
            if (this.Container.ShowTooltips && this.Item is null == false)
            {
                GUIManager.GetGUI(GUINames.TOOLTIP)
                    .GetComponent<Tooltip>()
                    .Show(
                        this.Item.JoyName,
                        this.Item.DisplayDescription,
                        this.Item.Sprite);
            }
        }

        protected virtual void CloseTooltip() 
        {
            if (this.Container.ShowTooltips)
            {
                GUIManager.CloseGUI(GUINames.TOOLTIP);
            }
        }

        protected void OpenContainer()
        {
            if (this.Item.HasTag("container"))
            {
                GUIManager?.OpenGUI(GUINames.INVENTORY_CONTAINER);
                ItemContainer container = GUIManager?.GetGUI(GUINames.INVENTORY_CONTAINER)
                    .GetComponent<ItemContainer>();
                container.Owner = this.Item;
                container.RemoveAllItems();
                foreach (IItemInstance content in this.Item.Contents)
                {
                    container.StackOrAdd(content);
                }
            }
        }

        protected void DropItem()
        {
            this.GetBits();

            //Check if the item is droppable
            if (this.Item is null == false)
            {
                if (this.Item.MonoBehaviourHandler is ItemBehaviourHandler itemBehaviourHandler)
                {
                    itemBehaviourHandler.DropItem(this.Item);
                }

                this.Container.RemoveItem(this.Item);
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            //Check if we can start dragging
            if (this.Item is null == false && this.Container.CanDrag)
            {
                //If key for unstacking items is pressed and if the stack is greater then 1, show the unstack ui.
                if (InventoryManager.Input.unstackEvent.HasFlag<Configuration.Input.UnstackInput>(Configuration.Input.UnstackInput.OnDrag) && Input.GetKey(InventoryManager.Input.unstackKeyCode) && ObservedItem.Stack > 1){
                    Unstack();
                }else{
                    //Set the dragging slot
                    // draggedSlot = this;
                    Debug.Log(eventData.pointerCurrentRaycast.gameObject);
                    if(base.m_Ícon == null || !base.m_Ícon.raycastTarget || eventData.pointerCurrentRaycast.gameObject == base.m_Ícon.gameObject)
                        dragObject = new DragObject(this);
    
                }
            }
            if (this.m_ParentScrollRect != null && dragObject == null)
            {
                this.m_ParentScrollRect.OnBeginDrag(eventData);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            throw new System.NotImplementedException();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            throw new System.NotImplementedException();
        }

        public void OnDrop(PointerEventData eventData)
        {
            throw new System.NotImplementedException();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            throw new System.NotImplementedException();
        }
    }
}
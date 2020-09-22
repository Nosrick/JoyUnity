using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DevionGames.UIWidgets;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

namespace DevionGames.InventorySystem
{
    [DisallowMultipleComponent]
    public class Trigger : CallbackHandler, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
	{
        public enum FailureCause {
            Unknown,
            FurtherAction, // Requires a user action(Select an item for crafting)
            NotEnoughCurrency, // Player does not have enough money 
            Remove, // No longer exists (Item was removed)
            ContainerFull, // Given container is full
            InUse, // Something is already in use (Player is already crafting something-> no start possible)
            Requirement // Missing requirements for this action( Missing ingredient to craft)
        }


        public override string[] Callbacks
        {
            get
            {
                return new[] {
                    "OnPointerEnter",
                    "OnPointerExit",
                    "OnTriggerUsed",
                    "OnTriggerUnUsed",
                    "OnCameInRange",
                    "OnWentOutOfRange",
                };
            }
        }

        //The maximum distance for trigger useage
        public float useDistance = 1.2f;
        [EnumFlags]
        public TriggerInputType triggerType = TriggerInputType.OnClick | TriggerInputType.Key;
        //If in range and trigger input type includes key, the key to use the trigger.
        public KeyCode key = KeyCode.F;
        //Use a custom cursor if pointer is over trigger
        public bool useCustomCursor;
        //Cursor image to show
        [SerializeField]
        protected Sprite m_CursorSprite;
        //Size of the imgae
        [SerializeField]
        protected Vector2 m_CursorSize = new Vector2(32f, 32f);
        //Cursor animation, leave empty for none, 
        [SerializeField]
        protected string m_CursorAnimatorState="Cursor";
        //Lock the container, so player can't move items
        [SerializeField]
        protected bool m_LockContainersWhenUsed = false;

        //Actions to run when the trigger is used.
        [HideInInspector]
        public List<TriggerAction> actions = new List<TriggerAction>();
        //Is the player in range, set by OnTriggerEnter/OnTriggerExit?
        private bool m_InRange;
        public bool InRange {
            get {
                return this.m_InRange;
            }
            protected set {
                if (this.m_InRange != value)
                {
                    this.m_InRange = value;
                    if (this.m_InRange)
                    {
                        ExecuteEvent<ITriggerCameInRange>(Execute, true);
                        Trigger.m_TriggerInRange.Add(this);
                        //InputTriggerType.OnTriggerEnter is supported
                        if (triggerType.HasFlag<TriggerInputType>(TriggerInputType.OnTriggerEnter) && IsBestTrigger())
                        {
                            Use();
                        }
                    }else {
                        ExecuteEvent<ITriggerWentOutOfRange>(Execute, true);
                        Trigger.m_TriggerInRange.Remove(this);
                        this.InUse = false;

                    }
                }
            }
        }
        //Is the trigger currently in use?
        private bool m_InUse;
        public bool InUse {
            get { return this.m_InUse; }
            protected set {
                if (this.m_InUse != value) {
                    this.m_InUse = value;
                    if (!this.m_InUse) {
                        this.m_ActionBehavior.Stop();
                        ExecuteEvent<ITriggerUnUsedHandler>(Execute,true);
                        Trigger.currentUsedTrigger = null;
                        LockAllItemContainer(false);
                        LoadCachedAnimatorStates();
                    }
                    else {
                        Trigger.currentUsedTrigger = this;
                        LockAllItemContainer(true);
                        ExecuteEvent<ITriggerUsedHandler>(Execute);
                        CacheAnimatorStates();
                    }
                   
                }
            }
        }

        //Task behavior that runs custom actions
        private Sequence m_ActionBehavior;
        //Does the user quits application
        private bool m_ApplicationQuit;
        //Custom Trigger callbacks
        protected ITriggerEventHandler[] m_TriggerEvents;
        //Current trigger used by the player
        public static Trigger currentUsedTrigger;
        public static ItemContainer currentUsedWindow;

        //All triggers in range
        private static List<Trigger> m_TriggerInRange = new List<Trigger>();

        protected Dictionary<Type, string> m_CallbackHandlers;

        protected delegate void EventFunction<T>(T handler, GameObject player);
        protected delegate void PointerEventFunction<T>(T handler,PointerEventData eventData);
        protected delegate void ItemEventFunction<T>(T handler, Item item, GameObject player);
        protected delegate void FailureItemEventFunction<T>(T handler, Item item, GameObject player, FailureCause failureCause);

        protected AnimatorStateInfo[] m_LayerStateMap;

        protected static void Execute(ITriggerUsedHandler handler, GameObject player)
        {
            handler.OnTriggerUsed(player);
        }

        protected static void Execute(ITriggerUnUsedHandler handler, GameObject player)
        {
            handler.OnTriggerUnUsed(player);
        }

        protected static void Execute(ITriggerCameInRange handler, GameObject player)
        {
            handler.OnCameInRange(player);
        }

        protected static void Execute(ITriggerWentOutOfRange handler, GameObject player)
        {
            handler.OnWentOutOfRange(player);
        }

        protected static void Execute(ITriggerPointerEnter handler,PointerEventData eventData)
        {
            handler.OnPointerEnter(eventData);
        }

        protected static void Execute(ITriggerPointerExit handler, PointerEventData eventData)
        {
            handler.OnPointerExit(eventData);
        }

        protected virtual void Start() {
            this.RegisterCallbacks();
            this.m_ActionBehavior = new Sequence(actions.ToArray());
            this.m_TriggerEvents = GetComponentsInChildren<ITriggerEventHandler>();
            if (gameObject == InventoryManager.current.PlayerInfo.gameObject)
            {
                InRange = true;
            }
            else
            {
                //Create trigger collider
                CreateTriggerCollider();
            }
        }

        //Called once per frame
        protected virtual void Update() {
            if (!InRange) { return; }

            if (InventoryManager.DefaultSettings.debugMessages)
            {
                Vector3 targetPosition = InventoryManager.current.GetBounds(gameObject).center;
                Vector3 playerPosition = InventoryManager.current.PlayerInfo.transform.position;
                Bounds bounds = InventoryManager.current.PlayerInfo.bounds;
                playerPosition.y += bounds.center.y + bounds.extents.y;
                Vector3 direction = targetPosition - playerPosition;
                Debug.DrawRay(playerPosition, direction, Color.red);
            }

            //Check for key down and if trigger input type supports key.
            if (Input.GetKeyDown(key) && triggerType.HasFlag<TriggerInputType>(TriggerInputType.Key) && InRange && IsBestTrigger()) {
                Use();
            }
            //Update task behavior, set in use if it is running
           this.InUse = this.m_ActionBehavior.Tick();
        }

        //Called by the EventSystem, when player clicks on the game object.
        public virtual void OnPointerClick(PointerEventData eventData)
        {
            //Check if  TriggerInputType.OnClick is included.
            if (triggerType.HasFlag<TriggerInputType>(TriggerInputType.OnClick) && !UIUtility.IsPointerOverUI())
            {
                Use();
            }
        }

        //Called by the EventSystem, when the pointer is over the trigger.
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!UIUtility.IsPointerOverUI())
            {
                ExecuteEvent<ITriggerPointerEnter>(Execute, eventData);
                if (this.useCustomCursor)
                {
                    UICursor.Set(this.m_CursorSprite, this.m_CursorSize, false, this.m_CursorAnimatorState);
                }
            }
        }

        //Called by the EventSystem, when the pointer exits the trigger.
        public void OnPointerExit(PointerEventData eventData)
        {
            if (!UIUtility.IsPointerOverUI())
            {
                ExecuteEvent<ITriggerPointerExit>(Execute, eventData);
                if (this.useCustomCursor)
                {
                    UICursor.Clear();
                }
            }
        }

        //OnTriggerEnter is called when the Collider other enters the trigger.
        protected virtual void OnTriggerEnter(Collider other)
        {
            //Check if the collider other is player 
            if (other.tag == InventoryManager.current.PlayerInfo.gameObject.tag)
            {
                //Set that player is in range
                InRange = true;
            }
        }

        //OnTriggerExit is called when the Collider other has stopped touching the trigger.
        protected virtual void OnTriggerExit(Collider other)
        {
            //Check if the collider other is player
            if (other.tag == InventoryManager.current.PlayerInfo.gameObject.tag)
            {
                //Set that player is out of range
                InRange = false;
            }
        }

        //Called when the player quits the game
        private void OnApplicationQuit()
        {
            this.m_ApplicationQuit = true;
        }


        private void OnDestroy()
        {
            //Check if the user quits the game
            if (!this.m_ApplicationQuit){
                //Set in range to false when the game object gets destroyed to invoke OnTriggerUnUsed events
                InRange = false;
            }
        }

        //used for UI Button reference
        public void StartUse() {
            Use();
        }

        //Use the trigger
        public virtual bool Use() {
            return Use(InventoryManager.current.PlayerInfo.gameObject);
        }

        //Use the trigger
        public virtual bool Use(GameObject player) {

            //Can the trigger be used?
            if (!CanUse(player)) {
                return false;
            }
            Trigger.currentUsedTrigger = this;
            //Set the trigger in use
            this.InUse = true;
            this.m_ActionBehavior.Start();
            return true;
        }

        public virtual bool CanUse() {
            return CanUse(InventoryManager.current.PlayerInfo.gameObject);
        }

        //Can the trigger be used?
        public virtual bool CanUse(GameObject player) {
            //Return false if the trigger is already used
            if (InUse || (Trigger.currentUsedTrigger != null && Trigger.currentUsedTrigger.InUse)) {
                InventoryManager.Notifications.inUse.Show();
                return false;
            }

            //Return false if the player is not in range
            if (!InRange) {
                InventoryManager.Notifications.toFarAway.Show();
                return false;
            }
                
            Vector3 targetPosition = InventoryManager.current.GetBounds(gameObject).center;//transform.position;
            Vector3 playerPosition = InventoryManager.current.PlayerInfo.transform.position;
            Bounds bounds = InventoryManager.current.PlayerInfo.bounds;
            playerPosition.y += bounds.center.y + bounds.extents.y;
            Vector3 direction = targetPosition - playerPosition;

            InventoryManager.current.PlayerInfo.collider.enabled = false;
            RaycastHit hit;
            bool raycast = Physics.Raycast(playerPosition, direction, out hit);
            InventoryManager.current.PlayerInfo.collider.enabled = true;
            if (raycast && hit.transform != transform && gameObject != InventoryManager.current.PlayerInfo.gameObject) {
                if(InventoryManager.DefaultSettings.debugMessages)
                    Debug.Log("Can't use Trigger: "+transform.name+". Raycast failed hit: "+hit.transform.name);
                return false;
            }
            //Trigger can be used
            return true;
        }

        protected void CacheAnimatorStates() {
            Animator animator = InventoryManager.current.PlayerInfo.animator;
            this.m_LayerStateMap = new AnimatorStateInfo[animator.layerCount];
            for (int j = 0; j < animator.layerCount; j++)
            {
                AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(j);
                this.m_LayerStateMap[j] = stateInfo;
            }
        }

        protected void LoadCachedAnimatorStates()
        {
            Animator animator = InventoryManager.current.PlayerInfo.animator;
            for (int j = 0; j < this.m_LayerStateMap.Length; j++)
            {

                if (animator.GetCurrentAnimatorStateInfo(j).shortNameHash != this.m_LayerStateMap[j].shortNameHash && !animator.IsInTransition(j))
                {
#if Proxy
                    Proxy.SendToAll (gameObject, "CrossFadeInFixedTime", this.m_LayerStateMap [j].shortNameHash, 0.2f);
#else
                    animator.CrossFadeInFixedTime(this.m_LayerStateMap[j].shortNameHash, 0.2f);
#endif
                }
            }
        }
    

    /// <summary>
    /// Override the ItemSlot.Use behavior. 
    /// </summary>
    /// <param name="item">Item that is used</param>
    /// <param name="slot">Slot that is currently used(Can be a referenced slot!)</param>
    /// <returns>Return false to execute default behavior</returns>
    public virtual bool OverrideItemUse(Slot slot, Item item) {
            return false;
        }

        //Returns true if this is the best trigger. Used for TriggerInputType.Key and TriggerInputType.OnTriggerEnter
        //Calculated based on distance and rotation of the player to the trigger.
        protected virtual bool IsBestTrigger() {
            if (gameObject == InventoryManager.current.PlayerInfo.gameObject){
                return true;
            }
           
            Trigger tMin = null;
            float minDist = Mathf.Infinity;
            Vector3 currentPos = InventoryManager.current.PlayerInfo.transform.position;
            foreach (Trigger t in Trigger.m_TriggerInRange)
            {
                if (t.key != key) continue;
                Vector3 dir = t.transform.position - currentPos;
                float angle = 0f;
                if (dir != Vector3.zero)
                    Quaternion.Angle(InventoryManager.current.PlayerInfo.transform.rotation, Quaternion.LookRotation(dir));

                float dist = Vector3.Distance(t.transform.position, currentPos) * angle;
                if (dist < minDist)
                {
                    tMin = t;
                    minDist = dist;
                }
            }
            return tMin == this;
        }

        //Creates a sphere collider so OnTriggerEnter/OnTriggerExit gets called
        protected virtual void CreateTriggerCollider() {
            Vector3 position = Vector3.zero;

            Collider collider = GetComponent<Collider>();
            if (collider != null)
            {
                position = collider.bounds.center;
                position.y = (collider.bounds.center - collider.bounds.extents).y;
                position = transform.InverseTransformPoint(position);
            }

            SphereCollider sphereCollider = gameObject.AddComponent<SphereCollider>();
            sphereCollider.isTrigger = true;
            sphereCollider.center = position;
            Vector3 scale = transform.lossyScale;
            sphereCollider.radius =  useDistance/ Mathf.Max(scale.x, scale.y, scale.z);
        }

        //Execute event
        protected void ExecuteEvent<T>(EventFunction<T> func, bool includeDisabled = false) where T : ITriggerEventHandler{
            for (int i = 0; i < this.m_TriggerEvents.Length; i++) {
                ITriggerEventHandler handler = this.m_TriggerEvents[i];
                if (ShouldSendEvent<T>(handler, includeDisabled))
                {
                    func.Invoke((T)handler, InventoryManager.current.PlayerInfo.gameObject);
                }
            }
            string eventID = string.Empty;
            if (this.m_CallbackHandlers.TryGetValue(typeof(T), out eventID))
            {
                TriggerEventData triggerEventData = new TriggerEventData();
                triggerEventData.trigger = this;
                triggerEventData.player = InventoryManager.current.PlayerInfo.gameObject;
                triggerEventData.eventData = new PointerEventData(EventSystem.current);
                base.Execute(eventID, triggerEventData);
            }
        }

        protected void ExecuteEvent<T>(PointerEventFunction<T> func,PointerEventData eventData ,bool includeDisabled = false) where T : ITriggerEventHandler
        {
            for (int i = 0; i < this.m_TriggerEvents.Length; i++)
            {
                ITriggerEventHandler handler = this.m_TriggerEvents[i];
                if (ShouldSendEvent<T>(handler, includeDisabled))
                {
                    func.Invoke((T)handler,eventData);
                }
            }

            string eventID = string.Empty;
            if (this.m_CallbackHandlers.TryGetValue(typeof(T), out eventID)){
                TriggerEventData triggerEventData = new TriggerEventData();
                triggerEventData.trigger = this;
                triggerEventData.player = InventoryManager.current.PlayerInfo.gameObject;
                triggerEventData.eventData = eventData;
                base.Execute(eventID, triggerEventData);
            }
        }

        protected void ExecuteEvent<T>(ItemEventFunction<T> func, Item item, bool includeDisabled = false) where T : ITriggerEventHandler
        {
            for (int i = 0; i < this.m_TriggerEvents.Length; i++)
            {
                ITriggerEventHandler handler = this.m_TriggerEvents[i];
                if (ShouldSendEvent<T>(handler, includeDisabled))
                {
                    func.Invoke((T)handler, item, InventoryManager.current.PlayerInfo.gameObject);
                }
            }

            string eventID = string.Empty;
            if (this.m_CallbackHandlers.TryGetValue(typeof(T), out eventID))
            {
                TriggerEventData triggerEventData = new TriggerEventData();
                triggerEventData.trigger = this;
                triggerEventData.player = InventoryManager.current.PlayerInfo.gameObject;
                triggerEventData.eventData = new PointerEventData(EventSystem.current);
                triggerEventData.item = item;
                base.Execute(eventID, triggerEventData);
            }
        }

        protected void ExecuteEvent<T>(FailureItemEventFunction<T> func, Item item, FailureCause failureCause , bool includeDisabled = false) where T : ITriggerEventHandler
        {
            for (int i = 0; i < this.m_TriggerEvents.Length; i++)
            {
                ITriggerEventHandler handler = this.m_TriggerEvents[i];
                if (ShouldSendEvent<T>(handler, includeDisabled))
                {
                    func.Invoke((T)handler, item, InventoryManager.current.PlayerInfo.gameObject, failureCause);
                }
            }

            string eventID = string.Empty;
            if (this.m_CallbackHandlers.TryGetValue(typeof(T), out eventID))
            {
                TriggerEventData triggerEventData = new TriggerEventData();
                triggerEventData.trigger = this;
                triggerEventData.player = InventoryManager.current.PlayerInfo.gameObject;
                triggerEventData.eventData = new PointerEventData(EventSystem.current);
                triggerEventData.item = item;
                triggerEventData.failureCause = failureCause;
                base.Execute(eventID, triggerEventData);
            }
        }

        //Check if we should execute the event on that handler
        protected bool ShouldSendEvent<T>(ITriggerEventHandler handler, bool includeDisabled)
        {
            var valid = handler is T;
            if (!valid)
                return false;
            var behaviour = handler as Behaviour;
            if (behaviour != null && !includeDisabled)
                return behaviour.isActiveAndEnabled;

            return true;
        }

        //TODO: Auto registration
        protected virtual void RegisterCallbacks() {
            this.m_CallbackHandlers = new Dictionary<Type,string>();
            this.m_CallbackHandlers.Add(typeof(ITriggerPointerEnter), "OnPointerEnter");
            this.m_CallbackHandlers.Add(typeof(ITriggerPointerExit), "OnPointerExit");
            this.m_CallbackHandlers.Add(typeof(ITriggerUsedHandler), "OnTriggerUsed");
            this.m_CallbackHandlers.Add(typeof(ITriggerUnUsedHandler), "OnTriggerUnUsed");
            this.m_CallbackHandlers.Add(typeof(ITriggerCameInRange), "OnCameInRange");
            this.m_CallbackHandlers.Add(typeof(ITriggerWentOutOfRange), "OnWentOutOfRange");

        }

        protected void LockAllItemContainer(bool state) {
            if (this.m_LockContainersWhenUsed)
            {
                ItemContainer[] containers = FindObjectsOfType<ItemContainer>();
                for (int i = 0; i < containers.Length; i++)
                {
                    containers[i].Lock(state);
                }
            }
        }

        [System.Flags]
        public enum TriggerInputType
        {
            OnClick = 1,
            Key = 2,
            OnTriggerEnter = 4
        }
	}
}
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace JoyLib.Code.Unity.GUI
{
    [RequireComponent(typeof(GUIData))]
    public class ContextMenu : MonoBehaviour
    {
        [SerializeField]
        protected MenuItem m_MenuItemPrefab = null;
        protected List<MenuItem> ItemCache { get; set; }
        protected RectTransform RectTransform { get; set; }
        
        protected GUIData GUIData { get; set; }

        public void OnEnable()
        {
            if (this.RectTransform is null)
            {
                this.RectTransform = this.GetComponent<RectTransform>();
                this.ItemCache = new List<MenuItem>();
                this.GUIData = this.GetComponent<GUIData>();
            }
        }

        public void Show()
        {
            if (this.ItemCache.Any(item => item.gameObject.activeSelf))
            {
                this.RectTransform.position = Mouse.current.position.ReadValue();
            }
        }

        public void Update()
        {
            if (Mouse.current.leftButton.wasReleasedThisFrame) 
            {
                var pointer = new PointerEventData (EventSystem.current);
                pointer.position = Mouse.current.position.ReadValue();
                var raycastResults = new List<RaycastResult> ();
                EventSystem.current.RaycastAll (pointer, raycastResults);

                for (int i = 0; i < raycastResults.Count; i++) 
                {
                    MenuItem item = raycastResults[i].gameObject.GetComponent<MenuItem> ();
                    if (item is null == false) 
                    {
                        item.OnPointerClick(pointer);
                        return;
                    }
                }
                
                this.GUIData.GUIManager.CloseGUI(GUINames.CONTEXT_MENU);
            }
        }

        public virtual void Clear ()
        {
            for (int i = 0; i < this.ItemCache.Count; i++) 
            {
                this.ItemCache [i].gameObject.SetActive(false);
            }
        }

        public virtual MenuItem AddMenuItem (string text, UnityAction used)
        {
            MenuItem item = null;
            if (this.ItemCache.All(x => x.gameObject.activeSelf))
            {
                item = Instantiate(this.m_MenuItemPrefab);
                this.ItemCache.Add(item);
            }
            else
            {
                item = this.ItemCache.First(x => !x.gameObject.activeSelf);
            }

            item.Text.text = text;
            item.Trigger.RemoveAllListeners();
            item.gameObject.SetActive(true);
            item.transform.SetParent(this.RectTransform, false);
            item.Trigger.AddListener(
                delegate
                {
                    this.GUIData.GUIManager.CloseGUI(GUINames.CONTEXT_MENU);
                    if (used is null == false) 
                    {
                        used.Invoke();
                    }
                });
            return item;
        }
    }
}
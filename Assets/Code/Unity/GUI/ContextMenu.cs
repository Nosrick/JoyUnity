using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace JoyLib.Code.Unity.GUI
{
    public class ContextMenu : GUIData
    {
        [SerializeField]
        protected MenuItem m_MenuItemPrefab = null;
        protected List<MenuItem> ItemCache { get; set; }
        protected RectTransform RectTransform { get; set; }

        public void OnEnable()
        {
            if (this.RectTransform is null)
            {
                this.RectTransform = this.GetComponent<RectTransform>();
            }
        }

        public override void Show()
        {
            base.Show();
            this.RectTransform.position = Input.mousePosition;
            base.Show ();
        }

        public void Update()
        {
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2)) 
            {
                var pointer = new PointerEventData (EventSystem.current);
                pointer.position = Input.mousePosition;
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

                this.Close ();
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
            MenuItem item = this.ItemCache.First(x => !x.gameObject.activeSelf);

            if (item is null) 
            {
                Debug.Log(text);
                item = Instantiate(this.m_MenuItemPrefab) as MenuItem;
                this.ItemCache.Add(item);
            }
            item.Text.text = text;
            item.Trigger.RemoveAllListeners();
            item.gameObject.SetActive(true);
            item.transform.SetParent(this.RectTransform, false);
            item.Trigger.AddListener(
                delegate() 
                {
                    this.Close();
                    if (used is null == false) 
                    {
                        used.Invoke();
                    }
                });
            return item;
        }
    }
}
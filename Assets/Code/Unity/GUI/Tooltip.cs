using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Core.Internal;
using JoyLib.Code.Graphics;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace JoyLib.Code.Unity.GUI
{
    public class Tooltip : GUIData
    {
        [SerializeField] protected TextMeshProUGUI m_Title;
        [SerializeField] protected TextMeshProUGUI m_Text;
        [SerializeField] protected ManagedUISprite m_Icon;
        [SerializeField] protected RectTransform m_IconSlot;
        [SerializeField] protected Image m_Background;
        [SerializeField] protected StringPairContainer m_ItemPrefab;
        [SerializeField] protected LayoutGroup m_ParentLayout;
        [SerializeField] protected Vector2 m_PositionOffset;

        protected Canvas Canvas { get; set; }
        protected RectTransform RectTransform { get; set; }
        
        protected List<StringPairContainer> ItemCache { get; set; }

        public override void Awake()
        {
            base.Awake();
            if (this.Canvas is null)
            {
                this.ItemCache = new List<StringPairContainer>();
                this.Canvas = this.GetComponentInParent<Canvas>();
                this.RectTransform = this.GetComponent<RectTransform>();
                this.m_Icon.Awake();
            }
        }

        public void Update()
        {
            this.UpdatePosition();
        }

        protected void UpdatePosition()
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                this.Canvas.transform as RectTransform,
                mousePosition, 
                this.Canvas.worldCamera, 
                out Vector2 pos);

            
            Vector2 offset = Vector2.zero;
            if (mousePosition.x < this.RectTransform.sizeDelta.x)
            {
                offset += new Vector2(this.RectTransform.sizeDelta.x * 0.5f, 0) + this.m_PositionOffset;
            }
            else
            {
                offset -= new Vector2(this.RectTransform.sizeDelta.x * 0.5f, 0) + this.m_PositionOffset;
            }

            if (mousePosition.y > Screen.height - this.RectTransform.sizeDelta.y)
            {
                offset -= new Vector2(0, this.RectTransform.sizeDelta.y * 0.5f) + this.m_PositionOffset;
            }
            else
            {
                offset += new Vector2(0, this.RectTransform.sizeDelta.y * 0.5f) + this.m_PositionOffset;
            }

            this.transform.position = this.Canvas.transform.TransformPoint(pos + offset);
        }

        public virtual void Show(
            string title = null, 
            string content = null, 
            ISpriteState sprite = null, 
            IEnumerable<Tuple<string, string>> data = null, 
            bool showBackground = true)
        {
            if (!string.IsNullOrEmpty(title))
            {
                this.m_Title.gameObject.SetActive(true);
                this.m_Title.text = title;
            }
            else
            {
                this.m_Title.gameObject.SetActive(false);
            }

            this.m_Text.text = content;
            this.m_Text.gameObject.SetActive(!content.IsNullOrEmpty());

            if (sprite is null == false)
            {
                this.m_IconSlot.gameObject.SetActive(true);
                this.SetIcon(sprite);
            }
            else
            {
                this.m_IconSlot.gameObject.SetActive(false);
            }

            if (data.IsNullOrEmpty() == false)
            {
                Tuple<string,string>[] dataArray = data.ToArray();
                if (this.ItemCache.Count < dataArray.Length)
                {
                    for (int i = this.ItemCache.Count; i < dataArray.Length; i++)
                    {
                        this.ItemCache.Add(Instantiate(this.m_ItemPrefab, this.m_ParentLayout.transform, false));
                    }
                }
                else if (this.ItemCache.Count > dataArray.Length)
                {
                    for (int i = dataArray.Count(); i < this.ItemCache.Count; i++)
                    {
                        this.ItemCache[i].gameObject.SetActive(false);
                    }
                }
                    
                for (int i = 0; i < dataArray.Length; i++)
                {
                    this.ItemCache[i].Target = dataArray[i];
                    this.ItemCache[i].gameObject.SetActive(true);
                }
            }
            else
            {
                foreach(var item in this.ItemCache)
                {
                    item.gameObject.SetActive(false);
                }
            }
           
            this.m_Background.gameObject.SetActive(showBackground);
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(this.RectTransform);

            base.Show();
        }

        protected void SetIcon(ISpriteState state)
        {
            this.m_Icon.Clear();
            this.m_Icon.AddSpriteState(state, true);
        }
    }
}
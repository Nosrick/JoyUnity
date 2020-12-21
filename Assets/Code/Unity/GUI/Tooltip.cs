using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Core.Internal;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JoyLib.Code.Unity.GUI
{
    public class Tooltip : GUIData
    {
        [SerializeField] protected TextMeshProUGUI m_Title;
        [SerializeField] protected TextMeshProUGUI m_Text;
        [SerializeField] protected Image m_Icon;
        [SerializeField] protected Image m_Background;
        [SerializeField] protected StringPairContainer m_ItemPrefab;
        [SerializeField] protected LayoutGroup m_ParentLayout;

        protected Canvas Canvas { get; set; }
        protected RectTransform RectTransform { get; set; }
        
        protected List<StringPairContainer> ItemCache { get; set; }

        public void Awake()
        {
            if (this.Canvas is null)
            {
                this.ItemCache = new List<StringPairContainer>();
                this.Canvas = this.GetComponentInParent<Canvas>();
                this.RectTransform = this.GetComponent<RectTransform>();
            }
        }

        public void Update()
        {
            this.UpdatePosition();
        }

        protected void UpdatePosition()
        {
            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                this.Canvas.transform as RectTransform,
                Input.mousePosition, 
                this.Canvas.worldCamera, 
                out pos);
            
            Vector2 offset = Vector2.zero;

            if (Input.mousePosition.x < this.RectTransform.sizeDelta.x)
            {
                offset += new Vector2(this.RectTransform.sizeDelta.x * 0.5f, 0);
            }
            else
            {
                offset += new Vector2(-this.RectTransform.sizeDelta.x * 0.5f, 0);
            }

            if (Screen.height - Input.mousePosition.y > this.RectTransform.sizeDelta.y)
            {
                offset += new Vector2(0, this.RectTransform.sizeDelta.y * 0.5f);
            }
            else
            {
                offset += new Vector2(0, -this.RectTransform.sizeDelta.y * 0.5f);
            }

            this.transform.position = this.Canvas.transform.TransformPoint(pos);
        }

        public virtual void Show(
            string title = null, 
            string content = null, 
            Sprite sprite = null, 
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

            if (sprite is null == false)
            {
                this.m_Icon.overrideSprite = sprite;
                this.m_Icon.transform.parent.gameObject.SetActive(true);
            }
            else
            {
                this.m_Icon.transform.parent.gameObject.SetActive(false);
            }

            if (data.IsNullOrEmpty() == false && data.Count() > 0)
            {

                if (this.ItemCache.Count < data.Count())
                {
                    for (int i = this.ItemCache.Count; i < data.Count(); i++)
                    {
                        this.ItemCache.Add(GameObject.Instantiate(this.m_ItemPrefab, this.m_ParentLayout.transform, false));
                    }
                }
                else if (this.ItemCache.Count > data.Count())
                {
                    for (int i = data.Count(); i < this.ItemCache.Count; i++)
                    {
                        this.ItemCache[i].gameObject.SetActive(false);
                    }
                }
                    
                List<Tuple<string, string>> dataList = data.ToList();
                for (int i = 0; i < dataList.Count; i++)
                {
                    this.ItemCache[i].Target = dataList[i];
                    this.ItemCache[i].gameObject.SetActive(true);
                }
            }
           
            this.m_Background.gameObject.SetActive(showBackground);

            base.Show();
        }
    }
}
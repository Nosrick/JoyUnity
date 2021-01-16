using System.Collections.Generic;
using JoyLib.Code.Graphics;
using JoyLib.Code.Unity.GUI.Managed_Assets;
using UnityEngine;
using UnityEngine.UI;

namespace JoyLib.Code.Unity.GUI
{
    public class ManagedBackground : ManagedElement
    {
        [SerializeField] protected ManagedUISprite m_BackgroundPrefab;

        public bool HasBackground { get; protected set; }
        public bool HasColours { get; protected set; }

        protected ManagedUISprite m_BackgroundInstance;

        public void Awake()
        {
            if (this.Initialised)
            {
                return;
            }
            
            this.m_BackgroundInstance = Instantiate(this.m_BackgroundPrefab, this.transform);
            this.m_BackgroundInstance.gameObject.AddComponent<LayoutElement>().ignoreLayout = true;
            this.m_BackgroundInstance.Awake();
            this.m_BackgroundInstance.transform.SetAsFirstSibling();
            RectTransform myRect = this.GetComponent<RectTransform>();
            RectTransform backgroundRect = this.m_BackgroundInstance.GetComponent<RectTransform>();
            backgroundRect.anchorMin = Vector2.zero;
            backgroundRect.anchorMax = Vector2.one;
            backgroundRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, myRect.rect.width);
            backgroundRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, myRect.rect.height);

            this.Initialised = true;
        }
        
        public void SetBackground(ISpriteState sprite)
        {
            if (this.Initialised == false)
            {
                this.Awake();
            }
            this.m_BackgroundInstance.Clear();
            this.m_BackgroundInstance.AddSpriteState(sprite);
            this.HasBackground = true;
        }

        public void SetColours(IDictionary<string, Color> colours)
        {
            if (this.Initialised == false)
            {
                this.Awake();
            }
            if (this.m_BackgroundInstance.CurrentSpriteState is null)
            {
                GlobalConstants.ActionLog.AddText("Trying to set colours of a null sprite state. " + this.name);
                //GlobalConstants.ActionLog.AddText();
                return;
            }

            this.m_BackgroundInstance.OverrideAllColours(colours);
            this.HasColours = true;
        }
    }
}
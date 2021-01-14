using System.Collections.Generic;
using JoyLib.Code.Graphics;
using UnityEngine;
using UnityEngine.UI;

namespace JoyLib.Code.Unity.GUI
{
    public class ManagedBackground : MonoBehaviour
    {
        [SerializeField] protected ManagedUISprite m_BackgroundPrefab;
        
        public bool HasBackground { get; protected set; }
        public bool HasColours { get; protected set; }

        protected ManagedUISprite m_BackgroundInstance;

        public void Awake()
        {
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
        }
        
        public void SetBackground(ISpriteState sprite)
        {
            this.m_BackgroundInstance.Clear();
            this.m_BackgroundInstance.AddSpriteState(sprite);
            this.HasBackground = true;
        }

        public void SetColours(IDictionary<string, Color> colours)
        {
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
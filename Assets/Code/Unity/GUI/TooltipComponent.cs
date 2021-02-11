using System;
using System.Collections.Generic;
using System.Linq;
using JoyLib.Code.Graphics;
using JoyLib.Code.Unity.GUI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace JoyLib.Code.Unity
{
    [RequireComponent(typeof(ManagedSprite))]
    public class TooltipComponent : GridPosition, IPointerEnterHandler, IPointerExitHandler, IPosition
    {
        public delegate IEnumerable<Tuple<string, string>> SetTooltip(IPosition positionable);

        [SerializeField] protected bool m_ShowIcon = true;
        
        protected ManagedSprite ManagedSprite { get; set; }
        protected ISpriteState Sprite { get; set; }
        public IGUIManager GUIManager { get; set; }

        public IEnumerable<Tuple<string, string>> Tooltip { get; set; }

        public SetTooltip RefreshTooltip { get; set; }

        public void Awake()
        {
            if (this.GUIManager is null)
            {
                this.GUIManager = GlobalConstants.GameManager.GUIManager;
            }
            if (!(this.ManagedSprite is null))
            {
                return;
            }
            this.ManagedSprite = this.gameObject.GetComponent<ManagedSprite>();
            if (this.ManagedSprite is null == false)
            {
                this.Sprite = this.GetComponent<ManagedSprite>().CurrentSpriteState;
            }
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (this.RefreshTooltip is null == false)
            {
                List<Tuple<string, string>> tooltipData = this.RefreshTooltip(this).ToList();
                this.Tooltip = tooltipData;
            }

            this.GUIManager.OpenGUI(GUINames.TOOLTIP)
                .GetComponent<Tooltip>()
                .Show(
                    this.gameObject.name,
                    null,
                    this.m_ShowIcon ? this.Sprite : null,
                    this.Tooltip);
        }

        public void OnPointerExit(PointerEventData eventData)
        {            
            this.GUIManager.CloseGUI(GUINames.TOOLTIP);
        }
    }
}
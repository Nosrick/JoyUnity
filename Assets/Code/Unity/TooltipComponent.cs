using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Core.Internal;
using JoyLib.Code.Unity.GUI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace JoyLib.Code.Unity
{
    public class TooltipComponent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPosition
    {
        public delegate IEnumerable<Tuple<string, string>> SetTooltip(IPosition positionable);
        
        protected SpriteRenderer SpriteRenderer { get; set; }
        protected Sprite Sprite { get; set; }
        public static IGUIManager GUIManager { get; set; }

        public IEnumerable<Tuple<string, string>> Tooltip { get; set; }

        public SetTooltip RefreshTooltip { get; set; }

        public Vector2Int WorldPosition { get; set; }

        public void Awake()
        {
            if (GUIManager is null)
            {
                GUIManager = GlobalConstants.GameManager.GUIManager;
            }
            if (!(this.SpriteRenderer is null))
            {
                return;
            }
            this.SpriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
            if (this.SpriteRenderer is null == false)
            {
                this.Sprite = this.SpriteRenderer.sprite;
            }
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (this.Tooltip.IsNullOrEmpty() && this.RefreshTooltip is null)
            {
                return;
            }
            
            List<Tuple<string, string>> tooltipData = this.RefreshTooltip(this).ToList();
            this.Tooltip = tooltipData;

            GUIManager.OpenGUI(GUINames.TOOLTIP)
                .GetComponent<Tooltip>()
                .Show(
                    this.gameObject.name,
                    null,
                    this.Sprite,
                    this.Tooltip);
        }

        public void OnPointerExit(PointerEventData eventData)
        {            
            GUIManager.CloseGUI(GUINames.TOOLTIP);
        }
        public void Move(Vector2Int position)
        {
            this.WorldPosition = position;
            this.gameObject.transform.position = new Vector3(position.x, position.y);
        }
    }
}
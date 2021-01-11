using System.Collections.Generic;
using JoyLib.Code.Graphics;
using UnityEngine;
using UnityEngine.InputSystem;

namespace JoyLib.Code.Unity.GUI
{
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(GUIData))]
    public class Cursor : MonoBehaviour
    {
        [SerializeField] protected ManagedUISprite m_PartPrefab;
        
        protected ManagedUISprite CursorObject { get; set; }
        protected CanvasGroup CanvasGroup { get; set; }
        protected RectTransform MyRect { get; set; }
        
        public int FrameIndex { get; protected set; }
        public string ChosenSpriteState { get; protected set; }
        public string TileSet { get; protected set; }
        public float TimeSinceLastChange { get; protected set; }
        public bool IsAnimated { get; set; }
        
        protected ManagedUISprite DragObject { get; set; }

        public void Awake()
        {
            this.CanvasGroup = this.GetComponent<CanvasGroup>();
            this.MyRect = this.GetComponent<RectTransform>();

            if (this.DragObject is null)
            {
                this.DragObject = Instantiate(this.m_PartPrefab, this.transform);
                this.DragObject.Awake();
                this.DragObject.gameObject.SetActive(false);
                RectTransform dragRect = this.DragObject.GetComponent<RectTransform>();
                dragRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, this.MyRect.rect.width * 2);
                dragRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, this.MyRect.rect.height * 2);
            }
            
            UnityEngine.Cursor.visible = false;

            if (this.CursorObject is null)
            {
                this.CursorObject = Instantiate(this.m_PartPrefab, this.transform);
                this.CursorObject.Awake();
            }
        }

        public void Update()
        {
            if (!this.enabled)
            {
                return;
            }

            Rect rect = this.MyRect.rect;
            this.transform.position = Mouse.current.position.ReadValue() + new Vector2(rect.width / 4, -(rect.height / 4));
        }

        public void SetCursorSize(int width, int height)
        {
            this.MyRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            this.MyRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            RectTransform cursorRect = this.CursorObject.GetComponent<RectTransform>();
            cursorRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            cursorRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            RectTransform dragRect = this.DragObject.GetComponent<RectTransform>();
            dragRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width * 2);
            dragRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height * 2);
        }

        public void SetCursorSprites(ISpriteState state)
        {
            this.CursorObject.gameObject.SetActive(true);
            this.CursorObject.Clear();
            this.CursorObject.AddSpriteState(state, true);
        }

        public void SetCursorColours(IDictionary<string, Color> colours)
        {
            foreach (ISpriteState state in this.CursorObject.States)
            {
                for (int j = 0; j < state.SpriteData.m_Parts.Count; j++)
                {
                    SpritePart part = state.SpriteData.m_Parts[j];
                    if (!colours.ContainsKey(part.m_Name))
                    {
                        continue;
                    }

                    part.m_PossibleColours = new List<Color> {colours[part.m_Name]};
                    state.SpriteData.m_Parts[j] = part;
                    this.CursorObject.Clear();
                }
                this.CursorObject.AddSpriteState(state, true);
            }
        }

        public void Show(ISpriteState replacement)
        {
            this.DragObject.Clear();
            this.DragObject.gameObject.SetActive(false);
            if (replacement is null)
            {
                return;
            }
            this.DragObject.gameObject.SetActive(true);
            this.DragObject.AddSpriteState(replacement, true);
        }

        public void Reset()
        {
            this.DragObject.Clear();
        }
    }
}
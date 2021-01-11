using System.Collections.Generic;
using JoyLib.Code.Graphics;
using UnityEngine;
using UnityEngine.InputSystem;

namespace JoyLib.Code.Unity.GUI
{
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(Sprite))]
    public class Cursor : GUIData
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

        public override void Awake()
        {
            base.Awake();
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

        public override void Close()
        {
            base.Close();
            this.CanvasGroup.alpha = 0f;
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
            this.CursorObject.Clear();
            this.CursorObject.AddSpriteState(state);
            this.CursorObject.ChangeState(state.Name);
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

        public override void Show()
        {
            base.Show();
            this.CanvasGroup.alpha = 1f;
        }

        public void Show(ISpriteState replacement)
        {
            this.Show();
            this.DragObject.Clear();
            if (replacement is null)
            {
                return;
            }
            this.DragObject.AddSpriteState(replacement);
            this.DragObject.ChangeState(replacement.Name);
        }

        public void Reset()
        {
            this.DragObject.Clear();
        }
    }
}
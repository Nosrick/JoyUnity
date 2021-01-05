using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace JoyLib.Code.Unity.GUI
{
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(Sprite))]
    public class Cursor : GUIData
    {
        [SerializeField] protected Image m_PartPrefab;
        protected List<Image> CursorObjects { get; set; }
        
        protected CanvasGroup CanvasGroup { get; set; }
        protected RectTransform MyRect { get; set; }
        protected List<Sprite> CursorSprites { get; set; }
        protected List<Color> CursorColours { get; set; }
        
        protected Image DragObject { get; set; }

        public override void Awake()
        {
            base.Awake();
            this.CanvasGroup = this.GetComponent<CanvasGroup>();
            this.MyRect = this.GetComponent<RectTransform>();

            if (this.DragObject is null)
            {
                this.DragObject = Instantiate(this.m_PartPrefab, this.transform);
                this.DragObject.gameObject.SetActive(false);
                RectTransform dragRect = this.DragObject.GetComponent<RectTransform>();
                dragRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, this.MyRect.rect.width * 2);
                dragRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, this.MyRect.rect.height * 2);
            }
            
            UnityEngine.Cursor.visible = false;

            this.CursorObjects = new List<Image>();
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
            foreach (Image cursorPart in this.CursorObjects)
            {
                RectTransform cursorRect = cursorPart.GetComponent<RectTransform>();
                cursorRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
                cursorRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            }
            RectTransform dragRect = this.DragObject.GetComponent<RectTransform>();
            dragRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width * 2);
            dragRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height * 2);
        }

        public void SetCursorSprites(IEnumerable<Sprite> sprites, IEnumerable<Color> spriteColours)
        {
            this.CursorSprites = new List<Sprite>(sprites);
            this.CursorColours = new List<Color>(spriteColours);
            if (this.CursorSprites.Count > this.CursorObjects.Count)
            {
                for (int i = this.CursorObjects.Count; i < this.CursorSprites.Count; i++)
                {
                    this.CursorObjects.Add(Instantiate(this.m_PartPrefab, this.transform));
                }
            }
            else if (this.CursorSprites.Count < this.CursorObjects.Count)
            {
                for (int i = this.CursorSprites.Count; i < this.CursorObjects.Count; i++)
                {
                    this.CursorObjects[i].gameObject.SetActive(false);
                }
            }

            if (this.CursorColours.Count < this.CursorSprites.Count)
            {
                for (int i = this.CursorColours.Count; i < this.CursorSprites.Count; i++)
                {
                    this.CursorColours.Add(Color.magenta);
                }
            }
            
            for (int i = 0; i < this.CursorObjects.Count; i++)
            {
                this.CursorObjects[i].sprite = this.CursorSprites[i];
                this.CursorObjects[i].color = this.CursorColours[i];
                this.CursorObjects[i].gameObject.SetActive(true);
            }
        }

        public void SetCursorColours(IEnumerable<Color> spriteColours)
        {
            this.SetCursorSprites(this.CursorSprites, spriteColours);
        }

        public override void Show()
        {
            base.Show();
            this.CanvasGroup.alpha = 1f;
        }

        public void Show(Sprite replacement)
        {
            this.Show();
            this.DragObject.sprite = replacement;
            this.DragObject.gameObject.SetActive(!(replacement is null));
        }

        public void Reset()
        {
            this.DragObject.sprite = null;
            this.DragObject.gameObject.SetActive(false);
        }
    }
}
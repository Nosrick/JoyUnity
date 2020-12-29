using UnityEngine;
using UnityEngine.UI;

namespace JoyLib.Code.Unity.GUI
{
    public class DynamicResize : MonoBehaviour
    {
        [SerializeField] protected bool ResizeThis = false;
        [SerializeField] protected bool ResizeChild = true;
        [SerializeField] protected LayoutGroup ChildLayoutGroup;
        [SerializeField] protected ScrollRect ScrollRect;
        [SerializeField] protected bool ResizeHorizontal = false;
        [SerializeField] protected bool ResizeVertical = false;
        
        protected RectTransform ChildRectTransform { get; set; }
        protected RectTransform ItemTransform { get; set; }
        protected RectTransform MyRectTransform { get; set; }
        protected int ChildLastChildren { get; set; }
        
        protected Vector2 OriginalSize { get; set; }
        protected Vector2 LastChildSize { get; set; }
        
        protected void Awake()
        {
            this.MyRectTransform = this.GetComponent<RectTransform>();
            this.OriginalSize = this.MyRectTransform.sizeDelta;
            if (ChildLayoutGroup is null == false)
            {
                ChildRectTransform = ChildLayoutGroup.GetComponent<RectTransform>();
                
                if (ChildLayoutGroup.transform.childCount > 0)
                {
                    ItemTransform = this.transform.GetChild(0).GetComponent<RectTransform>();
                }
            }
        }

        public void Update()
        {
            if (ItemTransform is null 
                && ChildLayoutGroup is null == false 
                && ChildLayoutGroup.transform.childCount > 0)
            {
                ItemTransform = ChildLayoutGroup.transform.GetChild(0).GetComponent<RectTransform>();
            }

            this.Resize();
        }

        protected void Resize()
        {
            if (this.ResizeChild 
                && (ChildRectTransform is null || this.ChildRectTransform.sizeDelta == this.LastChildSize) 
                || ChildLastChildren == this.ChildRectTransform.transform.childCount
                || ItemTransform is null)
            {
                return;
            }
            
            float width, height;
            if (ChildLayoutGroup is VerticalLayoutGroup vertical)
            {
                width = ChildRectTransform.rect.width;
                height = vertical.transform.childCount *
                         (vertical.spacing
                         + vertical.padding.bottom
                         + vertical.padding.top
                         + ItemTransform.rect.height);
            }
            else if (ChildLayoutGroup is HorizontalLayoutGroup horizontal)
            {
                width = horizontal.transform.childCount *
                        (horizontal.spacing
                         + horizontal.padding.left
                         + horizontal.padding.right
                         + ItemTransform.rect.width);
                height = ChildRectTransform.rect.height;
            }
            else
            {
                width = ChildRectTransform.rect.width;
                height = ChildRectTransform.rect.height;
            }


            if (this.ResizeHorizontal)
            {
                if (this.ResizeThis)
                {
                    switch (this.ResizeChild)
                    {
                        case true when this.MyRectTransform.sizeDelta.x != this.LastChildSize.x:
                            this.MyRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, this.LastChildSize.x);
                            break;
                        case true:
                            this.MyRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
                            break;
                    }
                }
                if (this.ResizeChild)
                {
                    this.ChildRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
                }
            }

            if (this.ResizeVertical)
            {
                if (this.ResizeThis)
                {
                    switch (this.ResizeChild)
                    {
                        case true when this.MyRectTransform.sizeDelta.y != this.LastChildSize.y:
                            this.MyRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, this.LastChildSize.y);
                            break;
                        case true:
                            this.MyRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
                            break;
                    }
                }
                if (this.ResizeChild)
                {
                    this.ChildRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
                }
            }

            this.ChildLastChildren = this.ChildRectTransform.transform.childCount;
            this.LastChildSize = this.ChildRectTransform.sizeDelta;
            
            if (this.ScrollRect is null)
            {
                return;
            }
            if (this.ScrollRect.horizontalScrollbar is null == false)
            {
                this.ScrollRect.horizontalScrollbar.numberOfSteps = this.ChildLayoutGroup.transform.childCount;
            }

            if (this.ScrollRect.verticalScrollbar is null == false)
            {
                this.ScrollRect.verticalScrollbar.numberOfSteps = this.ChildLayoutGroup.transform.childCount;
            }
        }
    }
}
using UnityEngine;
using UnityEngine.UI;

namespace JoyLib.Code.Unity.GUI
{
    public class DynamicResize : MonoBehaviour
    {
        [SerializeField] protected bool ResizeChild = true;
        [SerializeField] protected LayoutGroup ChildLayoutGroup;
        [SerializeField] protected ScrollRect ScrollRect;
        
        protected RectTransform ChildRectTransform { get; set; }
        protected RectTransform ItemTransform { get; set; }
        protected Vector2 ChildLastSizes { get; set; }
        
        protected void Awake()
        {
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

            if (ResizeChild && ChildLayoutGroup is null == false)
            {
                Resize();
            }
        }

        protected void Resize()
        {
            if (ChildRectTransform is null 
                || ChildLastSizes == ChildRectTransform.rect.size
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
            
            
            ChildRectTransform.SetSizeWithCurrentAnchors(UnityEngine.RectTransform.Axis.Horizontal, width);
            ChildRectTransform.SetSizeWithCurrentAnchors(UnityEngine.RectTransform.Axis.Vertical, height);

            ChildLastSizes = ChildRectTransform.rect.size;
            if (ScrollRect.horizontalScrollbar is null == false)
            {
                ScrollRect.horizontalScrollbar.numberOfSteps = ChildLayoutGroup.transform.childCount;
            }

            if (ScrollRect.verticalScrollbar is null == false)
            {
                ScrollRect.verticalScrollbar.numberOfSteps = ChildLayoutGroup.transform.childCount;
            }
        }
    }
}
using UnityEngine;

namespace JoyLib.Code.Unity.GUI
{
    public class DynamicAnchoring : MonoBehaviour
    {
        [SerializeField] protected RectTransform.Axis Direction;

        protected RectTransform RectTransform { get; set; }
        
        protected int LastChildCount { get; set; }
        
        public void OnEnable()
        {
            this.RectTransform = this.GetComponent<RectTransform>();
            this.LastChildCount = 0;
        }

        public void Update()
        {
            if (this.LastChildCount != this.transform.childCount)
            {
                this.ReAnchor();
            }

            this.LastChildCount = this.transform.childCount;
        }

        protected void ReAnchor()
        {
            for (int i = 0; i < this.transform.childCount; i++)
            {
                RectTransform child = this.transform.GetChild(i).GetComponent<RectTransform>();
                
                float left = 0;
                float top = 0;
                float right = 1;
                float bottom = 1;
                
                if (this.Direction == RectTransform.Axis.Horizontal)
                {
                    top = this.RectTransform.rect.yMin / this.RectTransform.rect.height;
                    bottom = this.RectTransform.rect.yMax / this.RectTransform.rect.height;

                    left = (i * child.rect.width) / this.RectTransform.rect.width;
                    right = (i * child.rect.width + child.rect.width) /
                            this.RectTransform.rect.width;
                }
                else if (this.Direction == RectTransform.Axis.Vertical)
                {
                    Debug.Log(this.RectTransform.rect);
                    //Debug.Log("xMax: " + this.RectTransform.rect.);
                    Debug.Log("Width: " + this.RectTransform.rect.width);
                    
                    left = this.RectTransform.rect.xMin / this.RectTransform.rect.width;
                    right = this.RectTransform.rect.xMax / this.RectTransform.rect.width;

                    top = (i * child.rect.height) / this.RectTransform.rect.height;
                    bottom = (i * child.rect.height + child.rect.height) /
                             this.RectTransform.rect.height;
                }

                child.anchorMin = new Vector2(left, top);
                child.anchorMax = new Vector2(right, bottom);
                child.anchoredPosition = Vector2.zero;
            }
        }
    }
}
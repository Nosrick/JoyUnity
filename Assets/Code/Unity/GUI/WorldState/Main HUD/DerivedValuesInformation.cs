using System.Collections.Generic;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Events;
using UnityEngine;
using UnityEngine.UI;

namespace JoyLib.Code.Unity.GUI
{
    public class DerivedValuesInformation : MonoBehaviour
    {
        [SerializeField] protected DerivedValueBarContainer DerivedValuePrefab;
        [SerializeField] protected Transform m_Container;

        protected Dictionary<string, DerivedValueBarContainer> Items { get; set; }
        protected IEntity Player { get; set; }
        
        protected RectTransform RectTransform { get; set; }       
        protected IGameManager GameManager { get; set; }
        
        public void OnEnable()
        {
            if (this.GameManager is null)
            {
                this.GameManager = GlobalConstants.GameManager;
            }

            if (this.Items is null)
            {
                this.Items = new Dictionary<string, DerivedValueBarContainer>();
            }

            if (this.GameManager.Player is null)
            {
                return;
            }

            this.RectTransform = this.GetComponent<RectTransform>();
            
            this.Player = this.GameManager.Player;
            this.Player.OnDerivedValueChange -= this.DerivedValueChange;
            this.Player.OnDerivedValueChange += this.DerivedValueChange;
            this.Player.OnMaximumChange -= this.DerivedValueMaximumChange;
            this.Player.OnMaximumChange += this.DerivedValueMaximumChange;
            this.SetUpDerivedValues(this.Player.DerivedValues);
        }

        protected void DerivedValueChange(object sender, ValueChangedEventArgs args)
        {
            this.Items[args.Name].Value = args.NewValue;
        }

        protected void DerivedValueMaximumChange(object sender, ValueChangedEventArgs args)
        {
            this.Items[args.Name].Maximum = args.NewValue;
        }

        public void SetUpDerivedValues(IDictionary<string, IDerivedValue> values)
        {
            List<IDerivedValue> valueList = new List<IDerivedValue>(values.Values);
            
            if (this.Items.Count < valueList.Count)
            {
                for (int i = this.Items.Count; i < valueList.Count; i++)
                {
                    DerivedValueBarContainer newItem =
                        Instantiate(this.DerivedValuePrefab, this.m_Container).GetComponent<DerivedValueBarContainer>();
                    newItem.gameObject.SetActive(true);
                    newItem.Initialise();
                    this.Items.Add(valueList[i].Name, newItem);
                }
            }
            
            for(int i = 0; i < valueList.Count; i++)
            {
                this.Items[valueList[i].Name].Name = valueList[i].Name;
                
                this.Items[valueList[i].Name].BarColour =
                    this.GameManager.DerivedValueHandler.GetBackgroundColour(valueList[i].Name);
                
                this.Items[valueList[i].Name].TextColour =
                    this.GameManager.DerivedValueHandler.GetTextColour(valueList[i].Name);

                this.Items[valueList[i].Name].OutlineColour =
                    this.GameManager.DerivedValueHandler.GetOutlineColour(valueList[i].Name);
                
                this.Items[valueList[i].Name].DirectValueSet(valueList[i].Value);
                this.Items[valueList[i].Name].Minimum = -valueList[i].Maximum;
                this.Items[valueList[i].Name].Maximum = valueList[i].Maximum;
            }
            
            this.ResizeMe();
        }

        protected void ResizeMe()
        {
            VerticalLayoutGroup layoutGroup = this.m_Container.GetComponent<VerticalLayoutGroup>();
            RectTransform childRect = this.DerivedValuePrefab.GetComponent<RectTransform>();
            float height = this.m_Container.childCount *
                           (childRect.rect.height
                            + layoutGroup.padding.top
                            + layoutGroup.padding.bottom
                            + layoutGroup.spacing);

            float width = childRect.rect.width + layoutGroup.padding.left + layoutGroup.padding.right;
            
            this.RectTransform.anchorMin = new Vector2(1.0f - width / Screen.width, 0);
            this.RectTransform.anchorMax = new Vector2(1, height / Screen.height);
            
            this.RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            this.RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        }
    }
}
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
        
        public RectTransform RectTransform { get; protected set; }       
        protected IGameManager GameManager { get; set; }
        
        public bool Initialised { get; protected set; }

        public void FixedUpdate()
        {
            this.Initialise();
        }

        protected void Initialise()
        {
            if (this.Initialised)
            {
                return;
            }
            
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

            this.Initialised = true;
        }

        protected void DerivedValueChange(object sender, ValueChangedEventArgs<int> args)
        {
            this.Items[args.Name].Value = args.NewValue;
        }

        protected void DerivedValueMaximumChange(object sender, ValueChangedEventArgs<int> args)
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

            float flexibleHeight = 1f / valueList.Count;
            
            for(int i = 0; i < valueList.Count; i++)
            {
                var item = this.Items[valueList[i].Name];
                
                item.Name = valueList[i].Name;
                
                item.BarColour =
                    this.GameManager.DerivedValueHandler.GetBackgroundColour(valueList[i].Name);
                
                item.TextColour =
                    this.GameManager.DerivedValueHandler.GetTextColour(valueList[i].Name);

                item.OutlineColour =
                    this.GameManager.DerivedValueHandler.GetOutlineColour(valueList[i].Name);
                
                item.DirectValueSet(valueList[i].Value);
                item.Minimum = -valueList[i].Maximum;
                item.Maximum = valueList[i].Maximum;
                item.GetComponent<LayoutElement>().flexibleHeight = flexibleHeight;
            }
            
            this.ResizeMe();
        }

        protected void ResizeMe()
        {
            float height = this.m_Container.childCount * 0.055f;

            float width = 0.25f;

            this.RectTransform.anchorMin = new Vector2(1.0f - width, 0);
            this.RectTransform.anchorMax = new Vector2(1, height);
        }
    }
}
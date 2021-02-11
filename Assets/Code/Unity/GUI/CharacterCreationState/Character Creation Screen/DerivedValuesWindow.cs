using System.Collections.Generic;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Events;
using TMPro;
using UnityEngine;

namespace JoyLib.Code.Unity.GUI
{
    public class DerivedValuesWindow : ValueContainer
    {
        [SerializeField] protected NamedItem DerivedValuePrefab;
        [SerializeField] protected TextMeshProUGUI m_PointsRemainingText;
        
        protected List<NamedItem> Items { get; set; }

        public void Initialise()
        {
            base.OnEnable();
            this.DerivedValuePrefab.gameObject.SetActive(false);
            if (this.Items is null)
            {
                this.Items = new List<NamedItem>();
            }

            this.Value = this.Maximum;
            this.SetRemainingPointsText();
        }

        public void SetDerivedValues(IDictionary<string, IDerivedValue> values)
        {
            this.Value = this.Maximum;
            
            List<IDerivedValue> valueList = new List<IDerivedValue>(values.Values);
            
            if (this.Items.Count < valueList.Count)
            {
                for (int i = this.Items.Count; i < valueList.Count; i++)
                {
                    NamedItem newItem =
                        Instantiate(this.DerivedValuePrefab, this.transform).GetComponent<NamedItem>();
                    newItem.gameObject.SetActive(true);
                    newItem.Parent = this;
                    this.Items.Add(newItem);
                }
            }
            
            for(int i = 0; i < valueList.Count; i++)
            {
                this.Items[i].Name = valueList[i].Name;
                this.Items[i].ValueChanged -= this.ChangeDerivedValue;
                this.Items[i].DirectValueSet(valueList[i].Value);
                this.Items[i].Minimum = valueList[i].Value;
                this.Items[i].Maximum = valueList[i].Value + 5;
                this.Items[i].ValueChanged += this.ChangeDerivedValue;
                this.Items[i].gameObject.GetComponent<RectTransform>()
                    .SetSizeWithCurrentAnchors(
                        RectTransform.Axis.Horizontal, 
                        this.GetComponent<RectTransform>().rect.width);
            }

            this.SetRemainingPointsText();
        }

        public IDictionary<string, IDerivedValue> GetDerivedValues()
        {
            IDictionary<string, IDerivedValue> values = new Dictionary<string, IDerivedValue>();

            foreach (NamedItem item in this.Items)
            {
                values.Add(
                    item.Name.ToLower(),
                    new ConcreteDerivedIntValue(
                        item.Name.ToLower(),
                        item.Minimum,
                        item.Value - item.Minimum,
                        item.Value));
            }

            return values;
        }

        protected void ChangeDerivedValue(object sender, ValueChangedEventArgs args)
        {
            this.Value -= args.Delta;
            this.SetRemainingPointsText();
        }

        protected void SetRemainingPointsText()
        {
            this.m_PointsRemainingText.text = "Derived Value Points Remaining: " + this.Value;
        }
    }
}
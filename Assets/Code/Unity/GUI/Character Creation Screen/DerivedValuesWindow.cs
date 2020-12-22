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

        public override void OnEnable()
        {
            base.OnEnable();
            DerivedValuePrefab.gameObject.SetActive(false);
            if (Items is null)
            {
                Items = new List<NamedItem>();
            }

            Value = Maximum;
            SetRemainingPointsText();
        }

        public void SetDerivedValues(IDictionary<string, IDerivedValue> values)
        {
            Value = Maximum;
            
            List<IDerivedValue> valueList = new List<IDerivedValue>(values.Values);
            
            if (Items.Count < valueList.Count)
            {
                for (int i = Items.Count; i < valueList.Count; i++)
                {
                    NamedItem newItem =
                        GameObject.Instantiate(DerivedValuePrefab, this.transform).GetComponent<NamedItem>();
                    newItem.gameObject.SetActive(true);
                    Items.Add(newItem);
                }
            }
            
            for(int i = 0; i < valueList.Count; i++)
            {
                Items[i].Name = valueList[i].Name;
                Items[i].ValueChanged -= ChangeDerivedValue;
                Items[i].DirectValueSet(valueList[i].Value);
                Items[i].Minimum = valueList[i].Value;
                Items[i].Maximum = valueList[i].Value + 5;
                Items[i].ValueChanged += ChangeDerivedValue;
                Items[i].gameObject.GetComponent<RectTransform>()
                    .SetSizeWithCurrentAnchors(
                        RectTransform.Axis.Horizontal, 
                        this.GetComponent<RectTransform>().rect.width);
            }
            
            SetRemainingPointsText();
        }

        public IDictionary<string, IDerivedValue> GetDerivedValues()
        {
            IDictionary<string, IDerivedValue> values = new Dictionary<string, IDerivedValue>();

            foreach (NamedItem item in Items)
            {
                values.Add(
                    item.Name.ToLower(),
                    new ConcreteDerivedIntValue(
                        item.Name.ToLower(),
                        item.Value,
                        item.Value));
            }

            return values;
        }

        protected void ChangeDerivedValue(object sender, ValueChangedEventArgs args)
        {
            Value -= args.Delta;
            SetRemainingPointsText();
        }

        protected void SetRemainingPointsText()
        {
            m_PointsRemainingText.text = "Derived Value Points Remaining: " + Value;
        }
    }
}
using System.Collections.Generic;
using DevionGames.UIWidgets;
using JoyLib.Code.Collections;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Statistics;
using UnityEngine;

namespace JoyLib.Code.Unity.GUI
{
    public class DerivedValuesInformation : UIWidget
    {
        [SerializeField] protected DerivedValueBarContainer DerivedValuePrefab;
        [SerializeField] protected GameManager GameManager;

        protected List<DerivedValueBarContainer> Items { get; set; }
        protected IEntity Player { get; set; }

        public void OnEnable()
        {
            if (this.GameManager is null)
            {
                return;
            }

            if (this.Items is null)
            {
                this.Items = new List<DerivedValueBarContainer>();
            }

            this.Player = this.GameManager.Player;
        }

        public void SetUpDerivedValues(BasicValueContainer<IDerivedValue> values)
        {
            List<IDerivedValue> valueList = new List<IDerivedValue>(values.Values);
            
            if (this.Items.Count < valueList.Count)
            {
                for (int i = this.Items.Count; i < valueList.Count; i++)
                {
                    DerivedValueBarContainer newItem =
                        Instantiate(this.DerivedValuePrefab, this.transform).GetComponent<DerivedValueBarContainer>();
                    newItem.gameObject.SetActive(true);
                    this.Items.Add(newItem);
                }
            }
            
            for(int i = 0; i < valueList.Count; i++)
            {
                this.Items[i].Name = valueList[i].Name;
                this.Items[i].DirectValueSet(valueList[i].Value);
                this.Items[i].Minimum = -valueList[i].Maximum;
                this.Items[i].Maximum = valueList[i].Maximum;
            }
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using JoyLib.Code.Entities.Statistics;
using UnityEngine;

namespace JoyLib.Code.Unity.GUI.Character_Sheet
{
    public class StaticValueDisplay : MonoBehaviour
    {
        [SerializeField] protected StaticValueContainer ValueContainerPrefab;
        
        protected List<StaticValueContainer> Items { get; set; }

        public void OnEnable()
        {
            if (this.Items is null)
            {
                this.Items = new List<StaticValueContainer>();
            }
        }

        public virtual void SetValues(IEnumerable<IRollableValue<int>> values)
        {
            List<IRollableValue<int>> listValues = values.ToList();
            
            foreach (StaticValueContainer item in this.Items)
            {
                item.gameObject.SetActive(false);
            }
            
            if (this.Items.Count < listValues.Count)
            {
                for (int i = this.Items.Count; i < listValues.Count; i++)
                {
                    StaticValueContainer newItem =
                        Instantiate(this.ValueContainerPrefab, this.transform).GetComponent<StaticValueContainer>();
                    newItem.gameObject.SetActive(true);
                    this.Items.Add(newItem);
                }
            }
            
            for(int i = 0; i < listValues.Count; i++)
            {
                this.Items[i].Name = listValues[i].Name;
                this.Items[i].DirectValueSet(listValues[i].Value);
                this.Items[i].gameObject.SetActive(true);
            }
        }
    }
}
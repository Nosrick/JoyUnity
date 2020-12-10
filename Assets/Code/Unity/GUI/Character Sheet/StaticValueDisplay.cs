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
            if (Items is null)
            {
                Items = new List<StaticValueContainer>();
            }
        }

        public void SetValues(IEnumerable<IBasicValue> values)
        {
            List<IBasicValue> listValues = values.ToList();
            
            foreach (StaticValueContainer item in Items)
            {
                item.gameObject.SetActive(false);
            }
            
            if (Items.Count < listValues.Count)
            {
                for (int i = Items.Count; i < listValues.Count; i++)
                {
                    StaticValueContainer newItem =
                        GameObject.Instantiate(ValueContainerPrefab, this.transform).GetComponent<StaticValueContainer>();
                    newItem.gameObject.SetActive(true);
                    Items.Add(newItem);
                }
            }
            
            for(int i = 0; i < listValues.Count; i++)
            {
                Items[i].Name = listValues[i].Name;
                Items[i].DirectValueSet(listValues[i].Value);
                Items[i].gameObject.SetActive(true);
            }
        }
    }
}
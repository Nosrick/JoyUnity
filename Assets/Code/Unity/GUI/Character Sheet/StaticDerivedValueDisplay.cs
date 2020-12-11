using System.Collections.Generic;
using System.Linq;
using JoyLib.Code.Entities.Statistics;
using UnityEngine;

namespace JoyLib.Code.Unity.GUI.Character_Sheet
{
    public class StaticDerivedValueDisplay : MonoBehaviour
    {
        [SerializeField] protected DerivedValueContainer ValueContainerPrefab;
        
        protected List<DerivedValueContainer> Items { get; set; }

        public void OnEnable()
        {
            if (Items is null)
            {
                Items = new List<DerivedValueContainer>();
            }
        }

        public void SetValues(IDictionary<string, IDerivedValue<int>> values)
        {
            List<IDerivedValue<int>> listValues = values.Values.ToList();
            
            foreach (DerivedValueContainer item in Items)
            {
                item.gameObject.SetActive(false);
            }
            
            if (Items.Count < listValues.Count)
            {
                for (int i = Items.Count; i < listValues.Count; i++)
                {
                    DerivedValueContainer newItem =
                        GameObject.Instantiate(ValueContainerPrefab, this.transform).GetComponent<DerivedValueContainer>();
                    newItem.gameObject.SetActive(true);
                    Items.Add(newItem);
                }
            }
            
            for(int i = 0; i < listValues.Count; i++)
            {
                Items[i].Name = listValues[i].Name;
                Items[i].DirectValueSet(listValues[i].Value);
                Items[i].Maximum = listValues[i].Maximum;
                Items[i].gameObject.SetActive(true);
            }
        }
    }
}
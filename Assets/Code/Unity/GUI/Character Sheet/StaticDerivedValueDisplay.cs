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
            if (this.Items is null)
            {
                this.Items = new List<DerivedValueContainer>();
            }
        }

        public void SetValues(IDictionary<string, IDerivedValue> values)
        {
            List<IDerivedValue> listValues = values.Values.ToList();
            
            foreach (DerivedValueContainer item in this.Items)
            {
                item.gameObject.SetActive(false);
            }
            
            if (this.Items.Count < listValues.Count)
            {
                for (int i = this.Items.Count; i < listValues.Count; i++)
                {
                    DerivedValueContainer newItem =
                        Instantiate(this.ValueContainerPrefab, this.transform).GetComponent<DerivedValueContainer>();
                    newItem.gameObject.SetActive(true);
                    this.Items.Add(newItem);
                }
            }
            
            for(int i = 0; i < listValues.Count; i++)
            {
                this.Items[i].Name = listValues[i].Name;
                this.Items[i].DirectValueSet(listValues[i].Value);
                this.Items[i].Maximum = listValues[i].Maximum;
                this.Items[i].gameObject.SetActive(true);
            }
        }
    }
}
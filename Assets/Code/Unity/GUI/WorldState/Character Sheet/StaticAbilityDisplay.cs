using System.Collections.Generic;
using System.Linq;
using JoyLib.Code.Entities.Abilities;
using UnityEngine;

namespace JoyLib.Code.Unity.GUI.Character_Sheet
{
    public class StaticAbilityDisplay : MonoBehaviour
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

        public virtual void SetValues(IEnumerable<IAbility> abilities)
        {
            List<IAbility> listAbilities = abilities.ToList();
            
            foreach (StaticValueContainer item in this.Items)
            {
                item.gameObject.SetActive(false);
            }
            
            if (this.Items.Count < listAbilities.Count)
            {
                for (int i = this.Items.Count; i < listAbilities.Count; i++)
                {
                    StaticValueContainer newItem =
                        Instantiate(this.ValueContainerPrefab, this.transform).GetComponent<StaticValueContainer>();
                    newItem.gameObject.SetActive(true);
                    this.Items.Add(newItem);
                }
            }
            
            for(int i = 0; i < listAbilities.Count; i++)
            {
                this.Items[i].Name = listAbilities[i].Name;
                this.Items[i].Tooltip = listAbilities[i].Description;
                this.Items[i].gameObject.SetActive(true);
            }
        }
    }
}
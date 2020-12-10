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
            if (Items is null)
            {
                Items = new List<StaticValueContainer>();
            }
        }

        public void SetValues(IEnumerable<IAbility> abilities)
        {
            List<IAbility> listAbilities = abilities.ToList();
            
            foreach (StaticValueContainer item in Items)
            {
                item.gameObject.SetActive(false);
            }
            
            if (Items.Count < listAbilities.Count)
            {
                for (int i = Items.Count; i < listAbilities.Count; i++)
                {
                    StaticValueContainer newItem =
                        GameObject.Instantiate(ValueContainerPrefab, this.transform).GetComponent<StaticValueContainer>();
                    newItem.gameObject.SetActive(true);
                    Items.Add(newItem);
                }
            }
            
            for(int i = 0; i < listAbilities.Count; i++)
            {
                Items[i].Name = listAbilities[i].Name;
                Items[i].Tooltip = listAbilities[i].Description;
                Items[i].gameObject.SetActive(true);
            }
        }
    }
}
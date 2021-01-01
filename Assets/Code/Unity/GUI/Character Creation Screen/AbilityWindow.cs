using System.Collections.Generic;
using System.Linq;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Abilities;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Events;
using TMPro;
using UnityEngine;

namespace JoyLib.Code.Unity.GUI
{
    public class AbilityWindow : ValueContainer
    {
        [SerializeField] protected GameManager GameManager;
        [SerializeField] protected TextMeshProUGUI PointsRemainingText;
        [SerializeField] protected AbilityItem AbilityItemPrefab;
        [SerializeField] protected BasicPlayerInfo PlayerInfo;
        [SerializeField] protected StatisticWindow StatisticWindow;
        [SerializeField] protected SkillWindow SkillWindow;
        
        protected List<AbilityItem> Items { get; set; }

        public void Initialise()
        {
            base.OnEnable();
            Items = new List<AbilityItem>();
            Value = Maximum;
            PointsRemainingText.text = "Picks Remaining: " + Value;
            SkillWindow.ValueChanged += GetAvailableAbilities;
        }

        public void GetAvailableAbilities(object sender, ValueChangedEventArgs args)
        {
            GetAvailableAbilities(
                PlayerInfo.CurrentTemplate,
                StatisticWindow.GetStatistics(),
                SkillWindow.GetSkillsBlock());
        }

        public IEnumerable<IAbility> GetPickedAbilities()
        {
            List<IAbility> abilities = new List<IAbility>();
            IEnumerable<string> names = Items.Where(item => item.Selected).Select(item => item.Name);
            foreach (string name in names)
            {
                abilities.Add(GameManager.AbilityHandler.GetAbility(name));
            }

            return abilities;
        }

        public void ChangePicksRemaining(object sender, ValueChangedEventArgs args)
        {
            Value -= args.Delta;
            PointsRemainingText.text = "Picks Remaining: " + Value;
        }

        public void GetAvailableAbilities(
            IEntityTemplate template,
            IDictionary<string, IRollableValue<int>> stats,
            IDictionary<string, IEntitySkill> skills)
        {
            if (GameManager.AbilityHandler is null)
            {
                return;
            }
            
            List<IAbility> abilities = GameManager.AbilityHandler.GetAvailableAbilities(
                                                template,
                                                stats,
                                                skills)
                                            .ToList();

            if (Items.Count < abilities.Count)
            {
                for (int i = Items.Count; i < abilities.Count(); i++)
                {
                    AbilityItem newItem =
                        GameObject.Instantiate(AbilityItemPrefab, this.transform).GetComponent<AbilityItem>();
                    newItem.gameObject.SetActive(true);
                    Items.Add(newItem);
                }
            }
            
            for(int i = 0; i < abilities.Count; i++)
            {
                Items[i].Name = abilities[i].Name;
                Items[i].OnSelect -= ChangePicksRemaining;
                Items[i].Tooltip = abilities[i].Description;
                Items[i].OnSelect += ChangePicksRemaining;
            }
        }
    }
}
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
        [SerializeField] protected TextMeshProUGUI PointsRemainingText;
        [SerializeField] protected AbilityItem AbilityItemPrefab;
        [SerializeField] protected BasicPlayerInfo PlayerInfo;
        [SerializeField] protected StatisticWindow StatisticWindow;
        [SerializeField] protected SkillWindow SkillWindow;
        
        protected List<AbilityItem> Items { get; set; }        
        protected IGameManager GameManager { get; set; }
        
        public void Initialise()
        {
            this.GameManager = GlobalConstants.GameManager;
            base.OnEnable();
            this.Items = new List<AbilityItem>();
            this.Value = this.Maximum;
            this.PointsRemainingText.text = "Picks Remaining: " + this.Value;
            this.SkillWindow.ValueChanged += this.GetAvailableAbilities;
        }

        public void GetAvailableAbilities(object sender, ValueChangedEventArgs args)
        {
            this.GetAvailableAbilities(this.PlayerInfo.CurrentTemplate, this.StatisticWindow.GetStatistics(), this.SkillWindow.GetSkillsBlock());
        }

        public IEnumerable<IAbility> GetPickedAbilities()
        {
            List<IAbility> abilities = new List<IAbility>();
            IEnumerable<string> names = this.Items.Where(item => item.Selected).Select(item => item.Name);
            foreach (string name in names)
            {
                abilities.Add(this.GameManager.AbilityHandler.GetAbility(name));
            }

            return abilities;
        }

        public void ChangePicksRemaining(object sender, ValueChangedEventArgs args)
        {
            this.Value -= args.Delta;
            this.PointsRemainingText.text = "Picks Remaining: " + this.Value;
        }

        public void GetAvailableAbilities(
            IEntityTemplate template,
            IDictionary<string, IRollableValue<int>> stats,
            IDictionary<string, IEntitySkill> skills)
        {
            if (this.GameManager.AbilityHandler is null)
            {
                return;
            }
            
            List<IAbility> abilities = this.GameManager.AbilityHandler.GetAvailableAbilities(
                                                template,
                                                stats,
                                                skills)
                                            .ToList();

            if (this.Items.Count < abilities.Count)
            {
                for (int i = this.Items.Count; i < abilities.Count(); i++)
                {
                    AbilityItem newItem =
                        Instantiate(this.AbilityItemPrefab, this.transform).GetComponent<AbilityItem>();
                    newItem.gameObject.SetActive(true);
                    newItem.Parent = this;
                    newItem.Awake();
                    this.Items.Add(newItem);
                }
            }
            
            for(int i = 0; i < abilities.Count; i++)
            {
                this.Items[i].Name = abilities[i].Name;
                this.Items[i].OnSelect -= this.ChangePicksRemaining;
                this.Items[i].Tooltip = abilities[i].Description;
                this.Items[i].OnSelect += this.ChangePicksRemaining;
            }
        }
    }
}
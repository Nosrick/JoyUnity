using System;
using System.Collections.Generic;
using System.Linq;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Abilities;
using JoyLib.Code.Entities.Jobs;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Events;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JoyLib.Code.Unity.GUI.Job_Management_Screen
{
    public class JobManagementScreen : ValueContainer
    {
        [SerializeField] protected GameManager GameManager;
        [SerializeField] protected TextMeshProUGUI ExperienceRemaining;
        [SerializeField] protected StatisticItem StatisticItemPrefab;
        [SerializeField] protected AbilityItem AbilityItemPrefab;
        [SerializeField] protected Image PlayerSprite;
        [SerializeField] protected TextMeshProUGUI PlayerName;
        [SerializeField] protected ConstrainedValueContainer JobSelection;

        [SerializeField] protected GameObject StatisticsPanel;
        [SerializeField] protected GameObject SkillsPanel;
        [SerializeField] protected GameObject AbilitiesPanel;
        
        protected List<StatisticItem> Statistics { get; set; }
        protected List<StatisticItem> Skills { get; set; }
        protected List<AbilityItem> Abilities { get; set; }

        protected IEntity Player;
        protected IJob CurrentJob; 
        
        protected Dictionary<IAbility, int> PurchasedAbilities { get; set; }
        protected Dictionary<string, int> PurchasedStatistics { get; set; }
        protected Dictionary<string, int> PurchasedSkills { get; set; }

        public void Awake()
        {
            if (GameManager.Player is null)
            {
                return;
            }
            
            Player = GameManager.Player;
            CurrentJob = Player.CurrentJob;
            Value = CurrentJob.Experience;
            PlayerSprite.sprite = Player.Sprite;
            PlayerName.text = Player.JoyName;
            JobSelection.Container = Player.Cultures.SelectMany(culture => culture.Jobs)
                .Distinct()
                .ToList();
            JobSelection.Value = JobSelection.Container.IndexOf(CurrentJob.Name);
            SetUp();
        }

        protected void OnValueChange(object sender, ValueChangedEventArgs args)
        {
            Value -= args.Delta;
        }

        protected void ChangeJob(object sender, ValueChangedEventArgs args)
        {
            CurrentJob = Player.Jobs.First(job =>
                job.Name.Equals(JobSelection.Selected, StringComparison.OrdinalIgnoreCase));
            Value = CurrentJob.Experience;
            SetUp();
        }

        protected void SetExperienceText()
        {
            ExperienceRemaining.text = "Experience Remaining: " + Value;
        }

        protected void SetUp()
        {
            PurchasedAbilities = new Dictionary<IAbility, int>();
            PurchasedSkills = new Dictionary<string, int>();
            PurchasedStatistics = new Dictionary<string, int>();
            
            SetUpStatistics();
            SetUpSkills();
            SetUpAbilities();
        }

        protected void SetUpStatistics()
        {
            List<IRollableValue> stats = Player.Statistics.Values.ToList();

            if (Statistics.Count < stats.Count)
            {
                for (int i = Statistics.Count; i < stats.Count(); i++)
                {
                    StatisticItem newItem =
                        GameObject.Instantiate(StatisticItemPrefab, StatisticsPanel.transform).GetComponent<StatisticItem>();
                    newItem.gameObject.SetActive(true);
                    Statistics.Add(newItem);
                }
            }
        
            for(int i = 0; i < stats.Count; i++)
            {
                Statistics[i].Name = stats[i].Name;
                Statistics[i].ValueChanged -= OnValueChange;
                Statistics[i].DirectValueSet(stats[i].Value);
                Statistics[i].ValueChanged += OnValueChange;
                Statistics[i].Tooltip = "Cost: " + Math.Max(1, stats[i].Value - CurrentJob.GetStatisticDiscount(stats[i].Name));
            }
        }

        protected void SetUpSkills()
        {
            List<IGrowingValue> skills = Player.Skills.Values.ToList();

            if (Skills.Count < skills.Count)
            {
                for (int i = Skills.Count; i < skills.Count(); i++)
                {
                    StatisticItem newItem =
                        GameObject.Instantiate(StatisticItemPrefab, SkillsPanel.transform)
                            .GetComponent<StatisticItem>();
                    newItem.gameObject.SetActive(true);
                    Skills.Add(newItem);
                }
            }

            for (int i = 0; i < skills.Count; i++)
            {
                Skills[i].Name = skills[i].Name;
                Skills[i].ValueChanged -= OnValueChange;
                Skills[i].DirectValueSet(skills[i].Value);
                Skills[i].ValueChanged += OnValueChange;
                Skills[i].Tooltip = "Cost: " + Math.Max(1, skills[i].Value - CurrentJob.GetSkillDiscount(skills[i].Name));
            }
        }

        protected void SetUpAbilities()
        {
            List<Tuple<IAbility, int>> abilities =
                CurrentJob.Abilities.Where(pair => Player.Abilities.Contains(pair.Key) == false)
                    .Select(pair => new Tuple<IAbility, int>(pair.Key, pair.Value))
                    .ToList();

            if (Abilities.Count < abilities.Count)
            {
                for (int i = Abilities.Count; i < abilities.Count; i++)
                {
                    AbilityItem newItem =
                        GameObject.Instantiate(AbilityItemPrefab, AbilitiesPanel.transform)
                            .GetComponent<AbilityItem>();
                    newItem.gameObject.SetActive(true);
                    Abilities.Add(newItem);
                }
            }
            
            for (int i = 0; i < abilities.Count; i++)
            {
                Abilities[i].Name = abilities[i].Item1.Name;
                Abilities[i].Delta = abilities[i].Item2;
                Abilities[i].OnSelect -= OnValueChange;
                Abilities[i].OnSelect += OnValueChange;
                Abilities[i].Tooltip = "Cost: " + abilities[i].Item2;
            }
        }

        public void MakeChanges()
        {
            int totalCost = PurchasedAbilities.Sum(pair => pair.Value)
                            + PurchasedSkills.Sum(pair => pair.Value)
                            + PurchasedStatistics.Sum(pair => pair.Value);
            if (totalCost > CurrentJob.Experience)
            {
                return;
            }
            
            Player.Abilities.AddRange(PurchasedAbilities.Keys);
            foreach (StatisticItem item in Statistics)
            {
                Player.Statistics[item.Name].SetValue(item.Value);
            }

            foreach (StatisticItem item in Skills)
            {
                Player.Skills[item.Name].SetValue(item.Value);
            }
        }

        public override int Value
        {
            get => m_Value;
            set
            {
                base.Value = value;
                SetExperienceText();
            }
        }
    }
}
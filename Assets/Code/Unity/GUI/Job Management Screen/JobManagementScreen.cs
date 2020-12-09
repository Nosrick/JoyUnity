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
        
        protected Dictionary<string, IAbility> PurchasedAbilities { get; set; }
        
        protected List<IRollableValue> OriginalStatistics { get; set; }
        protected List<IGrowingValue> OriginalSkills { get; set; }
        
        protected Dictionary<string, int> StatisticsDeltas { get; set; }
        protected Dictionary<string, int> SkillsDeltas { get; set; }

        public void OnEnable()
        {
            if (GameManager.Player is null)
            {
                return;
            }

            if (Statistics is null)
            {
                Statistics = new List<StatisticItem>();
            }

            if (Skills is null)
            {
                Skills = new List<StatisticItem>();
            }

            if (Abilities is null)
            {
                Abilities = new List<AbilityItem>();
            }
            
            Player = GameManager.Player;
            CurrentJob = Player.CurrentJob;
            Minimum = 0;
            Maximum = CurrentJob.Experience;
            Value = Maximum;
            PlayerSprite.sprite = Player.Sprite;
            PlayerName.text = Player.JoyName;
            JobSelection.Container = Player.Cultures.SelectMany(culture => culture.Jobs)
                .Distinct()
                .ToList();
            JobSelection.Value =
                JobSelection.Container.FindIndex(job =>
                    job.Equals(CurrentJob.Name, StringComparison.OrdinalIgnoreCase));
            OriginalStatistics = Player.Statistics.Values.ToList();
            OriginalSkills = Player.Skills.Values.ToList();
            
            StatisticsDeltas = new Dictionary<string, int>();
            SkillsDeltas = new Dictionary<string, int>();

            foreach (IRollableValue stat in OriginalStatistics)
            {
                StatisticsDeltas.Add(stat.Name, 0);
            }

            foreach (IGrowingValue skill in OriginalSkills)
            {
                SkillsDeltas.Add(skill.Name, 0);
            }
            
            SetUp();
        }

        protected void OnAbilityChange(object sender, ValueChangedEventArgs args)
        {
            Value -= args.Delta;
            if(PurchasedAbilities.ContainsKey(args.Name))
            {
                PurchasedAbilities.Remove(args.Name);
            }
            else
            {
                PurchasedAbilities.Add(args.Name,
                CurrentJob.Abilities.First(ability =>
                    ability.Key.Name.Equals(args.Name, StringComparison.OrdinalIgnoreCase)).Key);
            }
        }

        protected void OnSkillChange(object sender, ValueChangedEventArgs args)
        {
            Value -= args.Delta;
            SkillsDeltas[args.Name] += args.Delta;
            SetUpSkills();
        }

        protected void OnStatisticChange(object senver, ValueChangedEventArgs args)
        {
            Value -= args.Delta;
            StatisticsDeltas[args.Name] += args.Delta;
            SetUpStatistics();
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
            PurchasedAbilities = new Dictionary<string, IAbility>();
            
            SetUpStatistics();
            SetUpSkills();
            SetUpAbilities();
        }

        protected void SetUpStatistics()
        {
            if (Statistics.Count < OriginalStatistics.Count)
            {
                for (int i = Statistics.Count; i < OriginalStatistics.Count(); i++)
                {
                    StatisticItem newItem =
                        GameObject.Instantiate(StatisticItemPrefab, StatisticsPanel.transform).GetComponent<StatisticItem>();
                    newItem.gameObject.SetActive(true);
                    Statistics.Add(newItem);
                }
            }
        
            for(int i = 0; i < OriginalStatistics.Count; i++)
            {
                Statistics[i].Name = OriginalStatistics[i].Name;
                Statistics[i].ValueChanged -= OnStatisticChange;
                Statistics[i].DirectValueSet(OriginalStatistics[i].Value + StatisticsDeltas[OriginalStatistics[i].Name]);
                Statistics[i].ValueChanged += OnStatisticChange;
                Statistics[i].Minimum = OriginalStatistics[i].Value;
                
                Statistics[i].IncreaseDelta =
                    Math.Max(1,
                        1 + OriginalStatistics[i].Value + StatisticsDeltas[OriginalStatistics[i].Name] -
                        CurrentJob.GetStatisticDiscount(OriginalStatistics[i].Name));
                
                Statistics[i].DecreaseDelta = Statistics[i].IncreaseDelta - 2;
                Statistics[i].Tooltip = "Cost: " + Statistics[i].IncreaseDelta;
            }
        }

        protected void SetUpSkills()
        {
            if (Skills.Count < OriginalSkills.Count)
            {
                for (int i = Skills.Count; i < OriginalSkills.Count(); i++)
                {
                    StatisticItem newItem =
                        GameObject.Instantiate(StatisticItemPrefab, SkillsPanel.transform)
                            .GetComponent<StatisticItem>();
                    newItem.gameObject.SetActive(true);
                    Skills.Add(newItem);
                }
            }

            for (int i = 0; i < OriginalSkills.Count; i++)
            {
                Skills[i].Name = OriginalSkills[i].Name;
                Skills[i].ValueChanged -= OnSkillChange;
                Skills[i].DirectValueSet(OriginalSkills[i].Value + SkillsDeltas[OriginalSkills[i].Name]);
                Skills[i].ValueChanged += OnSkillChange;
                Skills[i].Minimum = OriginalSkills[i].Value;
                
                Skills[i].IncreaseDelta = Math.Max(1,
                    1 + OriginalSkills[i].Value + SkillsDeltas[OriginalSkills[i].Name] -
                    CurrentJob.GetSkillDiscount(OriginalSkills[i].Name));
                
                Skills[i].DecreaseDelta = Skills[i].IncreaseDelta - 2;
                Skills[i].Tooltip = "Cost: " + Skills[i].IncreaseDelta;
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
                Abilities[i].OnSelect -= OnAbilityChange;
                Abilities[i].OnSelect += OnAbilityChange;
                Abilities[i].Tooltip = "Cost: " + abilities[i].Item2;
            }
        }

        public void MakeChanges()
        {
            Player.Abilities.AddRange(PurchasedAbilities.Values);
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
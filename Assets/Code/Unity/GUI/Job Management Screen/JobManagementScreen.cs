﻿using System;
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
        [SerializeField] protected GrowingStatisticItem StatisticItemPrefab;
        [SerializeField] protected AbilityItem AbilityItemPrefab;
        [SerializeField] protected Image PlayerSprite;
        [SerializeField] protected TextMeshProUGUI PlayerName;
        [SerializeField] protected ConstrainedValueContainer JobSelection;

        [SerializeField] protected GameObject StatisticsPanel;
        [SerializeField] protected GameObject SkillsPanel;
        [SerializeField] protected GameObject AbilitiesPanel;
        
        protected List<GrowingStatisticItem> Statistics { get; set; }
        protected List<GrowingStatisticItem> Skills { get; set; }
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
                Statistics = new List<GrowingStatisticItem>();
            }

            if (Skills is null)
            {
                Skills = new List<GrowingStatisticItem>();
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
            JobSelection.Container = Player.Jobs.Select(job => job.Name)
                .ToList();
            JobSelection.Value =
                JobSelection.Container.FindIndex(job =>
                    job.Equals(CurrentJob.Name, StringComparison.OrdinalIgnoreCase));
            OriginalStatistics = Player.Statistics.Values.ToList();
            OriginalSkills = Player.Skills.Values.ToList();
            
            StatisticsDeltas = new Dictionary<string, int>();
            SkillsDeltas = new Dictionary<string, int>();
            JobSelection.ValueChanged -= ChangeJob;
            JobSelection.ValueChanged += ChangeJob;

            foreach (IRollableValue stat in OriginalStatistics)
            {
                StatisticsDeltas.Add(stat.Name.ToLower(), stat.Value * 10);
            }

            foreach (IGrowingValue skill in OriginalSkills)
            {
                SkillsDeltas.Add(skill.Name.ToLower(), skill.Value * 10);
            }
            
            SetUp();
        }

        protected void OnAbilityChange(object sender, ValueChangedEventArgs args)
        {
            Value -= args.Delta;
            if(PurchasedAbilities.ContainsKey(args.Name.ToLower()))
            {
                PurchasedAbilities.Remove(args.Name.ToLower());
            }
            else
            {
                KeyValuePair<IAbility, int> ability = CurrentJob.Abilities.First(a =>
                    a.Key.Name.Equals(args.Name, StringComparison.OrdinalIgnoreCase));
                PurchasedAbilities.Add(args.Name.ToLower(), ability.Key);
            }
        }

        protected void OnSkillChange(object sender, ValueChangedEventArgs args)
        {
            Value -= args.Delta;
            
            SetUpSkillDeltas();
        }

        protected void OnStatisticChange(object senver, ValueChangedEventArgs args)
        {
            Value -= args.Delta;
            
            SetUpStatisticDeltas();
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
            foreach (GrowingStatisticItem item in Statistics)
            {
                item.gameObject.SetActive(false);
            }
            
            if (Statistics.Count < OriginalStatistics.Count)
            {
                for (int i = Statistics.Count; i < OriginalStatistics.Count(); i++)
                {
                    GrowingStatisticItem newItem =
                        GameObject.Instantiate(StatisticItemPrefab, StatisticsPanel.transform).GetComponent<GrowingStatisticItem>();
                    newItem.gameObject.SetActive(true);
                    Statistics.Add(newItem);
                }
            }
        
            for(int i = 0; i < OriginalStatistics.Count; i++)
            {
                Statistics[i].gameObject.SetActive(true);
                Statistics[i].Name = OriginalStatistics[i].Name;
                Statistics[i].ValueChanged -= OnStatisticChange;
                Statistics[i].DirectValueSet(OriginalStatistics[i].Value);
                Statistics[i].ValueChanged += OnStatisticChange;
                Statistics[i].Minimum = OriginalStatistics[i].Value;
                SetUpStatisticDeltas();
            }
        }

        protected void SetUpSkills()
        {
            foreach (GrowingStatisticItem item in Skills)
            {
                item.gameObject.SetActive(false);
            }
            
            if (Skills.Count < OriginalSkills.Count)
            {
                for (int i = Skills.Count; i < OriginalSkills.Count(); i++)
                {
                    GrowingStatisticItem newItem =
                        GameObject.Instantiate(StatisticItemPrefab, SkillsPanel.transform)
                            .GetComponent<GrowingStatisticItem>();
                    newItem.gameObject.SetActive(true);
                    Skills.Add(newItem);
                }
            }

            for (int i = 0; i < OriginalSkills.Count; i++)
            {
                Skills[i].gameObject.SetActive(true);
                Skills[i].Name = OriginalSkills[i].Name;
                Skills[i].ValueChanged -= OnSkillChange;
                Skills[i].DirectValueSet(OriginalSkills[i].Value);
                Skills[i].ValueChanged += OnSkillChange;
                Skills[i].Minimum = OriginalSkills[i].Value;
                SetUpSkillDeltas();
            }
        }

        protected void SetUpAbilities()
        {
            List<Tuple<IAbility, int>> abilities =
                CurrentJob.Abilities.Where(pair => Player.Abilities.Contains(pair.Key) == false)
                    .Select(pair => new Tuple<IAbility, int>(pair.Key, pair.Value))
                    .ToList();

            foreach (AbilityItem item in Abilities)
            {
                item.gameObject.SetActive(false);
            }

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
                Abilities[i].gameObject.SetActive(true);
                Abilities[i].Name = abilities[i].Item1.Name;
                Abilities[i].Delta = abilities[i].Item2;
                Abilities[i].OnSelect -= OnAbilityChange;
                Abilities[i].OnSelect += OnAbilityChange;
                Abilities[i].Tooltip = "Cost: " + abilities[i].Item2;
            }
        }

        protected void SetUpSkillDeltas()
        {
            for (int i = 0; i < OriginalSkills.Count; i++)
            {
                int delta = SkillsDeltas[OriginalSkills[i].Name.ToLower()] =
                    (Skills[i].Value * 10) - CurrentJob.GetSkillDiscount(OriginalSkills[i].Name);
                Skills[i].IncreaseCost = delta + 10;
                Skills[i].DecreaseCost = -delta;
                Skills[i].Tooltip = "Cost: " + (delta + 10);
            }
        }

        protected void SetUpStatisticDeltas()
        {
            for (int i = 0; i < OriginalStatistics.Count; i++)
            {
                int delta = StatisticsDeltas[OriginalStatistics[i].Name.ToLower()] =
                    (Statistics[i].Value * 10) - CurrentJob.GetStatisticDiscount(OriginalStatistics[i].Name);
                Statistics[i].IncreaseCost = delta + 10;
                Statistics[i].DecreaseCost = -delta;
                Statistics[i].Tooltip = "Cost: " + (delta + 10);
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
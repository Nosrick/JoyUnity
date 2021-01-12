using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Abilities;
using JoyLib.Code.Entities.Jobs;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Events;
using TMPro;
using UnityEngine;

namespace JoyLib.Code.Unity.GUI.Job_Management_Screen
{
    public class JobManagementScreen : ValueContainer
    {
        [SerializeField] protected GameManager GameManager;
        [SerializeField] protected TextMeshProUGUI ExperienceRemaining;
        [SerializeField] protected GrowingNamedItem namedItemPrefab;
        [SerializeField] protected AbilityItem AbilityItemPrefab;
        [SerializeField] protected ManagedUISprite PlayerSprite;
        [SerializeField] protected TextMeshProUGUI PlayerName;
        [SerializeField] protected ConstrainedValueContainer JobSelection;

        [SerializeField] protected GameObject StatisticsPanel;
        [SerializeField] protected GameObject SkillsPanel;
        [SerializeField] protected GameObject AbilitiesPanel;
        
        protected List<GrowingNamedItem> Statistics { get; set; }
        protected List<GrowingNamedItem> Skills { get; set; }
        protected List<AbilityItem> Abilities { get; set; }

        protected IEntity Player { get; set; }
        protected IJob CurrentJob { get; set; } 
        
        protected Dictionary<string, IAbility> PurchasedAbilities { get; set; }
        
        protected List<IRollableValue<int>> OriginalStatistics { get; set; }
        protected List<IEntitySkill> OriginalSkills { get; set; }
        
        protected Dictionary<string, int> StatisticsDeltas { get; set; }
        protected Dictionary<string, int> SkillsDeltas { get; set; }

        public override void OnEnable()
        {
            if (this.GameManager.Player is null)
            {
                return;
            }

            if (this.Statistics is null)
            {
                this.Statistics = new List<GrowingNamedItem>();
            }

            if (this.Skills is null)
            {
                this.Skills = new List<GrowingNamedItem>();
            }

            if (this.Abilities is null)
            {
                this.Abilities = new List<AbilityItem>();
            }

            this.Player = this.GameManager.Player;
            this.CurrentJob = this.Player.CurrentJob;
            this.Minimum = 0;
            this.Maximum = this.CurrentJob.Experience;
            this.Value = this.Maximum;
            this.PlayerSprite.Clear();
            this.PlayerSprite.AddSpriteState(this.Player.MonoBehaviourHandler.CurrentSpriteState);
            this.PlayerName.text = this.Player.JoyName;
            this.JobSelection.Container = this.Player.Jobs.Select(job => job.Name)
                .ToList();
            this.JobSelection.Value = this.JobSelection.Container.FindIndex(job =>
                    job.Equals(this.CurrentJob.Name, StringComparison.OrdinalIgnoreCase));
            this.OriginalStatistics = this.Player.Statistics.Values.ToList();
            this.OriginalSkills = this.Player.Skills.Values.ToList();

            this.StatisticsDeltas = new Dictionary<string, int>();
            this.SkillsDeltas = new Dictionary<string, int>();
            this.JobSelection.ValueChanged -= this.ChangeJob;
            this.JobSelection.ValueChanged += this.ChangeJob;

            foreach (IRollableValue<int> stat in this.OriginalStatistics)
            {
                this.StatisticsDeltas.Add(stat.Name.ToLower(), stat.Value * 10);
            }

            foreach (IRollableValue<int> skill in this.OriginalSkills)
            {
                this.SkillsDeltas.Add(skill.Name.ToLower(), skill.Value * 10);
            }

            this.SetUp();
        }

        public void OnDisable()
        {
            GUIManager?.CloseGUI(GUINames.TOOLTIP);
        }

        protected void OnAbilityChange(object sender, ValueChangedEventArgs args)
        {
            this.Value -= args.Delta;
            if(this.PurchasedAbilities.ContainsKey(args.Name.ToLower()))
            {
                this.PurchasedAbilities.Remove(args.Name.ToLower());
            }
            else
            {
                KeyValuePair<IAbility, int> ability = this.CurrentJob.Abilities.First(a =>
                    a.Key.Name.Equals(args.Name, StringComparison.OrdinalIgnoreCase));
                this.PurchasedAbilities.Add(args.Name.ToLower(), ability.Key);
            }
        }

        protected void OnSkillChange(object sender, ValueChangedEventArgs args)
        {
            this.Value -= args.Delta;

            this.SetUpSkillDeltas();
        }

        protected void OnStatisticChange(object senver, ValueChangedEventArgs args)
        {
            this.Value -= args.Delta;

            this.SetUpStatisticDeltas();
        }

        protected void ChangeJob(object sender, ValueChangedEventArgs args)
        {
            this.CurrentJob = this.Player.Jobs.First(job =>
                job.Name.Equals(this.JobSelection.Selected, StringComparison.OrdinalIgnoreCase));
            this.Value = this.CurrentJob.Experience;
            this.SetUp();
        }

        protected void SetExperienceText()
        {
            this.ExperienceRemaining.text = "Experience Remaining: " + this.Value;
        }

        protected void SetUp()
        {
            this.PurchasedAbilities = new Dictionary<string, IAbility>();

            this.SetUpStatistics();
            this.SetUpSkills();
            this.SetUpAbilities();
        }

        protected void SetUpStatistics()
        {
            if (this.Statistics.Count < this.OriginalStatistics.Count)
            {
                for (int i = this.Statistics.Count; i < this.OriginalStatistics.Count(); i++)
                {
                    GrowingNamedItem newItem =
                        Instantiate(this.namedItemPrefab, this.StatisticsPanel.transform).GetComponent<GrowingNamedItem>();
                    newItem.gameObject.SetActive(true);
                    this.Statistics.Add(newItem);
                }
            }
            else if (this.Statistics.Count >= this.OriginalStatistics.Count)
            {
                for (int i = this.OriginalStatistics.Count; i < this.Statistics.Count; i++)
                {
                    this.Statistics[i].gameObject.SetActive(false);
                }
            }
        
            for(int i = 0; i < this.OriginalStatistics.Count; i++)
            {
                this.Statistics[i].gameObject.SetActive(true);
                this.Statistics[i].Name = this.OriginalStatistics[i].Name;
                this.Statistics[i].ValueChanged -= this.OnStatisticChange;
                this.Statistics[i].DirectValueSet(this.OriginalStatistics[i].Value);
                this.Statistics[i].ValueChanged += this.OnStatisticChange;
                this.Statistics[i].Minimum = this.OriginalStatistics[i].Value;
                this.SetUpStatisticDeltas();
            }
        }

        protected void SetUpSkills()
        {
            foreach (GrowingNamedItem item in this.Skills)
            {
                item.gameObject.SetActive(false);
            }
            
            if (this.Skills.Count < this.OriginalSkills.Count)
            {
                for (int i = this.Skills.Count; i < this.OriginalSkills.Count(); i++)
                {
                    GrowingNamedItem newItem =
                        Instantiate(this.namedItemPrefab, this.SkillsPanel.transform)
                            .GetComponent<GrowingNamedItem>();
                    newItem.gameObject.SetActive(true);
                    this.Skills.Add(newItem);
                }
            }
            else if (this.Skills.Count >= this.OriginalSkills.Count)
            {
                for (int i = this.OriginalSkills.Count; i < this.Skills.Count; i++)
                {
                    this.Skills[i].gameObject.SetActive(false);
                }
            }

            for (int i = 0; i < this.OriginalSkills.Count; i++)
            {
                this.Skills[i].gameObject.SetActive(true);
                this.Skills[i].Name = this.OriginalSkills[i].Name;
                this.Skills[i].ValueChanged -= this.OnSkillChange;
                this.Skills[i].DirectValueSet(this.OriginalSkills[i].Value);
                this.Skills[i].ValueChanged += this.OnSkillChange;
                this.Skills[i].Minimum = this.OriginalSkills[i].Value;
                this.SetUpSkillDeltas();
            }
        }

        protected void SetUpAbilities()
        {
            StringBuilder builder = new StringBuilder();
            
            List<Tuple<IAbility, int>> abilities = this.CurrentJob.Abilities.Where(pair => this.Player.Abilities.Contains(pair.Key) == false)
                    .Select(pair => new Tuple<IAbility, int>(pair.Key, pair.Value))
                    .ToList();

            if (this.Abilities.Count < abilities.Count)
            {
                for (int i = this.Abilities.Count; i < abilities.Count; i++)
                {
                    AbilityItem newItem =
                        Instantiate(this.AbilityItemPrefab, this.AbilitiesPanel.transform)
                            .GetComponent<AbilityItem>();
                    newItem.gameObject.SetActive(true);
                    this.Abilities.Add(newItem);
                }
            }
            else if (this.Abilities.Count > abilities.Count)
            {
                for (int i = abilities.Count; i < this.Abilities.Count; i++)
                {
                    this.Abilities[i].gameObject.SetActive(false);
                }
            }
            
            for (int i = 0; i < abilities.Count; i++)
            {
                builder.AppendLine(abilities[i].Item1.Description);
                builder.AppendLine("Cost: " + abilities[i].Item2);
                this.Abilities[i].gameObject.SetActive(true);
                this.Abilities[i].Name = abilities[i].Item1.Name;
                this.Abilities[i].Delta = abilities[i].Item2;
                this.Abilities[i].OnSelect -= this.OnAbilityChange;
                this.Abilities[i].OnSelect += this.OnAbilityChange;
                this.Abilities[i].Tooltip = builder.ToString();
                if (this.Abilities[i].Selected)
                {
                    this.Abilities[i].ToggleMe();
                }
                builder.Clear();
            }

            if (abilities.Count == 0)
            {
                if (this.Abilities.Count == 0)
                {
                    AbilityItem newItem =
                        Instantiate(this.AbilityItemPrefab, this.AbilitiesPanel.transform)
                            .GetComponent<AbilityItem>();
                    this.Abilities.Add(newItem);
                }

                this.Abilities[0].gameObject.SetActive(true);
                this.Abilities[0].OnSelect -= this.OnAbilityChange;
                this.Abilities[0].Name = "No Abilities Available";
                this.Abilities[0].Delta = 0;
                this.Abilities[0].Tooltip =
                    "You either have all of the abilities from this class, or do not qualify for any more.";
            }
        }

        protected void SetUpSkillDeltas()
        {
            for (int i = 0; i < this.OriginalSkills.Count; i++)
            {
                int delta = this.SkillsDeltas[this.OriginalSkills[i].Name.ToLower()] =
                    (this.Skills[i].Value * 10) - this.CurrentJob.GetSkillDiscount(this.OriginalSkills[i].Name);
                this.Skills[i].IncreaseCost = delta + 10;
                this.Skills[i].DecreaseCost = -delta;
                this.Skills[i].Tooltip = "Cost: " + (delta + 10);
            }
        }

        protected void SetUpStatisticDeltas()
        {
            for (int i = 0; i < this.OriginalStatistics.Count; i++)
            {
                int delta = this.StatisticsDeltas[this.OriginalStatistics[i].Name.ToLower()] =
                    (this.Statistics[i].Value * 10) - this.CurrentJob.GetStatisticDiscount(this.OriginalStatistics[i].Name);
                this.Statistics[i].IncreaseCost = delta + 10;
                this.Statistics[i].DecreaseCost = -delta;
                this.Statistics[i].Tooltip = "Cost: " + (delta + 10);
            }
        }

        public void MakeChanges()
        {
            int index = this.Player.Jobs.FindIndex(job => job.Name.Equals(this.CurrentJob.Name, StringComparison.OrdinalIgnoreCase));
            int cost = -(this.Value - this.CurrentJob.Experience);
            this.Player.Jobs[index].SpendExperience(cost);
            this.Player.Abilities.AddRange(this.PurchasedAbilities.Values);
            foreach (GrowingNamedItem item in this.Statistics)
            {
                int delta = item.Value - this.OriginalStatistics
                    .First(stat => stat.Name.Equals(item.Name, StringComparison.OrdinalIgnoreCase)).Value;

                if (delta != 0)
                {
                    this.Player.ModifyValue(item.Name.ToLower(), delta);
                }
            }

            foreach (GrowingNamedItem item in this.Skills)
            {
                int delta = item.Value - this.OriginalSkills
                    .First(stat => stat.Name.Equals(item.Name, StringComparison.OrdinalIgnoreCase)).Value;

                if (delta != 0)
                {
                    this.Player.ModifyValue(item.Name.ToLower(), delta);
                }
            }

            this.OriginalSkills = this.Player.Skills.Values.ToList();
            this.OriginalStatistics = this.Player.Statistics.Values.ToList();
            this.SetUp();
        }

        public override int Value
        {
            get => this.m_Value;
            set
            {
                int previous = this.m_Value;
                this.m_Value = value;
                this.OnChangeValue(this, new ValueChangedEventArgs
                {
                    NewValue = this.m_Value,
                    Delta = this.m_Value - previous
                });
                this.SetExperienceText();
            }
        }
    }
}
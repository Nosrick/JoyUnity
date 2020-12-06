using System.Collections.Generic;
using System.Linq;
using JoyLib.Code.Collections;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Events;
using TMPro;
using UnityEngine;

namespace JoyLib.Code.Unity.GUI
{
    public class SkillWindow : ValueContainer
    {
        [SerializeField] protected GameManager GameManager;
        [SerializeField] protected StatisticItem ValueContainerPrefab;
        [SerializeField] protected TextMeshProUGUI PointsRemainingText;
        [SerializeField] protected BasicPlayerInfo PlayerInfo;
        protected List<StatisticItem> Items { get; set; }
        
        protected BasicValueContainer<IGrowingValue> Skills { get; set; }

        public void Awake()
        {
            ValueContainerPrefab.gameObject.SetActive(false);
            Items = new List<StatisticItem>();
            Skills = new BasicValueContainer<IGrowingValue>();
            
            Value = Maximum;
            SetPointsRemaining();
        }

        public List<KeyValuePair<string, int>> GetSkillNames()
        {
            Skills = GameManager.SkillHandler.GetDefaultSkillBlock(GameManager.NeedHandler.Needs);

            Skills.AddRange(PlayerInfo.CurrentTemplate.Skills);
            
            return Skills.Select(skill => new KeyValuePair<string, int>(skill.Key, skill.Value.Value)).ToList();
        }

        public BasicValueContainer<IGrowingValue> GetSkillsBlock()
        {
            for (int i = 0; i < Items.Count; i++)
            {
                Skills[Items[i].Name].SetValue(Items[i].Value);
            }

            return Skills;
        }

        public void SetSkills(List<KeyValuePair<string, int>> skills)
        {
            if (Items.Count < skills.Count())
            {
                for (int i = Items.Count; i < skills.Count(); i++)
                {
                    StatisticItem newItem =
                        GameObject.Instantiate(ValueContainerPrefab, this.transform).GetComponent<StatisticItem>();
                    newItem.gameObject.SetActive(true);
                    Items.Add(newItem);
                }
            }
            
            for(int i = 0; i < Items.Count; i++)
            {
                Items[i].Name = skills[i].Key;
                Items[i].ValueChanged -= ChangeSkill;
                Items[i].DirectValueSet(skills[i].Value);
                Items[i].Minimum = skills[i].Value;
            }

            foreach (StatisticItem item in Items)
            {
                item.ValueChanged += ChangeSkill;
            }
        }

        public void SetPointsRemaining()
        {
            PointsRemainingText.text = "Points Remaining: " + Value;
        }

        protected void ChangeSkill(object sender, ValueChangedEventArgs args)
        {
            Value -= args.Delta;
            SetPointsRemaining();
        }
    }
}
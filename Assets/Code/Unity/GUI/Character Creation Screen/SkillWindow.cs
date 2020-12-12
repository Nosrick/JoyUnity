using System.Collections.Generic;
using System.Linq;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Events;
using TMPro;
using UnityEngine;

namespace JoyLib.Code.Unity.GUI
{
    public class SkillWindow : ValueContainer
    {
        [SerializeField] protected GameManager GameManager;
        [SerializeField] protected NamedItem ValueContainerPrefab;
        [SerializeField] protected TextMeshProUGUI PointsRemainingText;
        [SerializeField] protected BasicPlayerInfo PlayerInfo;
        protected List<NamedItem> Items { get; set; }
        
        protected IDictionary<string, EntitySkill> Skills { get; set; }

        public void Awake()
        {
            ValueContainerPrefab.gameObject.SetActive(false);
            Items = new List<NamedItem>();
            Skills = new Dictionary<string, EntitySkill>();
            
            Value = Maximum;
            SetPointsRemaining();
        }

        public List<KeyValuePair<string, int>> GetSkillNames()
        {
            Skills = GameManager.SkillHandler.GetDefaultSkillBlock(this.GameManager.NeedHandler.Needs);

            foreach (KeyValuePair<string, EntitySkill> pair in this.PlayerInfo.CurrentTemplate.Skills)
            {
                Skills.Add(pair);
            }

            return Skills.Select(skill => new KeyValuePair<string, int>(skill.Key, skill.Value.Value)).ToList();
        }

        public IDictionary<string, EntitySkill> GetSkillsBlock()
        {
            for (int i = 0; i < Items.Count; i++)
            {
                Skills[Items[i].Name.ToLower()].Value = Items[i].Value;
            }

            return Skills;
        }

        public void SetSkills(List<KeyValuePair<string, int>> skills)
        {
            if (Items.Count < skills.Count())
            {
                for (int i = Items.Count; i < skills.Count(); i++)
                {
                    NamedItem newItem =
                        GameObject.Instantiate(ValueContainerPrefab, this.transform).GetComponent<NamedItem>();
                    newItem.gameObject.SetActive(true);
                    Items.Add(newItem);
                }
            }
            
            for(int i = 0; i < skills.Count; i++)
            {
                Items[i].Name = skills[i].Key;
                Items[i].ValueChanged -= ChangeSkill;
                Items[i].DirectValueSet(skills[i].Value);
                Items[i].Minimum = skills[i].Value;
                Items[i].ValueChanged += ChangeSkill;
                //Items[i].Tooltip = skills[i].Key;
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
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
        [SerializeField] protected NamedItem ValueContainerPrefab;
        [SerializeField] protected TextMeshProUGUI PointsRemainingText;
        [SerializeField] protected BasicPlayerInfo PlayerInfo;
        protected List<NamedItem> Items { get; set; }
        protected IGameManager GameManager { get; set; }
        
        protected IDictionary<string, IEntitySkill> Skills { get; set; }

        public void Initialise()
        {
            this.GameManager = GlobalConstants.GameManager;
            base.OnEnable();
            this.ValueContainerPrefab.gameObject.SetActive(false);
            this.Items = new List<NamedItem>();
            this.Skills = new Dictionary<string, IEntitySkill>();

            this.Value = this.Maximum;
            this.SetPointsRemaining();
        }

        public List<KeyValuePair<string, int>> GetSkillNames()
        {
            this.Skills = this.GameManager.SkillHandler.GetDefaultSkillBlock();

            foreach (KeyValuePair<string, IEntitySkill> pair in this.PlayerInfo.CurrentTemplate.Skills)
            {
                this.Skills.Add(pair);
            }

            return this.Skills.Select(skill => new KeyValuePair<string, int>(skill.Key, skill.Value.Value)).ToList();
        }

        public IDictionary<string, IEntitySkill> GetSkillsBlock()
        {
            for (int i = 0; i < this.Items.Count; i++)
            {
                this.Skills[this.Items[i].Name.ToLower()].SetValue(this.Items[i].Value);
            }

            return this.Skills;
        }

        public void SetSkills(List<KeyValuePair<string, int>> skills)
        {
            if (this.Items.Count < skills.Count())
            {
                for (int i = this.Items.Count; i < skills.Count(); i++)
                {
                    NamedItem newItem =
                        Instantiate(this.ValueContainerPrefab, this.transform).GetComponent<NamedItem>();
                    newItem.gameObject.SetActive(true);
                    newItem.Parent = this;
                    this.Items.Add(newItem);
                }
            }
            
            for(int i = 0; i < skills.Count; i++)
            {
                this.Items[i].Name = skills[i].Key;
                this.Items[i].ValueChanged -= this.ChangeSkill;
                this.Items[i].DirectValueSet(skills[i].Value);
                this.Items[i].Minimum = skills[i].Value;
                this.Items[i].Maximum = 3;
                this.Items[i].ValueChanged += this.ChangeSkill;
                //Items[i].Tooltip = skills[i].Key;
            }
        }

        public void SetPointsRemaining()
        {
            this.PointsRemainingText.text = "Points Remaining: " + this.Value;
        }

        protected void ChangeSkill(object sender, ValueChangedEventArgs<int> args)
        {
            this.Value -= args.Delta;
            this.SetPointsRemaining();
        }
    }
}
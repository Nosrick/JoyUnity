using System;
using System.Collections.Generic;
using System.Linq;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Events;
using TMPro;
using UnityEngine;

namespace JoyLib.Code.Unity.GUI
{
    public class StatisticWindow : ValueContainer
    {
        [SerializeField] protected NamedItem namedItem;
        [SerializeField] protected TextMeshProUGUI m_PointsRemainingText;
        protected List<NamedItem> Items { get; set; }

        public void Initialise()
        {
            base.OnEnable();
            this.namedItem.gameObject.SetActive(false);
            if (this.Items is null)
            {
                this.Items = new List<NamedItem>();
            }

            this.Value = this.Maximum;
            this.SetRemainingPointsText();
        }

        public IDictionary<string, IEntityStatistic> GetStatistics()
        {
            IDictionary<string, IEntityStatistic> stats = new Dictionary<string, IEntityStatistic>();
            foreach (NamedItem item in this.Items)
            {
                stats.Add(
                    item.Name.ToLower(),
                    new EntityStatistic(
                        item.Name.ToLower(),
                        item.Value,
                        GlobalConstants.DEFAULT_SUCCESS_THRESHOLD));
            }

            return stats;
        }

        public void SetStatistics(List<Tuple<string, int>> statistics)
        {
            if (this.Items.Count < statistics.Count())
            {
                for (int i = this.Items.Count; i < statistics.Count(); i++)
                {
                    NamedItem newItem =
                        Instantiate(this.namedItem, this.transform).GetComponent<NamedItem>();
                    newItem.gameObject.SetActive(true);
                    newItem.Parent = this;
                    this.Items.Add(newItem);
                }
            }
            
            for(int i = 0; i < statistics.Count; i++)
            {
                this.Items[i].Name = statistics[i].Item1;
                this.Items[i].ValueChanged -= this.ChangeStatistic;
                this.Items[i].DirectValueSet(statistics[i].Item2);
                this.Items[i].ValueChanged += this.ChangeStatistic;
                this.Items[i].gameObject.GetComponent<RectTransform>()
                    .SetSizeWithCurrentAnchors(
                        RectTransform.Axis.Horizontal, 
                        this.GetComponent<RectTransform>().rect.width);
            }

            this.Value = this.Maximum;
            this.SetRemainingPointsText();
        }

        protected void ChangeStatistic(object sender, ValueChangedEventArgs args)
        {
            this.Value -= args.Delta;
            this.SetRemainingPointsText();
        }

        protected void SetRemainingPointsText()
        {
            this.m_PointsRemainingText.text = "Statistic Points Remaining: " + this.Value;
        }
    }
}
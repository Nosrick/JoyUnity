using System;
using System.Collections.Generic;
using System.Linq;
using JoyLib.Code.Collections;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Events;
using JoyLib.Code.Rollers;
using TMPro;
using UnityEngine;

namespace JoyLib.Code.Unity.GUI
{
    public class StatisticWindow : ValueContainer
    {
        [SerializeField] protected NamedItem namedItem;
        [SerializeField] protected TextMeshProUGUI m_PointsRemainingText;
        protected List<NamedItem> Items { get; set; }

        public void Awake()
        {
            namedItem.gameObject.SetActive(false);
            Items = new List<NamedItem>();
            
            Value = Maximum;
            SetRemainingPointsText();
        }

        public BasicValueContainer<IRollableValue> GetStatistics()
        {
            BasicValueContainer<IRollableValue> stats = new BasicValueContainer<IRollableValue>();
            foreach (NamedItem item in Items)
            {
                stats.Add(new EntityStatistic(
                    item.Name,
                    item.Value,
                    GlobalConstants.DEFAULT_SUCCESS_THRESHOLD,
                    new StandardRoller()));
            }

            return stats;
        }

        public void SetStatistics(List<Tuple<string, int>> statistics)
        {
            Value = Maximum;
            
            if (Items.Count < statistics.Count())
            {
                for (int i = Items.Count; i < statistics.Count(); i++)
                {
                    NamedItem newItem =
                        GameObject.Instantiate(namedItem, this.transform).GetComponent<NamedItem>();
                    newItem.gameObject.SetActive(true);
                    Items.Add(newItem);
                }
            }
            
            for(int i = 0; i < statistics.Count; i++)
            {
                Items[i].Name = statistics[i].Item1;
                Items[i].ValueChanged -= ChangeStatistic;
                Items[i].DirectValueSet(statistics[i].Item2);
                Items[i].ValueChanged += ChangeStatistic;
            }
            
            SetRemainingPointsText();
        }

        protected void ChangeStatistic(object sender, ValueChangedEventArgs args)
        {
            Value -= args.Delta;
            SetRemainingPointsText();
        }

        protected void SetRemainingPointsText()
        {
            m_PointsRemainingText.text = "Points Remaining: " + Value;
        }
    }
}
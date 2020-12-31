﻿using System;
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
            namedItem.gameObject.SetActive(false);
            if (Items is null)
            {
                Items = new List<NamedItem>();
            }
            
            Value = Maximum;
            SetRemainingPointsText();
        }

        public IDictionary<string, IRollableValue<int>> GetStatistics()
        {
            IDictionary<string, IRollableValue<int>> stats = new Dictionary<string, IRollableValue<int>>();
            foreach (NamedItem item in Items)
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
                Items[i].gameObject.GetComponent<RectTransform>()
                    .SetSizeWithCurrentAnchors(
                        RectTransform.Axis.Horizontal, 
                        this.GetComponent<RectTransform>().rect.width);
            }
            
            Value = Maximum;
            SetRemainingPointsText();
        }

        protected void ChangeStatistic(object sender, ValueChangedEventArgs args)
        {
            Value -= args.Delta;
            SetRemainingPointsText();
        }

        protected void SetRemainingPointsText()
        {
            m_PointsRemainingText.text = "Statistic Points Remaining: " + Value;
        }
    }
}
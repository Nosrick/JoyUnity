﻿using System;
using System.Collections.Generic;
using System.Linq;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Unity.GUI;
using UnityEngine;

namespace JoyLib.Code.Unity.GUI
{
    public class StatisticWindow : MonoBehaviour
    {
        public int PointsRemaining { get; set; }
        protected StatisticItem StatisticItem { get; set; }
        protected List<StatisticItem> Items { get; set; } 

        public void Awake()
        {
            StatisticItem = this.transform.Find("Statistic Item").gameObject.GetComponent<StatisticItem>();
            StatisticItem.gameObject.SetActive(false);
            Items = new List<StatisticItem>();
            
            PointsRemaining = 12;
        }

        public void SetStatistics(List<Tuple<string, int>> statistics)
        {
            if (Items.Count < statistics.Count())
            {
                for (int i = Items.Count; i < statistics.Count(); i++)
                {
                    StatisticItem newItem =
                        GameObject.Instantiate(StatisticItem, this.transform).GetComponent<StatisticItem>();
                    newItem.gameObject.SetActive(true);
                    Items.Add(newItem);
                }
            }
            
            for(int i = 0; i < Items.Count; i++)
            {
                Items[i].Name = statistics[i].Item1;
                Items[i].Value = statistics[i].Item2;
            }
        }
    }
}
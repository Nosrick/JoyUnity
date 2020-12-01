using System;
using System.Collections.Generic;
using JoyLib.Code.Entities.Statistics;
using UnityEngine;

namespace JoyLib.Code.Unity.GUI.StatisticWindow
{
    public class StatisticWindow : MonoBehaviour
    {
        public int PointsRemaining { get; set; }
        protected Dictionary<string, int> PlayerStatistics { get; set; }
        protected StatisticItem StatisticItem { get; set; }
        protected List<StatisticItem> Items { get; set; }

        public void Awake()
        {
            StatisticItem = this.transform.Find("Statistic Item").gameObject.GetComponent<StatisticItem>();
            StatisticItem.gameObject.SetActive(false);
            Items = new List<StatisticItem>();
            
            PointsRemaining = 12;

            string[] names = EntityStatistic.NAMES;
            PlayerStatistics = new Dictionary<string, int>(names.Length);

            foreach (string index in names)
            {
                PlayerStatistics.Add(index, 4);
                StatisticItem newItem =
                    GameObject.Instantiate(StatisticItem, this.transform).GetComponent<StatisticItem>();
                newItem.gameObject.SetActive(true);
                newItem.Name = index;
                newItem.Value = 4;
                Items.Add(newItem);
            }
        }
    }
}
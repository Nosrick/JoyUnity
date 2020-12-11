﻿using System.Collections.Generic;
using DevionGames.UIWidgets;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Events;
using UnityEngine;

namespace JoyLib.Code.Unity.GUI
{
    public class DerivedValuesInformation : UIWidget
    {
        [SerializeField] protected DerivedValueBarContainer DerivedValuePrefab;
        [SerializeField] protected GameManager GameManager;

        protected Dictionary<string, DerivedValueBarContainer> Items { get; set; }
        protected IEntity Player { get; set; }

        public void OnEnable()
        {
            if (this.GameManager is null)
            {
                return;
            }

            if (this.Items is null)
            {
                this.Items = new Dictionary<string, DerivedValueBarContainer>();
            }

            this.Player = this.GameManager.Player;
            this.Player.DerivedValueChange -= this.DerivedValueChange;
            this.Player.DerivedValueChange += this.DerivedValueChange;
            this.Player.DerivedValueMaximumChange -= this.DerivedValueMaximumChange;
            this.Player.DerivedValueMaximumChange += this.DerivedValueMaximumChange;
            this.SetUpDerivedValues(this.Player.DerivedValues);
        }

        protected void DerivedValueChange(object sender, ValueChangedEventArgs args)
        {
            this.Items[args.Name].Value = args.NewValue;
        }

        protected void DerivedValueMaximumChange(object sender, ValueChangedEventArgs args)
        {
            this.Items[args.Name].Maximum = args.NewValue;
        }

        public void SetUpDerivedValues(IDictionary<string, IDerivedValue<int>> values)
        {
            List<IDerivedValue<int>> valueList = new List<IDerivedValue<int>>(values.Values);
            
            if (this.Items.Count < valueList.Count)
            {
                for (int i = this.Items.Count; i < valueList.Count; i++)
                {
                    DerivedValueBarContainer newItem =
                        Instantiate(this.DerivedValuePrefab, this.transform).GetComponent<DerivedValueBarContainer>();
                    newItem.gameObject.SetActive(true);
                    this.Items.Add(valueList[i].Name, newItem);
                }
            }
            
            for(int i = 0; i < valueList.Count; i++)
            {
                this.Items[valueList[i].Name].Name = valueList[i].Name;
                this.Items[valueList[i].Name].DirectValueSet(valueList[i].Value);
                this.Items[valueList[i].Name].Minimum = -valueList[i].Maximum;
                this.Items[valueList[i].Name].Maximum = valueList[i].Maximum;
            }
        }
    }
}
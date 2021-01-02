﻿using JoyLib.Code.Entities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JoyLib.Code.Unity.GUI.Character_Sheet
{
    public class CharacterSheet : GUIData
    {
        [SerializeField] protected Image PlayerSprite;
        [SerializeField] protected TextMeshProUGUI PlayerName;
        [SerializeField] protected TextMeshProUGUI PlayerJobAndSpecies;
        [SerializeField] protected StaticDerivedValueDisplay DerivedValues;
        [SerializeField] protected StaticValueDisplay Statistics;
        [SerializeField] protected StaticValueDisplay Skills;
        [SerializeField] protected StaticAbilityDisplay Abilities;
        
        protected IEntity Player;

        public void OnEnable()
        {
            if (GlobalConstants.GameManager is null)
            {
                return;
            }

            this.Player = GlobalConstants.GameManager.Player;
            this.Statistics.SetValues(this.Player.Statistics.Values);
            this.Skills.SetValues(this.Player.Skills.Values);
            this.Abilities.SetValues(this.Player.Abilities);
            this.DerivedValues.SetValues(this.Player.DerivedValues);
            this.PlayerSprite.sprite = this.Player.Sprite;
            this.PlayerName.text = this.Player.JoyName;
            this.PlayerJobAndSpecies.text = this.Player.CreatureType + " " + this.Player.CurrentJob.Name;
        }
    }
}
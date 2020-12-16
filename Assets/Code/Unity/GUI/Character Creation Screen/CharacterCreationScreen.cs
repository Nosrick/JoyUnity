using System;
using System.Collections.Generic;
using DevionGames.UIWidgets;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Statistics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JoyLib.Code.Unity.GUI
{
    public class CharacterCreationScreen : UIWidget
    {
        [SerializeField] protected GameManager GameManager;
        [SerializeField] protected StatisticWindow StatisticWindow;
        [SerializeField] protected DerivedValuesWindow DerivedValuesWindow;
        [SerializeField] protected SkillWindow SkillWindow;
        [SerializeField] protected BasicPlayerInfo PlayerInfo;
        [SerializeField] protected AbilityWindow AbilityWindow;

        [SerializeField] protected Image PlayerSprite_Part1;
        [SerializeField] protected Image PlayerSprite_Part2;
        [SerializeField] protected TMP_InputField PlayerName_Part1;
        [SerializeField] protected TMP_InputField PlayerName_Part2;

        public void Initialise()
        {
            this.Awake();
            PlayerInfo.JobChanged += SetSprites;
            PlayerInfo.CultureChanged += SetRandomName;
            StatisticWindow.ValueChanged += ChangedStatistics;
            PlayerInfo.Initialise();
        }

        public IEntity CreatePlayer()
        {
            return GameManager.EntityFactory.CreateFromTemplate(
                this.PlayerInfo.CurrentTemplate,
                GlobalConstants.NO_TARGET,
                this.StatisticWindow.GetStatistics(),
                this.DerivedValuesWindow.GetDerivedValues(),
                this.SkillWindow.GetSkillsBlock(), 
                this.AbilityWindow.GetPickedAbilities(),
                this.PlayerInfo.CurrentCultures,
                this.GameManager.GenderHandler.Get(this.PlayerInfo.Gender),
                this.GameManager.BioSexHandler.Get(this.PlayerInfo.Sex),
                this.GameManager.SexualityHandler.Get(this.PlayerInfo.Sexuality),
                this.GameManager.RomanceHandler.Get(this.PlayerInfo.Romance),
                this.GameManager.JobHandler.Get(this.PlayerInfo.Job), 
                this.GameManager.ObjectIconHandler.GetSprites(this.PlayerInfo.CurrentTemplate.CreatureType, this.PlayerInfo.Job));
        }

        protected void ChangedStatistics(object sender, EventArgs args)
        {
            IDictionary<string, EntityStatistic> stats = StatisticWindow.GetStatistics();
            if (stats.Count == 0)
            {
                return;
            }

            DerivedValuesWindow.SetDerivedValues(this.GameManager.DerivedValueHandler.GetEntityStandardBlock(stats.Values));
        }

        public void SetRandomName(object sender, EventArgs args)
        {
            this.SetRandomName();
        }

        public void SetRandomName()
        {
            PlayerName_Part1.text = PlayerInfo.CurrentCulture.GetRandomName(PlayerInfo.Gender);
            PlayerName_Part2.text = PlayerName_Part1.text;
        }

        public void SetSprites(object sender, EventArgs args)
        {
            PlayerSprite_Part1.sprite = GameManager.ObjectIconHandler.GetSprite(PlayerInfo.CurrentTemplate.CreatureType, PlayerInfo.Job);
            PlayerSprite_Part2.sprite = PlayerSprite_Part1.sprite;
        }

        public void GoToSkillsAndAbilities()
        {
            GameManager.GUIManager.CloseGUI(this.name);
            GameManager.GUIManager.OpenGUI(GlobalConstants.CHARACTER_CREATION_PART_2);
            SkillWindow.SetSkills(SkillWindow.GetSkillNames());
            AbilityWindow.GetAvailableAbilities(PlayerInfo.CurrentTemplate, StatisticWindow.GetStatistics(), SkillWindow.GetSkillsBlock());
        }
    }
}
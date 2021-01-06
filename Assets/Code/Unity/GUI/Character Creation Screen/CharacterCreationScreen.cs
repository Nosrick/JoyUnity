using System;
using System.Collections.Generic;
using System.Linq;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.AI.Drivers;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Graphics;
using TMPro;
using UnityEngine;

namespace JoyLib.Code.Unity.GUI
{
    public class CharacterCreationScreen : GUIData
    {
        [SerializeField] protected GameManager GameManager;
        [SerializeField] protected StatisticWindow StatisticWindow;
        [SerializeField] protected DerivedValuesWindow DerivedValuesWindow;
        [SerializeField] protected SkillWindow SkillWindow;
        [SerializeField] protected BasicPlayerInfo PlayerInfo;
        [SerializeField] protected AbilityWindow AbilityWindow;

        [SerializeField] protected ManagedSprite PlayerSprite_Part1;
        [SerializeField] protected ManagedSprite PlayerSprite_Part2;
        [SerializeField] protected TMP_InputField PlayerName_Part1;
        [SerializeField] protected TMP_InputField PlayerName_Part2;
        
        protected string PlayerName { get; set; }

        public void Initialise()
        {
            this.PlayerInfo.JobChanged += this.SetSprites;
            this.PlayerInfo.CultureChanged += this.SetRandomName;
            this.StatisticWindow.ValueChanged += this.ChangedStatistics;
            this.PlayerName_Part1.GetComponent<TextWatcher>().OnTextChange += this.UpdatePlayerName;
            this.PlayerName_Part2.GetComponent<TextWatcher>().OnTextChange += this.UpdatePlayerName;
            this.StatisticWindow.Initialise();
            this.SkillWindow.Initialise();
            this.DerivedValuesWindow.Initialise();
            this.PlayerInfo.Initialise();
            this.AbilityWindow.Initialise();
            //this.GetComponent<FontSizeManager>().ResizeFonts();
        }

        public IEntity CreatePlayer()
        {
            var data = this.GameManager.ObjectIconHandler.GetSprites(
                this.PlayerInfo.CurrentTemplate.CreatureType,
                this.PlayerInfo.Job);
            List<ISpriteState> spriteStates = data.Select(d => SpriteState.MakeWithDefaultColour(d.m_Name, d)).ToList();

            return this.GameManager.EntityFactory.CreateFromTemplate(
                this.PlayerInfo.CurrentTemplate,
                GlobalConstants.NO_TARGET,
                this.PlayerName,
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
                spriteStates,
                null,
                new PlayerDriver());
        }

        protected void ChangedStatistics(object sender, EventArgs args)
        {
            IDictionary<string, IRollableValue<int>> stats = this.StatisticWindow.GetStatistics();
            if (stats.Count == 0)
            {
                return;
            }

            this.DerivedValuesWindow.SetDerivedValues(this.GameManager.DerivedValueHandler.GetEntityStandardBlock(stats.Values));
        }

        public void SetRandomName(object sender, EventArgs args)
        {
            this.SetRandomName();
        }

        public void SetRandomName()
        {
            this.PlayerName = this.PlayerInfo.CurrentCulture.GetRandomName(this.PlayerInfo.Gender);
            this.PlayerName_Part1.text = this.PlayerName;
            this.PlayerName_Part2.text = this.PlayerName;
        }

        public void SetSprites(object sender, EventArgs args)
        {
            SpriteData data = this.GameManager.ObjectIconHandler.GetSprites(
                this.PlayerInfo.CurrentTemplate.CreatureType,
                "default").First();
            ISpriteState state = SpriteState.MakeWithDefaultColour(data.m_Name, data);
            this.PlayerSprite_Part1.Clear();
            this.PlayerSprite_Part1.AddSpriteState(state, true);
            this.PlayerSprite_Part2.Clear();
            this.PlayerSprite_Part2.AddSpriteState(state, true);
        }

        public void GoToSkillsAndAbilities()
        {
            this.GameManager.GUIManager.CloseGUI(this.name);
            this.GameManager.GUIManager.OpenGUI(GUINames.CHARACTER_CREATION_PART_2);
            this.SkillWindow.SetSkills(this.SkillWindow.GetSkillNames());
            this.AbilityWindow.GetAvailableAbilities(this.PlayerInfo.CurrentTemplate, this.StatisticWindow.GetStatistics(), this.SkillWindow.GetSkillsBlock());
        }

        public void GoToPlayerInfo()
        {
            this.GameManager.GUIManager.CloseGUI(GUINames.CHARACTER_CREATION_PART_2);
            this.GameManager.GUIManager.OpenGUI(this.name);
        }

        protected void UpdatePlayerName(object sender, TextChangedEventArgs args)
        {
            this.PlayerName = args.NewValue;
            if (sender.Equals(this.PlayerName_Part2))
            {
                this.PlayerName_Part1.text = this.PlayerName;
            }
            else if (sender.Equals(this.PlayerName_Part1))
            {
                this.PlayerName_Part2.text = this.PlayerName;
            }
        }
    }
}
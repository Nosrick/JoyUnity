using System;
using System.Collections.Generic;
using System.Linq;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.AI.Drivers;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Graphics;
using JoyLib.Code.States;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace JoyLib.Code.Unity.GUI
{
    public class CharacterCreationScreen : MonoBehaviour
    {
        [SerializeField] protected StatisticWindow StatisticWindow;
        [SerializeField] protected DerivedValuesWindow DerivedValuesWindow;
        [SerializeField] protected SkillWindow SkillWindow;
        [SerializeField] protected BasicPlayerInfo PlayerInfo;
        [SerializeField] protected AbilityWindow AbilityWindow;

        [SerializeField] protected ManagedUISprite PlayerSprite_Part1;
        [SerializeField] protected ManagedUISprite PlayerSprite_Part2;
        [SerializeField] protected TMP_InputField PlayerName_Part1;
        [SerializeField] protected TMP_InputField PlayerName_Part2;
        
        protected string PlayerName { get; set; }
        protected IGameManager GameManager { get; set; }

        public void Initialise()
        {
            this.GameManager = GlobalConstants.GameManager;
            this.PlayerInfo.JobChanged += this.SetSprites;
            this.PlayerInfo.CultureChanged += this.CultureChangeHandler;
            this.StatisticWindow.ValueChanged += this.ChangedStatistics;
            this.PlayerName_Part1.GetComponent<TextWatcher>().OnTextChange += this.UpdatePlayerName;
            this.PlayerName_Part2.GetComponent<TextWatcher>().OnTextChange += this.UpdatePlayerName;
            this.PlayerSprite_Part1.Awake();
            this.PlayerSprite_Part2.Awake();
            this.StatisticWindow.Initialise();
            this.SkillWindow.Initialise();
            this.DerivedValuesWindow.Initialise();
            this.PlayerInfo.Initialise();
            this.AbilityWindow.Initialise();
            
            this.GameManager.GUIManager.RecolourGUIs();
        }

        public void CreatePlayer()
        {
            var data = this.GameManager.ObjectIconHandler.GetSprites(
                this.PlayerInfo.CurrentCulture.CultureName,
                this.PlayerInfo.CurrentTemplate.CreatureType, "idle");
            List<SpriteState> spriteStates = data.Select(d => new SpriteState(d.m_Name, d)).ToList();

            this.GameManager.GUIManager.SetUIColours(
                this.PlayerInfo.CurrentCulture.BackgroundColours,
                this.PlayerInfo.CurrentCulture.CursorColours, 
                this.PlayerInfo.CurrentCulture.FontColours);

            IEntity player = this.GameManager.EntityFactory.CreateFromTemplate(
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
            
            this.GameManager.SetNextState(new WorldCreationState(player));
            SceneManager.LoadScene("MainGame");
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

        public void CultureChangeHandler(object sender, EventArgs args)
        {
            this.SetRandomName();
        }

        public void SetRandomName()
        {
            this.UpdatePlayerName(this.PlayerInfo.CurrentCulture.GetRandomName(this.PlayerInfo.Gender));
        }

        public void SetSprites(object sender, EventArgs args)
        {
            SpriteData data = this.GameManager.ObjectIconHandler.GetSprites(
                this.PlayerInfo.CurrentCulture.CultureName,
                this.PlayerInfo.CurrentTemplate.CreatureType, "idle").First();
            ISpriteState state = new SpriteState(data.m_Name, data);
            state.RandomiseColours();
            this.PlayerSprite_Part1.Clear();
            this.PlayerSprite_Part1.AddSpriteState(state, true);
            this.PlayerSprite_Part2.Clear();
            this.PlayerSprite_Part2.AddSpriteState(state, true);
        }

        public void GoToSkillsAndAbilities()
        {
            this.GameManager.GUIManager.OpenGUI(GUINames.CHARACTER_CREATION_PART_2, true);
            this.SkillWindow.SetSkills(this.SkillWindow.GetSkillNames());
            this.AbilityWindow.GetAvailableAbilities(this.PlayerInfo.CurrentTemplate, this.StatisticWindow.GetStatistics(), this.SkillWindow.GetSkillsBlock());
        }

        public void GoToPlayerInfo()
        {
            this.GameManager.GUIManager.OpenGUI(this.name, true);
        }

        protected void UpdatePlayerName(GameObject sender, TextChangedEventArgs args)
        {
            this.UpdatePlayerName(args.NewValue);
        }

        protected void UpdatePlayerName(string name)
        {
            this.PlayerName = name;
            this.PlayerName_Part1.text = this.PlayerName;
            this.PlayerName_Part2.text = this.PlayerName;
        }
    }
}
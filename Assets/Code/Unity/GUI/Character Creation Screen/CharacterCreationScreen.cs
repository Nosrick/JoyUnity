using System;
using DevionGames.UIWidgets;
using JoyLib.Code.Collections;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Needs;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Rollers;
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

        public void OnEnable()
        {
            this.Awake();
            PlayerInfo.JobChanged += SetSprites;
            PlayerInfo.CultureChanged += SetRandomName;
            StatisticWindow.ValueChanged += ChangedStatistics;
        }

        public IEntity CreatePlayer()
        {
            return GameManager.EntityFactory.CreateFromTemplate(
                PlayerInfo.CurrentTemplate,
                GlobalConstants.NO_TARGET,
                new ConcreteGrowingValue(
                    "level",
                    1,
                    0,
                    0,
                    GlobalConstants.DEFAULT_SUCCESS_THRESHOLD,
                    new StandardRoller(),
                    new NonUniqueDictionary<INeed, float>()),
                StatisticWindow.GetStatistics(),
                SkillWindow.GetSkillsBlock(), 
                AbilityWindow.GetPickedAbilities(),
                PlayerInfo.CurrentCultures,
                GameManager.GenderHandler.Get(PlayerInfo.Gender),
                GameManager.BioSexHandler.Get(PlayerInfo.Sex),
                GameManager.SexualityHandler.Get(PlayerInfo.Sexuality),
                GameManager.RomanceHandler.Get(PlayerInfo.Romance),
                GameManager.JobHandler.Get(PlayerInfo.Job), 
                GameManager.ObjectIconHandler.GetSprites(PlayerInfo.CurrentTemplate.CreatureType, PlayerInfo.Job));
        }

        protected void ChangedStatistics(object sender, EventArgs args)
        {
            BasicValueContainer<IRollableValue> stats = StatisticWindow.GetStatistics();
            if (stats.Count == 0)
            {
                return;
            }
            
            DerivedValuesWindow.SetDerivedValues(
                EntityDerivedValue.GetDefault(
                        stats.GetRawValue(EntityStatistic.ENDURANCE),
                        stats.GetRawValue(EntityStatistic.FOCUS),
                        stats.GetRawValue(EntityStatistic.WIT))
                    .Values);
        }

        public void SetRandomName(object sender, EventArgs args)
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
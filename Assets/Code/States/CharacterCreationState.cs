using JoyLib.Code.Collections;
using JoyLib.Code.Cultures;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Jobs;
using JoyLib.Code.Entities.Needs;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Entities.Items;
using JoyLib.Code.Graphics;
using JoyLib.Code.Rollers;
using System;
using System.Collections.Generic;
using JoyLib.Code.Unity.GUI.StatisticWindow;
using UnityEngine;

namespace JoyLib.Code.States
{
    public class CharacterCreationState : GameState
    {
        protected List<JobType> m_Jobs;
        protected int JobIndex { get; set; }
        protected int SexIndex { get; set; }
        protected int GenderIndex { get; set; }
        protected int CultureIndex { get; set; }
        protected StatisticWindow StatisticWindow { get; set; }

        protected EntityPlayer m_Player;

        public CharacterCreationState()
        {
            JobIndex = 0;
            SexIndex = 0;
            GenderIndex = 0;
            CultureIndex = 0;
        }

        public override void Start()
        {
            base.Start();
            SetUpUi();
        }

        public override void SetUpUi()
        {
            base.SetUpUi();
            GUIManager.OpenGUI(GlobalConstants.CHARACTER_CREATION);
        }

        public override void HandleInput()
        {
            base.HandleInput();
        }

        public override void Update()
        {
            base.Update();
        }

        private void NextJob(object sender, EventArgs eventArgs)
        {
            JobIndex += 1;
            JobIndex %= m_Jobs.Count;
        }

        private void PreviousJob(object sender, EventArgs eventArgs)
        {
            JobIndex -= 1;
            JobIndex %= m_Jobs.Count;
        }

        private void ToggleSex(object sender, EventArgs eventArgs)
        {
            SexIndex += 1;
            SexIndex %= 3;
        }

        private string GetJobDescription()
        {
            string jobDescription = m_Jobs[JobIndex].Name + "\r\n" + m_Jobs[JobIndex].Description + "\r\n";
            return jobDescription;
        }

        private void NextState(object sender, EventArgs eventArgs)
        {
            IGameManager gameManager = GlobalConstants.GameManager;

            IEntityTemplateHandler templateHandler = gameManager.EntityTemplateHandler; 

            EntityTemplate humanTemplate = templateHandler.Get("Human");

            IGrowingValue level = new ConcreteGrowingValue("level", 1, 100, 0, GlobalConstants.DEFAULT_SUCCESS_THRESHOLD,
                new StandardRoller(GlobalConstants.GameManager.Roller), new NonUniqueDictionary<INeed, float>());

            IEntity temp = GlobalConstants.GameManager.EntityFactory.CreateFromTemplate(
                humanTemplate,
                level,
                new UnityEngine.Vector2Int(-1, -1),
                null,
                null,
                null,
                null,
                null,
                m_Jobs[JobIndex]);
            
            m_Player = new EntityPlayer(temp);

            m_Player.PlayerControlled = true;
            
            IItemInstance light = GlobalConstants.GameManager.ItemFactory.CreateRandomItemOfType(new string[] {"light source"}, true);
            m_Player.AddContents(light);
        }

        public override GameState GetNextState()
        {
            return new WorldCreationState(m_Player);
        }
    }
}
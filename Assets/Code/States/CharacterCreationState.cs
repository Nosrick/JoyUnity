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
using JoyLib.Code.Unity.GUI;
using UnityEngine;

namespace JoyLib.Code.States
{
    public class CharacterCreationState : GameState
    {
        protected int m_PointsRemaining;
        protected Dictionary<string, int> m_PlayerStatistics;

        protected List<JobType> m_Jobs;
        protected int m_JobIndex;
        protected int m_SexIndex;

        protected EntityPlayer m_Player;

        public CharacterCreationState() : base()
        {
            m_PointsRemaining = 12;

            string[] names = EntityStatistic.NAMES;
            m_PlayerStatistics = new Dictionary<string, int>(names.Length);

            foreach (string index in names)
            {
                m_PlayerStatistics.Add(index, 4);
            }
            m_JobIndex = 0;
            m_SexIndex = 0;
        }

        public override void Start()
        {
            base.Start();
            SetUpUi();
        }

        protected override void SetUpUi()
        {
            base.SetUpUi();
        }

        private void UpdateUI()
        {
        }

        public override void Draw()
        {
            base.Draw();
        }

        public override void OnGui()
        {
            base.OnGui();

            UpdateUI();
        }

        public override void HandleInput()
        {
            base.HandleInput();
        }

        public override void Update()
        {
            base.Update();
        }

        private void DecrementStatistic(string index)
        {
            if (m_PlayerStatistics[index] > 1)
            {
                m_PointsRemaining += 1;
                m_PlayerStatistics[index] -= 1;
            }
        }

        private void IncrementStatistic(string index)
        {
            if (m_PlayerStatistics[index] < 10 && m_PointsRemaining > 0)
            {
                m_PointsRemaining -= 1;
                m_PlayerStatistics[index] += 1;
            }
        }

        private void NextJob(object sender, EventArgs eventArgs)
        {
            m_JobIndex += 1;
            m_JobIndex %= m_Jobs.Count;
        }

        private void PreviousJob(object sender, EventArgs eventArgs)
        {
            m_JobIndex -= 1;
            m_JobIndex %= m_Jobs.Count;
        }

        private void Togglesex(object sender, EventArgs eventArgs)
        {
            m_SexIndex += 1;
            m_SexIndex %= 3;
        }

        private string GetJobDescription()
        {
            string jobDescription = m_Jobs[m_JobIndex].Name + "\r\n" + m_Jobs[m_JobIndex].Description + "\r\n";
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
                m_Jobs[m_JobIndex]);
            
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
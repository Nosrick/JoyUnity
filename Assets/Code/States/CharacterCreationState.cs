using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Items;
using JoyLib.Code.Entities.Jobs;
using JoyLib.Code.Entities.Needs;
using JoyLib.Code.Graphics;
using JoyLib.Code.Loaders;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JoyLib.Code.States
{
    public class CharacterCreationState : GameState
    {
        protected int m_PointsRemaining;
        protected Dictionary<StatisticIndex, float> m_PlayerStatistics;

        protected List<JobType> m_Jobs;
        protected int m_JobIndex;
        protected int m_sexIndex;

        protected Entity m_Player;

        public CharacterCreationState() : base()
        {
            m_PointsRemaining = 12;

            Array statisticIndices = Enum.GetValues(typeof(StatisticIndex));
            m_PlayerStatistics = new Dictionary<StatisticIndex, float>(statisticIndices.Length);

            foreach (StatisticIndex index in statisticIndices)
            {
                m_PlayerStatistics.Add(index, 30);
            }
            m_JobIndex = 0;
            m_sexIndex = 0;
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
            int numStats = Enum.GetNames(typeof(StatisticIndex)).Length;
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

        private void DecrementStatistic(StatisticIndex index)
        {
            if (m_PlayerStatistics[index] > 1)
            {
                m_PointsRemaining += 1;
                m_PlayerStatistics[index] -= 1;
            }
        }

        private void IncrementStatistic(StatisticIndex index)
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
            m_sexIndex += 1;
            m_sexIndex %= 3;
        }

        private string GetJobDescription()
        {
            string jobDescription = m_Jobs[m_JobIndex].name + "\r\n" + m_Jobs[m_JobIndex].description + "\r\n";
            return jobDescription;
        }

        private void NextState(object sender, EventArgs eventArgs)
        {
            Sex m_Playersex = (Sex)m_sexIndex;

            Dictionary<NeedIndex, EntityNeed> needs = EntityNeed.GetFullRandomisedNeeds();

            EntityTemplate humanTemplate = EntityTemplateHandler.Get("Human");
            m_Player = WorldState.EntityHandler.Create(humanTemplate, needs, 1, m_Jobs[m_JobIndex], m_Playersex, Sexuality.Bisexual, new UnityEngine.Vector2Int(-1, -1),
                ObjectIcons.GetSprites("Jobs", m_Jobs[m_JobIndex].name).ToList(), null);

            m_Player.PlayerControlled = true;

            m_Player.AddItem(new ItemInstance(ItemHandler.GetSpecificItem("Lantern"), new UnityEngine.Vector2Int(-1, -1), true));
        }

        public override GameState GetNextState()
        {
            return new WorldCreationState(m_Player);
        }
    }
}
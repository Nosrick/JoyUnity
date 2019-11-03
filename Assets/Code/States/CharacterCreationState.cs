using JoyLib.Code.Collections;
using JoyLib.Code.Cultures;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Jobs;
using JoyLib.Code.Entities.Needs;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Graphics;
using JoyLib.Code.Rollers;
using System;
using System.Collections.Generic;

namespace JoyLib.Code.States
{
    public class CharacterCreationState : GameState
    {
        protected int m_PointsRemaining;
        protected Dictionary<string, int> m_PlayerStatistics;

        protected List<JobType> m_Jobs;
        protected int m_JobIndex;
        protected int m_SexIndex;

        protected Entity m_Player;

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
            EntityTemplate humanTemplate = EntityTemplateHandler.Get("Human");
            CultureType culture = CultureHandler.GetByCultureName("Human");
      
            BasicValueContainer<INeed> needs = new BasicValueContainer<INeed>();
            needs.Add(NeedHandler.GetRandomised("hunger"));

            IGrowingValue level = new ConcreteGrowingValue("level", 1, 100, 0, GlobalConstants.DEFAULT_SUCCESS_THRESHOLD,
                new StandardRoller(), new NonUniqueDictionary<INeed, float>());

            m_Player = WorldState.EntityHandler.Create(humanTemplate, needs, level, m_Jobs[m_JobIndex], culture.ChooseSex(), culture.ChooseSexuality(), new UnityEngine.Vector2Int(-1, -1),
                ObjectIconHandler.instance.GetSprites("Human", m_Jobs[m_JobIndex].Name), null);

            m_Player.PlayerControlled = true;

            m_Player.AddItem(WorldState.ItemHandler.CreateRandomItemOfType(new string[] { "light source" }, true));
        }

        public override GameState GetNextState()
        {
            return new WorldCreationState(m_Player);
        }
    }
}
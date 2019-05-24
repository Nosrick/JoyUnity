using JoyLib.Code.Cultures;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Items;
using JoyLib.Code.Entities.Jobs;
using JoyLib.Code.Entities.Needs;
using JoyLib.Code.Entities.Sexes;
using JoyLib.Code.Entities.Sexuality;
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
        protected Dictionary<string, int> m_PlayerStatistics;

        protected List<JobType> m_Jobs;
        protected int m_JobIndex;
        protected int m_sexIndex;

        protected Entity m_Player;

        public CharacterCreationState() : base()
        {
            m_PointsRemaining = 12;

            string[] names = EntityStatistic.Names;
            m_PlayerStatistics = new Dictionary<string, int>(names.Length);

            foreach (string index in names)
            {
                m_PlayerStatistics.Add(index, 4);
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
            //REPLACE THIS WITH ENTITY CONSTRUCTOR
            Dictionary<string, INeed> needs = new Dictionary<string, INeed>();
            INeed hunger = NeedHandler.GetRandomised("Hunger");
            needs.Add(hunger.Name, hunger);

            EntityTemplate humanTemplate = EntityTemplateHandler.Get("Human");
            CultureType culture = CultureHandler.GetByCultureName("Human");

            m_Player = WorldState.EntityHandler.Create(humanTemplate, needs, 1, m_Jobs[m_JobIndex], culture.ChooseSex(), culture.ChooseSexuality(), new UnityEngine.Vector2Int(-1, -1),
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
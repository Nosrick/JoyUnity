using System;
using System.Collections.Generic;
using System.Linq;
using JoyLib.Code.Collections;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Needs;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Rollers;
using UnityEngine;

namespace JoyLib.Code.Unity.GUI.Containers
{
    public class CharacterCreationScreen : MonoBehaviour
    {
        [SerializeField] protected GameManager m_GameManager;
        [SerializeField] protected StatisticWindow m_StatisticWindow;
        protected List<IEntityTemplate> Templates { get; set; }

        public void Awake()
        {
        }

        public void Update()
        {
            if (Templates is null)
            {
                Initialise();
            }
        }

        protected void Initialise()
        {
            Templates = m_GameManager.EntityTemplateHandler.Templates;
            
            m_StatisticWindow.SetStatistics(Templates[0].Statistics.Select(stat => new Tuple<string, int>(stat.Name, stat.Value)).ToList());
        }

        public IEntity CreatePlayer()
        {
            IEntityTemplate template = Templates[0];
            return m_GameManager.EntityFactory.CreateFromTemplate(
                template,
                new ConcreteGrowingValue(
                    "level",
                    1,
                    0,
                    0,
                    GlobalConstants.DEFAULT_SUCCESS_THRESHOLD,
                    new StandardRoller(),
                    new NonUniqueDictionary<INeed, float>()),
                GlobalConstants.NO_TARGET);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using JoyLib.Code.Collections;
using JoyLib.Code.Cultures;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Gender;
using JoyLib.Code.Entities.Needs;
using JoyLib.Code.Entities.Sexes;
using JoyLib.Code.Entities.Sexuality;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Rollers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JoyLib.Code.Unity.GUI
{
    public class CharacterCreationScreen : MonoBehaviour
    {
        [SerializeField] protected GameManager m_GameManager;
        [SerializeField] protected StatisticWindow m_StatisticWindow;
        
        [SerializeField] protected Image PlayerSprite;
        [SerializeField] protected TMP_InputField PlayerName;
        [SerializeField] protected ConstrainedValueContainer PlayerType;
        [SerializeField] protected ConstrainedValueContainer GenderContainer;
        [SerializeField] protected ConstrainedValueContainer SexContainer;
        [SerializeField] protected ConstrainedValueContainer SexualityContainer;
        [SerializeField] protected ConstrainedValueContainer JobContainer;
        [SerializeField] protected ConstrainedValueContainer RomanceContainer;
        public List<IEntityTemplate> Templates { get; protected set; }

        public IEntityTemplate CurrentTemplate
        {
            get => m_CurrentTemplate;
            set
            {
                m_CurrentTemplate = value;
            }
        }
        protected List<ICulture> CurrentCultures { get; set; }

        protected IEntityTemplate m_CurrentTemplate;
        protected RNG Roller { get; set; }

        public void Awake()
        {
            Templates = m_GameManager.EntityTemplateHandler.Templates;
            Roller = new RNG();
            int result = Roller.Roll(0, Templates.Count);
            ChangeTemplate(Templates[result]);
            PlayerType.Container = Templates.Select(t => t.CreatureType).ToList();
            PlayerType.Value = result;
            PlayerType.ValueChanged += ChangeTemplateHandler;
            JobContainer.ValueChanged += ChangeJobHandler;
        }

        protected void ChangeTemplate(IEntityTemplate template)
        {
            CurrentTemplate = template;
            CurrentCultures = m_GameManager.CultureHandler.GetByCreatureType(m_CurrentTemplate.CreatureType);
            //PlayerType.Container = Templates.Select(t => t.CreatureType).ToList();
            GenderContainer.Container = CurrentCultures.SelectMany(culture => culture.Genders).ToList();
            SexContainer.Container = CurrentCultures.SelectMany(culture => culture.Sexes).ToList();
            SexualityContainer.Container = CurrentCultures.SelectMany(culture => culture.Sexualities).ToList();
            JobContainer.Container = CurrentCultures.SelectMany(culture => culture.Jobs).ToList();
            RomanceContainer.Container = CurrentCultures.SelectMany(culture => culture.RomanceTypes).ToList();

            PlayerSprite.sprite = m_GameManager.ObjectIconHandler.GetSprite(
                m_CurrentTemplate.Tileset, 
                JobContainer.Selected);
            PlayerName.text = GetRandomName();
            SetStatistics();
        }

        protected void ChangeTemplateHandler(object sender, EventArgs args)
        {
            ChangeTemplate(m_GameManager.EntityTemplateHandler.Get(PlayerType.Selected));
        }

        protected void ChangeJobHandler(object sender, EventArgs args)
        {
            PlayerSprite.sprite =
                m_GameManager.ObjectIconHandler.GetSprite(CurrentTemplate.CreatureType, JobContainer.Selected);
        }

        protected void SetStatistics()
        {
            m_StatisticWindow.SetStatistics(CurrentTemplate.Statistics.Select(stat => new Tuple<string, int>(stat.Name, stat.Value)).ToList());
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
                GlobalConstants.NO_TARGET,
                CurrentCultures,
                m_GameManager.GenderHandler.Get(GenderContainer.Selected),
                m_GameManager.BioSexHandler.Get(SexContainer.Selected),
                m_GameManager.SexualityHandler.Get(SexualityContainer.Selected),
                m_GameManager.RomanceHandler.Get(RomanceContainer.Selected),
                m_GameManager.JobHandler.Get(JobContainer.Selected),
                m_GameManager.ObjectIconHandler.GetSprites(m_CurrentTemplate.CreatureType, JobContainer.Selected));
        }

        public void SetRandomName()
        {
            PlayerName.text = GetRandomName();
        }

        public string GetRandomName()
        {
            return CurrentCultures[Roller.Roll(0, CurrentCultures.Count)].GetRandomName(GenderContainer.Selected);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using JoyLib.Code.Collections;
using JoyLib.Code.Cultures;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Gender;
using JoyLib.Code.Entities.Jobs;
using JoyLib.Code.Entities.Needs;
using JoyLib.Code.Entities.Romance;
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
        [SerializeField] protected ConstrainedValueContainer CultureContainer;
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
        protected ICulture CurrentCulture { get; set; }

        protected IEntityTemplate m_CurrentTemplate;
        protected RNG Roller { get; set; }

        public void Initialise()
        {
            Templates = m_GameManager.EntityTemplateHandler.Templates;
            Roller = new RNG();
            int result = Roller.Roll(0, Templates.Count);
            ChangeTemplate(Templates[result]);
            PlayerType.Container = Templates.Select(t => t.CreatureType).ToList();
            PlayerType.Value = result;
            PlayerType.ValueChanged += new EventHandler(ChangeTemplateHandler);
            JobContainer.ValueChanged += new EventHandler(ChangeJobHandler);
        }

        protected void ChangeTemplate(IEntityTemplate template)
        {
            CurrentTemplate = template;
            CurrentCultures = m_GameManager.CultureHandler.GetByCreatureType(m_CurrentTemplate.CreatureType);
            CultureContainer.Container = CurrentCultures
                                            .Select(culture => culture.CultureName)
                                            .ToList();
            CurrentCulture = CurrentCultures[CultureContainer.Value];
            
            SexContainer.Container = CurrentCulture.Sexes.ToList();
            IBioSex sex = CurrentCulture.ChooseSex(m_GameManager.BioSexHandler.Sexes);
            Debug.Log("SEX: " + sex.Name);
            SexContainer.Value =
                SexContainer.Container.FindIndex(s => s.Equals(sex.Name, StringComparison.CurrentCulture));
            
            GenderContainer.Container = CurrentCulture.Genders.ToList();
            IGender gender = CurrentCulture.ChooseGender(sex, m_GameManager.GenderHandler.Genders);
            Debug.Log("GENDER: " + gender.Name);
            GenderContainer.Value =
                GenderContainer.Container.FindIndex(s => 
                    s.Equals(gender.Name, StringComparison.Ordinal));
            
            
            SexualityContainer.Container = CurrentCulture.Sexualities.ToList();
            ISexuality sexuality = CurrentCulture.ChooseSexuality(m_GameManager.SexualityHandler.Sexualities);
            Debug.Log("SEXUALITY: " + sexuality.Name);
            SexualityContainer.Value =
                SexualityContainer.Container.FindIndex(s => 
                    s.Equals(sexuality.Name,
                        StringComparison.OrdinalIgnoreCase));
            
            JobContainer.Container = CurrentCulture.Jobs.ToList();
            IJob job = CurrentCulture.ChooseJob(m_GameManager.JobHandler.Jobs);
            Debug.Log("JOB: "+ job.Name);
            JobContainer.Value =
                JobContainer.Container.FindIndex(s =>
                    s.Equals(job.Name,
                        StringComparison.OrdinalIgnoreCase));
            
            RomanceContainer.Container = CurrentCulture.RomanceTypes.ToList();
            IRomance romance = CurrentCulture.ChooseRomance(m_GameManager.RomanceHandler.Romances);
            Debug.Log("ROMANCE: " + romance.Name);
            RomanceContainer.Value =
                RomanceContainer.Container.FindIndex(s => s.Equals(
                    romance.Name, StringComparison.OrdinalIgnoreCase));

            PlayerSprite.sprite = m_GameManager.ObjectIconHandler.GetSprite(
                CurrentCulture.Tileset, 
                JobContainer.Selected);
            PlayerName.text = CurrentCulture.GetRandomName(GenderContainer.Selected);
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

        protected void ChangeCultureHandler(object sender, EventArgs args)
        {
            ChangeTemplate(m_CurrentTemplate);
        }

        protected void SetStatistics()
        {
            m_StatisticWindow.SetStatistics(CurrentTemplate.Statistics.Select(stat => new Tuple<string, int>(stat.Key, stat.Value.Value)).ToList());
        }

        public IEntity CreatePlayer()
        {
            return m_GameManager.EntityFactory.CreateFromTemplate(
                CurrentTemplate,
                GlobalConstants.NO_TARGET,
                new ConcreteGrowingValue(
                    "level",
                    1,
                    0,
                    0,
                    GlobalConstants.DEFAULT_SUCCESS_THRESHOLD,
                    new StandardRoller(),
                    new NonUniqueDictionary<INeed, float>()),
                m_StatisticWindow.GetStatistics(),
                CurrentCultures,
                m_GameManager.GenderHandler.Get(GenderContainer.Selected),
                m_GameManager.BioSexHandler.Get(SexContainer.Selected),
                m_GameManager.SexualityHandler.Get(SexualityContainer.Selected),
                m_GameManager.RomanceHandler.Get(RomanceContainer.Selected),
                m_GameManager.JobHandler.Get(JobContainer.Selected), m_GameManager.ObjectIconHandler.GetSprites(CurrentTemplate.CreatureType, JobContainer.Selected));
        }

        public void SetRandomName()
        {
            PlayerName.text = CurrentCulture.GetRandomName(GenderContainer.Selected);
        }
    }
}
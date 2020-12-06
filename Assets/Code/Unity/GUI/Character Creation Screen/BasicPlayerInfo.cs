using System;
using System.Collections.Generic;
using System.Linq;
using JoyLib.Code.Cultures;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Gender;
using JoyLib.Code.Entities.Jobs;
using JoyLib.Code.Entities.Romance;
using JoyLib.Code.Entities.Sexes;
using JoyLib.Code.Entities.Sexuality;
using JoyLib.Code.Events;
using JoyLib.Code.Rollers;
using UnityEngine;

namespace JoyLib.Code.Unity.GUI
{
    public class BasicPlayerInfo : MonoBehaviour
    {
        [SerializeField] protected GameManager GameManager;
        [SerializeField] protected StatisticWindow StatisticWindow;
        
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
            get;
            protected set;
        }
        public List<ICulture> CurrentCultures { get; protected set; }
        public ICulture CurrentCulture { get; protected set; }
        protected RNG Roller { get; set; }

        public string Gender => GenderContainer.Selected;
        public string Sex => SexContainer.Selected;
        public string Sexuality => SexualityContainer.Selected;
        public string Romance => RomanceContainer.Selected;
        public string Job => JobContainer.Selected;

        public event EventHandler JobChanged;
        public event EventHandler CultureChanged;

        public void Initialise()
        {
            Templates = GameManager.EntityTemplateHandler.Templates;
            Roller = new RNG();
            int result = Roller.Roll(0, Templates.Count);
            ChangeTemplate(Templates[result]);
            PlayerType.Container = Templates.Select(t => t.CreatureType).ToList();
            PlayerType.Value = result;
            PlayerType.ValueChanged += ChangeTemplateHandler;
            CultureContainer.ValueChanged += ChangeCultureHandler;
            JobContainer.ValueChanged += ChangeJobHandler;
        }
        
        protected void ChangeTemplate(IEntityTemplate template)
        {
            CurrentTemplate = template;
            CurrentCultures = GameManager.CultureHandler.GetByCreatureType(CurrentTemplate.CreatureType);
            CultureContainer.Container = CurrentCultures
                                            .Select(culture => culture.CultureName)
                                            .ToList();
            CurrentCulture = CurrentCultures[CultureContainer.Value];
            SetCultureSpecificData(CurrentCulture);
        }

        protected void ChangeTemplateHandler(object sender, ValueChangedEventArgs args)
        {
            ChangeTemplate(GameManager.EntityTemplateHandler.Get(PlayerType.Selected));
        }

        protected void ChangeJobHandler(object sender, ValueChangedEventArgs args)
        {
        }

        protected void ChangeCultureHandler(object sender, ValueChangedEventArgs args)
        {
            SetCultureSpecificData(CurrentCultures.First(
                culture => culture.CultureName.Equals(CultureContainer.Selected)));
        }

        protected void SetCultureSpecificData(ICulture culture)
        {
            CurrentCulture = culture;
            SexContainer.Container = CurrentCulture.Sexes.ToList();
            IBioSex sex = CurrentCulture.ChooseSex(GameManager.BioSexHandler.Sexes);
            Debug.Log("SEX: " + sex.Name);
            SexContainer.Value =
                SexContainer.Container.FindIndex(s => s.Equals(sex.Name, StringComparison.CurrentCulture));
            
            GenderContainer.Container = CurrentCulture.Genders.ToList();
            IGender gender = CurrentCulture.ChooseGender(sex, GameManager.GenderHandler.Genders);
            Debug.Log("GENDER: " + gender.Name);
            GenderContainer.Value =
                GenderContainer.Container.FindIndex(s => 
                    s.Equals(gender.Name, StringComparison.Ordinal));
            
            
            SexualityContainer.Container = CurrentCulture.Sexualities.ToList();
            ISexuality sexuality = CurrentCulture.ChooseSexuality(GameManager.SexualityHandler.Sexualities);
            Debug.Log("SEXUALITY: " + sexuality.Name);
            SexualityContainer.Value =
                SexualityContainer.Container.FindIndex(s => 
                    s.Equals(sexuality.Name,
                        StringComparison.OrdinalIgnoreCase));
            
            JobContainer.Container = CurrentCulture.Jobs.ToList();
            IJob job = CurrentCulture.ChooseJob(GameManager.JobHandler.Jobs);
            Debug.Log("JOB: "+ job.Name);
            JobContainer.Value =
                JobContainer.Container.FindIndex(s =>
                    s.Equals(job.Name,
                        StringComparison.OrdinalIgnoreCase));
            
            RomanceContainer.Container = CurrentCulture.RomanceTypes.ToList();
            IRomance romance = CurrentCulture.ChooseRomance(GameManager.RomanceHandler.Romances);
            Debug.Log("ROMANCE: " + romance.Name);
            RomanceContainer.Value =
                RomanceContainer.Container.FindIndex(s => s.Equals(
                    romance.Name, StringComparison.OrdinalIgnoreCase));
            
            JobChanged?.Invoke(this, EventArgs.Empty);
            CultureChanged?.Invoke(this, EventArgs.Empty);
            SetStatistics();
        }

        protected void SetStatistics()
        {
            StatisticWindow.SetStatistics(CurrentTemplate.Statistics.Select(stat => new Tuple<string, int>(stat.Key, stat.Value.Value)).ToList());
        }
    }
}
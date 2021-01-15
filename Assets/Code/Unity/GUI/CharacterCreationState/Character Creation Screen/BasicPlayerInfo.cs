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
        [SerializeField] protected StatisticWindow StatisticWindow;
        
        [SerializeField] protected ConstrainedValueContainer PlayerType;
        [SerializeField] protected ConstrainedValueContainer CultureContainer;
        [SerializeField] protected ConstrainedValueContainer GenderContainer;
        [SerializeField] protected ConstrainedValueContainer SexContainer;
        [SerializeField] protected ConstrainedValueContainer SexualityContainer;
        [SerializeField] protected ConstrainedValueContainer JobContainer;
        [SerializeField] protected ConstrainedValueContainer RomanceContainer;
        
        public List<IEntityTemplate> Templates { get; protected set; }
        protected IGameManager GameManager { get; set; }

        public IEntityTemplate CurrentTemplate
        {
            get;
            protected set;
        }
        public List<ICulture> CurrentCultures { get; protected set; }
        public ICulture CurrentCulture { get; protected set; }
        protected RNG Roller { get; set; }

        public string Gender => this.GenderContainer.Selected;
        public string Sex => this.SexContainer.Selected;
        public string Sexuality => this.SexualityContainer.Selected;
        public string Romance => this.RomanceContainer.Selected;
        public string Job => this.JobContainer.Selected;

        public event EventHandler JobChanged;
        public event EventHandler CultureChanged;

        public void Initialise()
        {
            this.GameManager = GlobalConstants.GameManager;
            this.Templates = GlobalConstants.GameManager.EntityTemplateHandler.Templates.ToList();
            this.Roller = new RNG();
            int result = this.Roller.Roll(0, this.Templates.Count);
            this.ChangeTemplate(this.Templates[result]);
            this.PlayerType.Container = this.Templates.Select(t => t.CreatureType).ToList();
            this.PlayerType.Value = result;
            this.PlayerType.ValueChanged += this.ChangeTemplateHandler;
            this.CultureContainer.ValueChanged += this.ChangeCultureHandler;
            //this.JobContainer.ValueChanged += this.ChangeJobHandler;
        }
        
        protected void ChangeTemplate(IEntityTemplate template)
        {
            this.CurrentTemplate = template;
            this.CurrentCultures = this.GameManager.CultureHandler.GetByCreatureType(this.CurrentTemplate.CreatureType);
            this.CultureContainer.Container = this.CurrentCultures
                                            .Select(culture => culture.CultureName)
                                            .ToList();
            this.CurrentCulture = this.CurrentCultures[this.CultureContainer.Value];
            this.SetCultureSpecificData(this.CurrentCulture);
        }

        protected void ChangeTemplateHandler(object sender, ValueChangedEventArgs args)
        {
            this.ChangeTemplate(this.GameManager.EntityTemplateHandler.Get(this.PlayerType.Selected));
        }

        protected void ChangeJobHandler(object sender, ValueChangedEventArgs args)
        {
            this.JobChanged?.Invoke(sender, args);
        }

        protected void ChangeCultureHandler(object sender, ValueChangedEventArgs args)
        {
            this.SetCultureSpecificData(this.CurrentCultures.First(
                culture => culture.CultureName.Equals(this.CultureContainer.Selected)));
        }

        protected void SetCultureSpecificData(ICulture culture)
        {
            this.CurrentCulture = culture;
            this.SexContainer.Container = this.CurrentCulture.Sexes.ToList();
            IBioSex sex = this.CurrentCulture.ChooseSex(this.GameManager.BioSexHandler.Sexes);
            this.SexContainer.Value = this.SexContainer.Container.FindIndex(s => s.Equals(sex.Name, StringComparison.CurrentCulture));

            this.GenderContainer.Container = this.CurrentCulture.Genders.ToList();
            IGender gender = this.CurrentCulture.ChooseGender(sex, this.GameManager.GenderHandler.Genders);
            this.GenderContainer.Value = this.GenderContainer.Container.FindIndex(s => 
                    s.Equals(gender.Name, StringComparison.Ordinal));


            this.SexualityContainer.Container = this.CurrentCulture.Sexualities.ToList();
            ISexuality sexuality = this.CurrentCulture.ChooseSexuality(this.GameManager.SexualityHandler.Sexualities);
            this.SexualityContainer.Value = this.SexualityContainer.Container.FindIndex(s => 
                    s.Equals(sexuality.Name,
                        StringComparison.OrdinalIgnoreCase));

            this.JobContainer.Container = this.CurrentCulture.Jobs.ToList();
            IJob job = this.CurrentCulture.ChooseJob(this.GameManager.JobHandler.Jobs);
            this.JobContainer.Value = this.JobContainer.Container.FindIndex(s =>
                    s.Equals(job.Name,
                        StringComparison.OrdinalIgnoreCase));

            this.RomanceContainer.Container = this.CurrentCulture.RomanceTypes.ToList();
            IRomance romance = this.CurrentCulture.ChooseRomance(this.GameManager.RomanceHandler.Romances);
            this.RomanceContainer.Value = this.RomanceContainer.Container.FindIndex(s => s.Equals(
                    romance.Name, StringComparison.OrdinalIgnoreCase));

            this.JobChanged?.Invoke(this, EventArgs.Empty);
            this.CultureChanged?.Invoke(this, EventArgs.Empty);
            this.SetStatistics();

            this.GameManager.GUIManager.SetUIColours(
                this.CurrentCulture.BackgroundColours,
                this.CurrentCulture.CursorColours,
                this.CurrentCulture.AccentBackgroundColours,
                this.CurrentCulture.AccentFontColour);
        }

        protected void SetStatistics()
        {
            this.StatisticWindow.SetStatistics(this.CurrentTemplate.Statistics.Select(stat => new Tuple<string, int>(stat.Key, stat.Value.Value)).ToList());
        }
    }
}
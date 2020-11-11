using System.Collections.Generic;
using JoyLib.Code.Cultures;
using JoyLib.Code.Collections;
using JoyLib.Code.Entities.Needs;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.World;
using JoyLib.Code.Rollers;
using JoyLib.Code.Graphics;
using JoyLib.Code.Entities.Jobs;
using JoyLib.Code.Entities.Sexes;
using JoyLib.Code.Entities.Sexuality;
using JoyLib.Code.Entities.Items;
using JoyLib.Code.Entities.AI.Drivers;
using JoyLib.Code.Entities.Romance;
using UnityEngine;

namespace JoyLib.Code.Entities
{
    public class EntityFactory
    {
        protected GameObject GameManager { get; set; }
        protected NeedHandler NeedHandler { get; set; }

        protected ObjectIconHandler ObjectIcons { get; set; }

        protected CultureHandler CultureHandler { get; set; }

        protected EntitySexualityHandler SexualityHandler { get; set; }

        protected EntityBioSexHandler BioSexHandler { get; set; }
        
        protected EntityRomanceHandler RomanceHandler { get; set; }

        protected JobHandler JobHandler { get; set; }

        public EntityFactory()
        {
            Initialise();
        }

        protected void Initialise()
        {
            if(GameManager is null)
            {
                GameManager = GameObject.Find("GameManager");
                NeedHandler = GameManager.GetComponent<NeedHandler>();
                ObjectIcons = GameManager.GetComponent<ObjectIconHandler>();
                CultureHandler = GameManager.GetComponent<CultureHandler>();
                SexualityHandler = GameManager.GetComponent<EntitySexualityHandler>();
                BioSexHandler = GameManager.GetComponent<EntityBioSexHandler>();
                JobHandler = GameManager.GetComponent<JobHandler>();
                RomanceHandler = GameManager.GetComponent<EntityRomanceHandler>();
            }
        }

        public Entity CreateFromTemplate(
            EntityTemplate template,
            IGrowingValue level,
            Vector2Int position,
            List<ICulture> cultures = null,
            IBioSex sex = null,
            ISexuality sexuality = null,
            IRomance romance = null,
            IJob job = null,
            Sprite[] sprites = null,
            WorldInstance world = null,
            IDriver driver = null)
        {
            if (GameManager is null)
            {
                Initialise();
            }
            
            IJob selectedJob = job;
            IBioSex selectedSex = sex;
            ISexuality selectedSexuality = sexuality;
            IRomance selectedRomance = romance;
            Sprite[] selectedSprites = sprites;
            List<ICulture> creatureCultures = new List<ICulture>();
            IDriver selectedDriver = driver;
            if (!(cultures is null))
            {
                creatureCultures.AddRange(cultures);
            }
            else
            {
                creatureCultures = new List<ICulture>();
                List<ICulture> cultureTypes = CultureHandler.GetByCreatureType(template.CreatureType);
                creatureCultures.AddRange(cultureTypes);
            }

            BasicValueContainer<INeed> needs = new BasicValueContainer<INeed>();

            foreach (string need in template.Needs)
            {
                needs.Add(NeedHandler.GetRandomised(need));
            }

            int result = RNG.instance.Roll(0, creatureCultures.Count);
            ICulture dominantCulture = creatureCultures[result];

            if(selectedJob is null)
            {
                selectedJob = dominantCulture.ChooseJob(JobHandler.Jobs);
            }

            if(selectedSex is null)
            {
                selectedSex = dominantCulture.ChooseSex(BioSexHandler.Sexes);
            }

            if (selectedRomance is null)
            {
                selectedRomance = dominantCulture.ChooseRomance(RomanceHandler.Romances);
            }

            if(selectedSexuality is null)
            {
                selectedSexuality = dominantCulture.ChooseSexuality(SexualityHandler.Sexualities);
            }

            if(selectedSprites is null)
            {
                selectedSprites = ObjectIcons.GetSprites(template.Tileset, template.JoyType);
            }

            if(selectedDriver is null)
            {
                selectedDriver = new StandardDriver();
            }

            Entity entity = new Entity(
                template, 
                needs, 
                creatureCultures, 
                level, 
                selectedJob, 
                selectedSex, 
                selectedSexuality, 
                selectedRomance,
                position, 
                selectedSprites, 
                world,
                selectedDriver);

            return entity;
        }

        public Entity CreateLong(
            EntityTemplate template,
            BasicValueContainer<INeed> needs,
            IGrowingValue level,
            float experience,
            JobType job,
            IBioSex sex,
            ISexuality sexuality,
            IRomance romance,
            Vector2Int position,
            Sprite[] sprites,
            ItemInstance naturalWeapons,
            NonUniqueDictionary<string, ItemInstance> equipment,
            List<ItemInstance> backpack,
            List<string> identifiedItems,
            Dictionary<string, int> jobLevels,
            WorldInstance world,
            List<ICulture> cultures = null,
            IDriver driver = null)
        {
            if (GameManager is null)
            {
                Initialise();
            }
            
            List<ICulture> creatureCultures = new List<ICulture>();
            if (cultures != null)
            {
                creatureCultures.AddRange(cultures);
            }
            else
            {
                List<ICulture> cultureTypes = CultureHandler.GetByCreatureType(template.CreatureType);
                creatureCultures.AddRange(cultureTypes);
            }

            IDriver selectedDriver = driver;
            if(selectedDriver is null)
            {
                selectedDriver = new StandardDriver();
            }

            Entity entity = new Entity(
                template, 
                needs, 
                creatureCultures, 
                level, 
                experience, 
                job, 
                sex, 
                sexuality, 
                romance,
                position,
                sprites, 
                naturalWeapons, 
                equipment, 
                backpack, 
                identifiedItems, 
                jobLevels, 
                world,
                selectedDriver);

            return entity;
        }
    }
}
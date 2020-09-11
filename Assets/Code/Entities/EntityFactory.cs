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
using UnityEngine;

namespace JoyLib.Code.Entities
{
    public class EntityFactory
    {
        protected static GameObject s_GameManager;
        protected static NeedHandler s_NeedHandler;

        protected static ObjectIconHandler s_ObjectIcons;

        protected static CultureHandler s_CultureHandler;

        protected static EntitySexualityHandler s_SexualityHandler;

        protected static EntityBioSexHandler s_BioSexHandler;

        protected static JobHandler s_JobHandler;

        public EntityFactory()
        {
            if(s_GameManager is null)
            {
                s_GameManager = GameObject.Find("GameManager");
                s_NeedHandler = s_GameManager.GetComponent<NeedHandler>();
                s_ObjectIcons = s_GameManager.GetComponent<ObjectIconHandler>();
                s_CultureHandler = s_GameManager.GetComponent<CultureHandler>();
                s_SexualityHandler = s_GameManager.GetComponent<EntitySexualityHandler>();
                s_BioSexHandler = s_GameManager.GetComponent<EntityBioSexHandler>();
                s_JobHandler = s_GameManager.GetComponent<JobHandler>();
            }

        }

        public Entity CreateFromTemplate(
            EntityTemplate template,
            IGrowingValue level,
            Vector2Int position,
            List<CultureType> cultures = null,
            IBioSex sex = null,
            ISexuality sexuality = null,
            JobType job = null,
            Sprite[] sprites = null,
            WorldInstance world = null,
            IDriver driver = null)
        {
            JobType selectedJob = job;
            IBioSex selectedSex = sex;
            ISexuality selectedSexuality = sexuality;
            Sprite[] selectedSprites = sprites;
            List<CultureType> creatureCultures = new List<CultureType>();
            IDriver selectedDriver = driver;
            if (!(cultures is null))
            {
                creatureCultures.AddRange(cultures);
            }
            else
            {
                creatureCultures = new List<CultureType>();
                List<CultureType> cultureTypes = s_CultureHandler.GetByCreatureType(template.CreatureType);
                creatureCultures.AddRange(cultureTypes);
            }

            BasicValueContainer<INeed> needs = new BasicValueContainer<INeed>();

            foreach (string need in template.Needs)
            {
                needs.Add(s_NeedHandler.GetRandomised(need));
            }

            int result = RNG.instance.Roll(0, creatureCultures.Count);
            CultureType dominantCulture = creatureCultures[result];

            if(selectedJob is null)
            {
                selectedJob = dominantCulture.ChooseJob(s_JobHandler.Jobs);
            }

            if(selectedSex is null)
            {
                selectedSex = dominantCulture.ChooseSex(s_BioSexHandler.Sexes);
            }

            if(selectedSexuality is null)
            {
                selectedSexuality = dominantCulture.ChooseSexuality(s_SexualityHandler.Sexualities);
            }

            if(selectedSprites is null)
            {
                selectedSprites = s_ObjectIcons.GetSprites(template.Tileset, template.JoyType);
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
            Vector2Int position,
            Sprite[] sprites,
            ItemInstance naturalWeapons,
            NonUniqueDictionary<string, ItemInstance> equipment,
            List<ItemInstance> backpack,
            List<string> identifiedItems,
            Dictionary<string, int> jobLevels,
            WorldInstance world,
            List<CultureType> cultures = null,
            IDriver driver = null)
        {
            List<CultureType> creatureCultures = new List<CultureType>();
            if (cultures != null)
            {
                creatureCultures.AddRange(cultures);
            }
            else
            {
                List<CultureType> cultureTypes = s_CultureHandler.GetByCreatureType(template.CreatureType);
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
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
using UnityEngine;

namespace JoyLib.Code.Entities
{
    public class EntityFactory
    {
        protected static NeedHandler s_NeedHandler = GameObject.Find("GameManager")
                                                        .GetComponent<NeedHandler>();

        protected static ObjectIconHandler s_ObjectIcons = GameObject.Find("GameManager")
                                                            .GetComponent<ObjectIconHandler>();

        protected static CultureHandler s_CultureHandler = GameObject.Find("GameManager")
                                                            .GetComponent<CultureHandler>();

        public Entity CreateRandomFromTemplate(EntityTemplate template, IGrowingValue level, Vector2Int position, WorldInstance world, List<CultureType> cultures = null)
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

            BasicValueContainer<INeed> needs = new BasicValueContainer<INeed>();
            needs.Add(s_NeedHandler.GetRandomised("hunger"));
            needs.Add(s_NeedHandler.GetRandomised("thirst"));
            needs.Add(s_NeedHandler.GetRandomised("sex"));

            int result = RNG.instance.Roll(0, creatureCultures.Count - 1);
            CultureType dominantCulture = creatureCultures[result];

            Entity entity = new Entity(template, needs, creatureCultures, level, dominantCulture.ChooseJob(), dominantCulture.ChooseSex(),
                dominantCulture.ChooseSexuality(), position, s_ObjectIcons.GetSprites(template.Tileset, template.CreatureType), world);

            return entity;
        }

        public Entity Create(EntityTemplate template, BasicValueContainer<INeed> needs, IGrowingValue level, JobType job, IBioSex sex, ISexuality sexuality,
            Vector2Int position, Sprite[] sprites, WorldInstance world, List<CultureType> cultures = null)
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

            Entity entity = new Entity(template, needs, creatureCultures, level, job, sex, sexuality, position, sprites, world);

            return entity;
        }

        public Entity CreateLong(EntityTemplate template, BasicValueContainer<INeed> needs, IGrowingValue level, float experience, JobType job, IBioSex sex, ISexuality sexuality,
            Vector2Int position, Sprite[] sprites, ItemInstance naturalWeapons, NonUniqueDictionary<string, ItemInstance> equipment,
            List<ItemInstance> backpack, List<string> identifiedItems, Dictionary<string, int> jobLevels, WorldInstance world, 
            List<CultureType> cultures = null)
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

            Entity entity = new Entity(template, needs, creatureCultures, level, experience, job, sex, sexuality, position, 
                sprites, naturalWeapons, equipment, backpack, identifiedItems, jobLevels, world);

            return entity;
        }
    }
}
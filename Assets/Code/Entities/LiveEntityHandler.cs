using JoyLib.Code.Collections;
using JoyLib.Code.Cultures;
using JoyLib.Code.Entities.Items;
using JoyLib.Code.Entities.Jobs;
using JoyLib.Code.Entities.Needs;
using JoyLib.Code.Entities.Sexes;
using JoyLib.Code.Entities.Sexuality;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Graphics;
using JoyLib.Code.Rollers;
using JoyLib.Code.World;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace JoyLib.Code.Entities
{
    public class LiveEntityHandler
    {
        private static readonly Lazy<LiveEntityHandler> lazy = new Lazy<LiveEntityHandler>(() => new LiveEntityHandler());

        public static LiveEntityHandler instance => lazy.Value;

        private Dictionary<long, Entity> m_Entities = new Dictionary<long, Entity>();
        private Entity m_Player;

        public Entity CreateRandomFromTemplate(EntityTemplate template, IGrowingValue level, Vector2Int position, WorldInstance world, List<CultureType> cultures = null)
        {
            List<CultureType> creatureCultures = new List<CultureType>();
            if(cultures != null)
            {
                creatureCultures.AddRange(cultures);
            }
            else
            {
                List<CultureType> cultureTypes = CultureHandler.instance.GetByCreatureType(template.CreatureType);
                creatureCultures.AddRange(cultureTypes);
            }

            BasicValueContainer<INeed> needs = new BasicValueContainer<INeed>();
            needs.Add(NeedHandler.instance.GetRandomised("hunger"));

            int result = RNG.Roll(0, creatureCultures.Count - 1);
            CultureType dominantCulture = creatureCultures[result];

            Entity entity = new Entity(template, needs, creatureCultures, level, dominantCulture.ChooseJob(), dominantCulture.ChooseSex(), 
                dominantCulture.ChooseSexuality(), position, ObjectIconHandler.instance.GetSprites(template.Tileset, template.CreatureType), world);

            m_Entities.Add(entity.GUID, entity);

            if(entity.PlayerControlled)
            {
                m_Player = entity;
            }

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
                List<CultureType> cultureTypes = CultureHandler.instance.GetByCreatureType(template.CreatureType);
                creatureCultures.AddRange(cultureTypes);
            }

            Entity entity = new Entity(template, needs, creatureCultures, level, job, sex, sexuality, position, sprites, world);

            m_Entities.Add(entity.GUID, entity);

            if(entity.PlayerControlled)
            {
                m_Player = entity;
            }

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
                List<CultureType> cultureTypes = CultureHandler.instance.GetByCreatureType(template.CreatureType);
                creatureCultures.AddRange(cultureTypes);
            }

            Entity entity = new Entity(template, needs, creatureCultures, level, experience, job, sex, sexuality, position, 
                sprites, naturalWeapons, equipment, backpack, identifiedItems, jobLevels, world);

            m_Entities.Add(entity.GUID, entity);

            if (entity.PlayerControlled)
            {
                m_Player = entity;
            }

            return entity;
        }

        public void Remove(long GUID)
        {
            if(m_Entities.ContainsKey(GUID))
            {
                m_Entities.Remove(GUID);
            }
        }

        public Entity Get(long GUID)
        {
            if(m_Entities.ContainsKey(GUID))
            {
                return m_Entities[GUID];
            }
            return null;
        }

        public Entity GetPlayer()
        {
            return m_Player;
        }

        public void SetPlayer(Entity entity)
        {
            m_Player = entity;
        }
    }
}

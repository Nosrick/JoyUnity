using JoyLib.Code.Cultures;
using JoyLib.Code.Entities.Items;
using JoyLib.Code.Entities.Jobs;
using JoyLib.Code.Entities.Needs;
using JoyLib.Code.Entities.Sexes;
using JoyLib.Code.Entities.Sexuality;
using JoyLib.Code.Graphics;
using JoyLib.Code.Helpers;
using JoyLib.Code.World;
using System.Collections.Generic;
using UnityEngine;

namespace JoyLib.Code.Entities
{
    public class LiveEntityHandler
    {
        private Dictionary<long, Entity> m_Entities = new Dictionary<long, Entity>();
        private Entity m_Player;

        public Entity CreateRandomFromTemplate(EntityTemplate template, int level, Vector2Int position, WorldInstance world, List<CultureType> cultures = null)
        {
            List<CultureType> creatureCultures = new List<CultureType>();
            if(cultures != null)
            {
                creatureCultures.AddRange(cultures);
            }
            else
            {
                List<CultureType> cultureTypes = CultureHandler.GetByCreatureType(template.CreatureType);
                creatureCultures.AddRange(cultureTypes);
            }

            Dictionary<string, INeed> needs = new Dictionary<string, INeed>();
            needs.Add("Hunger", NeedHandler.GetRandomised("Hunger"));

            int result = RNG.Roll(0, creatureCultures.Count - 1);
            CultureType dominantCulture = creatureCultures[result];

            Entity entity = new Entity(template, needs, creatureCultures, level, dominantCulture.ChooseJob(), dominantCulture.ChooseSex(), 
                dominantCulture.ChooseSexuality(), position, ObjectIcons.GetSprites(template.Tileset, template.CreatureType), world);

            m_Entities.Add(entity.GUID, entity);

            if(entity.PlayerControlled)
            {
                m_Player = entity;
            }

            return entity;
        }

        public Entity Create(EntityTemplate template, Dictionary<string, INeed> needs, int level, JobType job, IBioSex sex, ISexuality sexuality,
            Vector2Int position, List<Sprite> sprites, WorldInstance world, List<CultureType> cultures = null)
        {
            List<CultureType> creatureCultures = new List<CultureType>();
            if (cultures != null)
            {
                creatureCultures.AddRange(cultures);
            }
            else
            {
                List<CultureType> cultureTypes = CultureHandler.GetByCreatureType(template.CreatureType);
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

        public Entity CreateLong(EntityTemplate template, Dictionary<string, INeed> needs, int level, float experience, JobType job, IBioSex sex, ISexuality sexuality,
            Vector2Int position, List<Sprite> sprites, ItemInstance naturalWeapons, Dictionary<string, ItemInstance> equipment,
            List<ItemInstance> backpack, List<string> identifiedItems, Dictionary<string, int> jobLevels, WorldInstance world, 
            string tileset, List<CultureType> cultures = null)
        {
            List<CultureType> creatureCultures = new List<CultureType>();
            if (cultures != null)
            {
                creatureCultures.AddRange(cultures);
            }
            else
            {
                List<CultureType> cultureTypes = CultureHandler.GetByCreatureType(template.CreatureType);
                creatureCultures.AddRange(cultureTypes);
            }

            Entity entity = new Entity(template, needs, creatureCultures, level, experience, job, sex, sexuality, position, 
                sprites, naturalWeapons, equipment, backpack, identifiedItems, jobLevels, world, tileset);

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

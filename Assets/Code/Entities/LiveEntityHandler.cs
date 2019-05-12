using JoyLib.Code.Entities.AI;
using JoyLib.Code.Entities.Items;
using JoyLib.Code.Entities.Jobs;
using JoyLib.Code.Entities.Needs;
using JoyLib.Code.World;
using System.Collections.Generic;
using UnityEngine;

namespace JoyLib.Code.Entities
{
    public class LiveEntityHandler
    {
        private Dictionary<long, Entity> m_Entities = new Dictionary<long, Entity>();
        private Entity m_Player;

        public Entity Create(EntityTemplate template, Dictionary<string, AbstractNeed> needs, int level, JobType job, Sex sex, Sexuality sexuality,
            Vector2Int position, List<Sprite> sprites, WorldInstance world)
        {
            Entity entity = new Entity(template, needs, level, job, sex, sexuality, position, sprites, world);

            m_Entities.Add(entity.GUID, entity);

            if(entity.PlayerControlled)
            {
                m_Player = entity;
            }

            return entity;
        }

        public Entity CreateLong(EntityTemplate template, Dictionary<string, AbstractNeed> needs, int level, float experience, JobType job, Sex sex, Sexuality sexuality,
            Vector2Int position, List<Sprite> sprites, ItemInstance naturalWeapons, Dictionary<string, ItemInstance> equipment,
            List<ItemInstance> backpack, Dictionary<long, int> relationships, List<string> identifiedItems, Dictionary<long, RelationshipStatus> family,
            Dictionary<string, int> jobLevels, WorldInstance world, string tileset)
        {
            Entity entity = new Entity(template, needs, level, experience, job, sex, sexuality, position, sprites, naturalWeapons, equipment, backpack, relationships, identifiedItems,
                family, jobLevels, world, tileset);

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

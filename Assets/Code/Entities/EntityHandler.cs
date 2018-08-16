using JoyLib.Code.Entities.AI;
using JoyLib.Code.Entities.Items;
using JoyLib.Code.Entities.Jobs;
using JoyLib.Code.Entities.Needs;
using JoyLib.Code.World;
using System.Collections.Generic;
using UnityEngine;

namespace JoyLib.Code.Entities
{
    public static class EntityHandler
    {
        private static Dictionary<long, Entity> s_Entities = new Dictionary<long, Entity>();

        public static Entity Create(EntityTemplate template, Dictionary<NeedIndex, EntityNeed> needs, int level, JobType job, Sex sex, Sexuality sexuality,
            Vector2Int position, List<Sprite> sprites, WorldInstance world)
        {
            Entity entity = new Entity(template, needs, level, job, sex, sexuality, position, sprites, world);
            s_Entities.Add(entity.GUID, entity);

            return entity;
        }

        public static Entity CreateLong(EntityTemplate template, Dictionary<NeedIndex, EntityNeed> needs, int level, float experience, JobType job, Sex sex, Sexuality sexuality,
            Vector2Int position, List<Sprite> sprites, ItemInstance naturalWeapons, Dictionary<string, ItemInstance> equipment,
            List<ItemInstance> backpack, Dictionary<long, int> relationships, List<string> identifiedItems, Dictionary<long, RelationshipStatus> family,
            Dictionary<string, int> jobLevels, WorldInstance world, string tileset)
        {
            Entity entity = new Entity(template, needs, level, experience, job, sex, sexuality, position, sprites, naturalWeapons, equipment, backpack, relationships, identifiedItems,
                family, jobLevels, world, tileset);

            s_Entities.Add(entity.GUID, entity);

            return entity;
        }

        public static Entity Get(long GUID)
        {
            if(s_Entities.ContainsKey(GUID))
            {
                return s_Entities[GUID];
            }
            return null;
        }
    }
}

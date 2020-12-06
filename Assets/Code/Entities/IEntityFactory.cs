using System.Collections.Generic;
using JoyLib.Code.Collections;
using JoyLib.Code.Cultures;
using JoyLib.Code.Entities.Abilities;
using JoyLib.Code.Entities.AI.Drivers;
using JoyLib.Code.Entities.Gender;
using JoyLib.Code.Entities.Items;
using JoyLib.Code.Entities.Jobs;
using JoyLib.Code.Entities.Needs;
using JoyLib.Code.Entities.Romance;
using JoyLib.Code.Entities.Sexes;
using JoyLib.Code.Entities.Sexuality;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.World;
using UnityEngine;

namespace JoyLib.Code.Entities
{
    public interface IEntityFactory
    {
        IEntity CreateFromTemplate(IEntityTemplate template,
            Vector2Int position,
            IGrowingValue level = null,
            BasicValueContainer<IRollableValue> statistics = null,
            BasicValueContainer<IGrowingValue> skills = null,
            IEnumerable<IAbility> abilities = null,
            List<ICulture> cultures = null,
            IGender gender = null,
            IBioSex sex = null,
            ISexuality sexuality = null,
            IRomance romance = null,
            IJob job = null,
            Sprite[] sprites = null,
            IWorldInstance world = null,
            IDriver driver = null);

        IEntity CreateLong(IEntityTemplate template,
            BasicValueContainer<IRollableValue> stats,
            BasicValueContainer<INeed> needs,
            BasicValueContainer<IGrowingValue> skills, 
            IEnumerable<IAbility> abilities,
            IGrowingValue level,
            float experience,
            IJob job,
            IGender gender,
            IBioSex sex,
            ISexuality sexuality,
            IRomance romance,
            Vector2Int position,
            Sprite[] sprites,
            IItemInstance naturalWeapons,
            NonUniqueDictionary<string, IItemInstance> equipment,
            List<IItemInstance> backpack,
            List<string> identifiedItems,
            Dictionary<string, int> jobLevels,
            IWorldInstance world,
            List<ICulture> cultures = null,
            IDriver driver = null);
    }
}
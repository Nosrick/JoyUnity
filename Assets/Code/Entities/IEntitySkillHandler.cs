using System.Collections.Generic;
using JoyLib.Code.Collections;
using JoyLib.Code.Entities.Needs;
using JoyLib.Code.Entities.Statistics;

namespace JoyLib.Code.Entities
{
    public interface IEntitySkillHandler
    {
        BasicValueContainer<IGrowingValue> GetDefaultSkillBlock(BasicValueContainer<INeed> needs);
        NonUniqueDictionary<INeed, float> GetEmptyCoefficients();
        NonUniqueDictionary<INeed, float> GetCoefficients(List<string> needNames, string skillName);

        /// <summary>
        /// Takes in the needs and skill name and spits out a NonUniqueDictionary for the skill
        /// </summary>
        /// <param name="container">Container of the entity's needs</param>
        /// <param name="skillName">The name of the skill in question</param>
        /// <returns></returns>
        NonUniqueDictionary<INeed, float> GetCoefficients(BasicValueContainer<INeed> needs, string skillName);
    }
}
using System.Collections.Generic;
using JoyLib.Code.Collections;
using JoyLib.Code.Entities.Needs;
using JoyLib.Code.Entities.Statistics;

namespace JoyLib.Code.Entities
{
    public interface IEntitySkillHandler
    {
        IEnumerable<string> SkillsNames { get; }
        
        IDictionary<string, EntitySkill> GetDefaultSkillBlock(IEnumerable<INeed> needs);
        NonUniqueDictionary<INeed, float> GetEmptyCoefficients();
        NonUniqueDictionary<INeed, float> GetCoefficients(IEnumerable<string> needNames, string skillName);

        /// <summary>
        /// Takes in the needs and skill name and spits out a NonUniqueDictionary for the skill
        /// </summary>
        /// <param name="needs"></param>
        /// <param name="skillName">The name of the skill in question</param>
        /// <param name="container">Container of the entity's needs</param>
        /// <returns></returns>
        NonUniqueDictionary<INeed, float> GetCoefficients(IEnumerable<INeed> needs, string skillName);
    }
}
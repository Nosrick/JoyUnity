using JoyLib.Code.Entities.Needs;
using JoyLib.Code.Loaders;
using System.Collections.Generic;

namespace JoyLib.Code.Entities
{
    public static class EntitySkillHandler
    {
        private static Dictionary<string, List<string>> s_GoverningNeeds = GoverningNeedsLoader.GetGoverningNeeds();

        private static Dictionary<string, Dictionary<string, float>> s_SkillCoefficients = SkillCoefficientLoader.LoadSkillCoefficients();
        
        public static Dictionary<string, EntitySkill> GetSkillBlock(Dictionary<string, INeed> governingNeeds)
        {
            Dictionary<string, EntitySkill> skills = new Dictionary<string, EntitySkill>();
            
            foreach(string key in s_SkillCoefficients.Keys)
            {
                skills.Add(key, new EntitySkill(1, GlobalConstants.DEFAULT_SUCCESS_THRESHOLD, 0, s_SkillCoefficients[key], governingNeeds));
            }

            return skills;
        }
    }
}

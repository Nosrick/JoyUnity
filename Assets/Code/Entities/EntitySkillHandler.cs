using JoyLib.Code.Loaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JoyLib.Code.Entities
{
    public static class EntitySkillHandler
    {
        private static Dictionary<string, List<NeedIndex>> s_GoverningNeeds = GoverningNeedsLoader.GetGoverningNeeds();

        private static Dictionary<string, Dictionary<NeedIndex, float>> s_SkillCoefficients = SkillCoefficientLoader.LoadSkillCoefficients();
        
        public static Dictionary<string, EntitySkill> GetSkillBlock(Dictionary<NeedIndex, EntityNeed> governingNeeds)
        {
            Dictionary<string, EntitySkill> skills = new Dictionary<string, EntitySkill>();
            
            foreach(string key in s_SkillCoefficients.Keys)
            {
                skills.Add(key, new EntitySkill(1, 0, s_SkillCoefficients[key], governingNeeds));
            }

            return skills;
        }
    }
}

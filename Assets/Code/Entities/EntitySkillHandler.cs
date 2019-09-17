using JoyLib.Code.Collections;
using JoyLib.Code.Entities.Needs;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Helpers;
using JoyLib.Code.Rollers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace JoyLib.Code.Entities
{
    public static class EntitySkillHandler
    {
        private static Dictionary<string, List<Tuple<string, float>>> s_SkillCoefficients = LoadSkillCoefficients();
        
        public static BasicValueContainer<IGrowingValue> GetDefaultSkillBlock(BasicValueContainer<INeed> needs)
        {
            BasicValueContainer<IGrowingValue> skills = new BasicValueContainer<IGrowingValue>();
            
            foreach(string key in s_SkillCoefficients.Keys)
            {
                NonUniqueDictionary<INeed, float> coefficients = GetCoefficients(needs, key);

                NonUniqueDictionary<INeed, float> governingNeeds = new NonUniqueDictionary<INeed, float>();
                foreach(Tuple<INeed, float> coefficient in coefficients)
                {
                    if(needs.Has(coefficient.Item1.Name))
                    {
                        governingNeeds.Add(needs[coefficient.Item1.Name], coefficient.Item2);
                    }
                }

                skills.Add(new EntitySkill(key, 0, GlobalConstants.DEFAULT_SUCCESS_THRESHOLD, 0, governingNeeds, new StandardRoller()));
            }

            return skills;
        }

        /// <summary>
        /// Takes in the needs and skill name and spits out a Dictionary for the skill
        /// </summary>
        /// <param name="container">Container of the entity's needs</param>
        /// <param name="skillName">The name of the skill in question</param>
        /// <returns></returns>
        public static NonUniqueDictionary<INeed, float> GetCoefficients(BasicValueContainer<INeed> container, string skillName)
        {
            if(s_SkillCoefficients.ContainsKey(skillName))
            {
                NonUniqueDictionary<INeed, float> coefficients = new NonUniqueDictionary<INeed, float>();

                foreach(string key in s_SkillCoefficients.Keys)
                {
                    if(key == skillName)
                    {
                        foreach(Tuple<string, float> tuple in s_SkillCoefficients[key])
                        {
                            INeed need = container[tuple.Item1];
                            coefficients.Add(need, tuple.Item2);
                        }
                    }
                }

                return coefficients;
            }
            throw new InvalidOperationException("Attempted to get coefficients for non-existent skill " + skillName);
        }

        private static Dictionary<string, List<Tuple<string, float>>> LoadSkillCoefficients()
        {
            Dictionary<string, List<Tuple<string, float>>> skillCoefficients = new Dictionary<string, List<Tuple<string, float>>>();

            XElement doc = XElement.Load(Directory.GetCurrentDirectory() + GlobalConstants.DATA_FOLDER + "SkillCoefficients.xml");

            foreach (XElement skill in doc.Elements("Skill"))
            {
                List<Tuple<string, float>> coefficients = (from coefficient in skill.Elements("Coefficient")
                                                           select new Tuple<string, float>(
                                                               coefficient.Element("Name").DefaultIfEmpty("DEFAULT").ToLower(),
                                                               coefficient.Element("Value").DefaultIfEmpty(0.0f))).ToList();

                skillCoefficients.Add(skill.Element("Name").DefaultIfEmpty("DEFAULT").ToLower(), coefficients);
            }

            return skillCoefficients;
        }
    }
}

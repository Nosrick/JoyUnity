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
using UnityEngine;

namespace JoyLib.Code.Entities
{
    public class EntitySkillHandler : IEntitySkillHandler
    {
        private Dictionary<string, List<Tuple<string, float>>> m_SkillCoefficients;

        protected INeedHandler NeedHandler { get; set; }
        
        public EntitySkillHandler(INeedHandler needHandler)
        {
            NeedHandler = needHandler;
            m_SkillCoefficients = LoadSkillCoefficients();
        }

        public BasicValueContainer<IGrowingValue> GetDefaultSkillBlock(IEnumerable<INeed> needs)
        {
            BasicValueContainer<IGrowingValue> skills = new BasicValueContainer<IGrowingValue>();

            foreach(string key in m_SkillCoefficients.Keys)
            {
                NonUniqueDictionary<INeed, float> coefficients = GetCoefficients(needs, key);

                NonUniqueDictionary<INeed, float> governingNeeds = new NonUniqueDictionary<INeed, float>();
                foreach(Tuple<INeed, float> coefficient in coefficients)
                {
                    if(needs.Any(need => need.Name.Equals(coefficient.Item1.Name, StringComparison.OrdinalIgnoreCase)))
                    {
                        governingNeeds.Add(coefficient.Item1, coefficient.Item2);
                    }
                }

                skills.Add(new EntitySkill(key, 0, GlobalConstants.DEFAULT_SUCCESS_THRESHOLD, 0, governingNeeds, new StandardRoller(new RNG())));
            }

            return skills;
        }

        public NonUniqueDictionary<INeed, float> GetEmptyCoefficients()
        {
            return new NonUniqueDictionary<INeed, float>();
        }
        
        public NonUniqueDictionary<INeed, float> GetCoefficients(List<string> needNames, string skillName)
        {
            BasicValueContainer<INeed> needs = new BasicValueContainer<INeed>();
            foreach(string needName in needNames)
            {
                needs.Add(NeedHandler.Get(needName));
            }

            return GetCoefficients((IEnumerable<INeed>) needs, skillName);
        }


        /// <summary>
        /// Takes in the needs and skill name and spits out a NonUniqueDictionary for the skill
        /// </summary>
        /// <param name="needs"></param>
        /// <param name="skillName">The name of the skill in question</param>
        /// <param name="container">Container of the entity's needs</param>
        /// <returns></returns>
        public NonUniqueDictionary<INeed, float> GetCoefficients(IEnumerable<INeed> needs, string skillName)
        {
            if(m_SkillCoefficients.ContainsKey(skillName))
            {
                NonUniqueDictionary<INeed, float> coefficients = new NonUniqueDictionary<INeed, float>();

                foreach(string key in m_SkillCoefficients.Keys)
                {
                    if(key.Equals(skillName, StringComparison.OrdinalIgnoreCase))
                    {
                        foreach(Tuple<string, float> tuple in m_SkillCoefficients[key])
                        {
                            try
                            {
                                INeed need = needs.First(n => n.Name.Equals(tuple.Item1, StringComparison.OrdinalIgnoreCase));
                                coefficients.Add(need, tuple.Item2);
                            }
                            catch (Exception e)
                            {
                                ActionLog.instance.AddText(
                                    "Suppressing Exception when trying to add Skill Coefficient. Skill is "
                                    + skillName + ", with need name " + tuple.Item1);
                                Debug.LogWarning(
                                    "Suppressing Exception when trying to add Skill Coefficient. Skill is "
                                                 + skillName + ", with need name " + tuple.Item1);
                                Debug.LogWarning(e.Message);
                                Debug.LogWarning(e.StackTrace);
                            }
                        }
                    }
                }

                return coefficients;
            }
            throw new InvalidOperationException("Attempted to get coefficients for non-existent skill " + skillName);
        }

        private Dictionary<string, List<Tuple<string, float>>> LoadSkillCoefficients()
        {
            Dictionary<string, List<Tuple<string, float>>> skillCoefficients = new Dictionary<string, List<Tuple<string, float>>>();

            XElement doc = XElement.Load(Directory.GetCurrentDirectory() + GlobalConstants.DATA_FOLDER + "Skills/" + "SkillCoefficients.xml");

            foreach (XElement skill in doc.Elements("Skill"))
            {
                List<Tuple<string, float>> coefficients = (from coefficient in skill.Elements("Coefficient")
                                                           select new Tuple<string, float>(
                                                               coefficient.Element("Name").DefaultIfEmpty("DEFAULT"),
                                                               coefficient.Element("Value").DefaultIfEmpty(0.0f))).ToList();

                skillCoefficients.Add(skill.Element("Name").DefaultIfEmpty("DEFAULT"), coefficients);
            }

            return skillCoefficients;
        }
    }
}

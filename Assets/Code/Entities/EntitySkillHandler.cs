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
    public class EntitySkillHandler : MonoBehaviour
    {
        private Dictionary<string, List<Tuple<string, float>>> m_SkillCoefficients;

        protected static NeedHandler s_NeedHandler;
        
        public void Awake() 
        {
            s_NeedHandler = GameObject.Find("GameManager").GetComponent<NeedHandler>();
            m_SkillCoefficients = LoadSkillCoefficients();
        }

        public BasicValueContainer<IGrowingValue> GetDefaultSkillBlock(BasicValueContainer<INeed> needs)
        {
            BasicValueContainer<IGrowingValue> skills = new BasicValueContainer<IGrowingValue>();

            foreach(string key in m_SkillCoefficients.Keys)
            {
                NonUniqueDictionary<INeed, float> coefficients = GetCoefficients(needs, key);

                NonUniqueDictionary<INeed, float> governingNeeds = new NonUniqueDictionary<INeed, float>();
                foreach(Tuple<INeed, float> coefficient in coefficients)
                {
                    if(needs.Has(coefficient.Item1.Name))
                    {
                        governingNeeds.Add(coefficient.Item1, coefficient.Item2);
                    }
                }

                skills.Add(new EntitySkill(key, 0, GlobalConstants.DEFAULT_SUCCESS_THRESHOLD, 0, governingNeeds, new StandardRoller()));
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
                needs.Add(s_NeedHandler.Get(needName));
            }

            return GetCoefficients(needs, skillName);
        }


        /// <summary>
        /// Takes in the needs and skill name and spits out a NonUniqueDictionary for the skill
        /// </summary>
        /// <param name="container">Container of the entity's needs</param>
        /// <param name="skillName">The name of the skill in question</param>
        /// <returns></returns>
        public NonUniqueDictionary<INeed, float> GetCoefficients(BasicValueContainer<INeed> needs, string skillName)
        {
            if(m_SkillCoefficients.ContainsKey(skillName))
            {
                NonUniqueDictionary<INeed, float> coefficients = new NonUniqueDictionary<INeed, float>();

                foreach(string key in m_SkillCoefficients.Keys)
                {
                    if(key == skillName)
                    {
                        foreach(Tuple<string, float> tuple in m_SkillCoefficients[key])
                        {
                            try
                            {
                                INeed need = needs[tuple.Item1];
                                coefficients.Add(need, tuple.Item2);
                            }
                            catch (Exception e)
                            {
                                ActionLog.instance.AddText(
                                    "Suppressing Exception when trying to add Skill Coefficient. Skill is "
                                    + skillName + ", with need name " + tuple.Item1);
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

            XElement doc = XElement.Load(Directory.GetCurrentDirectory() + GlobalConstants.DATA_FOLDER + "SkillCoefficients.xml");

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

using JoyLib.Code.Entities.Abilities;
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
    public class EntityTemplateHandler
    {
        private static readonly Lazy<EntityTemplateHandler> lazy = new Lazy<EntityTemplateHandler>(() => new EntityTemplateHandler());

        public static EntityTemplateHandler instance => lazy.Value;

        private List<EntityTemplate> m_Templates;

        public EntityTemplateHandler()
        {
            m_Templates = LoadTypes();
        }

        private List<EntityTemplate> LoadTypes()
        {
            List<EntityTemplate> entities = new List<EntityTemplate>();
            
            string[] files = Directory.GetFiles(Directory.GetCurrentDirectory() + GlobalConstants.DATA_FOLDER + "Entities", "*.xml", SearchOption.AllDirectories);

            foreach (string file in files)
            {
                try
                {
                    XElement doc = XElement.Load(file);

                    foreach(XElement entity in doc.Elements("Entity"))
                    {
                        List<EntityStatistic> statistics = (from stat in entity.Elements("Statistic")
                                                                          select new EntityStatistic(stat.Element("Name").DefaultIfEmpty("DEFAULT").ToLower(), 
                                                                          stat.Element("Value").DefaultIfEmpty(4), 
                                                                          stat.Element("Threshold").DefaultIfEmpty(7), new StandardRoller())).ToList();

                        //TODO: FIX THIS NASTY CAST
                        List<IRollableValue> statFudge = new List<IRollableValue>(statistics);
                        BasicValueContainer<IRollableValue> statisticContainer = new BasicValueContainer<IRollableValue>(statFudge);

                        List<INeed> needs = (from need in entity.Elements("Need")
                                             select NeedHandler.instance.GetRandomised(need.DefaultIfEmpty("DEFAULT").ToLower())).ToList();
                        BasicValueContainer<INeed> needContainer = new BasicValueContainer<INeed>(needs);

                        List<EntitySkill> skills = (from skill in entity.Elements("Skill")
                                                                  select new EntitySkill(skill.Element("Name").DefaultIfEmpty("DEFAULT").ToLower(),
                                                                  skill.Element("Value").DefaultIfEmpty(0),
                                                                  skill.Element("Threshold").DefaultIfEmpty(7),
                                                                  skill.Element("Experience").DefaultIfEmpty(0),
                                                                  EntitySkillHandler.instance.GetCoefficients(needContainer, skill.Element("Name").DefaultIfEmpty("DEFAULT")), new StandardRoller())).ToList();

                        //TODO: FIX THIS NASTY CAST
                        List<IGrowingValue> skillFudge = new List<IGrowingValue>(skills);
                        BasicValueContainer<IGrowingValue> skillContainer = new BasicValueContainer<IGrowingValue>(skillFudge);

                        string creatureType = entity.Element("CreatureType").DefaultIfEmpty("DEFAULT").ToLower();
                        string type = entity.Element("Type").DefaultIfEmpty("DEFAULT").ToLower();
                        string visionType = entity.Element("VisionType").DefaultIfEmpty("Diurnal").ToLower();
                        string tileset = entity.Element("Tileset").DefaultIfEmpty("DEFAULT").ToLower();

                        int size = entity.Element("Size").DefaultIfEmpty<int>(0);

                        List<string> tags = (from tag in entity.Elements("Tag")
                                             select tag.DefaultIfEmpty("NULL").ToLower()).ToList();

                        List<string> slots = (from slot in entity.Elements("Slot")
                                              select slot.DefaultIfEmpty("NULL").ToLower()).ToList();

                        List<IAbility> abilities = new List<IAbility>();
                        try
                        {
                            abilities = (from ability in entity.Elements("Ability")
                                                    select AbilityHandler.instance.GetAbility(ability.DefaultIfEmpty("DEFAULT"))).ToList();
                        }
                        catch(Exception e)
                        {
                            ActionLog.instance.AddText("ERROR LOADING ABILITY FOR ENTITY TEMPLATE " + file);
                            ActionLog.instance.AddText(e.Message);
                            ActionLog.instance.AddText(e.StackTrace);
                        }
                        

                        entities.Add(new EntityTemplate(statisticContainer, skillContainer, abilities.ToArray(), slots.ToArray(),
                            size, visionType, creatureType, type, tileset, tags.ToArray()));
                    }
                }
                catch (Exception e)
                {
                    ActionLog.instance.AddText("ERROR LOADING ENTITY TEMPLATES, FILE " + file);
                    ActionLog.instance.AddText(e.Message);
                    ActionLog.instance.AddText(e.StackTrace);
                }
            }

            return entities;
        }

        public EntityTemplate Get(string type)
        {
            if(m_Templates.Any(x => x.CreatureType == type))
            {
                return m_Templates.First(x => x.CreatureType == type);
            }

            return null;
        }

        public EntityTemplate[] Templates
        {
            get
            {
                return m_Templates.ToArray();
            }
        }
    }
}

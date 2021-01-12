using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using JoyLib.Code.Entities.Abilities;
using JoyLib.Code.Entities.AI.LOS.Providers;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Helpers;

namespace JoyLib.Code.Entities
{
    public class EntityTemplateHandler : IEntityTemplateHandler
    {
        protected List<IEntityTemplate> m_Templates;
        protected IEntitySkillHandler SkillHandler { get; set; }
        protected IVisionProviderHandler VisionProviderHandler { get; set; }
        protected IAbilityHandler AbilityHandler { get; set; }

        public IEnumerable<IEntityTemplate> Templates
        {
            get
            {
                if (this.m_Templates is null)
                {
                    this.m_Templates = this.LoadTypes();
                }

                return this.m_Templates;
            }
        }

        public EntityTemplateHandler(
            IEntitySkillHandler skillHandler,
            IVisionProviderHandler visionProviderHandler,
            IAbilityHandler abilityHandler)
        {
            this.AbilityHandler = abilityHandler;
            this.VisionProviderHandler = visionProviderHandler;
            this.SkillHandler = skillHandler;
            this.m_Templates = this.LoadTypes();
        }

        private List<IEntityTemplate> LoadTypes()
        {
            List<IEntityTemplate> entities = new List<IEntityTemplate>();
            
            string[] files = Directory.GetFiles(Directory.GetCurrentDirectory() + GlobalConstants.DATA_FOLDER + "Entities", "*.xml", SearchOption.AllDirectories);

            foreach (string file in files)
            {
                try
                {
                    XElement doc = XElement.Load(file);

                    foreach(XElement entity in doc.Elements("Entity"))
                    {
                        Dictionary<string, IRollableValue<int>> statistics = (from stat in entity.Elements("Statistic")
                                select new KeyValuePair<string, IRollableValue<int>>(
                                    stat.Element("Name").GetAs<string>(),
                                    new EntityStatistic(
                                        stat.Element("Name").DefaultIfEmpty("DEFAULT"),
                                        stat.Element("Value").DefaultIfEmpty(4),
                                        stat.Element("Threshold").DefaultIfEmpty(GlobalConstants.DEFAULT_SUCCESS_THRESHOLD))))
                            .ToDictionary(x => x.Key, x => x.Value);

                        List<string> needs = (from need in entity.Elements("Need")
                                                select need.DefaultIfEmpty("DEFAULT")).ToList();

                        Dictionary<string, IEntitySkill> skills = (from skill in entity.Elements("Skill")
                                                                  select new KeyValuePair<string, IEntitySkill>(
                                                                      skill.Element("Name").GetAs<string>(), 
                                                                      new EntitySkill(skill.Element("Name").DefaultIfEmpty("DEFAULT"),
                                                                          skill.Element("Value").DefaultIfEmpty(0),
                                                                          skill.Element("Threshold").DefaultIfEmpty(GlobalConstants.DEFAULT_SUCCESS_THRESHOLD),
                                                                          this.SkillHandler.GetCoefficients(needs, skill.Element("Name").DefaultIfEmpty("DEFAULT")))))
                                                                .ToDictionary(x => x.Key, x => x.Value);

                        string creatureType = entity.Element("CreatureType").DefaultIfEmpty("DEFAULT");
                        string type = entity.Element("Type").DefaultIfEmpty("DEFAULT");
                        string visionType = entity.Element("VisionType").DefaultIfEmpty("diurnal vision");
                        IVision vision = this.VisionProviderHandler.GetVision(visionType);

                        int size = entity.Element("Size").DefaultIfEmpty<int>(0);

                        List<string> tags = (from tag in entity.Elements("Tag")
                                             select tag.DefaultIfEmpty("NULL")).ToList();

                        List<string> slots = (from slot in entity.Elements("Slot")
                                              select slot.DefaultIfEmpty("NULL")).ToList();

                        List<IAbility> abilities = new List<IAbility>();
                        try
                        {
                            abilities = (from ability in entity.Elements("Ability")
                                                    select this.AbilityHandler.GetAbility(ability.GetAs<string>())).ToList();
                        }
                        catch(Exception e)
                        {
                            GlobalConstants.ActionLog.AddText("ERROR LOADING ABILITY FOR ENTITY TEMPLATE " + file);
                            GlobalConstants.ActionLog.AddText(e.Message);
                            GlobalConstants.ActionLog.AddText(e.StackTrace);
                        }
                        
                        entities.Add(new EntityTemplate(
                                            statistics, 
                                            skills,
                                            needs.ToArray(), 
                                            abilities.ToArray(), 
                                            slots.ToArray(),
                                            size, 
                                            vision, 
                                            creatureType, 
                                            type, 
                                            tags.ToArray()));
                    }
                }
                catch (Exception e)
                {
                    GlobalConstants.ActionLog.AddText("ERROR LOADING ENTITY TEMPLATES, FILE " + file);
                    GlobalConstants.ActionLog.AddText("ERROR LOADING ENTITY TEMPLATES, FILE " + file);
                    GlobalConstants.ActionLog.AddText(e.Message);
                    GlobalConstants.ActionLog.AddText(e.StackTrace);
                }
            }

            return entities;
        }

        public IEntityTemplate Get(string type)
        {
            if(this.Templates.Any(x => x.CreatureType == type))
            {
                return this.Templates.First(x => x.CreatureType == type);
            }

            throw new InvalidOperationException("Could not find entity template of type " + type);
        }

        public IEntityTemplate GetRandom()
        {
            int result = GlobalConstants.GameManager.Roller.Roll(0, this.m_Templates.Count);
            return this.m_Templates[result];
        }
    }
}

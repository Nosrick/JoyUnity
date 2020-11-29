using JoyLib.Code.Entities.Abilities;
using JoyLib.Code.Entities.Needs;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Helpers;
using JoyLib.Code.Rollers;
using JoyLib.Code.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using DevionGames.InventorySystem;
using UnityEngine;
using JoyLib.Code.Entities.AI.LOS.Providers;
using JoyLib.Code.Scripting;

namespace JoyLib.Code.Entities
{
    public class EntityTemplateHandler : IEntityTemplateHandler
    {
        protected List<EntityTemplate> m_Templates;
        protected IEntitySkillHandler SkillHandler { get; set; }

        public List<EntityTemplate> Templates
        {
            get
            {
                if (m_Templates is null)
                {
                    m_Templates = LoadTypes();
                }

                return m_Templates;
            }
        }

        public EntityTemplateHandler(IEntitySkillHandler skillHandler)
        {
            this.SkillHandler = skillHandler;
            m_Templates = LoadTypes();
        }

        private List<EntityTemplate> LoadTypes()
        {
            InventoryManager.Database.equipments.Clear();
            
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
                                                                          select new EntityStatistic(stat.Element("Name").DefaultIfEmpty("DEFAULT"), 
                                                                          stat.Element("Value").DefaultIfEmpty(4), 
                                                                          stat.Element("Threshold").DefaultIfEmpty(7), new StandardRoller(new RNG()))).ToList();

                        //TODO: FIX THIS NASTY CAST
                        List<IRollableValue> statFudge = new List<IRollableValue>(statistics);
                        BasicValueContainer<IRollableValue> statisticContainer = new BasicValueContainer<IRollableValue>(statFudge);

                        List<string> needs = (from need in entity.Elements("Need")
                                                select need.DefaultIfEmpty("DEFAULT")).ToList();

                        List<EntitySkill> skills = (from skill in entity.Elements("Skill")
                                                                  select new EntitySkill(skill.Element("Name").DefaultIfEmpty("DEFAULT"),
                                                                  skill.Element("Value").DefaultIfEmpty(0),
                                                                  skill.Element("Threshold").DefaultIfEmpty(7),
                                                                  skill.Element("Experience").DefaultIfEmpty(0),
                                                                  SkillHandler.GetCoefficients(needs, skill.Element("Name").DefaultIfEmpty("DEFAULT")), new StandardRoller(new RNG()))).ToList();

                        //TODO: FIX THIS NASTY CAST
                        List<IGrowingValue> skillFudge = new List<IGrowingValue>(skills);
                        BasicValueContainer<IGrowingValue> skillContainer = new BasicValueContainer<IGrowingValue>(skillFudge);

                        string creatureType = entity.Element("CreatureType").DefaultIfEmpty("DEFAULT");
                        string type = entity.Element("Type").DefaultIfEmpty("DEFAULT");
                        string visionType = entity.Element("VisionType").DefaultIfEmpty("DiurnalVisionProvider");
                        IVision vision = (IVision)ScriptingEngine.instance.FetchAndInitialise(visionType);
                        string tileset = entity.Element("Tileset").DefaultIfEmpty("DEFAULT");

                        int size = entity.Element("Size").DefaultIfEmpty<int>(0);

                        List<string> tags = (from tag in entity.Elements("Tag")
                                             select tag.DefaultIfEmpty("NULL")).ToList();

                        List<string> slots = (from slot in entity.Elements("Slot")
                                              select slot.DefaultIfEmpty("NULL")).ToList();

                        List<IAbility> abilities = new List<IAbility>();
                        try
                        {
                            abilities = (from ability in entity.Elements("Ability")
                                                    select GlobalConstants.GameManager.AbilityHandler.GetAbility(ability.DefaultIfEmpty("DEFAULT"))).ToList();
                        }
                        catch(Exception e)
                        {
                            ActionLog.instance.AddText("ERROR LOADING ABILITY FOR ENTITY TEMPLATE " + file);
                            ActionLog.instance.AddText(e.Message);
                            ActionLog.instance.AddText(e.StackTrace);
                        }
                        

                        entities.Add(new EntityTemplate(
                                            statisticContainer, 
                                            skillContainer,
                                            needs.ToArray(), 
                                            abilities.ToArray(), 
                                            slots.ToArray(),
                                            size, 
                                            vision, 
                                            creatureType, 
                                            type, 
                                            tileset, 
                                            tags.ToArray()));
                        
                        AddSlotsToDatabase(slots);
                    }
                }
                catch (Exception e)
                {
                    ActionLog.instance.AddText("ERROR LOADING ENTITY TEMPLATES, FILE " + file);
                    Debug.LogWarning("ERROR LOADING ENTITY TEMPLATES, FILE " + file);
                    Debug.LogWarning(e.Message);
                    Debug.LogWarning(e.StackTrace);
                }
            }

            return entities;
        }

        public EntityTemplate Get(string type)
        {
            if(Templates.Any(x => x.CreatureType == type))
            {
                return Templates.First(x => x.CreatureType == type);
            }

            return null;
        }

        public EntityTemplate GetRandom()
        {
            int result = GlobalConstants.GameManager.Roller.Roll(0, m_Templates.Count);
            return Templates[result];
        }

        protected void AddSlotsToDatabase(List<string> slots)
        {
            foreach (string slot in slots)
            {
                if (InventoryManager.Database.equipments.Any(equipmentSlot =>
                    equipmentSlot.Name.Equals(slot, StringComparison.OrdinalIgnoreCase)))
                {
                    continue;
                }

                EquipmentRegion region = ScriptableObject.CreateInstance<EquipmentRegion>();
                region.Name = slot;
                InventoryManager.Database.equipments.Add(region);                
            }
        }
    }
}

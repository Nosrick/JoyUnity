﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JoyLib.Code.Collections;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Items;
using JoyLib.Code.Entities.Needs;
using JoyLib.Code.Entities.Relationships;
using JoyLib.Code.Graphics;
using JoyLib.Code.Helpers;
using JoyLib.Code.Quests;
using JoyLib.Code.World;
using Sirenix.OdinSerializer;

namespace JoyLib.Code.IO
{
    public class WorldSerialiser
    {
        protected static IObjectIconHandler s_ObjectIcons = GlobalConstants.GameManager.ObjectIconHandler;

        protected const DataFormat DATA_FORMAT = DataFormat.JSON;

        public void Serialise(IWorldInstance world)
        {
            string directory = Directory.GetCurrentDirectory() + "/save/" + world.Name;
            try
            {
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
            }
            catch (Exception e)
            {
                GlobalConstants.ActionLog.AddText("Cannot open save directory.", LogLevel.Error);
                GlobalConstants.ActionLog.StackTrace(e);
            }
            try
            {
                byte[] array = SerializationUtility.SerializeValue(world, DATA_FORMAT);
                File.WriteAllBytes(directory + "/world.dat", array);
                /*
                StreamWriter writer = new StreamWriter(Directory.GetCurrentDirectory() + "/save/" + world.Name + "/sav.dat", false);
                JsonSerializer serializer = JsonSerializer.CreateDefault();
                serializer.Serialize(writer, world);
                writer.Close();
                */

                array = SerializationUtility.SerializeValue(GlobalConstants.GameManager.QuestTracker.AllQuests,
                    DATA_FORMAT);
                File.WriteAllBytes(directory + "/quests.dat", array);

                array = SerializationUtility.SerializeValue(GlobalConstants.GameManager.ItemHandler.QuestRewards,
                    DATA_FORMAT);
                File.WriteAllBytes(directory + "/rewards.dat", array);

                array = SerializationUtility.SerializeValue(
                    GlobalConstants.GameManager.RelationshipHandler.AllRelationships,
                    DATA_FORMAT);
                File.WriteAllBytes(directory + "/relationships.dat", array);

                array = SerializationUtility.SerializeValue(
                    GlobalConstants.GameManager.ItemHandler.AllItems,
                    DATA_FORMAT);
                File.WriteAllBytes(directory + "/items.dat", array);

                array = SerializationUtility.SerializeValue(
                    GlobalConstants.GameManager.EntityHandler.AllEntities,
                    DATA_FORMAT);
                File.WriteAllBytes(directory + "/entities.dat", array);

            }
            catch(Exception e)
            {
                GlobalConstants.ActionLog.AddText("Cannot serialise and/or write world to file.", LogLevel.Error);
                GlobalConstants.ActionLog.StackTrace(e);
            }
        }

        public IWorldInstance Deserialise(string worldName)
        {
            /*
            StreamReader reader = new StreamReader(Directory.GetCurrentDirectory() + "/save/" + worldName + "/sav.dat");
            JsonSerializer serializer = JsonSerializer.CreateDefault();
            IWorldInstance world = serializer.Deserialize<IWorldInstance>(new JsonTextReader(reader));
            reader.Close();
            */

            string directory = Directory.GetCurrentDirectory() + "/save/" + worldName;
            byte[] array = File.ReadAllBytes(directory + "/world.dat");
            IWorldInstance world = SerializationUtility.DeserializeValue<IWorldInstance>(array, DATA_FORMAT);
            world.Initialise();

            array = File.ReadAllBytes(directory + "/items.dat");
            IEnumerable<IItemInstance> items =
                SerializationUtility.DeserializeValue<IEnumerable<IItemInstance>>(array, DATA_FORMAT);
            this.Items(items);

            array = File.ReadAllBytes(directory + "/quests.dat");
            IEnumerable<IQuest> quests = SerializationUtility.DeserializeValue<IEnumerable<IQuest>>(array, DATA_FORMAT);
            this.Quests(quests);

            array = File.ReadAllBytes(directory + "/rewards.dat");
            NonUniqueDictionary<long, long> rewards =
                SerializationUtility.DeserializeValue<NonUniqueDictionary<long, long>>(array, DATA_FORMAT);
            this.QuestRewards(rewards);

            array = File.ReadAllBytes(directory + "/entities.dat");
            IEnumerable<IEntity> entities =
                SerializationUtility.DeserializeValue<IEnumerable<IEntity>>(array, DATA_FORMAT);
            this.Entities(entities, world);

            array = File.ReadAllBytes(directory + "/relationships.dat");
            IEnumerable<IRelationship> relationships =
                SerializationUtility.DeserializeValue<IEnumerable<IRelationship>>(array, DATA_FORMAT);
            this.Relationships(relationships);
            
            this.LinkWorlds(world);
            this.AssignIcons(world);
            
            return world;
        }

        private void LinkWorlds(IWorldInstance parent)
        {
            foreach (IWorldInstance world in parent.Areas.Values)
            {
                world.Parent = parent;
                this.LinkWorlds(world);
            }
        }
        
        private void AssignIcons(IWorldInstance parent)
        {
            foreach (IJoyObject wall in parent.Walls.Values)
            {
                foreach (ISpriteState state in wall.States)
                {
                    this.SetUpSpriteStates(wall.TileSet, state);
                }

                wall.MyWorld = parent;
            }

            foreach(IWorldInstance world in parent.Areas.Values)
            {
                this.AssignIcons(world);
            }
        }

        protected void SetUpSpriteStates(string tileSet, ISpriteState state)
        {
            foreach (SpritePart part in state.SpriteData.m_Parts)
            {
                part.m_FrameSprites = s_ObjectIcons.GetRawFrames(tileSet, state.Name, part.m_Name, state.SpriteData.m_State);
            }
        }

        protected void HandleContents(IItemContainer container)
        {
            foreach (IItemInstance item in container.Contents)
            {
                foreach (ISpriteState state in item.States)
                {
                    this.SetUpSpriteStates(item.TileSet, state);
                }

                foreach (IItemInstance content in item.Contents)
                {
                    foreach (ISpriteState state in content.States)
                    {
                        this.SetUpSpriteStates(content.TileSet, state);
                    }
                    this.HandleContents(content);
                }
            }
        }

        private void QuestRewards(NonUniqueDictionary<long, long> rewards)
        {
            foreach (long questID in rewards.Keys)
            {
                GlobalConstants.GameManager.ItemHandler.AddQuestRewards(questID, rewards.FetchValuesForKey(questID));
            }
        }

        private void Quests(IEnumerable<IQuest> quests)
        {
            foreach (IQuest quest in quests)
            {
                GlobalConstants.GameManager.QuestTracker.AddQuest(quest.Questor, quest);
            }
        }

        private void Relationships(IEnumerable<IRelationship> relationships)
        {
            foreach (IRelationship relationship in relationships)
            {
                GlobalConstants.GameManager.RelationshipHandler.AddRelationship(relationship);
            }
        }

        private void Items(IEnumerable<IItemInstance> items)
        {
            foreach (IItemInstance item in items)
            {
                foreach (ISpriteState state in item.States)
                {
                    this.SetUpSpriteStates(item.TileSet, state);
                }

                if (item is IItemContainer container)
                {
                    //this.HandleContents(container);
                }
                GlobalConstants.GameManager.ItemHandler.AddItem(item);
            }
        }

        private void Entities(IEnumerable<IEntity> entities, IWorldInstance overworld)
        {
            List<IWorldInstance> worlds = overworld.GetWorlds(overworld);
            foreach (IEntity entity in entities)
            {
                foreach (ISpriteState state in entity.States)
                {
                    this.SetUpSpriteStates(entity.TileSet, state);
                }

                foreach (INeed need in entity.Needs.Values)
                {
                    need.FulfillingSprite = new SpriteState(
                        need.Name, 
                        GlobalConstants.GameManager.ObjectIconHandler.GetFrame(
                            "needs", 
                            need.Name));
                }

                worlds.First(world => world.EntityGUIDs.Contains(entity.GUID))?.AddEntity(entity);
            }
        }
    }
}

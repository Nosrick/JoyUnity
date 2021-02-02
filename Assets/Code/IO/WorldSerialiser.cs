using System;
using System.Collections.Generic;
using System.IO;
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
                GlobalConstants.ActionLog.AddText(e.Message, LogLevel.Error);
            }
            try
            {
                byte[] array = SerializationUtility.SerializeValue(world, DataFormat.JSON);
                File.WriteAllBytes(directory + "/world.dat", array);
                /*
                StreamWriter writer = new StreamWriter(Directory.GetCurrentDirectory() + "/save/" + world.Name + "/sav.dat", false);
                JsonSerializer serializer = JsonSerializer.CreateDefault();
                serializer.Serialize(writer, world);
                writer.Close();
                */

                array = SerializationUtility.SerializeValue(GlobalConstants.GameManager.QuestTracker.AllQuests,
                    DataFormat.JSON);
                File.WriteAllBytes(directory + "/quests.dat", array);

                array = SerializationUtility.SerializeValue(
                    GlobalConstants.GameManager.RelationshipHandler.AllRelationships,
                    DataFormat.JSON);
                File.WriteAllBytes(directory + "/relationships.dat", array);

            }
            catch(Exception e)
            {
                GlobalConstants.ActionLog.AddText("Cannot serialise and/or write world to file.", LogLevel.Error);
                GlobalConstants.ActionLog.AddText(e.Message, LogLevel.Error);
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
            IWorldInstance world = SerializationUtility.DeserializeValue<IWorldInstance>(array, DataFormat.JSON);
            
            this.LinkWorlds(world);
            this.AssignIcons(world);

            array = File.ReadAllBytes(directory + "/quests.dat");
            IEnumerable<IQuest> quests = SerializationUtility.DeserializeValue<IEnumerable<IQuest>>(array, DataFormat.JSON);
            this.Quests(quests);

            array = File.ReadAllBytes(directory + "/relationships.dat");
            IEnumerable<IRelationship> relationships =
                SerializationUtility.DeserializeValue<IEnumerable<IRelationship>>(array, DataFormat.JSON);
            this.Relationships(relationships);
            
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
            foreach (IJoyObject obj in parent.Objects)
            {
                foreach (ISpriteState state in obj.States)
                {
                    this.SetUpSpriteStates(obj.TileSet, state);
                }

                if (obj is IItemContainer container)
                {
                    this.HandleContents(container);
                }
                
                obj.MyWorld = parent;
                
                GlobalConstants.GameManager.ItemHandler.AddItem(obj as IItemInstance);
            }

            foreach (IJoyObject wall in parent.Walls.Values)
            {
                foreach (ISpriteState state in wall.States)
                {
                    this.SetUpSpriteStates(wall.TileSet, state);
                }

                wall.MyWorld = parent;
            }

            foreach (IEntity entity in parent.Entities)
            {
                GlobalConstants.GameManager.EntityHandler.AddEntity(entity);
                
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

                entity.MyWorld = parent;
                this.HandleContents(entity);
                this.HandleContents(entity.Equipment);
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
    }
}

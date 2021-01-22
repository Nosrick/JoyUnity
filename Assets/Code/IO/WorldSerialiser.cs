using System;
using System.IO;
using JoyLib.Code.Entities;
using JoyLib.Code.Graphics;
using JoyLib.Code.Helpers;
using JoyLib.Code.World;
using OdinSerializer;

namespace JoyLib.Code.IO
{
    public class WorldSerialiser
    {
        protected static IObjectIconHandler s_ObjectIcons = GlobalConstants.GameManager.ObjectIconHandler; 

        public void Serialise(IWorldInstance world)
        {
            try
            {
                if (!Directory.Exists(Directory.GetCurrentDirectory() + "/save/" + world.Name))
                {
                    Directory.CreateDirectory(Directory.GetCurrentDirectory() + "/save/" + world.Name);
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
                File.WriteAllBytes(Directory.GetCurrentDirectory() + "/save/" + world.Name + "/sav.dat", array);
                /*
                StreamWriter writer = new StreamWriter(Directory.GetCurrentDirectory() + "/save/" + world.Name + "/sav.dat", false);
                JsonSerializer serializer = JsonSerializer.CreateDefault();
                serializer.Serialize(writer, world);
                writer.Close();
                */
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
            
            byte[] array = File.ReadAllBytes(Directory.GetCurrentDirectory() + "/save/" + worldName + "/sav.dat");
            IWorldInstance world = SerializationUtility.DeserializeValue<IWorldInstance>(array, DataFormat.JSON);
            
            this.LinkWorlds(world);
            this.EntityWorldKnowledge(world);
            //this.AssignIcons(world);
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

        private void EntityWorldKnowledge(IWorldInstance parent)
        {
            foreach (IEntity entity in parent.Entities)
            {
                entity.MyWorld = parent;
            }

            foreach (IWorldInstance world in parent.Areas.Values)
            {
                this.EntityWorldKnowledge(world);
            }
        }
        
        private void AssignIcons(IWorldInstance parent)
        {
            /*
            foreach(IJoyObject obj in parent.Objects)
            {
                obj.Sprites = s_ObjectIcons.GetSprites(obj.TileSet, obj.JoyName).ToArray();
            }

            foreach(IEntity entity in parent.Entities)
            {
                entity.Sprites = s_ObjectIcons.GetSprites(entity.TileSet, entity.CreatureType).ToArray();
            }

            foreach(IWorldInstance world in parent.Areas.Values)
            {
                this.AssignIcons(world);
            }
            */
        }
    }
}

using JoyLib.Code.Entities;
using JoyLib.Code.Graphics;
using JoyLib.Code.World;
using System;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace JoyLib.Code.IO
{
    public static class WorldSerialiser
    {
        public static void Serialise(WorldInstance world)
        {
            try
            {
                if (!Directory.Exists(Directory.GetCurrentDirectory() + "/save/" + world.Name))
                    Directory.CreateDirectory(Directory.GetCurrentDirectory() + "/save/" + world.Name);
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot open directory. Quitting.");
                Console.WriteLine(e.Message);
                Environment.Exit(-1);
            }
            try
            {
                string worldString = JsonUtility.ToJson(world);

                byte[] worldBytes = Encoding.ASCII.GetBytes(worldString);
                StreamWriter writer = new StreamWriter(Directory.GetCurrentDirectory() + "/save/" + world.Name + "/sav.dat", false);
                writer.WriteLine(worldString);
                writer.Close();
            }
            catch(Exception e)
            {
                Console.WriteLine("Cannot serialise and/or write world to file. Quitting.");
                Console.WriteLine(e.Message);
                Environment.Exit(-1);
            }
        }

        public static WorldInstance Deserialise(string worldName)
        {
            StreamReader reader = new StreamReader(Directory.GetCurrentDirectory() + "/save/" + worldName + "/sav.dat");
            string worldString = reader.ReadToEnd();
            
            WorldInstance world = JsonUtility.FromJson<WorldInstance>(worldString);
            LinkWorlds(world);
            EntityWorldKnowledge(world);
            AssignIcons(world);
            reader.Close();
            return world;
        }

        private static void LinkWorlds(WorldInstance parent)
        {
            foreach (WorldInstance world in parent.Areas.Values)
            {
                world.Parent = parent;
                LinkWorlds(world);
            }
        }

        private static void EntityWorldKnowledge(WorldInstance parent)
        {
            foreach (Entity entity in parent.Entities)
            {
                entity.MyWorld = parent;
            }

            foreach (WorldInstance world in parent.Areas.Values)
            {
                EntityWorldKnowledge(world);
            }
        }
        
        private static void AssignIcons(WorldInstance parent)
        {
            foreach(JoyObject obj in parent.Objects)
            {
                obj.SetIcons(ObjectIcons.GetIcons(obj.BaseType, obj.name));
            }

            foreach(Entity entity in parent.Entities)
            {
                if (entity.PlayerControlled)
                    entity.SetIcons(ObjectIcons.GetIcons("Jobs", entity.Job.name));
                else
                    entity.SetIcons(ObjectIcons.GetIcons(entity.Tileset, entity.CreatureType));
            }

            foreach(WorldInstance world in parent.Areas.Values)
            {
                AssignIcons(world);
            }
        }
    }
}

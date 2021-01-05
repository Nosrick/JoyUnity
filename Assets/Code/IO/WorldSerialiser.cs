using System;
using System.IO;
using JoyLib.Code.Entities;
using JoyLib.Code.Graphics;
using JoyLib.Code.World;
using UnityEngine;

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
                Console.WriteLine("Cannot open directory. Quitting.");
                Console.WriteLine(e.Message);
                Environment.Exit(-1);
            }
            try
            {
                string worldString = JsonUtility.ToJson(world);

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

        public IWorldInstance Deserialise(string worldName)
        {
            StreamReader reader = new StreamReader(Directory.GetCurrentDirectory() + "/save/" + worldName + "/sav.dat");
            string worldString = reader.ReadToEnd();
            
            IWorldInstance world = JsonUtility.FromJson<WorldInstance>(worldString);
            this.LinkWorlds(world);
            this.EntityWorldKnowledge(world);
            this.AssignIcons(world);
            reader.Close();
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

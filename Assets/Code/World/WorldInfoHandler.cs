using System.Xml.Linq;
using JoyLib.Code.Graphics;
using JoyLib.Code.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace JoyLib.Code.World
{
    public class WorldInfoHandler
    {
        private static readonly Lazy<WorldInfoHandler> lazy = new Lazy<WorldInfoHandler>(() => new WorldInfoHandler());

        public static WorldInfoHandler instance = lazy.Value;

        private List<WorldInfo> m_WorldInfo;

        public bool Load()
        {
            if (m_WorldInfo != null)
            {
                return true;
            }

            string[] files = Directory.GetFiles(Directory.GetCurrentDirectory() + GlobalConstants.DATA_FOLDER + "World Spaces", "*.xml", SearchOption.AllDirectories);
            m_WorldInfo = new List<WorldInfo>();

            foreach (string file in files)
            {
                XElement doc = XElement.Load(file);

                foreach (XElement worldInfo in doc.Elements("WorldInfo"))
                {
                    WorldInfo info = new WorldInfo()
                    {
                        name = worldInfo.Element("Name").GetAs<string>().ToLower(),
                        inhabitants = worldInfo.Elements("Inhabitant")
                            .Select(x => x.GetAs<string>().ToLower())
                            .ToArray(),
                        tileset = worldInfo.Element("Tileset").GetAs<string>().ToLower(),
                        tags = worldInfo.Elements("Tag")
                            .Select(x => x.GetAs<string>().ToLower())
                            .ToArray()
                    };

                    //This is optional
                    string filename = worldInfo.Element("Filename").DefaultIfEmpty("NULL");

                    List<IconData> iconData = new List<IconData>();
                    foreach (XElement icon in worldInfo.Elements("Icon"))
                    {
                        IconData newIcon = new IconData()
                        {
                            name = icon.Element("Name").GetAs<string>().ToLower(),
                            position = new UnityEngine.Vector2Int(
                                icon.Element("X").GetAs<int>(),
                                icon.Element("Y").GetAs<int>()),
                            data = icon.Element("Data").DefaultIfEmpty("").ToLower()
                        };

                        iconData.Add(newIcon);
                    }

                    m_WorldInfo.Add(info);

                    ObjectIconHandler.instance.AddIcons(
                        filename,
                        info.tileset,
                        iconData.ToArray());

                    StandardWorldTiles.instance.AddType(
                        new WorldTile(
                            info.name,
                            info.tileset,
                            info.tags));
                }
            }

            return true;
        }

        public WorldInfo[] GetWorldInfo(string name)
        {
            try
            {
                return m_WorldInfo.Where(info => info.name.StartsWith(name.ToLower())).ToArray();
            }
            catch (Exception e)
            {
                ActionLog.WriteToLog("ERROR GETTING WORLD INFO");
                ActionLog.WriteToLog(e.Message);
                ActionLog.WriteToLog(e.StackTrace);
                throw new InvalidOperationException("Error getting world info for " + name.ToLower());
            }
        }
    }

    public struct WorldInfo
    {
        public string name;
        public string[] inhabitants;
        public string tileset;

        public string[] tags;
    }
}

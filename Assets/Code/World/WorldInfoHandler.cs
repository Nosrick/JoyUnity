using System.Xml.Linq;
using JoyLib.Code.Graphics;
using JoyLib.Code.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace JoyLib.Code.World
{
    public class WorldInfoHandler : MonoBehaviour
    {
        protected ObjectIconHandler m_ObjectIcons;

        private List<WorldInfo> m_WorldInfo;

        public void Awake()
        {
            m_ObjectIcons = GameObject.Find("GameManager")
                                .GetComponent<ObjectIconHandler>();

            Load();
        }

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
                        name = worldInfo.Element("Name").GetAs<string>(),
                        inhabitants = worldInfo.Elements("Inhabitant")
                            .Select(x => x.GetAs<string>())
                            .ToArray(),
                        tileset = worldInfo.Element("Tileset").GetAs<string>(),
                        tags = worldInfo.Elements("Tag")
                            .Select(x => x.GetAs<string>())
                            .ToArray()
                    };

                    //This is optional
                    string filename = worldInfo.Element("Filename").DefaultIfEmpty("NULL");

                    List<IconData> iconData = new List<IconData>();
                    foreach (XElement icon in worldInfo.Elements("Icon"))
                    {
                        IconData newIcon = new IconData()
                        {
                            name = icon.Element("Name").GetAs<string>(),
                            position = new UnityEngine.Vector2Int(
                                icon.Element("X").GetAs<int>(),
                                icon.Element("Y").GetAs<int>()),
                            data = icon.Element("Data").DefaultIfEmpty("")
                        };

                        iconData.Add(newIcon);
                    }

                    m_WorldInfo.Add(info);

                    m_ObjectIcons.AddIcons(
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
                return m_WorldInfo.Where(info => info.name.StartsWith(name, StringComparison.OrdinalIgnoreCase)).ToArray();
            }
            catch (Exception e)
            {
                ActionLog.instance.AddText("ERROR GETTING WORLD INFO");
                ActionLog.instance.AddText(e.Message);
                ActionLog.instance.AddText(e.StackTrace);
                throw new InvalidOperationException("Error getting world info for " + name);
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

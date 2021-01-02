using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using JoyLib.Code.Graphics;
using JoyLib.Code.Helpers;

namespace JoyLib.Code.World
{
    public interface IWorldInfoHandler
    {
        WorldInfo[] GetWorldInfo(string name);
    }

    public class WorldInfoHandler : IWorldInfoHandler
    {
        protected IObjectIconHandler ObjectIcons { get; set; }

        protected List<WorldInfo> WorldInfo { get; set; }

        public WorldInfoHandler(IObjectIconHandler objectIconHandler)
        {
            this.ObjectIcons = objectIconHandler;

            this.Load();
        }

        protected bool Load()
        {
            if (this.WorldInfo is null == false)
            {
                return true;
            }

            string[] files = Directory.GetFiles(Directory.GetCurrentDirectory() + GlobalConstants.DATA_FOLDER + "World Spaces", "*.xml", SearchOption.AllDirectories);
            this.WorldInfo = new List<WorldInfo>();

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
                            data = icon.Element("Data").DefaultIfEmpty(""),
                            filename = filename,
                            position = icon.Element("Position").GetAs<int>()
                        };

                        iconData.Add(newIcon);
                    }

                    this.WorldInfo.Add(info);

                    this.ObjectIcons.AddIcons(
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
                return this.WorldInfo.Where(info => info.name.StartsWith(name, StringComparison.OrdinalIgnoreCase)).ToArray();
            }
            catch (Exception e)
            {
                GlobalConstants.ActionLog.AddText("ERROR GETTING WORLD INFO");
                GlobalConstants.ActionLog.AddText(e.Message);
                GlobalConstants.ActionLog.AddText(e.StackTrace);
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

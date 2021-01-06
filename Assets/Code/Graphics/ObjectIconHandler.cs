using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using JoyLib.Code.Helpers;
using JoyLib.Code.Rollers;
using UnityEngine;

namespace JoyLib.Code.Graphics
{
    public class ObjectIconHandler : IObjectIconHandler
    {
        protected RNG Roller { get; set; }
        
        protected int m_SpriteSize = 16;

        public ObjectIconHandler(RNG roller)
        {
            this.Roller = roller;
            this.Initalise(GlobalConstants.SPRITE_SIZE);
        }

        protected void Initalise(int spriteSize) 
        {
            this.SpriteSize = spriteSize;

            this.Load();
        }

        protected bool Load()
        {
            if(!(this.Icons is null))
            {
                return true;
            }

            this.Icons = new Dictionary<string, IDictionary<string, SpriteData>>();

            Sprite defaultSprite = Resources.Load<Sprite>("Sprites/default");
            //defaultSprite.pivot = new Vector2(0.5f, 0.5f);
            SpriteData iconData = new SpriteData
            {
                m_Name = "DEFAULT",
                m_Parts = new List<SpritePart>
                {
                    new SpritePart
                    {
                        m_Data = new[] { "DEFAULT" },
                        m_Filename = "Sprites/default",
                        m_Frames = 1,
                        m_Name = "DEFAULT",
                        m_Position = 0,
                        m_FrameSprites = new List<Sprite>
                        {
                            defaultSprite
                        },
                        m_PossibleColours = new List<Color>{ Color.white }
                    }
                }
            };

            this.Icons.Add("DEFAULT", new Dictionary<string, SpriteData>
            {
                { iconData.m_Name, iconData }
            });

            string[] files =
                Directory.GetFiles(
                    Directory.GetCurrentDirectory() + GlobalConstants.DATA_FOLDER + "/Sprite Definitions", "*.xml",
                    SearchOption.AllDirectories);

            foreach (string file in files)
            {
                try
                {
                    XElement doc = XElement.Load(file);

                    foreach (XElement tileSetElement in doc.Elements("TileSet"))
                    {
                        string tileSet = tileSetElement.Element("Name").GetAs<string>();

                        this.AddSpriteDataFromXML(tileSet, tileSetElement);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            return true;
        }

        public bool AddSpriteData(string tileSet, SpriteData dataToAdd)
        {
            if (this.Icons.ContainsKey(tileSet))
            {
                if (this.Icons[tileSet].ContainsKey(dataToAdd.m_Name))
                {
                    GlobalConstants.ActionLog.AddText("Trying to add/overwrite sprites, at tile set " + tileSet +
                                                      " with name " + dataToAdd.m_Name);
                    return false;
                }
            }

            List<SpritePart> parts = new List<SpritePart>();
            foreach (SpritePart part in dataToAdd.m_Parts)
            {
                SpritePart copy = part;
                copy.m_FrameSprites = Resources.LoadAll<Sprite>("Sprites/" + part.m_Filename)
                    .Where(
                        (sprite, i) =>
                            i >= part.m_Position
                            && i < part.m_Position +
                            part.m_Frames)
                    .ToList();
                parts.Add(copy);
            }

            dataToAdd.m_Parts = parts;

            if (this.Icons.ContainsKey(tileSet))
            {
                this.Icons[tileSet].Add(dataToAdd.m_Name, dataToAdd);
            }
            else
            {
                this.Icons.Add(new KeyValuePair<string, IDictionary<string, SpriteData>>(
                    tileSet,
                    new Dictionary<string, SpriteData>
                    {
                        { dataToAdd.m_Name, dataToAdd }
                    }));
            }

            return true;
        }

        public bool AddSpriteDataRange(string tileSet, IEnumerable<SpriteData> dataToAdd)
        {
            return dataToAdd.Aggregate(true, (current, data) => current & this.AddSpriteData(tileSet, data));
        }

        public bool AddSpriteDataFromXML(string tileSet, XElement spriteDataElement)
        {
            IEnumerable<SpriteData> spriteData = from data in spriteDataElement.Elements("SpriteData")
                select new SpriteData
                {
                    m_Name = data.Element("Name").GetAs<string>(),
                    m_Parts = (from part in data.Elements("Part")
                        select new SpritePart
                        {
                            m_Data = from d in part.Elements("Data")
                                select d.GetAs<string>(),
                            m_Filename = part.Element("Filename").GetAs<string>(),
                            m_Frames = part.Element("Frames").DefaultIfEmpty(1),
                            m_Name = part.Element("Name").GetAs<string>(),
                            m_Position = part.Element("Position").DefaultIfEmpty(0),
                            m_FrameSprites = Resources.LoadAll<Sprite>("Sprites/" + part.Element("Filename").GetAs<string>())
                                .Where(
                                    (sprite, i) => 
                                        i >= part.Element("Position").DefaultIfEmpty(0) 
                                        && i < part.Element("Position").DefaultIfEmpty(0) + part.Element("Frames").DefaultIfEmpty(1))
                                .ToList(),
                            m_PossibleColours = part.Elements("Colour").Any() 
                                ? (from colour in part.Elements("Colour")
                                select ColourHelper.ParseHTMLString(colour.GetAs<string>())).ToList()
                                : new List<Color> { Color.white },
                            m_SortingOrder = part.Element("SortOrder").DefaultIfEmpty(0)
                        }).ToList()
                };

            return this.AddSpriteDataRange(tileSet, spriteData);
        }

        public IEnumerable<SpriteData> ReturnDefaultData()
        {
            return this.Icons["DEFAULT"].Values;
        }

        public SpriteData ReturnDefaultIcon()
        {
            return this.Icons["DEFAULT"].Values.First();
        }

        public SpriteData GetFrame(string tileSet, string tileName, int frame)
        {
            SpriteData[] frames = this.GetSprites(tileSet, tileName).ToArray();
            return frames.Length >= frame ? frames[frame] : this.ReturnDefaultIcon();
        }

        public IEnumerable<SpriteData> GetSprites(string tileSet, string tileName)
        {
            List<SpriteData> data = this.Icons.Where(x => x.Key.Equals(tileSet, StringComparison.OrdinalIgnoreCase))
                .SelectMany(x => x.Value.Where(pair => pair.Key.Equals(tileName, StringComparison.OrdinalIgnoreCase)))
                .Select(x => x.Value)
                .ToList();

            return data.Any() == false ? this.ReturnDefaultData() : data;
        }

        public IEnumerable<SpriteData> GetTileSet(string tileSet)
        {
            return this.Icons.Where(pair => pair.Key.Equals(tileSet, StringComparison.OrdinalIgnoreCase))
                .SelectMany(pair => pair.Value)
                .Select(pair => pair.Value);
        }

        protected IDictionary<string, IDictionary<string, SpriteData>> Icons
        {
            get;
            set;
        }

        public int SpriteSize
        {
            get
            {
                return this.m_SpriteSize;
            }
            protected set
            {
                this.m_SpriteSize = value;
            }
        }
    }

    public struct SpriteData
    {
        public string m_Name;
        public List<SpritePart> m_Parts;
    }

    public struct SpritePart
    {
        public string m_Name;
        public int m_Frames;
        public IEnumerable<string> m_Data;
        public List<Sprite> m_FrameSprites;
        public string m_Filename;
        public int m_Position;
        public List<Color> m_PossibleColours;
        public int m_SelectedColour;
        public int m_SortingOrder;
    }
}

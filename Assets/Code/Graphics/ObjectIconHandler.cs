using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Castle.Core.Internal;
using JoyLib.Code.Helpers;
using JoyLib.Code.Rollers;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;

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

            this.Icons = new Dictionary<string, List<Tuple<string, SpriteData>>>();

            Sprite defaultSprite = Resources.Load<Sprite>("Sprites/default");
            //defaultSprite.pivot = new Vector2(0.5f, 0.5f);
            SpriteData iconData = new SpriteData
            {
                m_Name = "DEFAULT",
                m_State = "DEFAULT",
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

            this.Icons.Add("DEFAULT", new List<Tuple<string, SpriteData>>
            {
                new Tuple<string, SpriteData>(iconData.m_Name, iconData)
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
                this.Icons[tileSet].Add(new Tuple<string, SpriteData>(dataToAdd.m_Name, dataToAdd));
            }
            else
            {
                this.Icons.Add(new KeyValuePair<string, List<Tuple<string, SpriteData>>>(
                    tileSet,
                    new List<Tuple<string, SpriteData>>
                    {
                        new Tuple<string, SpriteData>(dataToAdd.m_Name, dataToAdd)
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
                    m_State = data.Element("State").DefaultIfEmpty("DEFAULT"),
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
                                select GraphicsHelper.ParseHTMLString(colour.GetAs<string>())).ToList()
                                : new List<Color> { Color.white },
                            m_SortingOrder = part.Element("SortOrder").DefaultIfEmpty(0),
                            m_ImageFillType = GraphicsHelper.ParseFillMethodString(part.Element("FillType").DefaultIfEmpty("filled")),
                            m_SpriteDrawMode = GraphicsHelper.ParseDrawModeString(part.Element("FillType").DefaultIfEmpty("simple"))
                        }).ToList()
                };

            return this.AddSpriteDataRange(tileSet, spriteData);
        }

        /// <summary>
        /// This must be passed the "SpriteData" node of the JSON
        /// </summary>
        /// <param name="tileSet">The tileset that the data belongs to</param>
        /// <param name="spriteDataToken">The JSON to pull the data from. MUST have a root of "SpriteData"</param>
        /// <returns></returns>
        public bool AddSpriteDataFromJson(string tileSet, JToken spriteDataToken)
        {
            List<SpriteData> spriteData = new List<SpriteData>();
            foreach (var data in spriteDataToken)
            {
                string spriteDataName = (string) data["Name"];
                string spriteDataState = (string) data["State"];
                List<SpritePart> parts = new List<SpritePart>();
                foreach (var part in data["Part"])
                {
                    IEnumerable<string> partData = part["Data"].Select(token => (string) token);
                    string filename = (string) part["Filename"];
                    int frames = (int) (part["Frames"] ?? 1);
                    string partName = (string) part["Name"];
                    int position = (int) (part["Position"] ?? 0);
                    List<Sprite> frameSprites = Resources.LoadAll<Sprite>("Sprites/" + filename)
                        .Where((sprite, i) =>
                            i >= position && i < position + frames)
                        .ToList();
                    List<Color> possibleColours = part["Colour"]?.Values<string>()
                        .Select(colour => GraphicsHelper.ParseHTMLString(colour))
                        .ToList();
                    if (possibleColours.IsNullOrEmpty())
                    {
                        possibleColours = new List<Color> {Color.white};
                    }

                    int sortOrder = (int) (part["SortOrder"] ?? 1);
                    Image.Type imageType = GraphicsHelper.ParseFillMethodString((string) part["FillType"]);
                    SpriteDrawMode drawMode = GraphicsHelper.ParseDrawModeString((string) part["FillType"]);
                    parts.Add(
                        new SpritePart
                        {
                            m_Data = partData,
                            m_Filename = filename,
                            m_Frames = frames,
                            m_FrameSprites = frameSprites,
                            m_ImageFillType = imageType,
                            m_Name = partName,
                            m_Position = position,
                            m_PossibleColours = possibleColours,
                            m_SortingOrder = sortOrder,
                            m_SpriteDrawMode = drawMode
                        });
                }
                spriteData.Add(new SpriteData
                {
                    m_Name = spriteDataName,
                    m_Parts = parts,
                    m_State = spriteDataState
                });
            }

            return this.AddSpriteDataRange(tileSet, spriteData);
        }

        public IEnumerable<SpriteData> ReturnDefaultData()
        {
            return this.Icons["DEFAULT"]
                .Select(tuple => tuple.Item2);
        }

        public SpriteData ReturnDefaultIcon()
        {
            SpriteRenderer v = new SpriteRenderer();
            return this.Icons["DEFAULT"].First().Item2;
        }

        public SpriteData GetFrame(string tileSet, string tileName, string state = "DEFAULT", int frame = 0)
        {
            SpriteData[] frames = this.GetSprites(tileSet, tileName, state).ToArray();
            return frames.Length >= frame ? frames[frame] : this.ReturnDefaultIcon();
        }

        public List<Sprite> GetRawFrames(string tileSet, string tileName, string partName, string state = "DEFAULT")
        {
            List<Sprite> sprites = this.GetSprites(tileSet, tileName, state).SelectMany(data => data.m_Parts)
                .Where(part => part.m_Name.Equals(partName, StringComparison.OrdinalIgnoreCase))
                .SelectMany(part => part.m_FrameSprites)
                .ToList();

            return sprites;
        }

        public IEnumerable<SpriteData> GetSprites(string tileSet, string tileName, string state = "DEFAULT")
        {
            List<SpriteData> data = this.Icons.Where(x => x.Key.Equals(tileSet, StringComparison.OrdinalIgnoreCase))
                .SelectMany(x => x.Value.Where(pair => pair.Item1.Equals(tileName, StringComparison.OrdinalIgnoreCase)))
                .Where(pair => pair.Item2.m_State.Equals(state, StringComparison.OrdinalIgnoreCase))
                .Select(x => x.Item2)
                .ToList();

            return data.Any() == false ? this.ReturnDefaultData() : data;
        }

        public IEnumerable<SpriteData> GetTileSet(string tileSet)
        {
            return this.Icons.Where(pair => pair.Key.Equals(tileSet, StringComparison.OrdinalIgnoreCase))
                .SelectMany(pair => pair.Value)
                .Select(pair => pair.Item2);
        }

        protected IDictionary<string, List<Tuple<string, SpriteData>>> Icons
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

    [Serializable]
    public class SpriteData
    {
        public string m_Name;
        public string m_State;
        public List<SpritePart> m_Parts;
    }

    [Serializable]
    public class SpritePart
    {
        public string m_Name;
        public int m_Frames;
        public IEnumerable<string> m_Data;
        [NonSerialized]
        public List<Sprite> m_FrameSprites;
        public string m_Filename;
        public int m_Position;
        public List<Color> m_PossibleColours;
        public int m_SelectedColour;
        public int m_SortingOrder;
        public Image.Type m_ImageFillType;
        public SpriteDrawMode m_SpriteDrawMode;

        public Color SelectedColour => this.m_PossibleColours[this.m_SelectedColour];
    }
}

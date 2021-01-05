using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using JoyLib.Code.Collections;
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

            this.Icons = new BucketCollection<IconData, string>();

            Texture2D loadedSprite = Resources.Load<Texture2D>("Sprites/default");

            IconData iconData = new IconData()
            {
                data = "DEFAULT",
                name = "DEFAULT",
                texture = loadedSprite,
                sprite = Sprite.Create(loadedSprite, new Rect(0,
                                                               0,
                                                               this.SpriteSize,
                                                               this.SpriteSize), 
                                                               Vector2.zero, 
                                                               this.SpriteSize),
                filename = "Sprites/default"
            };

            this.Icons.Add(iconData, "DEFAULT");

            string[] files =
                Directory.GetFiles(
                    Directory.GetCurrentDirectory() + GlobalConstants.DATA_FOLDER + "/Sprite Definitions", "*.xml",
                    SearchOption.AllDirectories);

            foreach (string file in files)
            {
                try
                {
                    XElement doc = XElement.Load(file);

                    string tileSet = doc.Element("TilesetName").GetAs<string>();
                    string sheet = doc.Element("Sheet").GetAs<string>();

                    IconData[] iconDatas = (from data in doc.Elements("Icon")
                        select new IconData()
                        {
                            data = data.Element("Data").DefaultIfEmpty("DEFAULT"),
                            filename = sheet,
                            frames = data.Element("Frames").DefaultIfEmpty(1),
                            name = data.Element("Name").GetAs<string>(),
                            position = data.Element("Position").GetAs<int>()
                        }).ToArray();

                    this.AddIcons(tileSet, iconDatas);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            return true;
        }

        public bool AddIcons(string tileSet, IconData[] data)
        {
            if(this.Icons is null)
            {
                this.Load();
            }

            for (int i = 0; i < data.Length; i++)
            {
                int jIndex = data[i].frames > 0 ? data[i].frames : 1;
                Sprite[] sheets = Resources.LoadAll<Sprite>("Sprites/" + data[i].filename);
                for (int j = 0; j < jIndex; j++)
                {
                    IconData icon = new IconData()
                    {
                        name = data[i].name,
                        data = data[i].data,
                        texture = sheets[data[i].position + j].texture,
                        sprite = sheets[data[i].position + j],
                        filename = data[i].filename,
                        position = data[i].position
                    };

                    if (this.Icons.ContainsKey(icon))
                    {
                        this.Icons[icon].Add(tileSet);
                    }
                    else
                    {
                        this.Icons.Add(icon, tileSet);
                    }
                }
            }

            return true;
        }

        private Tuple<Texture2D, Sprite> MakeSprite(Texture2D sheet, Vector2Int point, int spriteSize)
        {
            Color[] imageData = sheet.GetPixels();

            Rect sourceRectangle = new Rect(point.x * spriteSize, point.y * spriteSize, spriteSize, spriteSize);

            Color[] imagePiece = this.GetImageData(imageData, sheet.width, sourceRectangle);
            Texture2D subTexture = new Texture2D(spriteSize, spriteSize, TextureFormat.RGBA32, false, false);
            subTexture.SetPixels(imagePiece);
            subTexture.filterMode = FilterMode.Point;

            Sprite sprite = Sprite.Create(subTexture, new Rect(0, 0, spriteSize, spriteSize), Vector2.zero, spriteSize);
            return new Tuple<Texture2D, Sprite>(subTexture, sprite);
        }

        //This assumes using a single sheet laid out in a horizontal row
        private List<Tuple<Texture2D, Sprite>> MakeSpritesFromOneSheet(Texture2D sheet, int frames, Vector2Int startPoint, int spriteSize)
        {
            Color[] imageData = sheet.GetPixels();

            List<Tuple<Texture2D, Sprite>> tuples = new List<Tuple<Texture2D, Sprite>>();

            for(int i = 0; i < frames; i++)
            {
                Rect sourceRectangle = new Rect(startPoint.x * spriteSize + (i * spriteSize), startPoint.y * spriteSize, spriteSize, spriteSize);

                Color[] imagePiece = this.GetImageData(imageData, sheet.width, sourceRectangle);
                Texture2D subTexture = new Texture2D(spriteSize, spriteSize, TextureFormat.RGBA32, false, false);
                subTexture.SetPixels(imagePiece);
                subTexture.filterMode = FilterMode.Point;

                Sprite sprite = Sprite.Create(subTexture, new Rect(0, 0, spriteSize, spriteSize), Vector2.zero, spriteSize);
                tuples.Add(new Tuple<Texture2D, Sprite>(subTexture, sprite));
            }

            return tuples;
        }

        private Color[] GetImageData(Color[] colourData, int textureWidth, Rect rectangle)
        {
            int width, height;
            width = (int)rectangle.width;
            height = (int)rectangle.height;
            Color[] color = new Color[width * height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int index = (int)((rectangle.y * textureWidth) + (y * textureWidth) + x + rectangle.x);
                    color[x + y * width] = colourData[index];
                }
            }

            return color;
        }

        public Texture2D[] GetIcons(string tileSet, string tileName)
        {
            List<Texture2D> icons = new List<Texture2D>();
            if(this.Icons.ContainsValue(tileSet))
            {
                List<IconData> textures = this.Icons[tileSet].FindAll(x => x.name.StartsWith(tileName, StringComparison.OrdinalIgnoreCase) 
                                                                           || x.data.StartsWith(tileName, StringComparison.OrdinalIgnoreCase));

                if(textures.Count == 0)
                {
                    IconData[] defaultIcons = this.GetDefaultIconSet(tileSet);
                    List<Texture2D> tempTextures = new List<Texture2D>(defaultIcons.Length);
                    foreach(IconData iconData in defaultIcons)
                    {
                        tempTextures.Add(iconData.texture);
                    }
                    icons.AddRange(tempTextures);
                }
            }

            return icons.ToArray();
        }

        private IconData[] GetDefaultIconSet(string tileSet)
        {
            string lowerTileSet = tileSet;
            if(tileSet != null && this.Icons.ContainsValue(lowerTileSet))
            {
                IconData[] icons = this.Icons[lowerTileSet].Where(x => x.data.Equals("default", StringComparison.OrdinalIgnoreCase)).ToArray();
                if(icons.Length == 0)
                {
                    return this.ReturnDefaultArray();
                }
                int result = this.Roller.Roll(0, icons.Length);
                string[] nameToFind = Regex.Split(icons[result].name, @"^[^\d]+");
                icons = icons.Where(x => x.name.StartsWith(nameToFind[0])).ToArray();
                if(icons.Length == 0)
                {
                    return this.ReturnDefaultArray();
                }
                return icons;
            }
            else
            {
                return this.ReturnDefaultArray();
            }
        }

        public IconData[] ReturnDefaultArray()
        {
            IconData[] defaultIcon = this.Icons["DEFAULT"].ToArray();
            return defaultIcon;
        }

        public IconData ReturnDefaultIcon()
        {
            IconData[] defaultIcon = this.Icons["DEFAULT"].ToArray();
            return defaultIcon[0];
        }

        public Sprite[] GetDefaultSprites()
        {
            Sprite[] defaultSprites = this.Icons["DEFAULT"].Select(x => x.sprite).ToArray();
            return defaultSprites;
        }

        public Sprite GetSprite(string tileSet, string tileName)
        {
            List<KeyValuePair<IconData, List<string>>> data = this.Icons.Where(x => x.Value.Contains(tileSet, GlobalConstants.STRING_COMPARER)).ToList();
            List<IconData> query = new List<IconData>();

            foreach(KeyValuePair<IconData, List<string>> pair in data)
            {
                query.Add(pair.Key);
            }

            if(query.Count > 0)
            {
                if(query.Any(x => x.name.Equals(tileName, StringComparison.OrdinalIgnoreCase)))
                {
                    return query.First(x => x.name.Equals(tileName, StringComparison.OrdinalIgnoreCase) 
                                            || x.data.Equals(tileName, StringComparison.OrdinalIgnoreCase)).sprite;
                    
                }
                else
                {
                    if (query.Any(x => x.data.Equals("default", StringComparison.OrdinalIgnoreCase)))
                    {
                        return query.First(x => x.data.Equals("default", StringComparison.OrdinalIgnoreCase)).sprite;
                    }
                }
            }
            
            IconData[] icons = this.GetDefaultIconSet(tileSet);
            return icons[0].sprite;
        }

        public IEnumerable<Sprite> GetSprites(string tileSet, string tileName)
        {
            List<Sprite> sprites = new List<Sprite>();

            List<IconData> data = this.Icons.Where(x => x.Value.Contains(tileSet, GlobalConstants.STRING_COMPARER))
                .Select(pair => pair.Key)
                .ToList();

            List<IconData> find = data.FindAll(x => x.name.Equals(tileName, StringComparison.OrdinalIgnoreCase) 
                                                    || x.data.Equals(tileName, StringComparison.OrdinalIgnoreCase));
            foreach(IconData found in find)
            {
                sprites.Add(found.sprite);
            }

            if(sprites.Count == 0)
            {
                if (data.Any(x => x.data.Equals("default", StringComparison.OrdinalIgnoreCase)))
                {
                    IconData[] defaultIcons = data.Where(x => x.data.Equals("default", StringComparison.OrdinalIgnoreCase)).ToArray();
                    sprites.AddRange(defaultIcons.Select(icon => icon.sprite));
                }
                else
                {
                    IconData[] icons = this.GetDefaultIconSet(tileSet);
                    foreach (IconData icon in icons)
                    {
                        sprites.Add(icon.sprite);
                    }
                }
            }

            return sprites.ToArray();
        }

        public IEnumerable<Sprite> GetTileSet(string tileSet)
        {
            return this.Icons.Where(pair => pair.Value.Contains(tileSet))
                .Select(pair => pair.Key.sprite);
        }

        private BucketCollection<IconData, string> Icons
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

    public struct IconData
    {
        public string name;
        public string data;
        public int frames;
        public Texture2D texture;
        public Sprite sprite;
        public string filename;
        public int position;
    }
}

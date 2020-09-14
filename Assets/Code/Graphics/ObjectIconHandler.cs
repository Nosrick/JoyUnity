using JoyLib.Code.Rollers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using JoyLib.Code.Collections;

namespace JoyLib.Code.Graphics
{
    public class ObjectIconHandler : MonoBehaviour
    {

        protected int m_SpriteSize = 16;

        public void Awake()
        {
            this.SpriteSize = 16;
            Load();
        }

        public void Initalise(int spriteSize) 
        {
            this.SpriteSize = spriteSize;

            Load();
        }

        public bool Load()
        {
            if(!(Icons is null))
            {
                return true;
            }
            Icons = new BucketCollection<IconData, string>();

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

            Icons.Add(iconData, "DEFAULT");

            loadedSprite = Resources.Load<Texture2D>("Sprites/obscure");

            iconData = new IconData()
            {
                data = "obscure",
                name = "obscure",
                texture = loadedSprite,
                sprite = Sprite.Create(loadedSprite, new Rect(0,
                                                                0,
                                                                this.SpriteSize,
                                                                this.SpriteSize),
                                                                Vector2.zero,
                                                                this.SpriteSize),
                filename = "Sprites/obscure"
            };

            Icons.Add(iconData, "obscure");

            return true;
        }

        public bool AddIcons(string tileSet, IconData[] data)
        {
            if(Icons is null)
            {
                Load();
            }
            
            for(int i = 0; i < data.Length; i++)
            {
                for (int j = 0; j < data[i].frames; j++)
                {
                    Sprite sheet = Resources.Load<Sprite>("Sprites/" + data[i].filename);

                    IconData icon = new IconData()
                    {
                        name = data[i].name,
                        data = data[i].data,
                        texture = sheet.texture,
                        sprite = sheet,
                        filename = data[i].filename
                    };

                    if (Icons.ContainsKey(icon))
                    {
                        Icons[icon].Add(tileSet);
                    }
                    else
                    {
                        Icons.Add(icon, tileSet);
                    }
                }
            }

            /*
            Texture2D sheet = Resources.Load<Texture2D>("Sprites/" + filename);
            for(int i = 0; i < data.Length; i++)
            {
                if(data[i].frames > 1)
                {
                    List<Tuple<Texture2D, Sprite>> tuples = MakeSpritesFromOneSheet(sheet, data[i].frames, data[i].position, this.SpriteSize);
                    for (int j = 0; j < tuples.Count; j++)
                    {
                        Tuple<Texture2D, Sprite> tuple = tuples[j];
                        IconData iconData = new IconData()
                        {
                            name = data[i].name + j.ToString(),
                            data = data[i].data,
                            position = data[i].position,
                            texture = tuple.Item1,
                            sprite = tuple.Item2
                        };

                        if(Icons.ContainsKey(iconData))
                        {
                            Icons[iconData].Add(tileSet);
                        }
                        else
                        {
                            Icons.Add(iconData, tileSet);
                        }
                    }
                }
                else
                {
                    Tuple<Texture2D, Sprite> tuple = MakeSprite(sheet, data[i].position, this.SpriteSize);
                    IconData iconData = new IconData()
                    {
                        name = data[i].name,
                        data = data[i].data,
                        position = data[i].position,
                        texture = tuple.Item1,
                        sprite = tuple.Item2
                    };

                    if(Icons.ContainsKey(iconData))
                    {
                        Icons[iconData].Add(tileSet);
                    }
                    else
                    {
                        Icons.Add(iconData, tileSet);
                    }
                }
            }
            */

            return true;
        }

        private Tuple<Texture2D, Sprite> MakeSprite(Texture2D sheet, Vector2Int point, int spriteSize)
        {
            Color[] imageData = sheet.GetPixels();

            Rect sourceRectangle = new Rect(point.x * spriteSize, point.y * spriteSize, spriteSize, spriteSize);

            Color[] imagePiece = GetImageData(imageData, sheet.width, sourceRectangle);
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

                Color[] imagePiece = GetImageData(imageData, sheet.width, sourceRectangle);
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
            if(Icons.ContainsValue(tileSet))
            {
                List<IconData> textures = Icons[tileSet].FindAll(x => x.name.StartsWith(tileName, StringComparison.OrdinalIgnoreCase) 
                    || x.data.StartsWith(tileName, StringComparison.OrdinalIgnoreCase));

                if(textures.Count == 0)
                {
                    IconData[] defaultIcons = GetDefaultIconSet(tileSet);
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
            if(tileSet != null && Icons.ContainsValue(lowerTileSet))
            {
                IconData[] icons = Icons[lowerTileSet].Where(x => x.data.Equals("default", StringComparison.OrdinalIgnoreCase)).ToArray();
                if(icons.Length == 0)
                {
                    return ReturnDefaultArray();
                }
                int result = RNG.instance.Roll(0, icons.Length - 1);
                string[] nameToFind = Regex.Split(icons[result].name, @"^[^\d]+");
                icons = icons.Where(x => x.name.StartsWith(nameToFind[0])).ToArray();
                if(icons.Length == 0)
                {
                    return ReturnDefaultArray();
                }
                return icons;
            }
            else
            {
                return ReturnDefaultArray();
            }
        }

        public IconData[] ReturnDefaultArray()
        {
            IconData[] defaultIcon = Icons["DEFAULT"].ToArray();
            return defaultIcon;
        }

        public IconData ReturnDefaultIcon()
        {
            IconData[] defaultIcon = Icons["DEFAULT"].ToArray();
            return defaultIcon[0];
        }

        public Sprite[] GetDefaultSprites()
        {
            Sprite[] defaultSprites = Icons["DEFAULT"].Select(x => x.sprite).ToArray();
            return defaultSprites;
        }

        public Sprite GetSprite(string tileSet, string tileName)
        {
            List<KeyValuePair<IconData, List<string>>> data = Icons.Where(x => x.Value.Contains(tileSet, GlobalConstants.STRING_COMPARER)).ToList();
            List<IconData> query = new List<IconData>();

            foreach(KeyValuePair<IconData, List<string>> pair in data)
            {
                query.Add(pair.Key);
            }

            if(query.Count > 0)
            {
                return query.First(x => x.name.Equals(tileName, StringComparison.OrdinalIgnoreCase) 
                || x.data.Equals(tileName, StringComparison.OrdinalIgnoreCase)).sprite;
            }
            
            IconData[] icons = GetDefaultIconSet(tileSet);
            return icons[0].sprite;
        }

        public Sprite[] GetSprites(string tileSet, string tileName)
        {
            List<Sprite> sprites = new List<Sprite>();

            List<KeyValuePair<IconData, List<string>>> data = Icons.Where(x => x.Value.Contains(tileSet, GlobalConstants.STRING_COMPARER)).ToList();
            List<IconData> query = new List<IconData>();

            foreach(KeyValuePair<IconData, List<string>> pair in data)
            {
                query.Add(pair.Key);
            }

            List<IconData> find = query.FindAll(x => x.name.StartsWith(tileName, StringComparison.OrdinalIgnoreCase) 
                                                                    || x.data.StartsWith(tileName, StringComparison.OrdinalIgnoreCase));
            foreach(IconData found in find)
            {
                sprites.Add(found.sprite);
            }

            if(sprites.Count == 0)
            {
                IconData[] icons = GetDefaultIconSet(tileSet);
                foreach (IconData icon in icons)
                {
                    sprites.Add(icon.sprite);
                }
            }

            return sprites.ToArray();
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
                return m_SpriteSize;
            }
            protected set
            {
                m_SpriteSize = value;
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
    }
}

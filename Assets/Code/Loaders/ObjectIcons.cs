using JoyLib.Code.Rollers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using JoyLib.Code.Collections;

namespace JoyLib.Code.Graphics
{
    public class ObjectIconHandler
    {
        private static readonly Lazy<ObjectIconHandler> lazy = new Lazy<ObjectIconHandler>(() => new ObjectIconHandler(16));
        public static ObjectIconHandler instance = lazy.Value;

        public ObjectIconHandler(int spriteSize) 
        {
            this.SpriteSize = spriteSize;
        }

        public bool Load()
        {
            if(Icons != null)
            {
                return true;
            }

            Icons = new BucketCollection<IconData, string>();
            Texture2D defaultSprite = Resources.Load<Texture2D>("Sprites/default");

            List<IconData> defaultIconData = new List<IconData>();
            IconData iconData = new IconData()
            {
                data = "DEFAULT",
                name = "DEFAULT",
                position = Vector2Int.zero,
                texture = defaultSprite,
                sprite = Sprite.Create(defaultSprite, new Rect(0,
                                                               0,
                                                               this.SpriteSize,
                                                               this.SpriteSize), 
                                                               Vector2.zero, 
                                                               this.SpriteSize)
            };

            Icons.Add(iconData, "DEFAULT");

            return true;
        }

        public bool AddIcons(string filename, string tileSet, IconData[] data)
        {
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
                List<IconData> textures = Icons[tileSet].FindAll(x => x.name.StartsWith(tileName.ToLower()) || x.data.StartsWith(tileName.ToLower()));

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
            string lowerTileSet = tileSet.ToLower();
            if(tileSet != null && Icons.ContainsValue(lowerTileSet))
            {
                IconData[] icons = Icons[lowerTileSet].Where(x => x.data.ToLower() == "default").ToArray();
                if(icons.Length == 0)
                {
                    return ReturnDefaultArray();
                }
                int result = RNG.Roll(0, icons.Length - 1);
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

        public Sprite GetSprite(string tileSet, string tileName)
        {
            string lowerTileSet, lowerTileName;
            lowerTileName = tileName.ToLower();
            lowerTileSet = tileSet.ToLower();
            if(Icons.ContainsValue(lowerTileSet))
            {
                if(Icons.Any(x => x.Key.name.Equals(lowerTileName) && x.Value.Contains(lowerTileSet)))
                {
                    return Icons.First(x => x.Key.name.StartsWith(lowerTileName) && x.Value.Contains(lowerTileSet)).Key.sprite;
                }
            }

            IconData[] icons = GetDefaultIconSet(lowerTileSet);
            return icons[0].sprite;
        }

        public Sprite[] GetSprites(string tileSet, string tileName)
        {
            string lowerTileSet, lowerTileName;
            lowerTileSet = tileSet.ToLower();
            lowerTileName = tileName.ToLower();
            List<Sprite> sprites = new List<Sprite>();
            if(lowerTileSet != null && Icons.ContainsValue(lowerTileSet))
            {
                List<IconData> find = Icons[lowerTileSet].FindAll(x => x.name.StartsWith(lowerTileName) 
                                                                    || x.data.StartsWith(lowerTileName));
                foreach(IconData found in find)
                {
                    sprites.Add(found.sprite);
                }
            }

            if(sprites.Count == 0)
            {
                IconData[] icons = GetDefaultIconSet(lowerTileSet);
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
            get;
            protected set;
        }
    }

    public struct IconData
    {
        public string name;
        public string data;
        public int frames;
        public Vector2Int position;
        public Texture2D texture;
        public Sprite sprite;
    }
}

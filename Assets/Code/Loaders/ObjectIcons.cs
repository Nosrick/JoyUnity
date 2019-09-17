using JoyLib.Code.Helpers;
using JoyLib.Code.Rollers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using UnityEngine;

namespace JoyLib.Code.Graphics
{
    public static class ObjectIconHandler
    {
        public const int SPRITE_SIZE = 16;

        public static bool Load()
        {
            if(Icons != null)
            {
                return true;
            }

            Icons = new Dictionary<string, List<IconData>>();
            
            Texture2D defaultSprite = Resources.Load<Texture2D>("Sprites/default");

            List<IconData> defaultIconData = new List<IconData>();
            IconData iconData = new IconData()
            {
                data = "DEFAULT",
                name = "DEFAULT",
                position = Vector2Int.zero,
                texture = defaultSprite,
                sprite = Sprite.Create(defaultSprite, new Rect(0, 0, SPRITE_SIZE, SPRITE_SIZE), Vector2.zero, SPRITE_SIZE)
            };
            defaultIconData.Add(iconData);

            Icons.Add("DEFAULT", defaultIconData);

            return true;
            /*
            string[] objectDataFiles = Directory.GetFiles(Directory.GetCurrentDirectory() + GlobalConstants.DATA_FOLDER + "Sprite Definitions", "*.xml", SearchOption.AllDirectories);

            Dictionary<string, Texture2D> sheets = new Dictionary<string, Texture2D>();

            for (int i = 0; i < objectDataFiles.GetLength(0); i++)
            {
                List<IconData> tilesetIcons = new List<IconData>();
                string tilesetName = "DEFAULT TILESET";

                XmlReader reader = XmlReader.Create(objectDataFiles[i]);

                while (reader.Read())
                {
                    if (reader.Depth == 0 && reader.NodeType == XmlNodeType.Element && !reader.Name.Equals("Objects"))
                        break;

                    if (reader.Name.Equals(""))
                        continue;

                    if (reader.Name.Equals("TilesetName"))
                        tilesetName = reader.ReadElementContentAsString();

                    if (reader.Name.Equals("Sheet"))
                    {
                        try
                        {
                            string sheetName = reader.ReadElementContentAsString();
                            string fileName = sheetName;// + ".png";
                            sheets.Add(sheetName, Resources.Load<Texture2D>("Sprites/" + fileName));
                        }
                        catch (Exception e)
                        {
                            Console.Out.WriteLine(e.Message);
                            Console.Out.WriteLine(e.StackTrace);
                        }
                    }

                    if (reader.Name.Equals("Icon"))
                    {
                        string name = "DEFAULT";
                        int x = -1;
                        int y = -1;
                        string data = "DEFAULT";

                        do
                        {
                            reader.Read();

                            if (reader.Name.Equals("Name"))
                            {
                                name = reader.ReadElementContentAsString();
                            }
                            else if (reader.Name.Equals("X"))
                            {
                                x = reader.ReadElementContentAsInt();
                            }
                            else if (reader.Name.Equals("Y"))
                            {
                                y = reader.ReadElementContentAsInt();
                            }
                            else if (reader.Name.Equals("Data"))
                            {
                                data = reader.ReadElementContentAsString();
                            }
                        } while (reader.Name.Equals("Icon") == false && reader.NodeType != XmlNodeType.EndElement);

                        int j = 0;
                        foreach(KeyValuePair<string, Texture2D> sheetPair in sheets)
                        {
                            
                            //subTexture.SetData<Color>(imagePiece);

                            iconData = new IconData()
                            {
                                name = name + j,
                                data = data,
                                position = new Vector2Int(x, y),
                                texture = subTexture,
                                sprite = sprite
                            };

                            tilesetIcons.Add(iconData);
                            j += 1;
                        }
                    }
                }
                if (!Icons.ContainsKey(tilesetName))
                {
                    Icons.Add(tilesetName, tilesetIcons);
                }
                else
                {
                    Icons[tilesetName].AddRange(tilesetIcons);
                }
                sheets.Clear();

                reader.Close();
            }
            */
        }

        public static bool AddIcons(string filename, string tileSet, IconData[] data)
        {
            Texture2D sheet = Resources.Load<Texture2D>("Sprites/" + filename);
            for(int i = 0; i < data.Length; i++)
            {
                if(data[i].frames > 1)
                {
                    List<Tuple<Texture2D, Sprite>> tuples = MakeSpritesFromOneSheet(sheet, data[i].frames, data[i].position);
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

                        if(Icons.ContainsKey(tileSet))
                        {
                            Icons[tileSet].Add(iconData);
                        }
                        else
                        {
                            Icons.Add(tileSet, new List<IconData>() { iconData });
                        }
                    }
                }
                else
                {
                    Tuple<Texture2D, Sprite> tuple = MakeSprite(sheet, data[i].position);
                    IconData iconData = new IconData()
                    {
                        name = data[i].name,
                        data = data[i].data,
                        position = data[i].position,
                        texture = tuple.Item1,
                        sprite = tuple.Item2
                    };

                    if(Icons.ContainsKey(tileSet))
                    {
                        Icons[tileSet].Add(iconData);
                    }
                    else
                    {
                        Icons.Add(tileSet, new List<IconData>() { iconData });
                    }
                }
            }

            return true;
        }

        private static Tuple<Texture2D, Sprite> MakeSprite(Texture2D sheet, Vector2Int point, int spriteSize = SPRITE_SIZE)
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
        private static List<Tuple<Texture2D, Sprite>> MakeSpritesFromOneSheet(Texture2D sheet, int frames, Vector2Int startPoint, int spriteSize = SPRITE_SIZE)
        {
            Color[] imageData = sheet.GetPixels();

            List<Tuple<Texture2D, Sprite>> tuples = new List<Tuple<Texture2D, Sprite>>();

            for(int i = 0; i < frames; i++)
            {
                Rect sourceRectangle = new Rect(startPoint.x + (spriteSize * i), startPoint.y * spriteSize, spriteSize, spriteSize);

                Color[] imagePiece = GetImageData(imageData, sheet.width, sourceRectangle);
                Texture2D subTexture = new Texture2D(spriteSize, spriteSize, TextureFormat.RGBA32, false, false);
                subTexture.SetPixels(imagePiece);
                subTexture.filterMode = FilterMode.Point;

                Sprite sprite = Sprite.Create(subTexture, new Rect(0, 0, spriteSize, spriteSize), Vector2.zero, spriteSize);
                tuples.Add(new Tuple<Texture2D, Sprite>(subTexture, sprite));
            }

            return tuples;
        }

        private static Color[] GetImageData(Color[] colourData, int textureWidth, Rect rectangle)
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

        public static Texture2D[] GetIcons(string tileSet, string tileName)
        {
            List<Texture2D> icons = new List<Texture2D>();
            if(Icons.ContainsKey(tileSet))
            {
                List<IconData> textures = Icons[tileSet].FindAll(x => x.name.StartsWith(tileName) || x.data.StartsWith(tileName));

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

        private static IconData[] GetDefaultIconSet(string tileSet)
        {
            if(tileSet != null && Icons.ContainsKey(tileSet))
            {
                IconData[] icons = Icons[tileSet].Where(x => x.data.ToLower() == "default").ToArray();
                int result = RNG.Roll(0, icons.Length - 1);
                string[] nameToFind = Regex.Split(icons[result].name, @"^[^\d]+");
                icons = icons.Where(x => x.name.StartsWith(nameToFind[0])).ToArray();
                return icons;
            }
            else
            {
                List<IconData> defaultIcon = Icons["DEFAULT"];
                return new IconData[] { defaultIcon[0] };
            }
        }

        public static Sprite GetSprite(string tileSet, string tileName)
        {
            if(Icons.ContainsKey(tileSet))
            {
                if(Icons[tileSet].Any(x => x.name.StartsWith(tileName)))
                {
                    return Icons[tileSet].First(x => x.name.StartsWith(tileName)).sprite;
                }
            }

            IconData[] icons = GetDefaultIconSet(tileSet);
            return icons[0].sprite;
        }

        public static Sprite[] GetSprites(string tileSet, string tileName)
        {
            List<Sprite> sprites = new List<Sprite>();
            if(tileSet != null && Icons.ContainsKey(tileSet))
            {
                List<IconData> find = Icons[tileSet].FindAll(x => x.name.StartsWith(tileName) || x.data.StartsWith(tileName));
                foreach(IconData found in find)
                {
                    sprites.Add(found.sprite);
                }
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

        //First key is tileset name, second key is tile name
        private static Dictionary<string, List<IconData>> Icons
        {
            get;
            set;
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

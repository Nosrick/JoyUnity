using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using UnityEngine;

namespace JoyLib.Code.Graphics
{
    public static class ObjectIcons
    {
        public const int SPRITE_SIZE = 16;

        public static void Load()
        {
            objectIcons = new Dictionary<string, List<Tuple<string, Texture2D>>>();
            
            Texture2D defaultSprite = Resources.Load<Texture2D>("Sprites/default");

            List<Tuple<string, Texture2D>> defaultSpriteDictionary = new List<Tuple<string, Texture2D>>();
            defaultSpriteDictionary.Add(new Tuple<string, Texture2D>("DEFAULT", defaultSprite));
            objectIcons.Add("DEFAULT", defaultSpriteDictionary);

            string objectDataFolder = Directory.GetCurrentDirectory() + "//Data//Sprite Definitions";
            string[] objectDataFiles = Directory.GetFiles(objectDataFolder, "*.xml", SearchOption.AllDirectories);

            Dictionary<string, Texture2D> sheets = new Dictionary<string, Texture2D>();

            for (int i = 0; i < objectDataFiles.GetLength(0); i++)
            {
                List<Tuple<string, Texture2D>> tilesetIcons = new List<Tuple<string, Texture2D>>();
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

                    if (reader.Name.Equals("ItemIcon"))
                    {
                        string name = reader.GetAttribute("Name");
                        string positionString = reader.GetAttribute("Position");
                        string[] xyStrings = positionString.Split(new char[] { ',' });
                        int x, y;
                        int.TryParse(xyStrings[0], out x);
                        int.TryParse(xyStrings[1], out y);

                        int j = 0;
                        foreach(KeyValuePair<string, Texture2D> sheetPair in sheets)
                        {
                            Color[] imageData = sheetPair.Value.GetPixels();

                            Rect sourceRectangle = new Rect(x * 16, y * 16, 16, 16);

                            Color[] imagePiece = GetImageData(imageData, sheetPair.Value.width, sourceRectangle);
                            Texture2D subTexture = new Texture2D(16, 16, TextureFormat.RGBA32, false, false);
                            subTexture.SetPixels(imagePiece);
                            subTexture.filterMode = FilterMode.Point;
                            //subTexture.SetData<Color>(imagePiece);

                            tilesetIcons.Add(new Tuple<string, Texture2D>(name + j, subTexture));
                            j += 1;
                        }
                    }
                }
                if (!objectIcons.ContainsKey(tilesetName))
                {
                    objectIcons.Add(tilesetName, tilesetIcons);
                }
                else
                {
                    objectIcons[tilesetName].AddRange(tilesetIcons);
                }
                sheets.Clear();

                reader.Close();
            }

            Sprites = new Dictionary<string, List<Tuple<string, Sprite>>>();
            foreach(KeyValuePair<string, List<Tuple<string, Texture2D>>> pair in objectIcons)
            {
                List<Tuple<string, Sprite>> newSprites = new List<Tuple<string, Sprite>>();
                foreach (Tuple<string, Texture2D> tuple in pair.Value)
                {
                    Sprite sprite = Sprite.Create(tuple.Second, new Rect(0, 0, SPRITE_SIZE, SPRITE_SIZE), Vector2.zero, SPRITE_SIZE);
                    newSprites.Add(new Tuple<string, Sprite>(tuple.First, sprite));
                }
                Sprites.Add(pair.Key, newSprites);
            }
        }

        private static Color[] GetImageData(Color[] colourData, int textureWidth, Rect rectangle)
        {
            int width, height;
            width = (int)rectangle.width;
            height = (int)rectangle.height;
            Color[] color = new Color[width * height];
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                {
                    int index = (int)((rectangle.y * textureWidth) + (y * textureWidth) + x + rectangle.x);
                    color[x + y * width] = colourData[index];
                }

            return color;
        }

        public static Texture2D GetIcon(string tileSet, string tileName)
        {
            if(objectIcons.ContainsKey(tileSet))
            {
                if (objectIcons[tileSet].Any(x => x.First.Equals(tileName)))
                {
                    return objectIcons[tileSet].First(x => x.First.Equals(tileName)).Second;
                }
            }

            return objectIcons["DEFAULT"][0].Second;
        }

        public static Texture2D[] GetIcons(string tileSet, string tileName)
        {
            List<Texture2D> icons = new List<Texture2D>();
            if(objectIcons.ContainsKey(tileSet))
            {
                List<Tuple<string, Texture2D>> textures = objectIcons[tileSet].FindAll(x => x.First.Contains(tileName));
                foreach(Tuple<string, Texture2D> texture in textures)
                {
                    icons.Add(texture.Second);
                }
            }

            if(icons.Count == 0)
            {
                Texture2D[] defaultIcon = new Texture2D[1];
                defaultIcon[0] = objectIcons["DEFAULT"][0].Second;
                return defaultIcon;
            }

            return icons.ToArray();
        }

        public static Sprite GetSprite(string tileSet, string tileName)
        {
            if(Sprites.ContainsKey(tileSet))
            {
                if(Sprites[tileSet].Any(x => x.First.Equals(tileName)))
                {
                    return Sprites[tileSet].First(x => x.First.Equals(tileName)).Second;
                }
            }

            return Sprites["DEFAULT"][0].Second;
        }

        public static List<Sprite> GetSprites(string tileSet, string tileName)
        {
            List<Sprite> sprites = new List<Sprite>();
            if(Sprites.ContainsKey(tileSet))
            {
                List<Tuple<string, Sprite>> find = Sprites[tileSet].FindAll(x => x.First.Contains(tileName));
                foreach(Tuple<string, Sprite> found in find)
                {
                    sprites.Add(found.Second);
                }
            }

            if(sprites.Count == 0)
            {
                List<Sprite> defaultSprite = new List<Sprite>();
                defaultSprite.Add(Sprites["DEFAULT"][0].Second);
                return defaultSprite;
            }

            return sprites;
        }

        //First key is tileset name, second key is tile name
        public static Dictionary<string, List<Tuple<string, Texture2D>>> objectIcons
        {
            get;
            private set;
        }

        //First key is the tileset name, second key is the tile name
        public static Dictionary<string, List<Tuple<string, Sprite>>> Sprites
        {
            get;
            private set;
        }
    }
}

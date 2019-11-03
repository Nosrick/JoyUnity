using JoyLib.Code.Graphics;
using JoyLib.Code.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace JoyLib.Code.World
{
    public static class WorldInfoHandler
    {
        private static List<WorldInfo> s_WorldInfo;

        public static bool Load()
        {
            if(s_WorldInfo != null)
            {
                return true;
            }

            string[] files = Directory.GetFiles(Directory.GetCurrentDirectory() + GlobalConstants.DATA_FOLDER + "World Spaces", "*.xml", SearchOption.AllDirectories);
            s_WorldInfo = new List<WorldInfo>();

            foreach(string file in files)
            {
                XmlReader reader = XmlReader.Create(file);

                List<IconData> iconDatas = new List<IconData>();

                WorldInfo worldInfo = new WorldInfo();
                List<string> inhabitants = new List<string>();
                IconData iconData = new IconData();
                int x = -1;
                int y = -1;
                string filename = "";

                while(reader.Read())
                {
                    if(reader.Name.Equals(""))
                    {
                        continue;
                    }
                    else if(reader.Name.Equals("Inhabitant"))
                    {
                        inhabitants.Add(reader.ReadElementContentAsString());
                    }
                    else if(reader.Name.Equals("Filename"))
                    {
                        filename = reader.ReadElementContentAsString();
                    }
                    else if(reader.Name.Equals("Tileset"))
                    {
                        worldInfo.tileset = reader.ReadElementContentAsString();
                    }
                    else if(reader.Name.Equals("Name"))
                    {
                        worldInfo.name = reader.ReadElementContentAsString();
                    }
                    else if(reader.Name.Equals("Icon"))
                    {
                        while(reader.Read())
                        {
                            if(reader.Name.Equals("Icon") && reader.NodeType == XmlNodeType.EndElement)
                            {
                                break;
                            }
                            else if(reader.Name.Equals("Name"))
                            {
                                iconData.name = reader.ReadElementContentAsString();
                            }
                            else if(reader.Name.Equals("Data"))
                            {
                                iconData.data = reader.ReadElementContentAsString();
                            }
                            else if(reader.Name.Equals("X"))
                            {
                                x = reader.ReadElementContentAsInt();
                            }
                            else if(reader.Name.Equals("Y"))
                            {
                                y = reader.ReadElementContentAsInt();
                            }
                            else if(reader.Name.Equals("Frames"))
                            {
                                iconData.frames = reader.ReadElementContentAsInt();
                            }
                        }
                        iconData.position = new UnityEngine.Vector2Int(x, y);
                        iconDatas.Add(iconData);
                    }
                }

                worldInfo.inhabitants = inhabitants.ToArray();
                s_WorldInfo.Add(worldInfo);

                ObjectIconHandler.instance.AddIcons(
                    filename, 
                    worldInfo.tileset, 
                    iconDatas.ToArray());
            }

            return true;
        }

        public static WorldInfo[] GetWorldInfo(string name)
        {
            try
            {
                return s_WorldInfo.Where(info => info.name.StartsWith(name)).ToArray();
            }
            catch(Exception e)
            {
                ActionLog.WriteToLog("ERROR GETTING WORLD INFO");
                ActionLog.WriteToLog(e.Message);
                ActionLog.WriteToLog(e.StackTrace);
                return new WorldInfo[0];
            }
        }
    }

    public struct WorldInfo
    {
        public string name;
        public string[] inhabitants;
        public string tileset;
    }
}

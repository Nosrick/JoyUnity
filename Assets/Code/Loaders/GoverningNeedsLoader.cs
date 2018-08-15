using JoyLib.Code.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace JoyLib.Code.Loaders
{
    public static class GoverningNeedsLoader
    {
        public static Dictionary<string, List<NeedIndex>> GetGoverningNeeds()
        {
            Dictionary<string, List<NeedIndex>> governingNeeds = new Dictionary<string, List<NeedIndex>>();

            XmlReader reader = XmlReader.Create(Directory.GetCurrentDirectory() + GlobalConstants.DATA_FOLDER + "SkillCoefficients.xml");

            while (reader.Read())
            {
                if (reader.Depth == 0 && reader.NodeType == XmlNodeType.Element && !reader.Name.Equals("Coefficients"))
                    break;

                if (reader.Name.Equals(""))
                    continue;

                if (reader.Name.Equals("Coefficient"))
                {
                    string name = reader.GetAttribute("Skill");
                    reader.MoveToNextAttribute();
                    List<NeedIndex> needs = new List<NeedIndex>();
                    for (int i = 1; i < reader.AttributeCount; i++)
                    {
                        NeedIndex index;
                        reader.MoveToNextAttribute();

                        index = (NeedIndex)Enum.Parse(typeof(NeedIndex), reader.Name);
                        
                        needs.Add(index);
                    }
                    governingNeeds.Add(name, needs);
                }
            }

            reader.Close();

            return governingNeeds;
        }
    }
}

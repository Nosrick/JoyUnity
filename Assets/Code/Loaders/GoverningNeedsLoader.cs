using JoyLib.Code.Entities;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace JoyLib.Code.Loaders
{
    public static class GoverningNeedsLoader
    {
        public static Dictionary<string, List<string>> GetGoverningNeeds()
        {
            Dictionary<string, List<string>> governingNeeds = new Dictionary<string, List<string>>();

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
                    List<string> needs = new List<string>();
                    for (int i = 1; i < reader.AttributeCount; i++)
                    {
                        reader.MoveToNextAttribute();
                        
                        needs.Add(reader.Name);
                    }
                    governingNeeds.Add(name, needs);
                }
            }

            reader.Close();

            return governingNeeds;
        }
    }
}

using JoyLib.Code.Entities;
using System.Collections.Generic;
using System.IO;
using System.Xml;
namespace JoyLib.Code.Loaders
{
    public static class SkillCoefficientLoader
    {
        public static Dictionary<string, Dictionary<string, float>> LoadSkillCoefficients()
        {
            Dictionary<string, Dictionary<string, float>> skillCoefficients = new Dictionary<string, Dictionary<string, float>>();

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
                    Dictionary<string, float> coefficients = new Dictionary<string, float>();
                    for (int i = 1; i < reader.AttributeCount; i++)
                    {
                        reader.MoveToNextAttribute();

                        float coefficient = 0;
                        float.TryParse(reader.GetAttribute(i), out coefficient);
                        coefficients.Add(reader.Name, coefficient);
                    }

                    skillCoefficients.Add(name, coefficients);
                }
            }

            reader.Close();

            return skillCoefficients;
        }
    }
}

using JoyLib.Code.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
namespace JoyLib.Code.Loaders
{
    public static class SkillCoefficientLoader
    {
        public static Dictionary<string, Dictionary<NeedIndex, float>> LoadSkillCoefficients()
        {
            Dictionary<string, Dictionary<NeedIndex, float>> skillCoefficients = new Dictionary<string, Dictionary<NeedIndex, float>>();

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
                    Dictionary<NeedIndex, float> coefficients = new Dictionary<NeedIndex, float>();
                    for (int i = 1; i < reader.AttributeCount; i++)
                    {
                        NeedIndex index;
                        reader.MoveToNextAttribute();
                        index = (NeedIndex)Enum.Parse(typeof(NeedIndex), reader.Name);

                        float coefficient = 0;
                        float.TryParse(reader.GetAttribute(i), out coefficient);
                        coefficients.Add(index, coefficient);
                    }

                    skillCoefficients.Add(name, coefficients);
                }
            }

            reader.Close();

            return skillCoefficients;
        }
    }
}

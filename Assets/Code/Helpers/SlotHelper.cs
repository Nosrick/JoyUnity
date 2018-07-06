using JoyLib.Code.Entities.Items;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace JoyLib.Code.Helpers
{
    public static class SlotHelper
    {
        private static Dictionary<string, List<string>> m_Slots = new Dictionary<string, List<string>>();

        public static void Initialise()
        {
            if (m_Slots.Count == 0)
            {
                XmlReader reader = XmlReader.Create(Directory.GetCurrentDirectory() + "//Data//Slots.xml");

                string name = "DEFAULT ENTITY";
                List<string> slots = new List<string>();

                while (reader.Read())
                {

                    if (reader.Depth == 0 && reader.NodeType == XmlNodeType.Element && !reader.Name.Equals("Slots"))
                        break;

                    if (reader.Depth == 1 && reader.NodeType == XmlNodeType.Element && !reader.Name.Equals("EntityType"))
                        continue;

                    if (reader.Name.Equals(""))
                        continue;

                    if (reader.Name.Equals("Name"))
                    {
                        name = reader.ReadElementContentAsString();
                    }
                    else if (reader.Name.Equals("Slot"))
                    {
                        slots.Add(reader.ReadElementContentAsString());
                    }
                    else if (reader.Name.Equals("EntityType") && reader.NodeType == XmlNodeType.EndElement)
                    {
                        m_Slots.Add(name, slots);
                        name = "DEFAULT ENTITY";
                        slots = new List<string>();
                    }
                }

                reader.Close();
            }
        }

        public static Dictionary<string, ItemInstance> GetSlotsForType(string typeRef)
        {
            if (m_Slots.ContainsKey(typeRef))
            {
                Dictionary<string, ItemInstance> returnSlots = new Dictionary<string, ItemInstance>();
                foreach (string slot in m_Slots[typeRef])
                {
                    returnSlots.Add(slot, null);
                }
                return returnSlots;
            }
            return new Dictionary<string, ItemInstance>();
        }
    }
}

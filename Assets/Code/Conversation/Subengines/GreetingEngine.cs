using JoyLib.Code.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace JoyLib.Code.Conversation.Subengines
{
    public static class GreetingEngine
    {
        //Each greeting will have a ConversationCondition associated with it
        private static List<Tuple<string, ConversationCondition>> m_Greetings;

        public static void LoadGreetings()
        {
            if (m_Greetings != null)
                return;

            m_Greetings = new List<Tuple<string, ConversationCondition>>();

            XmlReader reader = XmlReader.Create(Directory.GetCurrentDirectory() + "//Data//Conversation//Greetings//Greetings.xml");

            string greeting = "DEFAULT GREETING";
            ConversationCondition condition = new ConversationCondition("=", 0);

            while (reader.Read())
            {
                if (reader.Depth == 0 && reader.NodeType == XmlNodeType.Element && !reader.Name.Equals("Conversations"))
                    break;

                if (reader.Name.Equals(""))
                    continue;

                if (reader.Name.Equals("Line"))
                {
                    greeting = reader.GetAttribute("Value");

                    string conditionValueString = reader.GetAttribute("Relationship");
                    string conditionString = "=";
                    int indexOfCondition = conditionValueString.IndexOf('>');
                    if (indexOfCondition == -1)
                    {
                        indexOfCondition = conditionValueString.IndexOf('<');
                        if (indexOfCondition != -1)
                        {
                            conditionString = conditionValueString.Substring(indexOfCondition, 1);
                        }
                    }
                    else
                    {
                        conditionString = conditionValueString.Substring(indexOfCondition, 1);
                    }
                    conditionValueString = conditionValueString.Replace(conditionString, "");
                    int conditionValue = 0;
                    int.TryParse(conditionValueString, out conditionValue);

                    condition = new ConversationCondition(conditionString, conditionValue);

                    m_Greetings.Add(new Tuple<string, ConversationCondition>(greeting, condition));
                }
            }

            reader.Close();
        }

        public static string Converse(Entity instigator, Entity listener)
        {
            List<Tuple<string, ConversationCondition>> validResponses = new List<Tuple<string, ConversationCondition>>();
            foreach (Tuple<string, ConversationCondition> tuple in m_Greetings)
            {
                if (tuple.Second.FulfillsCondition(listener.HasRelationship(instigator.GUID)))
                {
                    validResponses.Add(tuple);
                }
            }
            int index = 0;
            int highestValue = 0;
            for(int i = 0; i < validResponses.Count; i++)
            {
                if (validResponses[i].Second.value > highestValue)
                {
                    highestValue = validResponses[i].Second.value;
                    index = i;
                }
            }
            return validResponses[index].First;
        }
    }
}

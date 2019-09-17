using JoyLib.Code.Conversation.Conversations;
using JoyLib.Code.Entities;
using System;
using System.Collections.Generic;

namespace JoyLib.Code.Conversation.Subengines
{
    public static class GreetingEngine
    {
        //Each greeting will have a ConversationCondition associated with it
        private static List<ITopic> s_Greetings;

        public static bool LoadGreetings()
        {
            if (s_Greetings != null)
            {
                return true;
            }

            s_Greetings = new List<ITopic>();

            /*
            XmlReader reader = XmlReader.Create(Directory.GetCurrentDirectory() + "//Data//Conversation//Greetings//Greetings.xml");

            string greeting = "DEFAULT GREETING";

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

                    s_Greetings.Add(new Tuple<string, ConversationCondition>(greeting, condition));
                }
            }

            reader.Close();
            */

            return true;
        }

        public static ITopic Converse(Entity instigator, Entity listener)
        {
            List<ITopic> validResponses = new List<ITopic>();
            foreach (ITopic topic in s_Greetings)
            {
                string[] conditions = topic.GetConditionTags();
                Tuple<string, int>[] data = instigator.GetData(conditions);

                if (topic.PassesConditions(data))
                {
                    validResponses.Add(topic);
                }
            }

            //Sort by priority
            validResponses.Sort(new TopicComparer());
            return validResponses[0];
        }
    }
}

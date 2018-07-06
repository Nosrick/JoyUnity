using JoyLib.Code.Conversation.Conversations;
using JoyLib.Code.Conversation.Subengines;
using JoyLib.Code.Entities;
using JoyLib.Code.Helpers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace JoyLib.Code.Conversation
{
    public static class ConversationEngine
    {
        private static List<TopicData> s_Topics;
        private static List<TopicData> s_CurrentTopics;
        private static List<TopicData> s_PreviousTopics;
        private static Dictionary<int, BaseConversation> s_KeywordScripts;
        
        public static List<TopicData> Converse(Entity instigator, Entity listener, int selectedItem = 0)
        {
            instigator.FulfillNeed(NeedIndex.Friendship, listener.Statistics[StatisticIndex.Personality], 0);
            listener.FulfillNeed(NeedIndex.Friendship, instigator.Statistics[StatisticIndex.Personality], 0);
            instigator.InfluenceMe(listener.GUID, listener.Statistics[StatisticIndex.Personality]);
            listener.InfluenceMe(instigator.GUID, instigator.Statistics[StatisticIndex.Personality]);

            if (instigator.Family.ContainsKey(listener.GUID))
            {
                instigator.FulfillNeed(NeedIndex.Family, listener.Statistics[StatisticIndex.Personality], 0);
                listener.FulfillNeed(NeedIndex.Family, instigator.Statistics[StatisticIndex.Personality], 0);
            }

            //If selectedItem == 0, the conversation is just starting
            if (selectedItem == 0)
            {
                s_PreviousTopics = new List<TopicData>();

                string value = GreetingEngine.Converse(instigator, listener);
                int nextTopic = 1;

                List<TopicData> data = new List<TopicData>();
                data.Add(new TopicData(value, 0, nextTopic));
                s_CurrentTopics = data;
                return data;
            }
            else
            {
                s_CurrentTopics = s_Topics.Where(x => x.ID == selectedItem).ToList();

                s_CurrentTopics = TrimEmpty(s_CurrentTopics);

                for (int i = 0; i < s_CurrentTopics.Count; i++)
                {
                    if(s_KeywordScripts.ContainsKey(s_CurrentTopics[i].ID))
                    {
                        string rawReply = s_KeywordScripts[s_CurrentTopics[i].ID].Interact(instigator, listener);

                        if (rawReply == "")
                        {
                            s_CurrentTopics[i] = new TopicData(rawReply, 0, 0);
                            continue;
                        }

                        TopicData newData = new TopicData(rawReply[0] == 't' ? rawReply.Substring(1, rawReply.Length - 1) : rawReply,
                            s_CurrentTopics[i].ID, s_CurrentTopics[i].nextTopic, rawReply[0] == 't' ? true : false);
                        s_CurrentTopics[i] = newData;
                    }
                }
                s_CurrentTopics = s_CurrentTopics.Where(x => x.value != "").ToList();

                s_PreviousTopics = s_CurrentTopics;
                return s_CurrentTopics;
            }
        }

        private static List<TopicData> TrimEmpty(List<TopicData> topics)
        {
            List<TopicData> newTopics = new List<TopicData>(topics.Count);

            for(int i = 0; i < topics.Count; i++)
            {
                if(topics[i].value != "")
                {
                    newTopics.Add(topics[i]);
                }
            }

            return newTopics;
        }

        public static List<TopicData> Topics
        {
            get
            {
                return new List<TopicData>(s_CurrentTopics);
            }
        }
    }
}

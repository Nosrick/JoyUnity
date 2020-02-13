using System;
using System.Collections.Generic;
using System.Linq;

namespace JoyLib.Code.Entities.Relationships
{
    public class EntityRelationshipHandler
    {
        private static readonly Lazy<EntityRelationshipHandler> lazy = new Lazy<EntityRelationshipHandler>(() => new EntityRelationshipHandler());

        public static EntityRelationshipHandler instance => lazy.Value;

        private Dictionary<string, Type> m_RelationshipTypes;
        private Dictionary<string, IRelationship> m_Relationships;

        public EntityRelationshipHandler()
        {
            m_RelationshipTypes = new Dictionary<string, Type>();
            m_Relationships = new Dictionary<string, IRelationship>();
        }

        public bool AddRelationshipType(Type relationshipType)
        {
            if(relationshipType.IsAssignableFrom(typeof(IRelationship)))
            {
                IRelationship relationship = (IRelationship)Activator.CreateInstance(relationshipType);

                if (m_RelationshipTypes.ContainsKey(relationship.Name) == false)
                {
                    m_RelationshipTypes.Add(relationship.Name, relationshipType);
                    return true;
                }

            }
            return false;
        }

        public IRelationship CreateRelationship(Entity[] participants, string type = "Friendship")
        {
            if(m_RelationshipTypes.ContainsKey(type))
            {
                IRelationship newRelationship = (IRelationship)Activator.CreateInstance(m_RelationshipTypes[type]);
                foreach(Entity participant in participants)
                {
                    newRelationship.AddParticipant(participant);
                }

                m_Relationships.Add(newRelationship.GenerateHash(), newRelationship);
                return newRelationship;
            }
            return null;
        }

        public List<IRelationship> Get(long[] participants, string[] tags = null)
        {
            string hash = GenerateHash(participants);

            List<IRelationship> relationships = new List<IRelationship>();
            
            float bestPercentage = 0.0f;
            IRelationship bestRelationship = null;
            foreach(KeyValuePair<string, IRelationship> pair in m_Relationships)
            {
                Tuple<int, int> match = CompareHash(hash, pair.Key);
                float percentage = ((float)match.Item1 / match.Item2);
                bool tagsMatch = false;
                if (tags != null)
                {
                    float tagsPercentage = 0.0f;
                    int totalTags = 0;
                    string[] relationshipTags = pair.Value.GetTags();
                    foreach (string tag in tags)
                    {
                        if (relationshipTags.Contains(tag))
                        {
                            totalTags += 1;
                        }
                    }

                    tagsPercentage = ((float)totalTags / tags.Length);
                    tagsMatch = true;
                }
                if(percentage >= bestPercentage && (tagsMatch == true && tags != null))
                {
                    bestPercentage = percentage;
                    bestRelationship = pair.Value;
                }

                if((int)percentage == 100)
                {
                    relationships.Add(pair.Value);
                    percentage = 0.0f;
                }
            }

            if((int)bestPercentage != 100)
            {
                if(bestRelationship != null)
                {
                    relationships.Add(bestRelationship);
                }
            }

            return relationships;
        }

        public int GetHighestRelationshipValue(JoyObject speaker, JoyObject listener, string[] tags = null)
        {
            long[] participants = new long[] { speaker.GUID, listener.GUID };
            List<IRelationship> relationships = Get(participants, tags);

            int highestValue = int.MinValue;
            foreach(IRelationship relationship in relationships)
            {
                int value = relationship.GetRelationshipValue(speaker.GUID, listener.GUID);
                if (value > highestValue)
                {
                    highestValue = value;
                }
            }

            return highestValue;
        }

        private string GenerateHash(long[] participants)
        {
            string hash = "";
            List<long> sortedList = new List<long>(participants);
            sortedList.Sort();
            foreach(long GUID in sortedList)
            {
                hash += GUID + ":";
            }
            hash = hash.Substring(0, hash.Length - 1);
            return hash;
        }

        private Tuple<int, int> CompareHash(string left, string right)
        {
            string[] leftStrings = left.Split(':');
            string[] rightStrings = right.Split(':');

            long[] leftGUIDs = new long[leftStrings.Length];
            long[] rightGUIDs = new long[rightStrings.Length];

            for(int i = 0; i < leftStrings.Length; i++)
            {
                leftGUIDs[i] = long.Parse(leftStrings[i]);
            }

            for(int i = 0; i < rightStrings.Length; i++)
            {
                rightGUIDs[i] = long.Parse(rightStrings[i]);
            }

            int maximum = leftGUIDs.Length;
            if(rightGUIDs.Length > maximum)
            {
                maximum = rightGUIDs.Length;
            }

            long[] needleGUIDs;
            long[] haystackGUIDs;
            if(leftGUIDs.Length >= rightGUIDs.Length)
            {
                needleGUIDs = leftGUIDs;
                haystackGUIDs = rightGUIDs;
            }
            else
            {
                needleGUIDs = rightGUIDs;
                haystackGUIDs = leftGUIDs;
            }

            int matches = 0;
            foreach(long needle in needleGUIDs)
            {
                if(haystackGUIDs.Contains(needle))
                {
                    matches += 1;
                }
            }

            return new Tuple<int, int>(matches, maximum);
        }
    }
}

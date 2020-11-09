using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace JoyLib.Code.Entities.Relationships
{
    public abstract class AbstractRelationship : IRelationship
    {
        //Yeesh, this is messy
        //But this is a key value pair for how each participant feels about the other in the relationship
        protected SortedDictionary<long, Dictionary<long, int>> m_Values;
        protected SortedDictionary<long, IJoyObject> m_Participants;
        protected List<string> m_Tags;

        public AbstractRelationship()
        {
            m_Participants = new SortedDictionary<long, IJoyObject>();
            m_Values = new SortedDictionary<long, Dictionary<long, int>>();
            m_Tags = new List<string>();
        }

        public virtual bool AddParticipant(IJoyObject newParticipant)
        {
            if(m_Participants.ContainsKey(newParticipant.GUID) == false)
            {
                m_Participants.Add(newParticipant.GUID, newParticipant);

                m_Values.Add(newParticipant.GUID, new Dictionary<long, int>());

                List<long> participantGUIDs = m_Participants.Keys.ToList();

                foreach(KeyValuePair<long, Dictionary<long, int>> pair in m_Values)
                {
                    if(pair.Key == newParticipant.GUID)
                    {
                        foreach(long guid in participantGUIDs)
                        {
                            if(guid == newParticipant.GUID)
                            {
                                continue;
                            }
                            m_Values[newParticipant.GUID].Add(guid, 0);
                        }
                    }
                    else
                    {
                        m_Values[pair.Key].Add(newParticipant.GUID, 0);
                    }
                }

                return true;
            }
            return false;
        }

        public bool AddTag(string tag)
        {
            if(m_Tags.Contains(tag) == false)
            {
                m_Tags.Add(tag);
                return true;
            }
            return false;
        }

        public IJoyObject GetParticipant(long GUID)
        {
            if(m_Participants.ContainsKey(GUID))
            {
                return m_Participants[GUID];
            }
            return null;
        }

        public IJoyObject[] GetParticipants()
        {
            return m_Participants.Values.ToArray();
        }

        public int GetHighestRelationshipValue(long GUID)
        {
            return m_Values.Where(pair => pair.Key.Equals(GUID))
                .Max(pair => pair.Value.Max(valuePair => valuePair.Value));
        }

        public string[] GetTags()
        {
            return m_Tags.ToArray();
        }

        public Dictionary<long, int> GetValuesOfParticipant(long GUID)
        {
            if(m_Values.ContainsKey(GUID))
            {
                return m_Values[GUID];
            }
            return null;
        }

        public int ModifyValueOfParticipant(long actor, long observer, int value)
        {
            if(m_Values.ContainsKey(observer))
            {
                if(m_Values[observer].ContainsKey(actor))
                {
                    m_Values[observer][actor] += value;
                    return m_Values[observer][actor];
                }
            }
            return 0;
        }

        public int ModifyValueOfOtherParticipants(long actor, int value)
        {
            List<long> participantKeys = m_Values.Keys.ToList();
            foreach (long guid in participantKeys)
            {
                if (guid != actor)
                {
                    m_Values[guid][actor] += value;
                }
            }

            return value;
        }

        public int ModifyValueOfAllParticipants(int value)
        {
            List<long> participantKeys = m_Values.Keys.ToList();
            foreach(long guid in participantKeys)
            {
                List<long> involvedKeys = m_Values[guid].Keys.ToList();

                foreach(long involvedGUID in involvedKeys)
                {
                    m_Values[guid][involvedGUID] += value;
                }
            }

            return value;
        }

        public bool RemoveParticipant(long currentGUID)
        {
            if(m_Participants.ContainsKey(currentGUID))
            {
                m_Participants.Remove(currentGUID);
                m_Values.Remove(currentGUID);
                foreach (Dictionary<long, int> relationship in m_Values.Values)
                {
                    if(relationship.ContainsKey(currentGUID))
                    {
                        relationship.Remove(currentGUID);
                    }
                }
                return true;
            }
            return false;
        }

        public bool RemoveTag(string tag)
        {
            if(m_Tags.Contains(tag))
            {
                m_Tags.Remove(tag);
                return true;
            }
            return false;
        }

        public long GenerateHash(long[] participants)
        {
            long hash = 0;

            long s1 = 1;
            long s2 = 0;

            int hashMagic = 65521;

            List<long> sortedList = new List<long>(participants);
            sortedList.Sort();
            foreach(long GUID in sortedList)
            {
                s1 = (s1 + GUID) % hashMagic;
                s2 = (s2 + s1) % hashMagic;
            }
            hash = (s2 << 16) | s1;
            return hash;
        }

        public int GetRelationshipValue(long left, long right)
        {
            if(m_Values.ContainsKey(left))
            {
                if(m_Values[left].ContainsKey(right))
                {
                    return m_Values[left][right];
                }
            }
            return 0;
        }

        public abstract IRelationship Create(IEnumerable<IJoyObject> participants);
        public abstract IRelationship CreateWithValue(IEnumerable<IJoyObject> participants, int value);

        public virtual string Name => "abstractrelationship";

        public ReadOnlyCollection<string> Tags => m_Tags.AsReadOnly();

        public virtual string DisplayName => "SOMEONE FORGOT TO OVERRIDE THE DISPLAYNAME";
    }
}

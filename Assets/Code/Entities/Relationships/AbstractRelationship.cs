using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace JoyLib.Code.Entities.Relationships
{
    public abstract class AbstractRelationship : IRelationship
    {

        public virtual string Name => "abstractrelationship";

        public List<string> Tags { get; protected set; }

        public virtual string DisplayName => "SOMEONE FORGOT TO OVERRIDE THE DISPLAYNAME";
        
        //Yeesh, this is messy
        //But this is a key value pair for how each participant feels about the other in the relationship
        protected SortedDictionary<long, Dictionary<long, int>> m_Values;
        protected SortedDictionary<long, IJoyObject> m_Participants;

        public AbstractRelationship()
        {
            m_Participants = new SortedDictionary<long, IJoyObject>();
            m_Values = new SortedDictionary<long, Dictionary<long, int>>();
            Tags = new List<string>();
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
                            m_Values[newParticipant.GUID].Add(guid, 0);
                        }
                    }
                    else
                    {
                        m_Values[pair.Key].Add(newParticipant.GUID, 0);
                    }
                }
                
                if (newParticipant is Entity entity)
                {
                    foreach (string tag in entity.Sexuality.Tags)
                    {
                        AddTag(tag);
                    }
                }

                return true;
            }
            return false;
        }
        
        public bool HasTag(string tag)
        {
            return Tags.Any(t => t.Equals(tag, StringComparison.OrdinalIgnoreCase));
        }

        public bool AddTag(string tag)
        {
            if (HasTag(tag))
            {
                return false;
            }
            Tags.Add(tag);
            return true;
        }

        public bool RemoveTag(string tag)
        {
            if (HasTag(tag))
            {
                Tags.Remove(tag);
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
            Debug.Log("GUID IS " + GUID);
            Debug.Log("PARTICIPANT GUIDS:");
            foreach (long value in this.m_Participants.Keys)
            {
                Debug.Log(value);
            }
            
            return m_Values.Where(pair => pair.Key.Equals(GUID))
                .Max(pair => pair.Value.Max(valuePair => valuePair.Value));
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
                if (m_Values[guid].Keys.Count == 0)
                {
                    foreach (long participant in m_Participants.Keys)
                    {
                        if (guid != participant)
                        {
                            m_Values[guid].Add(participant, 0);
                        }
                    }
                }
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

        public long GenerateHash(IEnumerable<long> participants)
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
    }
}

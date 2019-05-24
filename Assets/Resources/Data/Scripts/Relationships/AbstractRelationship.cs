﻿using System.Collections.Generic;
using System.Linq;

namespace JoyLib.Code.Entities.Relationships
{
    public abstract class AbstractRelationship : IRelationship
    {
        //Yeesh, this is messy
        //But this is a key value pair for how each participant feels about the other in the relationship
        protected SortedDictionary<long, Dictionary<long, int>> m_Values;
        protected SortedDictionary<long, JoyObject> m_Participants;
        protected List<string> m_Tags;

        public AbstractRelationship()
        {
            m_Participants = new SortedDictionary<long, JoyObject>();
            m_Values = new SortedDictionary<long, Dictionary<long, int>>();
            m_Tags = new List<string>();
        }

        public virtual bool AddParticipant(JoyObject newParticipant)
        {
            if(m_Participants.ContainsKey(newParticipant.GUID) == false)
            {
                m_Participants.Add(newParticipant.GUID, newParticipant);
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

        public JoyObject GetParticipant(long GUID)
        {
            if(m_Participants.ContainsKey(GUID))
            {
                return m_Participants[GUID];
            }
            return null;
        }

        public JoyObject[] GetParticipants()
        {
            return m_Participants.Values.ToArray();
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

        public string GenerateHash()
        {
            string hash = "";
            foreach(long GUID in m_Participants.Keys)
            {
                hash += GUID + ":";
            }
            hash = hash.Substring(0, hash.Length - 1);
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

        public virtual string Name
        {
            get
            {
                return "AbstractRelationship";
            }
        }
    }
}

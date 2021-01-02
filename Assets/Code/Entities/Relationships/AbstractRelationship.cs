using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JoyLib.Code.Entities.Relationships
{
    public abstract class AbstractRelationship : IRelationship
    {
        protected List<string> m_Tags;
        
        public virtual string Name => "abstractrelationship";

        public IEnumerable<string> Tags
        {
            get => this.m_Tags;
            protected set => this.m_Tags = new List<string>(value);
        }

        public virtual string DisplayName => "SOMEONE FORGOT TO OVERRIDE THE DISPLAYNAME";
        
        //Yeesh, this is messy
        //But this is a key value pair for how each participant feels about the other in the relationship
        protected SortedDictionary<long, Dictionary<long, int>> m_Values;
        protected SortedDictionary<long, IJoyObject> m_Participants;

        public AbstractRelationship()
        {
            this.m_Participants = new SortedDictionary<long, IJoyObject>();
            this.m_Values = new SortedDictionary<long, Dictionary<long, int>>();
            this.Tags = new List<string>();
        }

        public long GenerateHashFromInstance()
        {
            return GenerateHash(this.m_Participants.Keys);
        }

        public virtual bool AddParticipant(IJoyObject newParticipant)
        {
            if(this.m_Participants.ContainsKey(newParticipant.GUID) == false)
            {
                this.m_Participants.Add(newParticipant.GUID, newParticipant);

                this.m_Values.Add(newParticipant.GUID, new Dictionary<long, int>());

                List<long> participantGUIDs = this.m_Participants.Keys.ToList();

                foreach(KeyValuePair<long, Dictionary<long, int>> pair in this.m_Values)
                {
                    if(pair.Key == newParticipant.GUID)
                    {
                        foreach(long guid in participantGUIDs)
                        {
                            this.m_Values[newParticipant.GUID].Add(guid, 0);
                        }
                    }
                    else
                    {
                        this.m_Values[pair.Key].Add(newParticipant.GUID, 0);
                    }
                }

                return true;
            }
            return false;
        }

        public bool AddParticipants(IEnumerable<IJoyObject> participants)
        {
            bool result = true;
            foreach (IJoyObject participant in participants)
            {
                result &= this.AddParticipant(participant);
            }

            return result;
        }

        public bool HasTag(string tag)
        {
            return this.m_Tags.Any(t => t.Equals(tag, StringComparison.OrdinalIgnoreCase));
        }

        public bool AddTag(string tag)
        {
            if (this.HasTag(tag))
            {
                return false;
            }
            this.m_Tags.Add(tag);
            return true;
        }

        public bool RemoveTag(string tag)
        {
            if (this.HasTag(tag))
            {
                this.m_Tags.Remove(tag);
                return true;
            }

            return false;
        }

        public IJoyObject GetParticipant(long GUID)
        {
            if(this.m_Participants.ContainsKey(GUID))
            {
                return this.m_Participants[GUID];
            }
            return null;
        }

        public IJoyObject[] GetParticipants()
        {
            return this.m_Participants.Values.ToArray();
        }

        public int GetHighestRelationshipValue(long GUID)
        {
            Debug.Log("GUID IS " + GUID);
            Debug.Log("PARTICIPANT GUIDS:");
            foreach (long value in this.m_Participants.Keys)
            {
                Debug.Log(value);
            }
            
            return this.m_Values.Where(pair => pair.Key.Equals(GUID))
                .Max(pair => pair.Value.Max(valuePair => valuePair.Value));
        }

        public Dictionary<long, int> GetValuesOfParticipant(long GUID)
        {
            if(this.m_Values.ContainsKey(GUID))
            {
                return this.m_Values[GUID];
            }
            return null;
        }

        public int ModifyValueOfParticipant(long actor, long observer, int value)
        {
            if(this.m_Values.ContainsKey(observer))
            {
                if(this.m_Values[observer].ContainsKey(actor))
                {
                    this.m_Values[observer][actor] += value;
                    return this.m_Values[observer][actor];
                }
            }
            return 0;
        }

        public int ModifyValueOfOtherParticipants(long actor, int value)
        {
            List<long> participantKeys = this.m_Values.Keys.ToList();
            foreach (long guid in participantKeys)
            {
                if (guid != actor)
                {
                    this.m_Values[guid][actor] += value;
                }
            }

            return value;
        }

        public int ModifyValueOfAllParticipants(int value)
        {
            List<long> participantKeys = this.m_Values.Keys.ToList();
            foreach(long guid in participantKeys)
            {
                if (this.m_Values[guid].Keys.Count == 0)
                {
                    foreach (long participant in this.m_Participants.Keys)
                    {
                        if (guid != participant)
                        {
                            this.m_Values[guid].Add(participant, 0);
                        }
                    }
                }
                List<long> involvedKeys = this.m_Values[guid].Keys.ToList();
                
                foreach(long involvedGUID in involvedKeys)
                {
                    this.m_Values[guid][involvedGUID] += value;
                }
            }

            return value;
        }

        public bool RemoveParticipant(long currentGUID)
        {
            if(this.m_Participants.ContainsKey(currentGUID))
            {
                this.m_Participants.Remove(currentGUID);
                this.m_Values.Remove(currentGUID);
                foreach (Dictionary<long, int> relationship in this.m_Values.Values)
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

        public static long GenerateHash(IEnumerable<long> participants)
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
            if(this.m_Values.ContainsKey(left))
            {
                if(this.m_Values[left].ContainsKey(right))
                {
                    return this.m_Values[left][right];
                }
            }
            return 0;
        }

        public abstract IRelationship Create(IEnumerable<IJoyObject> participants);
        public abstract IRelationship CreateWithValue(IEnumerable<IJoyObject> participants, int value);
    }
}

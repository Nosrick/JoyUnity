using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinSerializer;

namespace JoyLib.Code.Entities.Relationships
{
    [Serializable]
    public abstract class AbstractRelationship : IRelationship
    {
        [OdinSerialize]
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
        [OdinSerialize]
        protected SortedDictionary<Guid, Dictionary<Guid, int>> m_Values;
        [OdinSerialize]
        protected List<Guid> m_Participants;

        public AbstractRelationship(ILiveEntityHandler entityHandler = null)
        {
            this.m_Participants = new List<Guid>();
            this.m_Values = new SortedDictionary<Guid, Dictionary<Guid, int>>();
            this.Tags = new List<string>();
        }

        public long GenerateHashFromInstance()
        {
            return GenerateHash(this.m_Participants);
        }
        

        public virtual bool AddParticipant(IJoyObject newParticipant)
        {
            if(this.m_Participants.Contains(newParticipant.Guid) == false)
            {
                this.m_Participants.Add(newParticipant.Guid);

                this.m_Values.Add(newParticipant.Guid, new Dictionary<Guid, int>());

                foreach(KeyValuePair<Guid, Dictionary<Guid, int>> pair in this.m_Values)
                {
                    if(pair.Key == newParticipant.Guid)
                    {
                        foreach(Guid guid in this.m_Participants)
                        {
                            this.m_Values[newParticipant.Guid].Add(guid, 0);
                        }
                    }
                    else
                    {
                        this.m_Values[pair.Key].Add(newParticipant.Guid, 0);
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

        public IJoyObject GetParticipant(Guid GUID)
        {
            return this.m_Participants.Contains(GUID) 
                ? GlobalConstants.GameManager.EntityHandler.Get(GUID) 
                : null;
        }

        public IEnumerable<IJoyObject> GetParticipants()
        {
            List<IEntity> participants = new List<IEntity>();
            foreach (Guid participant in this.m_Participants)
            {
                participants.Add(GlobalConstants.GameManager.EntityHandler.Get(participant));
            }
            return participants;
        }

        public int ModifyValueOfParticipant(Guid actor, Guid observer, int value)
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

        public int GetHighestRelationshipValue(Guid GUID)
        {
            return this.m_Values.Where(pair => pair.Key.Equals(GUID))
                .Max(pair => pair.Value.Max(valuePair => valuePair.Value));
        }

        public Dictionary<Guid, int> GetValuesOfParticipant(Guid GUID)
        {
            if(this.m_Values.ContainsKey(GUID))
            {
                return this.m_Values[GUID];
            }
            return null;
        }

        public int ModifyValueOfOtherParticipants(Guid actor, int value)
        {
            List<Guid> participantKeys = this.m_Values.Keys.ToList();
            foreach (Guid guid in participantKeys)
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
            List<Guid> participantKeys = this.m_Values.Keys.ToList();
            foreach(Guid guid in participantKeys)
            {
                if (this.m_Values[guid].Keys.Count == 0)
                {
                    foreach (Guid participant in this.m_Participants)
                    {
                        if (guid != participant)
                        {
                            this.m_Values[guid].Add(participant, 0);
                        }
                    }
                }
                List<Guid> involvedKeys = this.m_Values[guid].Keys.ToList();
                
                foreach(Guid involvedGUID in involvedKeys)
                {
                    this.m_Values[guid][involvedGUID] += value;
                }
            }

            return value;
        }

        public bool RemoveParticipant(Guid currentGUID)
        {
            if(this.m_Participants.Contains(currentGUID))
            {
                this.m_Participants.Remove(currentGUID);
                this.m_Values.Remove(currentGUID);
                foreach (Dictionary<Guid, int> relationship in this.m_Values.Values)
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

        public static long GenerateHash(IEnumerable<Guid> participants)
        {
            long hash = 0;

            long s1 = 1;
            long s2 = 0;

            int hashMagic = 65521;

            List<Guid> sortedList = new List<Guid>(participants);
            sortedList.Sort();
            foreach(Guid GUID in sortedList)
            {
                s1 = (s1 + GUID.GetHashCode()) % hashMagic;
                s2 = (s2 + s1) % hashMagic;
            }
            hash = (s2 << 16) | s1;
            return hash;
        }

        public int GetRelationshipValue(Guid left, Guid right)
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

using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Core.Internal;
using JoyLib.Code.Collections;
using JoyLib.Code.Scripting;

namespace JoyLib.Code.Entities.Relationships
{
    public class EntityRelationshipHandler : IEntityRelationshipHandler
    {
        protected Dictionary<string, IRelationship> m_RelationshipTypes;
        protected NonUniqueDictionary<long, IRelationship> m_Relationships;

        public IEnumerable<IRelationship> AllRelationships => this.m_Relationships.Values;

        public EntityRelationshipHandler()
        {
            this.Initialise();
        }

        public bool Initialise()
        {
            this.m_RelationshipTypes = new Dictionary<string, IRelationship>();
            this.m_Relationships = new NonUniqueDictionary<long, IRelationship>();

            IEnumerable<IRelationship> types = ScriptingEngine.Instance.FetchAndInitialiseChildren<IRelationship>();
            foreach(IRelationship type in types)
            {
                this.m_RelationshipTypes.Add(type.Name, type);
            }

            return true;
        }

        public bool AddRelationship(IRelationship relationship)
        {
            this.m_Relationships.Add(relationship.GenerateHashFromInstance(), relationship);
            return true;
        }

        public bool RemoveRelationship(long ID)
        {
            return this.m_Relationships.RemoveByKey(ID) > 0;
        }

        public IRelationship CreateRelationship(IEnumerable<IJoyObject> participants, IEnumerable<string> tags)
        {
            if(this.m_RelationshipTypes.Any(t => tags.Any(tag => tag.Equals(t.Key, StringComparison.OrdinalIgnoreCase))))
            {
                IRelationship newRelationship = this.m_RelationshipTypes
                    .First(t => tags.Any(tag => tag.Equals(t.Key, StringComparison.OrdinalIgnoreCase))).Value
                    .Create(participants);
                
                List<long> GUIDs = new List<long>();
                foreach(IJoyObject participant in participants)
                {
                    GUIDs.Add(participant.GUID);
                }

                newRelationship.ModifyValueOfAllParticipants(0);

                this.m_Relationships.Add(newRelationship.GenerateHashFromInstance(), newRelationship);
                return newRelationship;
            }

            throw new InvalidOperationException("Relationship type " + tags + " not found.");
        }

        public IRelationship CreateRelationshipWithValue(IEnumerable<IJoyObject> participants, IEnumerable<string> tags,
            int value)
        {
            IRelationship relationship = this.CreateRelationship(participants, tags);
            relationship.ModifyValueOfAllParticipants(value);

            return relationship;
        }

        public IEnumerable<IRelationship> Get(IEnumerable<IJoyObject> participants, IEnumerable<string> tags = null,
            bool createNewIfNone = false)
        {
            IEnumerable<long> GUIDs = participants.Select(p => p.GUID);
            long hash = AbstractRelationship.GenerateHash(GUIDs);

            List<IRelationship> relationships = new List<IRelationship>();
            
            foreach(Tuple<long, IRelationship> pair in this.m_Relationships)
            {
                if(pair.Item1 != hash)
                {
                    continue;
                }
                
                if (tags.IsNullOrEmpty() == false && tags.Intersect(pair.Item2.Tags).Any())
                {
                    relationships.Add(pair.Item2);
                }
                else if (tags.IsNullOrEmpty())
                {
                    relationships.Add(pair.Item2);
                }
            }

            if (relationships.Count == 0 && createNewIfNone)
            {
                List<string> newTags = new List<string>(tags);
                if (newTags.IsNullOrEmpty())
                {
                    newTags.Add("friendship");
                }
                relationships.Add(this.CreateRelationship(participants, newTags));
            }

            return relationships.ToArray();
        }

        public int GetHighestRelationshipValue(IJoyObject speaker, IJoyObject listener, IEnumerable<string> tags = null)
        {
            try
            {
                return this.GetBestRelationship(speaker, listener, tags)
                    .GetRelationshipValue(speaker.GUID, listener.GUID);
            }
            catch (Exception e)
            {
                GlobalConstants.ActionLog.AddText("No relationship found for " + speaker + " and " + listener + ".");
                GlobalConstants.ActionLog.AddText("No relationship found for " + speaker + " and " + listener + ".");
                GlobalConstants.ActionLog.AddText(e.Message);
                GlobalConstants.ActionLog.AddText(e.StackTrace);
                return 0;
            }
        }

        public IRelationship GetBestRelationship(IJoyObject speaker, IJoyObject listener,
            IEnumerable<string> tags = null)
        {
            IJoyObject[] participants = {speaker, listener};
            IEnumerable<IRelationship> relationships = this.Get(participants, tags, false);

            int highestValue = int.MinValue;
            IRelationship bestMatch = null;
            foreach (IRelationship relationship in relationships)
            {
                int value = relationship.GetRelationshipValue(speaker.GUID, listener.GUID);
                if (value > highestValue)
                {
                    highestValue = value;
                    bestMatch = relationship;
                }
            }

            if (bestMatch is null)
            {
                throw new InvalidOperationException("No relationship between " + speaker.JoyName + " and " + listener.JoyName + ".");
            }

            return bestMatch;
        }

        public IEnumerable<IRelationship> GetAllForObject(IJoyObject actor)
        {
            return this.m_Relationships.Where(tuple => tuple.Item2.GetParticipant(actor.GUID) is null == false)
                .Select(tuple => tuple.Item2)
                .ToArray();
        }

        public bool IsFamily(IJoyObject speaker, IJoyObject listener)
        {
            IJoyObject[] participants = { speaker, listener };
            IEnumerable<IRelationship> relationships = this.Get(participants, new[] {"family"});

            return relationships.Any();
        }

        public NonUniqueDictionary<long, IRelationship> Relationships
        {
            get
            {
                if(this.m_Relationships is null)
                {
                    this.Initialise();
                }
                
                return this.m_Relationships;
            }
        }

        public List<IRelationship> RelationshipTypes => this.m_RelationshipTypes.Values.ToList();
    }
}

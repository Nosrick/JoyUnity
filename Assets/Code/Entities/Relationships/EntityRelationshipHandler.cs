using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Core.Internal;
using JoyLib.Code.Collections;
using JoyLib.Code.Helpers;
using JoyLib.Code.Scripting;
using UnityEngine;

namespace JoyLib.Code.Entities.Relationships
{
    public class EntityRelationshipHandler : IEntityRelationshipHandler
    {
        private Dictionary<string, IRelationship> m_RelationshipTypes;
        private NonUniqueDictionary<long, IRelationship> m_Relationships;

        public EntityRelationshipHandler()
        {
            Initialise();
        }

        public bool Initialise()
        {
            m_RelationshipTypes = new Dictionary<string, IRelationship>();
            m_Relationships = new NonUniqueDictionary<long, IRelationship>();

            IEnumerable<IRelationship> types = ScriptingEngine.instance.FetchAndInitialiseChildren<IRelationship>();
            foreach(IRelationship type in types)
            {
                m_RelationshipTypes.Add(type.Name, type);
            }

            return true;
        }

        public bool AddRelationship(IRelationship relationship)
        {
            m_Relationships.Add(relationship.GenerateHashFromInstance(), relationship);
            return true;
        }

        public bool RemoveRelationship(long ID)
        {
            return m_Relationships.RemoveByKey(ID) > 0;
        }

        public IRelationship CreateRelationship(IEnumerable<IJoyObject> participants, IEnumerable<string> tags)
        {
            if(m_RelationshipTypes.Any(t => tags.Any(tag => tag.Equals(t.Key, StringComparison.OrdinalIgnoreCase))))
            {
                IRelationship newRelationship = m_RelationshipTypes
                    .First(t => tags.Any(tag => tag.Equals(t.Key, StringComparison.OrdinalIgnoreCase))).Value
                    .Create(participants);
                
                List<long> GUIDs = new List<long>();
                foreach(IJoyObject participant in participants)
                {
                    GUIDs.Add(participant.GUID);
                }

                newRelationship.ModifyValueOfAllParticipants(0);

                m_Relationships.Add(newRelationship.GenerateHashFromInstance(), newRelationship);
                return newRelationship;
            }

            throw new InvalidOperationException("Relationship type " + tags + " not found.");
        }

        public IRelationship CreateRelationshipWithValue(IEnumerable<IJoyObject> participants, IEnumerable<string> tags,
            int value)
        {
            IRelationship relationship = CreateRelationship(participants, tags);
            relationship.ModifyValueOfAllParticipants(value);

            return relationship;
        }

        public IEnumerable<IRelationship> Get(IEnumerable<IJoyObject> participants, IEnumerable<string> tags = null,
            bool createNewIfNone = false)
        {
            IEnumerable<long> GUIDs = participants.Select(p => p.GUID);
            long hash = AbstractRelationship.GenerateHash(GUIDs);

            List<IRelationship> relationships = new List<IRelationship>();
            
            foreach(Tuple<long, IRelationship> pair in m_Relationships)
            {
                if(pair.Item1 != hash)
                {
                    continue;
                }
                
                if (tags != null && tags.Intersect(pair.Item2.Tags).Count() > 0)
                {
                    relationships.Add(pair.Item2);
                }
                else if (tags is null)
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
                relationships.Add(CreateRelationship(participants, newTags));
            }

            return relationships.ToArray();
        }

        public int GetHighestRelationshipValue(IJoyObject speaker, IJoyObject listener, IEnumerable<string> tags = null)
        {
            try
            {
                return GetBestRelationship(speaker, listener, tags)
                    .GetRelationshipValue(speaker.GUID, listener.GUID);
            }
            catch (Exception e)
            {
                ActionLog.instance.AddText("No relationship found for " + speaker + " and " + listener + ".");
                Debug.LogWarning("No relationship found for " + speaker + " and " + listener + ".");
                Debug.LogWarning(e.Message);
                Debug.LogWarning(e.StackTrace);
                return 0;
            }
        }

        public IRelationship GetBestRelationship(IJoyObject speaker, IJoyObject listener,
            IEnumerable<string> tags = null)
        {
            IJoyObject[] participants = {speaker, listener};
            IEnumerable<IRelationship> relationships = Get(participants, tags, false);

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
            return m_Relationships.Where(tuple => tuple.Item2.GetParticipant(actor.GUID) is null == false)
                .Select(tuple => tuple.Item2)
                .ToArray();
        }

        public bool IsFamily(IJoyObject speaker, IJoyObject listener)
        {
            IJoyObject[] participants = { speaker, listener };
            IEnumerable<IRelationship> relationships = Get(participants, new[] {"family"});

            return relationships.Any();
        }

        public NonUniqueDictionary<long, IRelationship> Relationships
        {
            get
            {
                if(m_Relationships is null)
                {
                    Initialise();
                }
                
                return m_Relationships;
            }
        }

        public List<IRelationship> RelationshipTypes => m_RelationshipTypes.Values.ToList();
    }
}

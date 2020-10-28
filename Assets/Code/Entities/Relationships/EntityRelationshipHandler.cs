using JoyLib.Code.Scripting;
using JoyLib.Code.Collections;

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace JoyLib.Code.Entities.Relationships
{
    public class EntityRelationshipHandler : MonoBehaviour
    {
        private Dictionary<string, IRelationship> m_RelationshipTypes;
        private NonUniqueDictionary<long, IRelationship> m_Relationships;

        public void Awake()
        {
            Initialise();
        }

        public bool Initialise()
        {
            m_RelationshipTypes = new Dictionary<string, IRelationship>();
            m_Relationships = new NonUniqueDictionary<long, IRelationship>();

            IRelationship[] types = ScriptingEngine.instance.FetchAndInitialiseChildren<IRelationship>();
            foreach(IRelationship type in types)
            {
                m_RelationshipTypes.Add(type.Name, type);
            }

            return true;
        }

        public IRelationship CreateRelationship(Entity[] participants, string type = "friendship")
        {
            if(m_RelationshipTypes.Any(t => t.Key.Equals(type, StringComparison.OrdinalIgnoreCase)))
            {
                IRelationship newRelationship = m_RelationshipTypes
                    .First(t => t.Key.Equals(type, StringComparison.OrdinalIgnoreCase)).Value
                    .Create(participants);
                
                List<long> GUIDs = new List<long>();
                foreach(Entity participant in participants)
                {
                    GUIDs.Add(participant.GUID);
                }

                m_Relationships.Add(newRelationship.GenerateHash(GUIDs.ToArray()), newRelationship);
                return newRelationship;
            }

            throw new InvalidOperationException("Relationship type " + type + " not found.");
        }

        public IRelationship CreateRelationshipWithValue(Entity[] participants, string type, int value)
        {
            if(m_RelationshipTypes.Any(t => t.Key.Equals(type, StringComparison.OrdinalIgnoreCase)))
            {
                IRelationship newRelationship = m_RelationshipTypes
                    .First(t => t.Key.Equals(type, StringComparison.OrdinalIgnoreCase)).Value
                    .CreateWithValue(participants, value);

                List<long> GUIDs = new List<long>();
                foreach(Entity participant in participants)
                {
                    GUIDs.Add(participant.GUID);
                }

                m_Relationships.Add(newRelationship.GenerateHash(GUIDs.ToArray()), newRelationship);
                return newRelationship;
            }

            throw new InvalidOperationException("Relationship type " + type + " not found.");
        }

        public IRelationship[] Get(JoyObject[] participants, string[] tags = null)
        {
            IRelationship query = null;
            
            foreach(IRelationship relationship in m_RelationshipTypes.Values)
            {
                query = relationship.Create(participants);
                break;
            }

            List<long> GUIDs = new List<long>();
            foreach (JoyObject participant in participants)
            {
                GUIDs.Add(participant.GUID);
            }
            long hash = query.GenerateHash(GUIDs.ToArray());

            List<IRelationship> relationships = new List<IRelationship>();
            
            float bestPercentage = 0.0f;
            IRelationship bestRelationship = null;
            foreach(Tuple<long, IRelationship> pair in m_Relationships)
            {
                if(pair.Item1 != hash)
                {
                    continue;
                }
                
                if (tags != null)
                {
                    float tagsPercentage = 0.0f;
                    int totalTags = 0;
                    string[] relationshipTags = pair.Item2.GetTags();
                    foreach (string tag in tags)
                    {
                        if (relationshipTags.Contains(tag))
                        {
                            totalTags += 1;
                        }
                    }

                    tagsPercentage = ((float)totalTags / tags.Length);

                    if(bestPercentage < tagsPercentage)
                    {
                        bestPercentage = tagsPercentage;
                        bestRelationship = pair.Item2;
                    }

                    if(tagsPercentage == 100)
                    {
                       relationships.Add(pair.Item2);
                       break; 
                    }
                }
                else
                {
                    relationships.Add(pair.Item2);
                }
            }

            if((int)bestPercentage != 100 && !(bestRelationship is null))
            {
                relationships.Add(bestRelationship);
            }

            return relationships.ToArray();
        }

        public int GetHighestRelationshipValue(JoyObject speaker, JoyObject listener, string[] tags = null)
        {
            JoyObject[] participants = new [] { speaker, listener };
            IRelationship[] relationships = Get(participants, tags);

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

        public IRelationship[] GetAllForObject(JoyObject actor)
        {
            return m_Relationships.Where(tuple => tuple.Item1.Equals(actor.GUID))
                .Select(tuple => tuple.Item2)
                .ToArray();
        }

        public bool IsFamily(JoyObject speaker, JoyObject listener)
        {
            JoyObject[] participants = new [] { speaker, listener };
            IRelationship[] relationships = Get(participants, new[] {"family"});

            return relationships.Length > 0;
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
    }
}

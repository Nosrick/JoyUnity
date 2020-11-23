using JoyLib.Code.Scripting;
using JoyLib.Code.Collections;

using System;
using System.Collections.Generic;
using System.Linq;
using JoyLib.Code.Helpers;
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

        public IRelationship CreateRelationship(IJoyObject[] participants, string type = "friendship")
        {
            if(m_RelationshipTypes.Any(t => t.Key.Equals(type, StringComparison.OrdinalIgnoreCase)))
            {
                IRelationship newRelationship = m_RelationshipTypes
                    .First(t => t.Key.Equals(type, StringComparison.OrdinalIgnoreCase)).Value
                    .Create(participants);
                
                List<long> GUIDs = new List<long>();
                foreach(IJoyObject participant in participants)
                {
                    GUIDs.Add(participant.GUID);
                }

                newRelationship.ModifyValueOfAllParticipants(0);

                m_Relationships.Add(newRelationship.GenerateHash(GUIDs.ToArray()), newRelationship);
                return newRelationship;
            }

            throw new InvalidOperationException("Relationship type " + type + " not found.");
        }

        public IRelationship CreateRelationshipWithValue(IJoyObject[] participants, string type, int value)
        {
            IRelationship relationship = CreateRelationship(participants, type);
            if (relationship.GetRelationshipValue(participants[0].GUID, participants[1].GUID) == 0)
            {
                relationship.ModifyValueOfAllParticipants(value);
            }

            return relationship;
        }

        public IRelationship[] Get(IJoyObject[] participants, string[] tags = null, bool createNewIfNone = false)
        {
            IRelationship query = null;
            
            foreach(IRelationship relationship in m_RelationshipTypes.Values)
            {
                query = relationship.Create(participants);
                break;
            }

            List<long> GUIDs = new List<long>();
            foreach (IJoyObject participant in participants)
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
                
                if (tags != null && tags.Length > 0)
                {
                    float tagsPercentage = 0.0f;
                    int totalTags = 0;
                    List<string> relationshipTags = pair.Item2.Tags;
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

            if (relationships.Count == 0 && createNewIfNone)
            {
                relationships.Add(CreateRelationship(participants));
            }

            return relationships.ToArray();
        }

        public int GetHighestRelationshipValue(IJoyObject speaker, IJoyObject listener, string[] tags = null)
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

        public IRelationship GetBestRelationship(IJoyObject speaker, IJoyObject listener, string[] tags = null)
        {
            IJoyObject[] participants = {speaker, listener};
            IRelationship[] relationships = Get(participants, tags, false);

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

        public IRelationship[] GetAllForObject(IJoyObject actor)
        {
            return m_Relationships.Where(tuple => tuple.Item2.GetParticipant(actor.GUID) is null == false)
                .Select(tuple => tuple.Item2)
                .ToArray();
        }

        public bool IsFamily(IJoyObject speaker, IJoyObject listener)
        {
            IJoyObject[] participants = { speaker, listener };
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

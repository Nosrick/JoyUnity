using JoyLib.Code.Entities.Relationships;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JoyLib.Code.Scripting.Actions
{
    public class ModifyRelationshipPointsAction : IJoyAction
    {
        public IJoyObject[] LastParticipants { get; protected set; }
        public string[] LastTags { get; protected set; }
        public object[] LastArgs { get; protected set; }
        
        public string Name => "modifyrelationshippointsaction";

        public string ActionString => "modification of relationship points";
        
        protected static EntityRelationshipHandler RelationshipHandler { get; set; }

        public ModifyRelationshipPointsAction()
        {
            if (RelationshipHandler is null)
            {
                RelationshipHandler = GameObject.Find("GameManager").GetComponent<EntityRelationshipHandler>();
            }
        }

        public bool Execute(IJoyObject[] participants, string[] tags = null, params object[] args)
        {
            if (args.Length == 0)
            {
                return false;
            }
            
            int relationshipMod = (int)args[0];

            IRelationship[] relationships = RelationshipHandler.Get(participants, tags);

            bool doAll = args.Length < 2 ? false : (bool)args[1];

            if(relationships.Length > 0)
            {
                foreach(IRelationship relationship in relationships)
                {
                    if (doAll)
                    {
                        relationship.ModifyValueOfAllParticipants(relationshipMod);
                    }
                    else
                    {
                        relationship.ModifyValueOfOtherParticipants(participants[0].GUID, relationshipMod);
                    }
                }
            }
            
            SetLastParameters(participants, tags, args);

            return true;
        }
        
        public void SetLastParameters(IJoyObject[] participants, string[] tags = null, params object[] args)
        {
            this.LastParticipants = participants;
            this.LastTags = tags;
            this.LastArgs = args;
        }
    }
}
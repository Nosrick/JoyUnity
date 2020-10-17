using JoyLib.Code.Entities.Relationships;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JoyLib.Code.Scripting.Actions
{
    public class ModifyRelationshipPointsAction : IJoyAction
    {

        public ModifyRelationshipPointsAction()
        {
            if (RelationshipHandler is null)
            {
                RelationshipHandler = GameObject.Find("GameManager").GetComponent<EntityRelationshipHandler>();
            }
        }
        
        public string Name
        {
            get
            {
                return "modifyrelationshippointsaction";
            }
        }

        public string ActionString
        {
            get
            {
                return "modification of relationship points";
            }
        }
        
        protected static EntityRelationshipHandler RelationshipHandler { get; set; }

        public bool Execute(JoyObject[] participants, string[] tags = null, params object[] args)
        {
            if (args.Length == 0)
            {
                return false;
            }
            
            int relationshipMod = (int)args[0];

            long[] guids = participants.Select(participant => participant.GUID).ToArray();

            IRelationship[] relationships = RelationshipHandler.Get(guids, tags);

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

            return false;
        }
    }
}
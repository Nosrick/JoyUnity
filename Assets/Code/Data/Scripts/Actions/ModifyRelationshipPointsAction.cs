using JoyLib.Code.Entities.Relationships;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JoyLib.Code.Scripting.Actions
{
    public class ModifyRelationshipPointsAction : AbstractAction
    {
        public override string Name => "modifyrelationshippointsaction";

        public override string ActionString => "modification of relationship points";

        protected static IEntityRelationshipHandler RelationshipHandler { get; set; }

        public ModifyRelationshipPointsAction()
        {
            if (GlobalConstants.GameManager is null == false)
            {
                RelationshipHandler = GlobalConstants.GameManager.RelationshipHandler;
            }
        }

        public override bool Execute(IJoyObject[] participants, string[] tags = null, params object[] args)
        {
            ClearLastParameters();
            
            if (args.Length == 0)
            {
                return false;
            }

            if (participants.Distinct().Count() != participants.Length)
            {
                return false;
            }
            
            int relationshipMod = (int)args[0];

            if (RelationshipHandler is null)
            {
                return false;
            }

            IRelationship[] relationships = RelationshipHandler?.Get(participants, tags, true);

            bool doAll = args.Length >= 2 && (bool)args[1];

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
    }
}
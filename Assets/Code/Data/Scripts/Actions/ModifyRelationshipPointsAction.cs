using System.Collections.Generic;
using System.Linq;
using JoyLib.Code.Entities.Relationships;

namespace JoyLib.Code.Scripting.Actions
{
    public class ModifyRelationshipPointsAction : AbstractAction
    {
        public override string Name => "modifyrelationshippointsaction";

        public override string ActionString => "modification of relationship points";

        protected IEntityRelationshipHandler RelationshipHandler { get; set; }

        public ModifyRelationshipPointsAction()
        {
            if (GlobalConstants.GameManager is null == false)
            {
                this.RelationshipHandler = GlobalConstants.GameManager.RelationshipHandler;
            }
        }

        public override bool Execute(IJoyObject[] participants, string[] tags = null, params object[] args)
        {
            this.ClearLastParameters();
            
            if (args.Length == 0)
            {
                return false;
            }

            if (participants.Distinct().Count() != participants.Length)
            {
                return false;
            }
            
            int relationshipMod = (int)args[0];

            if (this.RelationshipHandler is null)
            {
                return false;
            }

            IEnumerable<IRelationship> relationships = this.RelationshipHandler?.Get(participants, tags, true);

            bool doAll = args.Length >= 2 && (bool)args[1];

            if(relationships.Any())
            {
                foreach(IRelationship relationship in relationships)
                {
                    if (doAll)
                    {
                        relationship.ModifyValueOfAllParticipants(relationshipMod);
                    }
                    else
                    {
                        relationship.ModifyValueOfOtherParticipants(participants[0].Guid, relationshipMod);
                    }
                }

                this.SetLastParameters(participants, tags, args);

                return true;
            }

            return false;
        }
    }
}
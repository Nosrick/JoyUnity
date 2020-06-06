using JoyLib.Code.Entities.Relationships;
using System.Linq;
using System.Collections.Generic;

namespace JoyLib.Code.Scripting.Actions
{
    class ModifyRelationshipPointsAction : IJoyAction
    {
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

        public bool Execute(JoyObject[] participants, string[] tags = null, params object[] args)
        {
            int relationshipMod = (int)args[0];

            long[] guids = participants.Select(x => x.GUID).ToArray();
            List<IRelationship> relationships = EntityRelationshipHandler.instance.Get(guids, tags);

            if(relationships.Count > 0)
            {
                foreach(IRelationship relationship in relationships)
                {
                    relationship.ModifyValueOfAllParticipants(relationshipMod);
                }
            }

            return false;
        }
    }
}
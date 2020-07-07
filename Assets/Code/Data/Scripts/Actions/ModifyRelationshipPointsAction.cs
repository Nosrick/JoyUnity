using JoyLib.Code.Entities.Relationships;
using System.Collections.Generic;

namespace JoyLib.Code.Scripting.Actions
{
    public class ModifyRelationshipPointsAction : IJoyAction
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

            List<IRelationship> relationships = new List<IRelationship>();
            for(int index = 1; index < args.Length; index++)
            {
                relationships.Add((IRelationship)args[index]);
            }

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
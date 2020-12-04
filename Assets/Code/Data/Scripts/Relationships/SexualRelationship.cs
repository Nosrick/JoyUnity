using System.Collections.Generic;

namespace JoyLib.Code.Entities.Relationships
{
    public class SexualRelationship : AbstractRelationship
    {
        public override string Name => "sexual";

        public override string DisplayName => "sexual partner";
        
        public SexualRelationship()
        {
            AddTag("sexual");
        }

        public override IRelationship Create(IEnumerable<IJoyObject> participants)
        {
            IRelationship relationship = new SexualRelationship();
            foreach (IJoyObject obj in participants)
            {
                relationship.AddParticipant(obj);
            }

            return relationship;
        }

        public override IRelationship CreateWithValue(IEnumerable<IJoyObject> participants, int value)
        {
            IRelationship newRelationship = new SexualRelationship();
            foreach (IJoyObject obj in participants)
            {
                newRelationship.AddParticipant(obj);
            }

            newRelationship.ModifyValueOfAllParticipants(value);

            return newRelationship;
        }
    }
}
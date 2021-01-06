using System.Collections.Generic;

namespace JoyLib.Code.Entities.Relationships
{
    public class PolyamourousRelationship : AbstractRelationship
    {
        public PolyamourousRelationship()
        {
            this.AddTag("romantic");
        }

        public override IRelationship Create(IEnumerable<IJoyObject> participants)
        {
            IRelationship newRelationship = new PolyamourousRelationship();
            foreach (IJoyObject obj in participants)
            {
                newRelationship.AddParticipant(obj);
            }

            return newRelationship;
        }

        public override IRelationship CreateWithValue(IEnumerable<IJoyObject> participants, int value)
        {
            IRelationship newRelationship = new PolyamourousRelationship();
            foreach (IJoyObject obj in participants)
            {
                newRelationship.AddParticipant(obj);
            }

            newRelationship.ModifyValueOfAllParticipants(value);

            return newRelationship;
        }

        public override string Name => "polyamorous";

        public override string DisplayName => "lover";
    }
}

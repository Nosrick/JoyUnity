using System.Collections.Generic;

namespace JoyLib.Code.Entities.Relationships
{
    public class Friendship : AbstractRelationship
    {
        public Friendship()
        {
            AddTag("friendship");
        }
        
        public override IRelationship Create(IEnumerable<IJoyObject> participants)
        {
            IRelationship newRelationship = new Friendship();
            foreach (IJoyObject obj in participants)
            {
                newRelationship.AddParticipant(obj);
            }

            return newRelationship;
        }

        public override IRelationship CreateWithValue(IEnumerable<IJoyObject> participants, int value)
        {
            IRelationship newRelationship = new Friendship();
            foreach (IJoyObject obj in participants)
            {
                newRelationship.AddParticipant(obj);
            }

            newRelationship.ModifyValueOfAllParticipants(value);

            return newRelationship;
        }

        public override string Name => "friendship";

        public override string DisplayName => "friend";
    }
}

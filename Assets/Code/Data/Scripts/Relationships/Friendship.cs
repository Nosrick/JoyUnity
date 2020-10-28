using System.Collections.Generic;

namespace JoyLib.Code.Entities.Relationships
{
    public class Friendship : AbstractRelationship
    {
        public Friendship() : base()
        {
            AddTag("friendship");
        }
        
        public override IRelationship Create(IEnumerable<JoyObject> participants)
        {
            IRelationship newRelationship = new Friendship();
            foreach (JoyObject obj in participants)
            {
                newRelationship.AddParticipant(obj);
            }

            return newRelationship;
        }

        public override IRelationship CreateWithValue(IEnumerable<JoyObject> participants, int value)
        {
            IRelationship newRelationship = new Friendship();
            foreach (JoyObject obj in participants)
            {
                newRelationship.AddParticipant(obj);
            }

            newRelationship.ModifyValueOfAllParticipants(value);

            return newRelationship;
        }

        public override string Name
        {
            get
            {
                return "friendship";
            }
        }
    }
}

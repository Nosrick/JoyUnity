using System.Collections.Generic;

namespace JoyLib.Code.Entities.Relationships
{
    public class MonoamorousRelationship : AbstractRelationship
    {
        public MonoamorousRelationship()
        {
            AddTag("romantic");
            AddTag("sexual");
        }

        public override bool AddParticipant(IJoyObject newParticipant)
        {
            return m_Participants.Count < 2 && base.AddParticipant(newParticipant);
        }
        
        public override IRelationship Create(IEnumerable<IJoyObject> participants)
        {
            IRelationship newRelationship = new MonoamorousRelationship();
            foreach (IJoyObject obj in participants)
            {
                newRelationship.AddParticipant(obj);
            }

            return newRelationship;
        }

        public override IRelationship CreateWithValue(IEnumerable<IJoyObject> participants, int value)
        {
            IRelationship newRelationship = new MonoamorousRelationship();
            foreach (IJoyObject obj in participants)
            {
                newRelationship.AddParticipant(obj);
            }

            newRelationship.ModifyValueOfAllParticipants(value);

            return newRelationship;
        }

        public override string Name => "monoamorous";

        public override string DisplayName => "lover";
    }
}

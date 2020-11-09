using System.Collections.Generic;

namespace JoyLib.Code.Entities.Relationships
{
    public class MonogamousRelationship : AbstractRelationship
    {
        public MonogamousRelationship()
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
            IRelationship newRelationship = new MonogamousRelationship();
            foreach (IJoyObject obj in participants)
            {
                newRelationship.AddParticipant(obj);
            }

            return newRelationship;
        }

        public override IRelationship CreateWithValue(IEnumerable<IJoyObject> participants, int value)
        {
            IRelationship newRelationship = new MonogamousRelationship();
            foreach (IJoyObject obj in participants)
            {
                newRelationship.AddParticipant(obj);
            }

            newRelationship.ModifyValueOfAllParticipants(value);

            return newRelationship;
        }

        public override string Name => "monogamousrelationship";

        public override string DisplayName => "lover";
    }
}

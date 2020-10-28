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

        public override bool AddParticipant(JoyObject newParticipant)
        {
            return m_Participants.Count < 2 && base.AddParticipant(newParticipant);
        }
        
        public override IRelationship Create(IEnumerable<JoyObject> participants)
        {
            IRelationship newRelationship = new MonogamousRelationship();
            foreach (JoyObject obj in participants)
            {
                newRelationship.AddParticipant(obj);
            }

            return newRelationship;
        }

        public override IRelationship CreateWithValue(IEnumerable<JoyObject> participants, int value)
        {
            IRelationship newRelationship = new MonogamousRelationship();
            foreach (JoyObject obj in participants)
            {
                newRelationship.AddParticipant(obj);
            }

            newRelationship.ModifyValueOfAllParticipants(value);

            return newRelationship;
        }

        public override string Name => "monogamousrelationship";
    }
}

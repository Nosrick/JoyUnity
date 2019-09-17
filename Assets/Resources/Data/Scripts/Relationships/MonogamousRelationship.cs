namespace JoyLib.Code.Entities.Relationships
{
    public class MonogamousRelationship : AbstractRelationship
    {
        public MonogamousRelationship()
        {
            AddTag("romantic");
        }

        public override bool AddParticipant(JoyObject newParticipant)
        {
            if(m_Participants.Count < 2)
            {
                return base.AddParticipant(newParticipant);
            }
            return false;
        }

        public override string Name
        {
            get
            {
                return "Monogamous";
            }
        }
    }
}

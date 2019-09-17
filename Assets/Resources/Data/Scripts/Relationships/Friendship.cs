namespace JoyLib.Code.Entities.Relationships
{
    public class Friendship : AbstractRelationship
    {
        public Friendship() : base()
        {
            AddTag("friendship");
        }

        public override string Name
        {
            get
            {
                return "Friendship";
            }
        }
    }
}

namespace JoyLib.Code.Entities.Relationships
{
    public class PolyamourousRelationship : AbstractRelationship
    {
        public PolyamourousRelationship()
        {
            AddTag("romantic");
            AddTag("sexual");
        }
        public override string Name
        {
            get
            {
                return "polyamorous";
            }
        }
    }
}

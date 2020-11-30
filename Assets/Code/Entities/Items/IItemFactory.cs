namespace JoyLib.Code.Entities.Items
{
    public interface IItemFactory
    {
        IItemInstance CreateRandomItemOfType(string[] tags, bool identified = false);

        IItemInstance CreateSpecificType(string name, string[] tags, bool identified = false);

        IItemInstance CreateCompletelyRandomItem(bool identified = false, bool withAbility = false);
    }
}
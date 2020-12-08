namespace JoyLib.Code.Entities.Items
{
    public interface IItemFactory
    {
        IItemInstance CreateRandomItemOfType(string[] tags = null, bool identified = false, bool instantiate = false);
        IItemInstance CreateSpecificType(string name, string[] tags, bool identified = false, bool instantiate = false);

        IItemInstance CreateCompletelyRandomItem(bool identified = false, bool withAbility = false, bool instantiate = false);
    }
}
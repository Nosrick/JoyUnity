namespace JoyLib.Code.Entities.Items
{
    public interface IItemFactory
    {
        ItemInstance CreateRandomItemOfType(string[] tags, bool identified = false);

        ItemInstance CreateSpecificType(string name, string[] tags, bool identified = false);

        ItemInstance CreateCompletelyRandomItem(bool identified = false, bool withAbility = false);
    }
}
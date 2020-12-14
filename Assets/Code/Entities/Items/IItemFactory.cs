using UnityEngine;

namespace JoyLib.Code.Entities.Items
{
    public interface IItemFactory
    {
        IItemInstance CreateRandomItemOfType(string[] tags = null, bool identified = false, GameObject gameObject = null);
        IItemInstance CreateSpecificType(string name, string[] tags, bool identified = false, GameObject gameObject = null);
        IItemInstance CreateCompletelyRandomItem(bool identified = false, bool withAbility = false, GameObject gameObject = null);
    }
}
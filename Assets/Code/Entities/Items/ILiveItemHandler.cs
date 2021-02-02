using System.Collections.Generic;
using JoyLib.Code.World;

namespace JoyLib.Code.Entities.Items
{
    public interface ILiveItemHandler
    {
        BaseItemType[] FindItemsOfType(string[] tags, int tolerance = 1);

        bool AddItem(IItemInstance item, bool addToWorld = false);
        bool AddItems(IEnumerable<IItemInstance> item, bool addToWorld = false);

        bool RemoveItemFromWorld(long GUID);

        bool RemoveItemFromWorld(IItemInstance item);

        bool AddItemToWorld(WorldInstance world, long GUID);

        IItemInstance GetItem(long GUID);

        IEnumerable<IItemInstance> GetItems(IEnumerable<long> guids);
        
        List<BaseItemType> ItemDatabase { get; }
        
        IEnumerable<IItemInstance> AllItems { get; }
    }
}
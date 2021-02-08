using System.Collections.Generic;
using JoyLib.Code.Collections;
using JoyLib.Code.World;

namespace JoyLib.Code.Entities.Items
{
    public interface ILiveItemHandler
    {
        BaseItemType[] FindItemsOfType(string[] tags, int tolerance = 1);

        bool AddItem(IItemInstance item);
        bool AddItems(IEnumerable<IItemInstance> item, bool addToWorld = false);

        bool RemoveItemFromWorld(long GUID);

        bool RemoveItemFromWorld(IItemInstance item);

        bool AddItemToWorld(WorldInstance world, long GUID);

        IItemInstance GetItem(long GUID);
        IEnumerable<IItemInstance> GetQuestRewards(long questID);

        void CleanUpRewards(IEnumerable<long> GUIDs);
        void AddQuestReward(long questID, long reward);
        void AddQuestRewards(long questID, IEnumerable<long> rewards);
        void AddQuestRewards(long questID, IEnumerable<IItemInstance> rewards);

        IEnumerable<IItemInstance> GetItems(IEnumerable<long> guids);
        
        List<BaseItemType> ItemDatabase { get; }
        
        IEnumerable<IItemInstance> AllItems { get; }
        
        NonUniqueDictionary<long, long> QuestRewards { get; }
    }
}
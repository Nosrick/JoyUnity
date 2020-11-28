using System.Collections.Generic;
using JoyLib.Code.Entities.Abilities;

namespace JoyLib.Code.Entities.Items
{
    public interface IItemInstance : IJoyObject, IItemContainer, IOwnable
    {
        long OwnerGUID { get; }
        
        string OwnerString { get; }
        
        List<IAbility> UniqueAbilities { get; }

        ItemInstance Copy(ItemInstance copy);

        void SetOwner(long newOwner, bool recursive = false);

        void Interact(Entity user);

        void IdentifyMe();

        IItemInstance TakeMyItem(int index);
        
        bool Identified { get; }
        
        bool Broken { get; }
        
        int Efficiency { get; }
        
        string ConditionString { get; }
        
        string SlotString { get; }
        
        string IdentifiedName { get; }
        
        string DisplayDescription { get; }
        
        float Weight { get; }
        
        string WeightString { get; }
        
        BaseItemType ItemType { get; }
        
        int Value { get; }
    }
}
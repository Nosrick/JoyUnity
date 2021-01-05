using System;
using System.Collections.Generic;
using JoyLib.Code.Entities.Abilities;
using JoyLib.Code.Graphics;

namespace JoyLib.Code.Entities.Items
{
    public interface IItemInstance : IJoyObject, IItemContainer, IOwnable
    {
        IEnumerable<ISpriteState> Sprites { get; }
        
        IEnumerable<IAbility> UniqueAbilities { get; }
        
        IEnumerable<IAbility> AllAbilities { get; }

        IItemInstance Copy(IItemInstance copy);

        void Interact(IEntity user);

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
        
        string DisplayName { get; }
        
        IEnumerable<Tuple<string, string>> Tooltip { get; }
        
        BaseItemType ItemType { get; }
        
        int Value { get; }
    }
}
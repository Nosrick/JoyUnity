using System.Collections.Generic;
using JoyLib.Code.Entities.Abilities;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Graphics;
using JoyLib.Code.Rollers;
using JoyLib.Code.Scripting;
using UnityEngine;

namespace JoyLib.Code.Entities.Items
{
    public class ItemFactory :IItemFactory
    {
        protected IGameManager GameManager { get; set; }

        protected ILiveItemHandler ItemHandler { get; set; }

        protected IObjectIconHandler ObjectIcons { get; set; }
        
        protected IDerivedValueHandler DerivedValueHandler { get; set; }
        protected RNG Roller { get; set; }

        public ItemFactory(
            ILiveItemHandler itemHandler, 
            IObjectIconHandler objectIconHandler,
            IDerivedValueHandler derivedValueHandler,
            RNG roller)
        {
            this.ItemHandler = itemHandler;
            this.ObjectIcons = objectIconHandler;
            this.DerivedValueHandler = derivedValueHandler;
            this.Roller = roller;
        }

        public IItemInstance CreateRandomItemOfType(string[] tags, bool identified = false, bool instantiate = false)
        {
            BaseItemType[] matchingTypes = ItemHandler.FindItemsOfType(tags);
            if (matchingTypes.Length > 0)
            {
                int result = Roller.Roll(0, matchingTypes.Length);
                BaseItemType itemType = matchingTypes[result];

                List<IBasicValue<float>> values = new List<IBasicValue<float>>();
                values.Add(new ConcreteBasicFloatValue(
                    "weight", itemType.Weight));
                values.Add(new ConcreteBasicFloatValue(
                    "bonus", itemType.Material.Bonus));
                values.Add(new ConcreteBasicFloatValue(
                    "size", itemType.Size));
                values.Add(new ConcreteBasicFloatValue(
                    "hardness", itemType.Material.Hardness));
                values.Add(new ConcreteBasicFloatValue(
                    "density", itemType.Material.Density));
                
                ItemInstance itemInstance = ScriptableObject.CreateInstance<ItemInstance>();
                
                itemInstance.Initialise(
                    itemType,
                    this.DerivedValueHandler.GetItemStandardBlock(values),
                        new Vector2Int(-1, -1), 
                        identified, 
                        ObjectIcons.GetSprites(
                            itemType.SpriteSheet,
                            itemType.UnidentifiedName),
                        new RNG(),
                        new List<IAbility>(),
                        new List<IJoyAction>(),
                        instantiate);
                return itemInstance;
            }

            return null;
        }

        public IItemInstance CreateSpecificType(string name, string[] tags, bool identified = false, bool instantiate = false)
        {
            BaseItemType[] matchingTypes = ItemHandler.FindItemsOfType(tags);
            List<BaseItemType> secondRound = new List<BaseItemType>();
            foreach (BaseItemType itemType in matchingTypes)
            {
                if (identified == false)
                {
                    if (itemType.UnidentifiedName.Equals(name, System.StringComparison.OrdinalIgnoreCase))
                    {
                        secondRound.Add(itemType);
                    }
                }
                else
                {
                    if (itemType.IdentifiedName.Equals(name, System.StringComparison.OrdinalIgnoreCase))
                    {
                        secondRound.Add(itemType);
                    }
                }
            }
            if (secondRound.Count > 0)
            {
                int result = Roller.Roll(0, secondRound.Count);
                BaseItemType type = secondRound[result];
                ItemInstance itemInstance = ScriptableObject.CreateInstance<ItemInstance>(); 

                List<IBasicValue<float>> values = new List<IBasicValue<float>>();
                values.Add(new ConcreteBasicFloatValue(
                    "weight", type.Weight));
                values.Add(new ConcreteBasicFloatValue(
                    "bonus", type.Material.Bonus));
                values.Add(new ConcreteBasicFloatValue(
                    "size", type.Size));
                values.Add(new ConcreteBasicFloatValue(
                    "hardness", type.Material.Hardness));
                values.Add(new ConcreteBasicFloatValue(
                    "density", type.Material.Density));
                    
                    itemInstance.Initialise(
                        type, 
                        this.DerivedValueHandler.GetItemStandardBlock(values),
                        new Vector2Int(-1, -1), 
                        identified, 
                        ObjectIcons.GetSprites(
                            type.SpriteSheet,
                            type.UnidentifiedName),
                        new RNG(),
                        new List<IAbility>(),
                        new List<IJoyAction>(),
                        instantiate);
                    
                return itemInstance;
            }

            throw new ItemTypeNotFoundException(name, "Could not find an item type by the name of " + name);
        }

        public IItemInstance CreateCompletelyRandomItem(bool identified = false,
            bool withAbility = false, bool instantiate = false)
        {
            List<BaseItemType> itemDatabase = ItemHandler.ItemDatabase;

            int result = Roller.Roll(0, itemDatabase.Count);
            BaseItemType itemType = itemDatabase[result];
            ItemInstance itemInstance = ScriptableObject.CreateInstance<ItemInstance>(); 

            List<IBasicValue<float>> values = new List<IBasicValue<float>>();
            values.Add(new ConcreteBasicFloatValue(
                "weight", itemType.Weight));
            values.Add(new ConcreteBasicFloatValue(
                "bonus", itemType.Material.Bonus));
            values.Add(new ConcreteBasicFloatValue(
                "size", itemType.Size));
            values.Add(new ConcreteBasicFloatValue(
                "hardness", itemType.Material.Hardness));
            values.Add(new ConcreteBasicFloatValue(
                "density", itemType.Material.Density));
            
                itemInstance.Initialise(
                    itemType, 
                    this.DerivedValueHandler.GetItemStandardBlock(values),
                    new Vector2Int(-1, -1), 
                    identified, 
                    ObjectIcons.GetSprites(
                        itemType.SpriteSheet,
                        itemType.UnidentifiedName),
                    new RNG(),
                    new List<IAbility>(),
                    new List<IJoyAction>(),
                    instantiate);
                
            return itemInstance;
        }
    }
}
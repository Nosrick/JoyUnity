using System.Collections.Generic;
using System.Linq;
using Code.Collections;
using JoyLib.Code.Entities.Abilities;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Graphics;
using JoyLib.Code.Rollers;
using JoyLib.Code.Scripting;
using UnityEngine;

namespace JoyLib.Code.Entities.Items
{
    public class ItemFactory : IItemFactory
    {
        protected IGameManager GameManager { get; set; }

        protected ILiveItemHandler ItemHandler { get; set; }

        protected IObjectIconHandler ObjectIcons { get; set; }

        protected IDerivedValueHandler DerivedValueHandler { get; set; }

        protected GameObjectPool ItemPool { get; set; }

        protected RNG Roller { get; set; }

        public ItemFactory(
            ILiveItemHandler itemHandler,
            IObjectIconHandler objectIconHandler,
            IDerivedValueHandler derivedValueHandler,
            GameObjectPool itemPool,
            RNG roller = null)
        {
            this.ItemHandler = itemHandler;
            this.ObjectIcons = objectIconHandler;
            this.DerivedValueHandler = derivedValueHandler;
            this.ItemPool = itemPool;
            this.Roller = roller is null ? new RNG() : roller;
        }

        public IItemInstance CreateRandomItemOfType(string[] tags, bool identified = false)
        {
            BaseItemType[] matchingTypes = this.ItemHandler.FindItemsOfType(tags, tags.Length);
            if (matchingTypes.Length > 0)
            {
                int result = this.Roller.Roll(0, matchingTypes.Length);
                BaseItemType itemType = matchingTypes[result];

                IItemInstance itemInstance = this.CreateFromTemplate(itemType, identified);

                this.ItemHandler.AddItem(itemInstance);
                return itemInstance;
            }

            return null;
        }

        public IItemInstance CreateSpecificType(string name, string[] tags, bool identified = false)
        {
            BaseItemType[] matchingTypes = this.ItemHandler.FindItemsOfType(tags);
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
                int result = this.Roller.Roll(0, secondRound.Count);
                BaseItemType itemType = secondRound[result];

                IItemInstance itemInstance = this.CreateFromTemplate(itemType, identified);

                this.ItemHandler.AddItem(itemInstance);
                return itemInstance;
            }

            throw new ItemTypeNotFoundException(name, "Could not find an item type by the name of " + name);
        }

        public IItemInstance CreateCompletelyRandomItem(
            bool identified = false,
            bool withAbility = false)
        {
            List<BaseItemType> itemDatabase = this.ItemHandler.ItemDatabase;

            int result = this.Roller.Roll(0, itemDatabase.Count);
            BaseItemType itemType = itemDatabase[result];

            IItemInstance itemInstance = this.CreateFromTemplate(itemType, identified);

            this.ItemHandler.AddItem(itemInstance);
            return itemInstance;
        }

        public IItemInstance CreateFromTemplate(BaseItemType itemType, bool identified = false)
        {
            List<IBasicValue<float>> values = new List<IBasicValue<float>>
            {
                new ConcreteBasicFloatValue(
                    "weight", itemType.Weight),
                new ConcreteBasicFloatValue(
                    "bonus", itemType.Material.Bonus),
                new ConcreteBasicFloatValue(
                    "size", itemType.Size),
                new ConcreteBasicFloatValue(
                    "hardness", itemType.Material.Hardness),
                new ConcreteBasicFloatValue(
                    "density", itemType.Material.Density)
            };

            List<SpriteState> states = (from sprite in this.ObjectIcons.GetSprites(
                        itemType.SpriteSheet,
                        itemType.UnidentifiedName)
                    select new SpriteState(sprite.m_Name, sprite))
                .ToList();
            
            states[0].RandomiseColours();
            for (int i = 1; i < states.Count; i++)
            {
                states[i].SetColourIndices(states[0].GetIndices());
            }

            ItemInstance itemInstance = new ItemInstance(
                itemType,
                this.DerivedValueHandler.GetItemStandardBlock(values),
                new Vector2Int(-1, -1),
                identified, 
                states,
                new RNG(),
                new List<IAbility>(),
                new List<IJoyAction>(),
                this.ItemPool.Get());

            return itemInstance;
        }
    }
}
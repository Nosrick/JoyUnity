using System.Collections.Generic;
using JoyLib.Code.Entities.Items;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Graphics;
using UnityEngine;

namespace JoyLib.Code.Helpers
{
    public class NaturalWeaponHelper
    {
        protected IObjectIconHandler ObjectIcons { get; set; }
        protected IDerivedValueHandler DerivedValueHandler { get; set; }
        protected IMaterialHandler MaterialHandler { get; set; }

        public NaturalWeaponHelper(
            IObjectIconHandler objectIconHandler,
            IDerivedValueHandler derivedValueHandler,
            IMaterialHandler materialHandler)
        {
            ObjectIcons = objectIconHandler;
            MaterialHandler = materialHandler;
            this.DerivedValueHandler = derivedValueHandler;
        }

        //TODO: THIS NEEDS TO BE REWRITTEN ENTIRELY TO SUPPORT NEW SIZE MECHANICS
        public IItemInstance MakeNaturalWeapon(int wielderSize, string material = "flesh", params string[] tags)
        {
            IItemMaterial itemMaterial = MaterialHandler.GetMaterial(material);
            BaseItemType baseItem = new BaseItemType(tags, "A claw, fist or psuedopod.", "A claw, fist or psuedopod.", "Natural Weapon", "Natural Weapon", new string[] { "Hand" }, 
                (wielderSize + 1) * 40.0f, itemMaterial, "Martial Arts", "strikes", 0, 0, "None");

            List<IBasicValue<float>> values = new List<IBasicValue<float>>
            {
                new ConcreteBasicFloatValue("weight", baseItem.Weight),
                new ConcreteBasicFloatValue("size", baseItem.Size),
                new ConcreteBasicFloatValue("hardness", baseItem.Material.Hardness),
                new ConcreteBasicFloatValue("bonus", baseItem.Material.Bonus),
                new ConcreteBasicFloatValue("density", baseItem.Material.Density)
            };
            
            ItemInstance naturalWeapon = ScriptableObject.CreateInstance<ItemInstance>();
            naturalWeapon.Initialise(
                baseItem, 
                this.DerivedValueHandler.GetItemStandardBlock(values),
                new Vector2Int(-1, -1), 
                true, 
                ObjectIcons.GetDefaultSprites());
            return naturalWeapon;
        }
    }
}
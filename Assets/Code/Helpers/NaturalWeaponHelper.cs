using JoyLib.Code.Entities.Items;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Graphics;
using JoyLib.Code.Unity;
using UnityEngine;

namespace JoyLib.Code.Helpers
{
    public class NaturalWeaponHelper
    {
        protected IObjectIconHandler ObjectIcons { get; set; }

        protected IMaterialHandler MaterialHandler { get; set; }

        public NaturalWeaponHelper(IObjectIconHandler objectIconHandler, IMaterialHandler materialHandler)
        {
            ObjectIcons = objectIconHandler;
            MaterialHandler = materialHandler;
        }

        //TODO: THIS NEEDS TO BE REWRITTEN ENTIRELY TO SUPPORT NEW SIZE MECHANICS
        public IItemInstance MakeNaturalWeapon(int wielderSize, string material = "flesh", params string[] tags)
        {
            IItemMaterial itemMaterial = MaterialHandler.GetMaterial(material);
            BaseItemType baseItem = new BaseItemType(tags, "A claw, fist or psuedopod.", "A claw, fist or psuedopod.", "Natural Weapon", "Natural Weapon", new string[] { "Hand" }, 
                (wielderSize + 1) * 40.0f, itemMaterial, "Martial Arts", "strikes", 0, 0, "None");

            ItemInstance naturalWeapon = ScriptableObject.CreateInstance<ItemInstance>();
            naturalWeapon.Initialise(
                baseItem, 
                EntityDerivedValue.GetDefaultForItem(
                    baseItem.Material.Bonus,
                    baseItem.Weight),
                new Vector2Int(-1, -1), 
                true, 
                ObjectIcons.GetDefaultSprites());
            return naturalWeapon;
        }
    }
}
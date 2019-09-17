using JoyLib.Code.Entities.Items;
using UnityEngine;

namespace JoyLib.Code.Helpers
{
    public static class NaturalWeaponHelper
    {
        public static ItemInstance MakeNaturalWeapon(int wielderSize, string material = "Flesh", params string[] tags)
        {
            ItemMaterial itemMaterial = MaterialHandler.GetMaterial(material);
            BaseItemType baseItem = new BaseItemType(tags, "A claw, fist or psuedopod.", "A claw, fist or psuedopod.", "Natural Weapon", "Natural Weapon", new string[] { "Hand" }, 
                wielderSize * 40.0f, itemMaterial, "Martial Arts", "strikes", 0, 0, "None");

            return new ItemInstance(baseItem, new Vector2Int(-1, -1), true);
        }
    }
}
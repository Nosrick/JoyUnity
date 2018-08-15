using JoyLib.Code.Entities.Items;
using UnityEngine;

namespace JoyLib.Code.Helpers
{
    public static class NaturalWeaponHelper
    {
        public static ItemInstance MakeNaturalWeapon(int wielderSize)
        {
            ItemMaterial itemMaterial = MaterialHandler.GetMaterial("Flesh");
            BaseItemType baseItem = new BaseItemType("Weapon", "A claw, fist or psuedopod.", "A claw, fist or psuedopod.", "Natural Weapon", "Natural Weapon", "Hand1", 
                wielderSize * 40.0f, itemMaterial, "Weapon", "Martial Arts", "strikes", null, 0, 0);

            return new ItemInstance(baseItem, new Vector2Int(-1, -1), true);
        }
    }
}
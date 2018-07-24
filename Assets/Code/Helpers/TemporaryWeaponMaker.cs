using JoyLib.Code.Entities.Items;
using UnityEngine;

namespace JoyLib.Code.Helpers
{
    public static class TemporaryWeaponMaker
    {
        //Meant for making things like magic blasts that will never actually appear in the world.
        public static ItemInstance Make(int number, int faces, string actionString, string skill)
        {
            int division = 50;
            int size = number * division;
            float weight = (float)faces / number;

            ItemMaterial material = new ItemMaterial("", 1, 0, weight);

            BaseItemType tempItem = new BaseItemType("", "", "", "Temporary weapon", "Temporary weapon", "None", size, material, "Weapon", skill, actionString, null, 0, 0, 0);

            return new ItemInstance(tempItem, new Vector2Int(-1, -1), true);
        }
    }
}

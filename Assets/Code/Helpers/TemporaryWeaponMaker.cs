using JoyLib.Code.Entities.Items;
using JoyLib.Code.Graphics;
using UnityEngine;

namespace JoyLib.Code.Helpers
{
    public static class TemporaryWeaponMaker
    {
        //Meant for making things like magic blasts that will never actually appear in the world.
        public static ItemInstance Make(int size, string actionString, string skill, string materialName = "magic", params string[] tags)
        {
            ItemMaterial material = new ItemMaterial(materialName, 1, 0, 0, 0.0f);

            BaseItemType tempItem = new BaseItemType(tags, "", "Temporary weapon", "Temporary weapon", "Temporary weapon", new string[] { "None" }, size, material, skill, actionString, 0, 0, "None");

            return new ItemInstance(tempItem, new Vector2Int(-1, -1), true, ObjectIconHandler.instance.GetDefaultSprites());
        }
    }
}

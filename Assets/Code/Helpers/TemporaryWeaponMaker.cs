using JoyLib.Code.Entities.Items;
using JoyLib.Code.Graphics;
using UnityEngine;

namespace JoyLib.Code.Helpers
{
    public static class TemporaryWeaponMaker
    {
        private static ObjectIconHandler s_ObjectIcons;

        private static void Initialise()
        {
            s_ObjectIcons = GameObject.Find("GameManager").GetComponent<ObjectIconHandler>();
        }

        //Meant for making things like magic blasts that will never actually appear in the world.
        public static ItemInstance Make(int size, string actionString, string skill, string weaponName = "temporary weapon", string materialName = "magic", params string[] tags)
        {
            if(s_ObjectIcons is null)
            {
                Initialise();
            }

            ItemMaterial material = new ItemMaterial(materialName, 1, 0, 0, 0.0f);

            BaseItemType tempItem = new BaseItemType(
                tags, 
                "", 
                weaponName, 
                weaponName,
                weaponName,
                new string[] { "None" }, 
                size, 
                material, 
                skill, 
                actionString, 
                0, 
                0, 
                "None");

            return new ItemInstance(tempItem, new Vector2Int(-1, -1), true, s_ObjectIcons.GetDefaultSprites(), null);
        }
    }
}

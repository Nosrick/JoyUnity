using JoyLib.Code.Entities.Items;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Graphics;
using UnityEngine;

namespace JoyLib.Code.Helpers
{
    public static class TemporaryWeaponMaker
    {
        private static ObjectIconHandler s_ObjectIcons;

        private static void Initialise()
        {
            s_ObjectIcons = GlobalConstants.GameManager.GetComponent<ObjectIconHandler>();
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

            ItemInstance temporary = ScriptableObject.CreateInstance<ItemInstance>();
            temporary.Initialise(
                tempItem,
                EntityDerivedValue.GetDefaultForItem(
                    tempItem.Material.Bonus,
                    tempItem.Weight),
                GlobalConstants.NO_TARGET,
                true,
                s_ObjectIcons.GetDefaultSprites());

            return temporary;
        }
    }
}

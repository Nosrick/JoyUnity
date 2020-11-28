using JoyLib.Code.Entities.Items;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Graphics;
using JoyLib.Code.Unity;
using UnityEngine;

namespace JoyLib.Code.Helpers
{
    public static class NaturalWeaponHelper
    {
        private static IGameManager s_GameManager;

        private static IObjectIconHandler s_ObjectIcons;

        private static IMaterialHandler s_MaterialHandler;

        private static void Initialise()
        {
            s_GameManager = GlobalConstants.GameManager;
            s_ObjectIcons = s_GameManager.ObjectIconHandler;
            s_MaterialHandler = s_GameManager.MaterialHandler;
        }

        //TODO: THIS NEEDS TO BE REWRITTEN ENTIRELY TO SUPPORT NEW SIZE MECHANICS
        public static IItemInstance MakeNaturalWeapon(int wielderSize, string material = "flesh", params string[] tags)
        {
            if(s_GameManager is null)
            {
                Initialise();
            }

            IItemMaterial itemMaterial = s_MaterialHandler.GetMaterial(material);
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
                s_ObjectIcons.GetDefaultSprites());
            return naturalWeapon;
        }
    }
}
using JoyLib.Code.Entities.Items;
using JoyLib.Code.Graphics;
using UnityEngine;

namespace JoyLib.Code.Helpers
{
    public static class NaturalWeaponHelper
    {
        private static GameObject s_GameManager;

        private static ObjectIconHandler s_ObjectIcons;

        private static MaterialHandler s_MaterialHandler;

        private static void Initialise()
        {
            s_GameManager = GameObject.Find("GameManager");
            s_ObjectIcons = s_GameManager.GetComponent<ObjectIconHandler>();
            s_MaterialHandler = s_GameManager.GetComponent<MaterialHandler>();
        }

        //TODO: THIS NEEDS TO BE REWRITTEN ENTIRELY TO SUPPORT NEW SIZE MECHANICS
        public static ItemInstance MakeNaturalWeapon(int wielderSize, string material = "flesh", params string[] tags)
        {
            if(s_GameManager is null)
            {
                Initialise();
            }

            ItemMaterial itemMaterial = s_MaterialHandler.GetMaterial(material);
            BaseItemType baseItem = new BaseItemType(tags, "A claw, fist or psuedopod.", "A claw, fist or psuedopod.", "Natural Weapon", "Natural Weapon", new string[] { "Hand" }, 
                (wielderSize + 1) * 40.0f, itemMaterial, "Martial Arts", "strikes", 0, 0, "None");

            return new ItemInstance(baseItem, new Vector2Int(-1, -1), true, s_ObjectIcons.GetDefaultSprites(), null);
        }
    }
}
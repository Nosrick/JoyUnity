﻿using JoyLib.Code.Entities.Items;
using JoyLib.Code.Graphics;
using UnityEngine;

namespace JoyLib.Code.Helpers
{
    public class NaturalWeaponHelper
    {
        private static ObjectIconHandler s_ObjectIcons = GameObject.Find("GameManager")
                                                            .GetComponent<ObjectIconHandler>();

        //TODO: THIS NEEDS TO BE REWRITTEN ENTIRELY TO SUPPORT NEW SIZE MECHANICS
        public ItemInstance MakeNaturalWeapon(int wielderSize, string material = "Flesh", params string[] tags)
        {
            ItemMaterial itemMaterial = MaterialHandler.instance.GetMaterial(material);
            BaseItemType baseItem = new BaseItemType(tags, "A claw, fist or psuedopod.", "A claw, fist or psuedopod.", "Natural Weapon", "Natural Weapon", new string[] { "Hand" }, 
                (wielderSize + 1) * 40.0f, itemMaterial, "Martial Arts", "strikes", 0, 0, "None");

            return new ItemInstance(baseItem, new Vector2Int(-1, -1), true, s_ObjectIcons.GetDefaultSprites());
        }
    }
}
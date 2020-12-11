using System.Collections.Generic;
using JoyLib.Code.Entities.Items;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Graphics;
using UnityEngine;

namespace JoyLib.Code.Helpers
{
    public static class TemporaryWeaponMaker
    {
        private static IObjectIconHandler s_ObjectIcons;
        private static IDerivedValueHandler s_DerivedValueHandler;

        private static void Initialise()
        {
            s_ObjectIcons = GlobalConstants.GameManager.ObjectIconHandler;
            s_DerivedValueHandler = GlobalConstants.GameManager.DerivedValueHandler;
        }

        //Meant for making things like magic blasts that will never actually appear in the world.
        public static IItemInstance Make(int size, string actionString, string skill, string weaponName = "temporary weapon", string materialName = "magic", params string[] tags)
        {
            if(s_ObjectIcons is null)
            {
                Initialise();
            }

            ItemMaterial material = new ItemMaterial(materialName, 1, 0, 1, 0.0f);

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
            
            List<IBasicValue<float>> values = new List<IBasicValue<float>>
            {
                new ConcreteBasicFloatValue("weight", tempItem.Weight),
                new ConcreteBasicFloatValue("bonus", tempItem.Material.Bonus),
                new ConcreteBasicFloatValue("density", tempItem.Material.Density),
                new ConcreteBasicFloatValue("hardness", tempItem.Material.Hardness),
                new ConcreteBasicFloatValue("size", tempItem.Size)
            };

            ItemInstance temporary = ScriptableObject.CreateInstance<ItemInstance>();
            temporary.Initialise(
                tempItem,
                s_DerivedValueHandler.GetItemStandardBlock(values),
                GlobalConstants.NO_TARGET,
                true,
                s_ObjectIcons.GetDefaultSprites());

            return temporary;
        }
    }
}

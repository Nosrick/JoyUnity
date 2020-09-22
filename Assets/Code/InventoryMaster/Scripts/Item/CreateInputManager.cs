using InventoryMaster.Scripts.Inventory;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR

#endif

namespace InventoryMaster.Scripts.Item
{
    public class CreateInputManager
    {

        public static InputManager asset;

#if UNITY_EDITOR
        public static InputManager createInputManager()
        {
            asset = ScriptableObject.CreateInstance<InputManager>();

            AssetDatabase.CreateAsset(asset, "Assets/InventoryMaster/Resources/InputManager.asset");
            AssetDatabase.SaveAssets();
            return asset;
        }
#endif

    }
}

using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
#endif

namespace InventoryMaster.Scripts.Item
{
    public class CreateAttributeDatabase : MonoBehaviour
    {

        public static ItemAttributeList asset;                                                  //The List of all Items

#if UNITY_EDITOR
        public static ItemAttributeList createItemAttributeDatabase()                                    //creates a new ItemDatabase(new instance)
        {
            asset = ScriptableObject.CreateInstance<ItemAttributeList>();                       //of the ScriptableObject InventoryItemList

            AssetDatabase.CreateAsset(asset, "Assets/InventoryMaster/Resources/AttributeDatabase.asset");            //in the Folder Assets/Resources/ItemDatabase.asset
            AssetDatabase.SaveAssets();                                                         //and than saves it there        
            return asset;
        }
#endif

    }
}

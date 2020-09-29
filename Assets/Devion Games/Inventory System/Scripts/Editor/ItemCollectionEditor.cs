using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.Linq;

namespace DevionGames.InventorySystem
{
	public class ItemCollectionEditor : ScriptableObjectCollectionEditor<Item>
	{
		[SerializeField]
		protected List<string> searchFilters;
		[SerializeField]
		protected string searchFilter = "All";

        public override string ToolbarName
        {
            get
            {
                return "Items";
            }
        }

        public ItemCollectionEditor (UnityEngine.Object target, List<Item> items, List<string> searchFilters) : base (target, items)
		{
			this.target = target;
			this.items = items;
			this.searchFilters = searchFilters;
			this.searchFilters.Insert (0, "All");
            this.searchString = "All";
            sidebarRect.width = EditorPrefs.GetFloat("CollectionEditorSidebarWidth" + ToolbarName, sidebarRect.width);
        }

		protected override void DoSearchGUI ()
		{
			string[] searchResult = EditorTools.SearchField (searchString, searchFilter, searchFilters);
			searchFilter = searchResult [0];
			searchString = string.IsNullOrEmpty(searchResult [1])?searchFilter:searchResult[1] ;
		}

		protected override bool MatchesSearch (Item item, string search)
		{
			return (item.Name.ToLower ().Contains (search.ToLower ()) || searchString == searchFilter || search.ToLower() == item.GetType().Name.ToLower()) && (searchFilter == "All" || item.Category.Name == searchFilter);
		}

		protected override bool HasConfigurationErrors(Item item)
		{
			return Items.Any(x => !x.Equals(item) && x.Name == item.Name) ||
				string.IsNullOrEmpty(item.Name);
		}

        protected override void Duplicate(Item item)
        {
			Item duplicate = (Item)ScriptableObject.Instantiate(item);
			duplicate.hideFlags = HideFlags.HideInHierarchy;
			AssetDatabase.AddObjectToAsset(duplicate, target);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
			Items.Add(duplicate);
			selectedItem = duplicate;
			EditorUtility.SetDirty(target);
		}
    }
}
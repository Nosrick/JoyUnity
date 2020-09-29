using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace DevionGames.InventorySystem
{
	[CustomPropertyDrawer (typeof(PickerAttribute), true)]
	public abstract class PickerDrawer<T> : PropertyDrawer where T: ScriptableObject, INameable
	{

		protected ItemDatabase Database {
			get {
				if (InventorySystemEditor.Database != null) {
					return InventorySystemEditor.Database;
				}
                return null;
			}
		}

		protected abstract List<T> Items { get; }

		protected virtual string[] Names {
			get {
				string[] items = new string[Items.Count];
				for (int i = 0; i < Items.Count; i++) {
					items [i] = Items [i].Name;
				}
				return items;
			}
		}

		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
		{
		
			T current= GetCurrent(property);
            CheckForDatabase(current);

            if (Database == null || Items.Count == 0) {
				string errMsg = Database == null ? "No database selected. Please open the editor and select a database." : "There are no items in this database. Please open the editor and create at least one item.";
				position.height = 30;
				EditorGUI.HelpBox (position, errMsg, MessageType.Error);
				position.y += 32;
				position.height = EditorGUIUtility.singleLineHeight;
				//EditorGUI.Popup (position, System.Text.RegularExpressions.Regex.Replace (typeof(T).Name, "([a-z])_?([A-Z])", "$1 $2"), 0, new string[]{current!=null?current.Name:"Null"});
				EditorGUI.Popup (position, label.text, 0, new string[]{ current != null ? current.Name : "Null" });
				return;
			}

			DoSelection (position, property, label, current);
		}

        protected void CheckForDatabase(T current) {
            if (Database == null)
            {
                if (current != null)
                {
                    ItemDatabase[] databases = EditorTools.FindAssets<ItemDatabase>();

                    for (int i = 0; i < databases.Length; i++)
                    {
						List<INameable> items = new List<INameable>();
						items.AddRange(databases[i].items);
						items.AddRange(databases[i].categories);
						items.AddRange(databases[i].raritys);
						items.AddRange(databases[i].equipments);
						items.AddRange(databases[i].currencies);
						items.AddRange(databases[i].itemGroups);

						if (items.Find(x => x == current) != null) {
							InventorySystemEditor.Database = databases[i];
						}
                    }
                }
            }
        }

        protected T GetCurrent(SerializedProperty property) {
            T current;
            Type fieldType = fieldInfo.FieldType;
            object targetObject = GetParent(property);
            if (typeof(IEnumerable).IsAssignableFrom(fieldType))
            {
                int currentIndex = System.Convert.ToInt32(System.Text.RegularExpressions.Regex.Match(property.propertyPath, @"(\d+)(?!.*\d)").Value);

                IEnumerable<T> array = (IEnumerable<T>)fieldInfo.GetValue(targetObject);
                List<T> list = new List<T>(array);
                if (list.Count - 1 < currentIndex)
                {
                    for (int i = list.Count - 1; i < currentIndex; i++)
                    {
                        list.Add(default(T));
                    }
					if (fieldInfo.FieldType.IsArray)
					{
						fieldInfo.SetValue(targetObject, list.ToArray());
					}
					else
					{
						fieldInfo.SetValue(targetObject, list);
					}
                }
                current = list[currentIndex];
            }
            else
            {
                current = property.objectReferenceValue as T;
            }
            return current;
        }

		public object GetParent (SerializedProperty prop)
		{
			var path = prop.propertyPath.Replace (".Array.data[", "[");
			object obj = prop.serializedObject.targetObject;
			var elements = path.Split ('.');
			foreach (var element in elements.Take(elements.Length-1)) {
				if (element.Contains ("[")) {
					var elementName = element.Substring (0, element.IndexOf ("["));
					var index = Convert.ToInt32 (element.Substring (element.IndexOf ("[")).Replace ("[", "").Replace ("]", ""));
					obj = GetValue (obj, elementName, index);
				} else {
					obj = GetValue (obj, element);
				}
			}
			return obj;
		}

		public object GetValue (object source, string name)
		{
			if (source == null)
				return null;
			var type = source.GetType ();
			var f = type.GetSerializedField (name);
			if (f == null) {
				var p = type.GetProperty (name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
				if (p == null)
					return null;
				return p.GetValue (source, null);
			}
			return f.GetValue (source);
		}

		public object GetValue (object source, string name, int index)
		{
			var enumerable = GetValue (source, name) as IEnumerable;
			var enm = enumerable.GetEnumerator ();
			while (index-- >= 0)
				enm.MoveNext ();

			return enm.Current;
		}

		protected virtual void DoSelection (Rect position, SerializedProperty property, GUIContent label, T current)
		{
			
			if ((attribute as PickerAttribute).utility) {
				if (!string.IsNullOrEmpty (label.text)) {
					EditorGUI.LabelField (position, label);
					position.x += EditorGUIUtility.labelWidth;

                    position.width = Screen.width - position.x - EditorStyles.inspectorDefaultMargins.padding.right; //- 18 * 2);
				}
				if (GUI.Button (position, current != null ? current.Name : "Null", EditorStyles.objectField)) {
					string searchString = "Search...";
					UtilityInstanceWindow.ShowWindow (typeof(T).Name + " Picker ("+this.Database.name+")", delegate() {
						searchString = EditorTools.SearchField (searchString);
						for (int i = 0; i < Items.Count; i++) {
							if (!string.IsNullOrEmpty (searchString) && !searchString.Equals ("Search...") && !Items [i].Name.ToLower ().Contains (searchString.ToLower ())) {
								continue;
							}
							Color color = GUI.backgroundColor;
							GUI.backgroundColor = current != null && current.Name == Items [i].Name ? Color.green : color;
							if (GUILayout.Button (Items [i].Name)) {
                                SetValue (Items [i], property);
                                UtilityInstanceWindow.CloseWindow ();
							}
							GUI.backgroundColor = color;
						}
					});
				}
			} else {
				int selectedIndex = Items.IndexOf (current);
				selectedIndex = Mathf.Clamp (selectedIndex, 0, Items.Count);
				// selectedIndex = EditorGUI.Popup(position, selectedIndex, Names);
				selectedIndex = EditorGUI.Popup (position, System.Text.RegularExpressions.Regex.Replace (typeof(T).Name, "([a-z])_?([A-Z])", "$1 $2"), selectedIndex, Names);
                SetValue (Items [selectedIndex], property);
			}
		}

		protected void SetValue (T value, SerializedProperty property)
		{
			if (typeof(IEnumerable).IsAssignableFrom (fieldInfo.FieldType)) {
				int currentIndex = System.Convert.ToInt32 (System.Text.RegularExpressions.Regex.Match (property.propertyPath, @"(\d+)(?!.*\d)").Value);
				IList array = (IList)fieldInfo.GetValue (GetParent (property));
				array [currentIndex] = value;
				fieldInfo.SetValue (GetParent (property), array);
			} else {
                fieldInfo.SetValue(GetParent(property), value);
			}

            EditorUtility.SetDirty(property.serializedObject.targetObject);
            var prefabStage = UnityEditor.Experimental.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
            if (prefabStage != null)
            {
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(prefabStage.scene);
            }
        }


		public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
		{
            if (Database == null || (Items.Count == 0 )) {
				return 30 + EditorGUIUtility.singleLineHeight + 2;
			} 
			return base.GetPropertyHeight (property, label);
		}
	}
}
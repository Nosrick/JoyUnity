using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace DevionGames.InventorySystem
{
    [CustomPropertyDrawer(typeof(VisibleItem.Attachment),true)]
    public class AttachmentPropertyDrawer : PropertyDrawer
    {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position, property, label, true);
            if (property.isExpanded)
            {
                SerializedProperty region = property.FindPropertyRelative("region");
				SerializedProperty prefab = property.FindPropertyRelative("prefab");
				SerializedProperty pos = property.FindPropertyRelative("position");
				SerializedProperty rotation = property.FindPropertyRelative("rotation");
				SerializedProperty scale = property.FindPropertyRelative("scale");
				SerializedProperty gameObject = property.FindPropertyRelative("gameObject");
                EditorGUI.BeginDisabledGroup(region.objectReferenceValue == null || prefab.objectReferenceValue == null);
                
				if (GUI.Button(new Rect(position.xMin + 30f, position.yMax - 20f, position.width - 30f, 20f),gameObject.objectReferenceValue != null?"Remove Prefab Handle":"Attach Prefab Handle"))
                {
					if (gameObject.objectReferenceValue != null) {
						GameObject.DestroyImmediate(gameObject.objectReferenceValue);
						return;
					}

					VisibleItem visibleItem=GetParent(property) as VisibleItem;
					EquipmentHandler handler = visibleItem.GetComponent<EquipmentHandler>();
					EquipmentHandler.EquipmentBone bone = handler.Bones.Find(x => x.region == region.objectReferenceValue);
					if (bone != null) {
						GameObject go=(GameObject)GameObject.Instantiate(prefab.objectReferenceValue,bone.bone.transform);
						go.transform.localPosition = pos.vector3Value;
						go.transform.localEulerAngles = rotation.vector3Value;
						go.transform.localScale = scale.vector3Value;
						gameObject.objectReferenceValue = go;
					}
                }
                EditorGUI.EndDisabledGroup();

				if (gameObject.objectReferenceValue != null) {
					Transform transform = (gameObject.objectReferenceValue as GameObject).transform;
					pos.vector3Value = transform.localPosition;
					rotation.vector3Value = transform.localEulerAngles;
					scale.vector3Value = transform.localScale;
				}
              
            }
        }

		public object GetParent(SerializedProperty prop)
		{
			var path = prop.propertyPath.Replace(".Array.data[", "[");
			object obj = prop.serializedObject.targetObject;
			var elements = path.Split('.');
			foreach (var element in elements.Take(elements.Length - 1))
			{
				if (element.Contains("["))
				{
					var elementName = element.Substring(0, element.IndexOf("["));
					var index = Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
					obj = GetValue(obj, elementName, index);
				}
				else
				{
					obj = GetValue(obj, element);
				}
			}
			return obj;
		}

		public object GetValue(object source, string name)
		{
			if (source == null)
				return null;
			var type = source.GetType();
			var f = type.GetSerializedField(name);
			if (f == null)
			{
				var p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
				if (p == null)
					return null;
				return p.GetValue(source, null);
			}
			return f.GetValue(source);
		}

		public object GetValue(object source, string name, int index)
		{
			var enumerable = GetValue(source, name) as IEnumerable;
			var enm = enumerable.GetEnumerator();
			while (index-- >= 0)
				enm.MoveNext();

			return enm.Current;
		}


		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.isExpanded)
                return EditorGUI.GetPropertyHeight(property) + 20f;
            return EditorGUI.GetPropertyHeight(property);
        }
    }
}
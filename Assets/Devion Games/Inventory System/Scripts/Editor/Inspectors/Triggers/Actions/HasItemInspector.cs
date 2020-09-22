using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace DevionGames.InventorySystem
{
    [CustomEditor(typeof(HasItem))]
    public class HasItemInspector : Editor
    {
        private SerializedProperty m_Script;
        private SerializedProperty m_RequiredItems;
        private ReorderableList m_RequiredItemList;


        private void OnEnable()
        {
        
            this.m_Script = serializedObject.FindProperty("m_Script");
            this.m_RequiredItems = serializedObject.FindProperty("requiredItems");
            this.m_RequiredItemList = new ReorderableList(serializedObject, this.m_RequiredItems, true, true, true, true);
            this.m_RequiredItemList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
                var element = this.m_RequiredItemList.serializedProperty.GetArrayElementAtIndex(index);
                SerializedProperty itemProperty = element.FindPropertyRelative("item");
                rect.y += 2;
                rect.height = EditorGUIUtility.singleLineHeight;
                rect.width *= 0.5f;
                EditorGUI.PropertyField(rect, itemProperty, GUIContent.none);
                rect.x += rect.width + 5;
                rect.width -= 5f;
                SerializedProperty window = element.FindPropertyRelative("stringValue");
                if (InventorySystemEditor.Database == null || InventorySystemEditor.Database.items.Count == 0)
                {
                    rect.y += (9 + EditorGUIUtility.singleLineHeight + 6);
                }
                EditorGUI.PropertyField(rect, window, GUIContent.none);
            };
            this.m_RequiredItemList.drawHeaderCallback = (Rect rect) => {
                EditorGUI.LabelField(rect, "Required Items(Item, Window)");
            };
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(this.m_Script);
            EditorGUI.EndDisabledGroup();

            serializedObject.Update();
            GUILayout.BeginHorizontal();
            GUILayout.Space(16f);
            GUILayout.BeginVertical();
            EditorGUI.indentLevel -= 1;
            this.m_RequiredItemList.elementHeight = (InventorySystemEditor.Database != null && InventorySystemEditor.Database.items.Count > 0 || m_RequiredItemList.count == 0 ? 21 : (30 + EditorGUIUtility.singleLineHeight + 4));
            this.m_RequiredItemList.DoLayoutList();
            EditorGUI.indentLevel  += 1;
            GUILayout.EndVertical();
            GUILayout.Space(3f);
            GUILayout.EndHorizontal();
            serializedObject.ApplyModifiedProperties();

        }
    }
}
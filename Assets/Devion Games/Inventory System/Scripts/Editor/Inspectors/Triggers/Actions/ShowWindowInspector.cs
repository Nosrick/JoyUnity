using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DevionGames.InventorySystem
{
    [CustomEditor(typeof(ShowWindow))]
    public class ShowWindowInspector : Editor
    {
        private SerializedProperty m_Script;
        private SerializedProperty m_DestroyWhenEmpty;

        private void OnEnable()
        {
            this.m_Script = serializedObject.FindProperty("m_Script");
            this.m_DestroyWhenEmpty = serializedObject.FindProperty("m_DestroyWhenEmpty");
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(this.m_Script);
            EditorGUI.EndDisabledGroup();

            serializedObject.Update();
            DrawPropertiesExcluding(serializedObject,this.m_Script.propertyPath,this.m_DestroyWhenEmpty.propertyPath);
            bool hasCollection = (target as ShowWindow).gameObject.GetComponent<ItemCollection>() != null;
            if (!hasCollection)
            {
                this.m_DestroyWhenEmpty.boolValue = false;
                EditorGUILayout.HelpBox("You can only destroy triggers with an ItemCollection.", MessageType.Info);
            }
            EditorGUI.BeginDisabledGroup(!hasCollection);
            EditorGUILayout.PropertyField(this.m_DestroyWhenEmpty);
            EditorGUI.EndDisabledGroup();
            serializedObject.ApplyModifiedProperties();
        }
    }
}
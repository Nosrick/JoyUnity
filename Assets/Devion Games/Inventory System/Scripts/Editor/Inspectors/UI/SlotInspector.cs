using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DevionGames.InventorySystem
{
    [CustomEditor(typeof(Slot),true)]
    public class SlotInspector : Editor
    {
        protected virtual void OnEnable() {

        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (UnityEditorUtility.RightArrowButton(new GUIContent("Restrictions", "Slot restrictions")))
            {
                AssetWindow.ShowWindow("Slot Restrictions", serializedObject.FindProperty("restrictions"));
            }
            if (EditorWindow.mouseOverWindow != null)
            {
                EditorWindow.mouseOverWindow.Repaint();
            }
        }
    }
}
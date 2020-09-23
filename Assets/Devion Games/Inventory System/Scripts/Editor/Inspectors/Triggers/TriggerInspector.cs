using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine.Events;
using UnityEditorInternal;
using System;
using System.Linq;
using System.IO;
using System.Reflection;

namespace DevionGames.InventorySystem
{
    [CustomEditor(typeof(Trigger),true)]
    public class TriggerInspector : CallbackHandlerInspector
    {
       // private SerializedProperty m_Script;
        private SerializedProperty m_UseDistance;
        private SerializedProperty m_TriggerInputType;
        private SerializedProperty m_TriggerKey;
        private AnimBool m_KeyOptions;

        private SerializedProperty m_LockContainers;
        private SerializedProperty m_UseCustomCursor;
        private SerializedProperty m_CursorSprite;
        private SerializedProperty m_CursorSize;
        private SerializedProperty m_CursorAnimatorState;
        private AnimBool m_CursorOptions;
        private GameObject gameObject;

        protected override void OnEnable()
        {
            base.OnEnable();
            gameObject = (target as Trigger).gameObject;

            //this.m_Script = serializedObject.FindProperty("m_Script");
            this.m_LockContainers = serializedObject.FindProperty("m_LockContainersWhenUsed");
            this.m_UseDistance = serializedObject.FindProperty("useDistance");
            this.m_TriggerInputType = serializedObject.FindProperty("triggerType");
            this.m_TriggerKey = serializedObject.FindProperty("key");
            if (this.m_KeyOptions == null)
            {
                this.m_KeyOptions = new AnimBool((target as Trigger).triggerType.HasFlag<Trigger.TriggerInputType>(Trigger.TriggerInputType.Key));
                this.m_KeyOptions.valueChanged.AddListener(new UnityAction(Repaint));
            }

            this.m_UseCustomCursor = serializedObject.FindProperty("useCustomCursor");
            this.m_CursorSprite = serializedObject.FindProperty("m_CursorSprite");
            this.m_CursorSize = serializedObject.FindProperty("m_CursorSize");
            this.m_CursorAnimatorState = serializedObject.FindProperty("m_CursorAnimatorState");
            if (this.m_CursorOptions == null){
                this.m_CursorOptions = new AnimBool(this.m_UseCustomCursor.boolValue);
                this.m_CursorOptions.valueChanged.AddListener(new UnityAction(Repaint));
            }

            Trigger trigger = target as Trigger;
            for (int i = 0; i < trigger.actions.Count; i++)
            {
                if (trigger.actions[i] != null)
                {
                     trigger.actions[i].hideFlags = EditorPrefs.GetBool("InventorySystem.showAllComponents") ? HideFlags.None : HideFlags.HideInInspector;
                    
                    if (trigger.actions[i].gameObject != gameObject)
                    {
                        if (ComponentUtility.CopyComponent(trigger.actions[i]))
                        {
                            TriggerAction component = gameObject.AddComponent(trigger.actions[i].GetType()) as TriggerAction;
                            ComponentUtility.PasteComponentValues(component);
                            trigger.actions[i] = component;
                        }
                    }
                }
            }

            EditorApplication.playModeStateChanged += PlayModeState;
        }

        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= PlayModeState;
        }

        bool playModeStateChange;
        private void PlayModeState(PlayModeStateChange state)
        {
            playModeStateChange =( state == PlayModeStateChange.ExitingEditMode || state == PlayModeStateChange.EnteredEditMode || state == PlayModeStateChange.ExitingPlayMode);
        }

        private void OnDestroy()
        {
            if (gameObject != null && target == null && !playModeStateChange)
            {
                TriggerAction[] actions = gameObject.GetComponents<TriggerAction>();
                for (int i = 0; i < actions.Length; i++)
                {
                    DestroyImmediate(actions[i]);
                }
                Debug.LogWarning("Trigger component removed, cleaning up actions.");
            }
        }

        [UnityEditor.Callbacks.DidReloadScripts]
        private static void OnScriptsReloaded()
        {
            TriggerInspector[] triggerInspectors = Resources.FindObjectsOfTypeAll<TriggerInspector>();
            for (int j = 0; j < triggerInspectors.Length; j++)
            {
                Trigger trigger = triggerInspectors[j].target as Trigger;
                for (int i = 0; i < trigger.actions.Count; i++)
                {
                    if (trigger.actions[i] != null)
                    {
                        trigger.actions[i].hideFlags = HideFlags.None;
                    }
                }
            }
        }

        private void DrawInspector()
        {
        
            EditorGUILayout.PropertyField(this.m_UseDistance);
            EditorGUILayout.PropertyField(this.m_TriggerInputType);

            this.m_KeyOptions.target = (target as Trigger).triggerType.HasFlag<Trigger.TriggerInputType>(Trigger.TriggerInputType.Key);
            if (EditorGUILayout.BeginFadeGroup(this.m_KeyOptions.faded))
            {
                EditorGUI.indentLevel = EditorGUI.indentLevel + 1;
                EditorGUILayout.PropertyField(this.m_TriggerKey);
                EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
            }
            EditorGUILayout.EndFadeGroup();

            EditorGUILayout.PropertyField(this.m_UseCustomCursor, new GUIContent("Custom Cursor",(this.m_UseCustomCursor.boolValue?"Disable":"Enable")+" the custom cursor to show when pointer is over trigger."));
            this.m_CursorOptions.target = this.m_UseCustomCursor.boolValue;
            if (EditorGUILayout.BeginFadeGroup(this.m_CursorOptions.faded))
            {
                EditorGUI.indentLevel = EditorGUI.indentLevel + 1;
                EditorGUILayout.PropertyField(this.m_CursorSprite, new GUIContent("Sprite","The cursor image to show."));
                EditorGUILayout.PropertyField(this.m_CursorSize, new GUIContent("Size","Size of the cursor image."));
                EditorGUILayout.PropertyField(this.m_CursorAnimatorState, new GUIContent("Animator State", "The animator state name, leave empty for none."));
                EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
            }
            EditorGUILayout.EndFadeGroup();

            EditorGUILayout.PropertyField(this.m_LockContainers);
            serializedObject.ApplyModifiedProperties();

            if (UnityEditorUtility.RightArrowButton(new GUIContent("Edit Behavior", "Trigger use behavior"), GUILayout.Height(24f)))
            {
                AssetWindow.ShowWindow("Trigger Actions", serializedObject.FindProperty("actions"));
            }

            if (EditorWindow.mouseOverWindow != null){
                EditorWindow.mouseOverWindow.Repaint();
            }

            serializedObject.Update();
        }

        protected void DrawChildProperties<T>(SerializedObject obj)
        {
            string[] propertiesToExclude = typeof(T).GetAllSerializedFields().Select(x=>x.Name).ToArray();
            ArrayUtility.Add(ref propertiesToExclude, "m_Script");

            SerializedProperty iterator = obj.GetIterator();
            bool flag = true;
            while (iterator.NextVisible(flag))
            {
                flag = false;

                if (!propertiesToExclude.Contains<string>(iterator.name))
                {
                    EditorGUILayout.PropertyField(iterator, true, new GUILayoutOption[0]);
                }
            }
        }

        protected virtual void OnSceneGUI()
        {
            Trigger trigger = (Trigger)target;
            Vector3 position = trigger.transform.position;

            Collider collider = trigger.GetComponent<Collider>();      
            if (collider != null) {
       
               position = collider.bounds.center;
               position.y =  (collider.bounds.center.y-collider.bounds.extents.y);
            }

            Color color = Handles.color;
            Color green = Color.green;
            green.a = 0.05f;
            Handles.color = green;
            Handles.DrawSolidDisc(position, Vector3.up, trigger.useDistance);
            Handles.color = Color.white;
            Handles.DrawWireDisc(position, Vector3.up, trigger.useDistance);
            Handles.color = color;
        }

    }
}
 
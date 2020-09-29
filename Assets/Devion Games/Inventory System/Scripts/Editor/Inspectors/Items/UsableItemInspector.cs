using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using DevionGames.InventorySystem.ItemActions;
using System;
using System.Linq;
using System.IO;
using System.Reflection;
using UnityEditor.AnimatedValues;
using UnityEngine.Events;

namespace DevionGames.InventorySystem
{
    [CustomEditor(typeof(UsableItem), true)]
    public class UsableItemInspector : ItemInspector
    {
        private SerializedProperty m_UseCategoryCooldown;
        private SerializedProperty m_Cooldown;
        protected AnimBool m_ShowCategoryCooldownOptions;

        private GameObject m_GameObject;
        private int m_ComponentInstanceID;
        private UnityEngine.Object m_Target;
        private IList m_List;
        private string m_FieldName;
        private string m_ElementTypeName;
        private Type m_ElementType;
        private static object m_ObjectToCopy;

        private SerializedProperty m_Actions;

        protected override void OnEnable()
        {
            base.OnEnable();

            ArrayUtility.Add(ref this.m_PropertiesToExcludeForChildClasses, serializedObject.FindProperty("actions").propertyPath);
            this.m_UseCategoryCooldown = serializedObject.FindProperty("m_UseCategoryCooldown");
            this.m_Cooldown = serializedObject.FindProperty("m_Cooldown");
            this.m_ShowCategoryCooldownOptions = new AnimBool(!this.m_UseCategoryCooldown.boolValue);

            this.m_ShowCategoryCooldownOptions.valueChanged.AddListener(new UnityAction(this.Repaint));
            
            this.m_Target = target;
            this.m_List = (target as UsableItem).actions;
            this.m_ElementType = Utility.GetElementType(this.m_List.GetType());
            this.m_ElementTypeName = this.m_ElementType.FullName;
            FieldInfo[] fields = this.m_Target.GetType().GetSerializedFields();
            for (int i = 0; i < fields.Length; i++)
            {
                object temp = fields[i].GetValue(this.m_Target);
                if (temp == this.m_List)
                    this.m_FieldName = fields[i].Name;
            }
            m_Actions = serializedObject.FindProperty("actions");


            AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
            AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
            EditorApplication.playModeStateChanged += OnPlaymodeStateChange;

        }

        protected override void OnDisable() {
            AssemblyReloadEvents.beforeAssemblyReload -= OnBeforeAssemblyReload;
            AssemblyReloadEvents.afterAssemblyReload -= OnAfterAssemblyReload;
            EditorApplication.playModeStateChanged -= OnPlaymodeStateChange;
        }



        public override void OnInspectorGUI()
        {
            ScriptGUI();
            serializedObject.Update();
            DrawBaseInspector();
            for (int i = 0; i < m_DrawInspectors.Count; i++)
            {
                this.m_DrawInspectors[i].Invoke();
            }
         
            DrawPropertiesExcluding(serializedObject, this.m_PropertiesToExcludeForChildClasses);
            ActionGUI();
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawInspector() {
            EditorGUILayout.PropertyField(this.m_UseCategoryCooldown);
            this.m_ShowCategoryCooldownOptions.target = !this.m_UseCategoryCooldown.boolValue;
            if (EditorGUILayout.BeginFadeGroup(this.m_ShowCategoryCooldownOptions.faded))
            {
                EditorGUI.indentLevel = EditorGUI.indentLevel + 1;
                EditorGUILayout.PropertyField(this.m_Cooldown);
                EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
            }
            EditorGUILayout.EndFadeGroup();
        }


        private void ActionGUI() {
           
            GUILayout.Space(10f);

            for (int i = 0; i < this.m_List.Count; i++)
            {
                object value = this.m_List[i];
                EditorGUI.BeginChangeCheck();
                if (this.m_Target != null)
                    Undo.RecordObject(this.m_Target, "Inspector");


                if (EditorTools.Titlebar(value, ElementContextMenu(this.m_List, i)))
                {
                    EditorGUI.indentLevel += 1;
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.ObjectField("Script", EditorTools.FindMonoScript(value.GetType()), typeof(MonoScript), true);
                    EditorGUI.EndDisabledGroup();
                    SerializedProperty element = this.m_Actions.GetArrayElementAtIndex(i);
                    if (HasCustomPropertyDrawer(value.GetType()))
                    {
                        EditorGUILayout.PropertyField(element, true);
                    }
                    else
                    {
                         while (element.NextVisible(true))
                         {
                             if (element.propertyPath.Contains("actions.Array.data[" + i + "]") && !element.propertyPath.Replace("actions.Array.data[" + i + "].", "").Contains(".") )
                             {

                                if (this.m_List[i].GetType().GetSerializedField(element.propertyPath.Split('.').Last()).FieldType == typeof(TargetType))
                                {
                                    element.enumValueIndex = 1;
                                    EditorGUI.BeginDisabledGroup(true);
                                    EditorGUILayout.PropertyField(element, true);
                                    EditorGUI.EndDisabledGroup();
                                }
                                else
                                {

                                    EditorGUILayout.PropertyField(element, true);
                                }
                             }
                         }
                    }

                    EditorGUI.indentLevel -= 1;
                }
                if (EditorGUI.EndChangeCheck())
                    EditorUtility.SetDirty(this.m_Target);
            }
            GUILayout.FlexibleSpace();
            DoAddButton();
            GUILayout.Space(10f);

        }

        private bool HasCustomPropertyDrawer(Type type) {
            foreach (Type typesDerivedFrom in TypeCache.GetTypesDerivedFrom<GUIDrawer>())
            {
                object[] customAttributes = typesDerivedFrom.GetCustomAttributes<CustomPropertyDrawer>();
                for (int i = 0; i < (int)customAttributes.Length; i++)
                {
                    CustomPropertyDrawer customPropertyDrawer = (CustomPropertyDrawer)customAttributes[i];
                    FieldInfo field=customPropertyDrawer.GetType().GetField("m_Type",BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    Type type1 =(Type) field.GetValue(customPropertyDrawer);
                    if (type == type1)
                        return true;
                }
            }
                    
            return false;
        }

        private void Add(Type type)
        {
            object value = System.Activator.CreateInstance(type);
            serializedObject.Update();
            this.m_Actions.arraySize++;
            this.m_Actions.GetArrayElementAtIndex(this.m_Actions.arraySize - 1).managedReferenceValue = value;
            serializedObject.ApplyModifiedProperties();
        }

        private void CreateScript(string scriptName)
        {
          
        }

        private void DoAddButton()
        {
            GUIStyle buttonStyle = new GUIStyle("AC Button");
            GUIContent buttonContent = new GUIContent("Add " + this.m_ElementType.Name);
            Rect buttonRect = GUILayoutUtility.GetRect(buttonContent, buttonStyle, GUILayout.ExpandWidth(true));
            buttonRect.x = buttonRect.width * 0.5f - buttonStyle.fixedWidth * 0.5f;
            buttonRect.width = buttonStyle.fixedWidth;
            if (GUI.Button(buttonRect, buttonContent, buttonStyle))
            {
                AddObjectWindow.ShowWindow(buttonRect, this.m_ElementType, Add, CreateScript);
            }
        }

        private void OnPlaymodeStateChange(PlayModeStateChange stateChange)
        {
            Reload();
        }

        public void OnBeforeAssemblyReload()
        {
            /*this.m_ElementTypeName = this.m_ElementType.Name;
            FieldInfo[] fields = this.m_Target.GetType().GetSerializedFields();
            for (int i = 0; i < fields.Length; i++)
            {
                object temp = fields[i].GetValue(this.m_Target);
                if (temp == this.m_List)
                    this.m_FieldName = fields[i].Name;
            }*/
            if (this.m_Target is Component)
            {
                this.m_GameObject = (this.m_Target as Component).gameObject;
                this.m_ComponentInstanceID = (this.m_Target as Component).GetInstanceID();
            }
        }


        public void OnAfterAssemblyReload()
        {
            Reload();
        }

        private GenericMenu ElementContextMenu(IList list, int index)
        {
            GenericMenu menu = new GenericMenu();

            menu.AddItem(new GUIContent("Reset"), false, delegate {
                object value = System.Activator.CreateInstance(list[index].GetType());
                list[index] = value;
            });
            menu.AddSeparator(string.Empty);
            menu.AddItem(new GUIContent("Remove " + this.m_ElementType.Name), false, delegate { list.RemoveAt(index); });

            if (index > 0)
            {
                menu.AddItem(new GUIContent("Move Up"), false, delegate {
                    object value = list[index];
                    list.RemoveAt(index);
                    list.Insert(index - 1, value);
                });
            }
            else
            {
                menu.AddDisabledItem(new GUIContent("Move Up"));
            }

            if (index < list.Count - 1)
            {
                menu.AddItem(new GUIContent("Move Down"), false, delegate
                {
                    object value = list[index];
                    list.RemoveAt(index);
                    list.Insert(index + 1, value);
                });
            }
            else
            {
                menu.AddDisabledItem(new GUIContent("Move Down"));
            }

            menu.AddItem(new GUIContent("Copy " + this.m_ElementType.Name), false, delegate {
                object value = list[index];
                m_ObjectToCopy = value;
            });

            if (m_ObjectToCopy != null)
            {
                menu.AddItem(new GUIContent("Paste " + this.m_ElementType.Name + " As New"), false, delegate {
                    object instance = System.Activator.CreateInstance(m_ObjectToCopy.GetType());
                    FieldInfo[] fields = instance.GetType().GetSerializedFields();
                    for (int i = 0; i < fields.Length; i++)
                    {
                        object value = fields[i].GetValue(m_ObjectToCopy);
                        fields[i].SetValue(instance, value);
                    }
                    list.Insert(index + 1, instance);
                });

                if (list[index].GetType() == m_ObjectToCopy.GetType())
                {
                    menu.AddItem(new GUIContent("Paste " + this.m_ElementType.Name + " Values"), false, delegate
                    {
                        object instance = this.m_List[index];
                        FieldInfo[] fields = instance.GetType().GetSerializedFields();
                        for (int i = 0; i < fields.Length; i++)
                        {
                            object value = fields[i].GetValue(m_ObjectToCopy);
                            fields[i].SetValue(instance, value);
                        }
                    });
                }
                else
                {
                    menu.AddDisabledItem(new GUIContent("Paste " + this.m_ElementType.Name + " Values"));
                }
            }

            MonoScript script = EditorTools.FindMonoScript(list[index].GetType());
            if (script != null)
            {
                menu.AddSeparator(string.Empty);
                menu.AddItem(new GUIContent("Edit Script"), false, delegate { AssetDatabase.OpenAsset(script); });
            }
            return menu;
        }

        private void Reload()
        {
            if (this.m_GameObject != null)
            {
                Component[] components = this.m_GameObject.GetComponents(typeof(Component));
                this.m_Target = Array.Find(components, x => x.GetInstanceID() == this.m_ComponentInstanceID);
            }

            this.m_ElementType = Utility.GetType(this.m_ElementTypeName);
            this.m_List = this.m_Target.GetType().GetSerializedField(this.m_FieldName).GetValue(this.m_Target) as IList;
        }   
    }
}

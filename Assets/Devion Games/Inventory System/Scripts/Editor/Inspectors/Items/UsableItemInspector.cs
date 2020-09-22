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
        private List<Editor> m_Editors = new List<Editor>();
        private SerializedProperty m_Actions;
        private SerializedProperty m_UseCategoryCooldown;
        private SerializedProperty m_Cooldown;
        protected AnimBool m_ShowCategoryCooldownOptions;

        private static ItemAction m_CopyItemAction;

        protected override void OnEnable()
        {
            base.OnEnable();
            if (target == null) {
                return;
            }
            this.m_Actions = serializedObject.FindProperty("actions");
            for (int i = this.m_Editors.Count - 1; i >= 0; i--)
            {
                DestroyImmediate(this.m_Editors[i]);
            }
            this.m_Editors.Clear();
            for (int i = 0; i < this.m_Actions.arraySize; i++)
            {
                Editor editor = Editor.CreateEditor(this.m_Actions.GetArrayElementAtIndex(i).objectReferenceValue);
               m_Editors.Add(editor);
            }
            ArrayUtility.Add(ref this.m_PropertiesToExcludeForChildClasses, this.m_Actions.propertyPath);
            this.m_UseCategoryCooldown = serializedObject.FindProperty("m_UseCategoryCooldown");
            this.m_Cooldown = serializedObject.FindProperty("m_Cooldown");
            this.m_ShowCategoryCooldownOptions = new AnimBool(!this.m_UseCategoryCooldown.boolValue);
            if (InventorySystemEditor.instance != null)
            {
                this.m_ShowCategoryCooldownOptions.valueChanged.AddListener(new UnityAction(InventorySystemEditor.instance.Repaint));
            }
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
            UsableItem item = (UsableItem)target;
            if (this.m_Actions.arraySize > 0)
                GUILayout.Label("Use Actions:", EditorStyles.boldLabel);

            for (int i = 0; i < item.actions.Count; i++)
            {
                ItemAction target = item.actions[i];
                Editor editor = this.m_Editors[i];

                if (UnityEditorUtility.Titlebar(target, GetContextMenu(item, target)))
                {
                    EditorGUI.indentLevel += 1;
                    editor.OnInspectorGUI();
                    EditorGUI.indentLevel -= 1;
                }
            }

            GUIStyle buttonStyle = new GUIStyle("AC Button");
            Rect buttonRect = GUILayoutUtility.GetRect(new GUIContent("Add Behavior"), buttonStyle, GUILayout.ExpandWidth(true));
            buttonRect.x = buttonRect.width * 0.5f - buttonStyle.fixedWidth * 0.5f;
            buttonRect.width = buttonStyle.fixedWidth;
            if (GUI.Button(buttonRect, "Add Action", buttonStyle))
            {
                AddAssetWindow.ShowWindow(buttonRect, typeof(ItemAction), AddAsset, CreateScript);
            }
        }

        private void AddAsset(Type type)
        {
            ItemAction action = (ItemAction)ScriptableObject.CreateInstance(type);
            action.hideFlags = HideFlags.HideInHierarchy;
            AssetDatabase.AddObjectToAsset(action, target);
            AssetDatabase.SaveAssets();

            Editor editor = Editor.CreateEditor(action);
            this.m_Editors.Add(editor);

            serializedObject.Update();
            this.m_Actions.arraySize++;
            this.m_Actions.GetArrayElementAtIndex(this.m_Actions.arraySize - 1).objectReferenceValue = action;
            serializedObject.ApplyModifiedProperties();

        }

        private void CreateScript(string scriptName)
        {
            scriptName = scriptName.Replace(" ", "_");
            scriptName = scriptName.Replace("-", "_");
            string path = "Assets/" + scriptName + ".cs";
            Type elementType = typeof(ItemAction);
            if (File.Exists(path) == false)
            {
                using (StreamWriter outfile = new StreamWriter(path))
                {
                    MethodInfo[] methods = elementType.GetAllMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    methods = methods.Where(x => x.IsAbstract).ToArray();

                    outfile.WriteLine("using UnityEngine;");
                    outfile.WriteLine("using System.Collections;");
                    outfile.WriteLine("");

                    outfile.WriteLine("namespace " + elementType.Namespace + "{");

                    if (!typeof(Component).IsAssignableFrom(elementType))
                    {
                        outfile.WriteLine("\t[System.Serializable]");
                    }
                    outfile.WriteLine("\tpublic class " + scriptName + " : " + elementType.Name + "{");
                    for (int i = 0; i < methods.Length; i++)
                    {
                        MethodInfo method = methods[i];
                        ParameterInfo[] parameters = method.GetParameters();
                        string parameterString = string.Empty;
                        for (int j = 0; j < parameters.Length; j++)
                        {
                            string typeName = parameters[j].ParameterType.Name;
                            string parameterName = string.Empty;
                            if (Char.IsLower(typeName, 0))
                            {
                                parameterName = "_" + typeName;
                            }
                            else
                            {
                                parameterName = char.ToLowerInvariant(typeName[0]) + typeName.Substring(1);
                            }
                            parameterString += ", " + typeName + " " + parameterName;
                        }

                        if (!string.IsNullOrEmpty(parameterString))
                        {
                            parameterString = parameterString.Substring(1);
                        }

                        outfile.WriteLine("\t\t" + (method.IsPublic ? "public" : "protected") + " override " + UnityEditorUtility.CovertToAliasString(method.ReturnType) + " " + method.Name + "(" + parameterString + ") {");

                        if (method.ReturnType == typeof(string))
                        {
                            outfile.WriteLine("\t\t\treturn string.Empty;");
                        }
                        else if (method.ReturnType == typeof(bool))
                        {
                            outfile.WriteLine("\t\t\treturn true;");
                        }
                        else if (method.ReturnType == typeof(Vector2))
                        {
                            outfile.WriteLine("\t\t\treturn Vector2.zero;");
                        }
                        else if (method.ReturnType == typeof(Vector3))
                        {
                            outfile.WriteLine("\t\t\treturn Vector3.zero;");
                        }
                        else if (!method.ReturnType.IsValueType || method.ReturnType.IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(Nullable<>))
                        {
                            outfile.WriteLine("\t\t\treturn null;");
                        }
                        else if (UnityUtility.IsInteger(method.ReturnType))
                        {
                            outfile.WriteLine("\t\t\treturn 0;");
                        }
                        else if (UnityUtility.IsFloat(method.ReturnType))
                        {
                            outfile.WriteLine("\t\t\treturn 0.0f;");
                        }
                        else if (method.ReturnType.IsEnum)
                        {
                            outfile.WriteLine("\t\t\treturn " + method.ReturnType.Name + "." + Enum.GetNames(method.ReturnType)[0] + ";");
                        }

                        outfile.WriteLine("\t\t}");
                        outfile.WriteLine("");
                    }
                    outfile.WriteLine("\t}");
                    outfile.WriteLine("}");
                }
            }
            AssetDatabase.Refresh();
            EditorPrefs.SetString("NewScriptToCreate", scriptName);
            EditorPrefs.SetInt("AssetWindowID", GetInstanceID());
        }

        [UnityEditor.Callbacks.DidReloadScripts]
        private static void OnScriptsReloaded()
        {
            string scriptName = EditorPrefs.GetString("NewScriptToCreate");
            int windowID = EditorPrefs.GetInt("AssetWindowID");

            Type type = TypeUtility.GetType(scriptName);
            if (!EditorApplication.isPlayingOrWillChangePlaymode && !string.IsNullOrEmpty(scriptName) && type != null)
            {
                UsableItemInspector[] windows = Resources.FindObjectsOfTypeAll<UsableItemInspector>();
                UsableItemInspector window = windows.Where(x => x.GetInstanceID() == windowID).FirstOrDefault();
                if (window != null)
                {
                    window.AddAsset(type);
                }

            }
            EditorPrefs.DeleteKey("NewScriptToCreate");
            EditorPrefs.DeleteKey("AssetWindowID");
        }

        private GenericMenu GetContextMenu(UsableItem item, ItemAction target)
        {
            
            GenericMenu menu = new GenericMenu();
            int index = item.actions.IndexOf(target);
            menu.AddItem(new GUIContent("Reset"), false, delegate {
                Type type = target.GetType();
                DestroyImmediate(this.m_Editors[index]);
                DestroyImmediate(target, true);

                ItemAction action = (ItemAction)ScriptableObject.CreateInstance(type);
                action.hideFlags = HideFlags.HideInHierarchy;
                AssetDatabase.AddObjectToAsset(action, item);
                AssetDatabase.SaveAssets();

                Editor editor = Editor.CreateEditor(action);
                this.m_Editors[index]=editor;


                serializedObject.Update();
                this.m_Actions.GetArrayElementAtIndex(index).objectReferenceValue = action;
                serializedObject.ApplyModifiedProperties();

                
            });

            menu.AddSeparator(string.Empty);
            menu.AddItem(new GUIContent("Remove"), false, delegate {
                DestroyImmediate(this.m_Editors[index]);
                this.m_Editors.RemoveAt(index);
                DestroyImmediate(target, true);
  
                serializedObject.Update();
                this.m_Actions.GetArrayElementAtIndex(index).objectReferenceValue = null;
                this.m_Actions.DeleteArrayElementAtIndex(index);
                serializedObject.ApplyModifiedProperties();
            });

            menu.AddItem(new GUIContent("Copy"), false, delegate {
                m_CopyItemAction = target;
            });

            if (m_CopyItemAction != null && m_CopyItemAction.GetType() == target.GetType())
            {
                menu.AddItem(new GUIContent("Paste"), false, delegate {

                   
                });
            }
            else
            {
                menu.AddDisabledItem(new GUIContent("Paste"));
            }

            if (index > 0)
            {
                menu.AddItem(new GUIContent("Move Up"), false, delegate
                {
                    Editor editor = this.m_Editors[index];
                    this.m_Editors.RemoveAt(index);
                    this.m_Editors.Insert(index - 1, editor);

                    serializedObject.Update();
                    this.m_Actions.MoveArrayElement(index, index - 1);
                    serializedObject.ApplyModifiedProperties();
                   
                });
            }
            else
            {
                menu.AddDisabledItem(new GUIContent("Move Up"));
            }
            if (index < this.m_Actions.arraySize - 1)
            {
                menu.AddItem(new GUIContent("Move Down"), false, delegate
                {
                   
                    Editor editor = this.m_Editors[index];
                    this.m_Editors.RemoveAt(index);
                    this.m_Editors.Insert(index + 1, editor);

                    serializedObject.Update();
                    this.m_Actions.MoveArrayElement(index, index + 1);
                    serializedObject.ApplyModifiedProperties();
                   
                });
            }
            else
            {
                menu.AddDisabledItem(new GUIContent("Move Down"));
            }
            return menu;
        }

        private void OnDestroy()
        {
            for (int i = this.m_Editors.Count - 1; i >= 0; i--)
            {
                DestroyImmediate(this.m_Editors[i]);
            }
        }
    }
}

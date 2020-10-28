using JoyLib.Code.Entities.AI;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using JoyLib.Code.Helpers;

namespace JoyLib.Code.Scripting
{
    public class ScriptingEngine
    {
        private static readonly Lazy<ScriptingEngine> lazy = new Lazy<ScriptingEngine>(() => new ScriptingEngine());

        public static ScriptingEngine instance => lazy.Value;

        private Assembly m_ScriptDLL;

        private Type[] m_Types;

        private const string ABILITY_NAMESPACE = "JoyLib.Code.Entities.Abilities.";
        private const string NEED_NAMESPACE = "JoyLib.Code.Entities.Needs.";

        public ScriptingEngine()
        {
            try
            {
                string dir = Directory.GetCurrentDirectory() + "/" + GlobalConstants.SCRIPTS_FOLDER;
                string[] scriptFiles = Directory.GetFiles(dir, "*.cs", SearchOption.AllDirectories);

                List<Microsoft.CodeAnalysis.SyntaxTree> builtFiles = new List<Microsoft.CodeAnalysis.SyntaxTree>();

                foreach(string scriptFile in scriptFiles)
                {
                    string contents = File.ReadAllText(scriptFile);
                    SyntaxTree builtFile = CSharpSyntaxTree.ParseText(contents);
                    builtFiles.Add(builtFile);
                }
                Debug.Log("Loaded " + scriptFiles.Length + " script files.");
                List<MetadataReference> libs = new List<MetadataReference>
                {
                    MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(Entities.Entity).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(Vector2Int).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(Queue<bool>).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(System.Linq.IQueryable).Assembly.Location)
                };
                CSharpCompilation compilation = CSharpCompilation.Create("JoyScripts", builtFiles, libs, 
                    new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

                MemoryStream memory = new MemoryStream();
                EmitResult result = compilation.Emit(memory);

                if(result.Success == false)
                {
                    foreach(var diagnostic in result.Diagnostics)
                    {
                        Debug.Log(diagnostic.GetMessage());
                        Debug.Log(diagnostic.Severity.ToString());
                    }
                }

                memory.Seek(0, SeekOrigin.Begin);
                m_ScriptDLL = Assembly.Load(memory.ToArray());

                m_Types = m_ScriptDLL.GetTypes();
            }
            catch(Exception ex)
            {
                Debug.LogError(ex.Message);
                Debug.LogError(ex.StackTrace);
            }
        }

        public Type FetchType(string typeName)
        {
            try
            {
                Type directType = m_Types.Single(type => type.Name.Equals(typeName, StringComparison.OrdinalIgnoreCase));
                return directType;
            }
            catch(Exception ex)
            {
                Debug.LogWarning(ex.Message);
                Debug.LogWarning(ex.StackTrace);
                throw new InvalidOperationException("Error when searching for type, " + typeName);
            }
        }

        public object FetchAndInitialise(string type)
        {
            try
            {
                Type directType = m_Types.First(t => t.Name.Equals(type, StringComparison.OrdinalIgnoreCase));

                return Activator.CreateInstance(directType);
            }
            catch(Exception ex)
            {
                Debug.LogWarning(ex.Message);
                Debug.LogWarning(ex.StackTrace);
                throw new InvalidOperationException("Error when searching for Type in ScriptingEngine, type name " + type.ToString());
            }
        }

        public T[] FetchAndInitialiseChildren<T>()
        {
            try
            {
                Type[] types = m_Types.Where(t => typeof(T).IsAssignableFrom(t) && t.IsAbstract == false).ToArray();
                List<T> children = new List<T>();
                foreach (Type tempType in types)
                {
                    children.Add((T)Activator.CreateInstance(tempType));
                }

                return children.ToArray();
            }
            catch (Exception e)
            {
                Debug.LogWarning(e.Message);
                Debug.LogWarning(e.StackTrace);
                throw new InvalidOperationException("Error when searching for Type in ScriptingEngine, " + typeof(T).Name);
            }
        }

        public Type[] FetchTypeAndChildren(string typeName)
        {
            try
            {
                Type directType = m_Types.First(type => type.Name.Equals(typeName, StringComparison.OrdinalIgnoreCase));

                List<Type> children = new List<Type>();
                if(directType != null)
                {
                    children = m_Types.Where(type => directType.IsAssignableFrom(type)).ToList();
                }
                else
                {
                    directType = Type.GetType(typeName, true, true);
                    children = m_Types.Where(type => directType.IsAssignableFrom(type)).ToList();
                    children = children.Where(t => t.IsAbstract == false && t.IsInterface == false).ToList();
                }
                
                return children.ToArray();
            }
            catch(Exception ex)
            {
                Debug.LogWarning(ex.Message);
                Debug.LogWarning(ex.StackTrace);
                throw new InvalidOperationException("Error when searching for Type in ScriptingEngine, " + typeName);
            }
        }

        public Type[] FetchTypeAndChildren(Type type) 
        {
            try
            {
                Type[] types = m_Types.Where(t => type.IsAssignableFrom(t) && t.IsAbstract == false).ToArray();

                return types;
            }
            catch(Exception e)
            {
                Debug.LogWarning(e.Message);
                Debug.LogWarning(e.StackTrace);
                throw new InvalidOperationException("Error when searching for Type in ScriptingEngine, " + type.FullName);
            }
        }

        public IJoyAction FetchAction(string actionName)
        {
            try
            {
                Type type = m_Types.Single(t => t.Name.Equals(actionName, StringComparison.OrdinalIgnoreCase));

                IJoyAction action = (IJoyAction)Activator.CreateInstance(type);
                return action;
            }
            catch(Exception e)
            {
                Debug.LogWarning(e.Message);
                Debug.LogWarning(e.StackTrace);
                throw new InvalidOperationException("Error when finding action, no such action " + actionName);
            }
        }

        public IJoyAction[] FetchActions(string[] actionNames)
        {
            List<IJoyAction> actions = new List<IJoyAction>();
            foreach (string name in actionNames)
            {
                actions.Add(FetchAction(name));
            }

            return actions.ToArray();
        }
    }
}
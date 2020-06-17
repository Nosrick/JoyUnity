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

        private const string ABILITY_NAMESPACE = "JoyLib.Code.Entities.Abilities.";
        private const string NEED_NAMESPACE = "JoyLib.Code.Entities.Needs.";

        private Type m_ProvidedPathfinder;

        public ScriptingEngine()
        {
            try
            {
                string dir = Directory.GetCurrentDirectory() + "/" + GlobalConstants.SCRIPTS_FOLDER;
                string[] scriptFiles = Directory.GetFiles(dir, "*.cs", SearchOption.AllDirectories);

                List<SyntaxTree> builtFiles = new List<SyntaxTree>();

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

                m_ProvidedPathfinder = FetchType("CustomPathfinder");
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
                Type[] allTypes = m_ScriptDLL.GetTypes();

                Type directType = allTypes.Single(type => type.Name.ToLower().Equals(typeName.ToLower()));
                return directType;
            }
            catch(Exception ex)
            {
                Debug.LogError(ex.Message);
                Debug.LogError(ex.StackTrace);
                throw new InvalidOperationException("Error when searching for type, " + typeName);
            }
        }

        public Type[] FetchTypeAndChildren(string typeName)
        {
            try
            {
                Type[] allTypes = m_ScriptDLL.GetTypes();

                Type directType = null;
                foreach(Type type in allTypes)
                {
                    if(type.Name.Equals(typeName))
                    {
                        directType = type;
                        break;
                    }
                }

                List<Type> children = new List<Type>();
                if(directType != null)
                {
                    children = allTypes.Where(type => directType.IsAssignableFrom(type)).ToList();
                }
                else
                {
                    directType = Type.GetType(typeName, true, true);
                    children = allTypes.Where(type => directType.IsAssignableFrom(type)).ToList();
                }
                
                return children.ToArray();
            }
            catch(Exception ex)
            {
                Debug.LogError(ex.Message);
                Debug.LogError(ex.StackTrace);
                throw new InvalidOperationException("Error when searching for Type in ScriptingEngine, type name " + typeName);
            }
        }

        public Type[] FetchTypeAndChildren(Type type) 
        {
            try
            {
                Type[] types = m_ScriptDLL.GetTypes().Where(t => type.IsAssignableFrom(t)).ToArray();

                return types;
            }
            catch(Exception e)
            {
                Debug.LogError(e.Message);
                Debug.LogError(e.StackTrace);
                throw new InvalidOperationException("Error when searching for Type in ScriptingEngine, " + type.FullName);
            }
        }

        public IJoyAction FetchAction(string actionName)
        {
            try
            {
                Type[] allTypes = m_ScriptDLL.GetTypes();
                Type type = allTypes.Single(t => t.Name.ToLower().Equals(actionName.ToLower()));

                IJoyAction action = (IJoyAction)Activator.CreateInstance(type);
                return action;
            }
            catch(Exception e)
            {
                ActionLog.instance.AddText(e.Message);
                ActionLog.instance.AddText(e.StackTrace);
                throw new InvalidOperationException("Error when finding action, no such action " + actionName);
            }
        }

        public IPathfinder GetProvidedPathFinder()
        {
            return (IPathfinder)Activator.CreateInstance(m_ProvidedPathfinder);
        }
    }
}
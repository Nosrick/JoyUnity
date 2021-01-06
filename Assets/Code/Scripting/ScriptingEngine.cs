using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CodingSeb.ExpressionEvaluator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using UnityEngine;

namespace JoyLib.Code.Scripting
{
    public class ScriptingEngine
    {
        private static readonly Lazy<ScriptingEngine> LAZY = new Lazy<ScriptingEngine>(() => new ScriptingEngine());

        public static ScriptingEngine Instance => LAZY.Value;

        protected Assembly m_ScriptDLL;

        protected List<Type> m_Types;

        protected ExpressionEvaluator Eval;

        public ScriptingEngine()
        {
            if (this.m_Types is null)
            {
                try
                {
                    string dir = Directory.GetCurrentDirectory() + "/" + GlobalConstants.SCRIPTS_FOLDER;
                    string[] scriptFiles = Directory.GetFiles(dir, "*.cs", SearchOption.AllDirectories);

                    List<SyntaxTree> builtFiles = new List<SyntaxTree>();

                    foreach (string scriptFile in scriptFiles)
                    {
                        string contents = File.ReadAllText(scriptFile);
                        SyntaxTree builtFile = CSharpSyntaxTree.ParseText(contents);
                        builtFiles.Add(builtFile);
                    }

                    Debug.Log("Loaded " + scriptFiles.Length + " script files.");
                    List<MetadataReference> libs = new List<MetadataReference>
                    {
                        MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                        MetadataReference.CreateFromFile(typeof(GlobalConstants).Assembly.Location),
                        MetadataReference.CreateFromFile(typeof(Debug).Assembly.Location),
                        MetadataReference.CreateFromFile(typeof(Vector2Int).Assembly.Location),
                        MetadataReference.CreateFromFile(typeof(Queue<bool>).Assembly.Location),
                        MetadataReference.CreateFromFile(typeof(IQueryable).Assembly.Location),
                        MetadataReference.CreateFromFile(typeof(Castle.Core.Internal.CollectionExtensions).Assembly.Location)
                    };
                    CSharpCompilation compilation = CSharpCompilation.Create("JoyScripts", builtFiles, libs,
                        new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

                    MemoryStream memory = new MemoryStream();
                    EmitResult result = compilation.Emit(memory);

                    if (result.Success == false)
                    {
                        foreach (var diagnostic in result.Diagnostics)
                        {
                            if (diagnostic.Severity != DiagnosticSeverity.Error)
                            {
                                continue;
                            }
                            Debug.Log(diagnostic.Severity.ToString());
                            Debug.Log(diagnostic.GetMessage());
                        }
                    }

                    memory.Seek(0, SeekOrigin.Begin);
                    this.m_ScriptDLL = Assembly.Load(memory.ToArray());

                    this.m_Types = new List<Type>(this.m_ScriptDLL.GetTypes());
                    //this.m_Types.AddRange(typeof(IJoyObject).Assembly.GetExportedTypes());

                    this.Eval = new ExpressionEvaluator
                    {
                        OptionForceIntegerNumbersEvaluationsAsDoubleByDefault = true
                    };
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex.Message);
                    Debug.LogError(ex.StackTrace);
                }
            }
        }

        public object FetchAndInitialise(string type)
        {
            try
            {
                Type directType = this.m_Types.First(t => t.Name.Equals(type, StringComparison.OrdinalIgnoreCase));

                return Activator.CreateInstance(directType);
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex.Message);
                Debug.LogWarning(ex.StackTrace);
                throw new InvalidOperationException("Error when searching for Type in ScriptingEngine, type name " + type);
            }
        }

        public T Initialise<T>()
        {
            try
            {
                return Activator.CreateInstance<T>();
            }
            catch (Exception e)
            {
                Debug.LogWarning(e.Message);
                Debug.LogWarning(e.StackTrace);
                throw new InvalidOperationException("Error when searching for Type " + typeof(T).Name);
            }
        }

        public IEnumerable<T> FetchAndInitialiseChildren<T>()
        {
            try
            {
                Type[] types = this.m_Types.Where(t => typeof(T).IsAssignableFrom(t) && t.IsAbstract == false).ToArray();
                List<T> children = new List<T>();
                foreach (Type tempType in types)
                {
                    children.Add((T) Activator.CreateInstance(tempType));
                }

                return children;
            }
            catch (Exception e)
            {
                Debug.LogWarning(e.Message);
                Debug.LogWarning(e.StackTrace);
                throw new InvalidOperationException("Error when searching for Type in ScriptingEngine, " +
                                                    typeof(T).Name);
            }
        }

        public IEnumerable<Type> FetchTypeAndChildren(string typeName)
        {
            try
            {
                Type directType = this.m_Types.First(type => type.Name.Equals(typeName, StringComparison.OrdinalIgnoreCase));

                List<Type> children = new List<Type>();
                if (directType != null)
                {
                    children = this.m_Types.Where(type => directType.IsAssignableFrom(type)).ToList();
                }
                else
                {
                    directType = Type.GetType(typeName, true, true);
                    children = this.m_Types.Where(type => directType.IsAssignableFrom(type)).ToList();
                    children = children.Where(t => t.IsAbstract == false && t.IsInterface == false).ToList();
                }

                return children;
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex.Message);
                Debug.LogWarning(ex.StackTrace);
                throw new InvalidOperationException("Error when searching for Type in ScriptingEngine, " + typeName);
            }
        }

        public IEnumerable<Type> FetchTypeAndChildren(Type type)
        {
            try
            {
                Type[] types = this.m_Types.Where(t => type.IsAssignableFrom(t) && t.IsAbstract == false).ToArray();

                return types;
            }
            catch (Exception e)
            {
                Debug.LogWarning(e.Message);
                Debug.LogWarning(e.StackTrace);
                throw new InvalidOperationException(
                    "Error when searching for Type in ScriptingEngine, " + type.FullName);
            }
        }

        public IJoyAction FetchAction(string actionName)
        {
            try
            {
                Type type = this.m_Types.Single(t => t.Name.Equals(actionName, StringComparison.OrdinalIgnoreCase));

                IJoyAction action = (IJoyAction) Activator.CreateInstance(type);
                return action;
            }
            catch (Exception e)
            {
                Debug.LogWarning(e.Message);
                Debug.LogWarning(e.StackTrace);
                throw new InvalidOperationException("Error when finding action, no such action " + actionName);
            }
        }

        public IEnumerable<IJoyAction> FetchActions(string[] actionNames)
        {
            List<IJoyAction> actions = new List<IJoyAction>();
            foreach (string name in actionNames)
            {
                actions.Add(this.FetchAction(name));
            }

            return actions;
        }

        public T Evaluate<T>(string code)
        {
            return (T)Convert.ChangeType(this.Eval.Evaluate(code), typeof(T));
        }
    }
}
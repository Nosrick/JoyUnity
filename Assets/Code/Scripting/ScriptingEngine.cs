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

namespace JoyLib.Code.Scripting
{
    public static class ScriptingEngine
    {
        private static Assembly s_ScriptDLL;

        private const string ABILITY_NAMESPACE = "JoyLib.Code.Entities.Abilities.";
        private const string NEED_NAMESPACE = "JoyLib.Code.Entities.Needs.";

        private static Type s_ProvidedPathfinder;

        public static bool Initialise()
        {
            try
            {
                if(s_ScriptDLL != null)
                {
                    return true;
                }

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
                    return false;
                }

                memory.Seek(0, SeekOrigin.Begin);
                s_ScriptDLL = Assembly.Load(memory.ToArray());

                s_ProvidedPathfinder = FetchType("CustomPathfinder");

                return true;
            }
            catch(Exception ex)
            {
                Debug.LogError(ex.Message);
                Debug.LogError(ex.StackTrace);
                return false;
            }
        }

        public static Type FetchType(string typeName)
        {
            try
            {
                Type[] allTypes = s_ScriptDLL.GetTypes();

                Type directType = allTypes.Single(type => type.Name.Equals(typeName));
                return directType;
            }
            catch(Exception ex)
            {
                Debug.LogError(ex.Message);
                Debug.LogError(ex.StackTrace);
                return null;
            }
        }

        public static Type[] FetchTypeAndChildren(string typeName)
        {
            try
            {
                Type[] allTypes = s_ScriptDLL.GetTypes();

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
                return new Type[0];
            }
        }

        public static IPathfinder GetProvidedPathFinder()
        {
            return (IPathfinder)Activator.CreateInstance(s_ProvidedPathfinder);
        }
    }
}
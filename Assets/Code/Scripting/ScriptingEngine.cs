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
                    MetadataReference.CreateFromFile(typeof(Entities.Entity).Assembly.Location)
                };
                CSharpCompilation compilation = CSharpCompilation.Create("JoyScripts", builtFiles, libs, 
                    new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

                MemoryStream memory = new MemoryStream();
                EmitResult result = compilation.Emit(memory);

                if(result.Success == false)
                {
                    foreach(var diagnostic in result.Diagnostics)
                    {
                        Debug.LogError(diagnostic.GetMessage());
                    }
                    return false;
                }

                memory.Seek(0, SeekOrigin.Begin);
                s_ScriptDLL = Assembly.Load(memory.ToArray());

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
                return s_ScriptDLL.GetType(typeName, true, true);
            }
            catch(Exception ex)
            {
                Debug.LogError(ex.Message);
                Debug.LogError(ex.StackTrace);
                return null;
            }
        }

        public static List<Type> FetchTypeAndChildren(string typeName)
        {
            try
            {
                Type directType = s_ScriptDLL.GetType(typeName, true, true);
                Type[] allTypes = s_ScriptDLL.GetTypes();
                List<Type> children = allTypes.Where(type => directType.IsAssignableFrom(type)).ToList();
                return children;
            }
            catch(Exception ex)
            {
                Debug.LogError(ex.Message);
                Debug.LogError(ex.StackTrace);
                return new List<Type>();
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

namespace JoyLib.Code.Scripting
{
    public static class ScriptingEngine
    {
        private static Assembly s_ScriptDLL;

        private static Dictionary<string, object> s_ScriptObjects;
        private const string NAMESPACE = "JoyLib.Code.Entities.Abilities.";

        public static bool Initialise()
        {
            try
            {
                if(s_ScriptDLL != null)
                {
                    return true;
                }

                string[] scriptFiles = Directory.GetFiles(GlobalConstants.SCRIPTS_FOLDER, "*.cs", SearchOption.AllDirectories);

                List<SyntaxTree> builtFiles = new List<SyntaxTree>();

                foreach(string scriptFile in scriptFiles)
                {
                    string contents = File.ReadAllText(scriptFile);
                    SyntaxTree builtFile = CSharpSyntaxTree.ParseText(contents);
                    builtFiles.Add(builtFile);
                }
                Console.WriteLine("Loaded " + scriptFiles.Length + " script files.");
                CSharpCompilation compilation = CSharpCompilation.Create("JoyScripts", builtFiles);

                MemoryStream memory = new MemoryStream();
                EmitResult result = compilation.Emit(memory);

                if(result.Success == false)
                {
                    return false;
                }

                memory.Seek(0, SeekOrigin.Begin);
                s_ScriptDLL = Assembly.Load(memory.ToArray());

                s_ScriptObjects = new Dictionary<string, object>();
                foreach(Type type in s_ScriptDLL.GetTypes())
                {
                    Object obj = Activator.CreateInstance(type);
                    s_ScriptObjects.Add(type.Name, obj);
                }

                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public static object Execute(string className, string functionName, params object[] objects)
        {
            if(s_ScriptObjects.ContainsKey(className))
            {
                Object obj = s_ScriptObjects[className];
                Type type = obj.GetType();
                Object returnValue = type.InvokeMember(functionName, BindingFlags.Default | BindingFlags.InvokeMethod, null, obj, objects);
                return returnValue;
            }
            return null;
        }
    }
}
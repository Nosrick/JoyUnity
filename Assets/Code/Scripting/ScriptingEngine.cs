using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Items;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Loaders;
using System;
using System.IO;
using UnityEngine;

namespace JoyLib.Code.Scripting
{
    public static class ScriptingEngine
    {
        public static void Initialise()
        {
            UserData.RegisterProxyType<MoonEntity, Entity>(p => new MoonEntity(p));
            UserData.RegisterProxyType<MoonItem, ItemInstance>(p => new MoonItem(p));
        }

        public static dynamic RunScript(string code, string className, string functionName, params object[] arguments)
        {
            try
            {
                Script script = new Script();

                script.Options.DebugPrint = (s => Console.WriteLine(s));

                script.Globals["MoonEntity"] = UserData.CreateStatic<Entity>();
                script.Globals["MoonItem"] = UserData.CreateStatic<ItemInstance>();

                script.DoString(code);
                DynValue function = script.Globals.Get(functionName);
                DynValue result = script.Call(function, arguments);

                return result;
            }
            catch(Exception e)
            {
                Debug.LogError(e.Message);
                Debug.LogError(e.StackTrace);
                return null;
            }
        }

        public static dynamic RunScriptFromFile(string fileName, string className, string functionName, params dynamic[] arguments)
        {
            try
            {
                Script script = new Script();

                script.Options.DebugPrint = (s => Console.WriteLine(s));

                script.Globals["MoonEntity"] = UserData.CreateStatic<Entity>();
                script.Globals["MoonItem"] = UserData.CreateStatic<ItemInstance>();

                //DynValue result = script.DoString(code);
                DynValue result = Script.RunFile(fileName);
                return result;
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                Debug.LogError(e.StackTrace);
                return null;
            }
        }
    }
}
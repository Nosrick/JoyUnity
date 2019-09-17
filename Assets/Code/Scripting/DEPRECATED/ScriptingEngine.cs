/*

using JoyLib.Code.Cultures;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Items;
using MoonSharp.Interpreter;
using System;
using UnityEngine;

namespace JoyLib.Code.Scripting
{
    public static class ScriptingEngine
    {
        public static void Initialise()
        {
            UserData.RegisterProxyType<MoonEntity, Entity>(p => new MoonEntity(p));
            UserData.RegisterProxyType<MoonItem, ItemInstance>(p => new MoonItem(p));
            UserData.RegisterProxyType<MoonCulture, CultureType>(p => new MoonCulture(p));
            UserData.RegisterType(typeof(MoonVector2Int));
        }

        public static DynValue RunScript(string code, string className, string functionName, params object[] arguments)
        {
            try
            {
                Script script = new Script();

                script.Options.DebugPrint = (s => Console.WriteLine(s));

                script.Globals["MoonEntity"] = UserData.CreateStatic<Entity>();
                script.Globals["MoonItem"] = UserData.CreateStatic<ItemInstance>();
                script.Globals["MoonCulture"] = UserData.CreateStatic<CultureType>();
                script.Globals["MoonVector"] = UserData.CreateStatic<MoonVector2Int>();

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

        public static DynValue RunScriptFromFile(string fileName, string className, string functionName, params object[] arguments)
        {
            try
            {
                Script script = new Script();

                script.Options.DebugPrint = (s => Console.WriteLine(s));

                script.Globals["MoonEntity"] = UserData.CreateStatic<Entity>();
                script.Globals["MoonItem"] = UserData.CreateStatic<ItemInstance>();
                script.Globals["MoonCulture"] = UserData.CreateStatic<CultureType>();

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

*/
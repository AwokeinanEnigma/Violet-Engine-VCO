using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Loaders;
using System;
using System.Collections.Generic;
using System.IO;

namespace Violet.Lua
{
    public class LuaConfiguration
    {
        /// <summary>
        /// Refer to the official MoonSharp documentation for this one.
        /// https://www.moonsharp.org/scriptoptions.html
        /// </summary>
        public struct ScriptOptions
        {
            public bool CheckThreadAccess;
            public ColonOperatorBehaviour ColonOperatorClrCallbackBehaviour;
            public Func<string, string> DebugInput;
            public Action<string> DebugPrint;
            public IScriptLoader ScriptLoader;
            public Stream Stderr;
            public Stream Stdin;
            public Stream Stdout;
            public int TailCallOptimizationThreshold;
            public bool UseLuaErrorLocations;

        }

        /// <summary>
        /// In this list are the name of the Global and the reference to the C# class that goes with the Global.
        /// I.E Globals[myClass] would be MyClass in C#
        /// </summary>
        public Dictionary<string, object> Globals;

        /// <summary>
        /// Options for the script executor. Ignore these if you don't know what you're doing
        /// </summary>
        public ScriptOptions Options;

        public LuaConfiguration()
        {
            Globals = new Dictionary<string, object>();
        }

        public LuaConfiguration(Dictionary<string, object> globalDict)
        {
            Globals = globalDict;
            //Globals = new Dictionary<string, object>();
        }

        public LuaConfiguration(Dictionary<string, object> globalDict, ScriptOptions options)
        {
            Globals = globalDict;
            Options = options;
            //Globals = new Dictionary<string, object>();
        }

        /// <summary>
        /// This set all of the globals within a Lua script.
        /// </summary>
        /// <param name="script">The script which globals will be set to the Globals dictionary.</param>
        public void SetConfig(Script script, bool setOptions = false)
        {
            // go through our dict
            foreach (KeyValuePair<string, object> globalAndObjects in Globals)
            {

                // set the global's key to the value
                // basically, this means that you can do something like
                // overworldThing.DoSomething(); in lua
                // where overworldThing is a class
                script.Globals[globalAndObjects.Key] = globalAndObjects.Value;
            }


            // i don't even know what these do but it's better to have the support now! - Enigma 10/31/22
            if (setOptions)
            {
                script.Options.CheckThreadAccess = Options.CheckThreadAccess;
                script.Options.ColonOperatorClrCallbackBehaviour = Options.ColonOperatorClrCallbackBehaviour;
                script.Options.DebugInput = Options.DebugInput;
                script.Options.DebugPrint = Options.DebugPrint;
                script.Options.ScriptLoader = Options.ScriptLoader;
                script.Options.Stderr = Options.Stderr;
                script.Options.Stdin = Options.Stdin;
                script.Options.Stdout = Options.Stdout;
                script.Options.TailCallOptimizationThreshold = Options.TailCallOptimizationThreshold;
                script.Options.UseLuaErrorLocations = Options.UseLuaErrorLocations;
            }
        }
    }
}

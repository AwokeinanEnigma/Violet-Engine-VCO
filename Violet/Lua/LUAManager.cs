using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using System.Reflection;
using Violet;

namespace VCO.Lua
{
    /// <summary>
    /// Handles all 
    /// </summary>
    public class LUAManager 
    {
        public static LUAManager instance;

        private static Dictionary<string, string> luaFiles;

        public LUAManager(string luaDir)
        {
            Debug.LogS("LuaManager initializing.");

            BuildLuaScripts(luaDir);
        }

        /// <summary>
        /// Don't use UserData.RegisterAssembly, that one is worthless and breaks
        /// Instead, use this one, which finds all types in an assembly and registers them manually.
        /// </summary>
        /// <param name="asm">The assembly containing the types you want to register</param>
        /// <param name="mode">The interop access mode for the types you want to register</param>
        public void RegisterAssembly(Assembly asm, InteropAccessMode mode = InteropAccessMode.Default, List<Type> forbiddenTypes = null) {
            Type[] typesInAsm = asm.GetTypes();

            // the weakness of this is that you cannot manually pick and choose the InteropAccessMode for specific types
            // but that'll probably never be a problem.

            if (forbiddenTypes == null)
            {
                for (int i = 0; i < typesInAsm.Length; i++)
                {
                    // Change this later
                    UserData.RegisterType(typesInAsm[i], mode);
                }
            }
            else {
                for (int i = 0; i < typesInAsm.Length; i++)
                {
                    if (forbiddenTypes.Contains(typesInAsm[i]))
                    {
                        // warn
                        Debug.LogW($"Type '{typesInAsm[i]}' excluded from being registered with MoonSharp! Assembly: {asm.FullName}");
                        // skip
                        continue; 
                    }
                    // register if it's not excluded.
                    UserData.RegisterType(typesInAsm[i], mode);
                }
            }
        }

        public static void Initialize(string luaDir)
        {
            // return if we exist and give a warning
            if (instance != null)
            {
                Debug.LogW($"Another instance of the LUAManager class tried to be created!");
                return;
            }
            // initialize
            instance = new LUAManager(luaDir);
        }

        private void BuildLuaScripts(string luaDir) { 
            // in the future this will collect all lua scripts in the specified directory
        
        }
    }
}

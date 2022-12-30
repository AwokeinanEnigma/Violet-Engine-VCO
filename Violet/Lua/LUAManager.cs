using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Violet.Lua
{
    /// <summary>
    /// Handles all lua scripts.
    /// </summary>
    public class LuaManager
    {
        public static LuaManager instance;

        //key: name
        //value: path
        private static Dictionary<string, string> luaFiles;

        public LuaHandler CreateLuaHandler(string name, LuaConfiguration config)
        {

            // get path of our lua file
            luaFiles.TryGetValue(name, out string path);

            // read all text from the path of the lua file
            string scriptcode = File.ReadAllText(path);

            // create new luahandler
            return new LuaHandler(scriptcode, config);
        }

        public LuaManager(string luaDir)
        {
            Debug.LogLua("LuaManager initializing.");

            BuildLuaScripts(luaDir);
        }

        /// <summary>
        /// Don't use UserData.RegisterAssembly, that one is worthless and breaks
        /// Instead, use this one, which finds all types in an assembly and registers them manually.
        /// </summary>
        /// <param name="asm">The assembly containing the types you want to register</param>
        /// <param name="mode">The interop access mode for the types you want to register</param>
        public void RegisterAssembly(Assembly asm, InteropAccessMode mode = InteropAccessMode.Default, List<Type> forbiddenTypes = null)
        {
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
            else
            {
                for (int i = 0; i < typesInAsm.Length; i++)
                {
                    if (forbiddenTypes.Contains(typesInAsm[i]))
                    {
                        // warn
                        Debug.LogWarning($"Type '{typesInAsm[i]}' excluded from being registered with MoonSharp! Assembly: {asm.FullName}");
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
                Debug.LogWarning($"Another instance of the LUAManager class tried to be created!");
                return;
            }
            // initialize
            instance = new LuaManager(luaDir);
        }

        private void BuildLuaScripts(string luaDir)
        {
            // in the future this will collect all lua scripts in the specified directory
            // and the future is now
            luaFiles = new Dictionary<string, string>();
            ProcessDirectory(luaDir);
        }

        // stolen from
        // https://learn.microsoft.com/en-us/dotnet/api/system.io.directory.getfiles?view=net-7.0
        public static void ProcessDirectory(string targetDirectory)
        {
            // Process the list of files found in the directory.
            string[] fileEntries = Directory.GetFiles(targetDirectory);
            foreach (string fileName in fileEntries)
            {
                ProcessFile(fileName);
            }

            // Recurse into subdirectories of this directory.
            string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
            foreach (string subdirectory in subdirectoryEntries)
            {
                ProcessDirectory(subdirectory);

            }
        }

        // Insert logic for processing found files here.
        public static void ProcessFile(string path)
        {
            //;
            luaFiles.Add(Path.GetFileName(path), path);
            Debug.LogLua($"Processed LUA file '{Path.GetFileName(path)}'");
        }
    }
}

using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Violet.Lua
{
    public class LuaHandler
    {
        private LuaConfiguration _config;
        private Script _lua;
        private string _luaScriptString;

        public LuaHandler(string luaScriptString, LuaConfiguration conf) {
            // initiate script
            _lua = new Script();

            // set lua script string
            SetLua(luaScriptString);

            // set config
            SetConfig(conf);

        }

        #region Do Methods
        /// <summary>
        /// Executes the script with no arguments.
        /// </summary>
        public void Do()
        {
            _lua.DoString(_luaScriptString);
        }

        /// <summary>
        /// Executes the script with the table argument.
        /// </summary>
        /// <param name="table"></param>
        public void Do(Table table)
        {
            _lua.DoString(_luaScriptString, table);
        }

        /// <summary>
        /// Executes the script with the table argument and the codeFriendlyName argument.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="codeFriendlyName"></param>
        public void Do(Table table, string codeFriendlyName)
        {
            _lua.DoString(_luaScriptString, table, codeFriendlyName);
        }
        #endregion

        #region CallLuaFunc 
        /// <summary>
        /// Calls a function within the lua script 
        /// </summary>
        /// <param name="func">The lua function you want to call</param>
        public DynValue CallLuaFunc(string func) {
            return _lua.Call(func);
        }

        /// <summary>
        /// Calls a function within the lua script 
        /// </summary>
        /// <param name="func">The lua function you want to call</param>
        public DynValue CallLuaFunc(string func, object args)
        {
            return _lua.Call(func, args);
        }
        #endregion

        #region LuaHandler specific methods
        /// <summary>
        /// Sets the config of this LuaHandler and the script
        /// </summary>
        /// <param name="newConfig">The config to set this handler and script to.</param>
        public void SetConfig(LuaConfiguration newConfig) {
            _config = newConfig;
            _config.SetConfig(_lua);
        }

        /// <summary>
        /// Sets the lua script of this LuaHandler
        /// </summary>
        /// <param name="_lua">The lua script that will be assosicated with this LuaHandler</param>
        public void SetLua(string _lua)
        {
            this._luaScriptString = _lua;
        }
        #endregion
    }
}

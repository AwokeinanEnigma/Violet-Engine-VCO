using MoonSharp.Interpreter;
using System;

namespace Violet.Lua
{
    public class LuaHandler : IDisposable
    {
        private LuaConfiguration _config;
        private Script _lua;
        private string _luaScriptString;
        private bool disposed;

        // this is kinda stupid but i can't think of a better way to implement this other than giving raw access to the script field
        public object this[string name]
        {
            get => _lua.Globals[name];
        }

        public LuaHandler(string luaScriptString, LuaConfiguration conf)
        {
            // initiate script
            _lua = new Script();

            // set lua script string
            SetLua(luaScriptString);

            // set config
            SetConfig(conf);

        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _luaScriptString = null;
                    _lua = null;
                    _config = null;
                }
                this.disposed = true;
            }
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
        public DynValue CallLuaFunc(object function)
        {
            return _lua.Call(function);
        }

        /// <summary>
        /// Calls a function within the lua script 
        /// </summary>
        /// <param name="func">The lua function you want to call</param>
        public DynValue CallLuaFunc(object function, object args)
        {
            return _lua.Call(function, args);
        }
        #endregion

        #region LuaHandler specific methods
        /// <summary>
        /// Sets the config of this LuaHandler and the script
        /// </summary>
        /// <param name="newConfig">The config to set this handler and script to.</param>
        public void SetConfig(LuaConfiguration newConfig)
        {
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

using System;
using System.Collections.Generic;

namespace VCO.Scripts.Actions
{
    internal abstract class RufiniAction
    {

        public List<ActionParam> Params => this.paramList;

        public abstract string Code { get; }

        public RufiniAction()
        {
            this.paramValues = new Dictionary<string, object>();
        }

        public abstract ActionReturnContext Execute(ExecutionContext context);

        public void SetValue<T>(string param, T value)
        {
            ActionParam actionParam = this.paramList.Find((ActionParam x) => x.Name == param);
            if (actionParam == null)
            {
                throw new KeyNotFoundException("The key \"" + param + "\" was not found in the parameter list");
            }
            if (!(actionParam.Type == typeof(T)))
            {
                throw new InvalidOperationException(string.Concat(new object[]
                {
                    "Tried to set incorrect type for \"",
                    param,
                    "\", got ",
                    typeof(T),
                    " but expected ",
                    actionParam.Type
                }));
            }
            if (this.paramValues.ContainsKey(param))
            {
                this.paramValues[param] = value;
                return;
            }
            this.paramValues.Add(param, value);
        }

        public T GetValue<T>(string param)
        {
            T result = default(T);
            if (this.paramValues.ContainsKey(param))
            {
                object obj = this.paramValues[param];
                if (obj is T)
                {
                    result = (T)obj;
                }
            }
            return result;
        }

        public bool TryGetValue<T>(string param, out T value)
        {
            bool result = false;
            value = default(T);
            if (this.paramValues.ContainsKey(param))
            {
                object obj = this.paramValues[param];
                if (obj is T)
                {
                    result = true;
                    value = (T)obj;
                }
            }
            return result;
        }
        public bool HasValue(string param)
        {
            return this.paramValues.ContainsKey(param);
        }

        protected List<ActionParam> paramList;

        private readonly Dictionary<string, object> paramValues;
    }
}

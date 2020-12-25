using System.Collections.Generic;
using lang.core;

namespace language.lang.core {
    public class Environment {

        private readonly Dictionary<string, object> values = new();

        internal void define(string name, object value) {
            values.Add(name, value);
        }

        internal object get(Token name) {

            object retVal = null;

            if (values.TryGetValue(name.Lexeme, out retVal)) {
                return retVal;
            }

            throw new RuntimeError(name, "Undefined variable '" + name.Lexeme + "'.");
        }

        public void assign(Token name, object value) {
            if (values.ContainsKey(name.Lexeme)) {
                values[name.Lexeme] = value;
                return;
            }

            throw new RuntimeError(name, "Undefined varibale '" + name.Lexeme + "'.");
        }
    }
}
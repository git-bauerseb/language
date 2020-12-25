using System;

namespace language.lang.core {
    public class RuntimeError : Exception {
        public Token Token { get; }

        internal RuntimeError(Token token, string msg) : base(msg) {
            this.Token = token;
        }
    }
}
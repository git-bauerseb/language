using System.Text;

namespace language.lang.core {

    public class Token {
        internal readonly TokenType type;
        public string Lexeme { get; }
        internal readonly object literal;
        internal readonly int line;

        public Token(TokenType type, string lexeme, object literal, int line) {
            this.type = type;
            this.Lexeme = lexeme;
            this.literal = literal;
            this.line = line;
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();

            sb.Append(type).Append(' ');

            if (Lexeme == null) {
                sb.Append("(null)");
            }
            else {
                switch (type) {
                    case TokenType.PLUS: sb.Append('+');
                        break;
                    case TokenType.MINUS: sb.Append('-');
                        break;
                    default:
                        sb.Append("(unknown");
                        break;
                }
            }

            sb.Append(' ').Append(literal);

            return sb.ToString();
        }
    }
}
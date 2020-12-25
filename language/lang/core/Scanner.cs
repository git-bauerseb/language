using System.Collections.Generic;

namespace language.lang.core {
    public class Scanner {

        private readonly string source;
        private readonly List<Token> tokens = new List<Token>();
        private readonly Dictionary<string, TokenType> keywords;
        private int start = 0;
        private int current = 0;
        private int line = 1;


        public Scanner(string source) {
            this.keywords = new Dictionary<string, TokenType>();
            keywords.Add("and", TokenType.AND);
            keywords.Add("class", TokenType.CLASS);
            keywords.Add("else", TokenType.ELSE);
            keywords.Add("false", TokenType.FALSE);
            keywords.Add("for", TokenType.FOR);
            keywords.Add("func", TokenType.FUNC);
            keywords.Add("if", TokenType.IF);
            keywords.Add("none", TokenType.NONE);
            keywords.Add("var", TokenType.VAR);
            keywords.Add("print", TokenType.PRINT);
            keywords.Add("true", TokenType.TRUE);
            
            this.source = source;
        }

        public List<Token> scanTokens() {
            while (!isAtEnd()) {

                start = current;
                scanToken();
            }

            tokens.Add(new Token(TokenType.EOF, "", null, line));
            return tokens;
        }

        private char advance() {
            current++;
            return source[current - 1];
        }

        private void addToken(TokenType type) {
            addToken(type, null, null);
        }

        private void addToken(TokenType type, string lexeme, object literal) {
            tokens.Add(new Token(type, lexeme, literal, line));
        }

        private void scanToken() {
            char c = advance();

            switch (c) {
                case ' ':
                case '\r':
                case '\t':
                    break;
                case '\n':
                    line++;
                    break;
                case '(': addToken(TokenType.LEFT_PAREN); break;
                case ')': addToken(TokenType.RIGHT_PAREN); break;
                case '{': addToken(TokenType.LEFT_BRACE); break;
                case '}': addToken(TokenType.RIGHT_BRACE); break;
                case ',': addToken(TokenType.COMMA); break;
                case '.': addToken(TokenType.DOT); break;
                case '-': addToken(TokenType.MINUS); break;
                case '+': addToken(TokenType.PLUS); break;
                case ';': addToken(TokenType.SEMICOLON); break;
                case '*': addToken(TokenType.STAR); break;
             
                case '!':
                    addToken(match('=') ? TokenType.BANG_EQUAL : TokenType.BANG);
                    break;
                case '=':
                    addToken(match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL);
                    break;
                case '<':
                    addToken(match('=') ? TokenType.LESS_EQUAL : TokenType.LESS);
                    break;
                case '>':
                    addToken(match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER);
                    break;
                case '/':
                    if (match('/')) {
                        while (peek() != '\n' && !isAtEnd()) {
                            advance();
                        }
                    } else if (match('*'))
                    {
                        multilineComment();
                    }
                    else {
                        
                        addToken(TokenType.SLASH);
                    }
                    break;
                case '"': matchString(); break;
                case ':':
                    if (match(':')) {
                        addToken(TokenType.DOUBLE_COLON);
                    }
                    break;
                default:
                    if (isDigit(c)) {
                        number();
                    } else if (isAlpha(c)) {
                        identifier();
                    }
                    else {
                        Entry.error(line, "Unexpected character.");
                    }

                    break;
            }
        }


        private void multilineComment() {

            int depth = 1;
            
            while (true) {

                if (isAtEnd() && depth != 0) {
                    Entry.error(line, "Unterminated comment");
                    return;
                }

                if (match('\n')) {
                    line++;
                }

                if (match('/') && match('*')) {
                    depth++;
                }

                if (match('*') && match('/')) {
                    depth--;
                }

                if (depth == 0) {
                    break;
                }
                
                
                advance();
            }

            advance();
            advance();
        }

        private bool isDigit(char c) {
            return c >= '0' && c <= '9';
        }

        private bool isAlpha(char c) {
            return (c >= 'a' && c <= 'z') ||
                   (c >= 'A' && c <= 'Z') ||
                   c == '_';
        }

        private bool isAlphaNumeric(char c) {
            return isAlpha(c) || isDigit(c);
        }

        private void identifier() {

            int length = 1;
            
            while (isAlphaNumeric(peek())) {
                advance();
                length++;
            }

            string text = source.Substring(start, length);
            TokenType type;

            if (!keywords.TryGetValue(text, out type)) {
                type = TokenType.IDENTIFIER;
                addToken(type, text, null);
            }
            else
            {
                addToken(type, null, null);   
            }
        }

        private void number() {

            int length = 1;
            
            while (isDigit(peek())) {
                advance();
                length++;
            }

            if (peek() == '.' && isDigit(peekNext())) {
                advance();

                while (isDigit(peek())) {
                    advance();
                }
            }

            string number = source.Substring(start, length);

            addToken(
                TokenType.NUMBER,
                null,
                float.Parse(number));
        }

        private void matchString() {
            int length = 1;
            while (peek() != '"' && !isAtEnd()) {
                if (peek() == '\n') {line++;}
                advance();
                length++;
            }

            if (isAtEnd()) {
                Entry.error(line, "Unterminated string.");
            }

            advance();

            string value = source.Substring(start + 1, length - 1);
            addToken(TokenType.STRING, null, value);
        }

        private bool match(char expected) {
            if (isAtEnd()) {
                return false;
            }

            if (source[current] != expected) {
                return false;
            }

            current++;
            return true;
        }

        private char peek() {
            if (isAtEnd()) {return '\0';}
            return source[current];
        }

        private char peekNext() {
            if (current + 1 >= source.Length) {
                return '\0';
            }

            return source[current + 1];
        }

        private bool isAtEnd() {
            return current >= source.Length;
        }
    }
}
namespace language.lang.core {

    public enum TokenType {

        // Single character tokens
        LEFT_PAREN, RIGHT_PAREN, LEFT_BRACE, RIGHT_BRACE,
        COMMA, DOT, MINUS, PLUS, SEMICOLON, STAR,

        // Operators
        BANG_EQUAL, BANG, EQUAL_EQUAL, EQUAL, LESS_EQUAL, LESS,
        GREATER_EQUAL, GREATER, SLASH,

        STRING, NUMBER,

        AND, CLASS, IF, ELSE, FALSE, FOR, FUNC, NONE, VAR, TRUE,

        IDENTIFIER,

        EOF,

        RETURN,
        PRINT,
        DOUBLE_COLON
    }
}
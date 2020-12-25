using System;
using System.Collections.Generic;
using lang.core;

namespace language.lang.core {
    internal class Parser {
        
        private sealed class ParseError : Exception {
        }

        private readonly List<Token> tokens;
        private int current = 0;

        internal Parser(List<Token> tokens) {
            this.tokens = tokens;
        }

        internal List<Stmt> parse() {
            List<Stmt> statements = new List<Stmt>();
            while (!isAtEnd()) {
                statements.Add(declaration());
            }

            return statements;
        }

        private Stmt declaration() {
            try {
                if (match(TokenType.VAR)) {
                    return varDeclaration();
                }

                return statement();
            }
            catch (ParseError error) {
                synchronize();
                return null;
            }
        }

        private Stmt varDeclaration() {
            Token name = consume(TokenType.IDENTIFIER, "Expect variable name.");

            // Currently, there is no use for variable types
            consume(TokenType.DOUBLE_COLON, "Expect '::' after declaration name.");
            consume(TokenType.IDENTIFIER, "Expect type annotation for variable.");
            
            
            Expr initializer = null;
            if (match(TokenType.EQUAL)) {
                initializer = expression();
            }

            consume(TokenType.SEMICOLON, "Expect ';' after variable declaration.");
            return new Stmt.Var(name, initializer);
        }

        private Stmt statement() {
            if (match(TokenType.PRINT)) {
                return printStatement();
            }

            return expressionStatement();
        }

        private Stmt printStatement() {
            Expr value = expression();
            consume(TokenType.SEMICOLON, "Expect ';' after value.");
            return new Stmt.Print(value);
        }

        private Stmt expressionStatement() {
            Expr expr = expression();
            consume(TokenType.SEMICOLON, "Expect ';' after expression.");
            return new Stmt.Expression(expr);
        }
        
        private Expr primary() {
            if (match(TokenType.FALSE)) return new Expr.Literal(false);
            if (match(TokenType.TRUE)) return new Expr.Literal(true);
            if (match(TokenType.NONE)) return new Expr.Literal(null);

            if (match(TokenType.NUMBER, TokenType.STRING)) {
                return new Expr.Literal(previous().literal);
            }

            if (match(TokenType.LEFT_PAREN)) {
                Expr expr = expression();
                consume(TokenType.RIGHT_PAREN, "Expect ')' after expression.");
                return new Expr.Grouping(expr);
            }

            if (match(TokenType.IDENTIFIER)) {
                return new Expr.Variable(previous());
            }

            throw error(peek(), "Expect expression.");
        }

        private Expr unary() {
            if (match(TokenType.BANG, TokenType.MINUS)) {
                Token op = previous();
                Expr right = unary();
                return new Expr.Unary(op, right);
            }

            return primary();
        }
        
        private Expr factor() {
            Expr expr = unary();

            while (match(TokenType.SLASH, TokenType.STAR)) {
                Token op = previous();
                Expr right = unary();
                expr = new Expr.Binary(expr, op, right);
            }

            return expr;
        }

        private Expr term() {
            Expr expr = factor();

            while (match(TokenType.MINUS, TokenType.PLUS)) {
                Token op = previous();
                Expr right = factor();
                expr = new Expr.Binary(expr, op, right);
            }

            return expr;
        }

        private Expr comparison() {
            Expr expr = term();

            while (match(TokenType.GREATER,
                TokenType.GREATER_EQUAL,
                TokenType.LESS, TokenType.LESS_EQUAL)) {
                Token op = previous();
                Expr right = term();
                expr = new Expr.Binary(expr, op, right);
            }

            return expr;
        }
        
        private Expr equality() {
            Expr expr = comparison();

            while (match(TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL)) {
                Token op = previous();
                Expr right = comparison();
                expr = new Expr.Binary(expr, op, right);
            }

            return expr;
        }

        private Expr assignment() {
            Expr expr = equality();

            if (match(TokenType.EQUAL)) {
                Token equals = previous();
                Expr value = assignment();

                if (expr is Expr.Variable) {
                    Token name = ((Expr.Variable) expr).Name;
                    return new Expr.Assign(name, value);
                }

                error(equals, "Invalid assignment target.");
            }

            return expr;
        }
        
        private Expr expression() {
            return assignment();
        }

        private bool match(params TokenType[] types) {
            foreach (var type in types) {
                if (check(type)) {
                    advance();
                    return true;
                }
            }

            return false;
        }

        private bool check(TokenType type) {
            if (isAtEnd()) {
                return false;
            }

            return peek().type == type;
        }

        private Token advance() {
            if (!isAtEnd()) {
                current++;
            }

            return previous();
        }

        private bool isAtEnd() {
            return peek().type == TokenType.EOF;
        }

        private Token peek() {
            return tokens[current];
        }

        private Token previous() {
            return tokens[current - 1];
        }

        private Token consume(TokenType type, string msg) {
            if (check(type)) {
                return advance();
            }

            throw error(peek(), msg);
        }

        private ParseError error(Token token, string msg) {
            Entry.error(token, msg);
            return new ParseError();
        }

        private void synchronize() {
            advance();

            while (!isAtEnd()) {
                if (previous().type == TokenType.SEMICOLON) return;

                switch (peek().type) {
                    case TokenType.CLASS:
                    case TokenType.FUNC:
                    case TokenType.VAR: 
                    case TokenType.FOR: 
                    case TokenType.IF: 
                    case TokenType.RETURN:
                        return;
                }

                advance();
            }
        }
    }
}
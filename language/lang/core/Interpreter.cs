using System;
using System.Collections.Generic;
using lang.core;

namespace language.lang.core {

    public class Interpreter : ExpressionVisitor<object>, StmtVisitor<object> {

        private Environment environment = new Environment();
        
        public void interpret(List<Stmt> stmts) {
            try {
                foreach (var stmt in stmts) {
                    execute(stmt);
                }
            }
            catch (RuntimeError error) {
                Entry.runtimeError(error);
            }
        }
        
        public object visitBinaryExpr(Expr.Binary expr) {
            object left = evaluate(expr.Left);
            object right = evaluate(expr.Right);

            switch (expr.Op.type) {
                case TokenType.MINUS: 
                    checkNumberOperands(expr.Op, left, right);
                    return (float) left - (float) right;
                case TokenType.SLASH: 
                    checkNumberOperands(expr.Op, left, right);
                    checkDivisionByZero(expr.Op, right);
                    return (float) left / (float) right;
                case TokenType.STAR: 
                    checkNumberOperands(expr.Op, left, right);
                    return (float) left * (float) right;
                case TokenType.PLUS:
                    if (left is float && right is float) {
                        return (float) left + (float) right;
                    }
                    if (left is string && right is string) {
                        return (string) left + (string) right;
                    }

                    throw new RuntimeError(expr.Op, "Operands must both numbers/strings");
                case TokenType.GREATER: 
                    checkNumberOperands(expr.Op, left, right);
                    return (float) left > (float) right;
                case TokenType.BANG_EQUAL: 
                    checkNumberOperands(expr.Op, left, right);
                    return !isEqual(left, right);
                case TokenType.EQUAL_EQUAL: return isEqual(left, right);
            }

            return null;
        }

        public object visitGroupingExpr(Expr.Grouping expr) {
            return evaluate(expr.Expression);
        }

        public object visitLiteralExpr(Expr.Literal expr) {
            return expr.Value;
        }

        public object visitUnaryExpr(Expr.Unary expr) {
            object right = evaluate(expr.Right);

            switch (expr.Op.type) {
                case TokenType.MINUS:
                    checkNumberOperand(expr.Op, right);
                    return -(float)right;
                case TokenType.BANG: return !isTruthy(right);
            }
            
            return null;
        }

        public object visitVariable(Expr.Variable expr) {
            return environment.get(expr.Name);
        }

        public object visitAssign(Expr.Assign assign) {
            object value = evaluate(assign.Value);
            environment.assign(assign.Name, value);
            return value;
        }

        private string stringify(object obj) {
            if (obj == null) {
                return "none";
            }
            else {
                if (obj is float) {
                    string text = obj.ToString();
                    return text;
                }
            }

            return obj.ToString();
        }

        private void checkDivisionByZero(Token op, object denominator) {
            if (Math.Abs((float) denominator - 0.0) <  1e-4f) {
                throw new RuntimeError(op, "Division by Zero.");
            }
        }

        private void checkNumberOperands(Token op, object left, object right) {
            if (left is float && right is float) {
                return;
            }

            throw new RuntimeError(op, "Operands must be numbers");
        }

        private void checkNumberOperand(Token op, object operand) {
            if (operand is float) return;
            throw new RuntimeError(op, "Operand must be a number.");
        }

        private bool isEqual(object a, object b) {
            if (a == null && b == null) {
                return true;
            }

            if (a == null) {
                return false;
            }

            return a.Equals(b);
        }

        private bool isTruthy(object obj) {
            if (obj == null) {
                return false;
            }

            if (obj is bool) {
                return (bool)obj;
            }

            return true;
        }

        private object evaluate(Expr expr) {
            return expr.accept(this);
        }

        private void execute(Stmt stmt) {
            stmt.accept(this);
        }

        public object visitExpressionStmt(Stmt.Expression expr) {
            evaluate(expr.expression);
            return null;
        }

        public object visitPrintStmt(Stmt.Print stmt) {
            object val = evaluate(stmt.expression);
            Console.WriteLine(stringify(val));
            return null;
        }

        public object visitVariableStmt(Stmt.Var expr) {
            object value = null;

            if (expr.Initializer != null) {
                value = evaluate(expr.Initializer);
            }

            environment.define(expr.Name.Lexeme, value);
            return null;
        }
    }
}
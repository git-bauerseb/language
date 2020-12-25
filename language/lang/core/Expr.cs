using System.Collections.Generic;
using lang.core;
using language.lang.core;

namespace lang.core {
    public abstract class Expr {

        public abstract R accept<R>(ExpressionVisitor<R> expressionVisitor);

        public class Binary : Expr {
            internal Binary(Expr left, Token op, Expr right) {
                this.Left = left;
                this.Op = op;
                this.Right = right;
            }

            public Expr Left { get; }
            public Token Op { get; }
            public Expr Right { get; }

            public override R accept<R>(ExpressionVisitor<R> expressionVisitor) {
                return expressionVisitor.visitBinaryExpr(this);
            }
        }

        public class Grouping : Expr {
            internal Grouping(Expr expression) {
                this.Expression = expression;
            }

            public Expr Expression { get; }

            public override R accept<R>(ExpressionVisitor<R> expressionVisitor) {
                return expressionVisitor.visitGroupingExpr(this);
            }
        }

        public class Literal : Expr {
            internal Literal(object value) {
                this.Value = value;
            }

            public object Value { get; }

            public override R accept<R>(ExpressionVisitor<R> expressionVisitor) {
                return expressionVisitor.visitLiteralExpr(this);
            }
        }

        public class Unary : Expr {
            internal Unary(Token op, Expr right) {
                this.Op = op;
                this.Right = right;
            }

            public Token Op { get; }
            public Expr Right { get; }

            public override R accept<R>(ExpressionVisitor<R> expressionVisitor) {
                return expressionVisitor.visitUnaryExpr(this);
            }
        }
        
        public class Variable : Expr {
            internal Variable(Token name) {
                this.Name = name;
            }

            public Token Name { get; }

            public override R accept<R>(ExpressionVisitor<R> expressionVisitor) {
                return expressionVisitor.visitVariable(this);
            }
        }
        
        public class Assign : Expr {
            internal Assign(Token name, Expr value) {
                this.Name = name;
                this.Value = value;
            }

            public Token Name { get; }
            public Expr Value { get; }

            public override R accept<R>(ExpressionVisitor<R> expressionVisitor) {
                return expressionVisitor.visitAssign(this);
            }
        }
    }
}
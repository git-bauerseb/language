using language.lang.core;

namespace lang.core {
    public abstract class Stmt {
        
        
        public abstract R accept<R>(StmtVisitor<R> visitor);

        public class Expression : Stmt {
            internal Expression(Expr expression) {
                this.expression = expression;
            }

            public readonly Expr expression;

            public override R accept<R>(StmtVisitor<R> visitor) {
                return visitor.visitExpressionStmt(this);
            }
        }

        public class Print : Stmt {
            internal Print(Expr expression) {
                this.expression = expression;
            }

            public readonly Expr expression;

            public override R accept<R>(StmtVisitor<R> visitor) {
                return visitor.visitPrintStmt(this);
            }
        }
        
        public class Var : Stmt {
            internal Var(Token name, Expr initializer) {
                this.Name = name;
                this.Initializer = initializer;
            }

            public Expr Initializer { get; set; }
            public Token Name { get; }

            public override R accept<R>(StmtVisitor<R> expressionVisitor) {
                return expressionVisitor.visitVariableStmt(this);
            }
        }
    }
}
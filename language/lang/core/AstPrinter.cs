using System.Text;
using lang.core;

namespace language.lang.core {
    public class AstPrinter : ExpressionVisitor<string> {
        internal string print(Expr expr) {
            return expr.accept(this);
        }

        private string parenthesize(string name, params Expr[] exprs) {
            StringBuilder builder = new StringBuilder();

            builder.Append("(").Append(name);
            foreach (var expr in exprs) {
                builder.Append(" ");
                builder.Append(expr.accept(this));
            }

            builder.Append(")");
            return builder.ToString();
        }
        
        public string visitBinaryExpr(Expr.Binary expr) {

            switch (expr.Op.type) {
                case TokenType.PLUS: return parenthesize("+", expr.Left, expr.Right);
                case TokenType.MINUS: return parenthesize("-", expr.Left, expr.Right);
                default:
                    return parenthesize(expr.Op.Lexeme, expr.Left, expr.Right);;
            }
        }

        public string visitGroupingExpr(Expr.Grouping expr) {
            return parenthesize("group", expr.Expression);
        }

        public string visitLiteralExpr(Expr.Literal expr) {
            if (expr.Value == null) {
                return "none";
            }

            return expr.Value.ToString();
        }

        public string visitUnaryExpr(Expr.Unary expr) {
            return parenthesize(expr.Op.Lexeme, expr.Right);
        }

        public string visitVariable(Expr.Variable expr) {
            throw new System.NotImplementedException();
        }

        public string visitAssign(Expr.Assign assign) {
            throw new System.NotImplementedException();
        }
    }
}
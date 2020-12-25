using lang.core;

namespace language.lang.core {
    public interface ExpressionVisitor<R> {
        R visitBinaryExpr(Expr.Binary expr);
        R visitGroupingExpr(Expr.Grouping expr);
        R visitLiteralExpr(Expr.Literal expr);
        R visitUnaryExpr(Expr.Unary expr);
        R visitVariable(Expr.Variable expr);
        R visitAssign(Expr.Assign assign);
    }
}
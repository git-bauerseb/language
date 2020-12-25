using lang.core;

namespace language.lang.core {
    public interface StmtVisitor<R> {
        R visitExpressionStmt(Stmt.Expression expr);
        R visitPrintStmt(Stmt.Print stmt);
        
        R visitVariableStmt(Stmt.Var expr);
    }
}
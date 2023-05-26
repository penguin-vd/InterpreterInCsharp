using lexer;
namespace ast;

public interface Node {
    public string TokenLiteral();
    public string String();
}

public interface Statement : Node {
    public void StatementNode();
}

public interface Expression : Node {
    public void ExpressionNode();
}

public struct AstProgram : Node {
    public List<Statement> Statements;
    public string TokenLiteral() {
        if (Statements.Count > 0)
            return Statements[0].TokenLiteral();
        else
            return "";
    }
    public string String() {
        string temp = "";
        foreach (Statement statement in Statements) {
            temp += $"{statement.String()}\n";
        }
        return temp;
    }
}

public struct LetStatement : Statement {
    public Token TheToken;
    public Identifier Name;
    public Expression Value;
    public void StatementNode() {}
    public string TokenLiteral() => TheToken.Literal;

    public string String() => $"{TheToken.Literal} {Name.String()} = {(Value != null ? Value.String() : "")};";
}

public struct ReturnStatement : Statement {
    public Token TheToken;
    public Expression ReturnValue;
    public void StatementNode() {}
    public string TokenLiteral() => TheToken.Literal;
    public string String() => $"{TheToken.Literal} {ReturnValue.String()} = {(ReturnValue != null ? ReturnValue.String() : "")};";
}

public struct ExpressionStatement : Statement {
    public Token TheToken;
    public Expression TheExpression;
    public void StatementNode() {}
    public string TokenLiteral() => TheToken.Literal;
    public string String() => TheExpression != null ? TheExpression.String() : "";
}

public struct Identifier : Expression {
    public Token TheToken;
    public string Value;
    public void ExpressionNode() {}
    public string TokenLiteral() => TheToken.Literal;
    public string String() => Value;
}

public struct IntergerLiteral : Expression {
    public Token TheToken;
    public long Value;
    public void ExpressionNode() {}
    public string TokenLiteral() => TheToken.Literal;
    public string String() => TheToken.Literal;
}

public struct PrefixExpression : Expression {
    public Token TheToken;
    public string Operator;
    public Expression Right;
    public void ExpressionNode() {}
    public string TokenLiteral() => TheToken.Literal;
    public string String() => $"({Operator}{Right.String()})";
}

public struct InfixExpression : Expression {
    public Token TheToken;
    public Expression Left;
    public string Operator;
    public Expression Right;
    public void ExpressionNode() {}
    public string TokenLiteral() => TheToken.Literal;
    public string String() => $"({Left.String()} {Operator} {Right.String()})";
}

public struct BooleanExpression : Expression {
    public Token TheToken;
    public bool Value;
    public void ExpressionNode() {}
    public string TokenLiteral() => TheToken.Literal;
    public string String() => TheToken.Literal;
}

public struct IfExpression : Expression {
    public Token TheToken;
    public Expression Condition;
    public BlockStatement Consequence;
    public BlockStatement? Alternative;
    public void ExpressionNode() {}
    public string TokenLiteral() => TheToken.Literal;
    public string String() {
        string tmp = "if";
        tmp += $" {Condition.String()}";
        tmp += $" {Consequence.String()}";
        if (Alternative != null)
            tmp += $" else {Alternative?.String()}";
        return tmp;
    }
}

public struct BlockStatement : Statement{
    public Token TheToken;
    public List<Statement> Statements;
    public void StatementNode() {}
    public string TokenLiteral() => TheToken.Literal;
    public string String() {
        string tmp = "";
        foreach (Statement statement in Statements)
            tmp += statement.String();
        return tmp;
    }
}

public struct FunctionLiteral : Expression {
    public Token TheToken;
    public List<Identifier> Parameters;
    public BlockStatement Body;
    public void ExpressionNode() {}
    public string TokenLiteral() => TheToken.Literal;
    public string String() {
        string[] p = new string[Parameters.Count];
        for (int i = 0; i < Parameters.Count; i++)
            p[i] = Parameters[i].String();
        return $"{TokenLiteral()} ({string.Join(", ", p)}) {Body.String()}";
    }
}

public struct CallExpression : Expression{
    public Token TheToken;
    public Expression Function;
    public List<Expression> Arguments;
    public void ExpressionNode() {}
    public string TokenLiteral() => TheToken.Literal;
    public string String() {
        string[] args = new string[Arguments.Count];
        for (int i = 0; i < Arguments.Count; i++)
            args[i] = Arguments[i].String();
        return $"{Function.String()}({string.Join(", ", args)})";
    }
}

public struct StringLiteral : Expression {
    public Token TheToken;
    public string Value;
    public void ExpressionNode() {}
    public string TokenLiteral() => TheToken.Literal;
    public string String() => TheToken.Literal;
}
using lexer;
namespace ast;

public interface Node {
    public string TokenLiteral();
    public string ToString();
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
    public override string ToString() {
        string temp = "";
        foreach (Statement statement in Statements) {
            temp += $"{statement}\n";
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
    public override string ToString() => $"{TheToken.Literal} {Name} = {(Value != null ? Value.ToString() : "")};";
}

public struct AssignStatement : Statement {
    public Token TheToken;
    public Expression Name;
    public string Operator;
    public Expression Value;
    public void StatementNode() {}
    public string TokenLiteral() => TheToken.Literal;
    public override string ToString() =>  $"{Name} {Operator} {(Value != null ? Value.ToString() : "")};";
}

public struct ReturnStatement : Statement {
    public Token TheToken;
    public Expression ReturnValue;
    public void StatementNode() {}
    public string TokenLiteral() => TheToken.Literal;
    public override string ToString() => $"{TheToken.Literal} {ReturnValue} = {(ReturnValue != null ? ReturnValue.ToString() : "")};";
}

public struct BreakStatement : Statement {
    public Token TheToken;
    public void StatementNode() {}
    public string TokenLiteral() => TheToken.Literal;
    public override string ToString() => $"break";
}

public struct ExpressionStatement : Statement {
    public Token TheToken;
    public Expression TheExpression;
    public void StatementNode() {}
    public string TokenLiteral() => TheToken.Literal;
    public override string ToString() => TheExpression != null ? TheExpression.ToString() : "";
}

public struct Identifier : Expression {
    public Token TheToken;
    public string Value;
    public void ExpressionNode() {}
    public string TokenLiteral() => TheToken.Literal;
    public override string ToString() => Value;
}

public struct IntergerLiteral : Expression {
    public Token TheToken;
    public long Value;
    public void ExpressionNode() {}
    public string TokenLiteral() => TheToken.Literal;
    public override string ToString() => TheToken.Literal;
}

public struct FloatLiteral : Expression {
    public Token TheToken;
    public double Value;
    public void ExpressionNode() {}
    public string TokenLiteral() => TheToken.Literal;
    public override string ToString() => TheToken.Literal;
}

public struct PrefixExpression : Expression {
    public Token TheToken;
    public string Operator;
    public Expression Right;
    public void ExpressionNode() {}
    public string TokenLiteral() => TheToken.Literal;
    public override string ToString() => $"({Operator}{Right})";
}

public struct InfixExpression : Expression {
    public Token TheToken;
    public Expression Left;
    public string Operator;
    public Expression Right;
    public void ExpressionNode() {}
    public string TokenLiteral() => TheToken.Literal;
    public override string ToString() => $"({Left} {Operator} {Right})";
}

public struct BooleanExpression : Expression {
    public Token TheToken;
    public bool Value;
    public void ExpressionNode() {}
    public string TokenLiteral() => TheToken.Literal;
    public override string ToString() => TheToken.Literal;
}

public struct IfExpression : Expression {
    public Token TheToken;
    public Expression Condition;
    public BlockStatement Consequence;
    public BlockStatement? Alternative;
    public void ExpressionNode() {}
    public string TokenLiteral() => TheToken.Literal;
    public override string ToString() {
        string tmp = "if";
        tmp += $" {Condition}";
        tmp += $" {'{'} {Consequence}{'}'}";
        if (Alternative != null)
            tmp += $" else {Alternative}";
        return tmp;
    }
}

public struct ForExpression : Expression {
    public Token TheToken;
    public ForIterative Iterative;
    public BlockStatement Body;
    public void ExpressionNode() {}
    public string TokenLiteral() => TheToken.Literal;
    public override string ToString() => $"for({Iterative}) {'{'} {Body} {'}'}";
}

public struct ForIterative : Expression {
    public Token TheToken;
    public Identifier Index;
    public Expression Array;
    public void ExpressionNode() {}
    public string TokenLiteral() => TheToken.Literal;
    public override string ToString() => $"{Index} in {Array}";
}

public struct WhileExpression : Expression {
    public Token TheToken;
    public Expression Condition;
    public BlockStatement Body;
    public void ExpressionNode() {}
    public string TokenLiteral() => TheToken.Literal;
    public override string ToString() => $"while({Condition}) {'{'} {Body} {'}'}";
}

public struct BlockStatement : Statement {
    public Token TheToken;
    public List<Statement> Statements;
    public void StatementNode() {}
    public string TokenLiteral() => TheToken.Literal;
    public override string ToString() {
        string tmp = "";
        foreach (Statement statement in Statements)
            tmp += $"{statement}; ";
        return tmp;
    }
}

public struct FunctionLiteral : Expression {
    public Token TheToken;
    public List<Identifier> Parameters;
    public BlockStatement Body;
    public void ExpressionNode() {}
    public string TokenLiteral() => TheToken.Literal;
    public override string ToString() {
        string[] p = new string[Parameters.Count];
        for (int i = 0; i < Parameters.Count; i++)
            p[i] = Parameters[i].ToString();
        return $"{TokenLiteral()} ({string.Join(", ", p)}) {Body}";
    }
}

public struct CallExpression : Expression{
    public Token TheToken;
    public Expression Function;
    public List<Expression> Arguments;
    public void ExpressionNode() {}
    public string TokenLiteral() => TheToken.Literal;
    public override string ToString() {
        string[] args = new string[Arguments.Count];
        for (int i = 0; i < Arguments.Count; i++)
            args[i] = Arguments[i].ToString();
        return $"{Function}({string.Join(", ", args)})";
    }
}

public struct StringLiteral : Expression {
    public Token TheToken;
    public string Value;
    public void ExpressionNode() {}
    public string TokenLiteral() => TheToken.Literal;
    public override string ToString() => TheToken.Literal;
}

public struct ArrayLiteral : Expression {
    public Token TheToken;
    public List<Expression> Elements;
    public void ExpressionNode() {}
    public string TokenLiteral() => TheToken.Literal;
    public override string ToString() {
        string[] elements = new string[Elements.Count];
        for (int i = 0; i < Elements.Count; i++)
            elements[i] = Elements[i].ToString();

        return $"[{string.Join(", ", elements)}]";
    }
}

public struct IndexExpression : Expression {
    public Token TheToken;
    public Expression Left;
    public Expression Index;
    public void ExpressionNode() {}
    public string TokenLiteral() => TheToken.Literal;
    public override string ToString() => $"({Left}[{Index}])";
}

public struct HashLiteral : Expression {
    public Token TheToken;
    public Dictionary<Expression, Expression> Pairs;
    public void ExpressionNode() {}
    public string TokenLiteral() => TheToken.Literal;
    public override string ToString() {
        string[] pairs = new string[Pairs.Count];
        int i = 0;
        foreach (var kvp in Pairs) {
            pairs[i] = $"{kvp.Key}:{kvp.Value}";
            i++;
        }
        return $"{'{'}{string.Join(", ", pairs)}{'}'}";
    }
}

using ast;
using lexer;

public delegate Expression? PrefixParseFn();
public delegate Expression? InfixParseFn(Expression expression);

public enum Precedence
{
    LOWEST,
    EQUALS,        // ==
    LESSGREATER,   // > or <
    SUM,           // +
    PRODUCT,       // *
    PREFIX,        // -X or !X
    CALL           // myFunction(X)
}

public class Parser
{
    Lexer lexer;
    Token curToken;
    Token peekToken;
    Dictionary<TokenType, PrefixParseFn> prefixParseFns;
    Dictionary<TokenType, InfixParseFn> infixParseFns;
    Dictionary<TokenType, Precedence> precedences = new Dictionary<TokenType, Precedence>() {
        {TokenType.EQ, Precedence.EQUALS}, {TokenType.NOT_EQ, Precedence.EQUALS}, {TokenType.LT, Precedence.LESSGREATER},
        {TokenType.GT, Precedence.LESSGREATER}, {TokenType.PLUS, Precedence.SUM}, {TokenType.MINUS, Precedence.SUM},
        {TokenType.SLASH, Precedence.PRODUCT}, {TokenType.ASTERISK, Precedence.PRODUCT}, {TokenType.LPAREN, Precedence.CALL}
    };

    public List<string> Errors;

    public Parser(Lexer l) {
        lexer = l;
        Errors = new List<string>();

        prefixParseFns = new Dictionary<TokenType, PrefixParseFn>();
        RegisterPrefix(TokenType.IDENT, ParseIdentifier);
        RegisterPrefix(TokenType.INT, ParseIntegerLiteral);
        RegisterPrefix(TokenType.BANG, ParsePrefixExpression);
        RegisterPrefix(TokenType.MINUS, ParsePrefixExpression);
        RegisterPrefix(TokenType.TRUE, ParseBoolean);
        RegisterPrefix(TokenType.FALSE, ParseBoolean);
        RegisterPrefix(TokenType.LPAREN, ParseGroundedExpression);
        RegisterPrefix(TokenType.IF, ParseIfExpression);
        RegisterPrefix(TokenType.FUNCTION, ParseFunctionLiteral);

        infixParseFns = new Dictionary<TokenType, InfixParseFn>();
        RegisterInfix(TokenType.EQ, ParseInfixExpression);
        RegisterInfix(TokenType.NOT_EQ, ParseInfixExpression);
        RegisterInfix(TokenType.LT, ParseInfixExpression);
        RegisterInfix(TokenType.GT, ParseInfixExpression);
    	RegisterInfix(TokenType.PLUS, ParseInfixExpression);
        RegisterInfix(TokenType.MINUS, ParseInfixExpression);
        RegisterInfix(TokenType.SLASH, ParseInfixExpression);
        RegisterInfix(TokenType.ASTERISK, ParseInfixExpression);
        RegisterInfix(TokenType.LPAREN, ParseCallExpression);
        NextToken();
        NextToken();
    }

    public void NextToken() {
        curToken = peekToken;
        peekToken = lexer.NextToken();
    }

    public AstProgram ParseProgram()
    {
        AstProgram program = new AstProgram();
        program.Statements = new List<Statement>();
        while (curToken.Type != TokenType.EOF) {
            var statement = ParseStatement();
            if (statement != null)
                program.Statements.Add(statement);
            NextToken();
        }
        return program;
    }

    private Statement? ParseStatement()
    {
        switch (curToken.Type) {
            case TokenType.LET:
                return ParseLetStatement();
            case TokenType.RETURN:
                return ParseReturnStatement();
            default:
                return ParseExpressionStatement();
        }
    }

    private LetStatement? ParseLetStatement()
    {
        LetStatement statement = new LetStatement() {TheToken = curToken};
        if (!ExpectPeek(TokenType.IDENT))
            return null;

        statement.Name = new Identifier() {TheToken = curToken, Value = curToken.Literal};

        if (!ExpectPeek(TokenType.ASSIGN))
            return null;

        NextToken();

        statement.Value = ParseExpression(Precedence.LOWEST);

        if (PeekTokenIs(TokenType.SEMICOLON))
            NextToken();

        return statement;
    }

    private ReturnStatement? ParseReturnStatement()
    {
        ReturnStatement statement = new ReturnStatement() {TheToken = curToken};

        NextToken();

        statement.ReturnValue = ParseExpression(Precedence.LOWEST);

        if (PeekTokenIs(TokenType.SEMICOLON))
            NextToken();

        return statement;
    }

    private ExpressionStatement? ParseExpressionStatement()
    {
        ExpressionStatement statement = new ExpressionStatement() {TheToken = curToken};
        statement.TheExpression = ParseExpression(Precedence.LOWEST);
        if (PeekTokenIs(TokenType.SEMICOLON))
            NextToken();

        return statement;
    }

    private Expression? ParseExpression(Precedence pre) {
        if (!prefixParseFns.ContainsKey(curToken.Type)) {
            NoPrefixParseFnError(curToken.Type);
            return null;
        }
        var prefix = prefixParseFns[curToken.Type];
        var leftExp = prefix();

        while(!PeekTokenIs(TokenType.SEMICOLON) && pre < PeekPrecedence()) {
            if (!infixParseFns.ContainsKey(peekToken.Type))
                return leftExp;
            var infix = infixParseFns[peekToken.Type];
            NextToken();
            leftExp = infix(leftExp);
        }
        return leftExp;
    }

    private Expression ParsePrefixExpression() {
        var expression = new PrefixExpression() {TheToken = curToken, Operator = curToken.Literal};
        NextToken();
        expression.Right = ParseExpression(Precedence.PREFIX);
        return expression;
    }

    private Expression ParseInfixExpression(Expression left) {
        InfixExpression expression = new InfixExpression() {
            TheToken = curToken,
            Operator = curToken.Literal,
            Left = left,
        };
        var precedence = CurPrecedence();
        NextToken();
        expression.Right = ParseExpression(precedence);
        return expression;
    }

    private void NoPrefixParseFnError(TokenType t) => Errors.Add($"no prefix parse function for {t} found");

    private Expression? ParseIntegerLiteral() {
        IntergerLiteral literal = new IntergerLiteral() {TheToken = curToken};

        if (!int.TryParse(curToken.Literal, out int value)) {
            Errors.Add($"could not parse {curToken.Literal} as integer");
            return null;
        }
        literal.Value = value;
        return literal;
    }

    private Expression ParseBoolean() => new BooleanExpression() {TheToken = curToken, Value = CurTokenIs(TokenType.TRUE) };

    private Expression? ParseGroundedExpression() {
        NextToken();
        Expression? expression = ParseExpression(Precedence.LOWEST);
        if (!ExpectPeek(TokenType.RPAREN))
            return null;
        return expression;
    }

    private Expression? ParseIfExpression()
    {
        IfExpression expression = new IfExpression() {TheToken = curToken};
        if (!ExpectPeek(TokenType.LPAREN))
            return null;
        NextToken();
        expression.Condition = ParseExpression(Precedence.LOWEST);

        if (!ExpectPeek(TokenType.RPAREN))
            return null;
        if (!ExpectPeek(TokenType.LBRACE))
            return null;

        expression.Consequence = ParseBlockStatement();

        if (PeekTokenIs(TokenType.ELSE)) {
            NextToken();
            if (!ExpectPeek(TokenType.LBRACE))
                return null;
            expression.Alternative = ParseBlockStatement();
        }

        return expression;
    }

    private BlockStatement ParseBlockStatement()
    {
        BlockStatement block = new BlockStatement() {TheToken = curToken};
        block.Statements = new List<Statement>();


        NextToken();
        while (!CurTokenIs(TokenType.RBRACE) && !CurTokenIs(TokenType.EOF)) {
            Statement? statement = ParseStatement();
            if (statement != null)
                block.Statements.Add(statement);
            NextToken();
        }
        return block;
    }

    private Expression? ParseFunctionLiteral()
    {
        FunctionLiteral literal = new FunctionLiteral() {TheToken = curToken};
        if (!ExpectPeek(TokenType.LPAREN))
            return null;

        literal.Parameters = ParseFunctionParameters();
        if (!ExpectPeek(TokenType.LBRACE))
            return null;

        literal.Body = ParseBlockStatement();
        return literal;
    }

    private List<Identifier>? ParseFunctionParameters()
    {
        List<Identifier> identifiers = new();
        if (PeekTokenIs(TokenType.RPAREN)) {
            NextToken();
            return identifiers;
        }
        NextToken();
        Identifier ident = new Identifier{TheToken = curToken, Value = curToken.Literal};
        identifiers.Add(ident);

        while(PeekTokenIs(TokenType.COMMA)) {
            NextToken();
            NextToken();
            ident = new Identifier{TheToken = curToken, Value = curToken.Literal};
            identifiers.Add(ident);
        }
        if (!ExpectPeek(TokenType.RPAREN))
            return null;
        return identifiers;
    }

    private Expression ParseCallExpression(Expression function)
    {
        CallExpression exp = new CallExpression() {TheToken = curToken, Function = function};
        exp.Arguments = ParseCallArugments();
        return exp;
    }

    private List<Expression>? ParseCallArugments()
    {
        List<Expression> args = new List<Expression>();

        if (PeekTokenIs(TokenType.RPAREN)) {
            NextToken();
            return args;
        }

        NextToken();
        args.Add(ParseExpression(Precedence.LOWEST));

        while (PeekTokenIs(TokenType.COMMA)) {
            NextToken();
            NextToken();
            args.Add(ParseExpression(Precedence.LOWEST));
        }

        if (!ExpectPeek(TokenType.RPAREN))
            return null;

        return args;

    }

    private bool CurTokenIs(TokenType t) => curToken.Type == t;
    private bool PeekTokenIs(TokenType t) => peekToken.Type == t;
    private bool ExpectPeek(TokenType t) {
        if (PeekTokenIs(t)) {
            NextToken();
            return true;
        }
        PeekError(t);
        return false;
    }
    private void PeekError(TokenType t) => Errors.Add($"Expected next token to be {t}, got {peekToken.Type} instead");
    public void RegisterPrefix(TokenType tokenType, PrefixParseFn parseFn) => prefixParseFns[tokenType] = parseFn;
    public void RegisterInfix(TokenType tokenType, InfixParseFn parseFn) => infixParseFns[tokenType] = parseFn;
    private Expression ParseIdentifier() => new Identifier() {TheToken = curToken, Value = curToken.Literal};
    private Precedence PeekPrecedence() {
        if (!precedences.ContainsKey(peekToken.Type))
            return Precedence.LOWEST;
        return precedences[peekToken.Type];
    }

    private Precedence CurPrecedence() {
        if (!precedences.ContainsKey(curToken.Type))
            return Precedence.LOWEST;
        return precedences[curToken.Type];
    }
}
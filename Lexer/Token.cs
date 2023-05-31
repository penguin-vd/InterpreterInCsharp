namespace lexer;
public enum TokenType {
    ILLEGAL,
    EOF,
    COMMENT,

    // Identifiers + literals
    IDENT,
    INT,
    STRING,

    // Operators
    ASSIGN,
    PLUS,
    MINUS,
    PLUS_EQ,
    MIN_EQ,
    TIMES_EQ,
    DIVIDE_EQ,

    BANG,
    ASTERISK,
    SLASH,

    LT,
    GT,
    EQ,
    NOT_EQ,

    // Delimiters
    COMMA,
    SEMICOLON,
    COLON,

    LPAREN,
    RPAREN,
    LBRACE,
    RBRACE,
    LBRACKET,
    RBRACKET,

    // Keywords
    FUNCTION,
    LET,
    TRUE,
    FALSE,
    IF,
    ELSE,
    RETURN,
    FOR,
    IN,
    WHILE,
    BREAK
}

public struct Token {
    public TokenType Type;
    public string Literal;
    public Token(TokenType type, string literal) {
        Type = type;
        Literal = literal;
    }
}


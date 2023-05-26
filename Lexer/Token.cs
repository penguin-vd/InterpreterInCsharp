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

    LPAREN,
    RPAREN,
    LBRACE,
    RBRACE,

    // Keywords
    FUNCTION,
    LET,
    TRUE,
    FALSE,
    IF,
    ELSE,
    RETURN,
}

public struct Token {
    public TokenType Type;
    public string Literal;
    public Token(TokenType type, string literal) {
        Type = type;
        Literal = literal;
    }
}


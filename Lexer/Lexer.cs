namespace lexer;
public class Lexer
{
    public string Input;
    int position;
    int readPosition;
    char ch;
    const char eof = char.MinValue;
    Dictionary<string, TokenType> keywords = new Dictionary<string, TokenType>() {
        {"fn", TokenType.FUNCTION},
        {"let", TokenType.LET},
        {"true", TokenType.TRUE},
        {"false", TokenType.FALSE},
        {"if", TokenType.IF},
        {"else", TokenType.ELSE},
        {"return", TokenType.RETURN},
        {"for", TokenType.FOR},
        {"in", TokenType.IN},
        {"while", TokenType.WHILE},
        {"break", TokenType.BREAK}
    };

    public Lexer(string input) {
        Input = input;
        ReadChar();
    }

    public Token NextToken() {
        Token tok;
        SkipWhitespace();
        switch (ch) {
            case '=':
                if (PeekChar() == '=') {
                    char oldCh = ch;
                    ReadChar();
                    tok = new Token(TokenType.EQ, $"{oldCh}{ch}");
                    break;
                }
                tok = new Token(TokenType.ASSIGN, ch.ToString());
                break;
            case ';':
                tok = new Token(TokenType.SEMICOLON, ch.ToString());
                break;
            case '(':
                tok =new Token(TokenType.LPAREN, ch.ToString());
                break;
            case ')':
                tok = new Token(TokenType.RPAREN, ch.ToString());
                break;
            case '{':
                tok = new Token(TokenType.LBRACE, ch.ToString());
                break;
            case '}':
                tok = new Token(TokenType.RBRACE, ch.ToString());
                break;
            case ',':
                tok = new Token(TokenType.COMMA, ch.ToString());
                break;
            case '+':
                if (PeekChar() == '=') {
                    char oldCh = ch;
                    ReadChar();
                    tok = new Token(TokenType.PLUS_EQ, $"{oldCh}{ch}");
                    break;
                }
                tok = new Token(TokenType.PLUS, ch.ToString());
                break;
            case '-':
                if (PeekChar() == '=') {
                    char oldCh = ch;
                    ReadChar();
                    tok = new Token(TokenType.MIN_EQ, $"{oldCh}{ch}");
                    break;
                }
                tok = new Token(TokenType.MINUS, ch.ToString());
                break;
            case '!':
                if (PeekChar() == '=') {
                    char oldCh = ch;
                    ReadChar();
                    tok = new Token(TokenType.NOT_EQ, $"{oldCh}{ch}");
                    break;
                }
                tok = new Token(TokenType.BANG, ch.ToString());
                break;
            case '*':
                if (PeekChar() == '=') {
                    char oldCh = ch;
                    ReadChar();
                    tok = new Token(TokenType.TIMES_EQ, $"{oldCh}{ch}");
                    break;
                }
                tok = new Token(TokenType.ASTERISK, ch.ToString());
                break;
            case '/':
                if (PeekChar() == '/') {
                    tok = new Token(TokenType.COMMENT, SkipLine());
                    break;
                } else if (PeekChar() == '=') {
                    char oldCh = ch;
                    ReadChar();
                    tok = new Token(TokenType.DIVIDE_EQ, $"{oldCh}{ch}");
                    break;
                }
                tok = new Token(TokenType.SLASH, ch.ToString());
                break;
            case '<':
                tok = new Token(TokenType.LT, ch.ToString());
                break;
            case '>':
                tok = new Token(TokenType.GT, ch.ToString());
                break;
            case '"':
                tok = new Token(TokenType.STRING, ReadString());
                break;
            case '[':
                tok = new Token(TokenType.LBRACKET, ch.ToString());
                break;
            case ']':
                tok = new Token(TokenType.RBRACKET, ch.ToString());
                break;
            case ':':
                tok = new Token(TokenType.COLON, ch.ToString());
                break;
            case eof:
                tok = new Token(TokenType.EOF, ch.ToString());
                break;
            default:
                if (char.IsLetter(ch) || ch == '_') {
                    string literal = ReadIdentifier();
                    TokenType tokenType = LookupIdent(literal);
                    return new Token(tokenType, literal);
                } else if (char.IsDigit(ch)) {
                    return ReadNumber();
                }

                tok = new Token(TokenType.ILLEGAL, ch.ToString());
                break;
        }
        ReadChar();
        return tok;
    }

    private void ReadChar() {
        if (readPosition >= Input.Length)
            ch = eof;
        else
            ch = Input[readPosition];
        position = readPosition;
        readPosition += 1;
    }

    private string ReadIdentifier() {
        int oldPos = position;
        while (char.IsLetter(ch) || ch == '_') {
            ReadChar();
        }
        return Input.Substring(oldPos, position - oldPos);
    }

    private Token ReadNumber() {
        int oldPos = position;
        bool isFloat = false;
        while (char.IsDigit(ch) || ch == '.') {
            ReadChar();
            if (ch == '.')
                isFloat = true;
        }
        if (isFloat)
            return new Token(TokenType.FLOAT, Input.Substring(oldPos, position - oldPos));
        return new Token(TokenType.INT, Input.Substring(oldPos, position - oldPos));
    }

    private TokenType LookupIdent(string ident) {
        if (keywords.Keys.Contains(ident))
            return keywords[ident];
        return TokenType.IDENT;
    }

    private string SkipLine() {
        int oldPos = position + 1;
        while (ch != '\n') {
            ReadChar();
        }
        return Input.Substring(oldPos, position - oldPos);
    }

    private void SkipWhitespace() {
        while (char.IsWhiteSpace(ch)) {
            ReadChar();
        }
    }

    private char PeekChar() {
        if (readPosition >= Input.Length)
            return eof;
        else return Input[readPosition];
    }

    private string ReadString() {
        int oldPos = position + 1;
        while (true) {
            ReadChar();
            if (ch == '"' || ch == eof)
                break;
        }
        return Input.Substring(oldPos, position - oldPos);
    }
}
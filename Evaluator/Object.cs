using ast;
using enviroment;

namespace objects;

public enum ObjectType {
    INTEGER,
    BOOLEAN,
    STRING,
    NULL,
    RETURN_VALUE,
    ERROR,
    FUNCTION,
    BUILTIN,
    EXIT,
    INCLUDE
}

public interface IObject {
    public ObjectType Type();
    public string Inspect();
}

public struct ExitObj : IObject {
    public int Value;
    public ObjectType Type() => ObjectType.EXIT;
    public string Inspect() => $"{Value}";
}

public struct Integer : IObject {
    public long Value;
    public ObjectType Type() => ObjectType.INTEGER;
    public string Inspect() => $"{Value}";
}

public struct BooleanObj : IObject {
    public bool Value;
    public ObjectType Type() => ObjectType.BOOLEAN;
    public string Inspect() => $"{Value}";
}

public struct Null : IObject {
    public ObjectType Type() => ObjectType.NULL;
    public string Inspect() => "null";
}

public struct ReturnValue : IObject {
    public IObject Value;
    public ObjectType Type() => ObjectType.RETURN_VALUE;
    public string Inspect() => $"{Value.Inspect()}";
}

public struct Error : IObject {
    public string Message;
    public ObjectType Type() => ObjectType.ERROR;
    public string Inspect() => $"ERROR: {Message}";
}

public struct Function : IObject {
    public List<Identifier> Parameters;
    public BlockStatement Body;
    public Env Env;
    public ObjectType Type() => ObjectType.FUNCTION;
    public string Inspect() {
        string[] p = new string[Parameters.Count];
        for (int i = 0; i < Parameters.Count; i++)
            p[i] = Parameters[i].String();
        return $"fn({string.Join(", ", p)}) {"{\n"}{Body.String()}{"\n}"}";
    }
}

public struct StringObj : IObject {
    public string Value;
    public ObjectType Type() => ObjectType.STRING;
    public string Inspect() => $"'{Value}'";
}

public delegate IObject BuiltinFunction(params IObject[] args);
public struct BuiltinObj : IObject {
    public BuiltinFunction Function;
    public ObjectType Type() => ObjectType.BUILTIN;
    public string Inspect() => "builtin function";
}

public delegate Env IncludeFunction(params IObject[] args);
public struct IncludeObj : IObject {
    public IncludeFunction Function;
    public ObjectType Type() => ObjectType.INCLUDE;
    public string Inspect() => "include function";
}

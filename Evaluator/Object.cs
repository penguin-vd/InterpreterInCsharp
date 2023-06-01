using ast;
using enviroment;

namespace objects;

public enum ObjectType {
    INTEGER,
    FLOAT,
    BOOLEAN,
    STRING,
    NULL,
    RETURN_VALUE,
    ERROR,
    FUNCTION,
    BUILTIN,
    EXIT,
    INCLUDE,
    ARRAY,
    HASH,
    BREAK,
}

public interface IObject {
    public ObjectType Type();
    public string Inspect();
}

public interface IHashable {
    public HashKey HashKey();
}

public struct ExitObj : IObject {
    public int Value;
    public ObjectType Type() => ObjectType.EXIT;
    public string Inspect() => $"{Value}";
}

public struct Integer : IObject, IHashable {
    public long Value;
    public ObjectType Type() => ObjectType.INTEGER;
    public string Inspect() => $"{Value}";
    public HashKey HashKey() => new HashKey(this);
}

public struct Float : IObject, IHashable {
    public double Value;
    public ObjectType Type() => ObjectType.FLOAT;
    public string Inspect() => $"{Value}";
    public HashKey HashKey() => new HashKey(this);
}

public struct BooleanObj : IObject, IHashable {
    public bool Value;
    public ObjectType Type() => ObjectType.BOOLEAN;
    public string Inspect() => $"{Value}";
    public HashKey HashKey() => new HashKey(this);
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

public struct BreakObj : IObject {
    public ObjectType Type() => ObjectType.BREAK;
    public string Inspect() => $"break";
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
            p[i] = Parameters[i].ToString();
        return $"fn({string.Join(", ", p)}) {"{\n"}{Body.ToString()}{"\n}"}";
    }
}

public struct StringObj : IObject, IHashable {
    public string Value;
    public ObjectType Type() => ObjectType.STRING;
    public string Inspect() => $"{Value}";
    public HashKey HashKey() => new HashKey(this);
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

public struct ArrayObj : IObject {
    public List<IObject> Elements;
    public ObjectType Type() => ObjectType.ARRAY;
    public string Inspect() {
        string[] elements = new string[Elements.Count];
        for (int i = 0; i < Elements.Count; i++)
            elements[i] = Elements[i].Inspect();

        return $"[{string.Join(", ", elements)}]";
    }
}

public struct HashKey {
    public ObjectType Type;
    public UInt64 Value;

    public HashKey(BooleanObj boolean) {
        if (boolean.Value)
            Value = 1;
        else Value = 0;

        Type = boolean.Type();
    }

    public HashKey(Integer interger) {
        Value = (UInt64)interger.Value;
        Type = interger.Type();
    }

    public HashKey(StringObj stringObj) {
        char[] chars = stringObj.Value.ToArray();
        Value = 0;
        foreach (char ch in chars)
            Value += ((byte)ch);
        Type = ObjectType.STRING;
    }
    public HashKey(Float flt) {
        Value = (UInt64)flt.Value;
        Type = flt.Type();
    }
}

public struct HashPair {
    public IObject Key;
    public IObject Value;
}

public struct Hash : IObject
{
    public Dictionary<HashKey, HashPair> Pairs;
    public ObjectType Type() => ObjectType.HASH;
    public string Inspect() {
        string[] pairs = new string[Pairs.Count];
        int i = 0;
        foreach (var kvp in Pairs) {
            pairs[i] = $"{kvp.Value.Key.Inspect()}: {kvp.Value.Value.Inspect()}";
            i++;
        }
        return $"{'{'}{string.Join(", ", pairs)}{'}'}";
    }
}

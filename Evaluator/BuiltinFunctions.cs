using objects;
using enviroment;
using Eval;
using lexer;

namespace BuiltinFn;
public static class BuiltinFunctions {
    public static IObject GetLength(params IObject[] args) {
        if (args.Length != 1)
            return NewError($"wrong number of arguments. got {args.Length} want 1");

        switch (args[0].Type()) {
            case ObjectType.STRING:
                return new Integer() { Value = ((StringObj)args[0]).Value.Length };
            case ObjectType.ARRAY:
                return new Integer() { Value = ((ArrayObj)args[0]).Elements.Count };
            case ObjectType.HASH:
                return new Integer() { Value = ((Hash)args[0]).Pairs.Count };
            default:
                return NewError("argument to `len` not supported, got {0}", args[0].Type());
        }
    }

    public static IObject PrintLine(params IObject[] args) {
        if (args.Length != 1)
            return NewError($"wrong number of arguments. got {args.Length} want 1");

        Console.WriteLine(args[0].Inspect());
        return Evaluator.NULL;
    }
    public static IObject Print(params IObject[] args) {
        if (args.Length != 1)
            return NewError($"wrong number of arguments. got {args.Length} want 1");

        Console.Write(args[0].Inspect());
        return Evaluator.NULL;
    }

    public static IObject ToInt(params IObject[] args) {
        if (args.Length != 1)
            return NewError($"wrong number of arguments. got {args.Length} want 1");

        switch (args[0]) {
            case StringObj str:
                if (long.TryParse(str.Value, out long value))
                    return new Integer() { Value = value };
                return NewError("value '{0}' could not parse to int.", str.Value);
            case Integer:
                return args[0];
            case BooleanObj boolean:
                if (boolean.Value)
                    return new Integer() { Value = 1 };
                return new Integer() { Value = 0 };
            case Float fl:
                return new Integer() { Value = (long)fl.Value };
            default:
                return NewError("argument to `int` not supported, got {0}", args[0].Type());
        }
    }

    public static IObject IsDigit(params IObject[] args) {
        if (args.Length != 1)
            return NewError($"wrong number of arguments. got {args.Length} want 1");

        switch (args[0]) {
            case StringObj str:
                return new BooleanObj() { Value = long.TryParse(str.Value, out long _) };
            case Integer:
                return Evaluator.TRUE;
            default:
                return Evaluator.FALSE;
        }
    }

    public static IObject Read(params IObject[] args) {
        if (args.Length >= 1)
            Print(args[0]);

        string? input = Console.ReadLine();
        return new StringObj() { Value = input == null ? "" : input };
    }

    public static IObject ToString(params IObject[] args) {
        if (args.Length != 1)
            return NewError($"wrong number of arguments. got {args.Length} want 1");

        switch (args[0]) {
            case StringObj:
                return args[0];
            case Integer integer:
                return new StringObj() { Value = integer.Inspect() };
            case BooleanObj boolean:
                return new StringObj() { Value = boolean.Inspect() };
            case Float fl:
                return new StringObj() { Value = fl.Inspect() };
            default:
                return NewError("argument to `int` not supported, got {0}", args[0].Type());
        }
    }

    public static IObject Exit(params IObject[] args) {
        return new ExitObj() { Value = 0 };
    }

    public static Env Include(params IObject[] args) {
        if (args.Length != 1)
            return new Env() { Store = new Dictionary<string, IObject>() {{"ERROR", NewError("wrong number of arguments. got {0} want 1", args.Length)}}};

        if (args[0].Type() != ObjectType.STRING)
            return new Env() { Store = new Dictionary<string, IObject>() {{"ERROR", NewError("argument not supported, include wants a STRING", args.Length)}}};

        string fileName = ((StringObj)args[0]).Value;
        if (!fileName.Contains('.'))
            fileName += ".bigl";

        if (File.Exists(fileName)) {
            using (StreamReader reader = new StreamReader(fileName)) {
                Lexer l = new Lexer(reader.ReadToEnd());
                Parser p = new Parser(l);
                Env env = new Env();
                env.Set("_name", new StringObj() { Value = "_include" });
                Evaluator.Eval(p.ParseProgram(), env);
                return env; }
        }
        var pathOut = Path();
        if (pathOut.Type() == ObjectType.ERROR)
            return new Env() { Store = new Dictionary<string, IObject>() {{"ERROR", pathOut}}};

        string path = ((StringObj)pathOut).Value + $"\\{fileName}";
        if (File.Exists(path)) {
             using (StreamReader reader = new StreamReader(path)) {
                Lexer l = new Lexer(reader.ReadToEnd());
                Parser p = new Parser(l);
                Env env = new Env();
                env.Set("_name", new StringObj() { Value = "_include" });
                Evaluator.Eval(p.ParseProgram(), env);
                return env; }
        }

        return new Env() { Store = new Dictionary<string, IObject>() {{"ERROR", NewError("file {0} was not found", fileName)}}};
    }

    public static IObject Path(params IObject[] args) {
        if (args.Length != 0)
            return NewError($"wrong number of arguments. got {args.Length} want 0");

        string? path = Environment.GetEnvironmentVariable("bigl", EnvironmentVariableTarget.User);
        if (path != null)
            return new StringObj() { Value = path };

        return NewError("path of 'bigl' was not set correctly");
    }

    public static IObject First(params IObject[] args) {
        if (args.Length != 1)
            return NewError($"wrong number of arguments. got {args.Length} want 1");

        if (args[0].Type() != ObjectType.ARRAY)
            return NewError("argument to 'first' must be ARRAY got {0}", args[0].Type());

        var arr = (ArrayObj)args[0];
        if (arr.Elements.Count > 0)
            return arr.Elements[0];

        return Evaluator.NULL;
    }

    public static IObject Last(params IObject[] args) {
        if (args.Length != 1)
            return NewError($"wrong number of arguments. got {args.Length} want 1");

        if (args[0].Type() != ObjectType.ARRAY)
            return NewError("argument to 'first' must be ARRAY got {0}", args[0].Type());

        var arr = (ArrayObj)args[0];
        if (arr.Elements.Count > 0)
            return arr.Elements.Last();

        return Evaluator.NULL;
    }

    public static IObject Push(params IObject[] args) {
        if (args.Length != 2)
            return NewError("wrong number of arguments. got={0}, want=2", args.Length);
        if (args[0].Type() != ObjectType.ARRAY)
            return NewError("argument to `push` must be ARRAY, got {0}", args[0].Type());
        var arr = (ArrayObj)args[0];
        var newArr = new ArrayObj() { Elements = new List<IObject>(arr.Elements) };
        newArr.Elements.Add(args[1]);
        return newArr;
    }

    public static IObject Rest(params IObject[] args) {
        if (args.Length != 1)
            return NewError("wrong number of arguments. got={0}, want=1", args.Length);
        if (args[0].Type() != ObjectType.ARRAY)
            return NewError("argument to `rest` must be ARRAY, got {0}", args[0].Type());
        var arr = (ArrayObj)args[0];
        if (arr.Elements.Count > 0) {
            return new ArrayObj() { Elements = arr.Elements.GetRange(1, arr.Elements.Count - 1) };
        }
        return Evaluator.NULL;
    }

    public static IObject Range(params IObject[] args) { 
        if (args.Length != 2)
            return NewError("wrong number of arguments. got={0}, want=2", args.Length);
        if (args[0].Type() != ObjectType.INTEGER || args[1].Type() != ObjectType.INTEGER)
            return NewError("arguments to 'range' must be INTEGER, got {0}, {1}", args[0].Type(), args[1].Type());
        
        long low = ((Integer)args[0]).Value;
        long high = ((Integer)args[1]).Value;
        return new IterObj() { Low = low, High = high, Steps = 1 };
    }

    public static IObject Collect(params IObject[] args) {
        if (args.Length != 0) {
            return NewError("wrong number of arguments. got={0}, want=0", args.Length);
        }

        GC.Collect();
        return Evaluator.NULL;
    }

    public static IObject Clear(params IObject[] args) {
        if (args.Length > 1) {
            return NewError("wrong number of arguments. got={0}, want=0 or 1", args.Length);
        }

        if (args.Length == 0) {
            Console.Clear();
            return Evaluator.NULL;
        }
        
        Console.Clear();
        Console.WriteLine(args[0].Inspect());
        return Evaluator.NULL;
    }

    private static Error NewError(string format, params object[] args) {
        return new Error() {Message = string.Format(format, args)};
    }
}

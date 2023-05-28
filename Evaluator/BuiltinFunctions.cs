using objects;
using enviroment;
using Eval;
using lexer;

namespace BuiltinFn;
public static class BuiltinFunctions {
    public static readonly Null NULL = new Null();
    public static IObject GetLength(params IObject[] args) {
        if (args.Length != 1)
            return NewError($"wrong number of arguments. got {args.Length} want 1");

        switch (args[0].Type()) {
            case ObjectType.STRING:
                return new Integer() {Value = ((StringObj)args[0]).Value.Length };
            default:
                return NewError("argument to `len` not supported, got {0}", args[0].Type());
        }
    }

    public static IObject PrintLine(params IObject[] args) {
        if (args.Length != 1)
            return NewError($"wrong number of arguments. got {args.Length} want 1");

        switch (args[0].Type()) {
            case ObjectType.STRING:
                Console.WriteLine(((StringObj)args[0]).Value);
                return Evaluator.NULL;
            case ObjectType.INTEGER:
                Console.WriteLine(((Integer)args[0]).Value);
                return Evaluator.NULL;
            case ObjectType.BOOLEAN:
                Console.WriteLine(((BooleanObj)args[0]).Value);
                return Evaluator.NULL;
            default:
                return NewError("argument to `print` not supported, got {0}", args[0].Type());
        }
    }
    public static IObject Print(params IObject[] args) {
        if (args.Length != 1)
            return NewError($"wrong number of arguments. got {args.Length} want 1");

        switch (args[0].Type()) {
            case ObjectType.STRING:
                Console.Write(((StringObj)args[0]).Value);
                return Evaluator.NULL;
            case ObjectType.INTEGER:
                Console.Write(((Integer)args[0]).Value);
                return Evaluator.NULL;
            default:
                return NewError("argument to `print` not supported, got {0}", args[0].Type());
        }
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
            return NewError("wrong number of arguments. got {args.Length} want 1");

        string? path = Environment.GetEnvironmentVariable("bigl", EnvironmentVariableTarget.User);
        if (path != null)
            return new StringObj() { Value = path };

        return NewError("path of 'bigl' was not set correctly");
    }

    private static Error NewError(string format, params object[] args) {
        return new Error() {Message = string.Format(format, args)};
    }

}
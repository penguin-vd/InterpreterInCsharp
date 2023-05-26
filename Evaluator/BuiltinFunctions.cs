using objects;
using Eval;

namespace BuiltinFn;
public static class BuiltinFunctions {
    public static readonly Null NULL = new Null();
    public static IObject GetLength(params IObject[] args) {
        if (args.Length != 1)
            return NewError($"Wrong number of arguments. got {args.Length} want 1");

        switch (args[0].Type()) {
            case ObjectType.STRING:
                return new Integer() {Value = ((StringObj)args[0]).Value.Length };
            default:
                return NewError("argument to `len` not supported, got {0}", args[0].Type());
        }
    }

    public static IObject PrintLine(params IObject[] args) {
        if (args.Length != 1)
            return NewError($"Wrong number of arguments. got {args.Length} want 1");

        switch (args[0].Type()) {
            case ObjectType.STRING:
                Console.WriteLine(((StringObj)args[0]).Value);
                return Evaluator.NULL;
            case ObjectType.INTEGER:
                Console.WriteLine(((Integer)args[0]).Value);
                return Evaluator.NULL;
            default:
                return NewError("argument to `print` not supported, got {0}", args[0].Type());
        }
    }
    public static IObject Print(params IObject[] args) {
        if (args.Length != 1)
            return NewError($"Wrong number of arguments. got {args.Length} want 1");

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

    public static IObject Exit(params IObject[] args) {
        return new ExitObj() { Value = 0 };
    }

    private static Error NewError(string format, params object[] args) {
        return new Error() {Message = string.Format(format, args)};
    }
}
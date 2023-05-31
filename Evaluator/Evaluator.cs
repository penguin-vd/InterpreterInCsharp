using objects;
using ast;
using enviroment;
using BuiltinFn;

namespace Eval;
public static class Evaluator
{
    public static readonly BooleanObj TRUE = new BooleanObj() {Value = true};
    public static readonly BooleanObj FALSE = new BooleanObj() {Value = false};
    public static readonly Null NULL = new Null();
    public static Dictionary<string, IObject> Builtins = new() {
        {"len", new BuiltinObj() { Function = BuiltinFunctions.GetLength } },
        {"println", new BuiltinObj() { Function = BuiltinFunctions.PrintLine } },
        {"print", new BuiltinObj() { Function = BuiltinFunctions.Print } },
        {"exit", new BuiltinObj() { Function = BuiltinFunctions.Exit } },
        {"read", new BuiltinObj() { Function = BuiltinFunctions.Read} },
        {"toInt", new BuiltinObj() { Function = BuiltinFunctions.ToInt } },
        {"toStr", new BuiltinObj() { Function = BuiltinFunctions.ToString } },
        {"isDigit", new BuiltinObj() { Function = BuiltinFunctions.IsDigit } },
        {"include", new IncludeObj() { Function = BuiltinFunctions.Include } },
        {"path", new BuiltinObj() { Function = BuiltinFunctions.Path } },
        {"first", new BuiltinObj() { Function = BuiltinFunctions.First } },
        {"last", new BuiltinObj() { Function = BuiltinFunctions.Last } },
        {"push", new BuiltinObj() { Function = BuiltinFunctions.Push } },
        {"rest", new BuiltinObj() { Function = BuiltinFunctions.Rest } }, 
        {"range", new BuiltinObj() { Function = BuiltinFunctions.Range } }
    };
    public static IObject Eval(Node node, Env env) {
        switch (node) {
            case AstProgram program:
                return EvalProgram(program, env);
            case BlockStatement blockStatement:
                return EvalBlockStatement(blockStatement, env);
            case IfExpression ifExpression:
                return EvalIfExpression(ifExpression, env);
            case ExpressionStatement statement:
                return Eval(statement.TheExpression, env);
            case IntergerLiteral intergerLiteral:
                return new Integer() {Value = intergerLiteral.Value};
            case BooleanExpression boolean:
                return NativeBoolToBooleanObj(boolean.Value);
            case StringLiteral stringLiteral:
                return new StringObj() { Value = stringLiteral.Value };
            case PrefixExpression prefix:
                var preRight = Eval(prefix.Right, env);
                if (IsError(preRight))
                    return preRight;
                return EvalPrefixExpression(prefix.Operator, preRight);
            case InfixExpression infix:
                var inLeft = Eval(infix.Left, env);
                if (IsError(inLeft))
                    return inLeft;
                var inRight = Eval(infix.Right, env);
                if (IsError(inRight))
                    return inRight;
                return EvalInfixExpression(infix.Operator, inLeft, inRight);
            case ReturnStatement returnStatement:
                var retVal = Eval(returnStatement.ReturnValue, env);
                if (IsError(retVal))
                    return retVal;
                return new ReturnValue() {Value = retVal};
            case LetStatement letStatement:
                var letVal = Eval(letStatement.Value, env);
                if (IsError(letVal))
                    return letVal;
                if (Builtins.ContainsKey(letStatement.Name.Value))
                    return NewError("{0} is a builtin function", letStatement.Name.Value);
                env.Set(letStatement.Name.Value, letVal);
                break;
            case AssignStatement assignStatement:
                var assignVal = Eval(assignStatement.Value, env);
                if(IsError(assignVal))
                    return assignVal;
                var assignRes = EvalAssignStatement(assignStatement, assignVal, env);
                if (IsError(assignRes))
                    return assignRes;
                break;
            case Identifier identifier:
                return EvalIdentifier(identifier, env);
            case FunctionLiteral functionLiteral:
                var fnParams = functionLiteral.Parameters;
                var body = functionLiteral.Body;
                return new Function() {Parameters = fnParams, Env = env, Body = body};
            case CallExpression callExpression:
                var function = Eval(callExpression.Function, env);
                if (IsError(function))
                    return function;
                var callArgs = EvalExpressions(callExpression.Arguments, env);
                if (callArgs.Count == 1 && IsError(callArgs[0]))
                    return callArgs[0];
                return ApplyFunction(function, callArgs, env);
            case ArrayLiteral arrayLiteral:
                var elements = EvalExpressions(arrayLiteral.Elements, env);
                if (elements.Count == 1 && IsError(elements[0]))
                    return elements[0];
                return new ArrayObj() { Elements = elements };
            case IndexExpression indexExpression:
                var left = Eval(indexExpression.Left, env);
                if (IsError(left))
                    return left;

                var index = Eval(indexExpression.Index, env);
                if (IsError(index))
                    return index;
                return EvalIndexExpression(left, index);
            case HashLiteral hashLiteral:
                return EvalHashLiteral(hashLiteral, env);
            case ForExpression forExpression:
                EvalForExpression(forExpression, env);
                break;
        }
        return NULL;
    }

    private static IObject EvalProgram(AstProgram program, Env env) {
        IObject result = NULL;
        foreach (var statement in program.Statements) {
            result = Eval(statement, env);
            if (result.Type() == ObjectType.RETURN_VALUE)
                return ((ReturnValue)result).Value;
            if (result.Type() == ObjectType.ERROR)
                return result;
            if (result.Type() == ObjectType.EXIT)
                return result;
        }
        return result;
    }

    private static IObject EvalBlockStatement(BlockStatement block, Env env) {
        IObject result = NULL;
        foreach (Statement statement in block.Statements) {
            result = Eval(statement, env);
            if (result.Type() == ObjectType.RETURN_VALUE || result.Type() == ObjectType.ERROR)
                return result;

        }
        return result;
    }

    private static BooleanObj NativeBoolToBooleanObj(bool input) {
        if (input)
            return TRUE;
        else return FALSE;
    }

    private static IObject EvalAssignStatement(AssignStatement statement, IObject value, Env env) {
        switch (statement.Name) {
            case Identifier identifier:
                if (env.GetObject(identifier.Value) == null)
                    return NewError("variable with name {0} has not been found", identifier.Value);
                env.Set(identifier.Value, value);
                break;
            case IndexExpression indexExpression:
                var left = Eval(indexExpression.Left, env);
                if (IsError(left))
                    return left;

                var index = Eval(indexExpression.Index, env);
                if (IsError(index))
                    return index;
                env.Set(indexExpression.Left.TokenLiteral(), index, value);
                break;
        }
        return NULL;
    }

    private static IObject EvalPrefixExpression(string op, IObject obj) {
        switch (op) {
            case "!":
                return EvalBangOperatorExpression(obj);
            case "-":
                return EvalMinusOperatorExpression(obj);
            default:
                return NewError("unkown operator: {0}{1}", op, obj.Type());
        }
    }

    private static IObject EvalBangOperatorExpression(IObject obj) {
        switch (obj) {
            case BooleanObj boolean:
                if (boolean.Value == TRUE.Value)
                    return FALSE;
                return TRUE;
            case Null:
                return TRUE;
        }
        return FALSE;
    }

    private static IObject EvalMinusOperatorExpression(IObject obj) {
        if (obj.Type() != ObjectType.INTEGER)
            return NewError("unknown operator: -{0}", obj.Type());;
        return new Integer() {Value = -((Integer)obj).Value};
    }

    private static IObject EvalInfixExpression(string op, IObject left, IObject right) {
        if (left.Type() != right.Type())
            return NewError("type mismatch: {0} {1} {2}", left.Type(), op, right.Type());

        if (left.Type() == ObjectType.INTEGER && right.Type() == ObjectType.INTEGER)
            return EvalIntegerInfixExpression(op, left, right);

        if (left.Type() == ObjectType.STRING && right.Type() == ObjectType.STRING)
            return EvalStringInfixExpression(op, left, right);

        switch (op) {
            case "==":
                return NativeBoolToBooleanObj(left.Inspect() == right.Inspect());
            case "!=":
                return NativeBoolToBooleanObj(left.Inspect() != right.Inspect());
        }
        return NewError("unknown operator: {0} {1} {2}", left.Type(), op, right.Type());;
    }

    private static IObject EvalIntegerInfixExpression(string op, IObject left, IObject right) {
        long leftVal = ((Integer)left).Value;
        long rightVal = ((Integer)right).Value;
        switch (op) {
            case "+":
                return new Integer() { Value = leftVal + rightVal};
            case "-":
                return new Integer() { Value = leftVal - rightVal};
            case "*":
                return new Integer() { Value = leftVal * rightVal};
            case "/":
                return new Integer() { Value = leftVal / rightVal};
            case "<":
                return NativeBoolToBooleanObj(leftVal < rightVal);
            case ">":
                return NativeBoolToBooleanObj(leftVal > rightVal);
            case "==":
                return NativeBoolToBooleanObj(leftVal == rightVal);
            case "!=":
                return NativeBoolToBooleanObj(leftVal != rightVal);
            default:
                return NewError("unknown operator: {0} {1} {2}", left.Type(), op, right.Type());
        }
    }

    private static IObject EvalStringInfixExpression(string op, IObject left, IObject right) {
        string leftVal = ((StringObj)left).Value;
        string rightVal = ((StringObj)right).Value;
        switch (op) {
            case "+":
                return new StringObj() {Value = leftVal + rightVal};
            case "==":
                return NativeBoolToBooleanObj(leftVal == rightVal);
            case "!=":
                return NativeBoolToBooleanObj(leftVal != rightVal);
            default:
                return NewError("unknown operator: {0} {1} {2}", left.Type(), op, right.Type());
        }
    }

    private static IObject EvalIfExpression(IfExpression ie, Env env) {
        var condition = Eval(ie.Condition, env);
        if (IsError(condition))
            return condition;

        if (IsTruthy(condition))
            return Eval(ie.Consequence, env);
        else if (ie.Alternative != null)
            return Eval(ie.Alternative, env);
        return NULL;
    }

    private static IObject EvalIdentifier(Identifier node, Env env) {
        var res = env.GetObject(node.Value);
        if (res != null)
            return res;
        if (Builtins.ContainsKey(node.Value)) {
            return Builtins[node.Value];
        }
        return NewError("identifier not found: " + node.Value);
    }

    private static List<IObject> EvalExpressions(List<Expression> exps, Env env) {
        List<IObject> result = new List<IObject>();
        foreach (var e in exps) {
            var evalutated = Eval(e, env);
            if (IsError(evalutated))
                return new List<IObject>() {evalutated};
            result.Add(evalutated);
        }
        return result;
    }

    private static IObject EvalIndexExpression(IObject left, IObject index) {
        if (left.Type() == ObjectType.ARRAY && index.Type() == ObjectType.INTEGER)
            return EvalArrayIndexExpression((ArrayObj)left, (Integer)index);
        if (left.Type() == ObjectType.HASH)
            return EvalHashIndexExpression((Hash)left, index);
        return NewError("index operator not supported: {0}", left.Type());
    }

    private static IObject EvalArrayIndexExpression(ArrayObj array, Integer index) {
        long idx = index.Value;
        if (0 > idx || idx > array.Elements.Count)
            return NULL;
        return array.Elements[(int)idx];
    }

    private static IObject EvalHashIndexExpression(Hash hash, IObject index) {
        if (index is not IHashable)
            return NewError("unusable as has key: {0}", index.Type());

        if (!hash.Pairs.ContainsKey((index as IHashable).HashKey()))
            return NULL;

        return hash.Pairs[(index as IHashable).HashKey()].Value;
    }

    private static void EvalForExpression(ForExpression expression, Env env) {
        var array = Eval(expression.Iterative.Array, env);
        switch (array) {
            case ArrayObj arrayObj:
                foreach(var obj in arrayObj.Elements) {
                    env.Set(expression.Iterative.Index.Value, obj);
                    Eval(expression.Body, env);
                }
                break;
        }
        env.Remove(expression.Iterative.Index.Value);
    }

    private static IObject EvalHashLiteral(HashLiteral node, Env env) {
        var pairs = new Dictionary<HashKey, HashPair>();
        foreach (var kvpNode in node.Pairs) {
            var key = Eval(kvpNode.Key, env);
            if (IsError(key))
                return key;

            if (key is not IHashable)
                return NewError("unusable as has key: {0}", key.Type());

            var value = Eval(kvpNode.Value, env);
            if (IsError(value))
                return value;


            pairs[(key as IHashable).HashKey()] = new HashPair() { Key = key, Value = value };
        }
        return new Hash() { Pairs = pairs };
    }

    private static IObject ApplyFunction(IObject fn, List<IObject> args, Env env) {
        switch (fn.Type()) {
            case ObjectType.FUNCTION:
                Function function = (Function)fn;
                var extEnv = ExtendFunctionEnv(function, args);
                var evalutated = Eval(function.Body, extEnv);
                return UnWrapReturnValue(evalutated);
            case ObjectType.BUILTIN:
                return ((BuiltinObj)fn).Function(args.ToArray());
            case ObjectType.INCLUDE:
                return env.AddEnv(((IncludeObj)fn).Function(args.ToArray()));
            default:
                return NewError("not a function: {0}", fn.Type());
        }
    }

    private static Env ExtendFunctionEnv(Function fn, List<IObject> args) {
        var extEnv = Env.NewEnclosedEnviroment(fn.Env);
        for (int i = 0; i < fn.Parameters.Count; i++) {
            extEnv.Set(fn.Parameters[i].Value, args[i]);
        }
        return extEnv;
    }

    private static IObject UnWrapReturnValue(IObject obj) {
        if (obj.Type() == ObjectType.RETURN_VALUE)
            return ((ReturnValue)obj).Value;
        return obj;
    }

    private static bool IsTruthy(IObject obj) {
        if (obj.Type() != ObjectType.BOOLEAN && obj.Type() != ObjectType.NULL)
            return true;
        if (obj.Type() == ObjectType.BOOLEAN)
            return ((BooleanObj)obj).Value == true;
        return false;
    }
    private static Error NewError(string format, params object[] args) {
        return new Error() {Message = string.Format(format, args)};
    }

    private static bool IsError(IObject obj) {
        if (obj != null)
            return obj.Type() == ObjectType.ERROR;
        return false;

    }
}

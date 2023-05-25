using lexer;
using repl;
using Eval;
using objects;
using enviroment;
using Newtonsoft.Json;


public static class Program
{
    public static Dictionary<string, int> Inputs = new Dictionary<string, int>() {
        {"let identity = fn(x) { x; }; identity(5);", 5},
    };
    public static void Main(string[] args)
    {
        if (args.Length == 0)
            StartRepl();

        OpenFile(args[0]);
    }
    private static void StartRepl()
    {
        Console.WriteLine("Welkom! This is the BIGL programming language!");
        Console.WriteLine("Feel free to type in commands");
        Repl.Start();
    }
    private static void OpenFile(string file)
    {
        using (StreamReader reader = new(file)){
            Lexer lexer = new Lexer(reader.ReadToEnd());
            Parser parser = new Parser(lexer);
            Env env = new();
            var evalutated = Evaluator.Eval(parser.ParseProgram(), env);
            if (evalutated.Type() != ObjectType.NULL)
                Console.WriteLine(evalutated.Inspect());
        };
    }
}

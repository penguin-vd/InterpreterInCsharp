using lexer;
using repl;
using Eval;
using objects;
using enviroment;

public static class Program
{
    public static void Main(string[] args)
    {
        Console.Clear();
        if (args.Length == 0) {
            StartRepl();
            return;
        }
        OpenFile(args[0]);
    }
    private static void StartRepl()
    {
        Console.WriteLine("Welcome! This is the BIG-L programming language!");
        Console.WriteLine("Feel free to type in commands");
        Repl.Start();
    }
    private static void OpenFile(string file)
    {
        try {
            using (StreamReader reader = new(file)){
                Lexer lexer = new Lexer(reader.ReadToEnd());
                Parser parser = new Parser(lexer);
                Env env = new();
                var evalutated = Evaluator.Eval(parser.ParseProgram(), env);
                if (evalutated.Type() != ObjectType.NULL)
                    Console.WriteLine(evalutated.Inspect()); };
            } catch (FileNotFoundException) {
                Console.WriteLine($"File {file} was not found.");
            }
    }
}

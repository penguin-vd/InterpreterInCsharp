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
        if (args[0].ToLower() == "--setpath") {
            Environment.SetEnvironmentVariable("bigl", Directory.GetCurrentDirectory(), EnvironmentVariableTarget.User);
            return;
        }
        OpenFile(args[0]);
    }
    private static void StartRepl()
    {
        CheckPath();
        Console.WriteLine("Welcome! This is the BIG-L programming language!");
        Console.WriteLine("Feel free to type in commands");
        Repl.Start();
    }
    private static void OpenFile(string file)
    {
        CheckPath();
        try {
            using (StreamReader reader = new(file)){
                Lexer lexer = new Lexer(reader.ReadToEnd());
                Parser parser = new Parser(lexer);
                Env env = new Env();
                env.Set("_name", new StringObj() { Value = "_main" });
                var evalutated = Evaluator.Eval(parser.ParseProgram(), env);
                if (evalutated.Type() != ObjectType.NULL)
                    Console.WriteLine(evalutated.Inspect());
                };
            } catch (FileNotFoundException) {
                Console.WriteLine($"File {file} was not found.");
            }
    }

    private static void CheckPath() {
        if (Environment.GetEnvironmentVariable("bigl", EnvironmentVariableTarget.User) == null)
            Console.WriteLine("Please set bigl to an enviroment variable.\nYou can do this by calling bigl --setpath in the file directory");
    }
}

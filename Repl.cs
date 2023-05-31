using lexer;
using enviroment;
using objects;
using Eval;

namespace repl;

static public class Repl
{
    const string PROMPT = ">> ";
    public static void Start()
    {
        Env env = new Env();
        env.Set("_name", new StringObj() { Value = "_main" });
        while(true) {
            Console.Write(PROMPT);
            string? line = Console.ReadLine();
            if (line != null) {
                Lexer lexer = new Lexer(line);
                Parser parser = new Parser(lexer);
                var program = parser.ParseProgram();
                if (parser.Errors.Count != 0) {
                    foreach (string error in parser.Errors)
                        Console.WriteLine(error);
                    continue;
                }
                var evalutated = Evaluator.Eval(program, env);
                if (evalutated.Type() == ObjectType.EXIT) {
                    Console.WriteLine($"exited the program with code {((ExitObj)evalutated).Value}");
                    break;
                }
                if (evalutated.Type() != ObjectType.NULL) {
                    Console.WriteLine(evalutated.Inspect());
                }
            }
        }
    }
}
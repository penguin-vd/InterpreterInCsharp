using objects;
using Eval;

namespace enviroment;
public class Env {
    public Dictionary<string, IObject> Store;
    public Env? Outer;
    public Env() {
        Store = new();
    }

    public IObject? GetObject(string name) {
        if (!Store.Keys.Contains(name)) {
            if (Outer != null) {
                return Outer.GetObject(name);
            } else return null;
        }
        return Store[name];
    }

    public IObject Set(string name, IObject val) {
        Store[name] = val;
        return val;
    }

    public IObject AddEnv(Env env) {
        foreach (var kvp in env.Store) {
            if (kvp.Value.Type() == ObjectType.ERROR)
                return kvp.Value;
            if (kvp.Key == "_name")
                continue;
            Store[kvp.Key] = kvp.Value;
        }
        return Evaluator.NULL;
    }
    public static Env NewEnclosedEnviroment(Env env) {
        return new Env() {Outer = env};
    }
}
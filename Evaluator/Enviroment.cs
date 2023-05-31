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
        if (Outer?.GetObject(name) != null) {
            Outer.Set(name, val);
            return val;
        }
        Store[name] = val;
        return val;
    }
    public IObject Set(string key, IObject index, IObject assignVal) {
        switch (GetObject(key)) {
            case Hash hash:
                if (index is not IHashable)
                    return Evaluator.NULL;
                hash.Pairs[(index as IHashable).HashKey()] = new HashPair() { Key = index, Value = assignVal};
                break;
            case ArrayObj array:
                switch (index) {
                    case Integer integer:
                        if (0 < integer.Value && integer.Value < array.Elements.Count) {
                            array.Elements[(int)integer.Value] = assignVal;
                            break;
                        } else return Evaluator.NULL;
                    default:
                        return Evaluator.NULL;
                }
                break;
        }
        return assignVal;
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

    public void Remove(string key) {
        if (!Store.ContainsKey(key)) {
            if (Outer != null) {
                Outer.Remove(key);
                return;
            } else return;
        }
        Store.Remove(key);
    }
    public static Env NewEnclosedEnviroment(Env env) {
        return new Env() {Outer = env};
    }
}
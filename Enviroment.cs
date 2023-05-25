using objects;

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
    public static Env NewEnclosedEnviroment(Env env) {
        return new Env() {Outer = env};
    }
}
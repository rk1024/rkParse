namespace rkParse.Core.Symbols {
  public class Symbol {
    string name = null;

    public string Name => name;

    protected Symbol() { }

    public Symbol(string name) : this() {
      this.name = name;
    }

    public override string ToString() {
      return $"{name}()";
    }
  }
}

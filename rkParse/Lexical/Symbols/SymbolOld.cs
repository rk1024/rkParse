namespace rkParse.Lexical.Symbols {
  public class SymbolOld {
    string name;

    public string Name => name;

    public SymbolOld(string name) {
      this.name = name;
    }

    public SymbolOld() : this(null) { }

    public override string ToString() {
      return $"{Name}()";
    }
  }
}

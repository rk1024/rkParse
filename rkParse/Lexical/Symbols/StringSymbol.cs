using rkParse.Util;

namespace rkParse.Lexical.Symbols {
  public class StringSymbol : SymbolOld {
    string text;

    public string Text => text;

    public StringSymbol(string name, string text) : base(name) {
      this.text = text.ToLiteral();
    }

    public StringSymbol(string text) : this(null, text) { }

    public override string ToString() {
      return Name == null ? $"<{Text}>" : $"{Name}({Text})";
    }
  }
}

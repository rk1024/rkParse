using rkParse.Core.Symbols;
using rkParse.Util;

namespace rkParse.Lexical.Symbols {
  public class StringSymbol : Symbol {
    string text;

    protected override string StringContents => text.ToLiteral();

    public string Text => text;

    public StringSymbol(string name, string text) : base(name) {
      this.text = text;
    }

    public StringSymbol(string text) : this(null, text) { }
  }
}

using System;

namespace rkParse.Core.Symbols {
  public class Symbol {
    string name = null;

    public string Name => name;

    public Symbol(string name = null) {
      this.name = name;
    }

    protected string ToString(string content, string open, string close) {
      return name == null ? $"{open}{content}{close}" : $"{name}({content})";
    }

    protected string ToString(string content) => ToString(content, "<", ">");

    public override string ToString() {
      return ToString("");
    }
  }
}

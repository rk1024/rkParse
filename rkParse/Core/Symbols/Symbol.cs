using System;

namespace rkParse.Core.Symbols {
  public class Symbol {
    string name = null;

    public string Name => name;

    public Symbol(string name = null) {
      this.name = name;
    }

    protected string ToString(string content) {
      return name == null ? $"<{content}>" : $"{name}({content})";
    }

    public override string ToString() {
      return ToString("");
    }
  }
}

using System.Collections.Generic;
using System.Linq;

namespace rkParse.Lexical.Symbols {
  public class Production : SymbolOld {
    SymbolOld[] symbols;

    public SymbolOld[] Symbols => symbols.ToArray();

    public Production(string name, IEnumerable<SymbolOld> symbols) : base(name) {
      this.symbols = symbols.ToArray();
    }

    public Production(IEnumerable<SymbolOld> symbols) : this(null, symbols) { }


    public override string ToString() {
      return Name == null ?
        $"{{ {string.Join<SymbolOld>(", ", symbols)} }}" :
        $"{Name}({string.Join<SymbolOld>(", ", symbols)})";
    }
  }
}

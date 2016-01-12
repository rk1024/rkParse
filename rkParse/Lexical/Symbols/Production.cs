using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rkParse.Lexical.Symbols {
  public class Production : ISymbol {
    ISymbol[] symbols;

    public Production(IEnumerable<ISymbol> symbols) {
      this.symbols = symbols.ToArray();
    }

    public override string ToString() {
      return $"Production({string.Join<ISymbol>(", ", symbols)})";
    }
  }
}

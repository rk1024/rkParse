using rkParse.IO;
using rkParse.Lexical.Symbols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rkParse.Lexical.SymbolFactories {
  public interface ISymbolSequencer {
    bool Query(BufferedStreamReader reader, ref int start);

    bool Sequence(BufferedStreamReader reader, List<ISymbol> symbols);
  }
}

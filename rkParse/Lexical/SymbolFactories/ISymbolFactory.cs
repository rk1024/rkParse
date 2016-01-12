using rkParse.IO;
using rkParse.Lexical.Symbols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rkParse.Lexical.SymbolFactories {
  public interface ISymbolFactory {
    bool Query(BufferedStreamReader reader, ref int start);

    bool Consume(BufferedStreamReader reader, List<ISymbol> symbols);
  }
}

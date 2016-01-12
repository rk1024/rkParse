using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using rkParse.IO;
using rkParse.Lexical.Symbols;

namespace rkParse.Lexical.SymbolFactories {
  public class SymbolSeqSingle : ISymbolSequencer {
    ISymbolFactory factory;

    public SymbolSeqSingle(ISymbolFactory factory) {
      this.factory = factory;
    }

    public bool Query(BufferedStreamReader reader, ref int start) => factory.Query(reader, ref start);

    public bool Sequence(BufferedStreamReader reader, List<ISymbol> symbols) => factory.Consume(reader, symbols);
  }
}

using rkParse.IO;
using rkParse.Lexical.SymbolFactories;
using rkParse.Lexical.Symbols;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rkParse.Lexical {
  public class Lexer : ILexer {
    BufferedStreamReader reader;
    ISymbolFactory rootSymbolFactory;

    public Lexer(ISymbolFactory rootSymbolFactory, Stream stream) {
      this.rootSymbolFactory = rootSymbolFactory;

      reader = new BufferedStreamReader(stream);
    }

    public ISymbol[] Parse() {
      List<ISymbol> symbols = new List<ISymbol>();

      rootSymbolFactory.Consume(reader, symbols);

      return symbols.ToArray();
    }
  }
}

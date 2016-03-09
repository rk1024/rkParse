using rkParse.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using rkParse.Core.Symbols;
using rkParse.IO;

namespace rkParse.Lexical {
  public class Lexer : Producer<LexerContext> {
    BufferedReader reader;

    public Lexer(Lexicon<LexerContext> lexicon) : base(lexicon) {
    }

    public override LexerContext MakeContext() {
      return new LexerContext(this, reader);
    }

    public Symbol[] Read(string input) {
      BeginRead();

      return EndRead();
    }
  }
}

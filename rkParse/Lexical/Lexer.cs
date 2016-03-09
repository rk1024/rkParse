using rkParse.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using rkParse.Core.Symbols;
using rkParse.IO;

namespace rkParse.Lexical {
  public class Lexer : Producer<string, LexerContext> {
    BufferedReader reader;

    public Lexer(Lexicon<LexerContext> lexicon) : base(lexicon) {
    }

    protected override LexerContext MakeContext() {
      return new LexerContext(this, reader);
    }

    public override Symbol[] Read(string input) {
      BeginRead();



      EndRead();
      return new Symbol[] { };
    }
  }
}

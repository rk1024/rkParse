using rkParse.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using rkParse.Core.Symbols;
using rkParse.IO;
using System.IO;

namespace rkParse.Lexical {
  public class Lexer : Producer<string, LexerContext> {
    BufferedStreamReader reader;

    public Lexer(Stream stream) { reader = new BufferedStreamReader(stream); }

    protected override LexerContext MakeContext() {
      return new LexerContext(this, reader);
    }

    public override Symbol[] Read(string input) {
      BeginRead();

      Context.Execute(Steps.RootStep);

      Symbol[] ret = Context.Output;

      EndRead();
      return ret;
    }
  }
}

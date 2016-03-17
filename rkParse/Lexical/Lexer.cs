using rkParse.Core;
using rkParse.Core.Symbols;
using rkParse.IO;
using rkParse.Lexical.Symbols;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rkParse.Lexical {
  public class Lexer : Producer<Stream, LexerContext> {
    BufferedStreamReader reader;

    public Lexer() { }

    protected override LexerContext MakeContext() {
      return new LexerContext(this, reader);
    }

    public override Symbol[] Read(Stream input) {
      List<Symbol> ret;

      reader = new BufferedStreamReader(input);

      BeginRead();

      Context.Execute(Steps.RootStep);

      ret = Context.Output;


      if (reader.Buffer() > 0) {
        StringBuilder sb = new StringBuilder();
        string seg;

        while (reader.Read(out seg, 512) > 0) {
          sb.Append(seg);
        }

        ret.Add(new StringOverflowSymbol(sb.ToString()));
      }

      EndRead();

      reader = null;

      return ret.ToArray();
    }
  }
}

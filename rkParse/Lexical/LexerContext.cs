using rkParse.Core;
using rkParse.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rkParse.Lexical {
  public class LexerContext : ProducerContext {
    BufferedReader reader;

    public LexerContext(Lexer prod, BufferedReader reader) : base(prod) {
      this.reader = reader;
    }

    protected override int ConsumeInternal(int count) {
      throw new NotImplementedException();
    }
  }
}

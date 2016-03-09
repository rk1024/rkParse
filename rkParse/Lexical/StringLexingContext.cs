using rkParse.Core;
using rkParse.IO;
using System;
using System.Text.RegularExpressions;

namespace rkParse.Lexical {
  public class StringLexingContext : ProducerContext {
    public StringLexingContext(Lexer lex) : base(lex) { }

    public bool QueryString(string pat, int start = 0) {
      throw new NotImplementedException();
    }

    public bool QueryChar(char pat, int start = 0) {
      throw new NotImplementedException();
    }

    public bool QueryCharRegex(Regex pat, int start = 0) {
      throw new NotImplementedException();
    }

    protected override void ConsumeInternal(int count) {
      throw new NotImplementedException();
    }
  }
}

using rkParse.Core;
using rkParse.IO;
using rkParse.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace rkParse.Lexical {
  public class LexerContext : ProducerContext<LexerContext> {
    BufferedStreamReader reader;

    public LexerContext(Lexer prod, BufferedStreamReader reader) : base(prod) {
      this.reader = reader;
    }

    protected override int ConsumeInternal(int count) {
      return reader.Flush(count);
    }

    public bool QueryStringAhead(string match, int start) {
      string peek;
      return reader.PeekAhead(out peek, Position + start, match.Length) == match.Length && peek == match;
    }

    public bool QueryString(string match) => QueryStringAhead(match, 0);

    public bool QueryCharAhead(char match, int start) {
      return QueryStringAhead(match.ToString(), start);
    }

    public bool QueryChar(char match) => QueryCharAhead(match, 0);

    public bool QueryRegexAhead(out string peek, Regex pattern, int start, int count = 1) {
      reader.PeekAhead(out peek, Position + start, count);

      bool match = pattern.IsMatch(peek);
      if (!match) peek = null;

      return match;
    }

    public bool QueryRegex(out string peek, Regex pattern, int count = 1) => QueryRegexAhead(out peek, pattern, 0, count);
  }
}

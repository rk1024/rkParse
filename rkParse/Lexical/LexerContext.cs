using rkParse.Core;
using rkParse.IO;
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

    public bool QueryString(string match) {
      string peek;
      return reader.Peek(out peek, match.Length) == match.Length && peek == match;
    }

    public bool QueryStringAhead(string match, int start) {
      string peek;
      return reader.PeekAhead(out peek, start, match.Length) == match.Length && peek == match;
    }

    public bool QueryChar(char match) {
      return QueryString(match.ToString());
    }

    public bool QueryRegex(Regex pattern, int count = 1) {
      string peek;
      reader.Peek(out peek, count);
      return pattern.IsMatch(peek);
    }

    public bool QueryRegexAhead(Regex pattern, int start, int count = 1) {
      string peek;
      reader.PeekAhead(out peek, start, count);
      return pattern.IsMatch(peek);
    }
  }
}

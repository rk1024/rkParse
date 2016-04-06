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

      bool ret = reader.PeekAhead(out peek, Position + start, match.Length) == match.Length && peek == match;

      Print($"&2;Querying for string &3;{match}&2; at offset &5;{start}&2; (position &5;{Position + start}&2;); {(ret ? "&2;match" : "&c;no match")}&2; found.");

      return ret;
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

      Print($"&2;Querying for &5;{count}&2;-count regex &3;{pattern.ToString()}&2; at offset &5;{start}&2; (position &5;{Position + start}&2;); {(match ? "&2;match" : "&c;no match")}&2; found.");

      return match;
    }

    public bool QueryRegex(out string peek, Regex pattern, int count = 1) => QueryRegexAhead(out peek, pattern, 0, count);
  }
}

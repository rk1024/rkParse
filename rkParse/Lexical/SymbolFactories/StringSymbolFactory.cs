using rkParse.IO;
using rkParse.Lexical.Symbols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rkParse.Lexical.SymbolFactories {
  public class StringSymbolFactory : ISymbolFactory {
    string pattern;

    public StringSymbolFactory(string pattern) {
      this.pattern = pattern;
    }

    public bool Query(BufferedStreamReader reader, ref int start) {
      string buf;

      if (reader.PeekAhead(out buf, start, pattern.Length) < pattern.Length) return false;

      if (buf == pattern) {
        start += pattern.Length;
        return true;
      }
      return false;
    }

    public bool Consume(BufferedStreamReader reader, List<ISymbol> symbols) {
      string buf;

      if (reader.Peek(out buf, pattern.Length) < pattern.Length) return false;
      //else buffer length >= pattern length

      reader.Flush(pattern.Length);

      symbols.Add(CreateSymbol(buf));

      return true;
    }

    protected virtual ISymbol CreateSymbol(string buf) {
      return new StringSymbol(buf);
    }
  }
}

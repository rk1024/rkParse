using rkParse.IO;
using rkParse.Lexical.Symbols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rkParse.Lexical.SymbolFactories {
  public class ProductionOneOfFactory : ISymbolFactory {
    List<ISymbolSequencer> choices;

    public ProductionOneOfFactory(IEnumerable<ISymbolSequencer> choices) {
      this.choices = new List<ISymbolSequencer>(choices);
    }

    ISymbolSequencer GetLongest(BufferedStreamReader reader, int start) {
      int[] lengths = new int[choices.Count];

      int end, length, longestLen = -1;

      ISymbolSequencer longest = null;

      for (int i = 0; i < choices.Count; i++) {
        end = start;
        choices[i].Query(reader, ref end);

        length = end - start;

        if (length > longestLen) longest = choices[i];
      }

      return longest;
    }

    public bool Query(BufferedStreamReader reader, ref int start) => GetLongest(reader, start).Query(reader, ref start);

    public bool Consume(BufferedStreamReader reader, List<ISymbol> symbols) {
      List<ISymbol> childSymbols = new List<ISymbol>();
      bool match = GetLongest(reader, 0).Sequence(reader, symbols);

      if (match) symbols.Add(CreateSymbol(childSymbols));

      return match;
    }

    protected virtual ISymbol CreateSymbol(IEnumerable<ISymbol> childSymbols) {
      return new Production(childSymbols);
    }
  }
}

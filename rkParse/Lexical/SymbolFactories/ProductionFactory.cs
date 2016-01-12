using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using rkParse.IO;
using rkParse.Lexical.Symbols;

namespace rkParse.Lexical.SymbolFactories {

  public class ProductionFactory : ISymbolFactory {
    SymbolSequencer sequencer;

    public ProductionFactory(IEnumerable<SymbolSeqItem> items) {
      sequencer = new SymbolSequencer(items);
    }

    public void AddItem(ISymbolFactory factory, bool isOptional = false) => sequencer.AddItem(factory, isOptional);
    public void AddItem(SymbolSeqItem item) => sequencer.AddItem(item);

    public bool Query(BufferedStreamReader reader, ref int start) => sequencer.Query(reader, ref start);

    public bool Consume(BufferedStreamReader reader, List<ISymbol> symbols) {
      List<ISymbol> childSymbols = new List<ISymbol>();
      bool match = sequencer.Sequence(reader, childSymbols);

      if (match) symbols.Add(CreateSymbol(childSymbols));

      return match;
    }

    protected virtual ISymbol CreateSymbol(IEnumerable<ISymbol> childSymbols) {
      return new Production(childSymbols);
    }
  }
}

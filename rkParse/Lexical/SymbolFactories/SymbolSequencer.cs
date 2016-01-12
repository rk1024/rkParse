using rkParse.IO;
using rkParse.Lexical.Symbols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rkParse.Lexical.SymbolFactories {
  public struct SymbolSeqItem {
    bool isOptional;
    ISymbolFactory factory;

    public bool IsOptional => isOptional;
    public ISymbolFactory Factory => factory;

    public SymbolSeqItem(ISymbolFactory factory, bool isOptional = false) : this() {
      this.factory = factory;
      this.isOptional = isOptional;
    }
  }

  public class SymbolSequencer : ISymbolSequencer {
    List<SymbolSeqItem> items;

    public SymbolSequencer() {
      items = new List<SymbolSeqItem>();
    }

    public SymbolSequencer(IEnumerable<SymbolSeqItem> items) {
      this.items = new List<SymbolSeqItem>(items);
    }

    public void AddItem(SymbolSeqItem item) => items.Add(item);

    public void AddItem(ISymbolFactory factory, bool isOptional = false) {
      AddItem(new SymbolSeqItem(factory, isOptional));
    }

    public bool Query(BufferedStreamReader reader, ref int start) {
      foreach (SymbolSeqItem item in items)
        if (!(item.Factory.Query(reader, ref start) || item.IsOptional)) return false;

      return true;
    }

    public bool Sequence(BufferedStreamReader reader, List<ISymbol> symbols) {
      List<bool> matches = new List<bool>();

      int start = 0;
      bool isMatch;
      foreach (SymbolSeqItem item in items) {
        isMatch = item.Factory.Query(reader, ref start);

        if (!(isMatch || item.IsOptional)) return false;

        matches.Add(isMatch);
      }

      for (int i = 0; i < matches.Count; i++)
        if (matches[i]) items[i].Factory.Consume(reader, symbols);

      return true;
    }
  }
}

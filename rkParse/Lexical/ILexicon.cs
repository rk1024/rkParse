using rkParse.Lexical.SymbolFactories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rkParse.Lexical {
  public interface ILexicon {
    void RegisterSymbolFactory(ISymbolFactory symbol);
  }
}

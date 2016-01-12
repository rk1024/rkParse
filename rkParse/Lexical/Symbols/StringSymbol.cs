using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rkParse.Lexical.Symbols {
  public class StringSymbol : ISymbol {
    string text;

    public StringSymbol(string text) {
      this.text = text;
    }

    public override string ToString() { return $"Symbol('{text}')"; }
  }
}

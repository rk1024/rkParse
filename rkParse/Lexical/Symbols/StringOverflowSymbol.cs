using rkParse.Core.Symbols;
using rkParse.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rkParse.Lexical.Symbols {
  public class StringOverflowSymbol : OverflowSymbol<string> {
    protected override string StringContents => Input.ToLiteral();

    public StringOverflowSymbol(string name, string input) : base(name, input) { }

    public StringOverflowSymbol(string input) : base(input) { }
  }
}

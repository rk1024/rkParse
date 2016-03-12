using rkParse.Core.Symbols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rkParse.Lexical.Steps {
  public class LexerStringStep : LexerStep {
    string pattern;

    public override bool CanBeTerminal => true;

    public LexerStringStep(string name, string pattern) : base(name) {
      this.pattern = pattern;
    }

    public LexerStringStep(string pattern) : this(null, pattern) { }

    public override bool Execute(LexerContext ctx) {
      bool match = ctx.QueryString(pattern);

      if (match) ctx.AddSymbol(new Symbol(Name));

      return match;
    }
  }
}

using rkParse.Core.Steps;
using rkParse.Core.Symbols;
using rkParse.Lexical.Symbols;
using rkParse.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rkParse.Lexical.Steps {
  public class LexerStringStep : TerminalStep<LexerContext> {
    string pattern;

    public LexerStringStep(string name, string pattern) : base(name) {
      this.pattern = pattern;
    }

    public LexerStringStep(string pattern) : this(null, pattern) { }

    protected override bool ExecuteTerminal(LexerContext ctx) {
      bool match = ctx.QueryString(pattern);

      if (match) {
        ctx.AddSymbol(Name == null ? new StringSymbol(null, pattern) : new Symbol(Name));
        ctx.Consume(pattern.Length);
      }

      return match;
    }

    public override string ToString() {
      return $"{base.ToString()}({pattern.ToLiteral()})";
    }
  }
}

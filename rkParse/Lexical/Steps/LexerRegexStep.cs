using rkParse.Lexical.Symbols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace rkParse.Lexical.Steps {
  public class LexerRegexStep : LexerStep {
    Regex expr;
    int count;

    public override bool CanBeTerminal => true;

    public LexerRegexStep(string name, Regex expr, int count = 1) : base(name) {
      this.expr = expr;
      this.count = count;
    }

    public LexerRegexStep(Regex expr, int count = 1) : this(null, expr, count) { }

    public override bool Execute(LexerContext ctx) {
      string text;
      bool match = ctx.QueryRegex(out text, expr, count);

      if (match) {
        ctx.AddSymbol(new StringSymbol(Name, text));
        ctx.Consume(text.Length);
      }

      return match;
    }
  }
}

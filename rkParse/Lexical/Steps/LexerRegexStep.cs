using rkParse.Core.Steps;
using rkParse.Core.Symbols;
using rkParse.Lexical.Symbols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace rkParse.Lexical.Steps {
  public class LexerRegexStep : TerminalStep<LexerContext> {
    Regex expr;
    int count;

    public LexerRegexStep(string name, Regex expr, int count = 1) : base(name) {
      this.expr = expr;
      this.count = count;
    }

    public LexerRegexStep(Regex expr, int count = 1) : this(null, expr, count) { }

    protected override bool ExecuteTerminal(LexerContext ctx) {
      string text;
      bool match = ctx.QueryRegex(out text, expr, count);

      if (match) {
        Symbol sym = new StringSymbol(Name, text);
        Console.WriteLine($"[ProducerStep]{_DebugIndent()} Matched {sym}");

        ctx.AddSymbol(sym);
        ctx.Consume(text.Length);
      }

      return match;
    }
  }
}

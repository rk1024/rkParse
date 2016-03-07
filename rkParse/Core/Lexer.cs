using rkParse.Core.Symbols;
using System.Collections.Generic;

namespace rkParse.Core {
  public abstract class Lexer {
    protected Lexicon lexicon;
    protected bool isReading = false;

    public Lexicon Lexicon => lexicon;
    public bool IsReading => isReading;

    public Lexer(Lexicon lexicon) {
      this.lexicon = lexicon;
    }

    public abstract Symbol[] Read();
  }

  public abstract class Lexer<TContext> : Lexer where TContext : LexingContext {
    public Lexer(Lexicon lexicon) : base(lexicon) { }

    public abstract TContext MakeContext();

    public override Symbol[] Read() {
      isReading = true;

      TContext ctx = MakeContext();

      lexicon[lexicon.RootStep].Execute(ctx);

      isReading = false;

      return ctx.Output;
    }
  }
}

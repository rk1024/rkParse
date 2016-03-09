using rkParse.Core.Symbols;
using System.Collections.Generic;

namespace rkParse.Core {
  public abstract class Producer {
    protected Lexicon lexicon;
    protected bool isReading = false;

    public Lexicon Lexicon => lexicon;
    public bool IsReading => isReading;

    public Producer(Lexicon lexicon) {
      this.lexicon = lexicon;
    }

    public abstract Symbol[] Read();
  }

  public abstract class Producer<TContext> : Producer where TContext : LexingContext {
    public Producer(Lexicon lexicon) : base(lexicon) { }

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

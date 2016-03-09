using rkParse.Core.Symbols;
using System.Collections.Generic;

namespace rkParse.Core {
  public abstract class Producer<TContext> where TContext : ProducerContext {
    Lexicon<TContext> lexicon;
    TContext context = null;
    bool isReading = false;

    public Lexicon<TContext> Lexicon => lexicon;
    protected TContext Context => context;
    public bool IsReading => isReading;

    public Producer(Lexicon<TContext> lexicon) {
      this.lexicon = lexicon;
    }

    public Producer() : this(new Lexicon<TContext>()) { }

    public abstract TContext MakeContext();

    protected void BeginRead() {
      isReading = true;

      context = MakeContext();

      lexicon[lexicon.RootStep].Execute(context);
    }

    protected Symbol[] EndRead() {
      isReading = false;

      TContext ctx = context;

      context = null;

      return ctx.Output;
    }
  }
}

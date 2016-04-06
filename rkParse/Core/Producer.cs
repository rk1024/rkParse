using rkParse.Core.Symbols;
using System.Collections.Generic;

namespace rkParse.Core {
  public abstract class Producer<TContext> where TContext : ProducerContext<TContext> {
    Lexicon<TContext> steps = new Lexicon<TContext>();
    TContext context = null;
    bool isReading = false;

    public Lexicon<TContext> Steps => steps;
    protected TContext Context => context;

    public bool IsReading => isReading;

    public Producer() { }

    protected abstract TContext MakeContext();

    protected virtual void BeginRead() {
      isReading = true;

      context = MakeContext();
    }

    protected virtual void EndRead() {
      context = null;

      isReading = false;
    }
  }
}

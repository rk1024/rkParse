using rkParse.Core.Symbols;
using System.Collections.Generic;

namespace rkParse.Core {
  public abstract class Producer {
    bool isReading = false;

    public bool IsReading => isReading;

    protected virtual void BeginRead() {
      isReading = true;
    }

    protected virtual void EndRead() {
      isReading = false;
    }
  }

  public abstract class Producer<TInput, TContext> : Producer where TContext : ProducerContext<TContext> {
    Lexicon<TContext> steps = new Lexicon<TContext>();
    TContext context = null;

    public Lexicon<TContext> Steps => steps;
    protected TContext Context => context;

    public Producer() { }

    protected abstract TContext MakeContext();

    protected override void BeginRead() {
      base.BeginRead();

      context = MakeContext();
    }

    protected override void EndRead() {
      context = null;

      base.EndRead();
    }

    public abstract Symbol[] Read(TInput input);
  }
}

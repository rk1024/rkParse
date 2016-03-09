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

  public abstract class Producer<TInput, TContext> : Producer where TContext : ProducerContext {
    Lexicon<TContext> lexicon;
    TContext context = null;

    public Lexicon<TContext> Lexicon => lexicon;
    protected TContext Context => context;

    public Producer(Lexicon<TContext> lexicon) {
      this.lexicon = lexicon;
    }

    public Producer() : this(new Lexicon<TContext>()) { }

    protected abstract TContext MakeContext();

    public abstract Symbol[] Read(TInput input);
  }
}

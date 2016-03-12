using rkParse.Core.Staging;
using rkParse.Core.Steps;
using rkParse.Core.Symbols;
using System;
using System.Collections.Generic;

namespace rkParse.Core {
  public abstract class ProducerContext : ICacheParent<StagingCacheBase>, ICacheParent<RecursionCache> {

    List<Symbol> output = new List<Symbol>();
    Stack<StagingCacheBase> caches = new Stack<StagingCacheBase>();
    Stack<RecursionCache> recurCaches = new Stack<RecursionCache>();
    Producer prod;

    public Symbol[] Output => output.ToArray();

    public ProducerContext(Producer prod) {
      if (!prod.IsReading) throw new InvalidOperationException($"Can't make a LexingContext for a Producer that is not reading.");

      this.prod = prod;
    }

    public void AddSymbols(IEnumerable<Symbol> symbols) {
      if (caches.Count == 0) output.AddRange(symbols);
      else caches.Peek().Symbols.AddRange(symbols);
    }

    public void AddSymbol(Symbol symbol) {
      if (caches.Count == 0) output.Add(symbol);
      else caches.Peek().Symbols.Add(symbol);
    }

    protected abstract int ConsumeInternal(int count);

    public void Consume(int count) {
      if (caches.Count == 0) ConsumeInternal(count);
      else caches.Peek().Consume(count);
    }

    public StagingCache BeginStaging() {
      StagingCache cache;
      if (caches.Count == 0) cache = new StagingCache(this, 0);
      else cache = new StagingCache(this, caches.Peek().End);

      caches.Push(cache);

      return cache;
    }

    public BranchedStagingCache BeginStagingBranched() {
      BranchedStagingCache cache;
      if (caches.Count == 0) cache = new BranchedStagingCache(this, 0);
      else cache = new BranchedStagingCache(this, caches.Peek().End);

      caches.Push(cache);

      return cache;
    }

    public void EndStaging(StagingCacheBase cache, bool applyChanges) {
      if (IsCacheLocked(cache)) throw new InvalidOperationException("EndStaging called on invalid cache.");

      caches.Pop();

      if (applyChanges) {
        AddSymbols(cache.Symbols);
        Consume(cache.Consumed);
      }
    }

    public bool IsCacheLocked(StagingCacheBase cache) {
      return caches.Peek() != cache;
    }

    public bool IsCacheLocked(RecursionCache cache) {
      return recurCaches.Peek() != cache;
    }

    public RecursionCache BeginRecursion(int limit) {
      RecursionCache cache = new RecursionCache(this, limit);

      recurCaches.Push(cache);

      return cache;
    }

    public void EndRecursion(RecursionCache cache) {
      if (cache != recurCaches.Peek()) throw new InvalidOperationException("EndRecursion called on invalid cache.");

      recurCaches.Pop();
    }

    public bool PushRecursion() {
      if (recurCaches.Count == 0) throw new InvalidOperationException("Cannot call PushRecursion before BeginRecursion.");

      return recurCaches.Peek().PushRecursion();
    }

    public void PopRecursion() {
      if (recurCaches.Count == 0) throw new InvalidOperationException("Cannot call PopRecursion before BeginRecursion.");

      recurCaches.Peek().PopRecursion();
    }
  }

  public abstract class ProducerContext<TThis> : ProducerContext where TThis : ProducerContext {
    public bool Execute(ProducerStep<TThis> step) {
      return step.Execute(this as TThis);
    }

    public ProducerContext(Producer prod) : base(prod) { }
  }
}

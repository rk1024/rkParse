using rkParse.Core.Staging;
using rkParse.Core.Symbols;
using System;
using System.Collections.Generic;

namespace rkParse.Core {
  public abstract class LexingContext : IStagingCacheParent {

    List<Symbol> output = new List<Symbol>();
    Stack<StagingCacheBase> caches = new Stack<StagingCacheBase>();
    Stack<RecursionCache> recurCaches = new Stack<RecursionCache>();
    Lexer lex;

    public Symbol[] Output => output.ToArray();

    public LexingContext(Lexer lex) {
      if (!lex.IsReading) throw new InvalidOperationException("Can't make a LexingContext for a Lexer that is not reading.");

      this.lex = lex;
    }

    public void AddSymbols(IEnumerable<Symbol> symbols) {
      if (caches.Count == 0) output.AddRange(symbols);
      else caches.Peek().Symbols.AddRange(symbols);
    }

    public void AddSymbol(Symbol symbol) {
      if (caches.Count == 0) output.Add(symbol);
      else caches.Peek().Symbols.Add(symbol);
    }

    protected abstract void ConsumeInternal(int count);

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

    public RecursionCache BeginRecursion(int limit) {
      RecursionCache cache = new RecursionCache(limit);

      recurCaches.Push(cache);

      return cache;
    }

    public void EndRecursion(RecursionCache cache) {
      if (cache != recurCaches.Peek()) throw new InvalidOperationException("EndRecursion called on invalid cache.");

      recurCaches.Pop();
    }
  }
}

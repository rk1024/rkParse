using rkParse.Core.Staging;
using rkParse.Core.Steps;
using rkParse.Core.Symbols;
using System;
using System.Collections.Generic;
using System.Linq;

namespace rkParse.Core {
  public abstract class ProducerContext<TThis> : ICacheParent<StagingCacheBase> where TThis : ProducerContext<TThis> {
    List<Symbol> output = new List<Symbol>();
    Stack<StagingCacheBase> caches = new Stack<StagingCacheBase>();
    Producer prod;
    int recurDepth = -1, recurLimit = -1;

    public List<Symbol> Output => output.ToList();

    public bool SafeRecursing => recurLimit >= 0;
    public bool CanRecurse {
      get {
        if (!SafeRecursing) throw new InvalidOperationException("Cannot access CanRecurse while not safe-recursing.");

        return recurDepth < recurLimit;
      }
    }

    public int RecursionLimit {
      get { return recurLimit; }
      set {
        if (value < 0) throw new ArgumentOutOfRangeException("value", value, "Recursion limit must be greater than or equal to zero.");

        recurLimit = value;
      }
    }

    protected int Position => caches.Count == 0 ? 0 : caches.Peek().End;

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

    public void EndStaging(StagingCacheBase cache, bool consume, bool addSymbols) {
      if (IsCacheLocked(cache)) throw new InvalidOperationException("EndStaging called on invalid cache.");

      caches.Pop();

      if (consume) Consume(cache.Consumed);
      if (addSymbols) AddSymbols(cache.Symbols);
    }

    public void EndStaging(StagingCacheBase cache, bool applyChanges) => EndStaging(cache, applyChanges, applyChanges);

    public bool IsCacheLocked(StagingCacheBase cache) {
      return caches.Peek() != cache;
    }

    public bool BeginSafeRecursion(int limit = 0) {
      if (SafeRecursing) return false;

      RecursionLimit = limit;
      recurDepth = 0;

      return true;
    }

    public bool EndSafeRecursion() {
      if (!SafeRecursing) throw new InvalidOperationException("Context is not safe-recursing.");

      if (recurDepth > 0) throw new InvalidOperationException("Cannot stop safe-recursion before all recursions have completed.");

      recurLimit = recurDepth = -1;

      return true;
    }

    //NB: Return value is whether you can call PushRecursion again, not whether a recursion was actually pushed.
    //    IT WILL THROW AN EXCEPTION IF IT CANNOT PUSH
    public bool PushRecursion() {
      if (!SafeRecursing) throw new InvalidOperationException("Cannot push recursion when not safe-recursing.");

      if (recurDepth == recurLimit) throw new InvalidOperationException("Attempted to exceed recursion limit.");

      if (++recurDepth == recurLimit) return false;

      return true;
    }

    //NB: Return value is whether you can call PopRecursion again, not whether a recursion was actually popped.
    //    IT WILL THROW AN EXCEPTION IF IT CANNOT POP
    public bool PopRecursion() {
      if (!SafeRecursing) throw new InvalidOperationException("Cannot pop recursion when not safe-recursing.");

      if (recurDepth == 0) throw new InvalidOperationException("Attempted to pop nonexistent recursion.");

      if (--recurDepth == 0) return false;

      return true;
    }

    public StepResult Execute(ProducerStep<TThis> step) {
      return step.Execute(this as TThis);
    }
  }
}

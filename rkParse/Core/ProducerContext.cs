using rkParse.Core.Staging;
using rkParse.Core.Steps;
using rkParse.Core.Symbols;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace rkParse.Core {
  public abstract class ProducerContext<TThis> : ICacheParent<StagingCacheBase> where TThis : ProducerContext<TThis> {
    List<Symbol> output = new List<Symbol>();
    Stack<StagingCacheBase> caches = new Stack<StagingCacheBase>();
    Producer<TThis> prod;
    StringBuilder indent = new StringBuilder();
    int recurDepth = -1, recurLimit = -1;
    string logPrefix = "&7;&i;";

    public List<Symbol> Output => output.ToList();
    public Lexicon<TThis> Steps => prod.Steps;

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

        Print($"&9;Recursion limit {(recurLimit < 0 ? "is" : "changed to")} &d;{value}");

        recurLimit = value;
      }
    }

    public string LogPrefix {
      get { return logPrefix; }
      set { logPrefix = value; }
    }

    public int Position => caches.Count == 0 ? 0 : caches.Peek().End;

    public ProducerContext(Producer<TThis> prod) {
      if (!prod.IsReading) throw new InvalidOperationException("Can't make a LexingContext for a Producer that is not reading.");

      this.prod = prod;
    }

    public void AddSymbols(IEnumerable<Symbol> symbols) {
      if (caches.Count == 0) output.AddRange(symbols);
      else caches.Peek().Symbols.AddRange(symbols);

      Print($"&b;Added collection of &d;{symbols.Count()}&b; symbol(s) to {(caches.Count == 0 ? "output" : "cache")}.");
    }

    public void AddSymbol(Symbol symbol) {
      if (caches.Count == 0) output.Add(symbol);
      else caches.Peek().Symbols.Add(symbol);

      Print($"&b;Added &d;1&b; symbol to {(caches.Count == 0 ? "output" : "cache")}.");
    }

    protected abstract int ConsumeInternal(int count);

    public void Consume(int count) {
      if (caches.Count == 0) {
        ConsumeInternal(count);
        Print($"&b;Consumed &d;{count}&b; input elements.");
      }
      else {
        caches.Peek().Consume(count);
        Print($"&b;Cached consumption of &d;{count}&b; input elements; position is now &d;{Position}&b; cache(s).");
      }
    }

    public StagingCache BeginStaging() {
      StagingCache cache;
      if (caches.Count == 0) cache = new StagingCache(this, 0);
      else cache = new StagingCache(this, caches.Peek().End);

      caches.Push(cache);

      Print($"&b;Pushed staging cache onto the stack; stack now contains &d;{caches.Count}&b;.");

      return cache;
    }

    public BranchedStagingCache BeginStagingBranched() {
      BranchedStagingCache cache;
      if (caches.Count == 0) cache = new BranchedStagingCache(this, 0);
      else cache = new BranchedStagingCache(this, caches.Peek().End);

      caches.Push(cache);

      Print($"&b;Pushed &a;branched&b; staging cache onto the stack; stack now contains &d;{caches.Count}&b;.");

      return cache;
    }

    public void EndStaging(StagingCacheBase cache, bool consume, bool addSymbols) {
      if (IsCacheLocked(cache)) throw new InvalidOperationException("EndStaging called on invalid cache.");

      caches.Pop();

      Print($"&b;Popped staging cache off the stack; stack now contains &d;{caches.Count}&b;.");

      if (consume) Consume(cache.Consumed);
      else Print($"&3;Did not pass on consumed count; position is now &5;{Position}&3;.");
      if (addSymbols) AddSymbols(cache.Symbols);
      else Print($"&3;Did not pass on symbols.");
    }

    public void EndStaging(StagingCacheBase cache, bool applyChanges) => EndStaging(cache, applyChanges, applyChanges);

    public bool IsCacheLocked(StagingCacheBase cache) {
      return caches.Peek() != cache;
    }

    public bool BeginSafeRecursion(int limit = 0) {
      if (SafeRecursing) return false;

      RecursionLimit = limit;
      recurDepth = 0;

      Print($"&9;Began safe-recursing with limit &d;{limit}&9;.");

      return true;
    }

    public bool EndSafeRecursion() {
      if (!SafeRecursing) throw new InvalidOperationException("Context is not safe-recursing.");

      if (recurDepth > 0) throw new InvalidOperationException("Cannot stop safe-recursion before all recursions have completed.");

      recurLimit = recurDepth = -1;

      Print($"&9;Ended safe-recursing.");

      return true;
    }

    //NB: Return value is whether you can call PushRecursion again, not whether a recursion was actually pushed.
    //    IT WILL THROW AN EXCEPTION IF IT CANNOT PUSH
    public bool PushRecursion() {
      if (!SafeRecursing) throw new InvalidOperationException("Cannot push recursion when not safe-recursing.");

      if (recurDepth == recurLimit) throw new InvalidOperationException("Attempted to exceed recursion limit.");

      if (++recurDepth == recurLimit) {
        Print($"&9;Added a level of recursion; depth is &5;{recurDepth}&9;, &c;can not&9; recurse again.");
        return false;
      }

      Print($"&9;Added a level of recursion; depth is &5;{recurDepth}&9;, &a;can&9; recurse again.");
      return true;
    }

    //NB: Return value is whether you can call PopRecursion again, not whether a recursion was actually popped.
    //    IT WILL THROW AN EXCEPTION IF IT CANNOT POP
    public bool PopRecursion() {
      if (!SafeRecursing) throw new InvalidOperationException("Cannot pop recursion when not safe-recursing.");

      if (recurDepth == 0) throw new InvalidOperationException("Attempted to pop nonexistent recursion.");

      if (--recurDepth == 0) {
        Print($"&9;Removed the last level of recursion; depth is zero.");
        return false;
      }

      Print($"&9;Removed a level of recursion; depth is &5;{recurDepth}&9;.");
      return true;
    }

    public StepResult Execute(ProducerStep<TThis> step) {
      return step.Execute(this as TThis);
    }

    void PushIndent(string str) {
      indent.Append(str.Substring(0, 2));
    }

    public void PushIndent() => PushIndent("│ ");

    bool PopIndent() {
      if (indent.Length < 2) return false;

      indent.Remove(indent.Length - 2, 2);

      return true;
    }

    void SwapIndent(string str) {
      if (PopIndent())
        PushIndent(str);
    }

    static readonly Regex colorRegex = new Regex(@"&([-0-9a-fA-FrR]{1,2}|i);");

    public void Print(string str, bool popIndent = false) {
      ConsoleColor foreClr = Console.ForegroundColor,
        bkgdClr = Console.BackgroundColor;

      SwapIndent(popIndent ? "└─" : "├─");

      string[] parts = colorRegex.Split(logPrefix + str);

      for (int i = 0; i < parts.Length; i++) {
        if (i % 2 == 0) {
          Console.Write(parts[i]);
        }
        else {
          string part = parts[i].ToLower();

          if (part == "i") {
            Console.Write(indent.ToString());
            continue;
          }

          bool hasBkgd = part.Length == 2,
            resetFore = part[0] == 'r',
            resetBkgd = hasBkgd && part[1] == 'r';

          if (resetFore) Console.ForegroundColor = foreClr;
          if (resetBkgd) Console.BackgroundColor = bkgdClr;

          if (!resetFore && part[0] != '-')
            Console.ForegroundColor = (ConsoleColor)(int.Parse(part[0].ToString(), NumberStyles.HexNumber));

          if (hasBkgd && !resetBkgd && part[1] != '-')
            Console.BackgroundColor = (ConsoleColor)(int.Parse(part[1].ToString(), NumberStyles.HexNumber));
        }
      }

      Console.WriteLine();
      Console.ForegroundColor = foreClr;
      Console.BackgroundColor = bkgdClr;

      if (popIndent) PopIndent();
      else SwapIndent("│ ");
    }
  }
}

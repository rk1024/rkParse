using rkParse.Core.Staging;
using rkParse.Util;
using System;
using System.Linq;
using System.Text;

namespace rkParse.Core.Steps {
  public enum StepResult {
    Negative,
    Positive,
    AddRecursion,
  };

  public abstract class ProducerStep<TContext> where TContext : ProducerContext<TContext> {
    bool initialized = false;
    string name;

    public abstract bool IsRecursive { get; }

    public abstract bool CanBeTerminal { get; }

    public string Name => name;

    public ProducerStep(string name = null) {
      this.name = name;
    }

    protected abstract StepResult ExecuteInternal(TContext ctx);

    protected virtual void Initialize(TContext ctx) { }

    public StepResult Execute(TContext ctx) {
      ctx.Print($"&8;Executing &07;{this}&8r; at position &5;{ctx.Position}&8;...");
      ctx.PushIndent();

      StepResult result;

      if (!initialized) { Initialize(ctx); initialized = true; }

      //if (IsRecursive) {
      //  if (ctx.SafeRecursing && !ctx.CanRecurse)
      //    result = StepResult.AddRecursion;

      //  else if (ctx.BeginSafeRecursion()) {
      //    BranchedStagingCache cache = ctx.BeginStagingBranched();
      //    StagingCache last = null;

      //    while (true) {
      //      StagingCache branch = cache.BeginSingleBranch();
      //      result = ExecuteInternal(ctx);

      //      if (result == StepResult.Positive) {
      //        if (last != null) cache.EndBranch(last);
      //        last = branch;
      //      }
      //      else {
      //        cache.EndBranch(branch);

      //        if (result == StepResult.Negative) break;
      //      }

      //      ctx.RecursionLimit++;
      //    }

      //    if (last != null) result = StepResult.Positive;

      //    ctx.EndStaging(cache, last != null);
      //  }
      //  else {
      //    ctx.PushRecursion();

      //    result = ExecuteInternal(ctx);

      //    ctx.PopRecursion();
      //  }
      //}
      //else
      result = ExecuteInternal(ctx);

      if (result == StepResult.AddRecursion && !ctx.SafeRecursing) throw new InvalidOperationException("Step returned StepResult.AddRecursion when not safe-recursing.  This should not happen!");

      string resultStr = "&4;ERROR";

      switch (result) {
        case StepResult.Negative: resultStr = "&c;Negative"; break;
        case StepResult.Positive: resultStr = "&a;Positive"; break;
        case StepResult.AddRecursion: resultStr = "&e;Add Recursion"; break;
      }

      ctx.Print($"&8;Returned {resultStr} &7;(&8;{this}&7;)", true);

      return result;
    }

    public override string ToString() {
      Type type = GetType();
      string typeName = $"{type.GetNameWithoutArity()}";
      if (type.GenericTypeArguments.Length > 0) {
        string genName = string.Join(", ", from arg in type.GenericTypeArguments
                                           select arg.Name);

        typeName = $"{typeName}<{genName}>";
      }

      return Name == null ? typeName : $"{typeName} {Name}";
    }
  }
}

using rkParse.Core.Staging;
using rkParse.Util;
using System;
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
      StepResult result;

      if (!initialized) { Initialize(ctx); initialized = true; }

      if (IsRecursive) {
        if (ctx.SafeRecursing && !ctx.CanRecurse)
          result = StepResult.AddRecursion;

        else if (ctx.BeginSafeRecursion()) {
          BranchedStagingCache cache = ctx.BeginStagingBranched();
          StagingCache last = null;

          while (true) {
            StagingCache branch = cache.BeginSingleBranch();
            result = ExecuteInternal(ctx);

            if (result == StepResult.Positive) {
              if (last != null) cache.EndBranch(last);
              last = branch;
            }
            else {
              cache.EndBranch(branch);

              if (result == StepResult.Negative) break;
            }

            ctx.RecursionLimit++;
          }

          if (last != null) result = StepResult.Positive;

          ctx.EndStaging(cache, last != null);
        }
        else {
          ctx.PushRecursion();

          result = ExecuteInternal(ctx);

          ctx.PopRecursion();
        }
      }
      else result = ExecuteInternal(ctx);

      return result;
    }
  }
}

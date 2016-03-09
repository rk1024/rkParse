using rkParse.Core.Staging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rkParse.Core.Steps {
  public abstract class RecursiveProducerStep : ProducerStep {
    public override bool CanBeTerminal => false;

    public RecursiveProducerStep(string name = null) : base(name) {
    }

    /// <summary>
    /// When implemented in another class, performs the execution logic of the production.
    /// Note: This method MUST NOT execute a nonterminal step if canRecurse is false.
    /// </summary>
    /// <param name="ctx">The current lexing context.</param>
    /// <param name="canRecurse">Indicates whether another nonterminal step can be run.</param>
    /// <returns>True if this step was successfully executed; false otherwise.</returns>
    protected abstract bool Execute(ProducerContext ctx, bool canRecurse);

    bool Execute(ProducerContext ctx, int recurDepth) {
      Console.Write($"Executing, recursion depth {recurDepth}...");

      bool canRecurse = recurDepth > 0;
      if (Execute(ctx, canRecurse)) {
        if (canRecurse) {
          Console.WriteLine("recursing.");
          return Execute(ctx, recurDepth - 1);
        }

        Console.WriteLine("returned true.");

        return true;
      }

      Console.WriteLine("returned false.");

      return false;
    }

    public override bool Execute(ProducerContext ctx) {
      int lim = 0;

      BranchedStagingCache cache = ctx.BeginStagingBranched();
      StagingCache branchOld = null;

      while (true) {
        StagingCache branch = cache.BeginSingleBranch();

        bool result = Execute(ctx, lim);

        if (result) {
          cache.EndBranch(branchOld);
          branchOld = branch;
          lim++;
        }
        else {
          cache.EndBranch(branch, branchOld);
          break;
        }
      }

      bool ret = cache == null;

      ctx.EndStaging(cache, ret);
      return ret;
    }
  }
}

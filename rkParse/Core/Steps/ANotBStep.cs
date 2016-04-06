using rkParse.Core.Staging;
using rkParse.Core.Symbols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rkParse.Core.Steps {
  public class ANotBStep<TContext> : NonterminalStep<TContext> where TContext : ProducerContext<TContext> {
    private ProducerStep<TContext> a, b;

    protected override IEnumerable<ProducerStep<TContext>> SubSteps {
      get {
        yield return a;
        yield return b;
      }
    }

    public ANotBStep(string name, ProducerStep<TContext> a, ProducerStep<TContext> b) : base(name) {
      this.a = a;
      this.b = b;
    }

    public ANotBStep(ProducerStep<TContext> a, ProducerStep<TContext> b) : this(null, a, b) { }

    protected override StepResult ExecuteInternal(TContext ctx) {
      BranchedStagingCache cache = ctx.BeginStagingBranched();

      StagingCache posBranch = cache.BeginSingleBranch();

      StepResult posResult = a.Execute(ctx);

      if (posResult == StepResult.Positive) {
        StagingCache negBranch = cache.BeginSingleBranch();

        StepResult negResult = b.Execute(ctx);

        switch (negResult) {
          case StepResult.Negative:
            cache.EndBranch(negBranch, posBranch);

            ctx.EndStaging(cache, true, false);

            ctx.AddSymbol(new Production(Name, cache.Symbols));

            return StepResult.Positive;

          case StepResult.Positive:
            if (negBranch.Consumed < posBranch.Consumed) goto case StepResult.Negative;

            posResult = StepResult.Negative;
            break;

          case StepResult.AddRecursion:
            posResult = StepResult.AddRecursion;
            break;
        }
      }

      ctx.EndStaging(cache, false);

      return posResult;
    }
  }
}

using rkParse.Core.Staging;
using rkParse.Core.Symbols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rkParse.Core.Steps {
  public class OneOrMoreStep<TContext> : NonterminalStep<TContext> where TContext : ProducerContext<TContext> {
    ProducerStep<TContext> child;

    protected override IEnumerable<ProducerStep<TContext>> SubSteps {
      get { yield return child; }
    }

    public OneOrMoreStep(string name, ProducerStep<TContext> child) : base(name) {
      this.child = child;
    }

    public OneOrMoreStep(ProducerStep<TContext> child) : this(null, child) { }

    protected override StepResult ExecuteInternal(TContext ctx) {
      StagingCache cache = ctx.BeginStaging();

      StepResult result = child.Execute(ctx);

      if (result != StepResult.Positive) {
        ctx.EndStaging(cache, false);
        return result;
      }

      while (child.Execute(ctx) == StepResult.Positive) ;

      ctx.EndStaging(cache, true, false);
      ctx.AddSymbol(new Production(Name, cache.Symbols));

      return StepResult.Positive;
    }
  }
}

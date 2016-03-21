using rkParse.Core.Staging;
using rkParse.Core.Symbols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rkParse.Core.Steps {
  public class OneOrMoreStep<TContext> : ProducerStep<TContext> where TContext : ProducerContext<TContext> {
    ProducerStep<TContext> child;

    public override bool IsRecursionSafe => false;

    public OneOrMoreStep(string name, ProducerStep<TContext> child) : base(name) {
      this.child = child;
    }

    public OneOrMoreStep(ProducerStep<TContext> child) : this(null, child) { }

    protected override bool ExecuteInternal(TContext ctx) {
      StagingCache cache = ctx.BeginStaging();

      bool match = false;

      while (child.Execute(ctx)) match = true;

      ctx.EndStaging(cache, true, false);

      if (match) ctx.AddSymbol(new Production(Name, cache.Symbols));

      return match;
    }
  }
}

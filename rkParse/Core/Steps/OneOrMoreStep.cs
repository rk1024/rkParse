using rkParse.Core.Staging;
using rkParse.Lexical.Symbols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rkParse.Core.Steps {
  public class OneOrMoreStep<TContext> : ProducerStep<TContext> where TContext : ProducerContext {
    ProducerStep<TContext> child;

    public override bool CanBeTerminal => false;

    public OneOrMoreStep(ProducerStep<TContext> child) {
      this.child = child;
    }

    public override bool Execute(TContext ctx) {
      StagingCache cache = ctx.BeginStaging();

      bool match = false;

      while (child.Execute(ctx)) match = true;

      ctx.EndStaging(cache, true, false);

      ctx.AddSymbol(new ProductionOld);

      return match;
    }
  }
}

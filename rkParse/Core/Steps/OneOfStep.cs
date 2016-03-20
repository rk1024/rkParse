using rkParse.Core.Staging;
using rkParse.Core.Symbols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rkParse.Core.Steps {
  public class OneOfStep<TContext> : ProducerStep<TContext> where TContext : ProducerContext<TContext> {
    List<ProducerStep<TContext>> choices;

    public override bool CanBeTerminal => false;

    public OneOfStep(string name, IEnumerable<ProducerStep<TContext>> choices) : base(name) {
      this.choices = choices.ToList();
    }

    public OneOfStep(IEnumerable<ProducerStep<TContext>> choices) : this(null, choices) { }

    public OneOfStep(string name = null) : base(name) {
      choices = new List<ProducerStep<TContext>>();
    }

    public OneOfStep<TContext> Add(ProducerStep<TContext> choice) {
      choices.Add(choice);

      return this;
    }

    protected override bool ExecuteInternal(TContext ctx) {
      BranchedStagingCache cache = ctx.BeginStagingBranched();
      StagingCache longest = null;

      foreach (ProducerStep<TContext> choice in choices) {
        StagingCache branch = cache.BeginSingleBranch();

        if (choice.Execute(ctx)) {
          if (longest == null) goto keepBranch;
          else if (longest.Consumed < branch.Consumed) {
            cache.EndBranch(longest);
            goto keepBranch;
          }
        }

        cache.EndBranch(branch);

        goto @continue;

        keepBranch:

        longest = branch;

        @continue:

        if (longest != null) {
          cache.CurrentBranch = longest;
        }
      }

      if (longest == null) {
        ctx.EndStaging(cache, false);
        return false;
      }

      ctx.EndStaging(cache, true, false);
      ctx.AddSymbol(new Production(Name, cache.Symbols));

      return true;
    }
  }
}

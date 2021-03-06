﻿using rkParse.Core.Staging;
using rkParse.Core.Symbols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rkParse.Core.Steps {
  public class OneOfStep<TContext> : NonterminalStep<TContext> where TContext : ProducerContext<TContext> {
    List<ProducerStep<TContext>> choices;

    protected override IEnumerable<ProducerStep<TContext>> SubSteps => choices;

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

    protected IEnumerable<ProducerStep<TContext>> GetValidChoices(TContext ctx) {
      if (ctx.SafeRecursing && IsRecursive) {
        if (ctx.CanRecurse) {
          foreach (ProducerStep<TContext> choice in choices) {
            if (choice.IsRecursive) yield return choice;
          }
        }
        else {
          foreach (ProducerStep<TContext> choice in choices) {
            if (!choice.IsRecursive) yield return choice;
          }
        }
      }
      else {
        foreach (ProducerStep<TContext> choice in choices)
          yield return choice;
      }
    }

    protected override StepResult ExecuteInternal(TContext ctx) {
      BranchedStagingCache cache = ctx.BeginStagingBranched();
      StagingCache longest = null;
      bool addRecursion = false;

      foreach (ProducerStep<TContext> choice in GetValidChoices(ctx)) {
        StagingCache branch = cache.BeginSingleBranch();
        StepResult result = choice.Execute(ctx);

        if (result == StepResult.AddRecursion) addRecursion = true;
        else if (result == StepResult.Positive) {
          if (longest == null) goto keepBranch;

          if (longest.Consumed < branch.Consumed) {
            cache.EndBranch(longest);
            goto keepBranch;
          }
        }

        cache.EndBranch(branch);
        goto @continue;

        keepBranch:
        longest = branch;

        @continue:
        if (longest != null) cache.CurrentBranch = longest;
      }

      if (longest == null) {
        ctx.EndStaging(cache, false);

        return addRecursion ? StepResult.AddRecursion : StepResult.Negative;
      }

      ctx.EndStaging(cache, true, false);
      ctx.AddSymbol(new Production(Name, cache.Symbols));

      return StepResult.Positive;
    }
  }
}

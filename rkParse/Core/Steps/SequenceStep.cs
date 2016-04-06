using rkParse.Core.Staging;
using rkParse.Core.Symbols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rkParse.Core.Steps {
  public struct SequenceStepItem<TContext> where TContext : ProducerContext<TContext> {
    bool isOptional;
    ProducerStep<TContext> step;

    public bool IsOptional => isOptional;
    public ProducerStep<TContext> Step => step;

    public SequenceStepItem(ProducerStep<TContext> step, bool isOptional) {
      this.step = step;
      this.isOptional = isOptional;
    }
  }

  public class SequenceStep<TContext> : NonterminalStep<TContext> where TContext : ProducerContext<TContext> {
    List<SequenceStepItem<TContext>> items;

    protected override IEnumerable<ProducerStep<TContext>> SubSteps => from item in items
                                                                       select item.Step;

    public SequenceStep(string name, IEnumerable<SequenceStepItem<TContext>> items) : base(name) {
      this.items = items.ToList();
    }

    public SequenceStep(IEnumerable<SequenceStepItem<TContext>> items) : this(null, items) { }

    public SequenceStep(string name = null) : base(name) {
      items = new List<SequenceStepItem<TContext>>();
    }

    public SequenceStep<TContext> Add(ProducerStep<TContext> step, bool isOptional = false) {
      items.Add(new SequenceStepItem<TContext>(step, isOptional));

      return this;
    }

    protected override StepResult ExecuteInternal(TContext ctx) {
      StagingCache cache = ctx.BeginStaging();
      StepResult result;
      bool addRecursion = false;

      foreach (SequenceStepItem<TContext> item in items) {
        result = item.Step.Execute(ctx);
        switch (result) {
          case StepResult.Positive: continue;
          case StepResult.Negative:
            if (!item.IsOptional) goto negative;
            break;
          case StepResult.AddRecursion:
            addRecursion = true;
            goto case StepResult.Negative;
        }

        negative:
        ctx.EndStaging(cache, false);

        return addRecursion ? StepResult.AddRecursion : StepResult.Negative;
      }

      ctx.EndStaging(cache, true, false);

      ctx.AddSymbol(new Production(Name, cache.Symbols));

      return StepResult.Positive;
    }
  }
}

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

    protected override bool ExecuteInternal(TContext ctx) {
      StagingCache cache = ctx.BeginStaging();

      foreach (SequenceStepItem<TContext> item in items) {
        if (!(item.Step.Execute(ctx) || item.IsOptional)) {
          ctx.EndStaging(cache, false);

          return false;
        }
      }

      ctx.EndStaging(cache, true, false);

      ctx.AddSymbol(new Production(Name, cache.Symbols));

      return true;
    }
  }
}

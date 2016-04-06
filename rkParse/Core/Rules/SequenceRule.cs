using rkParse.Core.Steps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rkParse.Core.Rules {
  public struct SequenceItem<TRule> where TContext : ProducerContext<TContext> {
    public ProducerStep<TContext> Step { get; }
    public bool IsOptional { get; }

    public SequenceItem(ProducerStep<TContext> step, bool isOptional = false) {
      Step = step;
      IsOptional = isOptional;
    }
  }

  public class SequenceRule<TContext> where TContext : ProducerContext<TContext> {
    SequenceItem<TContext>[]
  }
}

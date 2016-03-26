using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rkParse.Core.Steps {
  public abstract class TerminalStep<TContext> : ProducerStep<TContext> where TContext : ProducerContext<TContext> {
    public sealed override bool CanBeTerminal => true;
    public sealed override bool IsRecursive => false;

    public TerminalStep(string name = null) : base(name) { }

    protected abstract bool ExecuteTerminal(TContext ctx);

    protected sealed override StepResult ExecuteInternal(TContext ctx) {
      return ExecuteTerminal(ctx) ? StepResult.Positive : StepResult.Negative;
    }
  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rkParse.Core.Steps {
  public abstract class TerminalStep<TContext> : ProducerStep<TContext> where TContext : ProducerContext<TContext> {
    public sealed override bool IsRecursionSafe => false;

    public TerminalStep(string name = null) : base(name) { }
  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rkParse.Core.Steps {
  public class NamedProducerStep<TContext> : ProducerStep<TContext> where TContext : ProducerContext<TContext> {
    string refName;

    public string ReferenceName => refName;

    public override bool CanBeTerminal {
      get {
        throw new NotImplementedException();
      }
    }

    public override bool Execute(TContext ctx) {
      return ctx.Execute(refName);
    }
  }
}

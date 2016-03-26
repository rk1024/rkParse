using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rkParse.Core.Steps {
  public class NamedStep<TContext> : NonterminalStep<TContext> where TContext : ProducerContext<TContext> {
    Lexicon<TContext> steps;
    string refName;

    protected override IEnumerable<ProducerStep<TContext>> SubSteps {
      get { yield return Step; }
    }

    public ProducerStep<TContext> Step => steps[refName];
    public string ReferenceName => refName;

    public NamedStep(Lexicon<TContext> lexicon, string refName) : base(refName) {
      steps = lexicon;
      this.refName = refName;
    }

    protected override StepResult ExecuteInternal(TContext ctx) {
      return ctx.Execute(Step);
    }
  }
}

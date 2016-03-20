using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rkParse.Core.Steps {
  public class NamedStep<TContext> : ProducerStep<TContext> where TContext : ProducerContext<TContext> {
    Lexicon<TContext> steps;
    string refName;

    public override bool CanBeTerminal => Step.CanBeTerminal;

    public string ReferenceName => refName;
    public ProducerStep<TContext> Step => steps[refName];

    public NamedStep(string name, Lexicon<TContext> lexicon, string refName) : base(name) {
      steps = lexicon;
      this.refName = refName;
    }

    public NamedStep(Lexicon<TContext> lexicon, string refName) : this(null, lexicon, refName) { }

    protected override bool ExecuteInternal(TContext ctx) {
      return ctx.Execute(Step);
    }
  }
}

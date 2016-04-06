using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rkParse.Core.Steps {
  public class NamedStep<TContext> : NonterminalStep<TContext>, IEquatable<NamedStep<TContext>> where TContext : ProducerContext<TContext> {
    Lexicon<TContext> steps;
    string refName;

    protected override IEnumerable<ProducerStep<TContext>> SubSteps {
      get { yield return Step; }
    }

    public ProducerStep<TContext> Step => steps[refName];
    public string ReferenceName => refName;

    public NamedStep(string name, Lexicon<TContext> lexicon, string refName) : base(name) {
      steps = lexicon;
      this.refName = refName;
    }

    public NamedStep(Lexicon<TContext> lexicon, string refName) : this(refName, lexicon, refName) { }

    protected override StepResult ExecuteInternal(TContext ctx) {
      return ctx.Execute(Step);
    }

    public override int GetHashCode() {
      return steps.GetHashCode() ^ refName.GetHashCode();
    }

    public bool Equals(NamedStep<TContext> other) {
      return this == other;
    }

    public override bool Equals(object obj) {
      return Equals(obj as NamedStep<TContext>);
    }

    public static bool operator ==(NamedStep<TContext> a, NamedStep<TContext> b) {
      bool aNull = Equals(a, null);
      if (aNull != Equals(b, null)) return false;
      if (aNull) return true;

      return ReferenceEquals(a, b) || (a.steps == b.steps && a.refName == b.refName);
    }

    public static bool operator !=(NamedStep<TContext> a, NamedStep<TContext> b) {
      bool aNull = Equals(a, null);
      if (aNull != Equals(b, null)) return true;
      if (aNull) return false;

      return !ReferenceEquals(a, b) && (a.steps != b.steps || a.refName != b.refName);
    }
  }
}

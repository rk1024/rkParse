using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rkParse.Core.Steps {
  public abstract class NonterminalStep<TContext> : ProducerStep<TContext> where TContext : ProducerContext<TContext> {
    bool? isRecursive = null,
      canBeTerminal = null;

    protected abstract IEnumerable<ProducerStep<TContext>> SubSteps { get; }

    protected IEnumerable<TerminalStep<TContext>> TerminalSubSteps => from step in SubSteps
                                                                      let term = step as TerminalStep<TContext>
                                                                      where term != null
                                                                      select term;

    protected IEnumerable<NonterminalStep<TContext>> NontermSubSteps => from step in SubSteps
                                                                        let nonterm = step as NonterminalStep<TContext>
                                                                        where nonterm != null
                                                                        select nonterm;

    protected IEnumerable<ProducerStep<TContext>> SubStepTree {
      get {
        HashSet<ProducerStep<TContext>> @checked = new HashSet<ProducerStep<TContext>>();
        Queue<NonterminalStep<TContext>> queue = new Queue<NonterminalStep<TContext>>();

        queue.Enqueue(this);

        do {
          NonterminalStep<TContext> nonterm = queue.Dequeue();

          foreach (TerminalStep<TContext> step in nonterm.TerminalSubSteps) {
            if (!@checked.Contains(step)) {
              @checked.Add(step);

              yield return step;
            }
          }

          foreach (NonterminalStep<TContext> step in nonterm.NontermSubSteps) {
            if (!@checked.Contains(step)) {
              @checked.Add(step);
              queue.Enqueue(step);

              yield return step;
            }
          }
        } while (queue.Any());
      }
    }

    public NonterminalStep(string name = null) : base(name) { }

    public sealed override bool CanBeTerminal {
      get {
        if (!canBeTerminal.HasValue) canBeTerminal = GetCanBeTerminal();

        return canBeTerminal.Value;
      }
    }

    public sealed override bool IsRecursive {
      get {
        if (!isRecursive.HasValue) isRecursive = GetIsRecursive();

        return isRecursive.Value;
      }
    }

    protected virtual bool GetIsRecursive() {
      return NontermSubSteps.Any() && MayDeferTo(this);
    }

    protected virtual bool GetCanBeTerminal() {
      return TerminalSubSteps.Any() || !NontermSubSteps.All(e => e.MayDeferTo(this));
    }

    public virtual bool MayDeferTo(ProducerStep<TContext> descendant) {
      return SubStepTree.Any(e => e == descendant);
    }
  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rkParse.Core.Steps {
  public abstract class NonterminalStep<TContext> : ProducerStep<TContext> where TContext : ProducerContext<TContext> {
    bool? isRecursionSafe = null;

    protected abstract IEnumerable<ProducerStep<TContext>> SubSteps { get; }

    public NonterminalStep(string name = null) : base(name) { }

    public sealed override bool IsRecursionSafe {
      get {
        if (!isRecursionSafe.HasValue) {
          isRecursionSafe = true;

          HashSet<ProducerStep<TContext>> @checked = new HashSet<ProducerStep<TContext>>();
          Queue<ProducerStep<TContext>> queue = new Queue<ProducerStep<TContext>>();

          queue.Enqueue(this);

          while (queue.Count > 0) {
            if (queue.Peek() != this) @checked.Add(queue.Peek());

            NonterminalStep<TContext> nonterm = queue.Dequeue() as NonterminalStep<TContext>;

            if (nonterm != null) {
              foreach (ProducerStep<TContext> step in nonterm.SubSteps) {
                if (step == this) { isRecursionSafe = false; break; }

                if (!@checked.Contains(step)) queue.Enqueue(step);
              }
            }
          }
        }

        return isRecursionSafe.Value;
      }
    }
  }
}

using rkParse.Core.Steps;
using System.Collections.Generic;

namespace rkParse.Core {
  public class Lexicon<TContext> where TContext : ProducerContext {
    Dictionary<string, ProducerStep<TContext>> steps = new Dictionary<string, ProducerStep<TContext>>();
    string rootStep;

    public string RootStep {
      get { return rootStep; }
      set {
        if (!steps.ContainsKey(value)) throw new KeyNotFoundException("Specified step name does not exist in Lexicon's dictionary.");

        rootStep = value;
      }
    }

    public Lexicon() { }

    public ProducerStep<TContext> this[string key] => steps[key];

    public void Add(ProducerStep<TContext> step) => steps.Add(step.Name, step);
  }
}

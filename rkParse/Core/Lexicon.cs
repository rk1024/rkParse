using rkParse.Core.Steps;
using System;
using System.Collections.Generic;
using System.Linq;

namespace rkParse.Core {
  public class Lexicon<TContext> where TContext : ProducerContext {
    Dictionary<string, ProducerStep<TContext>> steps = new Dictionary<string, ProducerStep<TContext>>();
    string rootStepName;

    public string RootStepName {
      get { return rootStepName; }
      set {
        if (!steps.ContainsKey(value)) throw new KeyNotFoundException("Specified step name does not exist in Lexicon's dictionary.");

        rootStepName = value;
      }
    }

    public ProducerStep<TContext> RootStep {
      get {
        ProducerStep<TContext> step;
        if (steps.TryGetValue(RootStepName, out step)) return step;
        return null;
      }
      set {
        if (!steps.ContainsValue(value)) throw new KeyNotFoundException("Specified step does not exist in Lexicon's dictionary.");

        rootStepName = steps.First((el) => el.Value == value).Key;
      }
    }

    public Lexicon() { }

    public ProducerStep<TContext> this[string key] => steps[key];

    public Lexicon<TContext> Add(ProducerStep<TContext> step) {
      if (step.Name == null) throw new ArgumentOutOfRangeException("step", step, "Step name cannot be null.");

      steps.Add(step.Name, step);
      return this;
    }
  }
}

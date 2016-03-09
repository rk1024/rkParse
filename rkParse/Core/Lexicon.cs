using rkParse.Core.Steps;
using System.Collections.Generic;

namespace rkParse.Core {
  public class Lexicon {
    Dictionary<string, ProducerStep> steps = new Dictionary<string, ProducerStep>();
    string rootStep;

    public string RootStep {
      get { return rootStep; }
      set {
        if (!steps.ContainsKey(value)) throw new KeyNotFoundException("Specified step name does not exist in Lexicon's dictionary.");

        rootStep = value;
      }
    }

    public Lexicon() { }

    public ProducerStep this[string key] => steps[key];

    public void Add(ProducerStep step) => steps.Add(step.Name, step);
  }
}

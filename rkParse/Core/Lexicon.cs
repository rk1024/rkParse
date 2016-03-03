using rkParse.Core.Steps;
using System.Collections.Generic;

namespace rkParse.Core {
  public class Lexicon {
    Dictionary<string, LexerStep> steps = new Dictionary<string, LexerStep>();
    string rootStep;

    public string RootStep {
      get { return rootStep; }
      set {
        if (!steps.ContainsKey(value)) throw new KeyNotFoundException("Specified step name does not exist in Lexicon's dictionary.");

        rootStep = value;
      }
    }

    public Lexicon() { }

    public LexerStep this[string key] => steps[key];

    public void Add(LexerStep step) => steps.Add(step.Name, step);
  }
}

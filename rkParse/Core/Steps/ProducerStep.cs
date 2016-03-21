using rkParse.Util;
using System;

namespace rkParse.Core.Steps {
  public abstract class ProducerStep<TContext> where TContext : ProducerContext<TContext> {
    bool initialized = false;
    string name;

    public abstract bool IsRecursionSafe { get; }

    public string Name => name;

    public ProducerStep(string name = null) {
      this.name = name;
    }

    protected abstract bool ExecuteInternal(TContext ctx);

    protected virtual void Initialize(TContext ctx) {
      if (Name == null) Console.WriteLine($"Initializing anonymous {GetType().Name}...");
      else Console.WriteLine($"Initializing {GetType().Name} {name.ToLiteral()}...");
    }

    public bool Execute(TContext ctx) {
      if (!initialized) { Initialize(ctx); initialized = true; }

      return ExecuteInternal(ctx);
    }
  }
}

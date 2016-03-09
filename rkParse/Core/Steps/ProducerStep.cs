namespace rkParse.Core.Steps {
  public abstract class ProducerStep<TContext> where TContext : ProducerContext {
    string name;

    public abstract bool CanBeTerminal { get; }

    public string Name => name;

    public ProducerStep(string name = null) {
      this.name = name;
    }

    public abstract bool Execute(TContext ctx);
  }
}

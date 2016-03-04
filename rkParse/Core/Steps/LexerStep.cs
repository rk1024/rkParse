namespace rkParse.Core.Steps {
  public abstract class LexerStep {
    string name;

    public abstract bool CanBeTerminal { get; }

    public string Name => name;

    public LexerStep(string name = null) {
      this.name = name;
    }

    public abstract bool Execute(LexingContext ctx);
  }
}

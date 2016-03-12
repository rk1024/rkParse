namespace rkParse.Lexical.Steps {
  public abstract class LexerStepOld {
    string name;

    public string Name => name;

    public LexerStepOld(string name) {
      this.name = name;
    }

    public LexerStepOld() : this(null) { }

    public abstract bool Exec(LexingContextOld ctx);

    public abstract bool Query(LexingContextOld ctx, out int count, int start = 0);

    //Use start as the out parameter for compactness
    public bool Query(LexingContextOld ctx, int start = 0) => Query(ctx, out start, start);
  }
}

namespace rkParse.Lexical.Steps {
  public abstract class LexerQueryConsumeStep : LexerStepOld {
    public LexerQueryConsumeStep(string name) : base(name) { }

    public LexerQueryConsumeStep() : base() { }

    public abstract void Consume(LexingContextOld ctx, int count);

    public override bool Exec(LexingContextOld ctx) {
      int count;
      if (Query(ctx, out count)) {
        Consume(ctx, count);

        return true;
      }

      return false;
    }
  }
}

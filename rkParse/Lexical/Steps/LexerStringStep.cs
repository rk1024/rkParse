using rkParse.Lexical.Symbols;

namespace rkParse.Lexical.Steps {
  public class LexerStringStep : LexerQueryConsumeStep {
    string pat;

    public string Pattern => pat;

    public LexerStringStep(string name, string pat) : base(name) {
      this.pat = pat;
    }

    public LexerStringStep(string pat) : this(null, pat) { }

    public override bool Query(LexingContextOld ctx, out int count, int start = 0) {
      if (ctx.QueryString(Pattern, start)) {
        count = Pattern.Length;
        return true;
      }

      count = 0;
      return false;
    }

    public override void Consume(LexingContextOld ctx, int count) {
      ctx.AddSymbol(new StringSymbol(Name, Pattern));
    }
  }
}

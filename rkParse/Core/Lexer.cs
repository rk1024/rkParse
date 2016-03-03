using rkParse.Core.Symbols;
using System.Collections.Generic;

namespace rkParse.Core {
  public abstract class Lexer {
    Lexicon lexicon;
    bool isReading = false;

    public Lexicon Lexicon => lexicon;
    public bool IsReading => isReading;

    public Lexer(Lexicon lexicon) {
      this.lexicon = lexicon;
    }

    public abstract LexingContext MakeContext();

    public Symbol[] Read() {
      isReading = true;

      LexingContext ctx = MakeContext();

      lexicon[lexicon.RootStep].Execute(ctx);

      isReading = false;

      return ctx.Output;
    }
  }
}

using rkParse.IO;
using rkParse.Lexical.Symbols;
using System.Collections.Generic;
using System.IO;

namespace rkParse.Lexical {
  public class LexerOld {
    BufferedStreamReader reader;
    LexiconOld lexicon;
    LexingContextOld ctx = null;
    string rootStep;
    bool isLexing = false;

    public bool IsLexing => isLexing;

    public LexerOld(LexiconOld lexicon, string rootStep, Stream stream) {
      this.lexicon = lexicon;
      this.rootStep = rootStep;

      reader = new BufferedStreamReader(stream);
    }

    public SymbolOld[] Lex() {
      isLexing = true;

      ctx = new LexingContextOld(this, lexicon, reader);

      List<SymbolOld> symbols = new List<SymbolOld>();

      ctx.BeginProduction(symbols);

      ctx.ExecStep(rootStep);

      ctx.EndProduction(symbols);

      isLexing = false;

      return symbols.ToArray();
    }
  }
}

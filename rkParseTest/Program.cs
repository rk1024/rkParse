using rkParse.IO;
using rkParse.Lexical;
using rkParse.Lexical.SymbolFactories;
using rkParse.Lexical.Symbols;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace rkParseTest {
  class InputSymbolFactory : ProductionFactory {
    public InputSymbolFactory() : base(new[] { new SymbolSeqItem(new InputSectionSymbolFactory(), true) }) { }

    protected override ISymbol CreateSymbol(IEnumerable<ISymbol> childSymbols) {
      return new InputSymbol(childSymbols);
    }
  }

  class InputSymbol : Production {
    public InputSymbol(IEnumerable<ISymbol> symbols) : base(symbols) { }
  }

  class InputSectionSymbolFactory : ProductionOneOfFactory {
    static InputSectionSymbolFactory instance;

    public InputSectionSymbolFactory() : base(new[] {
      new SymbolSequencer(new[] {
        new SymbolSeqItem(new InputSectionPartSymbolFactory())
      }),
      new SymbolSequencer(new[] {
        new SymbolSeqItem(new InputSectionSymbolFactory()), //TODO: work around stack overflow (maybe add recurse symbol?)
        new SymbolSeqItem(new InputSectionPartSymbolFactory()),
      })
    }) {
    }

    protected override ISymbol CreateSymbol(IEnumerable<ISymbol> childSymbols) {
      return new InputSectionSymbol(childSymbols);
    }
  }

  class InputSectionSymbol : Production {
    public InputSectionSymbol(IEnumerable<ISymbol> symbols) : base(symbols) { }
  }

  class InputSectionPartSymbolFactory : ProductionOneOfFactory {
    public InputSectionPartSymbolFactory() : base(new[] {
      new SymbolSeqSingle(new StringSymbolFactory("hi")),
      new SymbolSeqSingle(new StringSymbolFactory(" ")),
      new SymbolSeqSingle(new StringSymbolFactory("there"))
    }) { }
  }

  class Program {
    static void Main(string[] args) {
      using (var stream = new MemoryStream())
      using (var writer = new StreamWriter(stream)) {
        Lexer lex = new Lexer(new InputSymbolFactory(), stream);

        Console.Write("Enter text to stream: ");

        writer.Write(Console.ReadLine());
        writer.Flush();

        stream.Position = 0;

        ISymbol[] symbols = lex.Parse();

        Console.WriteLine($"Symbols ({symbols.Length}): {string.Join<ISymbol>(", ", symbols)}");

        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
      }
    }
  }
}
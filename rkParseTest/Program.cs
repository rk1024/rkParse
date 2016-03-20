using rkParse.Core.Steps;
using rkParse.Lexical;
using rkParse.Lexical.Symbols;
using System;
using System.IO;
using rkParse.Core;
using rkParse.Core.Symbols;
using rkParse.Lexical.Steps;
using rkParse.Util;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace rkParseTest {
  class Program {
    static void Main(string[] args) {
      start:
      Lexer lexer = new Lexer();

      lexer.Steps.Add(new OneOfStep<LexerContext>("Article")
          .Add(new LexerStringStep("The"))
          .Add(new LexerStringStep("Th"))
          .Add(new LexerStringStep("A")))

        .Add(new LexerRegexStep("TestRegex", new Regex(@"[A-Za-z]")))
        .Add(new OneOrMoreStep<LexerContext>("TestRegexes", lexer.Steps.NamedStep("TestRegex")))

        .Add(new LexerRegexStep("SpaceCharacter", new Regex(@"\s")))
        .Add(new OneOrMoreStep<LexerContext>("SpaceCharacters", lexer.Steps.NamedStep("SpaceCharacter")))

        .Add(new SequenceStep<LexerContext>("QualifiedWord")
          .Add(lexer.Steps.NamedStep("Article"))
          .Add(lexer.Steps.NamedStep("SpaceCharacters"))
          .Add(lexer.Steps.NamedStep("TestRegexes")))

        .RootStepName = "QualifiedWord";

      Console.Write("> ");

      string line = Console.ReadLine();

      Console.WriteLine($"Input: {line.ToLiteral()}");

      Symbol[] symbols;

      using (Stream stream = new MemoryStream(Encoding.Default.GetBytes(line)))
        symbols = lexer.Read(stream);

      Console.WriteLine($"Result: [{string.Join<Symbol>(", ", symbols)}]");

      Console.CursorVisible = false;
      Console.WriteLine("Press any key to continue...");
      Console.ReadKey(true);
      Console.CursorVisible = true;

      Console.Clear();

      goto start;
    }
  }
}
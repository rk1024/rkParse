using rkParse.Core.Steps;
using rkParse.Lexical;
using rkParse.Lexical.Symbols;
using System;
using System.IO;
using rkParse.Core;
using rkParse.Core.Symbols;
using rkParse.Lexical.Steps;
using rkParse.Util;
using System.Text;
using System.Text.RegularExpressions;

namespace rkParseTest {
  class Program {
    static void Main(string[] args) {
      start:
      Lexer lexer = new Lexer();

      lexer.Steps.Add(new LexerStringStep("Test", "foo"));

      lexer.Steps.Add(new LexerRegexStep("TestRegex", new Regex(@"[A-Za-z]")));

      lexer.Steps.RootStepName = "TestRegex";

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
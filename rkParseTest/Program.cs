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
using System.Diagnostics;

namespace rkParseTest {
  class Program {
    static void Main(string[] args) {
      start:
      Lexer lexer = new Lexer();

      var steps = lexer.Steps;

      steps.Add(new SequenceStep<LexerContext>("Input")
        .Add(steps.NamedStep("InputSection"), true));

      steps.Add(new OneOfStep<LexerContext>("InputSection")
        .Add(steps.NamedStep("InputSectionPart"))
        .Add(new SequenceStep<LexerContext>()
          .Add(steps.NamedStep("InputSection"))
          .Add(steps.NamedStep("InputSectionPart"))));

      steps.Add(new OneOfStep<LexerContext>("InputSectionPart")
        .Add(new SequenceStep<LexerContext>()
          .Add(steps.NamedStep("InputElements"), true)
          .Add(steps.NamedStep("NewLine")))
        .Add(steps.NamedStep("PPDirective")));

      steps.Add(new OneOfStep<LexerContext>("InputElements")
        .Add(new SequenceStep<LexerContext>()
          .Add(steps.NamedStep("InputElement"))
          .Add(steps.NamedStep("InputElements")))
        .Add(steps.NamedStep("InputElement")));

      steps.Add(new OneOfStep<LexerContext>("InputElement")
        .Add(steps.NamedStep("Whitespace"))
        .Add(steps.NamedStep("Comment"))
        .Add(steps.NamedStep("Token")));

      steps.Add(new OneOfStep<LexerContext>("NewLine")
        .Add(new LexerStringStep("\u000d"))
        .Add(new LexerStringStep("\u000a"))
        .Add(new LexerStringStep("\u000d\u000a"))
        .Add(new LexerStringStep("\u2085"))
        .Add(new LexerStringStep("\u2028"))
        .Add(new LexerStringStep("\u2029")));

      steps.Add(new NamedStep<LexerContext>("Whitespace", steps, "WhitespaceCharacters"));

      steps.Add(new OneOfStep<LexerContext>("WhitespaceCharacters")
        .Add(new SequenceStep<LexerContext>()
          .Add(steps.NamedStep("WhitespaceCharacter"))
          .Add(steps.NamedStep("WhitespaceCharacters")))
        .Add(steps.NamedStep("WhitespaceCharacter")));

      steps.Add(new OneOfStep<LexerContext>("WhitespaceCharacter")
        .Add(new LexerRegexStep(new Regex(@"\p{Zs}")))
        .Add(new LexerStringStep("\u0009"))
        .Add(new LexerStringStep("\u000b"))
        .Add(new LexerStringStep("\u000c")));

      steps.Add(new OneOfStep<LexerContext>("Comment")
        .Add(steps.NamedStep("SingleLineComment"))
        .Add(steps.NamedStep("DelimitedComment")));

      lexer.Steps.Add(new OneOfStep<LexerContext>("Article")
          .Add(new LexerStringStep("The"))
          .Add(new LexerStringStep("Th"))
          .Add(new LexerStringStep("A")))

        .Add(new LexerRegexStep("TestRegex", new Regex(@"[A-Za-z]")))
        //.Add(new OneOrMoreStep<LexerContext>("TestRegexes", lexer.Steps.NamedStep("TestRegex")))

        .Add(new ANotBStep<LexerContext>("NotC", lexer.Steps.NamedStep("TestRegex"), new LexerStringStep("c")))

        .Add(new OneOfStep<LexerContext>("TestRegexes")
          .Add(new SequenceStep<LexerContext>()
            .Add(lexer.Steps.NamedStep("TestRegexes"))
            .Add(lexer.Steps.NamedStep("NotC")))
          .Add(lexer.Steps.NamedStep("NotC")))

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
      byte[] bytes = Encoding.Default.GetBytes(line);

      using (Stream stream = new MemoryStream(bytes))
        symbols = lexer.Read(stream);

      Console.WriteLine($"Result: [{string.Join<Symbol>(", ", symbols)}]");
      Console.WriteLine($"Compct: [{string.Join(", ", (from symbol in symbols select symbol.ToStringCompact()))}]");
      Console.WriteLine("Multiline: " + string.Join(",\n", (from symbol in symbols
                                                            select symbol.ToStringMultiline())));

      Console.WriteLine("--- Performance test ---");
      Console.WriteLine("Enter test duration in seconds (0 to skip):");

      double seconds;

      {
        @input:

        Console.Write("> ");

        bool success = double.TryParse(Console.ReadLine().Trim(), out seconds);

        if (!success) {
          Console.ForegroundColor = ConsoleColor.Red;
          Console.WriteLine("Invalid input.");
          Console.ResetColor();

          goto @input;
        }
      }

      if (seconds == 0) {
        Console.WriteLine("Performance testing skipped.");
      }
      else {
        TimeSpan endSpan = TimeSpan.FromSeconds(seconds);

        Console.WriteLine($"Running performance test for duration of {endSpan:d\\.hh\\:mm\\:ss\\.ffFFFFF} ({endSpan.Ticks} tick{(endSpan.Ticks == 1 ? "" : "s")})...");

        Stopwatch watch = new Stopwatch();
        long count = 0,
          end = (long)Math.Round(endSpan.TotalSeconds * Stopwatch.Frequency),
          interval = (long)Math.Round(new TimeSpan(0, 0, 5).TotalSeconds * Stopwatch.Frequency),
          intervals = 0, div;

        try {
          watch.Start();

          do {
            using (Stream stream = new MemoryStream(bytes))
              lexer.Read(stream);

            if ((div = watch.ElapsedTicks / interval) > intervals) {
              intervals = div;
              Console.Write(count);
              Console.WriteLine(" iterations...");
            }

            ++count;
          } while (watch.ElapsedTicks < end);
        }
        catch (Exception e) {
          watch.Stop();

          Console.WriteLine($"Test terminated due to exception:\n{e.ToString().Replace("\n", "\n  ")}");
        }

        watch.Stop();

        Console.WriteLine($"Lexer was run {count} times in {watch.Elapsed:hh\\:mm\\:ss\\.ffffff}");
        Console.WriteLine($"  Average: {count / watch.Elapsed.TotalSeconds} iters per second,");
        Console.WriteLine($"           {watch.Elapsed.TotalSeconds / count} seconds per iter");

      }

      Console.CursorVisible = false;
      Console.WriteLine("Press any key to continue...");
      Console.ReadKey(true);
      Console.CursorVisible = true;

      Console.Clear();

      goto start;
    }
  }
}
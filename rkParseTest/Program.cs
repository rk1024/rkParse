using rkParse.Core.Steps;
using rkParse.Lexical;
using rkParse.Lexical.Symbols;
using System;
using System.IO;
using rkParse.Core;
using rkParse.Core.Symbols;

namespace rkParseTest {
  class TestProductionStep : RecursiveLexerStep {
    Random rand = new Random();

    protected override bool Execute(LexingContext ctx, bool canRecurse) {
      if (rand.NextDouble() < .99) {
        ctx.Consume(1);
        ctx.AddSymbol(new Symbol("Test"));
        return true;
      }
      return false;
    }

    public TestProductionStep(string name = null) : base(name) { }
  }

  class TestLexingContext : LexingContext {
    protected override void ConsumeInternal(int count) {
      Console.WriteLine($"Consumed {count} character(s).");
    }

    public TestLexingContext(TestLexer lex) : base(lex) { }
  }

  class TestLexer : Lexer {
    public override LexingContext MakeContext() {
      return new TestLexingContext(this);
    }

    public TestLexer(Lexicon lcon) : base(lcon) { }
  }

  class Program {
    static void Main(string[] args) {
      Lexicon lcon = new Lexicon();
      Lexer lex = new TestLexer(lcon);

      lcon.Add(new TestProductionStep("Test"));

      lcon.RootStep = "Test";

      lex.Read();

      Console.CursorVisible = false;
      Console.WriteLine("Press any key to continue...");
      Console.ReadKey(true);
      Console.CursorVisible = true;
    }

    static void _Main(string[] args) {
      LexiconOld lc = new LexiconOld();

      //lc.RegStringSym("Test", "test");

      //lc.RegOneOrMoreProd("Tests", "Test");

      //lc.RegOneOfProd("Choice", new[] {
      //  lc.MakeStringSym(" "),
      //  lc.MakeStringSym("hi"),
      //  lc.MakeStringSym("there"),
      //  lc.MakeSeqProd(new[] {
      //    lc.MakeStringSym("lel").ToSeqStep(),
      //    lc.MakeOneOrMoreProd(lc.MakeStringSym(" ")).ToSeqStep(),
      //    lc.MakeStringSym("jk").ToSeqStep(),
      //  })
      //}, new[] {
      //  "Tests"
      //});

      //lc.RegOneOrMoreProd("Choices", "Choice");

      //new Producer("Input", delegate (Producer self, LexiconBase lex) {
      //  self.AddItem(lex["InputSection"], true);
      //}).Register(lexicon);

      lc.RegSeqProd("Input", new[] {
        lc.MakeSeqStep("InputSection", true),
      });

      //new ProducerOneOf("InputSection", delegate (ProducerOneOf self, LexiconBase lex) {
      //  self.AddChoice(lex["InputSectionPart"]);
      //  self.AddChoice(lex["InputSection"], lex["InputSectionPart"]);
      //}).Register(lexicon);

      lc.RegOneOrMoreProd("InputSection", "InputSectionPart");

      //new ProducerOneOf("InputSectionPart", delegate (ProducerOneOf self, LexiconBase lex) {
      //  self.AddChoice(new SymbolSeqItem(lex["InputElements"], true), new SymbolSeqItem(lex["NewLine"]));
      //  //self.AddChoice(lex["PPDirective"]);
      //}).Register(lexicon);

      lc.RegOneOfProd("InputSectionPart", new[] {
        lc.MakeSeqProd(new[] {
          lc.MakeSeqStep("InputElements", true),
          lc.MakeSeqStep("NewLine", true),
        }),
      });//, new[] {
         //"PPDirective",
         //});

      //new ProducerOneOf("InputElements", delegate (ProducerOneOf self, LexiconBase lex) {
      //  self.AddChoice(lex["InputElement"]);
      //  self.AddChoice(lex["InputElements"], lex["InputElement"]);
      //}).Register(lexicon);

      lc.RegOneOrMoreProd("InputElements", "InputElement");

      //new ProducerOneOf("InputElement", delegate (ProducerOneOf self, LexiconBase lex) {
      //  //self.AddChoice(lex["Whitespace"]);
      //  self.AddChoice(lex["Comment"]);
      //  //self.AddChoice(lex["Token"]);
      //}).Register(lexicon);

      lc.RegOneOfProd("InputElement", new[] {
        //"Whitespace",
        "Comment",
        //"Token",
      });

      //new ProducerOneOf("NewLine", delegate (ProducerOneOf self, LexiconBase lex) {
      //  self.AddChoice(lex["CarriageReturn"]);
      //  self.AddChoice(lex["LineFeed"]);
      //  self.AddChoice(lex["CarriageReturn"], lex["LineFeed"]);
      //  self.AddChoice(lex["NextLine"]);
      //  self.AddChoice(lex["LineSeparator"]);
      //  self.AddChoice(lex["ParagraphSeparator"]);
      //}).Register(lexicon);

      lc.RegOneOfProd("NewLine", new[] {
        lc.MakeSeqProd(new[] {
          lc.MakeSeqStep("CarriageReturn"),
          lc.MakeSeqStep("LineFeed"),
        }),
      }, new[] {
        "CarriageReturn",
        "LineFeed",
        "NextLine",
        "LineSeparator",
        "ParagraphSeparator",
      });

      //new StringSymbolFactory("CarriageReturn", "\u000d").Register(lexicon);
      //new StringSymbolFactory("LineFeed", "\u000a").Register(lexicon);
      //new StringSymbolFactory("NextLine", "\u2085").Register(lexicon);
      //new StringSymbolFactory("LineSeparator", "\u2028").Register(lexicon);
      //new StringSymbolFactory("ParagraphSeparator", "\u2029").Register(lexicon);

      lc.RegStringSym("CarriageReturn", "\u000d");
      lc.RegStringSym("LineFeed", "\u000a");
      lc.RegStringSym("NextLine", "\u2085");
      lc.RegStringSym("LineSeparator", "\u2028");
      lc.RegStringSym("ParagraphSeparator", "\u2029");

      //new ProducerOneOf("Comment", delegate (ProducerOneOf self, LexiconBase lex) {
      //  self.AddChoice(lex["SingleLineComment"]);
      //  self.AddChoice(lex["DelimitedComment"]);
      //}).Register(lexicon);

      lc.RegOneOfProd("Comment", new[] {
        "SingleLineComment",
        //"DelimitedComment",
      });

      //new Producer("SingleLineComment", delegate (Producer self, LexiconBase lex) {
      //  self.AddItem(new StringSymbolFactory("//"));
      //  self.AddItem(lex["InputCharacters"], true);
      //}).Register(lexicon);

      lc.RegSeqProd("SingleLineComment", new[] {
        lc.MakeStringSym("//").ToSeqStep(),
        lc.MakeSeqStep("InputCharacters", true),
      });

      //new ProducerOneOf("InputCharacters", delegate (ProducerOneOf self, LexiconBase lex) {
      //  self.AddChoice(lex["InputCharacter"]);
      //  self.AddChoice(lex["InputCharacters"], lex["InputCharacter"]);
      //}).Register(lexicon);

      lc.RegOneOrMoreProd("InputCharacters", "InputCharacter");

      //new StringSymbolFactory("InputCharacter", "a").Register(lexicon); //TODO: Add a 'not' symbol

      lc.RegStringSym("InputCharacter", "a"); //TODO: Add a 'not' step

      //new ProducerOneOf("NewLineCharacter", delegate (ProducerOneOf self, LexiconBase lex) {
      //  self.AddChoice(lex["CarriageReturn"]);
      //  self.AddChoice(lex["LineFeed"]);
      //  self.AddChoice(lex["NextLine"]);
      //  self.AddChoice(lex["LineSeparator"]);
      //  self.AddChoice(lex["ParagraphSeparator"]);
      //}).Register(lexicon);

      lc.RegOneOfProd("NewLineCharacter", new[] {
        "CarriageReturn",
        "LineFeed",
        "NextLine",
        "LineSeparator",
        "ParagraphSeparator",
      });

      //lexicon.InitFactories();

      using (var stream = new MemoryStream())
      using (var writer = new StreamWriter(stream)) {
        LexerOld lex = new LexerOld(lc, "Input", stream);

        Console.Write("Enter text to stream: ");

        writer.Write(Console.ReadLine());
        writer.Flush();

        stream.Position = 0;

        SymbolOld[] symbols = lex.Lex();

        Console.WriteLine($"Symbols ({symbols.Length}): {string.Join<SymbolOld>(", ", symbols)}");

        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
      }
    }
  }
}
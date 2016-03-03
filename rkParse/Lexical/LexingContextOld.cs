using rkParse.IO;
using rkParse.Lexical.Symbols;
using rkParse.Util;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace rkParse.Lexical {
  public class LexingContextOld {
    LexerOld lexer;
    LexiconOld lexicon;
    BufferedStreamReader reader;
    Stack<List<SymbolOld>> productions = new Stack<List<SymbolOld>>(1);

    BufferedStreamReader Reader => reader;

    public LexingContextOld(LexerOld lexer, LexiconOld lexicon, BufferedStreamReader reader) {
      if (!lexer.IsLexing) throw new InvalidOperationException("Lexer must be currently lexing in order to create a LexingContext.");

      this.lexer = lexer;
      this.lexicon = lexicon;
      this.reader = reader;
    }

    public void BeginProduction(List<SymbolOld> symbols) {
      productions.Push(symbols);
    }

    /// <summary>
    /// Pops the symbol list for a production off of the stack.
    /// Throws an <code>InvalidOperationException</code> if the symbol list does not equal the argument passed.
    /// </summary>
    /// <param name="symbols">The symbol list to check the top of the stack against.</param>
    public void EndProduction(List<SymbolOld> symbols) {
      if (productions.Peek() != symbols) throw new InvalidOperationException("Tried to pop a production that was not at the top of the stack!");

      productions.Pop();
    }

    public int Consume(int count = 1) => Reader.Flush(count);

    public bool QueryString(string pattern) {
      Console.WriteLine($"Querying for string {pattern.ToLiteral()}");

      string buf;

      if (reader.Peek(out buf, pattern.Length) != pattern.Length) return false;

      return buf == pattern;
    }

    public bool QueryString(string pattern, int start) {
      Console.WriteLine($"Querying for string {pattern.ToLiteral()} at position {start}");

      string buf;
      if (reader.PeekAhead(out buf, start, pattern.Length) != pattern.Length) return false;

      return buf == pattern;
    }

    public bool QueryChar(char pattern) {
      string buf;
      if (reader.Peek(out buf) == 0) return false;

      return buf[0] == pattern;
    }

    public bool QueryChar(char pattern, int start) {
      string buf;
      if (reader.PeekAhead(out buf, start) == 0) return false;

      return buf[0] == pattern;
    }

    public bool QueryCharRegex(Regex pattern) {
      string buf;
      if (reader.Peek(out buf) == 0) return false;

      return pattern.IsMatch(buf);
    }

    public bool QueryCharRegex(Regex pattern, int start) {
      string buf;
      if (reader.PeekAhead(out buf, start) == 0) return false;

      return pattern.IsMatch(buf);
    }

    public void AddSymbol(SymbolOld symbol) {
      productions.Peek()?.Add(symbol);
    }

    public bool ExecStep(LexiconOld.LexerStep step) => step.Exec(this);

    public bool ExecStep(string name) => ExecStep(lexicon[name]);

    public bool QueryStep(LexiconOld.LexerStep step, int start, out int count) => step.Query(this, start, out count);

    public bool QueryStep(LexiconOld.LexerStep step, out int count) => QueryStep(step, 0, out count);

    public bool QueryStep(string name, int start, out int count) => QueryStep(lexicon[name], start, out count);

    public bool QueryStep(string name, out int count) => QueryStep(name, 0, out count);
  }
}

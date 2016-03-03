using rkParse.Lexical.Symbols;
using System.Collections.Generic;

namespace rkParse.Lexical {
  public class LexiconOld {
    public delegate bool LexerStepExecutor(LexingContextOld ctx);
    public delegate bool LexerStepQuerier(LexingContextOld ctx, int start, out int count);

    public delegate bool NamedLexerStepExecutor(string name, LexingContextOld ctx);
    public delegate bool NamedLexerStepQuerier(string name, LexingContextOld ctx, int start, out int count);

    public struct LexerStep {
      LexerStepExecutor executor;
      LexerStepQuerier querier;

      internal LexerStep(LexerStepExecutor executor, LexerStepQuerier querier) {
        this.executor = executor;
        this.querier = querier;
      }

      public bool Exec(LexingContextOld ctx) => executor(ctx);

      public bool Query(LexingContextOld ctx, int start, out int count) => querier(ctx, start, out count);

      public bool Query(LexingContextOld ctx, out int count) => Query(ctx, 0, out count);

      public LexerSeqStepBase ToSeqStep(bool isOptional = false) {
        return new LexerSeqStep(this, isOptional);
      }
    }

    public abstract class LexerSeqStepBase {
      bool isOptional;

      public bool IsOptional => isOptional;

      public LexerSeqStepBase(bool isOptional) {
        this.isOptional = isOptional;
      }

      public abstract bool Exec(LexingContextOld ctx);

      public abstract bool Query(LexingContextOld ctx, int start, out int count);

      public abstract bool Query(LexingContextOld ctx, out int count);
    }

    public class LexerSeqStep : LexerSeqStepBase {
      LexerStep step;

      internal LexerSeqStep(LexerStep step, bool isOptional) : base(isOptional) {
        this.step = step;
      }

      public override bool Exec(LexingContextOld ctx) => step.Exec(ctx);

      public override bool Query(LexingContextOld ctx, int start, out int count) => step.Query(ctx, start, out count);

      public override bool Query(LexingContextOld ctx, out int count) => Query(ctx, 0, out count);
    }

    public class RefLexerSeqStep : LexerSeqStepBase {
      string name;

      internal RefLexerSeqStep(string name, bool isOptional) : base(isOptional) {
        this.name = name;
      }

      public override bool Exec(LexingContextOld ctx) => ctx.ExecStep(name);

      public override bool Query(LexingContextOld ctx, int start, out int count) => ctx.QueryStep(name, start, out count);

      public override bool Query(LexingContextOld ctx, out int count) => Query(ctx, 0, out count);
    }

    private Dictionary<string, LexerStep> steps = new Dictionary<string, LexerStep>();

    public LexiconOld() { }

    public LexerStep this[string key] {
      get {
        return steps[key];
      }
    }

    public LexerStep MakeStep(LexerStepExecutor executor, LexerStepQuerier querier) => new LexerStep(executor, querier);

    public LexerSeqStepBase MakeSeqStep(LexerStep step, bool isOptional = false) => step.ToSeqStep(isOptional);

    public LexerSeqStepBase MakeSeqStep(string stepName, bool isOptional = false) => new RefLexerSeqStep(stepName, isOptional);

    public LexerStep MakeStringSym(string name, string pat) {
      return MakeStep((ctx) => {
        if (ctx.QueryString(pat)) {
          ctx.AddSymbol(name == null ? new StringSymbol(pat) : new SymbolOld(name));
          ctx.Consume(pat.Length);

          return true;
        }

        return false;
      }, (LexingContextOld ctx, int start, out int count) => {
        bool match = ctx.QueryString(pat, start);

        count = match ? pat.Length : 0;

        return match;
      });
    }

    public LexerStep MakeStringSym(string pat) => MakeStringSym(null, pat);

    public LexerStep MakeOneOrMoreProd(string name, LexerStep step) {
      return MakeStep((ctx) => {
        List<SymbolOld> symbols = new List<SymbolOld>();

        ctx.BeginProduction(symbols);

        while (step.Exec(ctx)) ;

        ctx.EndProduction(symbols);

        if (symbols.Count > 0) {
          ctx.AddSymbol(new Production(name, symbols));
          return true;
        }

        return false;
      }, (LexingContextOld ctx, int start, out int count) => {
        count = 0;

        bool match = false;
        int qCount;

        while (step.Query(ctx, start + count, out qCount)) {
          match = true;
          count += qCount;
        }

        return match;
      });
    }

    public LexerStep MakeOneOrMoreProd(LexerStep step) => MakeOneOrMoreProd(null, step);

    public LexerStep MakeOneOrMoreProd(string name, string stepName) {
      return MakeStep((ctx) => {
        List<SymbolOld> symbols = new List<SymbolOld>();

        ctx.BeginProduction(symbols);

        while (ctx.ExecStep(stepName)) ;

        ctx.EndProduction(symbols);

        if (symbols.Count > 0) {
          ctx.AddSymbol(new Production(name, symbols));
          return true;
        }

        return false;
      }, (LexingContextOld ctx, int start, out int count) => {
        count = 0;

        bool match = false;
        int qCount;

        while (ctx.QueryStep(stepName, start + count, out qCount)) {
          match = true;
          count += qCount;
        }

        return match;
      });
    }

    public LexerStep MakeOneOrMoreProd(string stepName) => MakeOneOrMoreProd(null, stepName);

    public LexerStep MakeOneOfProd(string name, LexerStep[] steps, string[] regSteps = null) {
      return MakeStep((ctx) => {
        LexerStep? longest = null;
        int len, longestLen = -1;

        if (steps != null) {
          foreach (LexerStep step in steps) {
            if (step.Query(ctx, out len) && len > longestLen) {
              longest = step;
              longestLen = len;
            }
          }
        }

        if (regSteps != null) {
          foreach (string stepName in regSteps) {
            if (ctx.QueryStep(stepName, out len) && len > longestLen) {
              longest = this[stepName];
              longestLen = len;
            }
          }
        }

        if (longestLen < 0) return false;

        List<SymbolOld> symbols = new List<SymbolOld>(1);

        ctx.BeginProduction(symbols);

        ctx.ExecStep(longest.Value);

        ctx.EndProduction(symbols);

        ctx.AddSymbol(new Production(name, symbols));

        return true;
      }, (LexingContextOld ctx, int start, out int count) => {
        count = -1;

        int len;

        if (steps != null) {
          foreach (LexerStep step in steps) {
            if (step.Query(ctx, start, out len) && len > count) {
              count = len;
            }
          }
        }

        if (regSteps != null) {
          foreach (string stepName in regSteps) {
            if (ctx.QueryStep(stepName, start, out len) && len > count) {
              count = len;
            }
          }
        }

        if (count < 0) {
          count = 0;

          return false;
        }

        return true;
      });
    }

    public LexerStep MakeOneOfProd(string name, string[] regSteps) => MakeOneOfProd(name, null, regSteps);

    public LexerStep MakeOneOfProd(LexerStep[] steps, string[] regSteps = null) => MakeOneOfProd(null, steps, regSteps);

    public LexerStep MakeOneOfProd(string[] regSteps) => MakeOneOfProd(null, null, regSteps);

    public LexerStep MakeSeqProd(string name, LexerSeqStepBase[] steps) {
      return MakeStep((ctx) => {
        int count = 0, qCount, nSteps = 0;

        foreach (LexerSeqStepBase step in steps) {
          if (step.Query(ctx, count, out qCount)) {
            count += qCount;
            nSteps++;
          }
          else {
            if (!step.IsOptional) {
              return false;
            }
          }
        }

        List<SymbolOld> symbols = new List<SymbolOld>(nSteps);

        ctx.BeginProduction(symbols);

        foreach (LexerSeqStepBase step in steps) {
          step.Exec(ctx);
        }

        ctx.EndProduction(symbols);

        ctx.AddSymbol(new Production(name, symbols));

        return true;
      }, (LexingContextOld ctx, int start, out int count) => {
        count = 0;
        int qCount;
        bool anyMatched = false; //Avoid infinite loops with all-optional sequences

        foreach (LexerSeqStepBase step in steps) {
          if (step.Query(ctx, start + count, out qCount)) {
            count += qCount;
            anyMatched = true;
          }
          else {
            if (!step.IsOptional) {
              count = 0;
              return false;
            }
          }
        }

        return anyMatched;
      });
    }

    public LexerStep MakeSeqProd(LexerSeqStepBase[] steps) => MakeSeqProd(null, steps);

    public void RegStep(string name, LexerStep step) => steps.Add(name, step);

    public void RegStep(string name, LexerStepExecutor executor, LexerStepQuerier querier) => RegStep(name, MakeStep(executor, querier));

    public void RegNamedStep(string name, NamedLexerStepExecutor executor, NamedLexerStepQuerier querier) =>
      steps.Add(name, new LexerStep(
        (ctx) => executor(name, ctx),
        (LexingContextOld ctx, int start, out int count) => querier(name, ctx, start, out count)));

    public void RegStringSym(string name, string pat) => RegStep(name, MakeStringSym(name, pat));

    public void RegOneOfProd(string name, LexerStep[] steps, string[] regSteps = null) => RegStep(name, MakeOneOfProd(name, steps, regSteps));

    public void RegOneOfProd(string name, string[] regSteps) => RegStep(name, MakeOneOfProd(name, null, regSteps));

    public void RegOneOrMoreProd(string name, string stepName) => RegStep(name, MakeOneOrMoreProd(name, stepName));

    public void RegSeqProd(string name, LexerSeqStepBase[] steps) => RegStep(name, MakeSeqProd(name, steps));
  }
}

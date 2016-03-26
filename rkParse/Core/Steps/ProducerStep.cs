using rkParse.Core.Staging;
using rkParse.Util;
using System;
using System.Text;

namespace rkParse.Core.Steps {
  public enum StepResult {
    Negative,
    Positive,
    AddRecursion,
  };

  public abstract class ProducerStep<TContext> where TContext : ProducerContext<TContext> {
    bool initialized = false;
    string name;

    public abstract bool IsRecursive { get; }

    public abstract bool CanBeTerminal { get; }

    public string Name => name;

    public ProducerStep(string name = null) {
      this.name = name;
    }

    protected abstract StepResult ExecuteInternal(TContext ctx);

    static int indent = 0;

    protected string _DebugIndent() {
      StringBuilder sb = new StringBuilder();

      for (int i = 0; i < indent; i++)
        sb.Append(" |");

      return sb.ToString();
    }

    protected string _DebugName() {
      return $"{GetType().Name} {(Name == null ? "(anonymous)" : Name.ToLiteral())}";
    }

    protected void _DebugPrintResult(StepResult result) {
      switch (result) {
        case StepResult.Negative:
          Console.ForegroundColor = ConsoleColor.Red;
          break;
        case StepResult.Positive:
          Console.ForegroundColor = ConsoleColor.Green;
          break;
        case StepResult.AddRecursion:
          Console.ForegroundColor = ConsoleColor.Yellow;
          break;
      }

      Console.Write(Enum.GetName(typeof(StepResult), result));
      Console.ResetColor();
    }

    protected virtual void Initialize(TContext ctx) {
      Console.WriteLine($"[ProducerStep]{_DebugIndent()} Initializing {_DebugName()}...");
    }

    public StepResult Execute(TContext ctx) {
      StepResult result;

      Console.Write($"[ProducerStep]{_DebugIndent()} ");
      Console.ForegroundColor = ConsoleColor.DarkGreen;
      Console.WriteLine($"Executing {_DebugName()}...");
      Console.ResetColor();

      ++indent;

      if (!initialized) {
        Initialize(ctx);
        initialized = true;

        Console.Write($"[ProducerStep]{_DebugIndent()} ");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"{_DebugName()} is{(IsRecursive ? "" : " not")} recursive.");
        Console.ResetColor();
      }

      if (IsRecursive) {
        if (ctx.SafeRecursing && !ctx.CanRecurse) {
          Console.Write($"[ProducerStep]{_DebugIndent()} ");
          Console.ForegroundColor = ConsoleColor.DarkYellow;
          Console.WriteLine("Recursion limit hit; skipping execution.");
          Console.ResetColor();

          result = StepResult.AddRecursion;
        }

        else if (ctx.BeginSafeRecursion()) {
          Console.WriteLine($"[ProducerStep]{_DebugIndent()} Beginning safe recursion...");

          BranchedStagingCache cache = ctx.BeginStagingBranched();
          StagingCache last = null;

          while (true) {
            StagingCache branch = cache.BeginSingleBranch();
            result = ExecuteInternal(ctx);

            if (result == StepResult.Positive) {
              if (last != null) cache.EndBranch(last);
              last = branch;
            }
            else {
              cache.EndBranch(branch);

              if (result == StepResult.Negative) break;
            }

            ctx.RecursionLimit++;

            Console.Write($"[ProducerStep]{_DebugIndent()} Got result of ");
            _DebugPrintResult(result);
            Console.WriteLine($"; incremented limit to {ctx.RecursionLimit}.");
          }

          if (last != null) result = StepResult.Positive;

          ctx.EndStaging(cache, last != null);
        }
        else {
          ctx.PushRecursion();

          result = ExecuteInternal(ctx);

          ctx.PopRecursion();
        }
      }
      else result = ExecuteInternal(ctx);

      Console.Write($"[ProducerStep]{_DebugIndent()} {_DebugName()} returned ");
      _DebugPrintResult(result);
      Console.WriteLine(".");

      --indent;

      return result;
    }
  }
}

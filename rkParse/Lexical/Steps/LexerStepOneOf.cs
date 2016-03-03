using System.Collections.Generic;

namespace rkParse.Lexical.Steps {
  public enum RecurseMode {
    DepthFirst,
    BreadthFirst,
  }

  public abstract class LexerStepOneOf : LexerStep {
    LexerStep[] steps;
    RecurseMode mode;

    public LexerStepOneOf(LexerStep[] steps, RecurseMode mode = RecurseMode.DepthFirst) {
      this.steps = steps;
      this.mode = mode;
    }

    protected abstract bool QueryTerminal(LexingContextOld ctx, out int count, int start = 0);

    bool GetPathBFS(LexingContextOld ctx, Stack<LexerStep> path, int depth, int start = 0) {
      foreach (LexerStep step in steps) {
        if (depth > 0 && step is LexerStepOneOf) {
          (step as LexerStepOneOf).GetPathBFS(ctx, path, depth - 1, start);
        }
      }

      return false;
    }
  }
}

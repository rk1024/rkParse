using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rkParse.Core.Staging {
  public class RecursionCache {
    int limit, depth = 0;

    public int Limit => limit;

    public bool CanRecurse => depth >= limit;

    public RecursionCache(int limit) {
      if (limit < 0) throw new ArgumentOutOfRangeException("limit", limit, "limit must be greater than or equal to zero.");
      this.limit = limit;
    }

    public bool PushRecursion() {
      if (!CanRecurse) throw new InvalidOperationException("Attempted to push recursion after limit was reached.");

      ++depth;

      return CanRecurse;
    }

    public void PopRecursion() {
      if (depth == 0) throw new InvalidOperationException("No recursions to pop.");

      --depth;
    }
  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rkParse.Core.Staging {
  public class RecursionCache {
    ICacheParent<RecursionCache> parent;
    int limit, depth = 0;

    public int Limit => limit;

    public bool CanRecurse => depth >= limit;

    public bool IsLocked => parent.IsCacheLocked(this);

    public RecursionCache(ICacheParent<RecursionCache> parent, int limit) {
      if (limit < 0) throw new ArgumentOutOfRangeException("limit", limit, "limit must be greater than or equal to zero.");

      this.parent = parent;
      this.limit = limit;
    }

    protected void AssertUnlocked() {
      if (IsLocked) throw new InvalidOperationException("Cannot modify a staging cache while it is locked.");
    }

    public bool PushRecursion() {
      AssertUnlocked();

      if (!CanRecurse) throw new InvalidOperationException("Attempted to push recursion after limit was reached.");

      ++depth;

      return CanRecurse;
    }

    public void PopRecursion() {
      AssertUnlocked();

      if (depth == 0) throw new InvalidOperationException("No recursions to pop.");

      --depth;
    }

    public int NextLimit() {
      AssertUnlocked();

      if (depth > 0) throw new InvalidOperationException("Cannot change recursion limit within a recursion.");

      return ++limit;
    }
  }
}

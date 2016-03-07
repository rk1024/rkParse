using rkParse.Core.Symbols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rkParse.Core.Staging {
  public abstract class StagingCacheBase {
    ICacheParent<StagingCacheBase> parent;
    int start;

    public abstract List<Symbol> Symbols { get; }

    public abstract int Consumed { get; }

    public int Start => start;

    public int End => Start + Consumed;

    public bool IsLocked => parent.IsCacheLocked(this);

    public StagingCacheBase(ICacheParent<StagingCacheBase> parent, int start) {
      this.parent = parent;
      this.start = start;
    }

    protected abstract void ConsumeInternal(int count);

    protected void AssertUnlocked() {
      if (IsLocked) throw new InvalidOperationException("Cannot modify a staging cache while it is locked.");
    }

    public void Consume(int count) {
      AssertUnlocked();
      ConsumeInternal(count);
    }
  }
}

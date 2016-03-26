using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using rkParse.Core.Symbols;

namespace rkParse.Core.Staging {
  public class BranchedStagingCache : StagingCacheBase, ICacheParent<StagingCacheBase> {
    List<StagingCacheBase> branches = new List<StagingCacheBase>();
    StagingCacheBase curr = null;

    public StagingCacheBase CurrentBranch {
      get {
        if (branches.Count == 0) return curr = null;

        return curr;
      }
      set {
        AssertUnlocked();

        if (value == null) { curr = value; return; }

        AssertHasBranch();

        if (!branches.Contains(value)) throw new InvalidOperationException("CurrentBranch set to value not in branch list.");
        curr = value;
      }
    }

    public override List<Symbol> Symbols {
      get {
        AssertHasBranch();
        return CurrentBranch.Symbols;
      }
    }

    public override int Consumed {
      get {
        AssertHasBranch();
        return CurrentBranch.Consumed;
      }
    }

    public BranchedStagingCache(ICacheParent<StagingCacheBase> parent, int start) : base(parent, start) { }

    protected override void ConsumeInternal(int count) {
      AssertHasBranch();
      CurrentBranch.Consume(count);
    }

    public StagingCache BeginSingleBranch(bool makeCurrent = true) {
      AssertUnlocked();

      StagingCache cache = new StagingCache(this, Start);

      branches.Add(cache);

      if (makeCurrent) CurrentBranch = cache;

      Console.WriteLine($"[BranchedStagingCache] Added single branch; list now contains {branches.Count} branch(es).");

      return cache;
    }

    public BranchedStagingCache BeginSubBranch(bool makeCurrent = true) {
      AssertUnlocked();

      BranchedStagingCache cache = new BranchedStagingCache(this, Start);

      branches.Add(cache);

      if (makeCurrent) CurrentBranch = cache;

      Console.WriteLine($"[BranchedStagingCache] Added sub-branch; list now contains {branches.Count} branch(es).");

      return cache;
    }

    public void EndBranch(StagingCacheBase branch, StagingCacheBase revertTo) {
      AssertUnlocked();
      AssertHasBranch();

      branches.Remove(branch);
      CurrentBranch = revertTo;
    }

    public void EndBranch(StagingCacheBase branch) {
      AssertUnlocked();
      AssertHasBranch();

      bool isCurrent = CurrentBranch == branch;

      if (!branches.Remove(branch)) throw new ArgumentOutOfRangeException("branch", branch, "branch must be a branch within this cache.");
      if (isCurrent) CurrentBranch = branches.Count == 0 ? null : branches[branches.Count - 1];

      Console.WriteLine($"[BranchedStagingCache] Removed branch; list now contains {branches.Count} branch(es).");
    }

    public bool IsCacheLocked(StagingCacheBase cache) {
      AssertHasBranch();
      return cache != CurrentBranch;
    }

    protected void AssertHasBranch() {
      if (branches.Count == 0) throw new InvalidOperationException("Cannot use BranchedStagingContext before branches are added.");
    }
  }
}

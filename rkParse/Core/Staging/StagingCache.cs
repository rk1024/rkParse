using rkParse.Core.Symbols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rkParse.Core.Staging {
  public class StagingCache : StagingCacheBase {
    List<Symbol> symbols = new List<Symbol>();
    int consumed = 0;

    public override List<Symbol> Symbols => symbols;

    public override int Consumed => consumed;

    public StagingCache(ICacheParent<StagingCacheBase> parent, int start) : base(parent, start) { }

    protected override void ConsumeInternal(int count) => consumed += count;
  }
}

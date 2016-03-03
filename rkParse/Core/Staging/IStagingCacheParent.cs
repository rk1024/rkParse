using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rkParse.Core.Staging {
  public interface IStagingCacheParent {
    bool IsCacheLocked(StagingCacheBase cache);
  }
}

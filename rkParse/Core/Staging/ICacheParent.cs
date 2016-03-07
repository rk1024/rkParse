using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rkParse.Core.Staging {
  public interface ICacheParent<T> {
    bool IsCacheLocked(T cache);
  }
}

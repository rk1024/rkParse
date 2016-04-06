using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rkParse.Util {
  public static class TypeExtensions {
    public static string GetNameWithoutArity(this Type type) {
      string name = type.Name;

      int idx = name.IndexOf('`');

      return idx < 0 ? name : name.Substring(0, idx);
    }
  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rkParse.Core.Symbols {
  public class Production : Symbol {
    Symbol[] children;

    public Production(string name, IEnumerable<Symbol> children) : base(name) {
      this.children = children.ToArray();
    }

    public Production(IEnumerable<Symbol> children) : this(null, children) { }

    public override string ToString() {
      return ToString(string.Join<Symbol>(", ", children), "{ ", " }");
    }
  }
}

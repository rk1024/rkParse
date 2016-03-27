using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rkParse.Core.Symbols {
  public class Production : Symbol {
    Symbol[] children;

    protected override string StringContents => string.Join(", ", from child in children
                                                                  select child.ToString());

    protected override string StringContentsCompact => string.Join(", ", from child in children
                                                                         select child.ToStringCompact());

    protected override string StringContentsMultiline => ("\n" + string.Join(",\n", from child in children
                                                                                    select child.ToStringMultiline())).Replace("\n", "\n  ") + "\n";

    protected override string StringOpen => "{ ";
    protected override string StringOpenMultiline => "{";
    protected override string StringClose => " }";
    protected override string StringCloseMultiline => "}";

    public Production(string name, IEnumerable<Symbol> children) : base(name) {
      this.children = children.ToArray();
    }

    public Production(IEnumerable<Symbol> children) : this(null, children) { }
  }
}

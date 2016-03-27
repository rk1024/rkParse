using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rkParse.Core.Symbols {
  public abstract class OverflowSymbol : Symbol {
    public OverflowSymbol(string name = null) : base(name) { }
  }

  public class OverflowSymbol<TInput> : OverflowSymbol {
    TInput input;

    protected override string StringContents => input.ToString();

    public TInput Input => input;

    public OverflowSymbol(string name, TInput input) : base(name) {
      this.input = input;
    }

    public OverflowSymbol(TInput input) : this(null, input) { }
  }
}

using System;

namespace rkParse.Core.Symbols {
  public class Symbol {
    string name = null;

    public string Name => name;

    protected virtual string StringContents => "";
    protected virtual string StringContentsCompact => StringContents;
    protected virtual string StringContentsMultiline => StringContents;
    protected virtual string StringOpen => "<";
    protected virtual string StringOpenMultiline => StringOpen;
    protected virtual string StringClose => ">";
    protected virtual string StringCloseMultiline => StringClose;

    public Symbol(string name = null) {
      this.name = name;
    }

    string ToStringCompact(string contents, string open, string close) {
      return $"{open}{contents}{close}";
    }

    string ToStringNamed(string contents) {
      return $"{name}({contents})";
    }

    string ToString(string contents, string open, string close) {
      return name == null ? ToStringCompact(contents, open, close) : ToStringNamed(contents);
    }

    public sealed override string ToString() {
      return ToString(StringContents, StringOpen, StringClose);
    }

    public string ToStringCompact() {
      return ToStringCompact(StringContentsCompact, StringOpen, StringClose);
    }

    public string ToStringMultiline() {
      return ToString(StringContentsMultiline, StringOpenMultiline, StringCloseMultiline);
    }
  }
}

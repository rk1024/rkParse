using rkParse.Core.Steps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rkParse.Lexical.Steps {
  public abstract class LexerStep : ProducerStep<LexerContext> {
    public LexerStep(string name = null) : base(name) { }
  }
}

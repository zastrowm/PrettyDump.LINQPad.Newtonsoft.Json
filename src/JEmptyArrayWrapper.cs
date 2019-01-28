using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace PrettyDump.LINQPad.Newtonsoft.Json
{
  public class JEmptyArrayDumper : JArrayDumper
  {
    public JEmptyArrayDumper(BaseJsonDumper parent, JArray instance, int depth, DumpOptions options)
      : base(parent, instance, depth, options)
    {
    }
  
    public override bool ShouldExpand
      => true;
    
    public override bool IsComplex
      => false;

    public override string GetName()
    {
      return "[ ] (empty)";
    }
  }
}
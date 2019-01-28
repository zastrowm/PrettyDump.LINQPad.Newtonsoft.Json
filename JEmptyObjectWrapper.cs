using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace PrettyDump.LINQPad.Newtonsoft.Json
{
  public class JEmptyObjectDumper : JObjectDumper
  {
    public JEmptyObjectDumper(BaseJsonDumper parent, JObject instance, int depth, DumpOptions options)
      : base(parent, instance, depth, options)
    {
    }
  
    public override string GetName()
      => "{ } (empty)";
    
    public override bool ShouldExpand
      => true;
    
    public override bool IsComplex
      => false;
  }
}
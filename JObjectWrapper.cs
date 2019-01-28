using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace PrettyDump.LINQPad.Newtonsoft.Json
{
  public class JObjectDumper : BaseJsonDumper<JObject>
  {
    public JObjectDumper(BaseJsonDumper parent, JObject instance, int depth, DumpOptions options)
      : base(parent, instance, depth, options)
    {
    }

    public override bool IsComplex
      => true;

    public override bool ShouldExpand
    {
      get
      {
        if (Options.ShouldAutoExpandAll)
          return true;

        if (Parent?.WasLazy == true && JsonValue.Count <= 10)
        {
            return true;
        }
        else if (Parent?.Parent?.WasLazy == true 
                 && Parent?.Parent?.JsonToken is JObject parent 
                 && parent.Count <= 10 && JsonValue.Count <= 3)
        {
          return true;
        }
        else if (Parent is JArrayDumper array)
        {
          if (!array.ShouldExpand
              && array.ShouldAutoExpandChildren
              // special case; array.ShouldExpand may be false at the first depth, but it will be auto-expanded
              // since it's LINQPad controlled, not us controlled
              && array.Depth > 1)
          {
            return true;
          }
        }

        return base.ShouldExpand;
      }
    }
    
    public override string GetName()
    {
      return "{ ... } ";
    }
  
    public override object ToDump()
    {
      var properties = JsonValue.Properties();
    
      if (Options.PropertyFilterCallback != null)
      {
        properties = Options.PropertyFilterCallback.Invoke(properties);
      }
    
      var kvp = properties.Select(p => (p.Name, p.Value));
    
      if (Options.ShouldSortProperties)
      {
        kvp = kvp.OrderBy(k => k.Name);
      }
    
      return WrapKeyValuePair(kvp);
    }
  }
}
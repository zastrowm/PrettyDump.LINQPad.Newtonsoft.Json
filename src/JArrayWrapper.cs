using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace PrettyDump.LINQPad.Newtonsoft.Json
{
  public class JArrayDumper : BaseJsonDumper<JArray>
  {
    public JArrayDumper(BaseJsonDumper parent, JArray instance, int depth, DumpOptions options)
      : base(parent, instance, depth, options)
    {
    }

    /// <inheritdoc />
    public override bool ShouldExpand
    {
      get
      {
        if (Options.ShouldAutoExpandAll)
          return true;
        
        return JsonValue.Count <= 1
               || (JsonValue.Count <= Options.ShortArrayLength && JsonValue.All(it => it is JValue))
               || base.ShouldExpand;
      }
    }
  
    /// <summary>
    ///   True if children should automatically expand themselves when rendering.
    /// </summary>
    public bool ShouldAutoExpandChildren
      => JsonValue.Count <= Options.ShortArrayLength;

    /// <inheritdoc />
    public override bool IsComplex
      => true;

    /// <inheritdoc />
    public override string GetName()
    {
      return $"[ ... ] ({JsonValue.Count})";
    }

    /// <inheritdoc />
    public override object ToDump()
    {
      var kvps = JsonValue.Select((value, index) => (index.ToString(), value));
    
      if (Options.ArrayLimitLength > 0)
      {
        kvps = kvps.Take(Options.ArrayLimitLength);
      }
    
      return WrapKeyValuePair(kvps);
    }
  }
}
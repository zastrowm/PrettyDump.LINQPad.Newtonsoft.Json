using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace PrettyDump.LINQPad.Newtonsoft.Json
{
  public class JTokenDumper : BaseJsonDumper<JToken>
  {
    public JTokenDumper(BaseJsonDumper parent, JToken instance, int depth, DumpOptions options)
      : base(parent, instance, depth, options)
    {
   
    }

    public override bool IsComplex
      => false;

    public override string GetName()
      => "Value";

    public override object ToDump()
    {
      if (JsonValue is JValue rawValue)
      {
        if (Options.ValueConverterCallback != null)
        {
          return Options.ValueConverterCallback.Invoke(rawValue);
        }

        // if (Options.ShouldEmphasizeStrings && rawValue.Value is string str)
        // {
        //   return Util.RawHtml(new XElement("em", str));
        // }

        return rawValue.Value;
      }
    
      return JsonValue;
    }

    public override string ToString()
      => JsonValue.ToString();
  }
}
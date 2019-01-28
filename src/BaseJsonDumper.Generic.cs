using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using LINQPad;
using Newtonsoft.Json.Linq;

namespace PrettyDump.LINQPad.Newtonsoft.Json
{
  public abstract class BaseJsonDumper<T> : BaseJsonDumper
    where T : JToken
  {
    public BaseJsonDumper(BaseJsonDumper parent, T jsonValue, int depth, DumpOptions options)
      : base(parent, depth, options)
    {
      JsonValue = jsonValue;
    }

    public T JsonValue { get; }

    public override JToken JsonToken
      => JsonValue;

    public override string CollapsedName
      => GetName();

    public abstract string GetName();
  }
}
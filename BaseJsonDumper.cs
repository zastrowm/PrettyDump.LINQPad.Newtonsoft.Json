using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using JetBrains.Annotations;
using LINQPad;
using Newtonsoft.Json.Linq;

namespace PrettyDump.LINQPad.Newtonsoft.Json
{
  /// <summary>
  ///   Base class for all wrappers of JSON data types.
  ///
  ///   Each derived type should specialize wrapping a specific derived type of <see cref="JToken"/> and should
  ///   provide a pretty LINQPad representation of <see cref="ToDump"/>
  /// </summary>
  public abstract class BaseJsonDumper
  {
    protected BaseJsonDumper(BaseJsonDumper parent, int depth, DumpOptions options)
    {
      Parent = parent;
      Depth = depth;
      Options = options;
    }

    /// <summary>
    ///   The wrapper class that created this instance of the wrapper.
    /// </summary>
    [CanBeNull]
    public BaseJsonDumper Parent { get; } 

    /// <summary>
    ///   The number of levels deep that this wrapper exists in the hierarchy.
    /// </summary>
    public int Depth { get; }
    
    /// <summary>
    ///   The options to use when rendering the Dump() contents.
    /// </summary>
    public DumpOptions Options { get; }
    
    /// <summary>
    ///   The raw JToken that this wrapper wraps for pretty dumping.
    /// </summary>
    public abstract JToken JsonToken { get; }
  
    /// <summary>
    ///   True if the wrapper wraps a type which contains child objects or values (i.e. array or object).
    /// </summary>
    public abstract bool IsComplex { get; }
  
    /// <summary>
    ///    Method used by LINQPad when dumping the contents of JSON.
    /// </summary>
    /// <returns> An Object that LINQPad can use to prettily show the contents of <see cref="JsonToken"/> </returns>
    public abstract object ToDump();
  
    /// <summary>
    ///   The simple name of the object when it has not yet been expanded.
    /// </summary>
    public abstract string CollapsedName { get; }

    /// <summary>
    ///   True if the object should be automatically expanded when dumping it in LINQPad.
    /// </summary>
    /// <remarks>
    ///   For example, an array of complex objects should return false here, as otherwise LINQPad may lag when trying
    ///   to render all of the objects.
    /// </remarks>
    public virtual bool ShouldExpand
      => Depth <= Options.AutoExpandDepth || !IsComplex;
  
    /// <summary>
    ///   True if the object was lazily-expanded.
    /// </summary>
    public bool WasLazy { get; internal set; }

    internal static BaseJsonDumper GetWrapperFor(BaseJsonDumper parent, JToken instance, int depth, DumpOptions options)
    {
      switch (instance)
      {
        case JObject jObject when jObject.Count == 0:
          return new JEmptyObjectDumper(parent, jObject, depth, options);
        case JObject jObject:
          return new JObjectDumper(parent, jObject, depth, options);
        case JArray jArray when jArray.Count == 0:
          return new JEmptyArrayDumper(parent, jArray, depth, options);
        case JArray jArray:
          return new JArrayDumper(parent, jArray, depth, options);
        case JValue jValue:
          return new JTokenDumper(parent, jValue, depth, options);
        default:
          return new JTokenDumper(parent, instance, depth, options);
      }
    }

    protected BaseJsonDumper Wrap(JToken instance)
    {
      return GetWrapperFor(this, instance, Depth + 1, Options);
    }

    protected object WrapKeyValuePair(IEnumerable<(string key, JToken value)> values)
    {
      IDictionary<string, object> expando = new ExpandoObject();

      if (Depth > 1 && Options.IncludePath)
      {
        expando["🌴 path "] = JsonToken.Path;
      }
      
      foreach (var kvp in values)
      {
        var unwrapped = Wrap(kvp.value);

        object wrappedMaybe;

        bool shouldExpand = Options.ShouldExpandCallback?.Invoke(unwrapped)
                            ?? unwrapped.ShouldExpand;
        
        if (!shouldExpand)
        {
          unwrapped.WasLazy = true;
          wrappedMaybe = (object)Util.OnDemand(unwrapped.CollapsedName, () => unwrapped);
        }
        else
        {
          wrappedMaybe = (object)unwrapped;
        }

        expando[kvp.key?.ToString() ?? ""] = wrappedMaybe;
      }

      return expando;
    }
  }
}
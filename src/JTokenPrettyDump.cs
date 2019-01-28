using System;
using System.Collections.Generic;
using System.Linq;
using LINQPad;
using Newtonsoft.Json.Linq;
using PrettyDump.LINQPad.Newtonsoft.Json;

// ReSharper disable once CheckNamespace
namespace Newtonsoft.Json.Linq
{
  /// <summary>
  ///   Contains the extension methods for more prettily-dumping classes derived from <see cref="JToken"/>
  /// </summary>
  public static class JTokenPrettyDump
  {
    /// <summary>
    ///   In LINQPad, Dump the contents of the given <paramref name="token"/>, using a custom object that provides a
    ///   better representation in the Results window.
    /// </summary>
    /// <param name="token"> The token whose contents should be dumped. </param>
    /// <param name="minDepth"> The minimum depth to which the the objects should be auto-expanded.
    ///   See <see cref="DumpOptions.AutoExpandDepth"/> for more details. </param>
    /// <typeparam name="T"> The actual JSON type of of the value to dump. </typeparam>
    /// <returns> The original instance that was dumped. </returns>
    public static T PrettyDump<T>(this T token, int? minDepth = null)
      where T : JToken
    {
      var options = new DumpOptions(token);
      if (minDepth is int nonNullDepth)
      {
        options.AutoExpandDepth = nonNullDepth;
      }
      
      GetDumperFor(token).Dump();
      return token;
    }

    /// <summary>
    ///   In LINQPad, Dump the contents of the given <paramref name="token"/>, using a custom object that provides a
    ///   better representation in the Results window.
    /// </summary>
    /// <param name="token"> The token whose contents should be dumped. </param>
    /// <param name="options"> The options that should be used when rendering the results in the Results window. </param>
    /// <typeparam name="T"> The actual JSON type of of the value to dump. </typeparam>
    /// <returns> The original instance that was dumped. </returns>
    public static T PrettyDump<T>(this T token, DumpOptions options)
      where T : JToken
    {
      if (options == null)
        throw new ArgumentNullException(nameof(options));
      
      GetDumperFor(token, options).Dump();
      return token;
    }

    /// <summary>
    ///   Gets an object that wraps the given <see cref="instance"/> in an object, that when .Dump()d in LINQPad,
    ///   will provide a better rendering than the raw output that LINQPad provides.
    /// </summary>
    public static BaseJsonDumper GetDumperFor(this JToken instance, DumpOptions options = null)
      => BaseJsonDumper.GetWrapperFor(null, instance, 1, options ?? new DumpOptions(instance));
  }
}
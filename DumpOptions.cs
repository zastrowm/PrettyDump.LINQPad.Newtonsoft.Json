using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using PrettyDump.LINQPad.Newtonsoft.Json;

// ReSharper disable once CheckNamespace
namespace Newtonsoft.Json.Linq
{
  /// <summary>
  ///   Options that configure how <see cref="JTokenDumper.PrettyDump{T}(T)"/> work.
  /// </summary>
  public class DumpOptions
  {
    /// <summary>
    ///   Default constructor.
    /// </summary>
    public DumpOptions()
    {
      
    }
    
    /// <summary>
    ///   Copy constructor.
    /// </summary>
    /// <param name="options"> The options to clone. </param>
    public DumpOptions(DumpOptions options)
    {
      IncludePath = options.IncludePath;
      AutoExpandDepth = options.AutoExpandDepth;
      ArrayLimitLength = options.ArrayLimitLength;
      ShortArrayLength = options.ShortArrayLength;
      ShouldExpandCallback = options.ShouldExpandCallback;
      ShouldSortProperties = options.ShouldSortProperties;
      PropertyFilterCallback = options.PropertyFilterCallback;
      ShouldAutoExpandAll = options.ShouldAutoExpandAll;
      ValueConverterCallback = options.ValueConverterCallback;
    }
    
    /// <summary>
    ///   Create a new options based on the default, but setting <see cref="ShouldAutoExpandAll"/> to true if the object
    ///   is small enough.
    ///
    ///   Used by default in <see cref="JTokenPrettyDump.PrettyDump{T}(T,System.Nullable{int})"/>.
    /// </summary>
    /// <param name="root"> The object that determines if <see cref="ShouldAutoExpandAll"/>. </param>
    public DumpOptions(JToken root)
      : this(DefaultOptions)
    {
      ShouldAutoExpandAll = ContainsFewerThanNEntities(root, 200);
    }
    
    /// <summary>
    ///   The default options to use when <see cref="JTokenPrettyDump.PrettyDump{T}(T,System.Nullable{int})"/> is
    ///   provided no options.
    /// </summary>
    public static readonly DumpOptions DefaultOptions 
      = new DumpOptions();

    /// <summary>
    /// Gets the number of simple JToken descendents within the given value.
    /// </summary>
    /// <param name="instance"> The instance whose child descendents should be counted. </param>
    /// <param name="largeNumber">  </param>
    /// <returns> True if it contains less than <see cref="largeNumber"/>, false if it contains greater
    ///  than or equal to <see cref="largeNumber"/>. </returns>
    /// <remarks>
    ///  Entities are counted by counting values and properties.  For instance, an array with 5 elements would have
    ///  6 entities (the array and the 5 children).  An object with 4 properties would have 9 entities (1 for the object
    ///  itself, and 2 for each property and value).
    /// </remarks>
    internal static bool ContainsFewerThanNEntities(JToken instance, int largeNumber)
    {
      int countOfStructure = (instance as JContainer)?.DescendantsAndSelf().Take(largeNumber).Count()
                             ?? 1;

      return countOfStructure < largeNumber;
    }

    /// <summary>
    ///  True to include the <see cref="Newtonsoft.Json.Linq.JsonPath"/> in the output of objects and arrays.
    /// </summary>
    public bool IncludePath { get; set; }
      = false;
    
    /// <summary>
    ///   True if everything should just automatically expand, regardless of all other settings.
    /// </summary>
    public bool ShouldAutoExpandAll { get; }
      = false;

    /// <summary>
    ///   True if object properties should be sorted as they're rendered.  False if they should be rendered in the order
    ///   that they existed in the JSON.
    /// 
    ///   True by default. 
    /// </summary>
    public bool ShouldSortProperties { get; set; }
      = true;

    /// <summary>
    ///   The number of levels that should be auto-expanded by default.
    /// </summary>
    /// <remarks>
    ///   Note that this represents the minimum auto-expanded depth, and may actually expand items greater than this,
    ///   for instance, for objects/array that are small.
    ///
    ///   For absolute control over what is expanded, see <see cref="ShouldExpandCallback"/>. 
    /// </remarks>
    public int AutoExpandDepth { get; set; }
      = 1;

    /// <summary>
    ///  The maximum number of array elements to show.  Only use this if you want to show a subset of the array
    ///  elements.
    /// </summary>
    public int ArrayLimitLength { get; set; }
      = 0;

    /// <summary>
    ///   The number of elements an array can have that indicates it is "short" and should be auto-expanded.  This only
    ///   affects arrays that are made up of non-object/non-array elements.
    ///
    ///   This only takes effect up to <see cref="AutoExpandDepth"/>
    /// </summary>
    public int ShortArrayLength { get; set; }
      = 20;

    /// <summary>
    ///   Callback to use when printing <see cref="JValue"/> (non-<see cref="JArray"/> and <see cref="JObject"/> values)
    ///   in order to get the value to dump to the output window.
    ///
    ///   Useful in order to convert values into a more useful representation (for instance, by making urls clickable).
    /// </summary>
    public ValueConverterCallback ValueConverterCallback { get; set; }
      = null;

    /// <summary>
    ///   Callback to determine whether or not a property should be expanded. If a non null value is returned, it
    ///   overrides all built-int mechanisms that determine whether or not an item should be expanded.
    /// </summary>
    public ShouldExpandCallback ShouldExpandCallback { get; set; }
      = null;

    /// <summary>
    ///   Callback that determines what properties are included when listing an object.
    /// </summary>
    /// <seealso cref="PropertyFilterCallback"/>
    /// <seealso cref="ShouldSortProperties"/>
    public PropertyFilterCallback PropertyFilterCallback { get; set; }
      = null;
  }

  /// <summary>
  ///   Delegate that determines whether or not the given value should be expanded.
  /// </summary>
  /// <param name="wrappedValue"> The wrapper for the value whose expansion value should be determined. </param>
  /// <returns> True if it should be expanded, false if it should not, null if it should use the default behavior
  /// based on other options and the value returned by <see cref="BaseJsonDumper.ShouldExpand"/>. </returns>
  public delegate bool? ShouldExpandCallback(BaseJsonDumper wrappedValue);

  /// <summary>
  ///   Delegate that determines which properties on an instance of a <see cref="JObject"/> should be Dumped()
  /// </summary>
  /// <param name="properties"> The default properties that would be shown. </param>
  /// <returns> A filtered list of properties to Dump() for the object. </returns>
  public delegate IEnumerable<JProperty> PropertyFilterCallback(IEnumerable<JProperty> properties);

  /// <summary>
  ///   Delegate that allows converting non-<see cref="JArray"/> and non-<see cref="JObject"/> <see cref="JValue"/>s
  ///   into a different object prior to dumping.
  ///
  ///   Can be used to convert values into better representations like a more strongly typed data, or, for marking
  ///   up the value for better presentation (like making urls clickable). 
  /// </summary>
  public delegate object ValueConverterCallback(JValue value);
}
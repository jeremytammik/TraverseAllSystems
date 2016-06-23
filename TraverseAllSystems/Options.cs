namespace TraverseAllSystems
{
  class Options
  {
    /// <summary>
    /// Store element id or UniqueId in JSON output?
    /// </summary>
    public static bool StoreUniqueId = false;
    public static bool StoreElementId = !StoreUniqueId;

    /// <summary>
    /// Store parent node id in child, or recursive 
    /// tree of children in parent?
    /// </summary>
    public static bool StoreJsonGraphBottomUp = false;
    public static bool StoreJsonGraphTopDown 
      = !StoreJsonGraphBottomUp;
  }
}

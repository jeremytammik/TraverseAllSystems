namespace TraverseAllSystems
{
  class Options
  {
    /// <summary>
    /// Store element id or UniqueId in JSON output?
    /// </summary>
    public const bool StoreUniqueId = false;
    public const bool StoreElementId = !StoreUniqueId;

    /// <summary>
    /// Store parent node id in child, or recursive 
    /// tree of children in parent?
    /// </summary>
    public const bool StoreParentInChildNode = false;
    public const bool StoreChildrenInParentNode 
      = !StoreParentInChildNode;
  }
}

namespace TraverseAllSystems
{
    class Options
    {
        /// <summary>
        /// Store element id or UniqueId in JSON output?
        /// </summary>
        public static bool StoreUniqueId = true;
        public static bool StoreElementId = !StoreUniqueId;

        /// <summary>
        /// Store parent node id in child, or recursive 
        /// tree of children in parent?
        /// </summary>
        public static bool StoreJsonGraphBottomUp = false;
        public static bool StoreJsonGraphTopDown
          = !StoreJsonGraphBottomUp;

        public const string USC_FORGE_JSON_TREE_FORMAT = "{\"id\" : \"{0}\" , \"name\" : \"{1}\", \"udids\" : [ {2} ], \"children\" : [ ]}";
    }
}

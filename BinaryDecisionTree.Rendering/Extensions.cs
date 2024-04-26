namespace BinaryDecisionTree.Rendering
{
    using BinaryDecisionTree.Structure;

    /// <summary>
    /// Common extension methods
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Label for the specified <see cref="VertexType"/>.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The label.</returns>
        public static string Label(this VertexType type)
        {
            switch (type)
            {
                case VertexType.False:
                    return "F";
                case VertexType.True:
                    return "T";
                default:
                    return string.Empty;
            }
        }
    }
}

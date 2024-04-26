namespace BinaryDecisionTree.Structure
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// BDT Node
    /// </summary>
    [DebuggerDisplay("{Value}")]
    public class Node
    {
        public Node(string value = null)
        {
            this.Id = Guid.NewGuid();
            this.Value = value;
        }

        /// <summary>
        /// Gets the unique identifier.
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            var other = obj as Node;
            return other != null && other.GetHashCode() == this.GetHashCode();
        }
    }
}

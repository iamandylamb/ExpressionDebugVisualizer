namespace BinaryDecisionTree.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Entity
    {
        public string Name { get; set; }

        public int Count { get; set; }

        public int Length { get; set; }

        public bool Flag { get; set; }

        public bool? NullableFlag { get; set; }

        public Entity Nest { get; set; }

        public DayOfWeek Day { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public int Z { get; set; }
    }
}

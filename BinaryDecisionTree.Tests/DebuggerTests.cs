namespace BinaryDecisionTree.Tests
{
    using BinaryDecisionTree.Debugger;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text;
    using System.Threading.Tasks;

    [TestClass, Ignore]
    public class DebuggerTests
    {
        [TestMethod]
        public void Test1()
        {
            Expression<Func<Entity, bool>> target = e => e.Name == "A" && (e.Count == 3 || e.Count == 6);
            
            BinaryDecisionTreeExpressionVisualiser.TestShowVisualizer(target);
        }

        [TestMethod]
        public void Test2()
        {
            Expression<Func<Entity, DayOfWeek>> target = e => e.Name == "A" || e.Name == "B"
                ? e.Day : DayOfWeek.Monday;

            BinaryDecisionTreeExpressionVisualiser.TestShowVisualizer(target);
        }
        
        [TestMethod]
        public void Test3()
        {
            Expression<Func<Entity, DayOfWeek>> target = e => !(e.Name == "A" || e.Name == "B")
                ? e.Day : e.NullableFlag.Value ? DayOfWeek.Monday : DayOfWeek.Thursday;

            BinaryDecisionTreeExpressionVisualiser.TestShowVisualizer(target);
        }

        [TestMethod]
        public void Test4()
        {
            Expression<Func<Entity, bool>> target = e => (e.Count > e.Length ? e.Count : e.Length) > 10;

            BinaryDecisionTreeExpressionVisualiser.TestShowVisualizer(target);
        }

        [TestMethod]
        public void Test5()
        {
            Expression<Func<Entity, bool>> target = e => e.Count > 10;

            BinaryDecisionTreeExpressionVisualiser.TestShowVisualizer(target);
        }
    }
}

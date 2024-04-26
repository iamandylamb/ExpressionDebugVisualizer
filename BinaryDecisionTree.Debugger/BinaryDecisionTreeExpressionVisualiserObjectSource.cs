namespace BinaryDecisionTree.Debugger
{
    using BinaryDecisionTree.Rendering;
    using BinaryDecisionTree.Visitors;
    using Microsoft.VisualStudio.DebuggerVisualizers;
    using System.IO;
    using System.Linq.Expressions;
    using System.Xml.Linq;

    /// <summary>
    /// Serialises an Expression to a DGML document.
    /// </summary>
    public class BinaryDecisionTreeExpressionVisualiserObjectSource : VisualizerObjectSource
    {
        public static XDocument GetValue(IVisualizerObjectProvider provider)
        {
            var xml = XDocument.Parse(new StreamReader(provider.GetData()).ReadToEnd());
            return xml;
        }

        public override void GetData(object target, Stream outgoingData)
        {
            var writer = new StreamWriter(outgoingData);

            var expression = target as Expression;

            var bdt = BinaryDecisionTreeExpressionVisitor.Build(expression);
            var dgml = bdt.ToDgml();

            writer.Write(dgml.ToString());
            writer.Flush();
        }
    }
}

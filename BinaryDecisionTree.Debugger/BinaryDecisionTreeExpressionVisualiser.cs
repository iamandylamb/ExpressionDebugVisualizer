[assembly: System.Diagnostics.DebuggerVisualizer(
    typeof(BinaryDecisionTree.Debugger.BinaryDecisionTreeExpressionVisualiser),
    typeof(BinaryDecisionTree.Debugger.BinaryDecisionTreeExpressionVisualiserObjectSource),
    Target = typeof(System.Linq.Expressions.Expression<>),
    Description = BinaryDecisionTree.Debugger.BinaryDecisionTreeExpressionVisualiser.VisualiserName)]

namespace BinaryDecisionTree.Debugger
{
    using Microsoft.VisualStudio.DebuggerVisualizers;
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using BinaryDecisionTree.VisualStudio;

    /// <summary>
    /// A Visualizer for <see cref="Expression{T}"/>.  
    /// </summary>
    public class BinaryDecisionTreeExpressionVisualiser : DialogDebuggerVisualizer
    {
        internal const string VisualiserName = "Expression Debug Visualiser";
        private static string FileName = string.Format("{0}.dgml", VisualiserName);

        protected override void Show(IDialogVisualizerService windowService, IVisualizerObjectProvider objectProvider)
        {
            if (objectProvider == null)
            {
                throw new ArgumentNullException("objectProvider");
            }

            var dgml = BinaryDecisionTreeExpressionVisualiserObjectSource.GetValue(objectProvider);

            var path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var filePath = Path.Combine(path, FileName);

            Directory.CreateDirectory(path);
            dgml.Save(filePath);

            var dte = DteHelper.GetCurrent();

            if (dte != null)
            {
                // Open file in the instance of VS running the visualizer.
                dte.ItemOperations.OpenFile(filePath);
            }
            else
            {
                // Open file in the default instance of VS.
                Process.Start(filePath);
            }

            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(TimeSpan.FromSeconds(10));
                Directory.Delete(path, true);
            });
        }

        /// <summary>
        /// Tests the visualizer by hosting it outside of the debugger.
        /// </summary>
        /// <param name="objectToVisualize">The object to display in the visualizer.</param>
        public static void TestShowVisualizer(object objectToVisualize)
        {
            VisualizerDevelopmentHost visualizerHost = new VisualizerDevelopmentHost(objectToVisualize, 
                typeof(BinaryDecisionTreeExpressionVisualiser), 
                typeof(BinaryDecisionTreeExpressionVisualiserObjectSource));

            visualizerHost.ShowVisualizer();
        }
    }
}

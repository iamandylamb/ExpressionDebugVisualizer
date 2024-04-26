namespace BinaryDecisionTree.VisualStudio
{
    using EnvDTE;
    using System;
    using System.Runtime.InteropServices;
    using System.Runtime.InteropServices.ComTypes;
    using System.Text.RegularExpressions;
    using Process = System.Diagnostics.Process;

    public static class DteHelper
    {
        [DllImport("ole32.dll")]
        private static extern void CreateBindCtx(int reserved, out IBindCtx ppbc);

        [DllImport("ole32.dll")]
        private static extern void GetRunningObjectTable(int reserved,
                                                         out IRunningObjectTable prot);
        
        /// <summary>
        /// Get the DTE object for the currently running VS instance.
        /// </summary>
        /// <returns></returns>
        public static DTE GetCurrent()
        {
            // Runtime object table regex for VS running under current process.
            var rotRegex = GetDteRunningObjectTableRegex();

            IRunningObjectTable rot;
            GetRunningObjectTable(0, out rot);

            IEnumMoniker enumMoniker;
            rot.EnumRunning(out enumMoniker);
            enumMoniker.Reset();

            IntPtr fetched = IntPtr.Zero;
            IMoniker[] moniker = new IMoniker[1];

            while (enumMoniker.Next(1, moniker, fetched) == 0)
            {
                IBindCtx bindCtx;
                CreateBindCtx(0, out bindCtx);

                string displayName;
                moniker[0].GetDisplayName(bindCtx, null, out displayName);

                if (rotRegex.IsMatch(displayName))
                {
                    object comObject;
                    rot.GetObject(moniker[0], out comObject);
                    return comObject as DTE;
                }
            }

            return null; // Not found.
        }

        private static Regex GetDteRunningObjectTableRegex()
        {
            return new Regex(string.Format(@"!VisualStudio\.DTE\..*:{0}", Process.GetCurrentProcess().Id));
        }
    }
}

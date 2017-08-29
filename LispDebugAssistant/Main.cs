using System.Threading;
using Autodesk.AutoCAD.Runtime;
using Nucs.Alda.Forms;
using Nucs.Alda.Lsp;

namespace Nucs.Alda {
    public class Main : IExtensionApplication {
        public static MainForm MainForm { get; internal set; }
        /// <summary>
        ///     Signals when the mainform finishes loading.
        /// </summary>
        internal static ManualResetEventSlim _signal { get; set; } = new ManualResetEventSlim();

        public void Initialize() {
            if (MainForm.Config.AutoLaunch)
                MainCommands.InitiateCommand();
        }

        public void Terminate() {
            MainForm.Dispose();
            MainForm = null;
        }

        public static bool WaitForMainForm(int ms = -1) {
            return _signal.Wait(ms);
        }
    }
}
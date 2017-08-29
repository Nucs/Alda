using System.Media;
using System.Threading;
using System.Windows.Forms;
using Autodesk.AutoCAD.Runtime;
using Nucs.Alda.Forms;

namespace Nucs.Alda.Lsp {
    public static class MainCommands {
        /// <summary>
        ///     Starts a main form app.
        /// </summary>
        [CommandMethod("lspdbg")]
        public static void InitiateCommand() {
            var t = new Thread(() => {
                Mutex mutex = new System.Threading.Mutex(false, "lspdbg_instance");
                try {
                    if (mutex.WaitOne(0, false) || MessageBox.Show("An instance of Lisp Debugging Assistant is already running.\nMake sure to check on the tray icons.\nWould you still like to open a new instance?", "Instance Already Exists", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes) {
                        SystemSounds.Asterisk.Play();
                        Application.Run(Main.MainForm = new MainForm());
                    }
                } finally {
                    mutex?.Close();
                }
            });
            t.SetApartmentState(ApartmentState.STA);
            t.IsBackground = true;
            t.Start();
        }
    }
}
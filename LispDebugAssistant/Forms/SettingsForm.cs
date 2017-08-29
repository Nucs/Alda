using System;
using System.Windows.Forms;
using Nucs.Alda.Settings;

namespace Nucs.Alda.Forms {
    public partial class SettingsForm : Form {
        private AppConfig Cfg { get; }

        public SettingsForm(AppConfig cfg) {
            Cfg = cfg;
            InitializeComponent();
            chkAutoLaunch.Checked = cfg.AutoLaunch;
            chkAutostart.Checked = cfg.AutoStart;
            chkStartMinimized.Checked = cfg.StartMinimized;
            chkReloadOnStartup.Checked = cfg.LoadAllOnStartup;
            chkLoadOnTurningOn.Checked = cfg.LoadAllOnTurnOn;
        }

        private void btnSave_Click(object sender, EventArgs e) {
            Cfg.AutoLaunch = chkAutoLaunch.Checked;
            Cfg.AutoStart = chkAutostart.Checked;
            Cfg.StartMinimized = chkStartMinimized.Checked;
            Cfg.LoadAllOnStartup = chkReloadOnStartup.Checked;
            Cfg.LoadAllOnTurnOn = chkLoadOnTurningOn.Checked;
            Cfg.Save();
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void SettingsForm_Load(object sender, EventArgs e) {
            
        }
    }
}
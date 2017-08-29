﻿using System.IO;
using autonet.Common.Settings;
using Common;

namespace Nucs.Alda.Settings {
    public class AppConfig : JsonConfiguration {
        public override string FileName { get; } = Path.Combine(Paths.ConfigDirectory.FullName, "lspdbg.config.json");

        /// <summary>
        ///     current folder listening to
        /// </summary>
        public string[] Files { get; set; } = new string[0];

        /// <summary>
        ///     True: lspdbg will launch with autodesk.
        /// </summary>
        public bool AutoLaunch { get; set; } = false;

        /// <summary>
        ///     Watcher will be turned on automatically.
        /// </summary>
        public bool AutoStart { get; set; } = false;

        /// <summary>
        ///     The program will start minimized.
        /// </summary>
        public bool StartMinimized { get; set; } = false;

        /// <summary>
        ///     The program will load all on turning on. 
        /// </summary>
        public bool LoadAllOnTurnOn { get; set; } = false;
        /// <summary>
        ///     The program will load all on startup.
        /// </summary>
        public bool LoadAllOnStartup { get; set; } = false;
    }
}
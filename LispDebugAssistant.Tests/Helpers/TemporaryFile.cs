using System;
using System.IO;
using autonet.lsp;
using Nucs.Alda.Helpers;

namespace LispDebugAssistant.Tests.Helpers {
    public class TemporaryFile : IDisposable {
        private readonly string _resourcename;
        private readonly FileInfo _fileInfo;

        public TemporaryFile(string resourcename, int version = -1, bool writetofile = false) {
            _resourcename = resourcename;
            Resource = ResourceHelper.GetResource(resourcename, version);
            if (writetofile) {
                var tmpfile = Path.GetTempFileName();
                File.WriteAllText(Resource.FileName, Resource.Content);
                _fileInfo = new FileInfo(tmpfile);
            }
        }

        private LspLoader.ResourceInfo Resource { get; set; }

        public string ResourceName => Resource.FileName;

        public FileInfo FileInfo {
            get {
                if (_fileInfo == null)
                    throw new InvalidOperationException();
                return _fileInfo;
            }
        }

        public string Content => Resource.Content;

        public void Dispose() {
            if (_fileInfo != null) {
                try {
                    File.Delete(_fileInfo.FullName);
                } catch { }
            }
        }
    }
}
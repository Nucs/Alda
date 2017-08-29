using System;
using Common;
using LispDebugAssistant.Tests.Helpers;
using Nucs.Alda.Helpers;
using Nucs.Alda.Lsp;
using Xunit;
using Xunit.Sdk;

namespace LispDebugAssistant.Tests {
    public class LispTests {
        [Fact]
        public void FullCycleLoad() {
            using (var fi = new TemporaryFile("referencelesslsp", writetofile:true)) {
                using (var manager = new LspManager()) {
                    var lf = new LspFile(fi.FileInfo, manager);
                    
                }
            }
        }

        [Fact]
        public void FindLspFile() {
            using (var fi = new TemporaryFile("referencelesslsp", writetofile:true)) {
                using (var manager = new LspManager()) {
                    var lf = new LspFile(fi.FileInfo, manager);
                    manager.Watch(lf);
                    var f = manager.FindLspFile(fi.FileInfo.FullName);
                    Assert.True(Paths.CompareTo(lf.FullPathInfo, f.FullPathInfo));
                    f = manager.FindLspFile(fi.FileInfo.FullName.ToLowerInvariant());
                    Assert.True(Paths.CompareTo(lf.FullPathInfo, f.FullPathInfo));
                    f = manager.FindLspFileByDir(lf.FolderPathInfo.FullName)[0];
                    Assert.True(Paths.CompareTo(lf.FullPathInfo, f.FullPathInfo));
                    Assert.True(manager.FindReferencingFiles(lf).Count==0);
                    Assert.True(Paths.CompareTo(lf.FullPathInfo, f.FullPathInfo));
                    manager.Remove(lf);

                }
            }
        }
    }
}
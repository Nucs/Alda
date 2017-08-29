using LispDebugAssistant.Tests.Helpers;
using Nucs.Alda.Helpers;
using Xunit;

namespace LispDebugAssistant.Tests {
    public class ResourceLogicTests {
        [Fact]
        public void LoadNonExistingResourceLsp() {
            Assert.Throws<ResourceNotFoundException>(() => new TemporaryFile("i dont exist")?.Dispose());
        }

        [Fact]
        public void LoadResourceLsp() {
            new TemporaryFile("versionone").Dispose();
        }

        [Fact]
        public void LoadResourceLspVersionSpecific() {
            new TemporaryFile("versionone", 1).Dispose();
        }

        [Fact]
        public void LoadNonExistingVersionResourceLsp() {
            Assert.Throws<ResourceNotFoundException>(() => new TemporaryFile("versionone", 2)?.Dispose());
        }
    }
}
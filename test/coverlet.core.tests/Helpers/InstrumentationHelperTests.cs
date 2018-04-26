using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

using Xunit;
using Coverlet.Core.Helpers;

namespace Coverlet.Core.Helpers.Tests
{
    [ExcludeFromCodeCoverage]
    public class InstrumentationHelperTests : IDisposable
    {
        private readonly DirectoryInfo _tempDirectory;

        public InstrumentationHelperTests()
        {
            var tempPath = Path.GetTempPath();
            _tempDirectory = Directory.CreateDirectory(Path.Combine(tempPath, "tempdir"));
        }

        [Fact]
        public void GetDependencies__ReturnedDependenciesDoesNotContainsTargetModule()
        {
            // Arrange
            var module = typeof(InstrumentationHelperTests).Assembly.Location;
            // Act
            var modules = InstrumentationHelper.GetDependencies(module);
            // Assert
            Assert.DoesNotContain(module, modules);
        }

        [Fact]
        public void HasPdb__ReturnTrue()
        {
            // Arrange
            var module = typeof(InstrumentationHelperTests).Assembly.Location;
            // Act
            var hasPdb = InstrumentationHelper.HasPdb(module);
            // Assert
            Assert.True(hasPdb);
        }

        [Fact]
        public void BackupOriginalModule__BackupFileIsExists()
        {
            // Arrange
            var module = typeof(InstrumentationHelperTests).Assembly.Location;
            var identifier = Guid.NewGuid().ToString();
            // Act
            InstrumentationHelper.BackupOriginalModule(module, identifier);
            // Assert
            var backupPath = Path.Combine(
                Path.GetTempPath(),
                Path.GetFileNameWithoutExtension(module) + "_" + identifier + ".dll"
            );
            Assert.True(File.Exists(backupPath));
        }

        [Fact]
        public void CopyCoverletDependency__ForSomeModuleCopyAllDependencies()
        {
            // Arrange
            var module = Path.Combine(_tempDirectory.FullName, "somemodule.dll");
            // Act
            InstrumentationHelper.CopyCoverletDependency(module);
            // Assert
            var copiedFile = Path.Combine(_tempDirectory.FullName, "coverlet.core.dll");
            Assert.True(File.Exists(copiedFile));
        }

        [Fact]
        public void DontCopyCoverletDependency()
        { 
            // Arrange
            var module = Path.Combine(_tempDirectory.FullName, "coverlet.core.dll");
            // Act
            InstrumentationHelper.CopyCoverletDependency(module);
            // Assert
            var copiedFile = Path.Combine(_tempDirectory.FullName, "coverlet.core.dll");
            Assert.False(File.Exists(copiedFile));
        }

        [Fact]
        public void ReadHitsFile__TempFileIsExists__AfterReadingFromHitsFileLinesIsNotNull()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            // Act
            var lines = InstrumentationHelper.ReadHitsFile(tempFile);
            // Assert
            Assert.True(File.Exists(tempFile));
            Assert.NotNull(lines);
        }
        
        [Fact]
        public void DeleteHitsFile__TempFileIsExists__AfterDeletingTempFileWasRemoved()
        {
            var tempFile = Path.GetTempFileName();
            Assert.True(File.Exists(tempFile));
            // Act
            InstrumentationHelper.DeleteHitsFile(tempFile);
            // Assert
            Assert.False(File.Exists(tempFile));
        }

        public void Dispose()
        {
            Directory.Delete(_tempDirectory.FullName, true);
        }
    }
}
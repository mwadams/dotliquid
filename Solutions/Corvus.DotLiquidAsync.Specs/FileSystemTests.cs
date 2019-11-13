namespace DotLiquid.Tests
{
    using System.Globalization;
    using DotLiquid.Exceptions;
    using DotLiquid.FileSystems;
    using NUnit.Framework;
    using System.Reflection;
    using System.IO;

    [TestFixture]
    public class FileSystemTests
    {
        [Test]
        public void TestDefault()
        {
            Assert.Throws<FileSystemException>(() => new BlankFileSystem().ReadTemplateFileAsync(new Context(CultureInfo.InvariantCulture), "dummy").GetAwaiter().GetResult());
        }


        [Test]
        [Category("windows")]
        public void TestLocal()
        {
            var fileSystem = new LocalFileSystem(Path.GetFullPath(@"D:\Some\Path"));
            Assert.AreEqual(Path.GetFullPath(@"D:\Some\Path\_mypartial.liquid"), fileSystem.FullPath("mypartial"));
            Assert.AreEqual(Path.GetFullPath(@"D:\Some\Path\dir\_mypartial.liquid"), fileSystem.FullPath("dir/mypartial"));

            Assert.Throws<FileSystemException>(() => fileSystem.FullPath("../dir/mypartial"));
            Assert.Throws<FileSystemException>(() => fileSystem.FullPath("/dir/../../dir/mypartial"));
            Assert.Throws<FileSystemException>(() => fileSystem.FullPath("/etc/passwd"));
            Assert.Throws<FileSystemException>(() => fileSystem.FullPath(@"C:\mypartial"));
        }

        [Test]
        [Category("windows")]
        public void TestLocalWithBracketsInPath()
        {
            var fileSystem = new LocalFileSystem(Path.GetFullPath(@"D:\Some (thing)\Path"));
            Assert.AreEqual(Path.GetFullPath(@"D:\Some (thing)\Path\_mypartial.liquid"), fileSystem.FullPath("mypartial"));
            Assert.AreEqual(Path.GetFullPath(@"D:\Some (thing)\Path\dir\_mypartial.liquid"), fileSystem.FullPath("dir/mypartial"));
        }


        [Test]
        public void TestEmbeddedResource()
        {
            Assembly assembly = typeof(FileSystemTests).GetTypeInfo().Assembly;
            var fileSystem = new EmbeddedFileSystem(assembly, "DotLiquid.Tests.Embedded");
            Assert.AreEqual(@"DotLiquid.Tests.Embedded._mypartial.liquid", fileSystem.FullPath("mypartial"));
            Assert.AreEqual(@"DotLiquid.Tests.Embedded.dir._mypartial.liquid", fileSystem.FullPath("dir/mypartial"));

            Assert.Throws<FileSystemException>(() => fileSystem.FullPath("../dir/mypartial"));
            Assert.Throws<FileSystemException>(() => fileSystem.FullPath("/dir/../../dir/mypartial"));
            Assert.Throws<FileSystemException>(() => fileSystem.FullPath("/etc/passwd"));
            Assert.Throws<FileSystemException>(() => fileSystem.FullPath(@"C:\mypartial"));
        }
    }
}

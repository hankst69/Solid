
using FluentAssertions;

using NUnit.Framework;

using Solid.Infrastructure.Environment;
using Solid.Infrastructure.Environment.Impl;

namespace Solid.Infrastructure_uTest.Environment
{
    public class FolderProviderTests
    {
        private IFolderProvider _target;

        [SetUp]
        public void SetUp()
        {
            _target = new FolderProvider();
        }

        [TestCase(@"c:\folder\folder\", @"c:\folder\folder\")]
        [TestCase(@"c:\folder!folder\", @"c:\folder!folder\")]
        [TestCase(@"c:\folder/folder\", @"c:\folder/folder\")]
        [TestCase(@"c:\folder?folder\", @"c:\folder?folder\")]
        [TestCase(@"c:\folder""folder\", @"c:\folder""folder\")]
        [TestCase(@"c:\folder:folder\", @"c:\folder:folder\")]
        [TestCase("c:\\folder\nfolder", @"c:\folder_folder")]
        [TestCase("c:\\folder\tfolder", @"c:\folder_folder")]
        [TestCase("c:\\folder\rfolder", @"c:\folder_folder")]
        [TestCase("c:\\folder\vfolder", @"c:\folder_folder")]
        [TestCase("c:\\folder\afolder", @"c:\folder_folder")]
        [TestCase("c:\\folder\bfolder", @"c:\folder_folder")]
        [TestCase("c:\\folder\ffolder", @"c:\folder_folder")]
        public void EnsureValidPathName(string input, string expected)//ShouldReturnExpectedResult()
        {
            // ARRANGE
            // ACT
            var result = _target.EnsureValidPathName(input);
            // ASSERT
            result.Should().NotBeNull().And.BeEquivalentTo(expected);
        }

        [TestCase(@"filename.ext", @"filename.ext")]
        [TestCase(@"f!i""l§e%n&a/m(e)f=i?l{e[n]a}m\e*f+i~l#e'n:a;m,e.ext", @"f!i_l§e%n&a_m(e)f=i_l{e[n]a}m_e_f+i~l#e'n_a;m,e.ext")]
        public void EnsureValidFileName(string input, string expected)//ShouldReturnExpectedResult()
        {
            // ARRANGE
            // ACT
            var result = _target.EnsureValidFileName(input);
            // ASSERT
            result.Should().NotBeNull().And.BeEquivalentTo(expected);
        }

        [TestCase(@"c:\folder\folder\", @"c__folder_folder_")]
        public void ConvertPathNameIntoFileName(string input, string expected)//ShouldReturnExpectedResult()
        {
            // ARRANGE
            // ACT
            var result = _target.ConvertPathNameIntoFileName(input);
            // ASSERT
            result.Should().NotBeNull().And.BeEquivalentTo(expected);
        }
    }
}

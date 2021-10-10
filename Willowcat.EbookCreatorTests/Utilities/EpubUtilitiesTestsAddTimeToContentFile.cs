using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Text;
using Willowcat.EbookCreator.Utilities;

namespace Willowcat.EbookCreatorTests.Utilities
{
    [TestClass()]
    public class EpubUtilitiesTestsAddTimeToContentFile
    {
        private static void AreEqual(string expectedContents, string actualContents)
        {
            var expectedLines = expectedContents.Split('\n');
            var actualLines = actualContents.Split('\n');
            int actualIndex = 0;
            for (int expectedIndex = 0; expectedIndex < expectedLines.Length; expectedIndex++)
            {
                if (actualIndex < actualLines.Length)
                {
                    while (string.IsNullOrEmpty(actualLines[actualIndex].Trim()))
                    {
                        actualIndex++;
                    }
                    Assert.AreEqual(expectedLines[expectedIndex].Trim(), actualLines[actualIndex].Trim(), $"difference at line {actualIndex}");
                    actualIndex++;
                }
                else
                {
                    Assert.Fail($"expected at least {actualIndex + 1} lines");
                }
            }
        }

        private string RunTest(string contents, TimeSpan timeToRead)
        {
            string result = null;
            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(contents)))
            {
                result = EpubUtilities.AddOrSetMetadataElementInStream(stream, timeToRead);
            }
            return result;
        }

        [TestMethod()]
        public void AddOrSetMetadataElementInStreamTest_ExistingCustom()
            => AreEqual(Properties.Resources.contents_customfields, RunTest(Properties.Resources.contents_customfields_notime, new TimeSpan(3, 45, 0)));

        [TestMethod()]
        public void AddOrSetMetadataElementInStreamTest_NoCustom()
            => AreEqual(Properties.Resources.contents_timeread, RunTest(Properties.Resources.contents_basic, new TimeSpan(4, 45, 0)));

        [TestMethod()]
        public void AddOrSetMetadataElementInStreamTest_OldFormat()
            => AreEqual(Properties.Resources.contents_customfields_2, RunTest(Properties.Resources.contents_oldformat, new TimeSpan(3, 45, 0)));
    }
}

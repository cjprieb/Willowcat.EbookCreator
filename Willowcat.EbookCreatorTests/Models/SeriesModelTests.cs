using Microsoft.VisualStudio.TestTools.UnitTesting;
using Willowcat.EbookCreator.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Willowcat.EbookCreator.Models.Tests
{
    [TestClass()]
    public class SeriesModelTests
    {
        private void AssertEqual(WorkModel[] expectedItems, IEnumerable<WorkModel> actualItems)
        {
            int i = 0;
            foreach (var actual in actualItems)
            {
                if (i < expectedItems.Length)
                {
                    Assert.AreEqual(expectedItems[i].Title, actual.Title);
                }
                else
                {
                    Assert.Fail($"Did not expected {actual.Title} at index {i}");
                }
                i++;
            }
            if (i < expectedItems.Length)
            {
                Assert.Fail($"Expected {expectedItems[i]} at index {i}");
            }
        }

        private List<WorkModel> GetSampleWorks(int max = 10)
        {
            List<WorkModel> result = new List<WorkModel>();
            for (int i = 1; i <= max; i++)
            {
                result.Add(new WorkModel() { Index = i, Title = $"test {i}" });
            }
            return result;
        }

        [TestMethod()]
        public void SetWorkIndexesTest_null_to_3()
        {
            var sampleWorks = GetSampleWorks();
            var model = new SeriesModel();
            var expectedWorks = new WorkModel[]
            {
                sampleWorks[0],sampleWorks[1],sampleWorks[2]
            };

            model.SetWorkIndexes(null, 3);
            AssertEqual(expectedWorks, model.FilterWorksToInclude(sampleWorks));
        }

        [TestMethod()]
        public void SetWorkIndexesTest_3_to_7()
        {
            var sampleWorks = GetSampleWorks();
            var model = new SeriesModel();
            var expectedWorks = new WorkModel[]
            {
                sampleWorks[2],sampleWorks[3],sampleWorks[4],sampleWorks[5],sampleWorks[6]
            };

            model.SetWorkIndexes(3, 7);
            AssertEqual(expectedWorks, model.FilterWorksToInclude(sampleWorks));
        }

        [TestMethod()]
        public void SetWorkIndexesTest_7_to_null()
        {
            var sampleWorks = GetSampleWorks();
            var model = new SeriesModel();
            var expectedWorks = new WorkModel[]
            {
                sampleWorks[6],sampleWorks[7],sampleWorks[8],sampleWorks[9]
            };

            model.SetWorkIndexes(7, null);
            AssertEqual(expectedWorks, model.FilterWorksToInclude(sampleWorks));
        }

        [TestMethod()]
        public void SetWorkIndexesTest_7_to_null_9_items()
        {
            var sampleWorks = GetSampleWorks();
            sampleWorks.Remove(sampleWorks.Last());
            var model = new SeriesModel();
            var expectedWorks = new WorkModel[]
            {
                sampleWorks[6],sampleWorks[7],sampleWorks[8]
            };

            model.SetWorkIndexes(7, null);
            AssertEqual(expectedWorks, model.FilterWorksToInclude(sampleWorks));
        }

        [TestMethod()]
        public void WorkIndexes_2_4_6()
        {
            var sampleWorks = GetSampleWorks();
            sampleWorks.Remove(sampleWorks.Last());
            var model = new SeriesModel();
            var expectedWorks = new WorkModel[]
            {
                sampleWorks[1],sampleWorks[3],sampleWorks[5]
            };

            model.WorkIndexes = new int[] { 2, 4, 6 };
            AssertEqual(expectedWorks, model.FilterWorksToInclude(sampleWorks));
        }

        [TestMethod()]
        public void SetWorkIndexesFromStringTest_10_and_12_to_15()
        {
            var sampleWorks = GetSampleWorks(20);
            var model = new SeriesModel();
            var expectedWorks = new WorkModel[]
            {
                sampleWorks[9],sampleWorks[11],sampleWorks[12],sampleWorks[13],sampleWorks[14]
            };

            model.SetWorkIndexesFromString("10, 12 - 15");
            AssertEqual(expectedWorks, model.FilterWorksToInclude(sampleWorks));
        }

        [TestMethod()]
        public void SetWorkIndexesFromStringTest_3_to_7_spaces()
        {
            var sampleWorks = GetSampleWorks();
            var model = new SeriesModel();
            var expectedWorks = new WorkModel[]
            {
                sampleWorks[2],sampleWorks[3],sampleWorks[4],sampleWorks[5],sampleWorks[6]
            };

            model.SetWorkIndexesFromString("3 - 7");
            AssertEqual(expectedWorks, model.FilterWorksToInclude(sampleWorks));
        }

        [TestMethod()]
        public void SetWorkIndexesFromStringTest_3_to_7_nospace()
        {
            var sampleWorks = GetSampleWorks();
            var model = new SeriesModel();
            var expectedWorks = new WorkModel[]
            {
                sampleWorks[2],sampleWorks[3],sampleWorks[4],sampleWorks[5],sampleWorks[6]
            };

            model.SetWorkIndexesFromString("3-7");
            AssertEqual(expectedWorks, model.FilterWorksToInclude(sampleWorks));
        }

        [TestMethod()]
        public void SetWorkIndexesFromStringTest_3_and_7_spaces()
        {
            var sampleWorks = GetSampleWorks();
            var model = new SeriesModel();
            var expectedWorks = new WorkModel[]
            {
                sampleWorks[2],sampleWorks[6]
            };

            model.SetWorkIndexesFromString(" 3, 7");
            AssertEqual(expectedWorks, model.FilterWorksToInclude(sampleWorks));
        }

        [TestMethod()]
        public void SetWorkIndexesFromStringTest_3_and_7_nospace()
        {
            var sampleWorks = GetSampleWorks();
            var model = new SeriesModel();
            var expectedWorks = new WorkModel[]
            {
                sampleWorks[2],sampleWorks[6]
            };

            model.SetWorkIndexesFromString("3,7");
            AssertEqual(expectedWorks, model.FilterWorksToInclude(sampleWorks));
        }

        [TestMethod()]
        public void SetWorkIndexesFromStringTest_7_and_3()
        {
            var sampleWorks = GetSampleWorks();
            var model = new SeriesModel();
            var expectedWorks = new WorkModel[]
            {
                sampleWorks[6],sampleWorks[2]
            };

            model.SetWorkIndexesFromString("7,3");
            AssertEqual(expectedWorks, model.FilterWorksToInclude(sampleWorks));
        }

        [TestMethod()]
        public void SetWorkIndexesFromStringTest_7_and_3_to_5()
        {
            var sampleWorks = GetSampleWorks();
            var model = new SeriesModel();
            var expectedWorks = new WorkModel[]
            {
                sampleWorks[6],sampleWorks[2],sampleWorks[3],sampleWorks[4]
            };

            model.SetWorkIndexesFromString("7, 3 - 5");
            AssertEqual(expectedWorks, model.FilterWorksToInclude(sampleWorks));
        }

        [TestMethod()]
        public void SetWorkIndexesFromStringTest_none()
        {
            var sampleWorks = GetSampleWorks();
            var model = new SeriesModel();
            var expectedWorks = sampleWorks.ToArray();

            model.SetWorkIndexesFromString("none");
            AssertEqual(expectedWorks, model.FilterWorksToInclude(sampleWorks));
        }
    }
}
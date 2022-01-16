using Microsoft.VisualStudio.TestTools.UnitTesting;
using Willowcat.EbookCreator.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using Ionic.Zip;

namespace Willowcat.EbookCreator.Utilities.Tests
{
    [TestClass()]
    public class EpubUtilitiesTests
    {
        private const int _WordsPerMinute = 471;
        private const string _CalibreDirectory = @"D:\Users\Crystal\Sync\eBooks\Calibre\";

        private Dictionary<string, TestBook> _TestBooks = new Dictionary<string, TestBook>()
        {
            { "SaSarad", new TestBook(@"Blue_Sunshine\Sa Sarad (551)\Sa Sarad - Blue_Sunshine.epub", 184858, 6, 39) },
            { "Dragon", new TestBook(@"00AwkwardPenguin00\Dragon of the Yuyan (497)\Dragon of the Yuyan - 00AwkwardPenguin00.epub", 148936, 5, 0) },
            { "Pimpernel", new TestBook(@"Baroness Orczy\The Scarlet Pimpernel (25)\The Scarlet Pimpernel - Baroness Orczy.epub", 87616, 3, 3) },
            { "Ethan", new TestBook(@"Lois McMaster Bujold\Ethan of Athos (9)\Ethan of Athos - Lois McMaster Bujold.epub", 64539, 2, 14) },
            { "WayOfKings", new TestBook(@"Brandon Sanderson\The Way of Kings (154)\The Way of Kings - Brandon Sanderson.epub", 397120, 13, 47) }
        };

        private IEnumerable<CalibreBook> GetAllEbooks()
        {
            foreach (var authorDirectory in Directory.GetDirectories(_CalibreDirectory))
            {
                foreach (var bookDirectory in Directory.GetDirectories(authorDirectory))
                {
                    string contentPath = null;
                    string epubPath = null;
                    foreach (var bookPath in Directory.GetFiles(bookDirectory))
                    {
                        if (bookPath.EndsWith(".opf") && contentPath == null)
                        {
                            contentPath = bookPath;
                        }
                        else if (bookPath.EndsWith(".epub") && epubPath == null)
                        {
                            epubPath = bookPath;
                        }
                    }
                    yield return new CalibreBook()
                    {
                        contentPath = contentPath,
                        epubPath = epubPath
                    };
                }
            }
        }

        private void RunWordCountTest(TestBook testBook)
        {
            string path = Path.Combine(_CalibreDirectory, testBook.relativePath);
            Assert.AreEqual(testBook.expectedWordCount, EpubUtilities.GetWordCount(path));
        }

        private void RunCalculateTimeTest(TestBook testBook)
        {
            string path = Path.Combine(_CalibreDirectory, testBook.relativePath);
            Assert.AreEqual(testBook.expectedTimeToRead, EpubUtilities.CalculateTimeToReadBook(path, _WordsPerMinute));
        }

        //[TestMethod]
        //public void AddTimeToRead()
        //{
        //    var path = @"D:\Users\Crystal\Sync\eBooks\Calibre\Blue_Sunshine\Rise and Fall (556)\Rise and Fall - Blue_Sunshine.epub";
        //    EpubUtilities.AddTimeToReadToBook(path, _WordsPerMinute);
        //}

        [TestMethod]
        public void CalculateEstimatedWordsPerMinute()
        {
            List<int> compuatedValues = new List<int>();
            foreach (var kvp in _TestBooks)
            {
                string path = Path.Combine(_CalibreDirectory, kvp.Value.relativePath);
                int totalMinutes = (int)kvp.Value.expectedTimeToRead.TotalMinutes;
                int wordsPerMinute = EpubUtilities.GetWordCount(path) / totalMinutes;
                compuatedValues.Add(wordsPerMinute);
                Console.WriteLine($"Estimated words per minute: {wordsPerMinute} ({kvp.Key})");
            }
            Console.WriteLine($"Average: {compuatedValues.Average()}");
        }

        [TestMethod]
        public void CalculateEstimatedReadingTime()
        {
            List<string> computedValues = new List<string>();
            foreach (var book in GetAllEbooks().Skip(3).Take(1))
            {
                try
                {
                    var timeToReadBook = EpubUtilities.CalculateTimeToReadBook(book.epubPath, _WordsPerMinute);
                    var fileName = Path.GetFileNameWithoutExtension(book.epubPath);
                    var message = $"{fileName}\t{timeToReadBook}";
                    //Console.WriteLine(message);
                    if (EpubUtilities.AddTimeToReadToEpub(book.epubPath, timeToReadBook))
                    {
                        Console.WriteLine($"File updated: {book.epubPath}");
                    }
                    else
                    {
                        Console.WriteLine($"File skipped: {book.epubPath}");
                    }
                    if (EpubUtilities.AddTimeToReadToContentFile(book.contentPath, timeToReadBook))
                    {
                        Console.WriteLine($"File updated: {book.contentPath}");
                    }
                    else
                    {
                        Console.WriteLine($"File skipped: {book.contentPath}");
                    }
                    computedValues.Add(message);
                }
                catch (ZipException ex)
                {
                    Console.WriteLine($"Zip error while processing {book} - {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error while processing {book} - {ex.Message}");
                }

            }
            File.WriteAllLines(@"D:\Users\Crystal\Sync\eBooks\reading time.txt", computedValues.ToArray());
        }

        [TestMethod()]
        public void GetWordCountTest_Dragon() => RunWordCountTest(_TestBooks["Dragon"]);

        [TestMethod()]
        public void GetWordCountTest_Ethan() => RunWordCountTest(_TestBooks["Ethan"]);

        [TestMethod()]
        public void GetWordCountTest_Pimpernel() => RunWordCountTest(_TestBooks["Pimpernel"]);

        [TestMethod()]
        public void GetWordCountTest_SaSarad() => RunWordCountTest(_TestBooks["SaSarad"]);

        [TestMethod()]
        public void GetWordCountTest_WayOfKings() => RunWordCountTest(_TestBooks["WayOfKings"]);

        [TestMethod()]
        public void CalculateTimeToReadBook_Ethan() => RunCalculateTimeTest(_TestBooks["Ethan"]);

        [TestMethod()]
        public void CalculateTimeToReadBook_Dragon() => RunCalculateTimeTest(_TestBooks["Dragon"]);

        [TestMethod()]
        public void CalculateTimeToReadBook_Pimpernel() => RunCalculateTimeTest(_TestBooks["Pimpernel"]);

        [TestMethod]
        public void CalculateTimeToReadBook_SaSarad() => RunCalculateTimeTest(_TestBooks["SaSarad"]);

        [TestMethod]
        public void CalculateTimeToReadBook_WayOfKings() => RunCalculateTimeTest(_TestBooks["WayOfKings"]);
    }

    struct TestBook
    {
        public string relativePath;
        public TimeSpan expectedTimeToRead;
        public int expectedWordCount;
        public TestBook(string path, int wordCount, int hours, int minutes)
        {
            relativePath = path;
            expectedTimeToRead = new TimeSpan(hours, minutes, 0);
            expectedWordCount = wordCount;
        }
    }

    struct CalibreBook
    {
        public string epubPath;
        public string contentPath;
    }
}
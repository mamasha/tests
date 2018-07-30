using System;
using NUnit.Framework;

namespace CsvParsers._nunit
{
    [TestFixture, Explicit]
    class UsingCsvParserTests
    {
        private static readonly string CsvInput = @"
            time,status,result 
            18:05,200,OK 
            18:02,302,Moved Permanently 
            18:02,404,Not Found 
            18:04,500,Internal Server Error 
        ";

        [Test]
        public void TraverseThroughRandomAccessTest()
        {
            using (var file = new TmpFile(CsvInput))
            {
                var csv = CsvParser.New(file.FullPath);
                Assert.That(csv.IsEmpty, Is.False);

                for (int i = 0; i < csv.Count; i++)
                {
                    dynamic row = csv[i];
                    Console.WriteLine("{0}: {1}", row.time, row.result);
                }
            }

            Assert.Pass();
        }

        [Test]
        public void RandomAccessTest()
        {
            using (var file = new TmpFile(CsvInput))
            {
                var csv = CsvParser.New(file.FullPath);
                Assert.That(csv.IsEmpty, Is.False);

                dynamic row = csv[4];

                Assert.That(row.Time, Is.EqualTo("18:02"));
                Assert.That(row.status, Is.EqualTo("404"));
                Assert.That(row.result, Is.EqualTo("Not Found"));

                row = csv[2];

                Assert.That(row.Time, Is.EqualTo("18:05"));
                Assert.That(row.status, Is.EqualTo("200"));
                Assert.That(row.result, Is.EqualTo("OK"));

                Assert.That(() => csv[10], Throws.InstanceOf<IndexOutOfRangeException>());
            }
        }

        [Test]
        public void SequentialAccessTest()
        {
            using (var file = new TmpFile(CsvInput))
            {
                var csv = CsvParser.New(file.FullPath);
                Assert.That(csv.IsEmpty, Is.False);

                foreach (var row in csv)
                {
                    Console.WriteLine("{0}: {1}", row["time"], row["result"]);
                }
            }

            Assert.Pass();
        }

        [Test]
        public void FiltersTest()
        {
            using (var file = new TmpFile(CsvInput))
            {
                var csv = CsvParser.New(file.FullPath);

                var matches = csv.WhereEquals("result", "OK");
                Assert.That(matches.Count, Is.EqualTo(1));

                matches = csv.WhereGreaterThan("status", 200);
                Assert.That(matches.Count, Is.EqualTo(3));

                matches = csv.WhereLessThan("status", 500);
                Assert.That(matches.Count, Is.EqualTo(3));
            }
        }
    }
}
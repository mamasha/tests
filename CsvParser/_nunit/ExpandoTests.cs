using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace CsvParsers._nunit
{
    [TestFixture, Explicit]
    class ExpandoTests
    {
        [Test]
        public void use_expando_row()
        {
            var columns = new Dictionary<string, int>(StringComparer.InvariantCultureIgnoreCase) {
                {"time", 0}, {"status", 1}, {"result", 2}
            };

            dynamic row = new DynamicCsvRow(CsvRow.New(columns, "18:05,200,OK"));

            Assert.That(row.Time, Is.EqualTo("18:05"));
            Assert.That(row.status, Is.EqualTo("200"));
            Assert.That(row.Result, Is.EqualTo("OK"));
        }
    }
}
using System.Collections.Generic;
using System.Linq;

namespace CsvParsers
{
    public static class CsvExtensions
    {
        public static List<ICsvRow> WhereEquals(this ICsvParser csv, string name, string value)
        {
            return 
                csv.WhereEqualsI(name, value).ToList();
        }

        public static List<ICsvRow> WhereEquals(this ICsvParser csv, string name, int value)
        {
            return 
                csv.WhereEqualsI(name, value).ToList();
        }

        public static List<ICsvRow> WhereEquals(this ICsvParser csv, string name, double value)
        {
            return 
                csv.WhereEqualsI(name, value).ToList();
        }

        public static List<ICsvRow> WhereGreaterThan(this ICsvParser csv, string name, int value)
        {
            return 
                csv.WhereGreaterThanI(name, value).ToList();
        }

        public static List<ICsvRow> WhereGreaterThan(this ICsvParser csv, string name, double value)
        {
            return 
                csv.WhereGreaterThanI(name, value).ToList();
        }

        public static List<ICsvRow> WhereLessThan(this ICsvParser csv, string name, int value)
        {
            return 
                csv.WhereLessThanI(name, value).ToList();
        }

        public static List<ICsvRow> WhereLessThan(this ICsvParser csv, string name, double value)
        {
            return 
                csv.WhereLessThanI(name, value).ToList();
        }
    }
}
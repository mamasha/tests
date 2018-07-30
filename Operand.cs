using System;

namespace CsvParsers
{
    static class Operand
    {
        public static bool StringEqual(string field, string value)
        {
            return
                field.Equals(value, StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool IntEqual(string field, int value)
        {
            int parsed;

            if (!int.TryParse(field, out parsed))
                return false;

            return parsed == value;
        }

        public static bool DoubleEqual(string field, double value)
        {
            double parsed;

            if (!double.TryParse(field, out parsed))
                return false;

            return Math.Abs(parsed - value) <= double.Epsilon;
        }

        public static bool IntGreaterThan(string field, int value)
        {
            int parsed;

            if (!int.TryParse(field, out parsed))
                return false;

            return parsed > value;
        }

        public static bool DoubleGreaterThan(string field, double value)
        {
            double parsed;

            if (!double.TryParse(field, out parsed))
                return false;

            return parsed > value;
        }

        public static bool IntLessThan(string field, int value)
        {
            int parsed;

            if (!int.TryParse(field, out parsed))
                return false;

            return parsed < value;
        }

        public static bool DoubleLessThan(string field, double value)
        {
            double parsed;

            if (!double.TryParse(field, out parsed))
                return false;

            return parsed < value;
        }
    }
}
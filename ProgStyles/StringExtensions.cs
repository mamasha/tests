using System;

namespace ProgStyles
{
    static class StringExtensions
    {
        public static string Fmt(this string fmt, params object[] args)
        {
            return
                String.Format(fmt, args);
        }
    }
}
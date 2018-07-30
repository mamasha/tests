using System.Dynamic;

namespace CsvParsers
{
    public class DynamicCsvRow : DynamicObject
    {
        private readonly ICsvRow _row;

        public DynamicCsvRow(ICsvRow row)
        {
            _row = row;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var name = binder.Name;
            result = _row[name];

            return true;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObjectProcessor.CsvProcessor
{
    [AttributeUsage(AttributeTargets.Property)]
    public class CsvIndexAttribute : Attribute
    {
        public readonly int ColumnIndex;

        public CsvIndexAttribute(int columnIndex)
        {
            ColumnIndex = columnIndex;
        }
    }
}

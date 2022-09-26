using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObjectProcessor.CsvProcessor
{
    public class CsvProcessorOptions
    {
        public bool FirstRowHasHeadings { get; set; } = true;
        public string ColumnSeperator { get; set; } = ",";
        public bool FieldsAreEnclosedInQuotes { get; set; } = true;
        public bool ThrowWhenUnableToSetPropertyValue { get; set; } = true;
    }
}

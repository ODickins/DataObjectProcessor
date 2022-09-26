using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObjectProcessor.Exceptions
{
    public class PropertyIndexException : Exception
    {
        public PropertyIndexException(string? message) : base(message)
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncTask.Exceptions
{
    public class InvalidCmdParametersException : Exception
    {
        public InvalidCmdParametersException() : base("Invalid command line parameters.") { }
    }

}

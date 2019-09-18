using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SambaflexBot.CustomErrors
{
    /// <summary>
    /// Threatment for threated exceptions.
    /// This kind of exception can be showed to users.
    /// </summary>
    public class SambaflexException : Exception
    {
        public SambaflexException(string message) : base(message) { }
    }
}

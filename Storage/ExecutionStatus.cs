using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auto_Statistic.Storage
{
    public enum ExecutionStatus
    {
        Untested,
        Success,
        Error,
        Cancel,
        ExceededMemory
    }
}

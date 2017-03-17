using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public enum EExportResult
    {
        Accepted,
        FileNotFond,
        Created,
        InProcessing,
        Locked,
        NotSupportedFormat
    }
}

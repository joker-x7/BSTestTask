using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Header
    {
        public int version;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string type;
    }
}

#pragma warning disable 162,168,649,660,661,1522
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Trinity.TSL;
namespace GraphEngineServer.InvertedIndex
{
    [StructLayout(LayoutKind.Explicit, Size = 10)]
    struct IndexItem
    {
        [FieldOffset(0)]
        internal long CellId;
        [FieldOffset(8)]
        internal ushort Offset;
    }
}

#pragma warning restore 162,168,649,660,661,1522

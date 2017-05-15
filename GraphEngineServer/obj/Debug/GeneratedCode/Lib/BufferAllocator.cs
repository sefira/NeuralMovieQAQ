#pragma warning disable 162,168,649,660,661,1522
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Trinity.Core.Lib;
using Trinity.TSL;
using Trinity.TSL.Lib;
namespace GraphEngineServer
{
    /// <summary>
    /// Reusable buffer.
    /// </summary>
    internal unsafe class BufferAllocator
    {
        const int MAX_BUFFER_LEN = 1 << 20;
        [ThreadStatic]
        static int bufferLen;
        [ThreadStatic]
        static byte* bufferPtr;
        internal unsafe static byte* AllocBuffer(int length)
        {
            if (bufferPtr == null || bufferLen < length || bufferLen > MAX_BUFFER_LEN)
            {
                if (bufferPtr != null)
                {
                    bufferPtr = (byte*)Memory.realloc(bufferPtr, (ulong)length);
                }
                else
                {
                    bufferPtr = (byte*)Memory.malloc((ulong)length);
                }
            }
            bufferLen = length;
            return bufferPtr;
        }
    }
}

#pragma warning restore 162,168,649,660,661,1522

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
    /// Represents a Trinity type corresponding .Net Guid type.
    /// </summary>
    public unsafe class GuidAccessor
    {
        internal byte* CellPtr;
        internal long? CellID;
        ///
        /// <summary>
        ///     Converts the string representation of a GUID to the equivalent <see cref="GraphEngineServer.GuidAccessor"/>
        ///     structure.
        /// </summary>
        /// <param name="input">
        ///     The GUID to convert.
        /// </param>
        /// <param name="value">
        ///     The structure that will contain the parsed value.
        /// </param>
        ///
        /// <returns>
        ///     true if the parse operation was successful; otherwise, false.
        /// </returns>
        ///
        public static bool TryParse(string input, out GuidAccessor value)
        {
            Guid val;
            if (Guid.TryParse(input, out val))
            {
                value = val;
                return true;
            }
            else
            {
                value = null;
                return false;
            }
        }
        ///
        /// <summary>
        ///     Converts the string representation of a GUID to the equivalent <see cref="System.Guid"/>
        ///     structure.
        /// </summary>
        /// <param name="input">
        ///     The GUID to convert.
        /// </param>
        /// <param name="value">
        ///     The structure that will contain the parsed value.
        /// </param>
        ///
        /// <returns>
        ///     true if the parse operation was successful; otherwise, false.
        /// </returns>
        public static bool TryParse(string input, out Guid value)
        {
            return Guid.TryParse(input, out value);
        }
        internal GuidAccessor(byte* _CellPtr)
        {
            CellPtr = _CellPtr;
        }
        internal int length
        {
            get
            {
                return 16;
            }
        }
        /// <summary>
        /// Returns a 16 byte array that contains the value of this instance.
        /// </summary>
        /// <returns>A 16 byte array.</returns>
        public unsafe byte[] ToByteArray()
        {
            byte[] ret = new byte[16];
            fixed (byte* ptr = ret)
            {
                Memory.Copy(CellPtr, ptr, 16);
            }
            return ret;
        }
        /// <summary>
        /// Returns a string representation of the value of this instance in registry format. 
        /// </summary>
        /// <returns>Returns a string representation of the value of this instance in registry format.</returns>
        public override unsafe string ToString()
        {
            Guid tmp = this;
            return tmp.ToString();
        }
        /// <summary>
        /// Converts the current instance to a Guid value.
        /// </summary>
        /// <returns>A Guid value.</returns>
        public Guid ToGuid()
        {
            return new Guid(ToByteArray());
        }
        /// <summary>
        /// Returns a string representation of the value of this Guid instance, according to the provided format specifier.
        /// </summary>
        /// <param name="format">A single format specifier that indicates how to format the value of this Guid. The format parameter can be "N", "D", "B", "P", or "X". If format is null or an empty string (""), "D" is used. </param>
        /// <returns>The value of this Guid, represented as a series of lowercase hexadecimal digits in the specified format. </returns>
        public string ToString(string format)
        {
            Guid tmp = this;
            return tmp.ToString(format);
        }
        /// <summary>
        /// Implicitly converts a GuidAccessor instance to a Guid instance.
        /// </summary>
        /// <param name="accessor">The GuidAccessor instance.</param>
        /// <returns>A Guid instance.</returns>
        public static implicit operator Guid(GuidAccessor accessor)
        {
            byte[] data = accessor.ToByteArray();
            return new Guid(data);
        }
        /// <summary>
        /// Implicitly converts a Guid instance to a GuidAccessor instance.
        /// </summary>
        /// <param name="value">The Guid instance.</param>
        /// <returns>A GuidAccessor instance.</returns>
        public static implicit operator GuidAccessor(Guid value)
        {
            byte* tmpcellptr = BufferAllocator.AllocBuffer(16);
            byte* targetPtr = tmpcellptr;
            byte[] tmpGuid = value.ToByteArray();
            fixed (byte* tmpGuidPtr = tmpGuid)
            {
                Memory.Copy(tmpGuidPtr, targetPtr, 16);
            }
            GuidAccessor ret = new GuidAccessor(tmpcellptr);
            ret.CellID = null;
            return ret;
        }
        /// <summary>
        /// Determines whether two specified GuidAccessor have the same value.
        /// </summary>
        /// <param name="a">The first GuidAccessor to compare, or null. </param>
        /// <param name="b">The second GuidAccessor to compare, or null. </param>
        /// <returns>true if the value of <paramref name="a" /> is the same as the value of <paramref name="b" />; otherwise, false.</returns>
        public static bool operator ==(GuidAccessor a, GuidAccessor b)
        {
            if (ReferenceEquals(a, b))
              return true;
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
              return false;
            if (a.CellPtr == b.CellPtr) return true;
            return Memory.Compare(a.CellPtr, b.CellPtr, 16);
        }
        /// <summary>Determines whether two specified GuidAccessor have different values.</summary>
        /// <returns>true if the value of <paramref name="a" /> is different from the value of <paramref name="b" />; otherwise, false.</returns>
        /// <param name="a">The first GuidAccessor to compare, or null. </param>
        /// <param name="b">The second GuidAccessor to compare, or null. </param>
        public static bool operator !=(GuidAccessor a, GuidAccessor b)
        {
            return !(a == b);
        }
        /// <summary>
        /// Determines whether this instance and a specified object have the same value.
        /// </summary>
        /// <param name="obj">The GuidAccessor to compare to this instance.</param>
        /// <returns>true if <paramref name="obj" /> is a GuidAccessor and its value is the same as this instance; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            GuidAccessor b = obj as GuidAccessor;
            if (b == null)
                return false;
            return (this == b);
        }
        /// <summary>
        /// Return the hash code for this doubleList.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            return HashHelper.HashBytes(this.CellPtr, this.length);
        }
    }
}

#pragma warning restore 162,168,649,660,661,1522

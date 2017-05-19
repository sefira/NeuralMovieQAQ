
#pragma warning disable 162,168,649,660,661,1522
using System;
using System.Text;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using System.Data;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.ExceptionServices;
using System.Security;
using System.Runtime.CompilerServices;

using Trinity;
using Trinity.Core.Lib;
using Trinity.Storage;
using Trinity.Utilities;
using Trinity.TSL.Lib;
using Trinity.Network;
using Trinity.Network.Sockets;
using Trinity.Network.Messaging;
using Trinity.TSL;
namespace GraphEngineServer
{

    public enum CellType : ushort 
    {
        Undefined = 0,
        Movie,
        Celebrity,
    }
    public partial struct __type_injection__
    {
		
        ///<summary>
        ///Initializes a new instance of the __type_injection__ class with the specified parameters.
        ///</summary>
        public __type_injection__(long f1=default(long),List<long> f2=null)
		{
            this.f1 = f1;
            this.f2 = f2;

		}

        public long f1;

        public List<long> f2;

        public static implicit operator Tuple<long,List<long>>(__type_injection__ FormatStruct)
        {
            return new Tuple<long,List<long>>(FormatStruct.f1,FormatStruct.f2);
        }

        public static implicit operator __type_injection__ (Tuple<long,List<long>>tuple)
        {
            return new __type_injection__(tuple.Item1,tuple.Item2);
        }

        public static implicit operator KeyValuePair<long,List<long>>(__type_injection__ FormatStruct)
        {
            return new KeyValuePair<long,List<long>>(FormatStruct.f1,FormatStruct.f2);
        }

        public static implicit operator __type_injection__ (KeyValuePair<long,List<long>>tuple)
        {
                return new __type_injection__(tuple.Key,tuple.Value);
        }

        public static bool operator == (__type_injection__ a, __type_injection__ b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }
            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }
            // Return true if the fields match:
            return a.f1 == b.f1 && a.f2 == b.f2;
        }

        public static bool operator != (__type_injection__ a, __type_injection__ b)
        {
            return !(a == b);
        }

    }

    /// <summary>
    /// Provides in-place operations of __type_injection__ defined in TSL.
    /// </summary>
    public unsafe class __type_injection___Accessor
    {
        ///<summary>
        ///The pointer to the content of the object.
        ///</summary>
        internal byte* CellPtr;
        internal long? CellID;
        internal ResizeFunctionDelegate ResizeFunction;
        internal unsafe __type_injection___Accessor(byte* _CellPtr, ResizeFunctionDelegate func)
        {
            CellPtr = _CellPtr;
            ResizeFunction = func;
            
        f2_Accessor_Field = new longListAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.CellPtr = this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        }

        public static implicit operator __type_injection___Accessor(__type_injection___Accessor_ReadOnly accessor )
        {
            return new __type_injection___Accessor(accessor.CellPtr, accessor.ResizeFunction);
        }

        internal static string[] optional_field_names;
        ///<summary>
        ///Get an array of the names of all optional fields for object type __type_injection__.
        ///</summary>
        public static string[] GetOptionalFieldNames()
        {
            if(optional_field_names == null)
                optional_field_names = new string[] {};
            return optional_field_names;   
        }

        ///<summary>
        ///Get a list of the names of available optional fields in the object being operated by this accessor.
        ///</summary>
        internal List<string> GetNotNullOptionalFields()
        {
            List<string> list = new List<string>();
            BitArray ba = new BitArray(GetOptionalFieldMap());
            string[] optional_fields = GetOptionalFieldNames();
            for (int i = 0; i < ba.Count; i++)
            {
                if(ba[i])
                    list.Add(optional_fields[i]);
            }
            return list;
        }

        internal unsafe byte[] GetOptionalFieldMap()
        {
            return new byte[0];
        }

        ///<summary>
        ///Copies the struct content into a byte array.
        ///</summary>
        public byte[] ToByteArray()
        {
            byte* targetPtr = CellPtr;
            targetPtr += 8;
            targetPtr += 4 + *(int*)targetPtr;

            int size = (int)(targetPtr - CellPtr);
            byte[] ret = new byte[size];
            Memory.Copy(CellPtr,0,ret,0,size);
            return ret;
        }

        ///<summary>
        ///Provides in-place access to the object field f1.
        ///</summary>
        public unsafe long f1
        {
            get
            {
                
                byte* targetPtr = CellPtr;

                return *(long*)(targetPtr);
            }

            set
            {
                byte* targetPtr = CellPtr;

                *(long*)(targetPtr) = value;
            }

        }
longListAccessor f2_Accessor_Field;
        ///<summary>
        ///Provides in-place access to the object field f2.
        ///</summary>
        public unsafe longListAccessor f2
        {
            get
            {
                
                byte* targetPtr = CellPtr;
            targetPtr += 8;

                f2_Accessor_Field.CellPtr = targetPtr + 4;
                f2_Accessor_Field.CellID = this.CellID;
                return f2_Accessor_Field;
            }

            set
            {
                if ((object)value == null) throw new ArgumentNullException("The assigned variable is null.");
                byte* targetPtr = CellPtr;
            targetPtr += 8;

                f2_Accessor_Field.CellID = this.CellID;
                
              int length = *(int*)(value.CellPtr - 4);

                //senario: cell_a.inlinks = cell_b.inlinks,
                //the later part will invoke the Get, filling cell_b.inlinks(a inlink_accessor_fiedld)'storage CellID
                int oldlength = *(int*)targetPtr;
                if (value.CellID != f2_Accessor_Field.CellID)
                {
                    //if not in the same Cell
                    f2_Accessor_Field.CellPtr = f2_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                    Memory.Copy(value.CellPtr - 4, f2_Accessor_Field.CellPtr, length + 4);
                }
                else
                {
                    byte[] tmpcell = new byte[length + 4];
                    fixed (byte* tmpcellptr = tmpcell)
                    {                        
                        Memory.Copy(value.CellPtr - 4, tmpcellptr, length + 4);
                        f2_Accessor_Field.CellPtr = f2_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                        Memory.Copy(tmpcellptr, f2_Accessor_Field.CellPtr, length + 4);
                    }
                }
                f2_Accessor_Field.CellPtr += 4;
                
            }

    }

        public static unsafe implicit operator __type_injection__(__type_injection___Accessor accessor)
        {
            
            return new __type_injection__(accessor.f1,accessor.f2);
        }

        public unsafe static implicit operator __type_injection___Accessor(__type_injection__ field)
        {  
            byte* targetPtr = null;
            
        {targetPtr += 8;

if(field.f2!= null)
{
    targetPtr += field.f2.Count*8+sizeof(int);
}else
{
    targetPtr += sizeof(int);
}


        }
            byte* tmpcellptr = BufferAllocator.AllocBuffer((int)targetPtr);
            Memory.memset(tmpcellptr, 0, (ulong)targetPtr);
            targetPtr = tmpcellptr;
        
        {*(long*)targetPtr = field.f1;
            targetPtr += 8;

if(field.f2!= null)
{
    *(int*)targetPtr = field.f2.Count*8;
    targetPtr += sizeof(int);
    for(int iterator_1 = 0;iterator_1<field.f2.Count;++iterator_1)
    {
*(long*)targetPtr = field.f2[iterator_1];
            targetPtr += 8;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

        }
            __type_injection___Accessor ret = new __type_injection___Accessor(tmpcellptr,null);
            ret.CellID = null;
            return ret;
        }


        public static bool operator == (__type_injection___Accessor a, __type_injection___Accessor b)
        {
            if (ReferenceEquals(a, b))
              return true;
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
              return false;
            // If both are same instance, return true.
            if (a.CellPtr == b.CellPtr) return true;
            byte* targetPtr = a.CellPtr;
            targetPtr += 8;
            targetPtr += 4 + *(int*)targetPtr;

            int lengthA = (int)(targetPtr - a.CellPtr);
            targetPtr = b.CellPtr;
            targetPtr += 8;
            targetPtr += 4 + *(int*)targetPtr;

            int lengthB = (int)(targetPtr - b.CellPtr);
            if(lengthA != lengthB) return false;
            return Memory.Compare(a.CellPtr,b.CellPtr,lengthA);
        }

        public static bool operator != (__type_injection___Accessor a, __type_injection___Accessor b)
        {
            return !(a == b);
        }

    }

    /// <summary>
    /// Provides in-place operations of __type_injection__ defined in TSL.
    /// </summary>
    public unsafe class __type_injection___Accessor_ReadOnly
    {
        ///<summary>
        ///The pointer to the content of the object.
        ///</summary>
        internal byte* CellPtr;
        internal long? CellID;
        internal ResizeFunctionDelegate ResizeFunction;
        internal unsafe __type_injection___Accessor_ReadOnly(byte* _CellPtr, ResizeFunctionDelegate func)
        {
            CellPtr = _CellPtr;
            ResizeFunction = func;
            
        f2_Accessor_Field = new longListAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.CellPtr = this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        }

        public static implicit operator __type_injection___Accessor_ReadOnly(__type_injection___Accessor accessor )
        {
            return new __type_injection___Accessor_ReadOnly(accessor.CellPtr, accessor.ResizeFunction);
        }

        internal static string[] optional_field_names;
        ///<summary>
        ///Get an array of the names of all optional fields for object type __type_injection__.
        ///</summary>
        public static string[] GetOptionalFieldNames()
        {
            if(optional_field_names == null)
                optional_field_names = new string[] {};
            return optional_field_names;   
        }

        ///<summary>
        ///Get a list of the names of available optional fields in the object being operated by this accessor.
        ///</summary>
        internal List<string> GetNotNullOptionalFields()
        {
            List<string> list = new List<string>();
            BitArray ba = new BitArray(GetOptionalFieldMap());
            string[] optional_fields = GetOptionalFieldNames();
            for (int i = 0; i < ba.Count; i++)
            {
                if(ba[i])
                    list.Add(optional_fields[i]);
            }
            return list;
        }

        internal unsafe byte[] GetOptionalFieldMap()
        {
            return new byte[0];
        }

        ///<summary>
        ///Copies the struct content into a byte array.
        ///</summary>
        public byte[] ToByteArray()
        {
            byte* targetPtr = CellPtr;
            targetPtr += 8;
            targetPtr += 4 + *(int*)targetPtr;

            int size = (int)(targetPtr - CellPtr);
            byte[] ret = new byte[size];
            Memory.Copy(CellPtr,0,ret,0,size);
            return ret;
        }

        ///<summary>
        ///Provides in-place access to the object field f1.
        ///</summary>
        public unsafe long f1
        {
            get
            {
                
                byte* targetPtr = CellPtr;

                return *(long*)(targetPtr);
            }

        }
longListAccessor f2_Accessor_Field;
        ///<summary>
        ///Provides in-place access to the object field f2.
        ///</summary>
        public unsafe longListAccessor f2
        {
            get
            {
                
                byte* targetPtr = CellPtr;
            targetPtr += 8;

                f2_Accessor_Field.CellPtr = targetPtr + 4;
                f2_Accessor_Field.CellID = this.CellID;
                return f2_Accessor_Field;
            }

    }

        public static unsafe implicit operator __type_injection__(__type_injection___Accessor_ReadOnly accessor)
        {
            
            return new __type_injection__(accessor.f1,accessor.f2);
        }

        public unsafe static implicit operator __type_injection___Accessor_ReadOnly(__type_injection__ field)
        {  
            byte* targetPtr = null;
            
        {targetPtr += 8;

if(field.f2!= null)
{
    targetPtr += field.f2.Count*8+sizeof(int);
}else
{
    targetPtr += sizeof(int);
}


        }
            byte* tmpcellptr = BufferAllocator.AllocBuffer((int)targetPtr);
            Memory.memset(tmpcellptr, 0, (ulong)targetPtr);
            targetPtr = tmpcellptr;
        
        {*(long*)targetPtr = field.f1;
            targetPtr += 8;

if(field.f2!= null)
{
    *(int*)targetPtr = field.f2.Count*8;
    targetPtr += sizeof(int);
    for(int iterator_1 = 0;iterator_1<field.f2.Count;++iterator_1)
    {
*(long*)targetPtr = field.f2[iterator_1];
            targetPtr += 8;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

        }
            __type_injection___Accessor_ReadOnly ret = new __type_injection___Accessor_ReadOnly(tmpcellptr,null);
            ret.CellID = null;
            return ret;
        }


        public static bool operator == (__type_injection___Accessor_ReadOnly a, __type_injection___Accessor_ReadOnly b)
        {
            if (ReferenceEquals(a, b))
              return true;
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
              return false;
            // If both are same instance, return true.
            if (a.CellPtr == b.CellPtr) return true;
            byte* targetPtr = a.CellPtr;
            targetPtr += 8;
            targetPtr += 4 + *(int*)targetPtr;

            int lengthA = (int)(targetPtr - a.CellPtr);
            targetPtr = b.CellPtr;
            targetPtr += 8;
            targetPtr += 4 + *(int*)targetPtr;

            int lengthB = (int)(targetPtr - b.CellPtr);
            if(lengthA != lengthB) return false;
            return Memory.Compare(a.CellPtr,b.CellPtr,lengthA);
        }

        public static bool operator != (__type_injection___Accessor_ReadOnly a, __type_injection___Accessor_ReadOnly b)
        {
            return !(a == b);
        }

    }

}

#pragma warning restore 162,168,649,660,661,1522
#pragma warning disable 162,168,649,660,661,1522

namespace GraphEngineServer
{
    /// <summary>
    /// A .NET runtime object representation of __type_injection__ defined in TSL.
    /// </summary>
    public partial struct __type_injection__
    {
        #region MUTE
        
        #endregion 
        /// <summary>
        /// Converts the string representation of a __type_injection__ to its
        /// struct equivalent. A return value indicates whether the 
        /// operation succeeded.
        /// </summary>
        /// <param name="input">A string to convert.</param>
        /// <param name="value">
        /// When this method returns, contains the struct equivalent of the value contained 
        /// in input, if the conversion succeeded, or default(__type_injection__) if the conversion
        /// failed. The conversion fails if the input parameter is null or String.Empty, or is 
        /// not of the correct format. This parameter is passed uninitialized. 
        /// </param>
        /// <returns>True if input was converted successfully; otherwise, false.</returns>
        public static bool TryParse(string input, out __type_injection__ value)
        {
            try
            {
                value = Newtonsoft.Json.JsonConvert.DeserializeObject<__type_injection__>(input);
                return true;
            }
            catch { value = default(__type_injection__); return false; }
        }
        public static __type_injection__ Parse(string input)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<__type_injection__>(input);
        }
        /// <summary>
        /// Serializes this object to a Json string.
        /// </summary>
        /// <returns>The Json string serialized.</returns>
        public override string ToString()
        {
            return Serializer.ToString(this);
        }
    }
}

#pragma warning restore 162,168,649,660,661,1522

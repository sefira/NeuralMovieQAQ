
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

    public partial struct Person
    {

        ///<summary>
        ///The id of the cell.
        ///</summary>
        public long CellID;		
        ///<summary>
        ///Initializes a new cell of the type Person with the specified parameters.
        ///</summary>
        public Person(long cell_id, int age=default(int),long parent=default(long),string Name=null,byte Gender=default(byte),bool Married=default(bool),long Spouse=default(long),List<long> Act=null,List<long> Direct=null)
		{
            this.age = age;
            this.parent = parent;
            this.Name = Name;
            this.Gender = Gender;
            this.Married = Married;
            this.Spouse = Spouse;
            this.Act = Act;
            this.Direct = Direct;
		
CellID = cell_id;
		}
		
        ///<summary>
        ///Initializes a new instance of the Person class with the specified parameters.
        ///</summary>
        public Person(int age=default(int),long parent=default(long),string Name=null,byte Gender=default(byte),bool Married=default(bool),long Spouse=default(long),List<long> Act=null,List<long> Direct=null)
		{
            this.age = age;
            this.parent = parent;
            this.Name = Name;
            this.Gender = Gender;
            this.Married = Married;
            this.Spouse = Spouse;
            this.Act = Act;
            this.Direct = Direct;

		CellID = CellIDFactory.NewCellID();
		}

        public int age;

        public long parent;

        public string Name;

        public byte Gender;

        public bool Married;

        public long Spouse;

        public List<long> Act;

        public List<long> Direct;

        public static implicit operator Tuple<int,long,string,byte,bool,long,List<long>,List<long>>(Person FormatStruct)
        {
            return new Tuple<int,long,string,byte,bool,long,List<long>,List<long>>(FormatStruct.age,FormatStruct.parent,FormatStruct.Name,FormatStruct.Gender,FormatStruct.Married,FormatStruct.Spouse,FormatStruct.Act,FormatStruct.Direct);
        }

        public static implicit operator Person (Tuple<int,long,string,byte,bool,long,List<long>,List<long>>tuple)
        {
            return new Person(tuple.Item1,tuple.Item2,tuple.Item3,tuple.Item4,tuple.Item5,tuple.Item6,tuple.Item7,tuple.Rest);
        }

        public static bool operator == (Person a, Person b)
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
            return a.age == b.age && a.parent == b.parent && a.Name == b.Name && a.Gender == b.Gender && a.Married == b.Married && a.Spouse == b.Spouse && a.Act == b.Act && a.Direct == b.Direct;
        }

        public static bool operator != (Person a, Person b)
        {
            return !(a == b);
        }

    }

    public unsafe partial class Person_Accessor: IDisposable
    {
        internal Person_Accessor(long cellId, byte[] buffer)
        {
            this.CellID  = cellId;
            handle       = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            this.CellPtr = (byte*)handle.AddrOfPinnedObject().ToPointer();
        Name_Accessor_Field = new StringAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        Act_Accessor_Field = new longListAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        Direct_Accessor_Field = new longListAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

            this.CellEntryIndex = -1;
        }


        internal static string[] optional_field_names;
        ///<summary>
        ///Get an array of the names of all optional fields for object type Person.
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
        ///Copies the cell content into a byte array.
        ///</summary>
        public byte[] ToByteArray()
        {
            byte* targetPtr = CellPtr;
            targetPtr += 12;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 10;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;

            int size   = (int)(targetPtr - CellPtr);
            byte[] ret = new byte[size];
            Memory.Copy(CellPtr,0,ret,0,size);
            return ret;
        }

        internal unsafe Person_Accessor(long CellID, CellAccessOptions options)
        {
            this.Initialize(CellID, options);

        Name_Accessor_Field = new StringAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        Act_Accessor_Field = new longListAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        Direct_Accessor_Field = new longListAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

            this.CellID = CellID;
        }

        internal unsafe Person_Accessor(byte* _CellPtr)
        {
            CellPtr = _CellPtr;
        Name_Accessor_Field = new StringAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        Act_Accessor_Field = new longListAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        Direct_Accessor_Field = new longListAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

            this.CellEntryIndex = -1;
        }		internal static unsafe byte[] construct(long CellID, int age=default(int),long parent=default(long),string Name=null,byte Gender=default(byte),bool Married=default(bool),long Spouse=default(long),List<long> Act=null,List<long> Direct=null)
		{

        byte* targetPtr = null;
targetPtr += 4;
targetPtr += 8;

        if(Name!= null)
        {
            int strlen_0 = Name.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }
targetPtr += 1;
targetPtr += 1;
targetPtr += 8;

if(Act!= null)
{
    targetPtr += Act.Count*8+sizeof(int);
}else
{
    targetPtr += sizeof(int);
}


if(Direct!= null)
{
    targetPtr += Direct.Count*8+sizeof(int);
}else
{
    targetPtr += sizeof(int);
}


        byte[] tmpcell = new byte[(int)(targetPtr)];
        fixed(byte* tmpcellptr = tmpcell)
        {
            targetPtr = tmpcellptr;
*(int*)targetPtr = age;
            targetPtr += 4;
*(long*)targetPtr = parent;
            targetPtr += 8;

        if(Name!= null)
        {
            int strlen_0 = Name.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = Name)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }
*(byte*)targetPtr = Gender;
            targetPtr += 1;
*(bool*)targetPtr = Married;
            targetPtr += 1;
*(long*)targetPtr = Spouse;
            targetPtr += 8;

if(Act!= null)
{
    *(int*)targetPtr = Act.Count*8;
    targetPtr += sizeof(int);
    for(int iterator_0 = 0;iterator_0<Act.Count;++iterator_0)
    {
*(long*)targetPtr = Act[iterator_0];
            targetPtr += 8;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

if(Direct!= null)
{
    *(int*)targetPtr = Direct.Count*8;
    targetPtr += sizeof(int);
    for(int iterator_0 = 0;iterator_0<Direct.Count;++iterator_0)
    {
*(long*)targetPtr = Direct[iterator_0];
            targetPtr += 8;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

        }

            return tmpcell;
        }

        ///<summary>
        ///Provides in-place access to the object field age.
        ///</summary>
        public unsafe int age
        {
            get
            {
                
                byte* targetPtr = CellPtr;

                return *(int*)(targetPtr);
            }

            set
            {
                byte* targetPtr = CellPtr;

                *(int*)(targetPtr) = value;
            }

        }

        ///<summary>
        ///Provides in-place access to the object field parent.
        ///</summary>
        public unsafe long parent
        {
            get
            {
                
                byte* targetPtr = CellPtr;
            targetPtr += 4;

                return *(long*)(targetPtr);
            }

            set
            {
                byte* targetPtr = CellPtr;
            targetPtr += 4;

                *(long*)(targetPtr) = value;
            }

        }
StringAccessor Name_Accessor_Field;
        ///<summary>
        ///Provides in-place access to the object field Name.
        ///</summary>
        public unsafe StringAccessor Name
        {
            get
            {
                
                byte* targetPtr = CellPtr;
            targetPtr += 12;

                Name_Accessor_Field.CellPtr = targetPtr + 4;
                Name_Accessor_Field.CellID = this.CellID;
                return Name_Accessor_Field;
            }

            set
            {
                if ((object)value == null) throw new ArgumentNullException("The assigned variable is null.");
                byte* targetPtr = CellPtr;
            targetPtr += 12;

                Name_Accessor_Field.CellID = this.CellID;
                
              int length = *(int*)(value.CellPtr - 4);

                //senario: cell_a.inlinks = cell_b.inlinks,
                //the later part will invoke the Get, filling cell_b.inlinks(a inlink_accessor_fiedld)'storage CellID
                int oldlength = *(int*)targetPtr;
                if (value.CellID != Name_Accessor_Field.CellID)
                {
                    //if not in the same Cell
                    Name_Accessor_Field.CellPtr = Name_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                    Memory.Copy(value.CellPtr - 4, Name_Accessor_Field.CellPtr, length + 4);
                }
                else
                {
                    byte[] tmpcell = new byte[length + 4];
                    fixed (byte* tmpcellptr = tmpcell)
                    {                        
                        Memory.Copy(value.CellPtr - 4, tmpcellptr, length + 4);
                        Name_Accessor_Field.CellPtr = Name_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                        Memory.Copy(tmpcellptr, Name_Accessor_Field.CellPtr, length + 4);
                    }
                }
                Name_Accessor_Field.CellPtr += 4;
                
            }

    }

        ///<summary>
        ///Provides in-place access to the object field Gender.
        ///</summary>
        public unsafe byte Gender
        {
            get
            {
                
                byte* targetPtr = CellPtr;
            targetPtr += 12;
            targetPtr += 4 + *(int*)targetPtr;

                return *(byte*)(targetPtr);
            }

            set
            {
                byte* targetPtr = CellPtr;
            targetPtr += 12;
            targetPtr += 4 + *(int*)targetPtr;

                *(byte*)(targetPtr) = value;
            }

        }

        ///<summary>
        ///Provides in-place access to the object field Married.
        ///</summary>
        public unsafe bool Married
        {
            get
            {
                
                byte* targetPtr = CellPtr;
            targetPtr += 12;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 1;

                return *(bool*)(targetPtr);
            }

            set
            {
                byte* targetPtr = CellPtr;
            targetPtr += 12;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 1;

                *(bool*)(targetPtr) = value;
            }

        }

        ///<summary>
        ///Provides in-place access to the object field Spouse.
        ///</summary>
        public unsafe long Spouse
        {
            get
            {
                
                byte* targetPtr = CellPtr;
            targetPtr += 12;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 2;

                return *(long*)(targetPtr);
            }

            set
            {
                byte* targetPtr = CellPtr;
            targetPtr += 12;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 2;

                *(long*)(targetPtr) = value;
            }

        }
longListAccessor Act_Accessor_Field;
        ///<summary>
        ///Provides in-place access to the object field Act.
        ///</summary>
        public unsafe longListAccessor Act
        {
            get
            {
                
                byte* targetPtr = CellPtr;
            targetPtr += 12;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 10;

                Act_Accessor_Field.CellPtr = targetPtr + 4;
                Act_Accessor_Field.CellID = this.CellID;
                return Act_Accessor_Field;
            }

            set
            {
                if ((object)value == null) throw new ArgumentNullException("The assigned variable is null.");
                byte* targetPtr = CellPtr;
            targetPtr += 12;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 10;

                Act_Accessor_Field.CellID = this.CellID;
                
              int length = *(int*)(value.CellPtr - 4);

                //senario: cell_a.inlinks = cell_b.inlinks,
                //the later part will invoke the Get, filling cell_b.inlinks(a inlink_accessor_fiedld)'storage CellID
                int oldlength = *(int*)targetPtr;
                if (value.CellID != Act_Accessor_Field.CellID)
                {
                    //if not in the same Cell
                    Act_Accessor_Field.CellPtr = Act_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                    Memory.Copy(value.CellPtr - 4, Act_Accessor_Field.CellPtr, length + 4);
                }
                else
                {
                    byte[] tmpcell = new byte[length + 4];
                    fixed (byte* tmpcellptr = tmpcell)
                    {                        
                        Memory.Copy(value.CellPtr - 4, tmpcellptr, length + 4);
                        Act_Accessor_Field.CellPtr = Act_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                        Memory.Copy(tmpcellptr, Act_Accessor_Field.CellPtr, length + 4);
                    }
                }
                Act_Accessor_Field.CellPtr += 4;
                
            }

    }
longListAccessor Direct_Accessor_Field;
        ///<summary>
        ///Provides in-place access to the object field Direct.
        ///</summary>
        public unsafe longListAccessor Direct
        {
            get
            {
                
                byte* targetPtr = CellPtr;
            targetPtr += 12;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 10;
            targetPtr += 4 + *(int*)targetPtr;

                Direct_Accessor_Field.CellPtr = targetPtr + 4;
                Direct_Accessor_Field.CellID = this.CellID;
                return Direct_Accessor_Field;
            }

            set
            {
                if ((object)value == null) throw new ArgumentNullException("The assigned variable is null.");
                byte* targetPtr = CellPtr;
            targetPtr += 12;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 10;
            targetPtr += 4 + *(int*)targetPtr;

                Direct_Accessor_Field.CellID = this.CellID;
                
              int length = *(int*)(value.CellPtr - 4);

                //senario: cell_a.inlinks = cell_b.inlinks,
                //the later part will invoke the Get, filling cell_b.inlinks(a inlink_accessor_fiedld)'storage CellID
                int oldlength = *(int*)targetPtr;
                if (value.CellID != Direct_Accessor_Field.CellID)
                {
                    //if not in the same Cell
                    Direct_Accessor_Field.CellPtr = Direct_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                    Memory.Copy(value.CellPtr - 4, Direct_Accessor_Field.CellPtr, length + 4);
                }
                else
                {
                    byte[] tmpcell = new byte[length + 4];
                    fixed (byte* tmpcellptr = tmpcell)
                    {                        
                        Memory.Copy(value.CellPtr - 4, tmpcellptr, length + 4);
                        Direct_Accessor_Field.CellPtr = Direct_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                        Memory.Copy(tmpcellptr, Direct_Accessor_Field.CellPtr, length + 4);
                    }
                }
                Direct_Accessor_Field.CellPtr += 4;
                
            }

    }

        public static unsafe implicit operator Person(Person_Accessor accessor)
        {
            
            if(accessor.CellID != null)
            return new Person(accessor.CellID.Value,accessor.age,accessor.parent,accessor.Name,accessor.Gender,accessor.Married,accessor.Spouse,accessor.Act,accessor.Direct);
            else
            return new Person(accessor.age,accessor.parent,accessor.Name,accessor.Gender,accessor.Married,accessor.Spouse,accessor.Act,accessor.Direct);
        }

        public unsafe static implicit operator Person_Accessor(Person field)
        {  
            byte* targetPtr = null;
            
        {targetPtr += 4;
targetPtr += 8;

        if(field.Name!= null)
        {
            int strlen_1 = field.Name.Length * 2;
            targetPtr += strlen_1+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }
targetPtr += 1;
targetPtr += 1;
targetPtr += 8;

if(field.Act!= null)
{
    targetPtr += field.Act.Count*8+sizeof(int);
}else
{
    targetPtr += sizeof(int);
}


if(field.Direct!= null)
{
    targetPtr += field.Direct.Count*8+sizeof(int);
}else
{
    targetPtr += sizeof(int);
}


        }
            byte* tmpcellptr = BufferAllocator.AllocBuffer((int)targetPtr);
            Memory.memset(tmpcellptr, 0, (ulong)targetPtr);
            targetPtr = tmpcellptr;
        
        {*(int*)targetPtr = field.age;
            targetPtr += 4;
*(long*)targetPtr = field.parent;
            targetPtr += 8;

        if(field.Name!= null)
        {
            int strlen_1 = field.Name.Length * 2;
            *(int*)targetPtr = strlen_1;
            targetPtr += sizeof(int);
            fixed(char* pstr_1 = field.Name)
            {
                Memory.Copy(pstr_1, targetPtr, strlen_1);
                targetPtr += strlen_1;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }
*(byte*)targetPtr = field.Gender;
            targetPtr += 1;
*(bool*)targetPtr = field.Married;
            targetPtr += 1;
*(long*)targetPtr = field.Spouse;
            targetPtr += 8;

if(field.Act!= null)
{
    *(int*)targetPtr = field.Act.Count*8;
    targetPtr += sizeof(int);
    for(int iterator_1 = 0;iterator_1<field.Act.Count;++iterator_1)
    {
*(long*)targetPtr = field.Act[iterator_1];
            targetPtr += 8;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

if(field.Direct!= null)
{
    *(int*)targetPtr = field.Direct.Count*8;
    targetPtr += sizeof(int);
    for(int iterator_1 = 0;iterator_1<field.Direct.Count;++iterator_1)
    {
*(long*)targetPtr = field.Direct[iterator_1];
            targetPtr += 8;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

        }
            Person_Accessor ret = new Person_Accessor(tmpcellptr);
            ret.CellID = field.CellID;
            return ret;
        }


        public static bool operator == (Person_Accessor a, Person_Accessor b)
        {
            if (ReferenceEquals(a, b))
              return true;
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
              return false;
            // If both are same instance, return true.
            if (a.CellPtr == b.CellPtr) return true;
            byte* targetPtr = a.CellPtr;
            targetPtr += 12;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 10;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;

            int lengthA = (int)(targetPtr - a.CellPtr);
            targetPtr = b.CellPtr;
            targetPtr += 12;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 10;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;

            int lengthB = (int)(targetPtr - b.CellPtr);
            if(lengthA != lengthB) return false;
            return Memory.Compare(a.CellPtr,b.CellPtr,lengthA);
        }

        public static bool operator != (Person_Accessor a, Person_Accessor b)
        {
            return !(a == b);
        }
	}

}

#pragma warning restore 162,168,649,660,661,1522
#pragma warning disable 162,168,649,660,661,1522

namespace GraphEngineServer
{
    
    /*    */
    
    /*    */
    
    /*    */
    
    /*    */
    
    /*    */
    
    /// <summary>
    /// A .NET runtime object representation of Person defined in TSL.
    /// </summary>
    public partial struct Person : ICell
    {
        #region MUTE
        
        #endregion
        #region Text processing
        /// <summary>
        /// Converts the string representation of a Person to its
        /// struct equivalent. A return value indicates whether the 
        /// operation succeeded.
        /// </summary>
        /// <param name="input>A string to convert.</param>
        /// <param name="value">
        /// When this method returns, contains the struct equivalent of the value contained 
        /// in input, if the conversion succeeded, or default(Person) if the conversion
        /// failed. The conversion fails if the input parameter is null or String.Empty, or is 
        /// not of the correct format. This parameter is passed uninitialized.
        /// </param>
        /// <returns>
        /// True if input was converted successfully; otherwise, false.
        /// </returns>
        public static bool TryParse(string input, out Person value)
        {
            try
            {
                value = Newtonsoft.Json.JsonConvert.DeserializeObject<Person>(input);
                return true;
            }
            catch { value = default(Person); return false; }
        }
        public static Person Parse(string input)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<Person>(input);
        }
        ///<summary>Converts a Person to its string representation, in JSON format.</summary>
        ///<returns>A string representation of the Person.</returns>
        public override string ToString()
        {
            return Serializer.ToString(this);
        }
        #endregion
        internal static StringLookupTable FieldLookupTable = new StringLookupTable(
            
            "age"
            ,
            "parent"
            ,
            "Name"
            ,
            "Gender"
            ,
            "Married"
            ,
            "Spouse"
            ,
            "Act"
            ,
            "Direct"
            
            );
        internal static HashSet<string> AppendToFieldRerouteSet = new HashSet<string>()
        {
            
            "age"
            ,
            "parent"
            ,
            "Gender"
            ,
            "Married"
            ,
            "Spouse"
            ,
        };
        #region ICell implementation
        /// <summary>
        /// Get the field of the specified name in the cell.
        /// </summary>
        /// <typeparam name="T">
        /// The desired type that the field is supposed 
        /// to be intepreted as. Automatic type casting 
        /// will be attempted if the desired type is not 
        /// implicitly convertible from the type of the field.
        /// </typeparam>
        /// <param name="fieldName">The name of the target field.</param>
        /// <returns>The value of the field.</returns>
        public T GetField<T>(string fieldName)
        {
            switch (FieldLookupTable.Lookup(fieldName))
            {
                case -1:
                    Throw.undefined_field();
                    break;
                
                case 0:
                    return TypeConverter<T>.ConvertFrom_int(this.age);
                
                case 1:
                    return TypeConverter<T>.ConvertFrom_long(this.parent);
                
                case 2:
                    return TypeConverter<T>.ConvertFrom_string(this.Name);
                
                case 3:
                    return TypeConverter<T>.ConvertFrom_byte(this.Gender);
                
                case 4:
                    return TypeConverter<T>.ConvertFrom_bool(this.Married);
                
                case 5:
                    return TypeConverter<T>.ConvertFrom_long(this.Spouse);
                
                case 6:
                    return TypeConverter<T>.ConvertFrom_List_long(this.Act);
                
                case 7:
                    return TypeConverter<T>.ConvertFrom_List_long(this.Direct);
                
            }
            /* Should not reach here */
            throw new Exception("Internal error T5005");
        }
        /// <summary>
        /// Set the value of the target field.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the value.
        /// </typeparam>
        /// <param name="fieldName">The name of the target field.</param>
        /// <param name="value">
        /// The value of the field. Automatic type casting 
        /// will be attempted if the desired type is not 
        /// implicitly convertible from the type of the field.
        /// </param>
        public void SetField<T>(string fieldName, T value)
        {
            switch (FieldLookupTable.Lookup(fieldName))
            {
                case -1:
                    Throw.undefined_field();
                    break;
                
                case 0:
                    this.age = TypeConverter<T>.ConvertTo_int(value);
                    break;
                
                case 1:
                    this.parent = TypeConverter<T>.ConvertTo_long(value);
                    break;
                
                case 2:
                    this.Name = TypeConverter<T>.ConvertTo_string(value);
                    break;
                
                case 3:
                    this.Gender = TypeConverter<T>.ConvertTo_byte(value);
                    break;
                
                case 4:
                    this.Married = TypeConverter<T>.ConvertTo_bool(value);
                    break;
                
                case 5:
                    this.Spouse = TypeConverter<T>.ConvertTo_long(value);
                    break;
                
                case 6:
                    this.Act = TypeConverter<T>.ConvertTo_List_long(value);
                    break;
                
                case 7:
                    this.Direct = TypeConverter<T>.ConvertTo_List_long(value);
                    break;
                
                default:
                    Throw.data_type_incompatible_with_field(typeof(T).ToString());
                    break;
            }
        }
        /// <summary>
        /// Tells if a field with the given name exists in the current cell.
        /// </summary>
        /// <param name="fieldName">The name of the field.</param>
        /// <returns>The existence of the field.</returns>
        public bool ContainsField(string fieldName)
        {
            switch (FieldLookupTable.Lookup(fieldName))
            {
                
                case 0:
                    
                    return true;
                    
                case 1:
                    
                    return true;
                    
                case 2:
                    
                    return true;
                    
                case 3:
                    
                    return true;
                    
                case 4:
                    
                    return true;
                    
                case 5:
                    
                    return true;
                    
                case 6:
                    
                    return true;
                    
                case 7:
                    
                    return true;
                    
                default:
                    return false;
            }
        }
        /// <summary>
        /// Append <paramref name="value"/> to the target field. Note that if the target field
        /// is not appendable(string or list), calling this method is equivalent to <see cref="GraphEngineServer.GenericCellAccessor.SetField(string, T)"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the value.
        /// </typeparam>
        /// <param name="fieldName">The name of the target field.</param>
        /// <param name="value">The value to be appended. 
        /// If the value is incompatible with the element 
        /// type of the field, automatic type casting will be attempted.
        /// </param>
        public void AppendToField<T>(string fieldName, T value)
        {
            if (AppendToFieldRerouteSet.Contains(fieldName))
            {
                SetField(fieldName, value);
                return;
            }
            switch (FieldLookupTable.Lookup(fieldName))
            {
                case -1:
                    Throw.undefined_field();
                    break;
                
                case 2:
                    
                    {
                        if (this.Name == null)
                            this.Name = TypeConverter<T>.ConvertTo_string(value);
                        else
                            this.Name += TypeConverter<T>.ConvertTo_string(value);
                    }
                    
                    break;
                
                case 6:
                    
                    {
                        if (this.Act == null)
                            this.Act = new List<long>();
                        switch (TypeConverter<T>.GetConversionActionTo_List_long())
                        {
                            case TypeConversionAction.TC_ASSIGN:
                                foreach (var element in value as List<long>)
                                    this.Act.Add(element);
                                break;
                            case TypeConversionAction.TC_CONVERTLIST:
                            case TypeConversionAction.TC_ARRAYTOLIST:
                                foreach (var element in TypeConverter<T>.Enumerate_long(value))
                                    this.Act.Add(element);
                                break;
                            case TypeConversionAction.TC_WRAPINLIST:
                            case TypeConversionAction.TC_PARSESTRING:
                                this.Act.Add(TypeConverter<T>.ConvertTo_long(value));
                                break;
                            default:
                                Throw.data_type_incompatible_with_list(typeof(T).ToString());
                                break;
                        }
                    }
                    
                    break;
                
                case 7:
                    
                    {
                        if (this.Direct == null)
                            this.Direct = new List<long>();
                        switch (TypeConverter<T>.GetConversionActionTo_List_long())
                        {
                            case TypeConversionAction.TC_ASSIGN:
                                foreach (var element in value as List<long>)
                                    this.Direct.Add(element);
                                break;
                            case TypeConversionAction.TC_CONVERTLIST:
                            case TypeConversionAction.TC_ARRAYTOLIST:
                                foreach (var element in TypeConverter<T>.Enumerate_long(value))
                                    this.Direct.Add(element);
                                break;
                            case TypeConversionAction.TC_WRAPINLIST:
                            case TypeConversionAction.TC_PARSESTRING:
                                this.Direct.Add(TypeConverter<T>.ConvertTo_long(value));
                                break;
                            default:
                                Throw.data_type_incompatible_with_list(typeof(T).ToString());
                                break;
                        }
                    }
                    
                    break;
                
                default:
                    Throw.target__field_not_list();
                    break;
            }
        }
        long ICell.CellID { get { return CellID; } set { CellID = value; } }
        public IEnumerable<KeyValuePair<string, T>> SelectFields<T>(string attributeKey, string attributeValue)
        {
            switch (TypeConverter<T>.type_id)
            {
                
                case  0:
                    
                    if (StorageSchema.Person_descriptor.check_attribute(StorageSchema.Person_descriptor.Name, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Name", TypeConverter<T>.ConvertFrom_string(this.Name));
                    
                    if (StorageSchema.Person_descriptor.check_attribute(StorageSchema.Person_descriptor.Gender, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Gender", TypeConverter<T>.ConvertFrom_byte(this.Gender));
                    
                    break;
                
                case  1:
                    
                    if (StorageSchema.Person_descriptor.check_attribute(StorageSchema.Person_descriptor.age, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("age", TypeConverter<T>.ConvertFrom_int(this.age));
                    
                    if (StorageSchema.Person_descriptor.check_attribute(StorageSchema.Person_descriptor.parent, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("parent", TypeConverter<T>.ConvertFrom_long(this.parent));
                    
                    if (StorageSchema.Person_descriptor.check_attribute(StorageSchema.Person_descriptor.Name, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Name", TypeConverter<T>.ConvertFrom_string(this.Name));
                    
                    if (StorageSchema.Person_descriptor.check_attribute(StorageSchema.Person_descriptor.Gender, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Gender", TypeConverter<T>.ConvertFrom_byte(this.Gender));
                    
                    if (StorageSchema.Person_descriptor.check_attribute(StorageSchema.Person_descriptor.Married, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Married", TypeConverter<T>.ConvertFrom_bool(this.Married));
                    
                    if (StorageSchema.Person_descriptor.check_attribute(StorageSchema.Person_descriptor.Spouse, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Spouse", TypeConverter<T>.ConvertFrom_long(this.Spouse));
                    
                    break;
                
                case  2:
                    
                    if (StorageSchema.Person_descriptor.check_attribute(StorageSchema.Person_descriptor.age, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("age", TypeConverter<T>.ConvertFrom_int(this.age));
                    
                    if (StorageSchema.Person_descriptor.check_attribute(StorageSchema.Person_descriptor.Name, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Name", TypeConverter<T>.ConvertFrom_string(this.Name));
                    
                    if (StorageSchema.Person_descriptor.check_attribute(StorageSchema.Person_descriptor.Gender, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Gender", TypeConverter<T>.ConvertFrom_byte(this.Gender));
                    
                    break;
                
                case  3:
                    
                    if (StorageSchema.Person_descriptor.check_attribute(StorageSchema.Person_descriptor.age, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("age", TypeConverter<T>.ConvertFrom_int(this.age));
                    
                    if (StorageSchema.Person_descriptor.check_attribute(StorageSchema.Person_descriptor.parent, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("parent", TypeConverter<T>.ConvertFrom_long(this.parent));
                    
                    if (StorageSchema.Person_descriptor.check_attribute(StorageSchema.Person_descriptor.Name, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Name", TypeConverter<T>.ConvertFrom_string(this.Name));
                    
                    if (StorageSchema.Person_descriptor.check_attribute(StorageSchema.Person_descriptor.Gender, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Gender", TypeConverter<T>.ConvertFrom_byte(this.Gender));
                    
                    if (StorageSchema.Person_descriptor.check_attribute(StorageSchema.Person_descriptor.Spouse, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Spouse", TypeConverter<T>.ConvertFrom_long(this.Spouse));
                    
                    break;
                
                case  4:
                    
                    if (StorageSchema.Person_descriptor.check_attribute(StorageSchema.Person_descriptor.age, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("age", TypeConverter<T>.ConvertFrom_int(this.age));
                    
                    if (StorageSchema.Person_descriptor.check_attribute(StorageSchema.Person_descriptor.parent, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("parent", TypeConverter<T>.ConvertFrom_long(this.parent));
                    
                    if (StorageSchema.Person_descriptor.check_attribute(StorageSchema.Person_descriptor.Name, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Name", TypeConverter<T>.ConvertFrom_string(this.Name));
                    
                    if (StorageSchema.Person_descriptor.check_attribute(StorageSchema.Person_descriptor.Gender, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Gender", TypeConverter<T>.ConvertFrom_byte(this.Gender));
                    
                    if (StorageSchema.Person_descriptor.check_attribute(StorageSchema.Person_descriptor.Married, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Married", TypeConverter<T>.ConvertFrom_bool(this.Married));
                    
                    if (StorageSchema.Person_descriptor.check_attribute(StorageSchema.Person_descriptor.Spouse, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Spouse", TypeConverter<T>.ConvertFrom_long(this.Spouse));
                    
                    if (StorageSchema.Person_descriptor.check_attribute(StorageSchema.Person_descriptor.Act, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Act", TypeConverter<T>.ConvertFrom_List_long(this.Act));
                    
                    if (StorageSchema.Person_descriptor.check_attribute(StorageSchema.Person_descriptor.Direct, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Direct", TypeConverter<T>.ConvertFrom_List_long(this.Direct));
                    
                    break;
                
                case  5:
                    
                    if (StorageSchema.Person_descriptor.check_attribute(StorageSchema.Person_descriptor.age, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("age", TypeConverter<T>.ConvertFrom_int(this.age));
                    
                    if (StorageSchema.Person_descriptor.check_attribute(StorageSchema.Person_descriptor.parent, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("parent", TypeConverter<T>.ConvertFrom_long(this.parent));
                    
                    if (StorageSchema.Person_descriptor.check_attribute(StorageSchema.Person_descriptor.Name, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Name", TypeConverter<T>.ConvertFrom_string(this.Name));
                    
                    if (StorageSchema.Person_descriptor.check_attribute(StorageSchema.Person_descriptor.Gender, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Gender", TypeConverter<T>.ConvertFrom_byte(this.Gender));
                    
                    if (StorageSchema.Person_descriptor.check_attribute(StorageSchema.Person_descriptor.Spouse, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Spouse", TypeConverter<T>.ConvertFrom_long(this.Spouse));
                    
                    if (StorageSchema.Person_descriptor.check_attribute(StorageSchema.Person_descriptor.Act, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Act", TypeConverter<T>.ConvertFrom_List_long(this.Act));
                    
                    if (StorageSchema.Person_descriptor.check_attribute(StorageSchema.Person_descriptor.Direct, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Direct", TypeConverter<T>.ConvertFrom_List_long(this.Direct));
                    
                    break;
                
                case  6:
                    
                    if (StorageSchema.Person_descriptor.check_attribute(StorageSchema.Person_descriptor.age, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("age", TypeConverter<T>.ConvertFrom_int(this.age));
                    
                    if (StorageSchema.Person_descriptor.check_attribute(StorageSchema.Person_descriptor.parent, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("parent", TypeConverter<T>.ConvertFrom_long(this.parent));
                    
                    if (StorageSchema.Person_descriptor.check_attribute(StorageSchema.Person_descriptor.Name, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Name", TypeConverter<T>.ConvertFrom_string(this.Name));
                    
                    if (StorageSchema.Person_descriptor.check_attribute(StorageSchema.Person_descriptor.Gender, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Gender", TypeConverter<T>.ConvertFrom_byte(this.Gender));
                    
                    if (StorageSchema.Person_descriptor.check_attribute(StorageSchema.Person_descriptor.Married, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Married", TypeConverter<T>.ConvertFrom_bool(this.Married));
                    
                    if (StorageSchema.Person_descriptor.check_attribute(StorageSchema.Person_descriptor.Spouse, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Spouse", TypeConverter<T>.ConvertFrom_long(this.Spouse));
                    
                    if (StorageSchema.Person_descriptor.check_attribute(StorageSchema.Person_descriptor.Act, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Act", TypeConverter<T>.ConvertFrom_List_long(this.Act));
                    
                    if (StorageSchema.Person_descriptor.check_attribute(StorageSchema.Person_descriptor.Direct, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Direct", TypeConverter<T>.ConvertFrom_List_long(this.Direct));
                    
                    break;
                
                default:
                    Throw.incompatible_with_cell();
                    break;
            }
            yield break;
        }
        #region enumerate value constructs
        
        private IEnumerable<T> _enumerate_from_age<T>()
        {
            
                switch (TypeConverter<T>.type_id)
                {
                    
                    case  1:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_int(this.age);
                            
                        }
                        break;
                    
                    case  2:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_int(this.age);
                            
                        }
                        break;
                    
                    case  3:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_int(this.age);
                            
                        }
                        break;
                    
                    case  4:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_int(this.age);
                            
                        }
                        break;
                    
                    case  5:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_int(this.age);
                            
                        }
                        break;
                    
                    case  6:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_int(this.age);
                            
                        }
                        break;
                    
                    default:
                        Throw.incompatible_with_cell();
                        break;
                }
            yield break;
        }
        
        private IEnumerable<T> _enumerate_from_parent<T>()
        {
            
                switch (TypeConverter<T>.type_id)
                {
                    
                    case  1:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_long(this.parent);
                            
                        }
                        break;
                    
                    case  3:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_long(this.parent);
                            
                        }
                        break;
                    
                    case  4:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_long(this.parent);
                            
                        }
                        break;
                    
                    case  5:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_long(this.parent);
                            
                        }
                        break;
                    
                    case  6:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_long(this.parent);
                            
                        }
                        break;
                    
                    default:
                        Throw.incompatible_with_cell();
                        break;
                }
            yield break;
        }
        
        private IEnumerable<T> _enumerate_from_Name<T>()
        {
            
                switch (TypeConverter<T>.type_id)
                {
                    
                    case  0:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Name);
                            
                        }
                        break;
                    
                    case  1:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Name);
                            
                        }
                        break;
                    
                    case  2:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Name);
                            
                        }
                        break;
                    
                    case  3:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Name);
                            
                        }
                        break;
                    
                    case  4:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Name);
                            
                        }
                        break;
                    
                    case  5:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Name);
                            
                        }
                        break;
                    
                    case  6:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Name);
                            
                        }
                        break;
                    
                    default:
                        Throw.incompatible_with_cell();
                        break;
                }
            yield break;
        }
        
        private IEnumerable<T> _enumerate_from_Gender<T>()
        {
            
                switch (TypeConverter<T>.type_id)
                {
                    
                    case  0:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_byte(this.Gender);
                            
                        }
                        break;
                    
                    case  1:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_byte(this.Gender);
                            
                        }
                        break;
                    
                    case  2:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_byte(this.Gender);
                            
                        }
                        break;
                    
                    case  3:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_byte(this.Gender);
                            
                        }
                        break;
                    
                    case  4:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_byte(this.Gender);
                            
                        }
                        break;
                    
                    case  5:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_byte(this.Gender);
                            
                        }
                        break;
                    
                    case  6:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_byte(this.Gender);
                            
                        }
                        break;
                    
                    default:
                        Throw.incompatible_with_cell();
                        break;
                }
            yield break;
        }
        
        private IEnumerable<T> _enumerate_from_Married<T>()
        {
            
                switch (TypeConverter<T>.type_id)
                {
                    
                    case  1:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_bool(this.Married);
                            
                        }
                        break;
                    
                    case  4:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_bool(this.Married);
                            
                        }
                        break;
                    
                    case  6:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_bool(this.Married);
                            
                        }
                        break;
                    
                    default:
                        Throw.incompatible_with_cell();
                        break;
                }
            yield break;
        }
        
        private IEnumerable<T> _enumerate_from_Spouse<T>()
        {
            
                switch (TypeConverter<T>.type_id)
                {
                    
                    case  1:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_long(this.Spouse);
                            
                        }
                        break;
                    
                    case  3:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_long(this.Spouse);
                            
                        }
                        break;
                    
                    case  4:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_long(this.Spouse);
                            
                        }
                        break;
                    
                    case  5:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_long(this.Spouse);
                            
                        }
                        break;
                    
                    case  6:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_long(this.Spouse);
                            
                        }
                        break;
                    
                    default:
                        Throw.incompatible_with_cell();
                        break;
                }
            yield break;
        }
        
        private IEnumerable<T> _enumerate_from_Act<T>()
        {
            
                switch (TypeConverter<T>.type_id)
                {
                    
                    case  1:
                        {
                            
                            {
                                
                                var element0 = this.Act;
                                
                                foreach (var element1 in element0)
                                
                                {
                                    yield return TypeConverter<T>.ConvertFrom_long(element1);
                                }
                            }
                            
                        }
                        break;
                    
                    case  3:
                        {
                            
                            {
                                
                                var element0 = this.Act;
                                
                                foreach (var element1 in element0)
                                
                                {
                                    yield return TypeConverter<T>.ConvertFrom_long(element1);
                                }
                            }
                            
                        }
                        break;
                    
                    case  4:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_List_long(this.Act);
                            
                        }
                        break;
                    
                    case  5:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_List_long(this.Act);
                            
                        }
                        break;
                    
                    case  6:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_List_long(this.Act);
                            
                        }
                        break;
                    
                    default:
                        Throw.incompatible_with_cell();
                        break;
                }
            yield break;
        }
        
        private IEnumerable<T> _enumerate_from_Direct<T>()
        {
            
                switch (TypeConverter<T>.type_id)
                {
                    
                    case  1:
                        {
                            
                            {
                                
                                var element0 = this.Direct;
                                
                                foreach (var element1 in element0)
                                
                                {
                                    yield return TypeConverter<T>.ConvertFrom_long(element1);
                                }
                            }
                            
                        }
                        break;
                    
                    case  3:
                        {
                            
                            {
                                
                                var element0 = this.Direct;
                                
                                foreach (var element1 in element0)
                                
                                {
                                    yield return TypeConverter<T>.ConvertFrom_long(element1);
                                }
                            }
                            
                        }
                        break;
                    
                    case  4:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_List_long(this.Direct);
                            
                        }
                        break;
                    
                    case  5:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_List_long(this.Direct);
                            
                        }
                        break;
                    
                    case  6:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_List_long(this.Direct);
                            
                        }
                        break;
                    
                    default:
                        Throw.incompatible_with_cell();
                        break;
                }
            yield break;
        }
        
        private static StringLookupTable s_field_attribute_id_table = new StringLookupTable(
            
            "GraphEdge"
            ,
            "Index"
            
            );
        #endregion
        public IEnumerable<T> EnumerateField<T>(string fieldName)
        {
            switch (FieldLookupTable.Lookup(fieldName))
            {
                
                case 0:
                    return _enumerate_from_age<T>();
                
                case 1:
                    return _enumerate_from_parent<T>();
                
                case 2:
                    return _enumerate_from_Name<T>();
                
                case 3:
                    return _enumerate_from_Gender<T>();
                
                case 4:
                    return _enumerate_from_Married<T>();
                
                case 5:
                    return _enumerate_from_Spouse<T>();
                
                case 6:
                    return _enumerate_from_Act<T>();
                
                case 7:
                    return _enumerate_from_Direct<T>();
                
                default:
                    Throw.undefined_field();
                    return null;
            }
        }
        public IEnumerable<T> EnumerateValues<T>(string attributeKey, string attributeValue)
        {
            int attr_id;
            if (attributeKey == null)
            {
                
                foreach (var val in _enumerate_from_age<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_parent<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_Name<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_Gender<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_Married<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_Spouse<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_Act<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_Direct<T>())
                    yield return val;
                
            }
            else if (-1 != (attr_id = s_field_attribute_id_table.Lookup(attributeKey)))
            {
                switch (attr_id)
                {
                    
                    case  0:
                        
                        {
                            
                        }
                        
                        {
                            
                            {
                                if (attributeValue == null || attributeValue == "")
                                {
                                    foreach (var val in _enumerate_from_parent<T>())
                                        yield return val;
                                }
                            }
                            
                        }
                        
                        {
                            
                        }
                        
                        {
                            
                        }
                        
                        {
                            
                        }
                        
                        {
                            
                            {
                                if (attributeValue == null || attributeValue == "")
                                {
                                    foreach (var val in _enumerate_from_Spouse<T>())
                                        yield return val;
                                }
                            }
                            
                        }
                        
                        {
                            
                            {
                                if (attributeValue == null || attributeValue == "")
                                {
                                    foreach (var val in _enumerate_from_Act<T>())
                                        yield return val;
                                }
                            }
                            
                        }
                        
                        {
                            
                            {
                                if (attributeValue == null || attributeValue == "")
                                {
                                    foreach (var val in _enumerate_from_Direct<T>())
                                        yield return val;
                                }
                            }
                            
                        }
                        
                        break;
                    
                    case  1:
                        
                        {
                            
                        }
                        
                        {
                            
                        }
                        
                        {
                            
                            {
                                if (attributeValue == null || attributeValue == "")
                                {
                                    foreach (var val in _enumerate_from_Name<T>())
                                        yield return val;
                                }
                            }
                            
                        }
                        
                        {
                            
                        }
                        
                        {
                            
                        }
                        
                        {
                            
                        }
                        
                        {
                            
                        }
                        
                        {
                            
                        }
                        
                        break;
                    
                }
            }
            yield break;
        }
        #endregion
        #region Other interfaces
        string ITypeDescriptor.TypeName
        {
            get { return StorageSchema.s_cellTypeName_Person; }
        }
        Type ITypeDescriptor.Type
        {
            get { return StorageSchema.s_cellType_Person; }
        }
        bool ITypeDescriptor.IsOfType<T>()
        {
            return typeof(T) == StorageSchema.s_cellType_Person;
        }
        bool ITypeDescriptor.IsList()
        {
            return false;
        }
        IEnumerable<IFieldDescriptor> ICellDescriptor.GetFieldDescriptors()
        {
            return StorageSchema.Person.GetFieldDescriptors();
        }
        IAttributeCollection ICellDescriptor.GetFieldAttributes(string fieldName)
        {
            return StorageSchema.Person.GetFieldAttributes(fieldName);
        }
        string IAttributeCollection.GetAttributeValue(string attributeKey)
        {
            return StorageSchema.Person.GetAttributeValue(attributeKey);
        }
        IReadOnlyDictionary<string, string> IAttributeCollection.Attributes
        {
            get { return StorageSchema.Person.Attributes; }
        }
        IEnumerable<string> ICellDescriptor.GetFieldNames()
        {
            
            {
                yield return "age";
            }
            
            {
                yield return "parent";
            }
            
            {
                yield return "Name";
            }
            
            {
                yield return "Gender";
            }
            
            {
                yield return "Married";
            }
            
            {
                yield return "Spouse";
            }
            
            {
                yield return "Act";
            }
            
            {
                yield return "Direct";
            }
            
        }
        ushort ICellDescriptor.CellType
        {
            get
            {
                return (ushort)CellType.Person;
            }
        }
        #endregion
    }
    /// <summary>
    /// Provides in-place operations of Person defined in TSL.
    /// </summary>
    public unsafe partial class Person_Accessor : ICellAccessor
    {
        #region MUTE
        
        #endregion
        #region Fields
        /// <summary>
        /// Get a pointer to the underlying raw binary blob. Take caution when accessing data with
        /// the raw pointer, as no boundary checks are employed, and improper operations will cause data corruption and/or system crash.
        /// </summary>
        internal byte* CellPtr { get; set; }
        /// <summary>
        /// Get the size of the cell content, in bytes.
        /// </summary>
        public int CellSize { get { int size; Global.LocalStorage.LockedGetCellSize(this.CellID.Value, this.CellEntryIndex, out size); return size; } }
        /// <summary>
        /// Get the cell id. The value can be null when the id is undefined.
        /// </summary>
        public long? CellID { get; internal set; }
        internal    int         	  		CellEntryIndex;
        internal    bool        	  		m_IsIterator   = false;
        internal    CellAccessOptions 		m_options      = 0;
        private     GCHandle                handle;
        private     const CellAccessOptions c_WALFlags     = CellAccessOptions.StrongLogAhead | CellAccessOptions.WeakLogAhead;
        #endregion
        #region Internal
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe void Initialize(long cellId, CellAccessOptions options)
        {
            int     cellSize;
            ushort  cellType;
            byte*   cellPtr;
            int     cellEntryIndex;
            var eResult = Global.LocalStorage.GetLockedCellInfo(cellId, out cellSize, out cellType, out cellPtr, out cellEntryIndex);
            switch (eResult)
            {
                case TrinityErrorCode.E_CELL_NOT_FOUND:
                    {
                        if ((options & CellAccessOptions.ThrowExceptionOnCellNotFound) != 0)
                        {
                            Throw.cell_not_found(cellId);
                        }
                        else if ((options & CellAccessOptions.CreateNewOnCellNotFound) != 0)
                        {
                            byte[]  defaultContent    = construct(cellId);
                            int     size              = defaultContent.Length;
                            eResult                   = Global.LocalStorage.AddOrUse(cellId, defaultContent, ref size, (ushort)CellType.Person, out cellPtr, out cellEntryIndex);
                            if (eResult == TrinityErrorCode.E_WRONG_CELL_TYPE)
                            {
                                Throw.wrong_cell_type();
                            }
                        }
                        else if ((options & CellAccessOptions.ReturnNullOnCellNotFound) != 0)
                        {
                            cellPtr        = null; /** Which indicates initialization failure. */
                            cellEntryIndex = -1;
                        }
                        else
                        {
                            Throw.cell_not_found(cellId);
                        }
                        break;
                    }
                case TrinityErrorCode.E_SUCCESS:
                    {
                        if (cellType != (ushort)CellType.Person)
                        {
                            Global.LocalStorage.ReleaseCellLock(cellId, cellEntryIndex);
                            Throw.wrong_cell_type();
                        }
                        break;
                    }
                default:
                    throw new NotImplementedException();
            }
            this.CellID         = cellId;
            this.CellPtr        = cellPtr;
            this.CellEntryIndex = cellEntryIndex;
            this.m_options      = options;
        }
        [ThreadStatic]
        internal static Person_Accessor s_accessor = null;
        internal static Person_Accessor New(long CellID, CellAccessOptions options)
        {
            Person_Accessor ret = null;
            if (s_accessor != (Person_Accessor)null)
            {
                ret = s_accessor;
                ret.Initialize(CellID, options);
                s_accessor = null;
            }
            else
            {
                ret = new Person_Accessor(CellID, options);
            }
            if (ret.CellPtr == null)
            {
                s_accessor = ret;
                ret        = null;
            }
            return ret;
        }
        internal static Person_Accessor New(long CellId, byte* cellPtr, int entryIndex, CellAccessOptions options)
        {
            Person_Accessor ret = null;
            if (s_accessor != (Person_Accessor)null)
            {
                ret = s_accessor;
                s_accessor = null;
                ret.CellPtr = cellPtr;
            }
            else
            {
                ret = new Person_Accessor(cellPtr);
            }
            ret.CellID         = CellId;
            ret.CellEntryIndex = entryIndex;
            ret.m_options      = options;
            return ret;
        }
        internal unsafe byte* ResizeFunction(byte* ptr, int ptr_offset, int delta)
        {
            int offset = (int)(ptr - CellPtr) + ptr_offset;
            CellPtr    = Global.LocalStorage.ResizeCell((long)CellID, CellEntryIndex, offset, delta);
            return CellPtr + (offset - ptr_offset);
        }
        internal static Person_Accessor AllocIterativeAccessor(CellInfo info)
        {
            Person_Accessor ret = null;
            if (s_accessor != (Person_Accessor)null)
            {
                ret                = s_accessor;
                ret.CellPtr        = info.CellPtr;
                s_accessor         = null;
            }
            else
            {
                ret                = new Person_Accessor(info.CellPtr);
            }
            ret.CellEntryIndex = info.CellEntryIndex;
            ret.CellID         = info.CellId;
            ret.m_IsIterator   = true;
            return ret;
        }
        #endregion
        #region Public
        /// <summary>
        /// Dispose the accessor.
        /// If <c><see cref="Trinity.TrinityConfig.ReadOnly"/> == false</c>,
        /// the cell lock will be released.
        /// If write-ahead-log behavior is specified on <see cref="GraphEngineServer.StorageExtension_Person.UsePerson"/>,
        /// the changes will be committed to the write-ahead log.
        /// </summary>
        public void Dispose()
        {
            if (CellEntryIndex >= 0)
            {
                if ((m_options & c_WALFlags) != 0)
                {
                    LocalMemoryStorage.CWriteAheadLog(this.CellID.Value, this.CellPtr, this.CellSize, (ushort)CellType.Person, m_options);
                }
                if (!m_IsIterator)
                {
                    Global.LocalStorage.ReleaseCellLock(CellID.Value, CellEntryIndex);
                }
                if (s_accessor == (Person_Accessor)null)
                {
                    CellPtr        = null;
                    m_IsIterator   = false;
                    s_accessor     = this;
                }
            }
            if (handle != null && handle.IsAllocated)
                handle.Free();
        }
        /// <summary>
        /// Get the cell type id.
        /// </summary>
        /// <returns>A 16-bit unsigned integer representing the cell type id.</returns>
        public ushort GetCellType()
        {
            if (!CellID.HasValue)
            {
                Throw.cell_id_is_null();
            }
            ushort cellType;
            if (Global.LocalStorage.GetCellType(CellID.Value, out cellType) == TrinityErrorCode.E_CELL_NOT_FOUND)
            {
                Throw.cell_not_found();
            }
            return cellType;
        }
        /// <summary>Converts a Person_Accessor to its string representation, in JSON format.</summary>
        /// <returns>A string representation of the Person.</returns>
        public override string ToString()
        {
            return Serializer.ToString(this);
        }
        #endregion
        internal static StringLookupTable FieldLookupTable = new StringLookupTable(
            
            "age"
            ,
            "parent"
            ,
            "Name"
            ,
            "Gender"
            ,
            "Married"
            ,
            "Spouse"
            ,
            "Act"
            ,
            "Direct"
            
            );
        static HashSet<string> AppendToFieldRerouteSet = new HashSet<string>()
        {
            
            "age"
            ,
            "parent"
            ,
            "Gender"
            ,
            "Married"
            ,
            "Spouse"
            ,
        };
        #region ICell implementation
        public T GetField<T>(string fieldName)
        {
            int field_divider_idx = fieldName.IndexOf('.');
            if (-1 != field_divider_idx)
            {
                string field_name_string = fieldName.Substring(0, field_divider_idx);
                switch (FieldLookupTable.Lookup(field_name_string))
                {
                    case -1:
                        Throw.undefined_field();
                        break;
                    
                    default:
                        Throw.member_access_on_non_struct__field(field_name_string);
                        break;
                }
            }
            switch (FieldLookupTable.Lookup(fieldName))
            {
                case -1:
                    Throw.undefined_field();
                    break;
                
                case 0:
                    return TypeConverter<T>.ConvertFrom_int(this.age);
                
                case 1:
                    return TypeConverter<T>.ConvertFrom_long(this.parent);
                
                case 2:
                    return TypeConverter<T>.ConvertFrom_string(this.Name);
                
                case 3:
                    return TypeConverter<T>.ConvertFrom_byte(this.Gender);
                
                case 4:
                    return TypeConverter<T>.ConvertFrom_bool(this.Married);
                
                case 5:
                    return TypeConverter<T>.ConvertFrom_long(this.Spouse);
                
                case 6:
                    return TypeConverter<T>.ConvertFrom_List_long(this.Act);
                
                case 7:
                    return TypeConverter<T>.ConvertFrom_List_long(this.Direct);
                
            }
            /* Should not reach here */
            throw new Exception("Internal error T5005");
        }
        public void SetField<T>(string fieldName, T value)
        {
            int field_divider_idx = fieldName.IndexOf('.');
            if (-1 != field_divider_idx)
            {
                string field_name_string = fieldName.Substring(0, field_divider_idx);
                switch (FieldLookupTable.Lookup(field_name_string))
                {
                    case -1:
                        Throw.undefined_field();
                        break;
                    
                    default:
                        Throw.member_access_on_non_struct__field(field_name_string);
                        break;
                }
                return;
            }
            switch (FieldLookupTable.Lookup(fieldName))
            {
                case -1:
                    Throw.undefined_field();
                    break;
                
                case 0:
                    {
                        int conversion_result = TypeConverter<T>.ConvertTo_int(value);
                        
            {
                this.age = conversion_result;
            }
            
                    }
                    break;
                
                case 1:
                    {
                        long conversion_result = TypeConverter<T>.ConvertTo_long(value);
                        
            {
                this.parent = conversion_result;
            }
            
                    }
                    break;
                
                case 2:
                    {
                        string conversion_result = TypeConverter<T>.ConvertTo_string(value);
                        
            {
                this.Name = conversion_result;
            }
            
                    }
                    break;
                
                case 3:
                    {
                        byte conversion_result = TypeConverter<T>.ConvertTo_byte(value);
                        
            {
                this.Gender = conversion_result;
            }
            
                    }
                    break;
                
                case 4:
                    {
                        bool conversion_result = TypeConverter<T>.ConvertTo_bool(value);
                        
            {
                this.Married = conversion_result;
            }
            
                    }
                    break;
                
                case 5:
                    {
                        long conversion_result = TypeConverter<T>.ConvertTo_long(value);
                        
            {
                this.Spouse = conversion_result;
            }
            
                    }
                    break;
                
                case 6:
                    {
                        List<long> conversion_result = TypeConverter<T>.ConvertTo_List_long(value);
                        
            {
                this.Act = conversion_result;
            }
            
                    }
                    break;
                
                case 7:
                    {
                        List<long> conversion_result = TypeConverter<T>.ConvertTo_List_long(value);
                        
            {
                this.Direct = conversion_result;
            }
            
                    }
                    break;
                
            }
        }
        /// <summary>
        /// Tells if a field with the given name exists in the current cell.
        /// </summary>
        /// <param name="fieldName">The name of the field.</param>
        /// <returns>The existence of the field.</returns>
        public bool ContainsField(string fieldName)
        {
            switch (FieldLookupTable.Lookup(fieldName))
            {
                
                case 0:
                    
                    return true;
                    
                case 1:
                    
                    return true;
                    
                case 2:
                    
                    return true;
                    
                case 3:
                    
                    return true;
                    
                case 4:
                    
                    return true;
                    
                case 5:
                    
                    return true;
                    
                case 6:
                    
                    return true;
                    
                case 7:
                    
                    return true;
                    
                default:
                    return false;
            }
        }
        public void AppendToField<T>(string fieldName, T value)
        {
            if (AppendToFieldRerouteSet.Contains(fieldName))
            {
                SetField(fieldName, value);
                return;
            }
            switch (FieldLookupTable.Lookup(fieldName))
            {
                case -1:
                    Throw.undefined_field();
                    break;
                
                case 2:
                    
                    {
                        
                        this.Name += TypeConverter<T>.ConvertTo_string(value);
                    }
                    
                    break;
                
                case 6:
                    
                    {
                        
                        switch (TypeConverter<T>.GetConversionActionTo_List_long())
                        {
                            case TypeConversionAction.TC_ASSIGN:
                                foreach (var element in value as List<long>)
                                    this.Act.Add(element);
                                break;
                            case TypeConversionAction.TC_CONVERTLIST:
                            case TypeConversionAction.TC_ARRAYTOLIST:
                                foreach (var element in TypeConverter<T>.Enumerate_long(value))
                                    this.Act.Add(element);
                                break;
                            case TypeConversionAction.TC_WRAPINLIST:
                            case TypeConversionAction.TC_PARSESTRING:
                                this.Act.Add(TypeConverter<T>.ConvertTo_long(value));
                                break;
                            default:
                                Throw.data_type_incompatible_with_list(typeof(T).ToString());
                                break;
                        }
                    }
                    
                    break;
                
                case 7:
                    
                    {
                        
                        switch (TypeConverter<T>.GetConversionActionTo_List_long())
                        {
                            case TypeConversionAction.TC_ASSIGN:
                                foreach (var element in value as List<long>)
                                    this.Direct.Add(element);
                                break;
                            case TypeConversionAction.TC_CONVERTLIST:
                            case TypeConversionAction.TC_ARRAYTOLIST:
                                foreach (var element in TypeConverter<T>.Enumerate_long(value))
                                    this.Direct.Add(element);
                                break;
                            case TypeConversionAction.TC_WRAPINLIST:
                            case TypeConversionAction.TC_PARSESTRING:
                                this.Direct.Add(TypeConverter<T>.ConvertTo_long(value));
                                break;
                            default:
                                Throw.data_type_incompatible_with_list(typeof(T).ToString());
                                break;
                        }
                    }
                    
                    break;
                
                default:
                    Throw.target__field_not_list();
                    break;
            }
        }
        long ICell.CellID { get { return CellID.Value; } set { CellID = value; } }
        IEnumerable<KeyValuePair<string, T>> ICell.SelectFields<T>(string attributeKey, string attributeValue)
        {
            switch (TypeConverter<T>.type_id)
            {
                
                case  0:
                    
                    if (StorageSchema.Person_descriptor.check_attribute(StorageSchema.Person_descriptor.Name, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Name", TypeConverter<T>.ConvertFrom_string(this.Name));
                    
                    if (StorageSchema.Person_descriptor.check_attribute(StorageSchema.Person_descriptor.Gender, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Gender", TypeConverter<T>.ConvertFrom_byte(this.Gender));
                    
                    break;
                
                case  1:
                    
                    if (StorageSchema.Person_descriptor.check_attribute(StorageSchema.Person_descriptor.age, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("age", TypeConverter<T>.ConvertFrom_int(this.age));
                    
                    if (StorageSchema.Person_descriptor.check_attribute(StorageSchema.Person_descriptor.parent, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("parent", TypeConverter<T>.ConvertFrom_long(this.parent));
                    
                    if (StorageSchema.Person_descriptor.check_attribute(StorageSchema.Person_descriptor.Name, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Name", TypeConverter<T>.ConvertFrom_string(this.Name));
                    
                    if (StorageSchema.Person_descriptor.check_attribute(StorageSchema.Person_descriptor.Gender, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Gender", TypeConverter<T>.ConvertFrom_byte(this.Gender));
                    
                    if (StorageSchema.Person_descriptor.check_attribute(StorageSchema.Person_descriptor.Married, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Married", TypeConverter<T>.ConvertFrom_bool(this.Married));
                    
                    if (StorageSchema.Person_descriptor.check_attribute(StorageSchema.Person_descriptor.Spouse, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Spouse", TypeConverter<T>.ConvertFrom_long(this.Spouse));
                    
                    break;
                
                case  2:
                    
                    if (StorageSchema.Person_descriptor.check_attribute(StorageSchema.Person_descriptor.age, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("age", TypeConverter<T>.ConvertFrom_int(this.age));
                    
                    if (StorageSchema.Person_descriptor.check_attribute(StorageSchema.Person_descriptor.Name, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Name", TypeConverter<T>.ConvertFrom_string(this.Name));
                    
                    if (StorageSchema.Person_descriptor.check_attribute(StorageSchema.Person_descriptor.Gender, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Gender", TypeConverter<T>.ConvertFrom_byte(this.Gender));
                    
                    break;
                
                case  3:
                    
                    if (StorageSchema.Person_descriptor.check_attribute(StorageSchema.Person_descriptor.age, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("age", TypeConverter<T>.ConvertFrom_int(this.age));
                    
                    if (StorageSchema.Person_descriptor.check_attribute(StorageSchema.Person_descriptor.parent, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("parent", TypeConverter<T>.ConvertFrom_long(this.parent));
                    
                    if (StorageSchema.Person_descriptor.check_attribute(StorageSchema.Person_descriptor.Name, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Name", TypeConverter<T>.ConvertFrom_string(this.Name));
                    
                    if (StorageSchema.Person_descriptor.check_attribute(StorageSchema.Person_descriptor.Gender, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Gender", TypeConverter<T>.ConvertFrom_byte(this.Gender));
                    
                    if (StorageSchema.Person_descriptor.check_attribute(StorageSchema.Person_descriptor.Spouse, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Spouse", TypeConverter<T>.ConvertFrom_long(this.Spouse));
                    
                    break;
                
                case  4:
                    
                    if (StorageSchema.Person_descriptor.check_attribute(StorageSchema.Person_descriptor.age, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("age", TypeConverter<T>.ConvertFrom_int(this.age));
                    
                    if (StorageSchema.Person_descriptor.check_attribute(StorageSchema.Person_descriptor.parent, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("parent", TypeConverter<T>.ConvertFrom_long(this.parent));
                    
                    if (StorageSchema.Person_descriptor.check_attribute(StorageSchema.Person_descriptor.Name, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Name", TypeConverter<T>.ConvertFrom_string(this.Name));
                    
                    if (StorageSchema.Person_descriptor.check_attribute(StorageSchema.Person_descriptor.Gender, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Gender", TypeConverter<T>.ConvertFrom_byte(this.Gender));
                    
                    if (StorageSchema.Person_descriptor.check_attribute(StorageSchema.Person_descriptor.Married, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Married", TypeConverter<T>.ConvertFrom_bool(this.Married));
                    
                    if (StorageSchema.Person_descriptor.check_attribute(StorageSchema.Person_descriptor.Spouse, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Spouse", TypeConverter<T>.ConvertFrom_long(this.Spouse));
                    
                    if (StorageSchema.Person_descriptor.check_attribute(StorageSchema.Person_descriptor.Act, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Act", TypeConverter<T>.ConvertFrom_List_long(this.Act));
                    
                    if (StorageSchema.Person_descriptor.check_attribute(StorageSchema.Person_descriptor.Direct, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Direct", TypeConverter<T>.ConvertFrom_List_long(this.Direct));
                    
                    break;
                
                case  5:
                    
                    if (StorageSchema.Person_descriptor.check_attribute(StorageSchema.Person_descriptor.age, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("age", TypeConverter<T>.ConvertFrom_int(this.age));
                    
                    if (StorageSchema.Person_descriptor.check_attribute(StorageSchema.Person_descriptor.parent, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("parent", TypeConverter<T>.ConvertFrom_long(this.parent));
                    
                    if (StorageSchema.Person_descriptor.check_attribute(StorageSchema.Person_descriptor.Name, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Name", TypeConverter<T>.ConvertFrom_string(this.Name));
                    
                    if (StorageSchema.Person_descriptor.check_attribute(StorageSchema.Person_descriptor.Gender, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Gender", TypeConverter<T>.ConvertFrom_byte(this.Gender));
                    
                    if (StorageSchema.Person_descriptor.check_attribute(StorageSchema.Person_descriptor.Spouse, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Spouse", TypeConverter<T>.ConvertFrom_long(this.Spouse));
                    
                    if (StorageSchema.Person_descriptor.check_attribute(StorageSchema.Person_descriptor.Act, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Act", TypeConverter<T>.ConvertFrom_List_long(this.Act));
                    
                    if (StorageSchema.Person_descriptor.check_attribute(StorageSchema.Person_descriptor.Direct, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Direct", TypeConverter<T>.ConvertFrom_List_long(this.Direct));
                    
                    break;
                
                case  6:
                    
                    if (StorageSchema.Person_descriptor.check_attribute(StorageSchema.Person_descriptor.age, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("age", TypeConverter<T>.ConvertFrom_int(this.age));
                    
                    if (StorageSchema.Person_descriptor.check_attribute(StorageSchema.Person_descriptor.parent, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("parent", TypeConverter<T>.ConvertFrom_long(this.parent));
                    
                    if (StorageSchema.Person_descriptor.check_attribute(StorageSchema.Person_descriptor.Name, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Name", TypeConverter<T>.ConvertFrom_string(this.Name));
                    
                    if (StorageSchema.Person_descriptor.check_attribute(StorageSchema.Person_descriptor.Gender, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Gender", TypeConverter<T>.ConvertFrom_byte(this.Gender));
                    
                    if (StorageSchema.Person_descriptor.check_attribute(StorageSchema.Person_descriptor.Married, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Married", TypeConverter<T>.ConvertFrom_bool(this.Married));
                    
                    if (StorageSchema.Person_descriptor.check_attribute(StorageSchema.Person_descriptor.Spouse, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Spouse", TypeConverter<T>.ConvertFrom_long(this.Spouse));
                    
                    if (StorageSchema.Person_descriptor.check_attribute(StorageSchema.Person_descriptor.Act, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Act", TypeConverter<T>.ConvertFrom_List_long(this.Act));
                    
                    if (StorageSchema.Person_descriptor.check_attribute(StorageSchema.Person_descriptor.Direct, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Direct", TypeConverter<T>.ConvertFrom_List_long(this.Direct));
                    
                    break;
                
                default:
                    Throw.incompatible_with_cell();
                    break;
            }
            yield break;
        }
        #region enumerate value methods
        
        private IEnumerable<T> _enumerate_from_age<T>()
        {
            
                switch (TypeConverter<T>.type_id)
                {
                    
                    case  1:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_int(this.age);
                            
                        }
                        break;
                    
                    case  2:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_int(this.age);
                            
                        }
                        break;
                    
                    case  3:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_int(this.age);
                            
                        }
                        break;
                    
                    case  4:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_int(this.age);
                            
                        }
                        break;
                    
                    case  5:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_int(this.age);
                            
                        }
                        break;
                    
                    case  6:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_int(this.age);
                            
                        }
                        break;
                    
                    default:
                        Throw.incompatible_with_cell();
                        break;
                }
            yield break;
        }
        
        private IEnumerable<T> _enumerate_from_parent<T>()
        {
            
                switch (TypeConverter<T>.type_id)
                {
                    
                    case  1:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_long(this.parent);
                            
                        }
                        break;
                    
                    case  3:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_long(this.parent);
                            
                        }
                        break;
                    
                    case  4:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_long(this.parent);
                            
                        }
                        break;
                    
                    case  5:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_long(this.parent);
                            
                        }
                        break;
                    
                    case  6:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_long(this.parent);
                            
                        }
                        break;
                    
                    default:
                        Throw.incompatible_with_cell();
                        break;
                }
            yield break;
        }
        
        private IEnumerable<T> _enumerate_from_Name<T>()
        {
            
                switch (TypeConverter<T>.type_id)
                {
                    
                    case  0:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Name);
                            
                        }
                        break;
                    
                    case  1:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Name);
                            
                        }
                        break;
                    
                    case  2:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Name);
                            
                        }
                        break;
                    
                    case  3:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Name);
                            
                        }
                        break;
                    
                    case  4:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Name);
                            
                        }
                        break;
                    
                    case  5:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Name);
                            
                        }
                        break;
                    
                    case  6:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Name);
                            
                        }
                        break;
                    
                    default:
                        Throw.incompatible_with_cell();
                        break;
                }
            yield break;
        }
        
        private IEnumerable<T> _enumerate_from_Gender<T>()
        {
            
                switch (TypeConverter<T>.type_id)
                {
                    
                    case  0:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_byte(this.Gender);
                            
                        }
                        break;
                    
                    case  1:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_byte(this.Gender);
                            
                        }
                        break;
                    
                    case  2:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_byte(this.Gender);
                            
                        }
                        break;
                    
                    case  3:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_byte(this.Gender);
                            
                        }
                        break;
                    
                    case  4:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_byte(this.Gender);
                            
                        }
                        break;
                    
                    case  5:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_byte(this.Gender);
                            
                        }
                        break;
                    
                    case  6:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_byte(this.Gender);
                            
                        }
                        break;
                    
                    default:
                        Throw.incompatible_with_cell();
                        break;
                }
            yield break;
        }
        
        private IEnumerable<T> _enumerate_from_Married<T>()
        {
            
                switch (TypeConverter<T>.type_id)
                {
                    
                    case  1:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_bool(this.Married);
                            
                        }
                        break;
                    
                    case  4:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_bool(this.Married);
                            
                        }
                        break;
                    
                    case  6:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_bool(this.Married);
                            
                        }
                        break;
                    
                    default:
                        Throw.incompatible_with_cell();
                        break;
                }
            yield break;
        }
        
        private IEnumerable<T> _enumerate_from_Spouse<T>()
        {
            
                switch (TypeConverter<T>.type_id)
                {
                    
                    case  1:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_long(this.Spouse);
                            
                        }
                        break;
                    
                    case  3:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_long(this.Spouse);
                            
                        }
                        break;
                    
                    case  4:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_long(this.Spouse);
                            
                        }
                        break;
                    
                    case  5:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_long(this.Spouse);
                            
                        }
                        break;
                    
                    case  6:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_long(this.Spouse);
                            
                        }
                        break;
                    
                    default:
                        Throw.incompatible_with_cell();
                        break;
                }
            yield break;
        }
        
        private IEnumerable<T> _enumerate_from_Act<T>()
        {
            
                switch (TypeConverter<T>.type_id)
                {
                    
                    case  1:
                        {
                            
                            {
                                
                                var element0 = this.Act;
                                
                                foreach (var element1 in element0)
                                
                                {
                                    yield return TypeConverter<T>.ConvertFrom_long(element1);
                                }
                            }
                            
                        }
                        break;
                    
                    case  3:
                        {
                            
                            {
                                
                                var element0 = this.Act;
                                
                                foreach (var element1 in element0)
                                
                                {
                                    yield return TypeConverter<T>.ConvertFrom_long(element1);
                                }
                            }
                            
                        }
                        break;
                    
                    case  4:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_List_long(this.Act);
                            
                        }
                        break;
                    
                    case  5:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_List_long(this.Act);
                            
                        }
                        break;
                    
                    case  6:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_List_long(this.Act);
                            
                        }
                        break;
                    
                    default:
                        Throw.incompatible_with_cell();
                        break;
                }
            yield break;
        }
        
        private IEnumerable<T> _enumerate_from_Direct<T>()
        {
            
                switch (TypeConverter<T>.type_id)
                {
                    
                    case  1:
                        {
                            
                            {
                                
                                var element0 = this.Direct;
                                
                                foreach (var element1 in element0)
                                
                                {
                                    yield return TypeConverter<T>.ConvertFrom_long(element1);
                                }
                            }
                            
                        }
                        break;
                    
                    case  3:
                        {
                            
                            {
                                
                                var element0 = this.Direct;
                                
                                foreach (var element1 in element0)
                                
                                {
                                    yield return TypeConverter<T>.ConvertFrom_long(element1);
                                }
                            }
                            
                        }
                        break;
                    
                    case  4:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_List_long(this.Direct);
                            
                        }
                        break;
                    
                    case  5:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_List_long(this.Direct);
                            
                        }
                        break;
                    
                    case  6:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_List_long(this.Direct);
                            
                        }
                        break;
                    
                    default:
                        Throw.incompatible_with_cell();
                        break;
                }
            yield break;
        }
        
        private static StringLookupTable s_field_attribute_id_table = new StringLookupTable(
            
            "GraphEdge"
            ,
            "Index"
            
            );
        #endregion
        public IEnumerable<T> EnumerateField<T>(string fieldName)
        {
            switch (FieldLookupTable.Lookup(fieldName))
            {
                
                case 0:
                    return _enumerate_from_age<T>();
                
                case 1:
                    return _enumerate_from_parent<T>();
                
                case 2:
                    return _enumerate_from_Name<T>();
                
                case 3:
                    return _enumerate_from_Gender<T>();
                
                case 4:
                    return _enumerate_from_Married<T>();
                
                case 5:
                    return _enumerate_from_Spouse<T>();
                
                case 6:
                    return _enumerate_from_Act<T>();
                
                case 7:
                    return _enumerate_from_Direct<T>();
                
                default:
                    Throw.undefined_field();
                    return null;
            }
        }
        IEnumerable<T> ICell.EnumerateValues<T>(string attributeKey, string attributeValue)
        {
            int attr_id;
            if (attributeKey == null)
            {
                
                foreach (var val in _enumerate_from_age<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_parent<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_Name<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_Gender<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_Married<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_Spouse<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_Act<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_Direct<T>())
                    yield return val;
                
            }
            else if (-1 != (attr_id = s_field_attribute_id_table.Lookup(attributeKey)))
            {
                switch (attr_id)
                {
                    
                    case  0:
                        
                        {
                            
                        }
                        
                        {
                            
                            {
                                if (attributeValue == null || attributeValue == "")
                                {
                                    foreach (var val in _enumerate_from_parent<T>())
                                        yield return val;
                                }
                            }
                            
                        }
                        
                        {
                            
                        }
                        
                        {
                            
                        }
                        
                        {
                            
                        }
                        
                        {
                            
                            {
                                if (attributeValue == null || attributeValue == "")
                                {
                                    foreach (var val in _enumerate_from_Spouse<T>())
                                        yield return val;
                                }
                            }
                            
                        }
                        
                        {
                            
                            {
                                if (attributeValue == null || attributeValue == "")
                                {
                                    foreach (var val in _enumerate_from_Act<T>())
                                        yield return val;
                                }
                            }
                            
                        }
                        
                        {
                            
                            {
                                if (attributeValue == null || attributeValue == "")
                                {
                                    foreach (var val in _enumerate_from_Direct<T>())
                                        yield return val;
                                }
                            }
                            
                        }
                        
                        break;
                    
                    case  1:
                        
                        {
                            
                        }
                        
                        {
                            
                        }
                        
                        {
                            
                            {
                                if (attributeValue == null || attributeValue == "")
                                {
                                    foreach (var val in _enumerate_from_Name<T>())
                                        yield return val;
                                }
                            }
                            
                        }
                        
                        {
                            
                        }
                        
                        {
                            
                        }
                        
                        {
                            
                        }
                        
                        {
                            
                        }
                        
                        {
                            
                        }
                        
                        break;
                    
                }
            }
            yield break;
        }
        IEnumerable<string> ICellDescriptor.GetFieldNames()
        {
            
            {
                yield return "age";
            }
            
            {
                yield return "parent";
            }
            
            {
                yield return "Name";
            }
            
            {
                yield return "Gender";
            }
            
            {
                yield return "Married";
            }
            
            {
                yield return "Spouse";
            }
            
            {
                yield return "Act";
            }
            
            {
                yield return "Direct";
            }
            
        }
        IAttributeCollection ICellDescriptor.GetFieldAttributes(string fieldName)
        {
            return StorageSchema.Person.GetFieldAttributes(fieldName);
        }
        IEnumerable<IFieldDescriptor> ICellDescriptor.GetFieldDescriptors()
        {
            return StorageSchema.Person.GetFieldDescriptors();
        }
        string ITypeDescriptor.TypeName
        {
            get { return StorageSchema.s_cellTypeName_Person; }
        }
        Type ITypeDescriptor.Type
        {
            get { return StorageSchema.s_cellType_Person; }
        }
        bool ITypeDescriptor.IsOfType<T>()
        {
            return typeof(T) == StorageSchema.s_cellType_Person;
        }
        bool ITypeDescriptor.IsList()
        {
            return false;
        }
        IReadOnlyDictionary<string, string> IAttributeCollection.Attributes
        {
            get { return StorageSchema.Person.Attributes; }
        }
        string IAttributeCollection.GetAttributeValue(string attributeKey)
        {
            return StorageSchema.Person.GetAttributeValue(attributeKey);
        }
        ushort ICellDescriptor.CellType
        {
            get
            {
                return (ushort)CellType.Person;
            }
        }
        #endregion
    }
    
}

#pragma warning restore 162,168,649,660,661,1522

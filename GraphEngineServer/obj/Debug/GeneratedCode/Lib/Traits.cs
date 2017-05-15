#pragma warning disable 162,168,649,660,661,1522

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.TSL;
using Trinity.TSL.Lib;
namespace GraphEngineServer
{
    internal class TypeSystem
    {
        #region TypeID lookup table
        private static Dictionary<Type, uint> TypeIDLookupTable = new Dictionary<Type, uint>()
        {
            
            { typeof(byte), 0 }
            ,
            { typeof(bool), 1 }
            ,
            { typeof(int), 2 }
            ,
            { typeof(long), 3 }
            ,
            { typeof(string), 4 }
            ,
            { typeof(List<long>), 5 }
            ,
            { typeof(List<string>), 6 }
            ,
        };
        #endregion
        #region CellTypeID lookup table
        private static Dictionary<Type, uint> CellTypeIDLookupTable = new Dictionary<Type, uint>()
        {
            
            { typeof(Movie), 0 }
            ,
            { typeof(Person), 1 }
            
        };
        #endregion
        internal static uint GetTypeID(Type t)
        {
            uint type_id;
            if (!TypeIDLookupTable.TryGetValue(t, out type_id))
                type_id = uint.MaxValue;
            return type_id;
        }
        internal static uint GetCellTypeID(Type t)
        {
            uint type_id;
            if (!CellTypeIDLookupTable.TryGetValue(t, out type_id))
                throw new Exception("Type " + t.ToString() + " is not a cell.");
            return type_id;
        }
    }
    internal enum TypeConversionAction
    {
        TC_NONCONVERTIBLE = 0,
        TC_ASSIGN,
        TC_TOSTRING,
        TC_PARSESTRING,
        TC_TOBOOL,
        TC_CONVERTLIST,
        TC_WRAPINLIST,
        TC_ARRAYTOLIST,
        TC_EXTRACTNULLABLE,
    }
    internal interface ITypeConverter<T>
    {
        
        T ConvertFrom_byte(byte value);
        byte ConvertTo_byte(T value);
        TypeConversionAction GetConversionActionTo_byte();
        IEnumerable<byte> Enumerate_byte(T value);
        
        T ConvertFrom_bool(bool value);
        bool ConvertTo_bool(T value);
        TypeConversionAction GetConversionActionTo_bool();
        IEnumerable<bool> Enumerate_bool(T value);
        
        T ConvertFrom_int(int value);
        int ConvertTo_int(T value);
        TypeConversionAction GetConversionActionTo_int();
        IEnumerable<int> Enumerate_int(T value);
        
        T ConvertFrom_long(long value);
        long ConvertTo_long(T value);
        TypeConversionAction GetConversionActionTo_long();
        IEnumerable<long> Enumerate_long(T value);
        
        T ConvertFrom_string(string value);
        string ConvertTo_string(T value);
        TypeConversionAction GetConversionActionTo_string();
        IEnumerable<string> Enumerate_string(T value);
        
        T ConvertFrom_List_long(List<long> value);
        List<long> ConvertTo_List_long(T value);
        TypeConversionAction GetConversionActionTo_List_long();
        IEnumerable<List<long>> Enumerate_List_long(T value);
        
        T ConvertFrom_List_string(List<string> value);
        List<string> ConvertTo_List_string(T value);
        TypeConversionAction GetConversionActionTo_List_string();
        IEnumerable<List<string>> Enumerate_List_string(T value);
        
    }
    internal class TypeConverter<T> : ITypeConverter<T>
    {
        internal class _TypeConverterImpl : ITypeConverter<object>
            
            , ITypeConverter<byte>
        
            , ITypeConverter<bool>
        
            , ITypeConverter<int>
        
            , ITypeConverter<long>
        
            , ITypeConverter<string>
        
            , ITypeConverter<List<long>>
        
            , ITypeConverter<List<string>>
        
        {
            byte ITypeConverter<byte>.ConvertFrom_byte(byte value)
            {
                
                return (byte)value;
                
            }
            byte ITypeConverter<byte>.ConvertTo_byte(byte value)
            {
                return TypeConverter<byte>.ConvertFrom_byte(value);
            }
            TypeConversionAction ITypeConverter<byte>.GetConversionActionTo_byte()
            {
                
                return TypeConversionAction.TC_ASSIGN;
                
            }
            /// <summary>
            /// ONLY VALID FOR TC_CONVERTLIST AND TC_ARRAYTOLIST.
            /// </summary>
            IEnumerable<byte> ITypeConverter<byte>.Enumerate_byte(byte value)
            {
                
                yield break;
            }
            byte ITypeConverter<byte>.ConvertFrom_bool(bool value)
            {
                
                throw new InvalidCastException("Invalid cast from 'bool' to 'byte'.");
                
            }
            bool ITypeConverter<byte>.ConvertTo_bool(byte value)
            {
                return TypeConverter<bool>.ConvertFrom_byte(value);
            }
            TypeConversionAction ITypeConverter<byte>.GetConversionActionTo_bool()
            {
                
                return TypeConversionAction.TC_TOBOOL;
                
            }
            /// <summary>
            /// ONLY VALID FOR TC_CONVERTLIST AND TC_ARRAYTOLIST.
            /// </summary>
            IEnumerable<bool> ITypeConverter<byte>.Enumerate_bool(byte value)
            {
                
                yield break;
            }
            byte ITypeConverter<byte>.ConvertFrom_int(int value)
            {
                
                throw new InvalidCastException("Invalid cast from 'int' to 'byte'.");
                
            }
            int ITypeConverter<byte>.ConvertTo_int(byte value)
            {
                return TypeConverter<int>.ConvertFrom_byte(value);
            }
            TypeConversionAction ITypeConverter<byte>.GetConversionActionTo_int()
            {
                
                return TypeConversionAction.TC_ASSIGN;
                
            }
            /// <summary>
            /// ONLY VALID FOR TC_CONVERTLIST AND TC_ARRAYTOLIST.
            /// </summary>
            IEnumerable<int> ITypeConverter<byte>.Enumerate_int(byte value)
            {
                
                yield break;
            }
            byte ITypeConverter<byte>.ConvertFrom_long(long value)
            {
                
                throw new InvalidCastException("Invalid cast from 'long' to 'byte'.");
                
            }
            long ITypeConverter<byte>.ConvertTo_long(byte value)
            {
                return TypeConverter<long>.ConvertFrom_byte(value);
            }
            TypeConversionAction ITypeConverter<byte>.GetConversionActionTo_long()
            {
                
                return TypeConversionAction.TC_ASSIGN;
                
            }
            /// <summary>
            /// ONLY VALID FOR TC_CONVERTLIST AND TC_ARRAYTOLIST.
            /// </summary>
            IEnumerable<long> ITypeConverter<byte>.Enumerate_long(byte value)
            {
                
                yield break;
            }
            byte ITypeConverter<byte>.ConvertFrom_string(string value)
            {
                
                {
                    #region String parse
                    byte intermediate_result;
                    bool conversion_success;
                    
                    {
                        conversion_success = byte.TryParse(value, out intermediate_result);
                    }
                    
                    if (!conversion_success)
                    {
                        
                        Throw.cannot_parse(value, "byte");
                        
                    }
                    return intermediate_result;
                    #endregion
                }
                
            }
            string ITypeConverter<byte>.ConvertTo_string(byte value)
            {
                return TypeConverter<string>.ConvertFrom_byte(value);
            }
            TypeConversionAction ITypeConverter<byte>.GetConversionActionTo_string()
            {
                
                return TypeConversionAction.TC_TOSTRING;
                
            }
            /// <summary>
            /// ONLY VALID FOR TC_CONVERTLIST AND TC_ARRAYTOLIST.
            /// </summary>
            IEnumerable<string> ITypeConverter<byte>.Enumerate_string(byte value)
            {
                
                yield break;
            }
            byte ITypeConverter<byte>.ConvertFrom_List_long(List<long> value)
            {
                
                throw new InvalidCastException("Invalid cast from 'List<long>' to 'byte'.");
                
            }
            List<long> ITypeConverter<byte>.ConvertTo_List_long(byte value)
            {
                return TypeConverter<List<long>>.ConvertFrom_byte(value);
            }
            TypeConversionAction ITypeConverter<byte>.GetConversionActionTo_List_long()
            {
                
                return TypeConversionAction.TC_WRAPINLIST;
                
            }
            /// <summary>
            /// ONLY VALID FOR TC_CONVERTLIST AND TC_ARRAYTOLIST.
            /// </summary>
            IEnumerable<List<long>> ITypeConverter<byte>.Enumerate_List_long(byte value)
            {
                
                yield break;
            }
            byte ITypeConverter<byte>.ConvertFrom_List_string(List<string> value)
            {
                
                throw new InvalidCastException("Invalid cast from 'List<string>' to 'byte'.");
                
            }
            List<string> ITypeConverter<byte>.ConvertTo_List_string(byte value)
            {
                return TypeConverter<List<string>>.ConvertFrom_byte(value);
            }
            TypeConversionAction ITypeConverter<byte>.GetConversionActionTo_List_string()
            {
                
                return TypeConversionAction.TC_WRAPINLIST;
                
            }
            /// <summary>
            /// ONLY VALID FOR TC_CONVERTLIST AND TC_ARRAYTOLIST.
            /// </summary>
            IEnumerable<List<string>> ITypeConverter<byte>.Enumerate_List_string(byte value)
            {
                
                yield break;
            }
            bool ITypeConverter<bool>.ConvertFrom_byte(byte value)
            {
                
                return (value != 0);
                
            }
            byte ITypeConverter<bool>.ConvertTo_byte(bool value)
            {
                return TypeConverter<byte>.ConvertFrom_bool(value);
            }
            TypeConversionAction ITypeConverter<bool>.GetConversionActionTo_byte()
            {
                
                return TypeConversionAction.TC_NONCONVERTIBLE;
                
            }
            /// <summary>
            /// ONLY VALID FOR TC_CONVERTLIST AND TC_ARRAYTOLIST.
            /// </summary>
            IEnumerable<byte> ITypeConverter<bool>.Enumerate_byte(bool value)
            {
                
                yield break;
            }
            bool ITypeConverter<bool>.ConvertFrom_bool(bool value)
            {
                
                return (bool)value;
                
            }
            bool ITypeConverter<bool>.ConvertTo_bool(bool value)
            {
                return TypeConverter<bool>.ConvertFrom_bool(value);
            }
            TypeConversionAction ITypeConverter<bool>.GetConversionActionTo_bool()
            {
                
                return TypeConversionAction.TC_ASSIGN;
                
            }
            /// <summary>
            /// ONLY VALID FOR TC_CONVERTLIST AND TC_ARRAYTOLIST.
            /// </summary>
            IEnumerable<bool> ITypeConverter<bool>.Enumerate_bool(bool value)
            {
                
                yield break;
            }
            bool ITypeConverter<bool>.ConvertFrom_int(int value)
            {
                
                return (value != 0);
                
            }
            int ITypeConverter<bool>.ConvertTo_int(bool value)
            {
                return TypeConverter<int>.ConvertFrom_bool(value);
            }
            TypeConversionAction ITypeConverter<bool>.GetConversionActionTo_int()
            {
                
                return TypeConversionAction.TC_NONCONVERTIBLE;
                
            }
            /// <summary>
            /// ONLY VALID FOR TC_CONVERTLIST AND TC_ARRAYTOLIST.
            /// </summary>
            IEnumerable<int> ITypeConverter<bool>.Enumerate_int(bool value)
            {
                
                yield break;
            }
            bool ITypeConverter<bool>.ConvertFrom_long(long value)
            {
                
                return (value != 0);
                
            }
            long ITypeConverter<bool>.ConvertTo_long(bool value)
            {
                return TypeConverter<long>.ConvertFrom_bool(value);
            }
            TypeConversionAction ITypeConverter<bool>.GetConversionActionTo_long()
            {
                
                return TypeConversionAction.TC_NONCONVERTIBLE;
                
            }
            /// <summary>
            /// ONLY VALID FOR TC_CONVERTLIST AND TC_ARRAYTOLIST.
            /// </summary>
            IEnumerable<long> ITypeConverter<bool>.Enumerate_long(bool value)
            {
                
                yield break;
            }
            bool ITypeConverter<bool>.ConvertFrom_string(string value)
            {
                
                {
                    #region String parse
                    bool intermediate_result;
                    bool conversion_success;
                    
                    {
                        conversion_success = ExternalParser.TryParse_bool(value, out intermediate_result);
                    }
                    
                    if (!conversion_success)
                    {
                        
                        Throw.cannot_parse(value, "bool");
                        
                    }
                    return intermediate_result;
                    #endregion
                }
                
            }
            string ITypeConverter<bool>.ConvertTo_string(bool value)
            {
                return TypeConverter<string>.ConvertFrom_bool(value);
            }
            TypeConversionAction ITypeConverter<bool>.GetConversionActionTo_string()
            {
                
                return TypeConversionAction.TC_TOSTRING;
                
            }
            /// <summary>
            /// ONLY VALID FOR TC_CONVERTLIST AND TC_ARRAYTOLIST.
            /// </summary>
            IEnumerable<string> ITypeConverter<bool>.Enumerate_string(bool value)
            {
                
                yield break;
            }
            bool ITypeConverter<bool>.ConvertFrom_List_long(List<long> value)
            {
                
                throw new InvalidCastException("Invalid cast from 'List<long>' to 'bool'.");
                
            }
            List<long> ITypeConverter<bool>.ConvertTo_List_long(bool value)
            {
                return TypeConverter<List<long>>.ConvertFrom_bool(value);
            }
            TypeConversionAction ITypeConverter<bool>.GetConversionActionTo_List_long()
            {
                
                return TypeConversionAction.TC_NONCONVERTIBLE;
                
            }
            /// <summary>
            /// ONLY VALID FOR TC_CONVERTLIST AND TC_ARRAYTOLIST.
            /// </summary>
            IEnumerable<List<long>> ITypeConverter<bool>.Enumerate_List_long(bool value)
            {
                
                yield break;
            }
            bool ITypeConverter<bool>.ConvertFrom_List_string(List<string> value)
            {
                
                throw new InvalidCastException("Invalid cast from 'List<string>' to 'bool'.");
                
            }
            List<string> ITypeConverter<bool>.ConvertTo_List_string(bool value)
            {
                return TypeConverter<List<string>>.ConvertFrom_bool(value);
            }
            TypeConversionAction ITypeConverter<bool>.GetConversionActionTo_List_string()
            {
                
                return TypeConversionAction.TC_WRAPINLIST;
                
            }
            /// <summary>
            /// ONLY VALID FOR TC_CONVERTLIST AND TC_ARRAYTOLIST.
            /// </summary>
            IEnumerable<List<string>> ITypeConverter<bool>.Enumerate_List_string(bool value)
            {
                
                yield break;
            }
            int ITypeConverter<int>.ConvertFrom_byte(byte value)
            {
                
                return (int)value;
                
            }
            byte ITypeConverter<int>.ConvertTo_byte(int value)
            {
                return TypeConverter<byte>.ConvertFrom_int(value);
            }
            TypeConversionAction ITypeConverter<int>.GetConversionActionTo_byte()
            {
                
                return TypeConversionAction.TC_NONCONVERTIBLE;
                
            }
            /// <summary>
            /// ONLY VALID FOR TC_CONVERTLIST AND TC_ARRAYTOLIST.
            /// </summary>
            IEnumerable<byte> ITypeConverter<int>.Enumerate_byte(int value)
            {
                
                yield break;
            }
            int ITypeConverter<int>.ConvertFrom_bool(bool value)
            {
                
                throw new InvalidCastException("Invalid cast from 'bool' to 'int'.");
                
            }
            bool ITypeConverter<int>.ConvertTo_bool(int value)
            {
                return TypeConverter<bool>.ConvertFrom_int(value);
            }
            TypeConversionAction ITypeConverter<int>.GetConversionActionTo_bool()
            {
                
                return TypeConversionAction.TC_TOBOOL;
                
            }
            /// <summary>
            /// ONLY VALID FOR TC_CONVERTLIST AND TC_ARRAYTOLIST.
            /// </summary>
            IEnumerable<bool> ITypeConverter<int>.Enumerate_bool(int value)
            {
                
                yield break;
            }
            int ITypeConverter<int>.ConvertFrom_int(int value)
            {
                
                return (int)value;
                
            }
            int ITypeConverter<int>.ConvertTo_int(int value)
            {
                return TypeConverter<int>.ConvertFrom_int(value);
            }
            TypeConversionAction ITypeConverter<int>.GetConversionActionTo_int()
            {
                
                return TypeConversionAction.TC_ASSIGN;
                
            }
            /// <summary>
            /// ONLY VALID FOR TC_CONVERTLIST AND TC_ARRAYTOLIST.
            /// </summary>
            IEnumerable<int> ITypeConverter<int>.Enumerate_int(int value)
            {
                
                yield break;
            }
            int ITypeConverter<int>.ConvertFrom_long(long value)
            {
                
                throw new InvalidCastException("Invalid cast from 'long' to 'int'.");
                
            }
            long ITypeConverter<int>.ConvertTo_long(int value)
            {
                return TypeConverter<long>.ConvertFrom_int(value);
            }
            TypeConversionAction ITypeConverter<int>.GetConversionActionTo_long()
            {
                
                return TypeConversionAction.TC_ASSIGN;
                
            }
            /// <summary>
            /// ONLY VALID FOR TC_CONVERTLIST AND TC_ARRAYTOLIST.
            /// </summary>
            IEnumerable<long> ITypeConverter<int>.Enumerate_long(int value)
            {
                
                yield break;
            }
            int ITypeConverter<int>.ConvertFrom_string(string value)
            {
                
                {
                    #region String parse
                    int intermediate_result;
                    bool conversion_success;
                    
                    {
                        conversion_success = int.TryParse(value, out intermediate_result);
                    }
                    
                    if (!conversion_success)
                    {
                        
                        Throw.cannot_parse(value, "int");
                        
                    }
                    return intermediate_result;
                    #endregion
                }
                
            }
            string ITypeConverter<int>.ConvertTo_string(int value)
            {
                return TypeConverter<string>.ConvertFrom_int(value);
            }
            TypeConversionAction ITypeConverter<int>.GetConversionActionTo_string()
            {
                
                return TypeConversionAction.TC_TOSTRING;
                
            }
            /// <summary>
            /// ONLY VALID FOR TC_CONVERTLIST AND TC_ARRAYTOLIST.
            /// </summary>
            IEnumerable<string> ITypeConverter<int>.Enumerate_string(int value)
            {
                
                yield break;
            }
            int ITypeConverter<int>.ConvertFrom_List_long(List<long> value)
            {
                
                throw new InvalidCastException("Invalid cast from 'List<long>' to 'int'.");
                
            }
            List<long> ITypeConverter<int>.ConvertTo_List_long(int value)
            {
                return TypeConverter<List<long>>.ConvertFrom_int(value);
            }
            TypeConversionAction ITypeConverter<int>.GetConversionActionTo_List_long()
            {
                
                return TypeConversionAction.TC_WRAPINLIST;
                
            }
            /// <summary>
            /// ONLY VALID FOR TC_CONVERTLIST AND TC_ARRAYTOLIST.
            /// </summary>
            IEnumerable<List<long>> ITypeConverter<int>.Enumerate_List_long(int value)
            {
                
                yield break;
            }
            int ITypeConverter<int>.ConvertFrom_List_string(List<string> value)
            {
                
                throw new InvalidCastException("Invalid cast from 'List<string>' to 'int'.");
                
            }
            List<string> ITypeConverter<int>.ConvertTo_List_string(int value)
            {
                return TypeConverter<List<string>>.ConvertFrom_int(value);
            }
            TypeConversionAction ITypeConverter<int>.GetConversionActionTo_List_string()
            {
                
                return TypeConversionAction.TC_WRAPINLIST;
                
            }
            /// <summary>
            /// ONLY VALID FOR TC_CONVERTLIST AND TC_ARRAYTOLIST.
            /// </summary>
            IEnumerable<List<string>> ITypeConverter<int>.Enumerate_List_string(int value)
            {
                
                yield break;
            }
            long ITypeConverter<long>.ConvertFrom_byte(byte value)
            {
                
                return (long)value;
                
            }
            byte ITypeConverter<long>.ConvertTo_byte(long value)
            {
                return TypeConverter<byte>.ConvertFrom_long(value);
            }
            TypeConversionAction ITypeConverter<long>.GetConversionActionTo_byte()
            {
                
                return TypeConversionAction.TC_NONCONVERTIBLE;
                
            }
            /// <summary>
            /// ONLY VALID FOR TC_CONVERTLIST AND TC_ARRAYTOLIST.
            /// </summary>
            IEnumerable<byte> ITypeConverter<long>.Enumerate_byte(long value)
            {
                
                yield break;
            }
            long ITypeConverter<long>.ConvertFrom_bool(bool value)
            {
                
                throw new InvalidCastException("Invalid cast from 'bool' to 'long'.");
                
            }
            bool ITypeConverter<long>.ConvertTo_bool(long value)
            {
                return TypeConverter<bool>.ConvertFrom_long(value);
            }
            TypeConversionAction ITypeConverter<long>.GetConversionActionTo_bool()
            {
                
                return TypeConversionAction.TC_TOBOOL;
                
            }
            /// <summary>
            /// ONLY VALID FOR TC_CONVERTLIST AND TC_ARRAYTOLIST.
            /// </summary>
            IEnumerable<bool> ITypeConverter<long>.Enumerate_bool(long value)
            {
                
                yield break;
            }
            long ITypeConverter<long>.ConvertFrom_int(int value)
            {
                
                return (long)value;
                
            }
            int ITypeConverter<long>.ConvertTo_int(long value)
            {
                return TypeConverter<int>.ConvertFrom_long(value);
            }
            TypeConversionAction ITypeConverter<long>.GetConversionActionTo_int()
            {
                
                return TypeConversionAction.TC_NONCONVERTIBLE;
                
            }
            /// <summary>
            /// ONLY VALID FOR TC_CONVERTLIST AND TC_ARRAYTOLIST.
            /// </summary>
            IEnumerable<int> ITypeConverter<long>.Enumerate_int(long value)
            {
                
                yield break;
            }
            long ITypeConverter<long>.ConvertFrom_long(long value)
            {
                
                return (long)value;
                
            }
            long ITypeConverter<long>.ConvertTo_long(long value)
            {
                return TypeConverter<long>.ConvertFrom_long(value);
            }
            TypeConversionAction ITypeConverter<long>.GetConversionActionTo_long()
            {
                
                return TypeConversionAction.TC_ASSIGN;
                
            }
            /// <summary>
            /// ONLY VALID FOR TC_CONVERTLIST AND TC_ARRAYTOLIST.
            /// </summary>
            IEnumerable<long> ITypeConverter<long>.Enumerate_long(long value)
            {
                
                yield break;
            }
            long ITypeConverter<long>.ConvertFrom_string(string value)
            {
                
                {
                    #region String parse
                    long intermediate_result;
                    bool conversion_success;
                    
                    {
                        conversion_success = long.TryParse(value, out intermediate_result);
                    }
                    
                    if (!conversion_success)
                    {
                        
                        Throw.cannot_parse(value, "long");
                        
                    }
                    return intermediate_result;
                    #endregion
                }
                
            }
            string ITypeConverter<long>.ConvertTo_string(long value)
            {
                return TypeConverter<string>.ConvertFrom_long(value);
            }
            TypeConversionAction ITypeConverter<long>.GetConversionActionTo_string()
            {
                
                return TypeConversionAction.TC_TOSTRING;
                
            }
            /// <summary>
            /// ONLY VALID FOR TC_CONVERTLIST AND TC_ARRAYTOLIST.
            /// </summary>
            IEnumerable<string> ITypeConverter<long>.Enumerate_string(long value)
            {
                
                yield break;
            }
            long ITypeConverter<long>.ConvertFrom_List_long(List<long> value)
            {
                
                throw new InvalidCastException("Invalid cast from 'List<long>' to 'long'.");
                
            }
            List<long> ITypeConverter<long>.ConvertTo_List_long(long value)
            {
                return TypeConverter<List<long>>.ConvertFrom_long(value);
            }
            TypeConversionAction ITypeConverter<long>.GetConversionActionTo_List_long()
            {
                
                return TypeConversionAction.TC_WRAPINLIST;
                
            }
            /// <summary>
            /// ONLY VALID FOR TC_CONVERTLIST AND TC_ARRAYTOLIST.
            /// </summary>
            IEnumerable<List<long>> ITypeConverter<long>.Enumerate_List_long(long value)
            {
                
                yield break;
            }
            long ITypeConverter<long>.ConvertFrom_List_string(List<string> value)
            {
                
                throw new InvalidCastException("Invalid cast from 'List<string>' to 'long'.");
                
            }
            List<string> ITypeConverter<long>.ConvertTo_List_string(long value)
            {
                return TypeConverter<List<string>>.ConvertFrom_long(value);
            }
            TypeConversionAction ITypeConverter<long>.GetConversionActionTo_List_string()
            {
                
                return TypeConversionAction.TC_WRAPINLIST;
                
            }
            /// <summary>
            /// ONLY VALID FOR TC_CONVERTLIST AND TC_ARRAYTOLIST.
            /// </summary>
            IEnumerable<List<string>> ITypeConverter<long>.Enumerate_List_string(long value)
            {
                
                yield break;
            }
            string ITypeConverter<string>.ConvertFrom_byte(byte value)
            {
                
                return Serializer.ToString(value);
                
            }
            byte ITypeConverter<string>.ConvertTo_byte(string value)
            {
                return TypeConverter<byte>.ConvertFrom_string(value);
            }
            TypeConversionAction ITypeConverter<string>.GetConversionActionTo_byte()
            {
                
                return TypeConversionAction.TC_PARSESTRING;
                
            }
            /// <summary>
            /// ONLY VALID FOR TC_CONVERTLIST AND TC_ARRAYTOLIST.
            /// </summary>
            IEnumerable<byte> ITypeConverter<string>.Enumerate_byte(string value)
            {
                
                yield break;
            }
            string ITypeConverter<string>.ConvertFrom_bool(bool value)
            {
                
                return Serializer.ToString(value);
                
            }
            bool ITypeConverter<string>.ConvertTo_bool(string value)
            {
                return TypeConverter<bool>.ConvertFrom_string(value);
            }
            TypeConversionAction ITypeConverter<string>.GetConversionActionTo_bool()
            {
                
                return TypeConversionAction.TC_PARSESTRING;
                
            }
            /// <summary>
            /// ONLY VALID FOR TC_CONVERTLIST AND TC_ARRAYTOLIST.
            /// </summary>
            IEnumerable<bool> ITypeConverter<string>.Enumerate_bool(string value)
            {
                
                yield break;
            }
            string ITypeConverter<string>.ConvertFrom_int(int value)
            {
                
                return Serializer.ToString(value);
                
            }
            int ITypeConverter<string>.ConvertTo_int(string value)
            {
                return TypeConverter<int>.ConvertFrom_string(value);
            }
            TypeConversionAction ITypeConverter<string>.GetConversionActionTo_int()
            {
                
                return TypeConversionAction.TC_PARSESTRING;
                
            }
            /// <summary>
            /// ONLY VALID FOR TC_CONVERTLIST AND TC_ARRAYTOLIST.
            /// </summary>
            IEnumerable<int> ITypeConverter<string>.Enumerate_int(string value)
            {
                
                yield break;
            }
            string ITypeConverter<string>.ConvertFrom_long(long value)
            {
                
                return Serializer.ToString(value);
                
            }
            long ITypeConverter<string>.ConvertTo_long(string value)
            {
                return TypeConverter<long>.ConvertFrom_string(value);
            }
            TypeConversionAction ITypeConverter<string>.GetConversionActionTo_long()
            {
                
                return TypeConversionAction.TC_PARSESTRING;
                
            }
            /// <summary>
            /// ONLY VALID FOR TC_CONVERTLIST AND TC_ARRAYTOLIST.
            /// </summary>
            IEnumerable<long> ITypeConverter<string>.Enumerate_long(string value)
            {
                
                yield break;
            }
            string ITypeConverter<string>.ConvertFrom_string(string value)
            {
                
                return (string)value;
                
            }
            string ITypeConverter<string>.ConvertTo_string(string value)
            {
                return TypeConverter<string>.ConvertFrom_string(value);
            }
            TypeConversionAction ITypeConverter<string>.GetConversionActionTo_string()
            {
                
                return TypeConversionAction.TC_ASSIGN;
                
            }
            /// <summary>
            /// ONLY VALID FOR TC_CONVERTLIST AND TC_ARRAYTOLIST.
            /// </summary>
            IEnumerable<string> ITypeConverter<string>.Enumerate_string(string value)
            {
                
                yield break;
            }
            string ITypeConverter<string>.ConvertFrom_List_long(List<long> value)
            {
                
                return Serializer.ToString(value);
                
            }
            List<long> ITypeConverter<string>.ConvertTo_List_long(string value)
            {
                return TypeConverter<List<long>>.ConvertFrom_string(value);
            }
            TypeConversionAction ITypeConverter<string>.GetConversionActionTo_List_long()
            {
                
                return TypeConversionAction.TC_PARSESTRING;
                
            }
            /// <summary>
            /// ONLY VALID FOR TC_CONVERTLIST AND TC_ARRAYTOLIST.
            /// </summary>
            IEnumerable<List<long>> ITypeConverter<string>.Enumerate_List_long(string value)
            {
                
                yield break;
            }
            string ITypeConverter<string>.ConvertFrom_List_string(List<string> value)
            {
                
                return Serializer.ToString(value);
                
            }
            List<string> ITypeConverter<string>.ConvertTo_List_string(string value)
            {
                return TypeConverter<List<string>>.ConvertFrom_string(value);
            }
            TypeConversionAction ITypeConverter<string>.GetConversionActionTo_List_string()
            {
                
                return TypeConversionAction.TC_PARSESTRING;
                
            }
            /// <summary>
            /// ONLY VALID FOR TC_CONVERTLIST AND TC_ARRAYTOLIST.
            /// </summary>
            IEnumerable<List<string>> ITypeConverter<string>.Enumerate_List_string(string value)
            {
                
                yield break;
            }
            List<long> ITypeConverter<List<long>>.ConvertFrom_byte(byte value)
            {
                
                {
                    List<long> intermediate_result = new List<long>();
                    intermediate_result.Add(TypeConverter<long>.ConvertFrom_byte(value));
                    return intermediate_result;
                }
                
            }
            byte ITypeConverter<List<long>>.ConvertTo_byte(List<long> value)
            {
                return TypeConverter<byte>.ConvertFrom_List_long(value);
            }
            TypeConversionAction ITypeConverter<List<long>>.GetConversionActionTo_byte()
            {
                
                return TypeConversionAction.TC_NONCONVERTIBLE;
                
            }
            /// <summary>
            /// ONLY VALID FOR TC_CONVERTLIST AND TC_ARRAYTOLIST.
            /// </summary>
            IEnumerable<byte> ITypeConverter<List<long>>.Enumerate_byte(List<long> value)
            {
                
                yield break;
            }
            List<long> ITypeConverter<List<long>>.ConvertFrom_bool(bool value)
            {
                
                throw new InvalidCastException("Invalid cast from 'bool' to 'List<long>'.");
                
            }
            bool ITypeConverter<List<long>>.ConvertTo_bool(List<long> value)
            {
                return TypeConverter<bool>.ConvertFrom_List_long(value);
            }
            TypeConversionAction ITypeConverter<List<long>>.GetConversionActionTo_bool()
            {
                
                return TypeConversionAction.TC_NONCONVERTIBLE;
                
            }
            /// <summary>
            /// ONLY VALID FOR TC_CONVERTLIST AND TC_ARRAYTOLIST.
            /// </summary>
            IEnumerable<bool> ITypeConverter<List<long>>.Enumerate_bool(List<long> value)
            {
                
                foreach (var element in value)
                    yield return TypeConverter<bool>.ConvertFrom_long(element);
                
                yield break;
            }
            List<long> ITypeConverter<List<long>>.ConvertFrom_int(int value)
            {
                
                {
                    List<long> intermediate_result = new List<long>();
                    intermediate_result.Add(TypeConverter<long>.ConvertFrom_int(value));
                    return intermediate_result;
                }
                
            }
            int ITypeConverter<List<long>>.ConvertTo_int(List<long> value)
            {
                return TypeConverter<int>.ConvertFrom_List_long(value);
            }
            TypeConversionAction ITypeConverter<List<long>>.GetConversionActionTo_int()
            {
                
                return TypeConversionAction.TC_NONCONVERTIBLE;
                
            }
            /// <summary>
            /// ONLY VALID FOR TC_CONVERTLIST AND TC_ARRAYTOLIST.
            /// </summary>
            IEnumerable<int> ITypeConverter<List<long>>.Enumerate_int(List<long> value)
            {
                
                yield break;
            }
            List<long> ITypeConverter<List<long>>.ConvertFrom_long(long value)
            {
                
                {
                    List<long> intermediate_result = new List<long>();
                    intermediate_result.Add(TypeConverter<long>.ConvertFrom_long(value));
                    return intermediate_result;
                }
                
            }
            long ITypeConverter<List<long>>.ConvertTo_long(List<long> value)
            {
                return TypeConverter<long>.ConvertFrom_List_long(value);
            }
            TypeConversionAction ITypeConverter<List<long>>.GetConversionActionTo_long()
            {
                
                return TypeConversionAction.TC_NONCONVERTIBLE;
                
            }
            /// <summary>
            /// ONLY VALID FOR TC_CONVERTLIST AND TC_ARRAYTOLIST.
            /// </summary>
            IEnumerable<long> ITypeConverter<List<long>>.Enumerate_long(List<long> value)
            {
                
                foreach (var element in value)
                    yield return TypeConverter<long>.ConvertFrom_long(element);
                
                yield break;
            }
            List<long> ITypeConverter<List<long>>.ConvertFrom_string(string value)
            {
                
                {
                    #region String parse
                    List<long> intermediate_result;
                    bool conversion_success;
                    
                    {
                        conversion_success = ExternalParser.TryParse_List_long(value, out intermediate_result);
                    }
                    
                    if (!conversion_success)
                    {
                        
                        try
                        {
                            long element = TypeConverter<long>.ConvertFrom_string(value);
                            intermediate_result = new List<long>();
                            intermediate_result.Add(element);
                        }
                        catch
                        {
                            throw new ArgumentException("Cannot parse \"" + value + "\" into either 'List<long>' or 'long'.");
                        }
                        
                    }
                    return intermediate_result;
                    #endregion
                }
                
            }
            string ITypeConverter<List<long>>.ConvertTo_string(List<long> value)
            {
                return TypeConverter<string>.ConvertFrom_List_long(value);
            }
            TypeConversionAction ITypeConverter<List<long>>.GetConversionActionTo_string()
            {
                
                return TypeConversionAction.TC_TOSTRING;
                
            }
            /// <summary>
            /// ONLY VALID FOR TC_CONVERTLIST AND TC_ARRAYTOLIST.
            /// </summary>
            IEnumerable<string> ITypeConverter<List<long>>.Enumerate_string(List<long> value)
            {
                
                foreach (var element in value)
                    yield return TypeConverter<string>.ConvertFrom_long(element);
                
                yield break;
            }
            List<long> ITypeConverter<List<long>>.ConvertFrom_List_long(List<long> value)
            {
                
                return (List<long>)value;
                
            }
            List<long> ITypeConverter<List<long>>.ConvertTo_List_long(List<long> value)
            {
                return TypeConverter<List<long>>.ConvertFrom_List_long(value);
            }
            TypeConversionAction ITypeConverter<List<long>>.GetConversionActionTo_List_long()
            {
                
                return TypeConversionAction.TC_ASSIGN;
                
            }
            /// <summary>
            /// ONLY VALID FOR TC_CONVERTLIST AND TC_ARRAYTOLIST.
            /// </summary>
            IEnumerable<List<long>> ITypeConverter<List<long>>.Enumerate_List_long(List<long> value)
            {
                
                foreach (var element in value)
                    yield return TypeConverter<List<long>>.ConvertFrom_long(element);
                
                yield break;
            }
            List<long> ITypeConverter<List<long>>.ConvertFrom_List_string(List<string> value)
            {
                
                {
                    List<long> intermediate_result = new List<long>();
                    foreach (var element in value)
                    {
                        intermediate_result.Add(TypeConverter<long>.ConvertFrom_string(element));
                    }
                    return intermediate_result;
                }
                
            }
            List<string> ITypeConverter<List<long>>.ConvertTo_List_string(List<long> value)
            {
                return TypeConverter<List<string>>.ConvertFrom_List_long(value);
            }
            TypeConversionAction ITypeConverter<List<long>>.GetConversionActionTo_List_string()
            {
                
                return TypeConversionAction.TC_CONVERTLIST;
                
            }
            /// <summary>
            /// ONLY VALID FOR TC_CONVERTLIST AND TC_ARRAYTOLIST.
            /// </summary>
            IEnumerable<List<string>> ITypeConverter<List<long>>.Enumerate_List_string(List<long> value)
            {
                
                foreach (var element in value)
                    yield return TypeConverter<List<string>>.ConvertFrom_long(element);
                
                yield break;
            }
            List<string> ITypeConverter<List<string>>.ConvertFrom_byte(byte value)
            {
                
                {
                    List<string> intermediate_result = new List<string>();
                    intermediate_result.Add(TypeConverter<string>.ConvertFrom_byte(value));
                    return intermediate_result;
                }
                
            }
            byte ITypeConverter<List<string>>.ConvertTo_byte(List<string> value)
            {
                return TypeConverter<byte>.ConvertFrom_List_string(value);
            }
            TypeConversionAction ITypeConverter<List<string>>.GetConversionActionTo_byte()
            {
                
                return TypeConversionAction.TC_NONCONVERTIBLE;
                
            }
            /// <summary>
            /// ONLY VALID FOR TC_CONVERTLIST AND TC_ARRAYTOLIST.
            /// </summary>
            IEnumerable<byte> ITypeConverter<List<string>>.Enumerate_byte(List<string> value)
            {
                
                foreach (var element in value)
                    yield return TypeConverter<byte>.ConvertFrom_string(element);
                
                yield break;
            }
            List<string> ITypeConverter<List<string>>.ConvertFrom_bool(bool value)
            {
                
                {
                    List<string> intermediate_result = new List<string>();
                    intermediate_result.Add(TypeConverter<string>.ConvertFrom_bool(value));
                    return intermediate_result;
                }
                
            }
            bool ITypeConverter<List<string>>.ConvertTo_bool(List<string> value)
            {
                return TypeConverter<bool>.ConvertFrom_List_string(value);
            }
            TypeConversionAction ITypeConverter<List<string>>.GetConversionActionTo_bool()
            {
                
                return TypeConversionAction.TC_NONCONVERTIBLE;
                
            }
            /// <summary>
            /// ONLY VALID FOR TC_CONVERTLIST AND TC_ARRAYTOLIST.
            /// </summary>
            IEnumerable<bool> ITypeConverter<List<string>>.Enumerate_bool(List<string> value)
            {
                
                foreach (var element in value)
                    yield return TypeConverter<bool>.ConvertFrom_string(element);
                
                yield break;
            }
            List<string> ITypeConverter<List<string>>.ConvertFrom_int(int value)
            {
                
                {
                    List<string> intermediate_result = new List<string>();
                    intermediate_result.Add(TypeConverter<string>.ConvertFrom_int(value));
                    return intermediate_result;
                }
                
            }
            int ITypeConverter<List<string>>.ConvertTo_int(List<string> value)
            {
                return TypeConverter<int>.ConvertFrom_List_string(value);
            }
            TypeConversionAction ITypeConverter<List<string>>.GetConversionActionTo_int()
            {
                
                return TypeConversionAction.TC_NONCONVERTIBLE;
                
            }
            /// <summary>
            /// ONLY VALID FOR TC_CONVERTLIST AND TC_ARRAYTOLIST.
            /// </summary>
            IEnumerable<int> ITypeConverter<List<string>>.Enumerate_int(List<string> value)
            {
                
                foreach (var element in value)
                    yield return TypeConverter<int>.ConvertFrom_string(element);
                
                yield break;
            }
            List<string> ITypeConverter<List<string>>.ConvertFrom_long(long value)
            {
                
                {
                    List<string> intermediate_result = new List<string>();
                    intermediate_result.Add(TypeConverter<string>.ConvertFrom_long(value));
                    return intermediate_result;
                }
                
            }
            long ITypeConverter<List<string>>.ConvertTo_long(List<string> value)
            {
                return TypeConverter<long>.ConvertFrom_List_string(value);
            }
            TypeConversionAction ITypeConverter<List<string>>.GetConversionActionTo_long()
            {
                
                return TypeConversionAction.TC_NONCONVERTIBLE;
                
            }
            /// <summary>
            /// ONLY VALID FOR TC_CONVERTLIST AND TC_ARRAYTOLIST.
            /// </summary>
            IEnumerable<long> ITypeConverter<List<string>>.Enumerate_long(List<string> value)
            {
                
                foreach (var element in value)
                    yield return TypeConverter<long>.ConvertFrom_string(element);
                
                yield break;
            }
            List<string> ITypeConverter<List<string>>.ConvertFrom_string(string value)
            {
                
                {
                    #region String parse
                    List<string> intermediate_result;
                    bool conversion_success;
                    
                    {
                        conversion_success = ExternalParser.TryParse_List_string(value, out intermediate_result);
                    }
                    
                    if (!conversion_success)
                    {
                        
                        try
                        {
                            string element = TypeConverter<string>.ConvertFrom_string(value);
                            intermediate_result = new List<string>();
                            intermediate_result.Add(element);
                        }
                        catch
                        {
                            throw new ArgumentException("Cannot parse \"" + value + "\" into either 'List<string>' or 'string'.");
                        }
                        
                    }
                    return intermediate_result;
                    #endregion
                }
                
            }
            string ITypeConverter<List<string>>.ConvertTo_string(List<string> value)
            {
                return TypeConverter<string>.ConvertFrom_List_string(value);
            }
            TypeConversionAction ITypeConverter<List<string>>.GetConversionActionTo_string()
            {
                
                return TypeConversionAction.TC_TOSTRING;
                
            }
            /// <summary>
            /// ONLY VALID FOR TC_CONVERTLIST AND TC_ARRAYTOLIST.
            /// </summary>
            IEnumerable<string> ITypeConverter<List<string>>.Enumerate_string(List<string> value)
            {
                
                foreach (var element in value)
                    yield return TypeConverter<string>.ConvertFrom_string(element);
                
                yield break;
            }
            List<string> ITypeConverter<List<string>>.ConvertFrom_List_long(List<long> value)
            {
                
                {
                    List<string> intermediate_result = new List<string>();
                    foreach (var element in value)
                    {
                        intermediate_result.Add(TypeConverter<string>.ConvertFrom_long(element));
                    }
                    return intermediate_result;
                }
                
            }
            List<long> ITypeConverter<List<string>>.ConvertTo_List_long(List<string> value)
            {
                return TypeConverter<List<long>>.ConvertFrom_List_string(value);
            }
            TypeConversionAction ITypeConverter<List<string>>.GetConversionActionTo_List_long()
            {
                
                return TypeConversionAction.TC_CONVERTLIST;
                
            }
            /// <summary>
            /// ONLY VALID FOR TC_CONVERTLIST AND TC_ARRAYTOLIST.
            /// </summary>
            IEnumerable<List<long>> ITypeConverter<List<string>>.Enumerate_List_long(List<string> value)
            {
                
                foreach (var element in value)
                    yield return TypeConverter<List<long>>.ConvertFrom_string(element);
                
                yield break;
            }
            List<string> ITypeConverter<List<string>>.ConvertFrom_List_string(List<string> value)
            {
                
                return (List<string>)value;
                
            }
            List<string> ITypeConverter<List<string>>.ConvertTo_List_string(List<string> value)
            {
                return TypeConverter<List<string>>.ConvertFrom_List_string(value);
            }
            TypeConversionAction ITypeConverter<List<string>>.GetConversionActionTo_List_string()
            {
                
                return TypeConversionAction.TC_ASSIGN;
                
            }
            /// <summary>
            /// ONLY VALID FOR TC_CONVERTLIST AND TC_ARRAYTOLIST.
            /// </summary>
            IEnumerable<List<string>> ITypeConverter<List<string>>.Enumerate_List_string(List<string> value)
            {
                
                foreach (var element in value)
                    yield return TypeConverter<List<string>>.ConvertFrom_string(element);
                
                yield break;
            }
            
            object ITypeConverter<object>.ConvertFrom_byte(byte value)
            {
                return value;
            }
            byte ITypeConverter<object>.ConvertTo_byte(object value)
            {
                throw new NotImplementedException();
            }
            TypeConversionAction ITypeConverter<object>.GetConversionActionTo_byte()
            {
                throw new NotImplementedException();
            }
            IEnumerable<byte> ITypeConverter<object>.Enumerate_byte(object value)
            {
                throw new NotImplementedException();
            }
            
            object ITypeConverter<object>.ConvertFrom_bool(bool value)
            {
                return value;
            }
            bool ITypeConverter<object>.ConvertTo_bool(object value)
            {
                throw new NotImplementedException();
            }
            TypeConversionAction ITypeConverter<object>.GetConversionActionTo_bool()
            {
                throw new NotImplementedException();
            }
            IEnumerable<bool> ITypeConverter<object>.Enumerate_bool(object value)
            {
                throw new NotImplementedException();
            }
            
            object ITypeConverter<object>.ConvertFrom_int(int value)
            {
                return value;
            }
            int ITypeConverter<object>.ConvertTo_int(object value)
            {
                throw new NotImplementedException();
            }
            TypeConversionAction ITypeConverter<object>.GetConversionActionTo_int()
            {
                throw new NotImplementedException();
            }
            IEnumerable<int> ITypeConverter<object>.Enumerate_int(object value)
            {
                throw new NotImplementedException();
            }
            
            object ITypeConverter<object>.ConvertFrom_long(long value)
            {
                return value;
            }
            long ITypeConverter<object>.ConvertTo_long(object value)
            {
                throw new NotImplementedException();
            }
            TypeConversionAction ITypeConverter<object>.GetConversionActionTo_long()
            {
                throw new NotImplementedException();
            }
            IEnumerable<long> ITypeConverter<object>.Enumerate_long(object value)
            {
                throw new NotImplementedException();
            }
            
            object ITypeConverter<object>.ConvertFrom_string(string value)
            {
                return value;
            }
            string ITypeConverter<object>.ConvertTo_string(object value)
            {
                throw new NotImplementedException();
            }
            TypeConversionAction ITypeConverter<object>.GetConversionActionTo_string()
            {
                throw new NotImplementedException();
            }
            IEnumerable<string> ITypeConverter<object>.Enumerate_string(object value)
            {
                throw new NotImplementedException();
            }
            
            object ITypeConverter<object>.ConvertFrom_List_long(List<long> value)
            {
                return value;
            }
            List<long> ITypeConverter<object>.ConvertTo_List_long(object value)
            {
                throw new NotImplementedException();
            }
            TypeConversionAction ITypeConverter<object>.GetConversionActionTo_List_long()
            {
                throw new NotImplementedException();
            }
            IEnumerable<List<long>> ITypeConverter<object>.Enumerate_List_long(object value)
            {
                throw new NotImplementedException();
            }
            
            object ITypeConverter<object>.ConvertFrom_List_string(List<string> value)
            {
                return value;
            }
            List<string> ITypeConverter<object>.ConvertTo_List_string(object value)
            {
                throw new NotImplementedException();
            }
            TypeConversionAction ITypeConverter<object>.GetConversionActionTo_List_string()
            {
                throw new NotImplementedException();
            }
            IEnumerable<List<string>> ITypeConverter<object>.Enumerate_List_string(object value)
            {
                throw new NotImplementedException();
            }
            
        }
        internal static readonly ITypeConverter<T> s_type_converter = new _TypeConverterImpl() as ITypeConverter<T> ?? new TypeConverter<T>();
        #region Default implementation
        
        T ITypeConverter<T>.ConvertFrom_byte(byte value)
        {
            throw new NotImplementedException("Internal error T5013.");
        }
        byte ITypeConverter<T>.ConvertTo_byte(T value)
        {
            throw new NotImplementedException("Internal error T5013.");
        }
        TypeConversionAction ITypeConverter<T>.GetConversionActionTo_byte()
        {
            throw new NotImplementedException("Internal error T5013.");
        }
        IEnumerable<byte> ITypeConverter<T>.Enumerate_byte(T value)
        {
            throw new NotImplementedException("Internal error T5013.");
        }
        
        T ITypeConverter<T>.ConvertFrom_bool(bool value)
        {
            throw new NotImplementedException("Internal error T5013.");
        }
        bool ITypeConverter<T>.ConvertTo_bool(T value)
        {
            throw new NotImplementedException("Internal error T5013.");
        }
        TypeConversionAction ITypeConverter<T>.GetConversionActionTo_bool()
        {
            throw new NotImplementedException("Internal error T5013.");
        }
        IEnumerable<bool> ITypeConverter<T>.Enumerate_bool(T value)
        {
            throw new NotImplementedException("Internal error T5013.");
        }
        
        T ITypeConverter<T>.ConvertFrom_int(int value)
        {
            throw new NotImplementedException("Internal error T5013.");
        }
        int ITypeConverter<T>.ConvertTo_int(T value)
        {
            throw new NotImplementedException("Internal error T5013.");
        }
        TypeConversionAction ITypeConverter<T>.GetConversionActionTo_int()
        {
            throw new NotImplementedException("Internal error T5013.");
        }
        IEnumerable<int> ITypeConverter<T>.Enumerate_int(T value)
        {
            throw new NotImplementedException("Internal error T5013.");
        }
        
        T ITypeConverter<T>.ConvertFrom_long(long value)
        {
            throw new NotImplementedException("Internal error T5013.");
        }
        long ITypeConverter<T>.ConvertTo_long(T value)
        {
            throw new NotImplementedException("Internal error T5013.");
        }
        TypeConversionAction ITypeConverter<T>.GetConversionActionTo_long()
        {
            throw new NotImplementedException("Internal error T5013.");
        }
        IEnumerable<long> ITypeConverter<T>.Enumerate_long(T value)
        {
            throw new NotImplementedException("Internal error T5013.");
        }
        
        T ITypeConverter<T>.ConvertFrom_string(string value)
        {
            throw new NotImplementedException("Internal error T5013.");
        }
        string ITypeConverter<T>.ConvertTo_string(T value)
        {
            throw new NotImplementedException("Internal error T5013.");
        }
        TypeConversionAction ITypeConverter<T>.GetConversionActionTo_string()
        {
            throw new NotImplementedException("Internal error T5013.");
        }
        IEnumerable<string> ITypeConverter<T>.Enumerate_string(T value)
        {
            throw new NotImplementedException("Internal error T5013.");
        }
        
        T ITypeConverter<T>.ConvertFrom_List_long(List<long> value)
        {
            throw new NotImplementedException("Internal error T5013.");
        }
        List<long> ITypeConverter<T>.ConvertTo_List_long(T value)
        {
            throw new NotImplementedException("Internal error T5013.");
        }
        TypeConversionAction ITypeConverter<T>.GetConversionActionTo_List_long()
        {
            throw new NotImplementedException("Internal error T5013.");
        }
        IEnumerable<List<long>> ITypeConverter<T>.Enumerate_List_long(T value)
        {
            throw new NotImplementedException("Internal error T5013.");
        }
        
        T ITypeConverter<T>.ConvertFrom_List_string(List<string> value)
        {
            throw new NotImplementedException("Internal error T5013.");
        }
        List<string> ITypeConverter<T>.ConvertTo_List_string(T value)
        {
            throw new NotImplementedException("Internal error T5013.");
        }
        TypeConversionAction ITypeConverter<T>.GetConversionActionTo_List_string()
        {
            throw new NotImplementedException("Internal error T5013.");
        }
        IEnumerable<List<string>> ITypeConverter<T>.Enumerate_List_string(T value)
        {
            throw new NotImplementedException("Internal error T5013.");
        }
        
        #endregion
        internal static readonly uint type_id = TypeSystem.GetTypeID(typeof(T));
        
        internal static T ConvertFrom_byte(byte value)
        {
            return s_type_converter.ConvertFrom_byte(value);
        }
        internal static byte ConvertTo_byte(T value)
        {
            return s_type_converter.ConvertTo_byte(value);
        }
        internal static TypeConversionAction GetConversionActionTo_byte()
        {
            return s_type_converter.GetConversionActionTo_byte();
        }
        internal static IEnumerable<byte> Enumerate_byte(T value)
        {
            return s_type_converter.Enumerate_byte(value);
        }
        
        internal static T ConvertFrom_bool(bool value)
        {
            return s_type_converter.ConvertFrom_bool(value);
        }
        internal static bool ConvertTo_bool(T value)
        {
            return s_type_converter.ConvertTo_bool(value);
        }
        internal static TypeConversionAction GetConversionActionTo_bool()
        {
            return s_type_converter.GetConversionActionTo_bool();
        }
        internal static IEnumerable<bool> Enumerate_bool(T value)
        {
            return s_type_converter.Enumerate_bool(value);
        }
        
        internal static T ConvertFrom_int(int value)
        {
            return s_type_converter.ConvertFrom_int(value);
        }
        internal static int ConvertTo_int(T value)
        {
            return s_type_converter.ConvertTo_int(value);
        }
        internal static TypeConversionAction GetConversionActionTo_int()
        {
            return s_type_converter.GetConversionActionTo_int();
        }
        internal static IEnumerable<int> Enumerate_int(T value)
        {
            return s_type_converter.Enumerate_int(value);
        }
        
        internal static T ConvertFrom_long(long value)
        {
            return s_type_converter.ConvertFrom_long(value);
        }
        internal static long ConvertTo_long(T value)
        {
            return s_type_converter.ConvertTo_long(value);
        }
        internal static TypeConversionAction GetConversionActionTo_long()
        {
            return s_type_converter.GetConversionActionTo_long();
        }
        internal static IEnumerable<long> Enumerate_long(T value)
        {
            return s_type_converter.Enumerate_long(value);
        }
        
        internal static T ConvertFrom_string(string value)
        {
            return s_type_converter.ConvertFrom_string(value);
        }
        internal static string ConvertTo_string(T value)
        {
            return s_type_converter.ConvertTo_string(value);
        }
        internal static TypeConversionAction GetConversionActionTo_string()
        {
            return s_type_converter.GetConversionActionTo_string();
        }
        internal static IEnumerable<string> Enumerate_string(T value)
        {
            return s_type_converter.Enumerate_string(value);
        }
        
        internal static T ConvertFrom_List_long(List<long> value)
        {
            return s_type_converter.ConvertFrom_List_long(value);
        }
        internal static List<long> ConvertTo_List_long(T value)
        {
            return s_type_converter.ConvertTo_List_long(value);
        }
        internal static TypeConversionAction GetConversionActionTo_List_long()
        {
            return s_type_converter.GetConversionActionTo_List_long();
        }
        internal static IEnumerable<List<long>> Enumerate_List_long(T value)
        {
            return s_type_converter.Enumerate_List_long(value);
        }
        
        internal static T ConvertFrom_List_string(List<string> value)
        {
            return s_type_converter.ConvertFrom_List_string(value);
        }
        internal static List<string> ConvertTo_List_string(T value)
        {
            return s_type_converter.ConvertTo_List_string(value);
        }
        internal static TypeConversionAction GetConversionActionTo_List_string()
        {
            return s_type_converter.GetConversionActionTo_List_string();
        }
        internal static IEnumerable<List<string>> Enumerate_List_string(T value)
        {
            return s_type_converter.Enumerate_List_string(value);
        }
        
    }
}

#pragma warning restore 162,168,649,660,661,1522

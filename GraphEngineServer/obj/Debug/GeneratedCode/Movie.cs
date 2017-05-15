
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

    public unsafe class StringAccessorListAccessor : IEnumerable<StringAccessor>
    {
        internal byte* CellPtr;
        internal long? CellID;
        internal ResizeFunctionDelegate ResizeFunction;

        internal StringAccessorListAccessor(byte* _CellPtr,ResizeFunctionDelegate func)
        {
            CellPtr = _CellPtr;
            ResizeFunction = func;
            CellPtr += 4;

        elementAccessor = new StringAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.CellPtr = this.ResizeFunction(this.CellPtr-sizeof(int), ptr_offset + substructure_offset +sizeof(int), delta);
                    *(int*)this.CellPtr += delta;
                    this.CellPtr += sizeof(int);
                    return this.CellPtr + substructure_offset;
                });

        }

        internal int length
        {
            get
            {
                return *(int*)(CellPtr-4);
            }
        }
StringAccessor elementAccessor ;
        /// <summary>
        /// Gets the number of elements actually contained in the List. 
        /// </summary>
        public unsafe int Count
        {
            get
            {
                byte* targetPtr = CellPtr;
                byte* endPtr = CellPtr + length;
                int ret = 0;
                while(targetPtr < endPtr)
                {
targetPtr += 4 + *(int*)targetPtr;

                    ++ret;
                }
                return ret;
            }
        }


        /// <summary>
        /// Gets or sets the element at the specified index. 
        /// </summary>
        /// <param name="index">Given index</param>
        /// <returns>Corresponding element at the specified index</returns>
        public unsafe StringAccessor this[int index]
        {
            get
            {
                byte* targetPtr = CellPtr;
                while(index-- > 0)
                {targetPtr += 4 + *(int*)targetPtr;

                }elementAccessor.CellPtr = targetPtr + 4;elementAccessor.CellID = this.CellID;
                return elementAccessor;
            }
            set
            {
                if ((object)value == null) throw new ArgumentNullException("The assigned variable is null.");

                byte* targetPtr = CellPtr;
                while(index-- > 0)
                {targetPtr += 4 + *(int*)targetPtr;

                }elementAccessor.CellID = this.CellID;
              int length = *(int*)(value.CellPtr - 4);
                int offset = (int)(targetPtr-CellPtr);
                int oldlength = *(int*)targetPtr;
                if (value.CellID != this.CellID)
                {
                    //if not in the same Cell
                  this.CellPtr = elementAccessor.ResizeFunction(this.CellPtr, (int)(targetPtr-CellPtr), length - oldlength);
                    Memory.Copy(value.CellPtr - 4, this.CellPtr+offset, length + 4);
                }
                else
                {
                    byte[] tmpcell = new byte[length + 4];
                    fixed (byte* tmpcellptr = tmpcell)
                    {                        
                        Memory.Copy(value.CellPtr - 4, tmpcellptr, length + 4);
                        this.CellPtr = elementAccessor.ResizeFunction(this.CellPtr,(int)(targetPtr-CellPtr), length - oldlength);
                        Memory.Copy(tmpcellptr, this.CellPtr + offset, length + 4);
                    }
                }
                
            }
        }

        /// <summary>
        /// Copies the elements to a new byte array
        /// </summary>
        /// <returns>Elements compactly arranged in a byte array.</returns>
        public unsafe byte[] ToByteArray()
        {
            byte[] ret = new byte[length];
            fixed (byte* retptr = ret)
            {
                Memory.Copy(CellPtr, retptr, length);
                return ret;
            }
        }

        /// <summary>
        /// Performs the specified action on each elements
        /// </summary>
        /// <param name="action">A lambda expression which has one parameter indicates element in List</param>
        public unsafe void ForEach(Action<StringAccessor> action)
        {
            byte* targetPtr = CellPtr;
            byte* endPtr = CellPtr + length;
            while( targetPtr < endPtr )
            {
                elementAccessor.CellPtr = targetPtr + 4;
                action(elementAccessor);
                targetPtr += 4 + *(int*)targetPtr;

            }
        }
        /// <summary>
        /// Performs the specified action on each elements
        /// </summary>
        /// <param name="action">A lambda expression which has two parameters. First indicates element in the List and second the index of this element.</param>
        public unsafe void ForEach(Action<StringAccessor,int> action)
        {
            byte* targetPtr = CellPtr;
            byte* endPtr = CellPtr + length;
            for(int index=0; targetPtr < endPtr;++index )
            {
                elementAccessor.CellPtr = targetPtr + 4;
                action(elementAccessor,index);
                targetPtr += 4 + *(int*)targetPtr;

            }
        }
        internal unsafe struct _iterator
        {
            byte* targetPtr;
            byte* endPtr;
            StringAccessorListAccessor target;
            internal _iterator(StringAccessorListAccessor target)
            {
                targetPtr = target.CellPtr;
                endPtr = targetPtr + target.length;
                this.target = target;
            }
            internal bool good()
            {
                return (targetPtr < endPtr);
            }
            internal StringAccessor current()
            {
                
                target.elementAccessor.CellPtr = targetPtr + 4;
                return (target.elementAccessor);
                
            }
            internal void move_next()
            {
                targetPtr += 4 + *(int*)targetPtr;

            }
        }
        public IEnumerator<StringAccessor> GetEnumerator()
        {
            _iterator _it = new _iterator(this);
            while(_it.good())
            {
                yield return _it.current();
                _it.move_next();
            }
        }
        unsafe IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        /// <summary>
        /// Adds an item to the end of the List
        /// </summary>
        /// <param name="element">The object to be added to the end of the List.</param>
        public unsafe void Add(string element)
        {
            byte* targetPtr = null;
            {
            
        if(element!= null)
        {
            int strlen_0 = element.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

            }
            int size = (int)targetPtr;
            this.CellPtr = this.ResizeFunction(this.CellPtr - sizeof(int), *(int*)(this.CellPtr-sizeof(int))+sizeof(int),size);
            targetPtr = this.CellPtr + (*(int*)this.CellPtr)+sizeof(int);
            *(int*)this.CellPtr += size;
            this.CellPtr += sizeof(int);

        if(element!= null)
        {
            int strlen_0 = element.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = element)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        }
        /// <summary>
        /// Inserts an element into the List at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="element">The object to insert.</param>
        public unsafe void Insert(int index, string element)
        {
            if (index < 0 || index > Count) throw new IndexOutOfRangeException();
            byte* targetPtr = null;
            {
            
        if(element!= null)
        {
            int strlen_0 = element.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

            }
            int size = (int)targetPtr;

            targetPtr = CellPtr;
            for(int i = 0; i < index; i++)
            {
            targetPtr += 4 + *(int*)targetPtr;

            }
            int offset = (int)(targetPtr - CellPtr);
            
            this.CellPtr = this.ResizeFunction(this.CellPtr - 4, offset + 4, size);
            *(int*)this.CellPtr += size;
            this.CellPtr += 4;

            targetPtr = this.CellPtr + offset;
            
        if(element!= null)
        {
            int strlen_0 = element.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = element)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        }
        /// <summary>
        /// Inserts an element into the List at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="element">The object to insert.</param>
        public unsafe void Insert(string element, Comparison<StringAccessor> comparison)

        {
            byte* targetPtr = null;
            {
            
        if(element!= null)
        {
            int strlen_0 = element.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

            }
            int size = (int)targetPtr;
            targetPtr = CellPtr;
            byte* endPtr = CellPtr + length;
            while( targetPtr < endPtr )
            {
                elementAccessor.CellPtr = targetPtr + 4;
                if(comparison(elementAccessor, element)<=0)
                {
                targetPtr += 4 + *(int*)targetPtr;

                }
                else
                {
                    break;
                }
            }
            int offset = (int)(targetPtr - CellPtr);
            
            this.CellPtr = this.ResizeFunction(this.CellPtr - 4, offset + 4, size);
            *(int*)this.CellPtr += size;
            this.CellPtr += 4;

            targetPtr = this.CellPtr + offset;
            
        if(element!= null)
        {
            int strlen_0 = element.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = element)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        }

        /// <summary>
        /// Removes the element at the specified index of the List.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        public unsafe void RemoveAt(int index)
        {
            if (index < 0 || index >= Count) throw new IndexOutOfRangeException();

            byte* targetPtr = CellPtr;    
            for(int i = 0; i < index; i++)
            {
            targetPtr += 4 + *(int*)targetPtr;

            }
            int offset = (int)(targetPtr - CellPtr);
            byte* oldtargetPtr = targetPtr;
            targetPtr += 4 + *(int*)targetPtr;

            int size = (int)(oldtargetPtr - targetPtr);
            
            this.CellPtr = this.ResizeFunction(this.CellPtr - 4, offset + 4, size);
            *(int*)this.CellPtr += size;
            this.CellPtr += 4;

        }

        /// <summary>
        /// Adds the elements of the specified collection to the end of the List
        /// </summary>
        /// <param name="collection">The collection whose elements should be added to the end of the List. The collection itself cannot be null.</param>
        public unsafe void AddRange(List<string> collection)
        {
            if (collection == null) throw new ArgumentNullException("collection is null.");
            StringAccessorListAccessor tcollection = collection;
            int delta = tcollection.length;
            CellPtr = ResizeFunction(CellPtr - 4, *(int*)(CellPtr - 4) + 4, delta);
            Memory.Copy(tcollection.CellPtr, CellPtr + *(int*)CellPtr + 4, delta);
            *(int*)CellPtr += delta;
            this.CellPtr += 4;
        }
        /// <summary>
        /// Adds the elements of the specified collection to the end of the List
        /// </summary>
        /// <param name="collection">The collection whose elements should be added to the end of the List. The collection itself cannot be null.</param>
        public unsafe void AddRange(StringAccessorListAccessor collection)
        {
            if (collection == null) throw new ArgumentNullException("collection is null.");
            int delta = collection.length;
            if (collection.CellID != CellID)
            {
                CellPtr = ResizeFunction(CellPtr - 4, *(int*)(CellPtr - 4) + 4, delta);
                Memory.Copy(collection.CellPtr, CellPtr + *(int*)CellPtr + 4, delta);
                *(int*)CellPtr += delta;
            }
            else
            {
                byte[] tmpcell = new byte[delta];
                fixed (byte* tmpcellptr = tmpcell)
                {
                    Memory.Copy(collection.CellPtr, tmpcellptr, delta);
                    CellPtr = ResizeFunction(CellPtr - 4, *(int*)(CellPtr - 4) + 4, delta);
                    Memory.Copy(tmpcellptr, CellPtr + *(int*)CellPtr + 4, delta);
                    *(int*)CellPtr += delta;
                }
            }
            this.CellPtr += 4;
        }

        /// <summary>
        /// Removes all elements from the List
        /// </summary>
        public unsafe void Clear()
        {
            int delta = length;
            Memory.memset(CellPtr, 0, (ulong)delta);
            CellPtr = ResizeFunction(CellPtr - 4, 4, -delta);
            *(int*)CellPtr = 0;
            this.CellPtr += 4;
        }

        /// <summary>
        /// Determines whether an element is in the List
        /// </summary>
        /// <param name="item">The object to locate in the List.The value can be null for non-atom types</param>
        /// <returns>true if item is found in the List; otherwise, false.</returns>
        public unsafe bool Contains(StringAccessor item)
        {
            bool ret = false;
            ForEach(x =>
            {
                if (item == x) ret = true;
            });
            return ret;
        }

        /// <summary>
        /// Determines whether the List contains elements that match the conditions defined by the specified predicate.
        /// </summary>
        /// <param name="match">The Predicate delegate that defines the conditions of the elements to search for.</param>
        /// <returns>true if the List contains one or more elements that match the conditions defined by the specified predicate; otherwise, false.</returns>
        public unsafe bool Exists(Predicate<string> match)
        {
            bool ret = false;
            ForEach(x => {
                if (match(x)) ret = true;
            });
            return ret;
        }
    
        /// <summary>
        /// Copies the entire List to a compatible one-dimensional array, starting at the beginning of the ptr1 array.
        /// </summary>
        /// <param name="array">The one-dimensional Array that is the destination of the elements copied from List. The Array must have zero-based indexing.</param>
        public unsafe void CopyTo(string[] array)
        {
            if (array == null) throw new ArgumentNullException("array is null.");
            if (array.Length < Count) throw new ArgumentException("The number of elements in the source List is greater than the number of elements that the destination array can contain.");
            ForEach((x, i) => array[i] = x);
        }

        /// <summary>
        /// Copies the entire List to a compatible one-dimensional array, starting at the specified index of the ptr1 array.
        /// </summary>
        /// <param name="array">The one-dimensional Array that is the destination of the elements copied from List. The Array must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        public unsafe void CopyTo(string[] array, int arrayIndex)
        {
            if (array == null) throw new ArgumentNullException("array is null.");
            if (arrayIndex < 0) throw new ArgumentOutOfRangeException("arrayIndex is less than 0.");
            if (array.Length - arrayIndex < Count) throw new ArgumentException("The number of elements in the source List is greater than the available space from arrayIndex to the end of the destination array.");
            ForEach((x, i) => array[i + arrayIndex] = x);
        }

        /// <summary>
        /// Copies a range of elements from the List to a compatible one-dimensional array, starting at the specified index of the ptr1 array.
        /// </summary>
        /// <param name="index">The zero-based index in the source List at which copying begins.</param>
        /// <param name="array">The one-dimensional Array that is the destination of the elements copied from List. The Array must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>;
        /// <param name="count">The number of elements to copy.</param>
        public unsafe void CopyTo(int index, string[] array, int arrayIndex, int count)
        {
            if (array == null) throw new ArgumentNullException("array is null.");
            if (arrayIndex < 0 || index < 0 || count < 0) throw new ArgumentOutOfRangeException("arrayIndex is less than 0 or index is less than 0 or count is less than 0.");
            if (array.Length - arrayIndex < Count - index) throw new ArgumentException("The number of elements from index to the end of the source List is greater than the available space from arrayIndex to the end of the destination array. ");
            if (index >= Count) throw new ArgumentException("index is equal to or greater than the Count of the source List.");
            int j = 0;
            for (int i = index; i < index + count; i++)
            {
                array[j + arrayIndex] = this[i];
                ++j;
            }
        }

        /// <summary>
        /// Inserts the elements of a collection into the List at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the new elements should be inserted.</param>
        /// <param name="collection">The collection whose elements should be inserted into the List. The collection itself cannot be null, but it can contain elements that are null, if type T is a reference type.</param>
        public unsafe void InsertRange(int index, List<string> collection)
        {
            if (collection == null) throw new ArgumentNullException("collection is null.");
            if (index < 0) throw new ArgumentOutOfRangeException("index is less than 0.");
            if (index > Count) throw new ArgumentOutOfRangeException("index is greater than Count.");
            StringAccessorListAccessor tmpAccessor = collection;
            byte* targetPtr = CellPtr;
            for (int i = 0; i < index; i++)
            {
                targetPtr += 4 + *(int*)targetPtr;

            }
            int offset = (int)(targetPtr - CellPtr);
            CellPtr = ResizeFunction(CellPtr - 4, offset + 4, tmpAccessor.length);
            Memory.Copy(tmpAccessor.CellPtr, CellPtr + offset + 4, tmpAccessor.length);
            *(int*)CellPtr += tmpAccessor.length;
            this.CellPtr += 4;
        }

        /// <summary>
        /// Removes a range of elements from the List.
        /// </summary>
        /// <param name="index">The zero-based starting index of the range of elements to remove.</param>
        /// <param name="count">The number of elements to remove.</param>
        public unsafe void RemoveRange(int index, int count)
        {
            if (index < 0) throw new ArgumentOutOfRangeException("index is less than 0.");
            if (index > Count) throw new ArgumentOutOfRangeException("index is greater than Count.");
            if (index + count > Count) throw new ArgumentException("index and count do not denote a valid range of elements in the List.");
            byte* targetPtr = CellPtr;
            for (int i = 0; i < index; i++)
            {targetPtr += 4 + *(int*)targetPtr;
}
            int offset = (int)(targetPtr - CellPtr);
            byte* oldtargetPtr = targetPtr;
            for (int i = 0; i < count; i++)
            {targetPtr += 4 + *(int*)targetPtr;
}
            int size = (int)(oldtargetPtr - targetPtr);
            CellPtr = ResizeFunction(CellPtr - 4, offset + 4, size);
            *(int*)CellPtr += size;
            this.CellPtr += 4;
        }

        public unsafe static implicit operator List<string>(StringAccessorListAccessor accessor)
        {
            if((object)accessor == null) return null;
            List<string> list = new List<string>();
            accessor.ForEach(element => list.Add(element));
            return list;
        }

        public unsafe static implicit operator StringAccessorListAccessor(List<string> field)
        {  
            byte* targetPtr = null;
            
{

    targetPtr += sizeof(int);
    if(field!= null)
    {
        for(int iterator_0 = 0;iterator_0<field.Count;++iterator_0)
        {

        if(field[iterator_0]!= null)
        {
            int strlen_1 = field[iterator_0].Length * 2;
            targetPtr += strlen_1+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        }
    }

}

            byte* tmpcellptr = BufferAllocator.AllocBuffer((int)targetPtr);
            Memory.memset(tmpcellptr, 0, (ulong)targetPtr);
            targetPtr = tmpcellptr;
        
{
byte *storedPtr_0 = targetPtr;

    targetPtr += sizeof(int);
    if(field!= null)
    {
        for(int iterator_0 = 0;iterator_0<field.Count;++iterator_0)
        {

        if(field[iterator_0]!= null)
        {
            int strlen_1 = field[iterator_0].Length * 2;
            *(int*)targetPtr = strlen_1;
            targetPtr += sizeof(int);
            fixed(char* pstr_1 = field[iterator_0])
            {
                Memory.Copy(pstr_1, targetPtr, strlen_1);
                targetPtr += strlen_1;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        }
    }
*(int*)storedPtr_0 = (int)(targetPtr - storedPtr_0 - 4);

}

            StringAccessorListAccessor ret = new StringAccessorListAccessor(tmpcellptr,null);
            ret.CellID = null;
            return ret;
        }

        public static bool operator == (StringAccessorListAccessor a, StringAccessorListAccessor b)
        {
            if (ReferenceEquals(a, b))
              return true;
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
              return false;
            // If both are same instance, return true.
            if (a.CellPtr == b.CellPtr) return true;
            // If length not equal, return false.
            if (a.length != b.length) return false;
            return Memory.Compare(a.CellPtr, b.CellPtr, a.length);
        }

        public static bool operator !=(StringAccessorListAccessor a, StringAccessorListAccessor b)
        {
            return !(a == b);
        }

    }

    public partial struct Movie
    {

        ///<summary>
        ///The id of the cell.
        ///</summary>
        public long CellID;		
        ///<summary>
        ///Initializes a new cell of the type Movie with the specified parameters.
        ///</summary>
        public Movie(long cell_id, string Key=null,string KGId=null,List<string> Genres=null,List<long> Artists=null,List<long> Directors=null,string Characters=null,List<string> Performance=null,string Distributors=null,string Channels=null,string Albums=null,string Name=null,string Alias=null,string Description=null,string Segments=null,string Categories=null,string IntEmbeddedFilters=null,string NumberOfWantToWatch=null,string Rating=null,string NumberOfShortReview=null,string ReviewCount=null,string NumberOfWatched=null,string NumberOfReviewer=null,string PublishDate=null,string Length=null,string Country=null,string Language=null,string SourceUrls=null,string ImageUrls=null,string OfficialSite=null,string EntityContainer=null,string Logo=null,string QueryRank=null)
		{
            this.Key = Key;
            this.KGId = KGId;
            this.Genres = Genres;
            this.Artists = Artists;
            this.Directors = Directors;
            this.Characters = Characters;
            this.Performance = Performance;
            this.Distributors = Distributors;
            this.Channels = Channels;
            this.Albums = Albums;
            this.Name = Name;
            this.Alias = Alias;
            this.Description = Description;
            this.Segments = Segments;
            this.Categories = Categories;
            this.IntEmbeddedFilters = IntEmbeddedFilters;
            this.NumberOfWantToWatch = NumberOfWantToWatch;
            this.Rating = Rating;
            this.NumberOfShortReview = NumberOfShortReview;
            this.ReviewCount = ReviewCount;
            this.NumberOfWatched = NumberOfWatched;
            this.NumberOfReviewer = NumberOfReviewer;
            this.PublishDate = PublishDate;
            this.Length = Length;
            this.Country = Country;
            this.Language = Language;
            this.SourceUrls = SourceUrls;
            this.ImageUrls = ImageUrls;
            this.OfficialSite = OfficialSite;
            this.EntityContainer = EntityContainer;
            this.Logo = Logo;
            this.QueryRank = QueryRank;
		
CellID = cell_id;
		}
		
        ///<summary>
        ///Initializes a new instance of the Movie class with the specified parameters.
        ///</summary>
        public Movie(string Key=null,string KGId=null,List<string> Genres=null,List<long> Artists=null,List<long> Directors=null,string Characters=null,List<string> Performance=null,string Distributors=null,string Channels=null,string Albums=null,string Name=null,string Alias=null,string Description=null,string Segments=null,string Categories=null,string IntEmbeddedFilters=null,string NumberOfWantToWatch=null,string Rating=null,string NumberOfShortReview=null,string ReviewCount=null,string NumberOfWatched=null,string NumberOfReviewer=null,string PublishDate=null,string Length=null,string Country=null,string Language=null,string SourceUrls=null,string ImageUrls=null,string OfficialSite=null,string EntityContainer=null,string Logo=null,string QueryRank=null)
		{
            this.Key = Key;
            this.KGId = KGId;
            this.Genres = Genres;
            this.Artists = Artists;
            this.Directors = Directors;
            this.Characters = Characters;
            this.Performance = Performance;
            this.Distributors = Distributors;
            this.Channels = Channels;
            this.Albums = Albums;
            this.Name = Name;
            this.Alias = Alias;
            this.Description = Description;
            this.Segments = Segments;
            this.Categories = Categories;
            this.IntEmbeddedFilters = IntEmbeddedFilters;
            this.NumberOfWantToWatch = NumberOfWantToWatch;
            this.Rating = Rating;
            this.NumberOfShortReview = NumberOfShortReview;
            this.ReviewCount = ReviewCount;
            this.NumberOfWatched = NumberOfWatched;
            this.NumberOfReviewer = NumberOfReviewer;
            this.PublishDate = PublishDate;
            this.Length = Length;
            this.Country = Country;
            this.Language = Language;
            this.SourceUrls = SourceUrls;
            this.ImageUrls = ImageUrls;
            this.OfficialSite = OfficialSite;
            this.EntityContainer = EntityContainer;
            this.Logo = Logo;
            this.QueryRank = QueryRank;

		CellID = CellIDFactory.NewCellID();
		}

        public string Key;

        public string KGId;

        public List<string> Genres;

        public List<long> Artists;

        public List<long> Directors;

        public string Characters;

        public List<string> Performance;

        public string Distributors;

        public string Channels;

        public string Albums;

        public string Name;

        public string Alias;

        public string Description;

        public string Segments;

        public string Categories;

        public string IntEmbeddedFilters;

        public string NumberOfWantToWatch;

        public string Rating;

        public string NumberOfShortReview;

        public string ReviewCount;

        public string NumberOfWatched;

        public string NumberOfReviewer;

        public string PublishDate;

        public string Length;

        public string Country;

        public string Language;

        public string SourceUrls;

        public string ImageUrls;

        public string OfficialSite;

        public string EntityContainer;

        public string Logo;

        public string QueryRank;

        public static bool operator == (Movie a, Movie b)
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
            return a.Key == b.Key && a.KGId == b.KGId && a.Genres == b.Genres && a.Artists == b.Artists && a.Directors == b.Directors && a.Characters == b.Characters && a.Performance == b.Performance && a.Distributors == b.Distributors && a.Channels == b.Channels && a.Albums == b.Albums && a.Name == b.Name && a.Alias == b.Alias && a.Description == b.Description && a.Segments == b.Segments && a.Categories == b.Categories && a.IntEmbeddedFilters == b.IntEmbeddedFilters && a.NumberOfWantToWatch == b.NumberOfWantToWatch && a.Rating == b.Rating && a.NumberOfShortReview == b.NumberOfShortReview && a.ReviewCount == b.ReviewCount && a.NumberOfWatched == b.NumberOfWatched && a.NumberOfReviewer == b.NumberOfReviewer && a.PublishDate == b.PublishDate && a.Length == b.Length && a.Country == b.Country && a.Language == b.Language && a.SourceUrls == b.SourceUrls && a.ImageUrls == b.ImageUrls && a.OfficialSite == b.OfficialSite && a.EntityContainer == b.EntityContainer && a.Logo == b.Logo && a.QueryRank == b.QueryRank;
        }

        public static bool operator != (Movie a, Movie b)
        {
            return !(a == b);
        }

    }

    public unsafe partial class Movie_Accessor: IDisposable
    {
        internal Movie_Accessor(long cellId, byte[] buffer)
        {
            this.CellID  = cellId;
            handle       = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            this.CellPtr = (byte*)handle.AddrOfPinnedObject().ToPointer();
        Key_Accessor_Field = new StringAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        KGId_Accessor_Field = new StringAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        Genres_Accessor_Field = new StringAccessorListAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        Artists_Accessor_Field = new longListAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        Directors_Accessor_Field = new longListAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        Characters_Accessor_Field = new StringAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        Performance_Accessor_Field = new StringAccessorListAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        Distributors_Accessor_Field = new StringAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        Channels_Accessor_Field = new StringAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        Albums_Accessor_Field = new StringAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        Name_Accessor_Field = new StringAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        Alias_Accessor_Field = new StringAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        Description_Accessor_Field = new StringAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        Segments_Accessor_Field = new StringAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        Categories_Accessor_Field = new StringAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        IntEmbeddedFilters_Accessor_Field = new StringAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        NumberOfWantToWatch_Accessor_Field = new StringAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        Rating_Accessor_Field = new StringAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        NumberOfShortReview_Accessor_Field = new StringAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        ReviewCount_Accessor_Field = new StringAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        NumberOfWatched_Accessor_Field = new StringAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        NumberOfReviewer_Accessor_Field = new StringAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        PublishDate_Accessor_Field = new StringAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        Length_Accessor_Field = new StringAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        Country_Accessor_Field = new StringAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        Language_Accessor_Field = new StringAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        SourceUrls_Accessor_Field = new StringAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        ImageUrls_Accessor_Field = new StringAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        OfficialSite_Accessor_Field = new StringAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        EntityContainer_Accessor_Field = new StringAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        Logo_Accessor_Field = new StringAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        QueryRank_Accessor_Field = new StringAccessor(null,
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
        ///Get an array of the names of all optional fields for object type Movie.
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
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;

            int size   = (int)(targetPtr - CellPtr);
            byte[] ret = new byte[size];
            Memory.Copy(CellPtr,0,ret,0,size);
            return ret;
        }

        internal unsafe Movie_Accessor(long CellID, CellAccessOptions options)
        {
            this.Initialize(CellID, options);

        Key_Accessor_Field = new StringAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        KGId_Accessor_Field = new StringAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        Genres_Accessor_Field = new StringAccessorListAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        Artists_Accessor_Field = new longListAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        Directors_Accessor_Field = new longListAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        Characters_Accessor_Field = new StringAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        Performance_Accessor_Field = new StringAccessorListAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        Distributors_Accessor_Field = new StringAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        Channels_Accessor_Field = new StringAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        Albums_Accessor_Field = new StringAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        Name_Accessor_Field = new StringAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        Alias_Accessor_Field = new StringAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        Description_Accessor_Field = new StringAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        Segments_Accessor_Field = new StringAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        Categories_Accessor_Field = new StringAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        IntEmbeddedFilters_Accessor_Field = new StringAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        NumberOfWantToWatch_Accessor_Field = new StringAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        Rating_Accessor_Field = new StringAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        NumberOfShortReview_Accessor_Field = new StringAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        ReviewCount_Accessor_Field = new StringAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        NumberOfWatched_Accessor_Field = new StringAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        NumberOfReviewer_Accessor_Field = new StringAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        PublishDate_Accessor_Field = new StringAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        Length_Accessor_Field = new StringAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        Country_Accessor_Field = new StringAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        Language_Accessor_Field = new StringAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        SourceUrls_Accessor_Field = new StringAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        ImageUrls_Accessor_Field = new StringAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        OfficialSite_Accessor_Field = new StringAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        EntityContainer_Accessor_Field = new StringAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        Logo_Accessor_Field = new StringAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        QueryRank_Accessor_Field = new StringAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

            this.CellID = CellID;
        }

        internal unsafe Movie_Accessor(byte* _CellPtr)
        {
            CellPtr = _CellPtr;
        Key_Accessor_Field = new StringAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        KGId_Accessor_Field = new StringAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        Genres_Accessor_Field = new StringAccessorListAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        Artists_Accessor_Field = new longListAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        Directors_Accessor_Field = new longListAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        Characters_Accessor_Field = new StringAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        Performance_Accessor_Field = new StringAccessorListAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        Distributors_Accessor_Field = new StringAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        Channels_Accessor_Field = new StringAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        Albums_Accessor_Field = new StringAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        Name_Accessor_Field = new StringAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        Alias_Accessor_Field = new StringAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        Description_Accessor_Field = new StringAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        Segments_Accessor_Field = new StringAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        Categories_Accessor_Field = new StringAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        IntEmbeddedFilters_Accessor_Field = new StringAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        NumberOfWantToWatch_Accessor_Field = new StringAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        Rating_Accessor_Field = new StringAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        NumberOfShortReview_Accessor_Field = new StringAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        ReviewCount_Accessor_Field = new StringAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        NumberOfWatched_Accessor_Field = new StringAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        NumberOfReviewer_Accessor_Field = new StringAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        PublishDate_Accessor_Field = new StringAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        Length_Accessor_Field = new StringAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        Country_Accessor_Field = new StringAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        Language_Accessor_Field = new StringAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        SourceUrls_Accessor_Field = new StringAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        ImageUrls_Accessor_Field = new StringAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        OfficialSite_Accessor_Field = new StringAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        EntityContainer_Accessor_Field = new StringAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        Logo_Accessor_Field = new StringAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

        QueryRank_Accessor_Field = new StringAccessor(null,
                (ptr,ptr_offset,delta)=>
                {
                    int substructure_offset = (int)(ptr - this.CellPtr);
                    this.ResizeFunction(this.CellPtr, ptr_offset + substructure_offset, delta);
                    return this.CellPtr + substructure_offset;
                });

            this.CellEntryIndex = -1;
        }		internal static unsafe byte[] construct(long CellID, string Key=null,string KGId=null,List<string> Genres=null,List<long> Artists=null,List<long> Directors=null,string Characters=null,List<string> Performance=null,string Distributors=null,string Channels=null,string Albums=null,string Name=null,string Alias=null,string Description=null,string Segments=null,string Categories=null,string IntEmbeddedFilters=null,string NumberOfWantToWatch=null,string Rating=null,string NumberOfShortReview=null,string ReviewCount=null,string NumberOfWatched=null,string NumberOfReviewer=null,string PublishDate=null,string Length=null,string Country=null,string Language=null,string SourceUrls=null,string ImageUrls=null,string OfficialSite=null,string EntityContainer=null,string Logo=null,string QueryRank=null)
		{

        byte* targetPtr = null;

        if(Key!= null)
        {
            int strlen_0 = Key.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(KGId!= null)
        {
            int strlen_0 = KGId.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

{

    targetPtr += sizeof(int);
    if(Genres!= null)
    {
        for(int iterator_0 = 0;iterator_0<Genres.Count;++iterator_0)
        {

        if(Genres[iterator_0]!= null)
        {
            int strlen_1 = Genres[iterator_0].Length * 2;
            targetPtr += strlen_1+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        }
    }

}

if(Artists!= null)
{
    targetPtr += Artists.Count*8+sizeof(int);
}else
{
    targetPtr += sizeof(int);
}


if(Directors!= null)
{
    targetPtr += Directors.Count*8+sizeof(int);
}else
{
    targetPtr += sizeof(int);
}


        if(Characters!= null)
        {
            int strlen_0 = Characters.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

{

    targetPtr += sizeof(int);
    if(Performance!= null)
    {
        for(int iterator_0 = 0;iterator_0<Performance.Count;++iterator_0)
        {

        if(Performance[iterator_0]!= null)
        {
            int strlen_1 = Performance[iterator_0].Length * 2;
            targetPtr += strlen_1+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        }
    }

}

        if(Distributors!= null)
        {
            int strlen_0 = Distributors.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(Channels!= null)
        {
            int strlen_0 = Channels.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(Albums!= null)
        {
            int strlen_0 = Albums.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(Name!= null)
        {
            int strlen_0 = Name.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(Alias!= null)
        {
            int strlen_0 = Alias.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(Description!= null)
        {
            int strlen_0 = Description.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(Segments!= null)
        {
            int strlen_0 = Segments.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(Categories!= null)
        {
            int strlen_0 = Categories.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(IntEmbeddedFilters!= null)
        {
            int strlen_0 = IntEmbeddedFilters.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(NumberOfWantToWatch!= null)
        {
            int strlen_0 = NumberOfWantToWatch.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(Rating!= null)
        {
            int strlen_0 = Rating.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(NumberOfShortReview!= null)
        {
            int strlen_0 = NumberOfShortReview.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(ReviewCount!= null)
        {
            int strlen_0 = ReviewCount.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(NumberOfWatched!= null)
        {
            int strlen_0 = NumberOfWatched.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(NumberOfReviewer!= null)
        {
            int strlen_0 = NumberOfReviewer.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(PublishDate!= null)
        {
            int strlen_0 = PublishDate.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(Length!= null)
        {
            int strlen_0 = Length.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(Country!= null)
        {
            int strlen_0 = Country.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(Language!= null)
        {
            int strlen_0 = Language.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(SourceUrls!= null)
        {
            int strlen_0 = SourceUrls.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(ImageUrls!= null)
        {
            int strlen_0 = ImageUrls.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(OfficialSite!= null)
        {
            int strlen_0 = OfficialSite.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(EntityContainer!= null)
        {
            int strlen_0 = EntityContainer.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(Logo!= null)
        {
            int strlen_0 = Logo.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(QueryRank!= null)
        {
            int strlen_0 = QueryRank.Length * 2;
            targetPtr += strlen_0+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        byte[] tmpcell = new byte[(int)(targetPtr)];
        fixed(byte* tmpcellptr = tmpcell)
        {
            targetPtr = tmpcellptr;

        if(Key!= null)
        {
            int strlen_0 = Key.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = Key)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(KGId!= null)
        {
            int strlen_0 = KGId.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = KGId)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

{
byte *storedPtr_0 = targetPtr;

    targetPtr += sizeof(int);
    if(Genres!= null)
    {
        for(int iterator_0 = 0;iterator_0<Genres.Count;++iterator_0)
        {

        if(Genres[iterator_0]!= null)
        {
            int strlen_1 = Genres[iterator_0].Length * 2;
            *(int*)targetPtr = strlen_1;
            targetPtr += sizeof(int);
            fixed(char* pstr_1 = Genres[iterator_0])
            {
                Memory.Copy(pstr_1, targetPtr, strlen_1);
                targetPtr += strlen_1;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        }
    }
*(int*)storedPtr_0 = (int)(targetPtr - storedPtr_0 - 4);

}

if(Artists!= null)
{
    *(int*)targetPtr = Artists.Count*8;
    targetPtr += sizeof(int);
    for(int iterator_0 = 0;iterator_0<Artists.Count;++iterator_0)
    {
*(long*)targetPtr = Artists[iterator_0];
            targetPtr += 8;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

if(Directors!= null)
{
    *(int*)targetPtr = Directors.Count*8;
    targetPtr += sizeof(int);
    for(int iterator_0 = 0;iterator_0<Directors.Count;++iterator_0)
    {
*(long*)targetPtr = Directors[iterator_0];
            targetPtr += 8;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

        if(Characters!= null)
        {
            int strlen_0 = Characters.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = Characters)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

{
byte *storedPtr_0 = targetPtr;

    targetPtr += sizeof(int);
    if(Performance!= null)
    {
        for(int iterator_0 = 0;iterator_0<Performance.Count;++iterator_0)
        {

        if(Performance[iterator_0]!= null)
        {
            int strlen_1 = Performance[iterator_0].Length * 2;
            *(int*)targetPtr = strlen_1;
            targetPtr += sizeof(int);
            fixed(char* pstr_1 = Performance[iterator_0])
            {
                Memory.Copy(pstr_1, targetPtr, strlen_1);
                targetPtr += strlen_1;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        }
    }
*(int*)storedPtr_0 = (int)(targetPtr - storedPtr_0 - 4);

}

        if(Distributors!= null)
        {
            int strlen_0 = Distributors.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = Distributors)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(Channels!= null)
        {
            int strlen_0 = Channels.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = Channels)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(Albums!= null)
        {
            int strlen_0 = Albums.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = Albums)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

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

        if(Alias!= null)
        {
            int strlen_0 = Alias.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = Alias)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(Description!= null)
        {
            int strlen_0 = Description.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = Description)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(Segments!= null)
        {
            int strlen_0 = Segments.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = Segments)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(Categories!= null)
        {
            int strlen_0 = Categories.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = Categories)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(IntEmbeddedFilters!= null)
        {
            int strlen_0 = IntEmbeddedFilters.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = IntEmbeddedFilters)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(NumberOfWantToWatch!= null)
        {
            int strlen_0 = NumberOfWantToWatch.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = NumberOfWantToWatch)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(Rating!= null)
        {
            int strlen_0 = Rating.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = Rating)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(NumberOfShortReview!= null)
        {
            int strlen_0 = NumberOfShortReview.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = NumberOfShortReview)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(ReviewCount!= null)
        {
            int strlen_0 = ReviewCount.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = ReviewCount)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(NumberOfWatched!= null)
        {
            int strlen_0 = NumberOfWatched.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = NumberOfWatched)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(NumberOfReviewer!= null)
        {
            int strlen_0 = NumberOfReviewer.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = NumberOfReviewer)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(PublishDate!= null)
        {
            int strlen_0 = PublishDate.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = PublishDate)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(Length!= null)
        {
            int strlen_0 = Length.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = Length)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(Country!= null)
        {
            int strlen_0 = Country.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = Country)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(Language!= null)
        {
            int strlen_0 = Language.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = Language)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(SourceUrls!= null)
        {
            int strlen_0 = SourceUrls.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = SourceUrls)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(ImageUrls!= null)
        {
            int strlen_0 = ImageUrls.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = ImageUrls)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(OfficialSite!= null)
        {
            int strlen_0 = OfficialSite.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = OfficialSite)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(EntityContainer!= null)
        {
            int strlen_0 = EntityContainer.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = EntityContainer)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(Logo!= null)
        {
            int strlen_0 = Logo.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = Logo)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(QueryRank!= null)
        {
            int strlen_0 = QueryRank.Length * 2;
            *(int*)targetPtr = strlen_0;
            targetPtr += sizeof(int);
            fixed(char* pstr_0 = QueryRank)
            {
                Memory.Copy(pstr_0, targetPtr, strlen_0);
                targetPtr += strlen_0;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        }

            return tmpcell;
        }
StringAccessor Key_Accessor_Field;
        ///<summary>
        ///Provides in-place access to the object field Key.
        ///</summary>
        public unsafe StringAccessor Key
        {
            get
            {
                
                byte* targetPtr = CellPtr;

                Key_Accessor_Field.CellPtr = targetPtr + 4;
                Key_Accessor_Field.CellID = this.CellID;
                return Key_Accessor_Field;
            }

            set
            {
                if ((object)value == null) throw new ArgumentNullException("The assigned variable is null.");
                byte* targetPtr = CellPtr;

                Key_Accessor_Field.CellID = this.CellID;
                
              int length = *(int*)(value.CellPtr - 4);

                //senario: cell_a.inlinks = cell_b.inlinks,
                //the later part will invoke the Get, filling cell_b.inlinks(a inlink_accessor_fiedld)'storage CellID
                int oldlength = *(int*)targetPtr;
                if (value.CellID != Key_Accessor_Field.CellID)
                {
                    //if not in the same Cell
                    Key_Accessor_Field.CellPtr = Key_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                    Memory.Copy(value.CellPtr - 4, Key_Accessor_Field.CellPtr, length + 4);
                }
                else
                {
                    byte[] tmpcell = new byte[length + 4];
                    fixed (byte* tmpcellptr = tmpcell)
                    {                        
                        Memory.Copy(value.CellPtr - 4, tmpcellptr, length + 4);
                        Key_Accessor_Field.CellPtr = Key_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                        Memory.Copy(tmpcellptr, Key_Accessor_Field.CellPtr, length + 4);
                    }
                }
                Key_Accessor_Field.CellPtr += 4;
                
            }

    }
StringAccessor KGId_Accessor_Field;
        ///<summary>
        ///Provides in-place access to the object field KGId.
        ///</summary>
        public unsafe StringAccessor KGId
        {
            get
            {
                
                byte* targetPtr = CellPtr;
            targetPtr += 4 + *(int*)targetPtr;

                KGId_Accessor_Field.CellPtr = targetPtr + 4;
                KGId_Accessor_Field.CellID = this.CellID;
                return KGId_Accessor_Field;
            }

            set
            {
                if ((object)value == null) throw new ArgumentNullException("The assigned variable is null.");
                byte* targetPtr = CellPtr;
            targetPtr += 4 + *(int*)targetPtr;

                KGId_Accessor_Field.CellID = this.CellID;
                
              int length = *(int*)(value.CellPtr - 4);

                //senario: cell_a.inlinks = cell_b.inlinks,
                //the later part will invoke the Get, filling cell_b.inlinks(a inlink_accessor_fiedld)'storage CellID
                int oldlength = *(int*)targetPtr;
                if (value.CellID != KGId_Accessor_Field.CellID)
                {
                    //if not in the same Cell
                    KGId_Accessor_Field.CellPtr = KGId_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                    Memory.Copy(value.CellPtr - 4, KGId_Accessor_Field.CellPtr, length + 4);
                }
                else
                {
                    byte[] tmpcell = new byte[length + 4];
                    fixed (byte* tmpcellptr = tmpcell)
                    {                        
                        Memory.Copy(value.CellPtr - 4, tmpcellptr, length + 4);
                        KGId_Accessor_Field.CellPtr = KGId_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                        Memory.Copy(tmpcellptr, KGId_Accessor_Field.CellPtr, length + 4);
                    }
                }
                KGId_Accessor_Field.CellPtr += 4;
                
            }

    }
StringAccessorListAccessor Genres_Accessor_Field;
        ///<summary>
        ///Provides in-place access to the object field Genres.
        ///</summary>
        public unsafe StringAccessorListAccessor Genres
        {
            get
            {
                
                byte* targetPtr = CellPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;

                Genres_Accessor_Field.CellPtr = targetPtr + 4;
                Genres_Accessor_Field.CellID = this.CellID;
                return Genres_Accessor_Field;
            }

            set
            {
                if ((object)value == null) throw new ArgumentNullException("The assigned variable is null.");
                byte* targetPtr = CellPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;

                Genres_Accessor_Field.CellID = this.CellID;
                
              int length = *(int*)(value.CellPtr - 4);

                //senario: cell_a.inlinks = cell_b.inlinks,
                //the later part will invoke the Get, filling cell_b.inlinks(a inlink_accessor_fiedld)'storage CellID
                int oldlength = *(int*)targetPtr;
                if (value.CellID != Genres_Accessor_Field.CellID)
                {
                    //if not in the same Cell
                    Genres_Accessor_Field.CellPtr = Genres_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                    Memory.Copy(value.CellPtr - 4, Genres_Accessor_Field.CellPtr, length + 4);
                }
                else
                {
                    byte[] tmpcell = new byte[length + 4];
                    fixed (byte* tmpcellptr = tmpcell)
                    {                        
                        Memory.Copy(value.CellPtr - 4, tmpcellptr, length + 4);
                        Genres_Accessor_Field.CellPtr = Genres_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                        Memory.Copy(tmpcellptr, Genres_Accessor_Field.CellPtr, length + 4);
                    }
                }
                Genres_Accessor_Field.CellPtr += 4;
                
            }

    }
longListAccessor Artists_Accessor_Field;
        ///<summary>
        ///Provides in-place access to the object field Artists.
        ///</summary>
        public unsafe longListAccessor Artists
        {
            get
            {
                
                byte* targetPtr = CellPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;

                Artists_Accessor_Field.CellPtr = targetPtr + 4;
                Artists_Accessor_Field.CellID = this.CellID;
                return Artists_Accessor_Field;
            }

            set
            {
                if ((object)value == null) throw new ArgumentNullException("The assigned variable is null.");
                byte* targetPtr = CellPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;

                Artists_Accessor_Field.CellID = this.CellID;
                
              int length = *(int*)(value.CellPtr - 4);

                //senario: cell_a.inlinks = cell_b.inlinks,
                //the later part will invoke the Get, filling cell_b.inlinks(a inlink_accessor_fiedld)'storage CellID
                int oldlength = *(int*)targetPtr;
                if (value.CellID != Artists_Accessor_Field.CellID)
                {
                    //if not in the same Cell
                    Artists_Accessor_Field.CellPtr = Artists_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                    Memory.Copy(value.CellPtr - 4, Artists_Accessor_Field.CellPtr, length + 4);
                }
                else
                {
                    byte[] tmpcell = new byte[length + 4];
                    fixed (byte* tmpcellptr = tmpcell)
                    {                        
                        Memory.Copy(value.CellPtr - 4, tmpcellptr, length + 4);
                        Artists_Accessor_Field.CellPtr = Artists_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                        Memory.Copy(tmpcellptr, Artists_Accessor_Field.CellPtr, length + 4);
                    }
                }
                Artists_Accessor_Field.CellPtr += 4;
                
            }

    }
longListAccessor Directors_Accessor_Field;
        ///<summary>
        ///Provides in-place access to the object field Directors.
        ///</summary>
        public unsafe longListAccessor Directors
        {
            get
            {
                
                byte* targetPtr = CellPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;

                Directors_Accessor_Field.CellPtr = targetPtr + 4;
                Directors_Accessor_Field.CellID = this.CellID;
                return Directors_Accessor_Field;
            }

            set
            {
                if ((object)value == null) throw new ArgumentNullException("The assigned variable is null.");
                byte* targetPtr = CellPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;

                Directors_Accessor_Field.CellID = this.CellID;
                
              int length = *(int*)(value.CellPtr - 4);

                //senario: cell_a.inlinks = cell_b.inlinks,
                //the later part will invoke the Get, filling cell_b.inlinks(a inlink_accessor_fiedld)'storage CellID
                int oldlength = *(int*)targetPtr;
                if (value.CellID != Directors_Accessor_Field.CellID)
                {
                    //if not in the same Cell
                    Directors_Accessor_Field.CellPtr = Directors_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                    Memory.Copy(value.CellPtr - 4, Directors_Accessor_Field.CellPtr, length + 4);
                }
                else
                {
                    byte[] tmpcell = new byte[length + 4];
                    fixed (byte* tmpcellptr = tmpcell)
                    {                        
                        Memory.Copy(value.CellPtr - 4, tmpcellptr, length + 4);
                        Directors_Accessor_Field.CellPtr = Directors_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                        Memory.Copy(tmpcellptr, Directors_Accessor_Field.CellPtr, length + 4);
                    }
                }
                Directors_Accessor_Field.CellPtr += 4;
                
            }

    }
StringAccessor Characters_Accessor_Field;
        ///<summary>
        ///Provides in-place access to the object field Characters.
        ///</summary>
        public unsafe StringAccessor Characters
        {
            get
            {
                
                byte* targetPtr = CellPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;

                Characters_Accessor_Field.CellPtr = targetPtr + 4;
                Characters_Accessor_Field.CellID = this.CellID;
                return Characters_Accessor_Field;
            }

            set
            {
                if ((object)value == null) throw new ArgumentNullException("The assigned variable is null.");
                byte* targetPtr = CellPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;

                Characters_Accessor_Field.CellID = this.CellID;
                
              int length = *(int*)(value.CellPtr - 4);

                //senario: cell_a.inlinks = cell_b.inlinks,
                //the later part will invoke the Get, filling cell_b.inlinks(a inlink_accessor_fiedld)'storage CellID
                int oldlength = *(int*)targetPtr;
                if (value.CellID != Characters_Accessor_Field.CellID)
                {
                    //if not in the same Cell
                    Characters_Accessor_Field.CellPtr = Characters_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                    Memory.Copy(value.CellPtr - 4, Characters_Accessor_Field.CellPtr, length + 4);
                }
                else
                {
                    byte[] tmpcell = new byte[length + 4];
                    fixed (byte* tmpcellptr = tmpcell)
                    {                        
                        Memory.Copy(value.CellPtr - 4, tmpcellptr, length + 4);
                        Characters_Accessor_Field.CellPtr = Characters_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                        Memory.Copy(tmpcellptr, Characters_Accessor_Field.CellPtr, length + 4);
                    }
                }
                Characters_Accessor_Field.CellPtr += 4;
                
            }

    }
StringAccessorListAccessor Performance_Accessor_Field;
        ///<summary>
        ///Provides in-place access to the object field Performance.
        ///</summary>
        public unsafe StringAccessorListAccessor Performance
        {
            get
            {
                
                byte* targetPtr = CellPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;

                Performance_Accessor_Field.CellPtr = targetPtr + 4;
                Performance_Accessor_Field.CellID = this.CellID;
                return Performance_Accessor_Field;
            }

            set
            {
                if ((object)value == null) throw new ArgumentNullException("The assigned variable is null.");
                byte* targetPtr = CellPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;

                Performance_Accessor_Field.CellID = this.CellID;
                
              int length = *(int*)(value.CellPtr - 4);

                //senario: cell_a.inlinks = cell_b.inlinks,
                //the later part will invoke the Get, filling cell_b.inlinks(a inlink_accessor_fiedld)'storage CellID
                int oldlength = *(int*)targetPtr;
                if (value.CellID != Performance_Accessor_Field.CellID)
                {
                    //if not in the same Cell
                    Performance_Accessor_Field.CellPtr = Performance_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                    Memory.Copy(value.CellPtr - 4, Performance_Accessor_Field.CellPtr, length + 4);
                }
                else
                {
                    byte[] tmpcell = new byte[length + 4];
                    fixed (byte* tmpcellptr = tmpcell)
                    {                        
                        Memory.Copy(value.CellPtr - 4, tmpcellptr, length + 4);
                        Performance_Accessor_Field.CellPtr = Performance_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                        Memory.Copy(tmpcellptr, Performance_Accessor_Field.CellPtr, length + 4);
                    }
                }
                Performance_Accessor_Field.CellPtr += 4;
                
            }

    }
StringAccessor Distributors_Accessor_Field;
        ///<summary>
        ///Provides in-place access to the object field Distributors.
        ///</summary>
        public unsafe StringAccessor Distributors
        {
            get
            {
                
                byte* targetPtr = CellPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;

                Distributors_Accessor_Field.CellPtr = targetPtr + 4;
                Distributors_Accessor_Field.CellID = this.CellID;
                return Distributors_Accessor_Field;
            }

            set
            {
                if ((object)value == null) throw new ArgumentNullException("The assigned variable is null.");
                byte* targetPtr = CellPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;

                Distributors_Accessor_Field.CellID = this.CellID;
                
              int length = *(int*)(value.CellPtr - 4);

                //senario: cell_a.inlinks = cell_b.inlinks,
                //the later part will invoke the Get, filling cell_b.inlinks(a inlink_accessor_fiedld)'storage CellID
                int oldlength = *(int*)targetPtr;
                if (value.CellID != Distributors_Accessor_Field.CellID)
                {
                    //if not in the same Cell
                    Distributors_Accessor_Field.CellPtr = Distributors_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                    Memory.Copy(value.CellPtr - 4, Distributors_Accessor_Field.CellPtr, length + 4);
                }
                else
                {
                    byte[] tmpcell = new byte[length + 4];
                    fixed (byte* tmpcellptr = tmpcell)
                    {                        
                        Memory.Copy(value.CellPtr - 4, tmpcellptr, length + 4);
                        Distributors_Accessor_Field.CellPtr = Distributors_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                        Memory.Copy(tmpcellptr, Distributors_Accessor_Field.CellPtr, length + 4);
                    }
                }
                Distributors_Accessor_Field.CellPtr += 4;
                
            }

    }
StringAccessor Channels_Accessor_Field;
        ///<summary>
        ///Provides in-place access to the object field Channels.
        ///</summary>
        public unsafe StringAccessor Channels
        {
            get
            {
                
                byte* targetPtr = CellPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;

                Channels_Accessor_Field.CellPtr = targetPtr + 4;
                Channels_Accessor_Field.CellID = this.CellID;
                return Channels_Accessor_Field;
            }

            set
            {
                if ((object)value == null) throw new ArgumentNullException("The assigned variable is null.");
                byte* targetPtr = CellPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;

                Channels_Accessor_Field.CellID = this.CellID;
                
              int length = *(int*)(value.CellPtr - 4);

                //senario: cell_a.inlinks = cell_b.inlinks,
                //the later part will invoke the Get, filling cell_b.inlinks(a inlink_accessor_fiedld)'storage CellID
                int oldlength = *(int*)targetPtr;
                if (value.CellID != Channels_Accessor_Field.CellID)
                {
                    //if not in the same Cell
                    Channels_Accessor_Field.CellPtr = Channels_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                    Memory.Copy(value.CellPtr - 4, Channels_Accessor_Field.CellPtr, length + 4);
                }
                else
                {
                    byte[] tmpcell = new byte[length + 4];
                    fixed (byte* tmpcellptr = tmpcell)
                    {                        
                        Memory.Copy(value.CellPtr - 4, tmpcellptr, length + 4);
                        Channels_Accessor_Field.CellPtr = Channels_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                        Memory.Copy(tmpcellptr, Channels_Accessor_Field.CellPtr, length + 4);
                    }
                }
                Channels_Accessor_Field.CellPtr += 4;
                
            }

    }
StringAccessor Albums_Accessor_Field;
        ///<summary>
        ///Provides in-place access to the object field Albums.
        ///</summary>
        public unsafe StringAccessor Albums
        {
            get
            {
                
                byte* targetPtr = CellPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;

                Albums_Accessor_Field.CellPtr = targetPtr + 4;
                Albums_Accessor_Field.CellID = this.CellID;
                return Albums_Accessor_Field;
            }

            set
            {
                if ((object)value == null) throw new ArgumentNullException("The assigned variable is null.");
                byte* targetPtr = CellPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;

                Albums_Accessor_Field.CellID = this.CellID;
                
              int length = *(int*)(value.CellPtr - 4);

                //senario: cell_a.inlinks = cell_b.inlinks,
                //the later part will invoke the Get, filling cell_b.inlinks(a inlink_accessor_fiedld)'storage CellID
                int oldlength = *(int*)targetPtr;
                if (value.CellID != Albums_Accessor_Field.CellID)
                {
                    //if not in the same Cell
                    Albums_Accessor_Field.CellPtr = Albums_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                    Memory.Copy(value.CellPtr - 4, Albums_Accessor_Field.CellPtr, length + 4);
                }
                else
                {
                    byte[] tmpcell = new byte[length + 4];
                    fixed (byte* tmpcellptr = tmpcell)
                    {                        
                        Memory.Copy(value.CellPtr - 4, tmpcellptr, length + 4);
                        Albums_Accessor_Field.CellPtr = Albums_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                        Memory.Copy(tmpcellptr, Albums_Accessor_Field.CellPtr, length + 4);
                    }
                }
                Albums_Accessor_Field.CellPtr += 4;
                
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
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;

                Name_Accessor_Field.CellPtr = targetPtr + 4;
                Name_Accessor_Field.CellID = this.CellID;
                return Name_Accessor_Field;
            }

            set
            {
                if ((object)value == null) throw new ArgumentNullException("The assigned variable is null.");
                byte* targetPtr = CellPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;

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
StringAccessor Alias_Accessor_Field;
        ///<summary>
        ///Provides in-place access to the object field Alias.
        ///</summary>
        public unsafe StringAccessor Alias
        {
            get
            {
                
                byte* targetPtr = CellPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;

                Alias_Accessor_Field.CellPtr = targetPtr + 4;
                Alias_Accessor_Field.CellID = this.CellID;
                return Alias_Accessor_Field;
            }

            set
            {
                if ((object)value == null) throw new ArgumentNullException("The assigned variable is null.");
                byte* targetPtr = CellPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;

                Alias_Accessor_Field.CellID = this.CellID;
                
              int length = *(int*)(value.CellPtr - 4);

                //senario: cell_a.inlinks = cell_b.inlinks,
                //the later part will invoke the Get, filling cell_b.inlinks(a inlink_accessor_fiedld)'storage CellID
                int oldlength = *(int*)targetPtr;
                if (value.CellID != Alias_Accessor_Field.CellID)
                {
                    //if not in the same Cell
                    Alias_Accessor_Field.CellPtr = Alias_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                    Memory.Copy(value.CellPtr - 4, Alias_Accessor_Field.CellPtr, length + 4);
                }
                else
                {
                    byte[] tmpcell = new byte[length + 4];
                    fixed (byte* tmpcellptr = tmpcell)
                    {                        
                        Memory.Copy(value.CellPtr - 4, tmpcellptr, length + 4);
                        Alias_Accessor_Field.CellPtr = Alias_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                        Memory.Copy(tmpcellptr, Alias_Accessor_Field.CellPtr, length + 4);
                    }
                }
                Alias_Accessor_Field.CellPtr += 4;
                
            }

    }
StringAccessor Description_Accessor_Field;
        ///<summary>
        ///Provides in-place access to the object field Description.
        ///</summary>
        public unsafe StringAccessor Description
        {
            get
            {
                
                byte* targetPtr = CellPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;

                Description_Accessor_Field.CellPtr = targetPtr + 4;
                Description_Accessor_Field.CellID = this.CellID;
                return Description_Accessor_Field;
            }

            set
            {
                if ((object)value == null) throw new ArgumentNullException("The assigned variable is null.");
                byte* targetPtr = CellPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;

                Description_Accessor_Field.CellID = this.CellID;
                
              int length = *(int*)(value.CellPtr - 4);

                //senario: cell_a.inlinks = cell_b.inlinks,
                //the later part will invoke the Get, filling cell_b.inlinks(a inlink_accessor_fiedld)'storage CellID
                int oldlength = *(int*)targetPtr;
                if (value.CellID != Description_Accessor_Field.CellID)
                {
                    //if not in the same Cell
                    Description_Accessor_Field.CellPtr = Description_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                    Memory.Copy(value.CellPtr - 4, Description_Accessor_Field.CellPtr, length + 4);
                }
                else
                {
                    byte[] tmpcell = new byte[length + 4];
                    fixed (byte* tmpcellptr = tmpcell)
                    {                        
                        Memory.Copy(value.CellPtr - 4, tmpcellptr, length + 4);
                        Description_Accessor_Field.CellPtr = Description_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                        Memory.Copy(tmpcellptr, Description_Accessor_Field.CellPtr, length + 4);
                    }
                }
                Description_Accessor_Field.CellPtr += 4;
                
            }

    }
StringAccessor Segments_Accessor_Field;
        ///<summary>
        ///Provides in-place access to the object field Segments.
        ///</summary>
        public unsafe StringAccessor Segments
        {
            get
            {
                
                byte* targetPtr = CellPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;

                Segments_Accessor_Field.CellPtr = targetPtr + 4;
                Segments_Accessor_Field.CellID = this.CellID;
                return Segments_Accessor_Field;
            }

            set
            {
                if ((object)value == null) throw new ArgumentNullException("The assigned variable is null.");
                byte* targetPtr = CellPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;

                Segments_Accessor_Field.CellID = this.CellID;
                
              int length = *(int*)(value.CellPtr - 4);

                //senario: cell_a.inlinks = cell_b.inlinks,
                //the later part will invoke the Get, filling cell_b.inlinks(a inlink_accessor_fiedld)'storage CellID
                int oldlength = *(int*)targetPtr;
                if (value.CellID != Segments_Accessor_Field.CellID)
                {
                    //if not in the same Cell
                    Segments_Accessor_Field.CellPtr = Segments_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                    Memory.Copy(value.CellPtr - 4, Segments_Accessor_Field.CellPtr, length + 4);
                }
                else
                {
                    byte[] tmpcell = new byte[length + 4];
                    fixed (byte* tmpcellptr = tmpcell)
                    {                        
                        Memory.Copy(value.CellPtr - 4, tmpcellptr, length + 4);
                        Segments_Accessor_Field.CellPtr = Segments_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                        Memory.Copy(tmpcellptr, Segments_Accessor_Field.CellPtr, length + 4);
                    }
                }
                Segments_Accessor_Field.CellPtr += 4;
                
            }

    }
StringAccessor Categories_Accessor_Field;
        ///<summary>
        ///Provides in-place access to the object field Categories.
        ///</summary>
        public unsafe StringAccessor Categories
        {
            get
            {
                
                byte* targetPtr = CellPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;

                Categories_Accessor_Field.CellPtr = targetPtr + 4;
                Categories_Accessor_Field.CellID = this.CellID;
                return Categories_Accessor_Field;
            }

            set
            {
                if ((object)value == null) throw new ArgumentNullException("The assigned variable is null.");
                byte* targetPtr = CellPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;

                Categories_Accessor_Field.CellID = this.CellID;
                
              int length = *(int*)(value.CellPtr - 4);

                //senario: cell_a.inlinks = cell_b.inlinks,
                //the later part will invoke the Get, filling cell_b.inlinks(a inlink_accessor_fiedld)'storage CellID
                int oldlength = *(int*)targetPtr;
                if (value.CellID != Categories_Accessor_Field.CellID)
                {
                    //if not in the same Cell
                    Categories_Accessor_Field.CellPtr = Categories_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                    Memory.Copy(value.CellPtr - 4, Categories_Accessor_Field.CellPtr, length + 4);
                }
                else
                {
                    byte[] tmpcell = new byte[length + 4];
                    fixed (byte* tmpcellptr = tmpcell)
                    {                        
                        Memory.Copy(value.CellPtr - 4, tmpcellptr, length + 4);
                        Categories_Accessor_Field.CellPtr = Categories_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                        Memory.Copy(tmpcellptr, Categories_Accessor_Field.CellPtr, length + 4);
                    }
                }
                Categories_Accessor_Field.CellPtr += 4;
                
            }

    }
StringAccessor IntEmbeddedFilters_Accessor_Field;
        ///<summary>
        ///Provides in-place access to the object field IntEmbeddedFilters.
        ///</summary>
        public unsafe StringAccessor IntEmbeddedFilters
        {
            get
            {
                
                byte* targetPtr = CellPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;

                IntEmbeddedFilters_Accessor_Field.CellPtr = targetPtr + 4;
                IntEmbeddedFilters_Accessor_Field.CellID = this.CellID;
                return IntEmbeddedFilters_Accessor_Field;
            }

            set
            {
                if ((object)value == null) throw new ArgumentNullException("The assigned variable is null.");
                byte* targetPtr = CellPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;

                IntEmbeddedFilters_Accessor_Field.CellID = this.CellID;
                
              int length = *(int*)(value.CellPtr - 4);

                //senario: cell_a.inlinks = cell_b.inlinks,
                //the later part will invoke the Get, filling cell_b.inlinks(a inlink_accessor_fiedld)'storage CellID
                int oldlength = *(int*)targetPtr;
                if (value.CellID != IntEmbeddedFilters_Accessor_Field.CellID)
                {
                    //if not in the same Cell
                    IntEmbeddedFilters_Accessor_Field.CellPtr = IntEmbeddedFilters_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                    Memory.Copy(value.CellPtr - 4, IntEmbeddedFilters_Accessor_Field.CellPtr, length + 4);
                }
                else
                {
                    byte[] tmpcell = new byte[length + 4];
                    fixed (byte* tmpcellptr = tmpcell)
                    {                        
                        Memory.Copy(value.CellPtr - 4, tmpcellptr, length + 4);
                        IntEmbeddedFilters_Accessor_Field.CellPtr = IntEmbeddedFilters_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                        Memory.Copy(tmpcellptr, IntEmbeddedFilters_Accessor_Field.CellPtr, length + 4);
                    }
                }
                IntEmbeddedFilters_Accessor_Field.CellPtr += 4;
                
            }

    }
StringAccessor NumberOfWantToWatch_Accessor_Field;
        ///<summary>
        ///Provides in-place access to the object field NumberOfWantToWatch.
        ///</summary>
        public unsafe StringAccessor NumberOfWantToWatch
        {
            get
            {
                
                byte* targetPtr = CellPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;

                NumberOfWantToWatch_Accessor_Field.CellPtr = targetPtr + 4;
                NumberOfWantToWatch_Accessor_Field.CellID = this.CellID;
                return NumberOfWantToWatch_Accessor_Field;
            }

            set
            {
                if ((object)value == null) throw new ArgumentNullException("The assigned variable is null.");
                byte* targetPtr = CellPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;

                NumberOfWantToWatch_Accessor_Field.CellID = this.CellID;
                
              int length = *(int*)(value.CellPtr - 4);

                //senario: cell_a.inlinks = cell_b.inlinks,
                //the later part will invoke the Get, filling cell_b.inlinks(a inlink_accessor_fiedld)'storage CellID
                int oldlength = *(int*)targetPtr;
                if (value.CellID != NumberOfWantToWatch_Accessor_Field.CellID)
                {
                    //if not in the same Cell
                    NumberOfWantToWatch_Accessor_Field.CellPtr = NumberOfWantToWatch_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                    Memory.Copy(value.CellPtr - 4, NumberOfWantToWatch_Accessor_Field.CellPtr, length + 4);
                }
                else
                {
                    byte[] tmpcell = new byte[length + 4];
                    fixed (byte* tmpcellptr = tmpcell)
                    {                        
                        Memory.Copy(value.CellPtr - 4, tmpcellptr, length + 4);
                        NumberOfWantToWatch_Accessor_Field.CellPtr = NumberOfWantToWatch_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                        Memory.Copy(tmpcellptr, NumberOfWantToWatch_Accessor_Field.CellPtr, length + 4);
                    }
                }
                NumberOfWantToWatch_Accessor_Field.CellPtr += 4;
                
            }

    }
StringAccessor Rating_Accessor_Field;
        ///<summary>
        ///Provides in-place access to the object field Rating.
        ///</summary>
        public unsafe StringAccessor Rating
        {
            get
            {
                
                byte* targetPtr = CellPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;

                Rating_Accessor_Field.CellPtr = targetPtr + 4;
                Rating_Accessor_Field.CellID = this.CellID;
                return Rating_Accessor_Field;
            }

            set
            {
                if ((object)value == null) throw new ArgumentNullException("The assigned variable is null.");
                byte* targetPtr = CellPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;

                Rating_Accessor_Field.CellID = this.CellID;
                
              int length = *(int*)(value.CellPtr - 4);

                //senario: cell_a.inlinks = cell_b.inlinks,
                //the later part will invoke the Get, filling cell_b.inlinks(a inlink_accessor_fiedld)'storage CellID
                int oldlength = *(int*)targetPtr;
                if (value.CellID != Rating_Accessor_Field.CellID)
                {
                    //if not in the same Cell
                    Rating_Accessor_Field.CellPtr = Rating_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                    Memory.Copy(value.CellPtr - 4, Rating_Accessor_Field.CellPtr, length + 4);
                }
                else
                {
                    byte[] tmpcell = new byte[length + 4];
                    fixed (byte* tmpcellptr = tmpcell)
                    {                        
                        Memory.Copy(value.CellPtr - 4, tmpcellptr, length + 4);
                        Rating_Accessor_Field.CellPtr = Rating_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                        Memory.Copy(tmpcellptr, Rating_Accessor_Field.CellPtr, length + 4);
                    }
                }
                Rating_Accessor_Field.CellPtr += 4;
                
            }

    }
StringAccessor NumberOfShortReview_Accessor_Field;
        ///<summary>
        ///Provides in-place access to the object field NumberOfShortReview.
        ///</summary>
        public unsafe StringAccessor NumberOfShortReview
        {
            get
            {
                
                byte* targetPtr = CellPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;

                NumberOfShortReview_Accessor_Field.CellPtr = targetPtr + 4;
                NumberOfShortReview_Accessor_Field.CellID = this.CellID;
                return NumberOfShortReview_Accessor_Field;
            }

            set
            {
                if ((object)value == null) throw new ArgumentNullException("The assigned variable is null.");
                byte* targetPtr = CellPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;

                NumberOfShortReview_Accessor_Field.CellID = this.CellID;
                
              int length = *(int*)(value.CellPtr - 4);

                //senario: cell_a.inlinks = cell_b.inlinks,
                //the later part will invoke the Get, filling cell_b.inlinks(a inlink_accessor_fiedld)'storage CellID
                int oldlength = *(int*)targetPtr;
                if (value.CellID != NumberOfShortReview_Accessor_Field.CellID)
                {
                    //if not in the same Cell
                    NumberOfShortReview_Accessor_Field.CellPtr = NumberOfShortReview_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                    Memory.Copy(value.CellPtr - 4, NumberOfShortReview_Accessor_Field.CellPtr, length + 4);
                }
                else
                {
                    byte[] tmpcell = new byte[length + 4];
                    fixed (byte* tmpcellptr = tmpcell)
                    {                        
                        Memory.Copy(value.CellPtr - 4, tmpcellptr, length + 4);
                        NumberOfShortReview_Accessor_Field.CellPtr = NumberOfShortReview_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                        Memory.Copy(tmpcellptr, NumberOfShortReview_Accessor_Field.CellPtr, length + 4);
                    }
                }
                NumberOfShortReview_Accessor_Field.CellPtr += 4;
                
            }

    }
StringAccessor ReviewCount_Accessor_Field;
        ///<summary>
        ///Provides in-place access to the object field ReviewCount.
        ///</summary>
        public unsafe StringAccessor ReviewCount
        {
            get
            {
                
                byte* targetPtr = CellPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;

                ReviewCount_Accessor_Field.CellPtr = targetPtr + 4;
                ReviewCount_Accessor_Field.CellID = this.CellID;
                return ReviewCount_Accessor_Field;
            }

            set
            {
                if ((object)value == null) throw new ArgumentNullException("The assigned variable is null.");
                byte* targetPtr = CellPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;

                ReviewCount_Accessor_Field.CellID = this.CellID;
                
              int length = *(int*)(value.CellPtr - 4);

                //senario: cell_a.inlinks = cell_b.inlinks,
                //the later part will invoke the Get, filling cell_b.inlinks(a inlink_accessor_fiedld)'storage CellID
                int oldlength = *(int*)targetPtr;
                if (value.CellID != ReviewCount_Accessor_Field.CellID)
                {
                    //if not in the same Cell
                    ReviewCount_Accessor_Field.CellPtr = ReviewCount_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                    Memory.Copy(value.CellPtr - 4, ReviewCount_Accessor_Field.CellPtr, length + 4);
                }
                else
                {
                    byte[] tmpcell = new byte[length + 4];
                    fixed (byte* tmpcellptr = tmpcell)
                    {                        
                        Memory.Copy(value.CellPtr - 4, tmpcellptr, length + 4);
                        ReviewCount_Accessor_Field.CellPtr = ReviewCount_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                        Memory.Copy(tmpcellptr, ReviewCount_Accessor_Field.CellPtr, length + 4);
                    }
                }
                ReviewCount_Accessor_Field.CellPtr += 4;
                
            }

    }
StringAccessor NumberOfWatched_Accessor_Field;
        ///<summary>
        ///Provides in-place access to the object field NumberOfWatched.
        ///</summary>
        public unsafe StringAccessor NumberOfWatched
        {
            get
            {
                
                byte* targetPtr = CellPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;

                NumberOfWatched_Accessor_Field.CellPtr = targetPtr + 4;
                NumberOfWatched_Accessor_Field.CellID = this.CellID;
                return NumberOfWatched_Accessor_Field;
            }

            set
            {
                if ((object)value == null) throw new ArgumentNullException("The assigned variable is null.");
                byte* targetPtr = CellPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;

                NumberOfWatched_Accessor_Field.CellID = this.CellID;
                
              int length = *(int*)(value.CellPtr - 4);

                //senario: cell_a.inlinks = cell_b.inlinks,
                //the later part will invoke the Get, filling cell_b.inlinks(a inlink_accessor_fiedld)'storage CellID
                int oldlength = *(int*)targetPtr;
                if (value.CellID != NumberOfWatched_Accessor_Field.CellID)
                {
                    //if not in the same Cell
                    NumberOfWatched_Accessor_Field.CellPtr = NumberOfWatched_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                    Memory.Copy(value.CellPtr - 4, NumberOfWatched_Accessor_Field.CellPtr, length + 4);
                }
                else
                {
                    byte[] tmpcell = new byte[length + 4];
                    fixed (byte* tmpcellptr = tmpcell)
                    {                        
                        Memory.Copy(value.CellPtr - 4, tmpcellptr, length + 4);
                        NumberOfWatched_Accessor_Field.CellPtr = NumberOfWatched_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                        Memory.Copy(tmpcellptr, NumberOfWatched_Accessor_Field.CellPtr, length + 4);
                    }
                }
                NumberOfWatched_Accessor_Field.CellPtr += 4;
                
            }

    }
StringAccessor NumberOfReviewer_Accessor_Field;
        ///<summary>
        ///Provides in-place access to the object field NumberOfReviewer.
        ///</summary>
        public unsafe StringAccessor NumberOfReviewer
        {
            get
            {
                
                byte* targetPtr = CellPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;

                NumberOfReviewer_Accessor_Field.CellPtr = targetPtr + 4;
                NumberOfReviewer_Accessor_Field.CellID = this.CellID;
                return NumberOfReviewer_Accessor_Field;
            }

            set
            {
                if ((object)value == null) throw new ArgumentNullException("The assigned variable is null.");
                byte* targetPtr = CellPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;

                NumberOfReviewer_Accessor_Field.CellID = this.CellID;
                
              int length = *(int*)(value.CellPtr - 4);

                //senario: cell_a.inlinks = cell_b.inlinks,
                //the later part will invoke the Get, filling cell_b.inlinks(a inlink_accessor_fiedld)'storage CellID
                int oldlength = *(int*)targetPtr;
                if (value.CellID != NumberOfReviewer_Accessor_Field.CellID)
                {
                    //if not in the same Cell
                    NumberOfReviewer_Accessor_Field.CellPtr = NumberOfReviewer_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                    Memory.Copy(value.CellPtr - 4, NumberOfReviewer_Accessor_Field.CellPtr, length + 4);
                }
                else
                {
                    byte[] tmpcell = new byte[length + 4];
                    fixed (byte* tmpcellptr = tmpcell)
                    {                        
                        Memory.Copy(value.CellPtr - 4, tmpcellptr, length + 4);
                        NumberOfReviewer_Accessor_Field.CellPtr = NumberOfReviewer_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                        Memory.Copy(tmpcellptr, NumberOfReviewer_Accessor_Field.CellPtr, length + 4);
                    }
                }
                NumberOfReviewer_Accessor_Field.CellPtr += 4;
                
            }

    }
StringAccessor PublishDate_Accessor_Field;
        ///<summary>
        ///Provides in-place access to the object field PublishDate.
        ///</summary>
        public unsafe StringAccessor PublishDate
        {
            get
            {
                
                byte* targetPtr = CellPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;

                PublishDate_Accessor_Field.CellPtr = targetPtr + 4;
                PublishDate_Accessor_Field.CellID = this.CellID;
                return PublishDate_Accessor_Field;
            }

            set
            {
                if ((object)value == null) throw new ArgumentNullException("The assigned variable is null.");
                byte* targetPtr = CellPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;

                PublishDate_Accessor_Field.CellID = this.CellID;
                
              int length = *(int*)(value.CellPtr - 4);

                //senario: cell_a.inlinks = cell_b.inlinks,
                //the later part will invoke the Get, filling cell_b.inlinks(a inlink_accessor_fiedld)'storage CellID
                int oldlength = *(int*)targetPtr;
                if (value.CellID != PublishDate_Accessor_Field.CellID)
                {
                    //if not in the same Cell
                    PublishDate_Accessor_Field.CellPtr = PublishDate_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                    Memory.Copy(value.CellPtr - 4, PublishDate_Accessor_Field.CellPtr, length + 4);
                }
                else
                {
                    byte[] tmpcell = new byte[length + 4];
                    fixed (byte* tmpcellptr = tmpcell)
                    {                        
                        Memory.Copy(value.CellPtr - 4, tmpcellptr, length + 4);
                        PublishDate_Accessor_Field.CellPtr = PublishDate_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                        Memory.Copy(tmpcellptr, PublishDate_Accessor_Field.CellPtr, length + 4);
                    }
                }
                PublishDate_Accessor_Field.CellPtr += 4;
                
            }

    }
StringAccessor Length_Accessor_Field;
        ///<summary>
        ///Provides in-place access to the object field Length.
        ///</summary>
        public unsafe StringAccessor Length
        {
            get
            {
                
                byte* targetPtr = CellPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;

                Length_Accessor_Field.CellPtr = targetPtr + 4;
                Length_Accessor_Field.CellID = this.CellID;
                return Length_Accessor_Field;
            }

            set
            {
                if ((object)value == null) throw new ArgumentNullException("The assigned variable is null.");
                byte* targetPtr = CellPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;

                Length_Accessor_Field.CellID = this.CellID;
                
              int length = *(int*)(value.CellPtr - 4);

                //senario: cell_a.inlinks = cell_b.inlinks,
                //the later part will invoke the Get, filling cell_b.inlinks(a inlink_accessor_fiedld)'storage CellID
                int oldlength = *(int*)targetPtr;
                if (value.CellID != Length_Accessor_Field.CellID)
                {
                    //if not in the same Cell
                    Length_Accessor_Field.CellPtr = Length_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                    Memory.Copy(value.CellPtr - 4, Length_Accessor_Field.CellPtr, length + 4);
                }
                else
                {
                    byte[] tmpcell = new byte[length + 4];
                    fixed (byte* tmpcellptr = tmpcell)
                    {                        
                        Memory.Copy(value.CellPtr - 4, tmpcellptr, length + 4);
                        Length_Accessor_Field.CellPtr = Length_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                        Memory.Copy(tmpcellptr, Length_Accessor_Field.CellPtr, length + 4);
                    }
                }
                Length_Accessor_Field.CellPtr += 4;
                
            }

    }
StringAccessor Country_Accessor_Field;
        ///<summary>
        ///Provides in-place access to the object field Country.
        ///</summary>
        public unsafe StringAccessor Country
        {
            get
            {
                
                byte* targetPtr = CellPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;

                Country_Accessor_Field.CellPtr = targetPtr + 4;
                Country_Accessor_Field.CellID = this.CellID;
                return Country_Accessor_Field;
            }

            set
            {
                if ((object)value == null) throw new ArgumentNullException("The assigned variable is null.");
                byte* targetPtr = CellPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;

                Country_Accessor_Field.CellID = this.CellID;
                
              int length = *(int*)(value.CellPtr - 4);

                //senario: cell_a.inlinks = cell_b.inlinks,
                //the later part will invoke the Get, filling cell_b.inlinks(a inlink_accessor_fiedld)'storage CellID
                int oldlength = *(int*)targetPtr;
                if (value.CellID != Country_Accessor_Field.CellID)
                {
                    //if not in the same Cell
                    Country_Accessor_Field.CellPtr = Country_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                    Memory.Copy(value.CellPtr - 4, Country_Accessor_Field.CellPtr, length + 4);
                }
                else
                {
                    byte[] tmpcell = new byte[length + 4];
                    fixed (byte* tmpcellptr = tmpcell)
                    {                        
                        Memory.Copy(value.CellPtr - 4, tmpcellptr, length + 4);
                        Country_Accessor_Field.CellPtr = Country_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                        Memory.Copy(tmpcellptr, Country_Accessor_Field.CellPtr, length + 4);
                    }
                }
                Country_Accessor_Field.CellPtr += 4;
                
            }

    }
StringAccessor Language_Accessor_Field;
        ///<summary>
        ///Provides in-place access to the object field Language.
        ///</summary>
        public unsafe StringAccessor Language
        {
            get
            {
                
                byte* targetPtr = CellPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;

                Language_Accessor_Field.CellPtr = targetPtr + 4;
                Language_Accessor_Field.CellID = this.CellID;
                return Language_Accessor_Field;
            }

            set
            {
                if ((object)value == null) throw new ArgumentNullException("The assigned variable is null.");
                byte* targetPtr = CellPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;

                Language_Accessor_Field.CellID = this.CellID;
                
              int length = *(int*)(value.CellPtr - 4);

                //senario: cell_a.inlinks = cell_b.inlinks,
                //the later part will invoke the Get, filling cell_b.inlinks(a inlink_accessor_fiedld)'storage CellID
                int oldlength = *(int*)targetPtr;
                if (value.CellID != Language_Accessor_Field.CellID)
                {
                    //if not in the same Cell
                    Language_Accessor_Field.CellPtr = Language_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                    Memory.Copy(value.CellPtr - 4, Language_Accessor_Field.CellPtr, length + 4);
                }
                else
                {
                    byte[] tmpcell = new byte[length + 4];
                    fixed (byte* tmpcellptr = tmpcell)
                    {                        
                        Memory.Copy(value.CellPtr - 4, tmpcellptr, length + 4);
                        Language_Accessor_Field.CellPtr = Language_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                        Memory.Copy(tmpcellptr, Language_Accessor_Field.CellPtr, length + 4);
                    }
                }
                Language_Accessor_Field.CellPtr += 4;
                
            }

    }
StringAccessor SourceUrls_Accessor_Field;
        ///<summary>
        ///Provides in-place access to the object field SourceUrls.
        ///</summary>
        public unsafe StringAccessor SourceUrls
        {
            get
            {
                
                byte* targetPtr = CellPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;

                SourceUrls_Accessor_Field.CellPtr = targetPtr + 4;
                SourceUrls_Accessor_Field.CellID = this.CellID;
                return SourceUrls_Accessor_Field;
            }

            set
            {
                if ((object)value == null) throw new ArgumentNullException("The assigned variable is null.");
                byte* targetPtr = CellPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;

                SourceUrls_Accessor_Field.CellID = this.CellID;
                
              int length = *(int*)(value.CellPtr - 4);

                //senario: cell_a.inlinks = cell_b.inlinks,
                //the later part will invoke the Get, filling cell_b.inlinks(a inlink_accessor_fiedld)'storage CellID
                int oldlength = *(int*)targetPtr;
                if (value.CellID != SourceUrls_Accessor_Field.CellID)
                {
                    //if not in the same Cell
                    SourceUrls_Accessor_Field.CellPtr = SourceUrls_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                    Memory.Copy(value.CellPtr - 4, SourceUrls_Accessor_Field.CellPtr, length + 4);
                }
                else
                {
                    byte[] tmpcell = new byte[length + 4];
                    fixed (byte* tmpcellptr = tmpcell)
                    {                        
                        Memory.Copy(value.CellPtr - 4, tmpcellptr, length + 4);
                        SourceUrls_Accessor_Field.CellPtr = SourceUrls_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                        Memory.Copy(tmpcellptr, SourceUrls_Accessor_Field.CellPtr, length + 4);
                    }
                }
                SourceUrls_Accessor_Field.CellPtr += 4;
                
            }

    }
StringAccessor ImageUrls_Accessor_Field;
        ///<summary>
        ///Provides in-place access to the object field ImageUrls.
        ///</summary>
        public unsafe StringAccessor ImageUrls
        {
            get
            {
                
                byte* targetPtr = CellPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;

                ImageUrls_Accessor_Field.CellPtr = targetPtr + 4;
                ImageUrls_Accessor_Field.CellID = this.CellID;
                return ImageUrls_Accessor_Field;
            }

            set
            {
                if ((object)value == null) throw new ArgumentNullException("The assigned variable is null.");
                byte* targetPtr = CellPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;

                ImageUrls_Accessor_Field.CellID = this.CellID;
                
              int length = *(int*)(value.CellPtr - 4);

                //senario: cell_a.inlinks = cell_b.inlinks,
                //the later part will invoke the Get, filling cell_b.inlinks(a inlink_accessor_fiedld)'storage CellID
                int oldlength = *(int*)targetPtr;
                if (value.CellID != ImageUrls_Accessor_Field.CellID)
                {
                    //if not in the same Cell
                    ImageUrls_Accessor_Field.CellPtr = ImageUrls_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                    Memory.Copy(value.CellPtr - 4, ImageUrls_Accessor_Field.CellPtr, length + 4);
                }
                else
                {
                    byte[] tmpcell = new byte[length + 4];
                    fixed (byte* tmpcellptr = tmpcell)
                    {                        
                        Memory.Copy(value.CellPtr - 4, tmpcellptr, length + 4);
                        ImageUrls_Accessor_Field.CellPtr = ImageUrls_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                        Memory.Copy(tmpcellptr, ImageUrls_Accessor_Field.CellPtr, length + 4);
                    }
                }
                ImageUrls_Accessor_Field.CellPtr += 4;
                
            }

    }
StringAccessor OfficialSite_Accessor_Field;
        ///<summary>
        ///Provides in-place access to the object field OfficialSite.
        ///</summary>
        public unsafe StringAccessor OfficialSite
        {
            get
            {
                
                byte* targetPtr = CellPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;

                OfficialSite_Accessor_Field.CellPtr = targetPtr + 4;
                OfficialSite_Accessor_Field.CellID = this.CellID;
                return OfficialSite_Accessor_Field;
            }

            set
            {
                if ((object)value == null) throw new ArgumentNullException("The assigned variable is null.");
                byte* targetPtr = CellPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;

                OfficialSite_Accessor_Field.CellID = this.CellID;
                
              int length = *(int*)(value.CellPtr - 4);

                //senario: cell_a.inlinks = cell_b.inlinks,
                //the later part will invoke the Get, filling cell_b.inlinks(a inlink_accessor_fiedld)'storage CellID
                int oldlength = *(int*)targetPtr;
                if (value.CellID != OfficialSite_Accessor_Field.CellID)
                {
                    //if not in the same Cell
                    OfficialSite_Accessor_Field.CellPtr = OfficialSite_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                    Memory.Copy(value.CellPtr - 4, OfficialSite_Accessor_Field.CellPtr, length + 4);
                }
                else
                {
                    byte[] tmpcell = new byte[length + 4];
                    fixed (byte* tmpcellptr = tmpcell)
                    {                        
                        Memory.Copy(value.CellPtr - 4, tmpcellptr, length + 4);
                        OfficialSite_Accessor_Field.CellPtr = OfficialSite_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                        Memory.Copy(tmpcellptr, OfficialSite_Accessor_Field.CellPtr, length + 4);
                    }
                }
                OfficialSite_Accessor_Field.CellPtr += 4;
                
            }

    }
StringAccessor EntityContainer_Accessor_Field;
        ///<summary>
        ///Provides in-place access to the object field EntityContainer.
        ///</summary>
        public unsafe StringAccessor EntityContainer
        {
            get
            {
                
                byte* targetPtr = CellPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;

                EntityContainer_Accessor_Field.CellPtr = targetPtr + 4;
                EntityContainer_Accessor_Field.CellID = this.CellID;
                return EntityContainer_Accessor_Field;
            }

            set
            {
                if ((object)value == null) throw new ArgumentNullException("The assigned variable is null.");
                byte* targetPtr = CellPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;

                EntityContainer_Accessor_Field.CellID = this.CellID;
                
              int length = *(int*)(value.CellPtr - 4);

                //senario: cell_a.inlinks = cell_b.inlinks,
                //the later part will invoke the Get, filling cell_b.inlinks(a inlink_accessor_fiedld)'storage CellID
                int oldlength = *(int*)targetPtr;
                if (value.CellID != EntityContainer_Accessor_Field.CellID)
                {
                    //if not in the same Cell
                    EntityContainer_Accessor_Field.CellPtr = EntityContainer_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                    Memory.Copy(value.CellPtr - 4, EntityContainer_Accessor_Field.CellPtr, length + 4);
                }
                else
                {
                    byte[] tmpcell = new byte[length + 4];
                    fixed (byte* tmpcellptr = tmpcell)
                    {                        
                        Memory.Copy(value.CellPtr - 4, tmpcellptr, length + 4);
                        EntityContainer_Accessor_Field.CellPtr = EntityContainer_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                        Memory.Copy(tmpcellptr, EntityContainer_Accessor_Field.CellPtr, length + 4);
                    }
                }
                EntityContainer_Accessor_Field.CellPtr += 4;
                
            }

    }
StringAccessor Logo_Accessor_Field;
        ///<summary>
        ///Provides in-place access to the object field Logo.
        ///</summary>
        public unsafe StringAccessor Logo
        {
            get
            {
                
                byte* targetPtr = CellPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;

                Logo_Accessor_Field.CellPtr = targetPtr + 4;
                Logo_Accessor_Field.CellID = this.CellID;
                return Logo_Accessor_Field;
            }

            set
            {
                if ((object)value == null) throw new ArgumentNullException("The assigned variable is null.");
                byte* targetPtr = CellPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;

                Logo_Accessor_Field.CellID = this.CellID;
                
              int length = *(int*)(value.CellPtr - 4);

                //senario: cell_a.inlinks = cell_b.inlinks,
                //the later part will invoke the Get, filling cell_b.inlinks(a inlink_accessor_fiedld)'storage CellID
                int oldlength = *(int*)targetPtr;
                if (value.CellID != Logo_Accessor_Field.CellID)
                {
                    //if not in the same Cell
                    Logo_Accessor_Field.CellPtr = Logo_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                    Memory.Copy(value.CellPtr - 4, Logo_Accessor_Field.CellPtr, length + 4);
                }
                else
                {
                    byte[] tmpcell = new byte[length + 4];
                    fixed (byte* tmpcellptr = tmpcell)
                    {                        
                        Memory.Copy(value.CellPtr - 4, tmpcellptr, length + 4);
                        Logo_Accessor_Field.CellPtr = Logo_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                        Memory.Copy(tmpcellptr, Logo_Accessor_Field.CellPtr, length + 4);
                    }
                }
                Logo_Accessor_Field.CellPtr += 4;
                
            }

    }
StringAccessor QueryRank_Accessor_Field;
        ///<summary>
        ///Provides in-place access to the object field QueryRank.
        ///</summary>
        public unsafe StringAccessor QueryRank
        {
            get
            {
                
                byte* targetPtr = CellPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;

                QueryRank_Accessor_Field.CellPtr = targetPtr + 4;
                QueryRank_Accessor_Field.CellID = this.CellID;
                return QueryRank_Accessor_Field;
            }

            set
            {
                if ((object)value == null) throw new ArgumentNullException("The assigned variable is null.");
                byte* targetPtr = CellPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;

                QueryRank_Accessor_Field.CellID = this.CellID;
                
              int length = *(int*)(value.CellPtr - 4);

                //senario: cell_a.inlinks = cell_b.inlinks,
                //the later part will invoke the Get, filling cell_b.inlinks(a inlink_accessor_fiedld)'storage CellID
                int oldlength = *(int*)targetPtr;
                if (value.CellID != QueryRank_Accessor_Field.CellID)
                {
                    //if not in the same Cell
                    QueryRank_Accessor_Field.CellPtr = QueryRank_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                    Memory.Copy(value.CellPtr - 4, QueryRank_Accessor_Field.CellPtr, length + 4);
                }
                else
                {
                    byte[] tmpcell = new byte[length + 4];
                    fixed (byte* tmpcellptr = tmpcell)
                    {                        
                        Memory.Copy(value.CellPtr - 4, tmpcellptr, length + 4);
                        QueryRank_Accessor_Field.CellPtr = QueryRank_Accessor_Field.ResizeFunction(targetPtr, 0, length - oldlength);
                        Memory.Copy(tmpcellptr, QueryRank_Accessor_Field.CellPtr, length + 4);
                    }
                }
                QueryRank_Accessor_Field.CellPtr += 4;
                
            }

    }

        public static unsafe implicit operator Movie(Movie_Accessor accessor)
        {
            
            if(accessor.CellID != null)
            return new Movie(accessor.CellID.Value,accessor.Key,accessor.KGId,accessor.Genres,accessor.Artists,accessor.Directors,accessor.Characters,accessor.Performance,accessor.Distributors,accessor.Channels,accessor.Albums,accessor.Name,accessor.Alias,accessor.Description,accessor.Segments,accessor.Categories,accessor.IntEmbeddedFilters,accessor.NumberOfWantToWatch,accessor.Rating,accessor.NumberOfShortReview,accessor.ReviewCount,accessor.NumberOfWatched,accessor.NumberOfReviewer,accessor.PublishDate,accessor.Length,accessor.Country,accessor.Language,accessor.SourceUrls,accessor.ImageUrls,accessor.OfficialSite,accessor.EntityContainer,accessor.Logo,accessor.QueryRank);
            else
            return new Movie(accessor.Key,accessor.KGId,accessor.Genres,accessor.Artists,accessor.Directors,accessor.Characters,accessor.Performance,accessor.Distributors,accessor.Channels,accessor.Albums,accessor.Name,accessor.Alias,accessor.Description,accessor.Segments,accessor.Categories,accessor.IntEmbeddedFilters,accessor.NumberOfWantToWatch,accessor.Rating,accessor.NumberOfShortReview,accessor.ReviewCount,accessor.NumberOfWatched,accessor.NumberOfReviewer,accessor.PublishDate,accessor.Length,accessor.Country,accessor.Language,accessor.SourceUrls,accessor.ImageUrls,accessor.OfficialSite,accessor.EntityContainer,accessor.Logo,accessor.QueryRank);
        }

        public unsafe static implicit operator Movie_Accessor(Movie field)
        {  
            byte* targetPtr = null;
            
        {
        if(field.Key!= null)
        {
            int strlen_1 = field.Key.Length * 2;
            targetPtr += strlen_1+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(field.KGId!= null)
        {
            int strlen_1 = field.KGId.Length * 2;
            targetPtr += strlen_1+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

{

    targetPtr += sizeof(int);
    if(field.Genres!= null)
    {
        for(int iterator_1 = 0;iterator_1<field.Genres.Count;++iterator_1)
        {

        if(field.Genres[iterator_1]!= null)
        {
            int strlen_2 = field.Genres[iterator_1].Length * 2;
            targetPtr += strlen_2+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        }
    }

}

if(field.Artists!= null)
{
    targetPtr += field.Artists.Count*8+sizeof(int);
}else
{
    targetPtr += sizeof(int);
}


if(field.Directors!= null)
{
    targetPtr += field.Directors.Count*8+sizeof(int);
}else
{
    targetPtr += sizeof(int);
}


        if(field.Characters!= null)
        {
            int strlen_1 = field.Characters.Length * 2;
            targetPtr += strlen_1+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

{

    targetPtr += sizeof(int);
    if(field.Performance!= null)
    {
        for(int iterator_1 = 0;iterator_1<field.Performance.Count;++iterator_1)
        {

        if(field.Performance[iterator_1]!= null)
        {
            int strlen_2 = field.Performance[iterator_1].Length * 2;
            targetPtr += strlen_2+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        }
    }

}

        if(field.Distributors!= null)
        {
            int strlen_1 = field.Distributors.Length * 2;
            targetPtr += strlen_1+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(field.Channels!= null)
        {
            int strlen_1 = field.Channels.Length * 2;
            targetPtr += strlen_1+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(field.Albums!= null)
        {
            int strlen_1 = field.Albums.Length * 2;
            targetPtr += strlen_1+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(field.Name!= null)
        {
            int strlen_1 = field.Name.Length * 2;
            targetPtr += strlen_1+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(field.Alias!= null)
        {
            int strlen_1 = field.Alias.Length * 2;
            targetPtr += strlen_1+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(field.Description!= null)
        {
            int strlen_1 = field.Description.Length * 2;
            targetPtr += strlen_1+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(field.Segments!= null)
        {
            int strlen_1 = field.Segments.Length * 2;
            targetPtr += strlen_1+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(field.Categories!= null)
        {
            int strlen_1 = field.Categories.Length * 2;
            targetPtr += strlen_1+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(field.IntEmbeddedFilters!= null)
        {
            int strlen_1 = field.IntEmbeddedFilters.Length * 2;
            targetPtr += strlen_1+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(field.NumberOfWantToWatch!= null)
        {
            int strlen_1 = field.NumberOfWantToWatch.Length * 2;
            targetPtr += strlen_1+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(field.Rating!= null)
        {
            int strlen_1 = field.Rating.Length * 2;
            targetPtr += strlen_1+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(field.NumberOfShortReview!= null)
        {
            int strlen_1 = field.NumberOfShortReview.Length * 2;
            targetPtr += strlen_1+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(field.ReviewCount!= null)
        {
            int strlen_1 = field.ReviewCount.Length * 2;
            targetPtr += strlen_1+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(field.NumberOfWatched!= null)
        {
            int strlen_1 = field.NumberOfWatched.Length * 2;
            targetPtr += strlen_1+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(field.NumberOfReviewer!= null)
        {
            int strlen_1 = field.NumberOfReviewer.Length * 2;
            targetPtr += strlen_1+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(field.PublishDate!= null)
        {
            int strlen_1 = field.PublishDate.Length * 2;
            targetPtr += strlen_1+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(field.Length!= null)
        {
            int strlen_1 = field.Length.Length * 2;
            targetPtr += strlen_1+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(field.Country!= null)
        {
            int strlen_1 = field.Country.Length * 2;
            targetPtr += strlen_1+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(field.Language!= null)
        {
            int strlen_1 = field.Language.Length * 2;
            targetPtr += strlen_1+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(field.SourceUrls!= null)
        {
            int strlen_1 = field.SourceUrls.Length * 2;
            targetPtr += strlen_1+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(field.ImageUrls!= null)
        {
            int strlen_1 = field.ImageUrls.Length * 2;
            targetPtr += strlen_1+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(field.OfficialSite!= null)
        {
            int strlen_1 = field.OfficialSite.Length * 2;
            targetPtr += strlen_1+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(field.EntityContainer!= null)
        {
            int strlen_1 = field.EntityContainer.Length * 2;
            targetPtr += strlen_1+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(field.Logo!= null)
        {
            int strlen_1 = field.Logo.Length * 2;
            targetPtr += strlen_1+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        if(field.QueryRank!= null)
        {
            int strlen_1 = field.QueryRank.Length * 2;
            targetPtr += strlen_1+sizeof(int);
        }else
        {
            targetPtr += sizeof(int);
        }

        }
            byte* tmpcellptr = BufferAllocator.AllocBuffer((int)targetPtr);
            Memory.memset(tmpcellptr, 0, (ulong)targetPtr);
            targetPtr = tmpcellptr;
        
        {
        if(field.Key!= null)
        {
            int strlen_1 = field.Key.Length * 2;
            *(int*)targetPtr = strlen_1;
            targetPtr += sizeof(int);
            fixed(char* pstr_1 = field.Key)
            {
                Memory.Copy(pstr_1, targetPtr, strlen_1);
                targetPtr += strlen_1;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(field.KGId!= null)
        {
            int strlen_1 = field.KGId.Length * 2;
            *(int*)targetPtr = strlen_1;
            targetPtr += sizeof(int);
            fixed(char* pstr_1 = field.KGId)
            {
                Memory.Copy(pstr_1, targetPtr, strlen_1);
                targetPtr += strlen_1;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

{
byte *storedPtr_1 = targetPtr;

    targetPtr += sizeof(int);
    if(field.Genres!= null)
    {
        for(int iterator_1 = 0;iterator_1<field.Genres.Count;++iterator_1)
        {

        if(field.Genres[iterator_1]!= null)
        {
            int strlen_2 = field.Genres[iterator_1].Length * 2;
            *(int*)targetPtr = strlen_2;
            targetPtr += sizeof(int);
            fixed(char* pstr_2 = field.Genres[iterator_1])
            {
                Memory.Copy(pstr_2, targetPtr, strlen_2);
                targetPtr += strlen_2;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        }
    }
*(int*)storedPtr_1 = (int)(targetPtr - storedPtr_1 - 4);

}

if(field.Artists!= null)
{
    *(int*)targetPtr = field.Artists.Count*8;
    targetPtr += sizeof(int);
    for(int iterator_1 = 0;iterator_1<field.Artists.Count;++iterator_1)
    {
*(long*)targetPtr = field.Artists[iterator_1];
            targetPtr += 8;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

if(field.Directors!= null)
{
    *(int*)targetPtr = field.Directors.Count*8;
    targetPtr += sizeof(int);
    for(int iterator_1 = 0;iterator_1<field.Directors.Count;++iterator_1)
    {
*(long*)targetPtr = field.Directors[iterator_1];
            targetPtr += 8;

    }

}else
{
    *(int*)targetPtr = 0;
    targetPtr += sizeof(int);
}

        if(field.Characters!= null)
        {
            int strlen_1 = field.Characters.Length * 2;
            *(int*)targetPtr = strlen_1;
            targetPtr += sizeof(int);
            fixed(char* pstr_1 = field.Characters)
            {
                Memory.Copy(pstr_1, targetPtr, strlen_1);
                targetPtr += strlen_1;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

{
byte *storedPtr_1 = targetPtr;

    targetPtr += sizeof(int);
    if(field.Performance!= null)
    {
        for(int iterator_1 = 0;iterator_1<field.Performance.Count;++iterator_1)
        {

        if(field.Performance[iterator_1]!= null)
        {
            int strlen_2 = field.Performance[iterator_1].Length * 2;
            *(int*)targetPtr = strlen_2;
            targetPtr += sizeof(int);
            fixed(char* pstr_2 = field.Performance[iterator_1])
            {
                Memory.Copy(pstr_2, targetPtr, strlen_2);
                targetPtr += strlen_2;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        }
    }
*(int*)storedPtr_1 = (int)(targetPtr - storedPtr_1 - 4);

}

        if(field.Distributors!= null)
        {
            int strlen_1 = field.Distributors.Length * 2;
            *(int*)targetPtr = strlen_1;
            targetPtr += sizeof(int);
            fixed(char* pstr_1 = field.Distributors)
            {
                Memory.Copy(pstr_1, targetPtr, strlen_1);
                targetPtr += strlen_1;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(field.Channels!= null)
        {
            int strlen_1 = field.Channels.Length * 2;
            *(int*)targetPtr = strlen_1;
            targetPtr += sizeof(int);
            fixed(char* pstr_1 = field.Channels)
            {
                Memory.Copy(pstr_1, targetPtr, strlen_1);
                targetPtr += strlen_1;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(field.Albums!= null)
        {
            int strlen_1 = field.Albums.Length * 2;
            *(int*)targetPtr = strlen_1;
            targetPtr += sizeof(int);
            fixed(char* pstr_1 = field.Albums)
            {
                Memory.Copy(pstr_1, targetPtr, strlen_1);
                targetPtr += strlen_1;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

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

        if(field.Alias!= null)
        {
            int strlen_1 = field.Alias.Length * 2;
            *(int*)targetPtr = strlen_1;
            targetPtr += sizeof(int);
            fixed(char* pstr_1 = field.Alias)
            {
                Memory.Copy(pstr_1, targetPtr, strlen_1);
                targetPtr += strlen_1;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(field.Description!= null)
        {
            int strlen_1 = field.Description.Length * 2;
            *(int*)targetPtr = strlen_1;
            targetPtr += sizeof(int);
            fixed(char* pstr_1 = field.Description)
            {
                Memory.Copy(pstr_1, targetPtr, strlen_1);
                targetPtr += strlen_1;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(field.Segments!= null)
        {
            int strlen_1 = field.Segments.Length * 2;
            *(int*)targetPtr = strlen_1;
            targetPtr += sizeof(int);
            fixed(char* pstr_1 = field.Segments)
            {
                Memory.Copy(pstr_1, targetPtr, strlen_1);
                targetPtr += strlen_1;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(field.Categories!= null)
        {
            int strlen_1 = field.Categories.Length * 2;
            *(int*)targetPtr = strlen_1;
            targetPtr += sizeof(int);
            fixed(char* pstr_1 = field.Categories)
            {
                Memory.Copy(pstr_1, targetPtr, strlen_1);
                targetPtr += strlen_1;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(field.IntEmbeddedFilters!= null)
        {
            int strlen_1 = field.IntEmbeddedFilters.Length * 2;
            *(int*)targetPtr = strlen_1;
            targetPtr += sizeof(int);
            fixed(char* pstr_1 = field.IntEmbeddedFilters)
            {
                Memory.Copy(pstr_1, targetPtr, strlen_1);
                targetPtr += strlen_1;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(field.NumberOfWantToWatch!= null)
        {
            int strlen_1 = field.NumberOfWantToWatch.Length * 2;
            *(int*)targetPtr = strlen_1;
            targetPtr += sizeof(int);
            fixed(char* pstr_1 = field.NumberOfWantToWatch)
            {
                Memory.Copy(pstr_1, targetPtr, strlen_1);
                targetPtr += strlen_1;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(field.Rating!= null)
        {
            int strlen_1 = field.Rating.Length * 2;
            *(int*)targetPtr = strlen_1;
            targetPtr += sizeof(int);
            fixed(char* pstr_1 = field.Rating)
            {
                Memory.Copy(pstr_1, targetPtr, strlen_1);
                targetPtr += strlen_1;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(field.NumberOfShortReview!= null)
        {
            int strlen_1 = field.NumberOfShortReview.Length * 2;
            *(int*)targetPtr = strlen_1;
            targetPtr += sizeof(int);
            fixed(char* pstr_1 = field.NumberOfShortReview)
            {
                Memory.Copy(pstr_1, targetPtr, strlen_1);
                targetPtr += strlen_1;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(field.ReviewCount!= null)
        {
            int strlen_1 = field.ReviewCount.Length * 2;
            *(int*)targetPtr = strlen_1;
            targetPtr += sizeof(int);
            fixed(char* pstr_1 = field.ReviewCount)
            {
                Memory.Copy(pstr_1, targetPtr, strlen_1);
                targetPtr += strlen_1;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(field.NumberOfWatched!= null)
        {
            int strlen_1 = field.NumberOfWatched.Length * 2;
            *(int*)targetPtr = strlen_1;
            targetPtr += sizeof(int);
            fixed(char* pstr_1 = field.NumberOfWatched)
            {
                Memory.Copy(pstr_1, targetPtr, strlen_1);
                targetPtr += strlen_1;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(field.NumberOfReviewer!= null)
        {
            int strlen_1 = field.NumberOfReviewer.Length * 2;
            *(int*)targetPtr = strlen_1;
            targetPtr += sizeof(int);
            fixed(char* pstr_1 = field.NumberOfReviewer)
            {
                Memory.Copy(pstr_1, targetPtr, strlen_1);
                targetPtr += strlen_1;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(field.PublishDate!= null)
        {
            int strlen_1 = field.PublishDate.Length * 2;
            *(int*)targetPtr = strlen_1;
            targetPtr += sizeof(int);
            fixed(char* pstr_1 = field.PublishDate)
            {
                Memory.Copy(pstr_1, targetPtr, strlen_1);
                targetPtr += strlen_1;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(field.Length!= null)
        {
            int strlen_1 = field.Length.Length * 2;
            *(int*)targetPtr = strlen_1;
            targetPtr += sizeof(int);
            fixed(char* pstr_1 = field.Length)
            {
                Memory.Copy(pstr_1, targetPtr, strlen_1);
                targetPtr += strlen_1;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(field.Country!= null)
        {
            int strlen_1 = field.Country.Length * 2;
            *(int*)targetPtr = strlen_1;
            targetPtr += sizeof(int);
            fixed(char* pstr_1 = field.Country)
            {
                Memory.Copy(pstr_1, targetPtr, strlen_1);
                targetPtr += strlen_1;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(field.Language!= null)
        {
            int strlen_1 = field.Language.Length * 2;
            *(int*)targetPtr = strlen_1;
            targetPtr += sizeof(int);
            fixed(char* pstr_1 = field.Language)
            {
                Memory.Copy(pstr_1, targetPtr, strlen_1);
                targetPtr += strlen_1;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(field.SourceUrls!= null)
        {
            int strlen_1 = field.SourceUrls.Length * 2;
            *(int*)targetPtr = strlen_1;
            targetPtr += sizeof(int);
            fixed(char* pstr_1 = field.SourceUrls)
            {
                Memory.Copy(pstr_1, targetPtr, strlen_1);
                targetPtr += strlen_1;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(field.ImageUrls!= null)
        {
            int strlen_1 = field.ImageUrls.Length * 2;
            *(int*)targetPtr = strlen_1;
            targetPtr += sizeof(int);
            fixed(char* pstr_1 = field.ImageUrls)
            {
                Memory.Copy(pstr_1, targetPtr, strlen_1);
                targetPtr += strlen_1;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(field.OfficialSite!= null)
        {
            int strlen_1 = field.OfficialSite.Length * 2;
            *(int*)targetPtr = strlen_1;
            targetPtr += sizeof(int);
            fixed(char* pstr_1 = field.OfficialSite)
            {
                Memory.Copy(pstr_1, targetPtr, strlen_1);
                targetPtr += strlen_1;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(field.EntityContainer!= null)
        {
            int strlen_1 = field.EntityContainer.Length * 2;
            *(int*)targetPtr = strlen_1;
            targetPtr += sizeof(int);
            fixed(char* pstr_1 = field.EntityContainer)
            {
                Memory.Copy(pstr_1, targetPtr, strlen_1);
                targetPtr += strlen_1;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(field.Logo!= null)
        {
            int strlen_1 = field.Logo.Length * 2;
            *(int*)targetPtr = strlen_1;
            targetPtr += sizeof(int);
            fixed(char* pstr_1 = field.Logo)
            {
                Memory.Copy(pstr_1, targetPtr, strlen_1);
                targetPtr += strlen_1;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        if(field.QueryRank!= null)
        {
            int strlen_1 = field.QueryRank.Length * 2;
            *(int*)targetPtr = strlen_1;
            targetPtr += sizeof(int);
            fixed(char* pstr_1 = field.QueryRank)
            {
                Memory.Copy(pstr_1, targetPtr, strlen_1);
                targetPtr += strlen_1;
            }
        }else
        {
            *(int*)targetPtr = 0;
            targetPtr += sizeof(int);
        }

        }
            Movie_Accessor ret = new Movie_Accessor(tmpcellptr);
            ret.CellID = field.CellID;
            return ret;
        }


        public static bool operator == (Movie_Accessor a, Movie_Accessor b)
        {
            if (ReferenceEquals(a, b))
              return true;
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
              return false;
            // If both are same instance, return true.
            if (a.CellPtr == b.CellPtr) return true;
            byte* targetPtr = a.CellPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;

            int lengthA = (int)(targetPtr - a.CellPtr);
            targetPtr = b.CellPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;
            targetPtr += 4 + *(int*)targetPtr;

            int lengthB = (int)(targetPtr - b.CellPtr);
            if(lengthA != lengthB) return false;
            return Memory.Compare(a.CellPtr,b.CellPtr,lengthA);
        }

        public static bool operator != (Movie_Accessor a, Movie_Accessor b)
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
    
    /// <summary>
    /// A .NET runtime object representation of Movie defined in TSL.
    /// </summary>
    public partial struct Movie : ICell
    {
        #region MUTE
        
        #endregion
        #region Text processing
        /// <summary>
        /// Converts the string representation of a Movie to its
        /// struct equivalent. A return value indicates whether the 
        /// operation succeeded.
        /// </summary>
        /// <param name="input>A string to convert.</param>
        /// <param name="value">
        /// When this method returns, contains the struct equivalent of the value contained 
        /// in input, if the conversion succeeded, or default(Movie) if the conversion
        /// failed. The conversion fails if the input parameter is null or String.Empty, or is 
        /// not of the correct format. This parameter is passed uninitialized.
        /// </param>
        /// <returns>
        /// True if input was converted successfully; otherwise, false.
        /// </returns>
        public static bool TryParse(string input, out Movie value)
        {
            try
            {
                value = Newtonsoft.Json.JsonConvert.DeserializeObject<Movie>(input);
                return true;
            }
            catch { value = default(Movie); return false; }
        }
        public static Movie Parse(string input)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<Movie>(input);
        }
        ///<summary>Converts a Movie to its string representation, in JSON format.</summary>
        ///<returns>A string representation of the Movie.</returns>
        public override string ToString()
        {
            return Serializer.ToString(this);
        }
        #endregion
        internal static StringLookupTable FieldLookupTable = new StringLookupTable(
            
            "Key"
            ,
            "KGId"
            ,
            "Genres"
            ,
            "Artists"
            ,
            "Directors"
            ,
            "Characters"
            ,
            "Performance"
            ,
            "Distributors"
            ,
            "Channels"
            ,
            "Albums"
            ,
            "Name"
            ,
            "Alias"
            ,
            "Description"
            ,
            "Segments"
            ,
            "Categories"
            ,
            "IntEmbeddedFilters"
            ,
            "NumberOfWantToWatch"
            ,
            "Rating"
            ,
            "NumberOfShortReview"
            ,
            "ReviewCount"
            ,
            "NumberOfWatched"
            ,
            "NumberOfReviewer"
            ,
            "PublishDate"
            ,
            "Length"
            ,
            "Country"
            ,
            "Language"
            ,
            "SourceUrls"
            ,
            "ImageUrls"
            ,
            "OfficialSite"
            ,
            "EntityContainer"
            ,
            "Logo"
            ,
            "QueryRank"
            
            );
        internal static HashSet<string> AppendToFieldRerouteSet = new HashSet<string>()
        {
            
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
                    return TypeConverter<T>.ConvertFrom_string(this.Key);
                
                case 1:
                    return TypeConverter<T>.ConvertFrom_string(this.KGId);
                
                case 2:
                    return TypeConverter<T>.ConvertFrom_List_string(this.Genres);
                
                case 3:
                    return TypeConverter<T>.ConvertFrom_List_long(this.Artists);
                
                case 4:
                    return TypeConverter<T>.ConvertFrom_List_long(this.Directors);
                
                case 5:
                    return TypeConverter<T>.ConvertFrom_string(this.Characters);
                
                case 6:
                    return TypeConverter<T>.ConvertFrom_List_string(this.Performance);
                
                case 7:
                    return TypeConverter<T>.ConvertFrom_string(this.Distributors);
                
                case 8:
                    return TypeConverter<T>.ConvertFrom_string(this.Channels);
                
                case 9:
                    return TypeConverter<T>.ConvertFrom_string(this.Albums);
                
                case 10:
                    return TypeConverter<T>.ConvertFrom_string(this.Name);
                
                case 11:
                    return TypeConverter<T>.ConvertFrom_string(this.Alias);
                
                case 12:
                    return TypeConverter<T>.ConvertFrom_string(this.Description);
                
                case 13:
                    return TypeConverter<T>.ConvertFrom_string(this.Segments);
                
                case 14:
                    return TypeConverter<T>.ConvertFrom_string(this.Categories);
                
                case 15:
                    return TypeConverter<T>.ConvertFrom_string(this.IntEmbeddedFilters);
                
                case 16:
                    return TypeConverter<T>.ConvertFrom_string(this.NumberOfWantToWatch);
                
                case 17:
                    return TypeConverter<T>.ConvertFrom_string(this.Rating);
                
                case 18:
                    return TypeConverter<T>.ConvertFrom_string(this.NumberOfShortReview);
                
                case 19:
                    return TypeConverter<T>.ConvertFrom_string(this.ReviewCount);
                
                case 20:
                    return TypeConverter<T>.ConvertFrom_string(this.NumberOfWatched);
                
                case 21:
                    return TypeConverter<T>.ConvertFrom_string(this.NumberOfReviewer);
                
                case 22:
                    return TypeConverter<T>.ConvertFrom_string(this.PublishDate);
                
                case 23:
                    return TypeConverter<T>.ConvertFrom_string(this.Length);
                
                case 24:
                    return TypeConverter<T>.ConvertFrom_string(this.Country);
                
                case 25:
                    return TypeConverter<T>.ConvertFrom_string(this.Language);
                
                case 26:
                    return TypeConverter<T>.ConvertFrom_string(this.SourceUrls);
                
                case 27:
                    return TypeConverter<T>.ConvertFrom_string(this.ImageUrls);
                
                case 28:
                    return TypeConverter<T>.ConvertFrom_string(this.OfficialSite);
                
                case 29:
                    return TypeConverter<T>.ConvertFrom_string(this.EntityContainer);
                
                case 30:
                    return TypeConverter<T>.ConvertFrom_string(this.Logo);
                
                case 31:
                    return TypeConverter<T>.ConvertFrom_string(this.QueryRank);
                
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
                    this.Key = TypeConverter<T>.ConvertTo_string(value);
                    break;
                
                case 1:
                    this.KGId = TypeConverter<T>.ConvertTo_string(value);
                    break;
                
                case 2:
                    this.Genres = TypeConverter<T>.ConvertTo_List_string(value);
                    break;
                
                case 3:
                    this.Artists = TypeConverter<T>.ConvertTo_List_long(value);
                    break;
                
                case 4:
                    this.Directors = TypeConverter<T>.ConvertTo_List_long(value);
                    break;
                
                case 5:
                    this.Characters = TypeConverter<T>.ConvertTo_string(value);
                    break;
                
                case 6:
                    this.Performance = TypeConverter<T>.ConvertTo_List_string(value);
                    break;
                
                case 7:
                    this.Distributors = TypeConverter<T>.ConvertTo_string(value);
                    break;
                
                case 8:
                    this.Channels = TypeConverter<T>.ConvertTo_string(value);
                    break;
                
                case 9:
                    this.Albums = TypeConverter<T>.ConvertTo_string(value);
                    break;
                
                case 10:
                    this.Name = TypeConverter<T>.ConvertTo_string(value);
                    break;
                
                case 11:
                    this.Alias = TypeConverter<T>.ConvertTo_string(value);
                    break;
                
                case 12:
                    this.Description = TypeConverter<T>.ConvertTo_string(value);
                    break;
                
                case 13:
                    this.Segments = TypeConverter<T>.ConvertTo_string(value);
                    break;
                
                case 14:
                    this.Categories = TypeConverter<T>.ConvertTo_string(value);
                    break;
                
                case 15:
                    this.IntEmbeddedFilters = TypeConverter<T>.ConvertTo_string(value);
                    break;
                
                case 16:
                    this.NumberOfWantToWatch = TypeConverter<T>.ConvertTo_string(value);
                    break;
                
                case 17:
                    this.Rating = TypeConverter<T>.ConvertTo_string(value);
                    break;
                
                case 18:
                    this.NumberOfShortReview = TypeConverter<T>.ConvertTo_string(value);
                    break;
                
                case 19:
                    this.ReviewCount = TypeConverter<T>.ConvertTo_string(value);
                    break;
                
                case 20:
                    this.NumberOfWatched = TypeConverter<T>.ConvertTo_string(value);
                    break;
                
                case 21:
                    this.NumberOfReviewer = TypeConverter<T>.ConvertTo_string(value);
                    break;
                
                case 22:
                    this.PublishDate = TypeConverter<T>.ConvertTo_string(value);
                    break;
                
                case 23:
                    this.Length = TypeConverter<T>.ConvertTo_string(value);
                    break;
                
                case 24:
                    this.Country = TypeConverter<T>.ConvertTo_string(value);
                    break;
                
                case 25:
                    this.Language = TypeConverter<T>.ConvertTo_string(value);
                    break;
                
                case 26:
                    this.SourceUrls = TypeConverter<T>.ConvertTo_string(value);
                    break;
                
                case 27:
                    this.ImageUrls = TypeConverter<T>.ConvertTo_string(value);
                    break;
                
                case 28:
                    this.OfficialSite = TypeConverter<T>.ConvertTo_string(value);
                    break;
                
                case 29:
                    this.EntityContainer = TypeConverter<T>.ConvertTo_string(value);
                    break;
                
                case 30:
                    this.Logo = TypeConverter<T>.ConvertTo_string(value);
                    break;
                
                case 31:
                    this.QueryRank = TypeConverter<T>.ConvertTo_string(value);
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
                    
                case 8:
                    
                    return true;
                    
                case 9:
                    
                    return true;
                    
                case 10:
                    
                    return true;
                    
                case 11:
                    
                    return true;
                    
                case 12:
                    
                    return true;
                    
                case 13:
                    
                    return true;
                    
                case 14:
                    
                    return true;
                    
                case 15:
                    
                    return true;
                    
                case 16:
                    
                    return true;
                    
                case 17:
                    
                    return true;
                    
                case 18:
                    
                    return true;
                    
                case 19:
                    
                    return true;
                    
                case 20:
                    
                    return true;
                    
                case 21:
                    
                    return true;
                    
                case 22:
                    
                    return true;
                    
                case 23:
                    
                    return true;
                    
                case 24:
                    
                    return true;
                    
                case 25:
                    
                    return true;
                    
                case 26:
                    
                    return true;
                    
                case 27:
                    
                    return true;
                    
                case 28:
                    
                    return true;
                    
                case 29:
                    
                    return true;
                    
                case 30:
                    
                    return true;
                    
                case 31:
                    
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
                
                case 0:
                    
                    {
                        if (this.Key == null)
                            this.Key = TypeConverter<T>.ConvertTo_string(value);
                        else
                            this.Key += TypeConverter<T>.ConvertTo_string(value);
                    }
                    
                    break;
                
                case 1:
                    
                    {
                        if (this.KGId == null)
                            this.KGId = TypeConverter<T>.ConvertTo_string(value);
                        else
                            this.KGId += TypeConverter<T>.ConvertTo_string(value);
                    }
                    
                    break;
                
                case 2:
                    
                    {
                        if (this.Genres == null)
                            this.Genres = new List<string>();
                        switch (TypeConverter<T>.GetConversionActionTo_List_string())
                        {
                            case TypeConversionAction.TC_ASSIGN:
                                foreach (var element in value as List<string>)
                                    this.Genres.Add(element);
                                break;
                            case TypeConversionAction.TC_CONVERTLIST:
                            case TypeConversionAction.TC_ARRAYTOLIST:
                                foreach (var element in TypeConverter<T>.Enumerate_string(value))
                                    this.Genres.Add(element);
                                break;
                            case TypeConversionAction.TC_WRAPINLIST:
                            case TypeConversionAction.TC_PARSESTRING:
                                this.Genres.Add(TypeConverter<T>.ConvertTo_string(value));
                                break;
                            default:
                                Throw.data_type_incompatible_with_list(typeof(T).ToString());
                                break;
                        }
                    }
                    
                    break;
                
                case 3:
                    
                    {
                        if (this.Artists == null)
                            this.Artists = new List<long>();
                        switch (TypeConverter<T>.GetConversionActionTo_List_long())
                        {
                            case TypeConversionAction.TC_ASSIGN:
                                foreach (var element in value as List<long>)
                                    this.Artists.Add(element);
                                break;
                            case TypeConversionAction.TC_CONVERTLIST:
                            case TypeConversionAction.TC_ARRAYTOLIST:
                                foreach (var element in TypeConverter<T>.Enumerate_long(value))
                                    this.Artists.Add(element);
                                break;
                            case TypeConversionAction.TC_WRAPINLIST:
                            case TypeConversionAction.TC_PARSESTRING:
                                this.Artists.Add(TypeConverter<T>.ConvertTo_long(value));
                                break;
                            default:
                                Throw.data_type_incompatible_with_list(typeof(T).ToString());
                                break;
                        }
                    }
                    
                    break;
                
                case 4:
                    
                    {
                        if (this.Directors == null)
                            this.Directors = new List<long>();
                        switch (TypeConverter<T>.GetConversionActionTo_List_long())
                        {
                            case TypeConversionAction.TC_ASSIGN:
                                foreach (var element in value as List<long>)
                                    this.Directors.Add(element);
                                break;
                            case TypeConversionAction.TC_CONVERTLIST:
                            case TypeConversionAction.TC_ARRAYTOLIST:
                                foreach (var element in TypeConverter<T>.Enumerate_long(value))
                                    this.Directors.Add(element);
                                break;
                            case TypeConversionAction.TC_WRAPINLIST:
                            case TypeConversionAction.TC_PARSESTRING:
                                this.Directors.Add(TypeConverter<T>.ConvertTo_long(value));
                                break;
                            default:
                                Throw.data_type_incompatible_with_list(typeof(T).ToString());
                                break;
                        }
                    }
                    
                    break;
                
                case 5:
                    
                    {
                        if (this.Characters == null)
                            this.Characters = TypeConverter<T>.ConvertTo_string(value);
                        else
                            this.Characters += TypeConverter<T>.ConvertTo_string(value);
                    }
                    
                    break;
                
                case 6:
                    
                    {
                        if (this.Performance == null)
                            this.Performance = new List<string>();
                        switch (TypeConverter<T>.GetConversionActionTo_List_string())
                        {
                            case TypeConversionAction.TC_ASSIGN:
                                foreach (var element in value as List<string>)
                                    this.Performance.Add(element);
                                break;
                            case TypeConversionAction.TC_CONVERTLIST:
                            case TypeConversionAction.TC_ARRAYTOLIST:
                                foreach (var element in TypeConverter<T>.Enumerate_string(value))
                                    this.Performance.Add(element);
                                break;
                            case TypeConversionAction.TC_WRAPINLIST:
                            case TypeConversionAction.TC_PARSESTRING:
                                this.Performance.Add(TypeConverter<T>.ConvertTo_string(value));
                                break;
                            default:
                                Throw.data_type_incompatible_with_list(typeof(T).ToString());
                                break;
                        }
                    }
                    
                    break;
                
                case 7:
                    
                    {
                        if (this.Distributors == null)
                            this.Distributors = TypeConverter<T>.ConvertTo_string(value);
                        else
                            this.Distributors += TypeConverter<T>.ConvertTo_string(value);
                    }
                    
                    break;
                
                case 8:
                    
                    {
                        if (this.Channels == null)
                            this.Channels = TypeConverter<T>.ConvertTo_string(value);
                        else
                            this.Channels += TypeConverter<T>.ConvertTo_string(value);
                    }
                    
                    break;
                
                case 9:
                    
                    {
                        if (this.Albums == null)
                            this.Albums = TypeConverter<T>.ConvertTo_string(value);
                        else
                            this.Albums += TypeConverter<T>.ConvertTo_string(value);
                    }
                    
                    break;
                
                case 10:
                    
                    {
                        if (this.Name == null)
                            this.Name = TypeConverter<T>.ConvertTo_string(value);
                        else
                            this.Name += TypeConverter<T>.ConvertTo_string(value);
                    }
                    
                    break;
                
                case 11:
                    
                    {
                        if (this.Alias == null)
                            this.Alias = TypeConverter<T>.ConvertTo_string(value);
                        else
                            this.Alias += TypeConverter<T>.ConvertTo_string(value);
                    }
                    
                    break;
                
                case 12:
                    
                    {
                        if (this.Description == null)
                            this.Description = TypeConverter<T>.ConvertTo_string(value);
                        else
                            this.Description += TypeConverter<T>.ConvertTo_string(value);
                    }
                    
                    break;
                
                case 13:
                    
                    {
                        if (this.Segments == null)
                            this.Segments = TypeConverter<T>.ConvertTo_string(value);
                        else
                            this.Segments += TypeConverter<T>.ConvertTo_string(value);
                    }
                    
                    break;
                
                case 14:
                    
                    {
                        if (this.Categories == null)
                            this.Categories = TypeConverter<T>.ConvertTo_string(value);
                        else
                            this.Categories += TypeConverter<T>.ConvertTo_string(value);
                    }
                    
                    break;
                
                case 15:
                    
                    {
                        if (this.IntEmbeddedFilters == null)
                            this.IntEmbeddedFilters = TypeConverter<T>.ConvertTo_string(value);
                        else
                            this.IntEmbeddedFilters += TypeConverter<T>.ConvertTo_string(value);
                    }
                    
                    break;
                
                case 16:
                    
                    {
                        if (this.NumberOfWantToWatch == null)
                            this.NumberOfWantToWatch = TypeConverter<T>.ConvertTo_string(value);
                        else
                            this.NumberOfWantToWatch += TypeConverter<T>.ConvertTo_string(value);
                    }
                    
                    break;
                
                case 17:
                    
                    {
                        if (this.Rating == null)
                            this.Rating = TypeConverter<T>.ConvertTo_string(value);
                        else
                            this.Rating += TypeConverter<T>.ConvertTo_string(value);
                    }
                    
                    break;
                
                case 18:
                    
                    {
                        if (this.NumberOfShortReview == null)
                            this.NumberOfShortReview = TypeConverter<T>.ConvertTo_string(value);
                        else
                            this.NumberOfShortReview += TypeConverter<T>.ConvertTo_string(value);
                    }
                    
                    break;
                
                case 19:
                    
                    {
                        if (this.ReviewCount == null)
                            this.ReviewCount = TypeConverter<T>.ConvertTo_string(value);
                        else
                            this.ReviewCount += TypeConverter<T>.ConvertTo_string(value);
                    }
                    
                    break;
                
                case 20:
                    
                    {
                        if (this.NumberOfWatched == null)
                            this.NumberOfWatched = TypeConverter<T>.ConvertTo_string(value);
                        else
                            this.NumberOfWatched += TypeConverter<T>.ConvertTo_string(value);
                    }
                    
                    break;
                
                case 21:
                    
                    {
                        if (this.NumberOfReviewer == null)
                            this.NumberOfReviewer = TypeConverter<T>.ConvertTo_string(value);
                        else
                            this.NumberOfReviewer += TypeConverter<T>.ConvertTo_string(value);
                    }
                    
                    break;
                
                case 22:
                    
                    {
                        if (this.PublishDate == null)
                            this.PublishDate = TypeConverter<T>.ConvertTo_string(value);
                        else
                            this.PublishDate += TypeConverter<T>.ConvertTo_string(value);
                    }
                    
                    break;
                
                case 23:
                    
                    {
                        if (this.Length == null)
                            this.Length = TypeConverter<T>.ConvertTo_string(value);
                        else
                            this.Length += TypeConverter<T>.ConvertTo_string(value);
                    }
                    
                    break;
                
                case 24:
                    
                    {
                        if (this.Country == null)
                            this.Country = TypeConverter<T>.ConvertTo_string(value);
                        else
                            this.Country += TypeConverter<T>.ConvertTo_string(value);
                    }
                    
                    break;
                
                case 25:
                    
                    {
                        if (this.Language == null)
                            this.Language = TypeConverter<T>.ConvertTo_string(value);
                        else
                            this.Language += TypeConverter<T>.ConvertTo_string(value);
                    }
                    
                    break;
                
                case 26:
                    
                    {
                        if (this.SourceUrls == null)
                            this.SourceUrls = TypeConverter<T>.ConvertTo_string(value);
                        else
                            this.SourceUrls += TypeConverter<T>.ConvertTo_string(value);
                    }
                    
                    break;
                
                case 27:
                    
                    {
                        if (this.ImageUrls == null)
                            this.ImageUrls = TypeConverter<T>.ConvertTo_string(value);
                        else
                            this.ImageUrls += TypeConverter<T>.ConvertTo_string(value);
                    }
                    
                    break;
                
                case 28:
                    
                    {
                        if (this.OfficialSite == null)
                            this.OfficialSite = TypeConverter<T>.ConvertTo_string(value);
                        else
                            this.OfficialSite += TypeConverter<T>.ConvertTo_string(value);
                    }
                    
                    break;
                
                case 29:
                    
                    {
                        if (this.EntityContainer == null)
                            this.EntityContainer = TypeConverter<T>.ConvertTo_string(value);
                        else
                            this.EntityContainer += TypeConverter<T>.ConvertTo_string(value);
                    }
                    
                    break;
                
                case 30:
                    
                    {
                        if (this.Logo == null)
                            this.Logo = TypeConverter<T>.ConvertTo_string(value);
                        else
                            this.Logo += TypeConverter<T>.ConvertTo_string(value);
                    }
                    
                    break;
                
                case 31:
                    
                    {
                        if (this.QueryRank == null)
                            this.QueryRank = TypeConverter<T>.ConvertTo_string(value);
                        else
                            this.QueryRank += TypeConverter<T>.ConvertTo_string(value);
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
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Key, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Key", TypeConverter<T>.ConvertFrom_string(this.Key));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.KGId, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("KGId", TypeConverter<T>.ConvertFrom_string(this.KGId));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Characters, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Characters", TypeConverter<T>.ConvertFrom_string(this.Characters));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Distributors, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Distributors", TypeConverter<T>.ConvertFrom_string(this.Distributors));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Channels, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Channels", TypeConverter<T>.ConvertFrom_string(this.Channels));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Albums, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Albums", TypeConverter<T>.ConvertFrom_string(this.Albums));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Name, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Name", TypeConverter<T>.ConvertFrom_string(this.Name));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Alias, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Alias", TypeConverter<T>.ConvertFrom_string(this.Alias));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Description, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Description", TypeConverter<T>.ConvertFrom_string(this.Description));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Segments, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Segments", TypeConverter<T>.ConvertFrom_string(this.Segments));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Categories, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Categories", TypeConverter<T>.ConvertFrom_string(this.Categories));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.IntEmbeddedFilters, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("IntEmbeddedFilters", TypeConverter<T>.ConvertFrom_string(this.IntEmbeddedFilters));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.NumberOfWantToWatch, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("NumberOfWantToWatch", TypeConverter<T>.ConvertFrom_string(this.NumberOfWantToWatch));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Rating, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Rating", TypeConverter<T>.ConvertFrom_string(this.Rating));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.NumberOfShortReview, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("NumberOfShortReview", TypeConverter<T>.ConvertFrom_string(this.NumberOfShortReview));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.ReviewCount, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("ReviewCount", TypeConverter<T>.ConvertFrom_string(this.ReviewCount));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.NumberOfWatched, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("NumberOfWatched", TypeConverter<T>.ConvertFrom_string(this.NumberOfWatched));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.NumberOfReviewer, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("NumberOfReviewer", TypeConverter<T>.ConvertFrom_string(this.NumberOfReviewer));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.PublishDate, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("PublishDate", TypeConverter<T>.ConvertFrom_string(this.PublishDate));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Length, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Length", TypeConverter<T>.ConvertFrom_string(this.Length));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Country, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Country", TypeConverter<T>.ConvertFrom_string(this.Country));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Language, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Language", TypeConverter<T>.ConvertFrom_string(this.Language));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.SourceUrls, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("SourceUrls", TypeConverter<T>.ConvertFrom_string(this.SourceUrls));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.ImageUrls, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("ImageUrls", TypeConverter<T>.ConvertFrom_string(this.ImageUrls));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.OfficialSite, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("OfficialSite", TypeConverter<T>.ConvertFrom_string(this.OfficialSite));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.EntityContainer, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("EntityContainer", TypeConverter<T>.ConvertFrom_string(this.EntityContainer));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Logo, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Logo", TypeConverter<T>.ConvertFrom_string(this.Logo));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.QueryRank, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("QueryRank", TypeConverter<T>.ConvertFrom_string(this.QueryRank));
                    
                    break;
                
                case  1:
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Key, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Key", TypeConverter<T>.ConvertFrom_string(this.Key));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.KGId, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("KGId", TypeConverter<T>.ConvertFrom_string(this.KGId));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Characters, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Characters", TypeConverter<T>.ConvertFrom_string(this.Characters));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Distributors, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Distributors", TypeConverter<T>.ConvertFrom_string(this.Distributors));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Channels, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Channels", TypeConverter<T>.ConvertFrom_string(this.Channels));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Albums, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Albums", TypeConverter<T>.ConvertFrom_string(this.Albums));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Name, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Name", TypeConverter<T>.ConvertFrom_string(this.Name));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Alias, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Alias", TypeConverter<T>.ConvertFrom_string(this.Alias));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Description, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Description", TypeConverter<T>.ConvertFrom_string(this.Description));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Segments, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Segments", TypeConverter<T>.ConvertFrom_string(this.Segments));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Categories, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Categories", TypeConverter<T>.ConvertFrom_string(this.Categories));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.IntEmbeddedFilters, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("IntEmbeddedFilters", TypeConverter<T>.ConvertFrom_string(this.IntEmbeddedFilters));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.NumberOfWantToWatch, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("NumberOfWantToWatch", TypeConverter<T>.ConvertFrom_string(this.NumberOfWantToWatch));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Rating, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Rating", TypeConverter<T>.ConvertFrom_string(this.Rating));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.NumberOfShortReview, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("NumberOfShortReview", TypeConverter<T>.ConvertFrom_string(this.NumberOfShortReview));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.ReviewCount, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("ReviewCount", TypeConverter<T>.ConvertFrom_string(this.ReviewCount));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.NumberOfWatched, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("NumberOfWatched", TypeConverter<T>.ConvertFrom_string(this.NumberOfWatched));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.NumberOfReviewer, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("NumberOfReviewer", TypeConverter<T>.ConvertFrom_string(this.NumberOfReviewer));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.PublishDate, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("PublishDate", TypeConverter<T>.ConvertFrom_string(this.PublishDate));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Length, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Length", TypeConverter<T>.ConvertFrom_string(this.Length));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Country, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Country", TypeConverter<T>.ConvertFrom_string(this.Country));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Language, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Language", TypeConverter<T>.ConvertFrom_string(this.Language));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.SourceUrls, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("SourceUrls", TypeConverter<T>.ConvertFrom_string(this.SourceUrls));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.ImageUrls, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("ImageUrls", TypeConverter<T>.ConvertFrom_string(this.ImageUrls));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.OfficialSite, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("OfficialSite", TypeConverter<T>.ConvertFrom_string(this.OfficialSite));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.EntityContainer, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("EntityContainer", TypeConverter<T>.ConvertFrom_string(this.EntityContainer));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Logo, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Logo", TypeConverter<T>.ConvertFrom_string(this.Logo));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.QueryRank, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("QueryRank", TypeConverter<T>.ConvertFrom_string(this.QueryRank));
                    
                    break;
                
                case  2:
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Key, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Key", TypeConverter<T>.ConvertFrom_string(this.Key));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.KGId, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("KGId", TypeConverter<T>.ConvertFrom_string(this.KGId));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Characters, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Characters", TypeConverter<T>.ConvertFrom_string(this.Characters));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Distributors, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Distributors", TypeConverter<T>.ConvertFrom_string(this.Distributors));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Channels, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Channels", TypeConverter<T>.ConvertFrom_string(this.Channels));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Albums, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Albums", TypeConverter<T>.ConvertFrom_string(this.Albums));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Name, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Name", TypeConverter<T>.ConvertFrom_string(this.Name));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Alias, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Alias", TypeConverter<T>.ConvertFrom_string(this.Alias));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Description, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Description", TypeConverter<T>.ConvertFrom_string(this.Description));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Segments, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Segments", TypeConverter<T>.ConvertFrom_string(this.Segments));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Categories, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Categories", TypeConverter<T>.ConvertFrom_string(this.Categories));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.IntEmbeddedFilters, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("IntEmbeddedFilters", TypeConverter<T>.ConvertFrom_string(this.IntEmbeddedFilters));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.NumberOfWantToWatch, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("NumberOfWantToWatch", TypeConverter<T>.ConvertFrom_string(this.NumberOfWantToWatch));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Rating, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Rating", TypeConverter<T>.ConvertFrom_string(this.Rating));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.NumberOfShortReview, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("NumberOfShortReview", TypeConverter<T>.ConvertFrom_string(this.NumberOfShortReview));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.ReviewCount, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("ReviewCount", TypeConverter<T>.ConvertFrom_string(this.ReviewCount));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.NumberOfWatched, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("NumberOfWatched", TypeConverter<T>.ConvertFrom_string(this.NumberOfWatched));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.NumberOfReviewer, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("NumberOfReviewer", TypeConverter<T>.ConvertFrom_string(this.NumberOfReviewer));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.PublishDate, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("PublishDate", TypeConverter<T>.ConvertFrom_string(this.PublishDate));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Length, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Length", TypeConverter<T>.ConvertFrom_string(this.Length));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Country, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Country", TypeConverter<T>.ConvertFrom_string(this.Country));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Language, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Language", TypeConverter<T>.ConvertFrom_string(this.Language));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.SourceUrls, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("SourceUrls", TypeConverter<T>.ConvertFrom_string(this.SourceUrls));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.ImageUrls, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("ImageUrls", TypeConverter<T>.ConvertFrom_string(this.ImageUrls));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.OfficialSite, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("OfficialSite", TypeConverter<T>.ConvertFrom_string(this.OfficialSite));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.EntityContainer, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("EntityContainer", TypeConverter<T>.ConvertFrom_string(this.EntityContainer));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Logo, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Logo", TypeConverter<T>.ConvertFrom_string(this.Logo));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.QueryRank, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("QueryRank", TypeConverter<T>.ConvertFrom_string(this.QueryRank));
                    
                    break;
                
                case  3:
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Key, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Key", TypeConverter<T>.ConvertFrom_string(this.Key));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.KGId, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("KGId", TypeConverter<T>.ConvertFrom_string(this.KGId));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Characters, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Characters", TypeConverter<T>.ConvertFrom_string(this.Characters));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Distributors, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Distributors", TypeConverter<T>.ConvertFrom_string(this.Distributors));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Channels, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Channels", TypeConverter<T>.ConvertFrom_string(this.Channels));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Albums, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Albums", TypeConverter<T>.ConvertFrom_string(this.Albums));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Name, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Name", TypeConverter<T>.ConvertFrom_string(this.Name));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Alias, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Alias", TypeConverter<T>.ConvertFrom_string(this.Alias));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Description, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Description", TypeConverter<T>.ConvertFrom_string(this.Description));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Segments, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Segments", TypeConverter<T>.ConvertFrom_string(this.Segments));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Categories, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Categories", TypeConverter<T>.ConvertFrom_string(this.Categories));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.IntEmbeddedFilters, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("IntEmbeddedFilters", TypeConverter<T>.ConvertFrom_string(this.IntEmbeddedFilters));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.NumberOfWantToWatch, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("NumberOfWantToWatch", TypeConverter<T>.ConvertFrom_string(this.NumberOfWantToWatch));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Rating, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Rating", TypeConverter<T>.ConvertFrom_string(this.Rating));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.NumberOfShortReview, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("NumberOfShortReview", TypeConverter<T>.ConvertFrom_string(this.NumberOfShortReview));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.ReviewCount, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("ReviewCount", TypeConverter<T>.ConvertFrom_string(this.ReviewCount));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.NumberOfWatched, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("NumberOfWatched", TypeConverter<T>.ConvertFrom_string(this.NumberOfWatched));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.NumberOfReviewer, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("NumberOfReviewer", TypeConverter<T>.ConvertFrom_string(this.NumberOfReviewer));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.PublishDate, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("PublishDate", TypeConverter<T>.ConvertFrom_string(this.PublishDate));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Length, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Length", TypeConverter<T>.ConvertFrom_string(this.Length));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Country, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Country", TypeConverter<T>.ConvertFrom_string(this.Country));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Language, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Language", TypeConverter<T>.ConvertFrom_string(this.Language));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.SourceUrls, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("SourceUrls", TypeConverter<T>.ConvertFrom_string(this.SourceUrls));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.ImageUrls, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("ImageUrls", TypeConverter<T>.ConvertFrom_string(this.ImageUrls));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.OfficialSite, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("OfficialSite", TypeConverter<T>.ConvertFrom_string(this.OfficialSite));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.EntityContainer, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("EntityContainer", TypeConverter<T>.ConvertFrom_string(this.EntityContainer));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Logo, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Logo", TypeConverter<T>.ConvertFrom_string(this.Logo));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.QueryRank, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("QueryRank", TypeConverter<T>.ConvertFrom_string(this.QueryRank));
                    
                    break;
                
                case  4:
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Key, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Key", TypeConverter<T>.ConvertFrom_string(this.Key));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.KGId, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("KGId", TypeConverter<T>.ConvertFrom_string(this.KGId));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Genres, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Genres", TypeConverter<T>.ConvertFrom_List_string(this.Genres));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Artists, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Artists", TypeConverter<T>.ConvertFrom_List_long(this.Artists));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Directors, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Directors", TypeConverter<T>.ConvertFrom_List_long(this.Directors));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Characters, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Characters", TypeConverter<T>.ConvertFrom_string(this.Characters));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Performance, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Performance", TypeConverter<T>.ConvertFrom_List_string(this.Performance));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Distributors, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Distributors", TypeConverter<T>.ConvertFrom_string(this.Distributors));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Channels, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Channels", TypeConverter<T>.ConvertFrom_string(this.Channels));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Albums, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Albums", TypeConverter<T>.ConvertFrom_string(this.Albums));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Name, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Name", TypeConverter<T>.ConvertFrom_string(this.Name));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Alias, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Alias", TypeConverter<T>.ConvertFrom_string(this.Alias));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Description, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Description", TypeConverter<T>.ConvertFrom_string(this.Description));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Segments, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Segments", TypeConverter<T>.ConvertFrom_string(this.Segments));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Categories, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Categories", TypeConverter<T>.ConvertFrom_string(this.Categories));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.IntEmbeddedFilters, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("IntEmbeddedFilters", TypeConverter<T>.ConvertFrom_string(this.IntEmbeddedFilters));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.NumberOfWantToWatch, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("NumberOfWantToWatch", TypeConverter<T>.ConvertFrom_string(this.NumberOfWantToWatch));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Rating, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Rating", TypeConverter<T>.ConvertFrom_string(this.Rating));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.NumberOfShortReview, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("NumberOfShortReview", TypeConverter<T>.ConvertFrom_string(this.NumberOfShortReview));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.ReviewCount, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("ReviewCount", TypeConverter<T>.ConvertFrom_string(this.ReviewCount));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.NumberOfWatched, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("NumberOfWatched", TypeConverter<T>.ConvertFrom_string(this.NumberOfWatched));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.NumberOfReviewer, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("NumberOfReviewer", TypeConverter<T>.ConvertFrom_string(this.NumberOfReviewer));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.PublishDate, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("PublishDate", TypeConverter<T>.ConvertFrom_string(this.PublishDate));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Length, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Length", TypeConverter<T>.ConvertFrom_string(this.Length));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Country, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Country", TypeConverter<T>.ConvertFrom_string(this.Country));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Language, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Language", TypeConverter<T>.ConvertFrom_string(this.Language));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.SourceUrls, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("SourceUrls", TypeConverter<T>.ConvertFrom_string(this.SourceUrls));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.ImageUrls, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("ImageUrls", TypeConverter<T>.ConvertFrom_string(this.ImageUrls));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.OfficialSite, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("OfficialSite", TypeConverter<T>.ConvertFrom_string(this.OfficialSite));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.EntityContainer, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("EntityContainer", TypeConverter<T>.ConvertFrom_string(this.EntityContainer));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Logo, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Logo", TypeConverter<T>.ConvertFrom_string(this.Logo));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.QueryRank, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("QueryRank", TypeConverter<T>.ConvertFrom_string(this.QueryRank));
                    
                    break;
                
                case  5:
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Key, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Key", TypeConverter<T>.ConvertFrom_string(this.Key));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.KGId, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("KGId", TypeConverter<T>.ConvertFrom_string(this.KGId));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Genres, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Genres", TypeConverter<T>.ConvertFrom_List_string(this.Genres));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Artists, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Artists", TypeConverter<T>.ConvertFrom_List_long(this.Artists));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Directors, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Directors", TypeConverter<T>.ConvertFrom_List_long(this.Directors));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Characters, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Characters", TypeConverter<T>.ConvertFrom_string(this.Characters));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Performance, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Performance", TypeConverter<T>.ConvertFrom_List_string(this.Performance));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Distributors, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Distributors", TypeConverter<T>.ConvertFrom_string(this.Distributors));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Channels, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Channels", TypeConverter<T>.ConvertFrom_string(this.Channels));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Albums, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Albums", TypeConverter<T>.ConvertFrom_string(this.Albums));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Name, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Name", TypeConverter<T>.ConvertFrom_string(this.Name));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Alias, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Alias", TypeConverter<T>.ConvertFrom_string(this.Alias));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Description, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Description", TypeConverter<T>.ConvertFrom_string(this.Description));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Segments, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Segments", TypeConverter<T>.ConvertFrom_string(this.Segments));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Categories, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Categories", TypeConverter<T>.ConvertFrom_string(this.Categories));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.IntEmbeddedFilters, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("IntEmbeddedFilters", TypeConverter<T>.ConvertFrom_string(this.IntEmbeddedFilters));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.NumberOfWantToWatch, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("NumberOfWantToWatch", TypeConverter<T>.ConvertFrom_string(this.NumberOfWantToWatch));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Rating, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Rating", TypeConverter<T>.ConvertFrom_string(this.Rating));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.NumberOfShortReview, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("NumberOfShortReview", TypeConverter<T>.ConvertFrom_string(this.NumberOfShortReview));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.ReviewCount, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("ReviewCount", TypeConverter<T>.ConvertFrom_string(this.ReviewCount));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.NumberOfWatched, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("NumberOfWatched", TypeConverter<T>.ConvertFrom_string(this.NumberOfWatched));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.NumberOfReviewer, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("NumberOfReviewer", TypeConverter<T>.ConvertFrom_string(this.NumberOfReviewer));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.PublishDate, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("PublishDate", TypeConverter<T>.ConvertFrom_string(this.PublishDate));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Length, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Length", TypeConverter<T>.ConvertFrom_string(this.Length));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Country, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Country", TypeConverter<T>.ConvertFrom_string(this.Country));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Language, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Language", TypeConverter<T>.ConvertFrom_string(this.Language));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.SourceUrls, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("SourceUrls", TypeConverter<T>.ConvertFrom_string(this.SourceUrls));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.ImageUrls, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("ImageUrls", TypeConverter<T>.ConvertFrom_string(this.ImageUrls));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.OfficialSite, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("OfficialSite", TypeConverter<T>.ConvertFrom_string(this.OfficialSite));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.EntityContainer, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("EntityContainer", TypeConverter<T>.ConvertFrom_string(this.EntityContainer));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Logo, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Logo", TypeConverter<T>.ConvertFrom_string(this.Logo));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.QueryRank, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("QueryRank", TypeConverter<T>.ConvertFrom_string(this.QueryRank));
                    
                    break;
                
                case  6:
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Key, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Key", TypeConverter<T>.ConvertFrom_string(this.Key));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.KGId, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("KGId", TypeConverter<T>.ConvertFrom_string(this.KGId));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Genres, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Genres", TypeConverter<T>.ConvertFrom_List_string(this.Genres));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Artists, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Artists", TypeConverter<T>.ConvertFrom_List_long(this.Artists));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Directors, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Directors", TypeConverter<T>.ConvertFrom_List_long(this.Directors));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Characters, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Characters", TypeConverter<T>.ConvertFrom_string(this.Characters));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Performance, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Performance", TypeConverter<T>.ConvertFrom_List_string(this.Performance));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Distributors, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Distributors", TypeConverter<T>.ConvertFrom_string(this.Distributors));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Channels, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Channels", TypeConverter<T>.ConvertFrom_string(this.Channels));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Albums, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Albums", TypeConverter<T>.ConvertFrom_string(this.Albums));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Name, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Name", TypeConverter<T>.ConvertFrom_string(this.Name));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Alias, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Alias", TypeConverter<T>.ConvertFrom_string(this.Alias));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Description, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Description", TypeConverter<T>.ConvertFrom_string(this.Description));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Segments, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Segments", TypeConverter<T>.ConvertFrom_string(this.Segments));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Categories, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Categories", TypeConverter<T>.ConvertFrom_string(this.Categories));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.IntEmbeddedFilters, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("IntEmbeddedFilters", TypeConverter<T>.ConvertFrom_string(this.IntEmbeddedFilters));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.NumberOfWantToWatch, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("NumberOfWantToWatch", TypeConverter<T>.ConvertFrom_string(this.NumberOfWantToWatch));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Rating, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Rating", TypeConverter<T>.ConvertFrom_string(this.Rating));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.NumberOfShortReview, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("NumberOfShortReview", TypeConverter<T>.ConvertFrom_string(this.NumberOfShortReview));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.ReviewCount, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("ReviewCount", TypeConverter<T>.ConvertFrom_string(this.ReviewCount));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.NumberOfWatched, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("NumberOfWatched", TypeConverter<T>.ConvertFrom_string(this.NumberOfWatched));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.NumberOfReviewer, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("NumberOfReviewer", TypeConverter<T>.ConvertFrom_string(this.NumberOfReviewer));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.PublishDate, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("PublishDate", TypeConverter<T>.ConvertFrom_string(this.PublishDate));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Length, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Length", TypeConverter<T>.ConvertFrom_string(this.Length));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Country, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Country", TypeConverter<T>.ConvertFrom_string(this.Country));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Language, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Language", TypeConverter<T>.ConvertFrom_string(this.Language));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.SourceUrls, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("SourceUrls", TypeConverter<T>.ConvertFrom_string(this.SourceUrls));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.ImageUrls, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("ImageUrls", TypeConverter<T>.ConvertFrom_string(this.ImageUrls));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.OfficialSite, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("OfficialSite", TypeConverter<T>.ConvertFrom_string(this.OfficialSite));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.EntityContainer, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("EntityContainer", TypeConverter<T>.ConvertFrom_string(this.EntityContainer));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Logo, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Logo", TypeConverter<T>.ConvertFrom_string(this.Logo));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.QueryRank, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("QueryRank", TypeConverter<T>.ConvertFrom_string(this.QueryRank));
                    
                    break;
                
                default:
                    Throw.incompatible_with_cell();
                    break;
            }
            yield break;
        }
        #region enumerate value constructs
        
        private IEnumerable<T> _enumerate_from_Key<T>()
        {
            
                switch (TypeConverter<T>.type_id)
                {
                    
                    case  0:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Key);
                            
                        }
                        break;
                    
                    case  1:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Key);
                            
                        }
                        break;
                    
                    case  2:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Key);
                            
                        }
                        break;
                    
                    case  3:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Key);
                            
                        }
                        break;
                    
                    case  4:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Key);
                            
                        }
                        break;
                    
                    case  5:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Key);
                            
                        }
                        break;
                    
                    case  6:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Key);
                            
                        }
                        break;
                    
                    default:
                        Throw.incompatible_with_cell();
                        break;
                }
            yield break;
        }
        
        private IEnumerable<T> _enumerate_from_KGId<T>()
        {
            
                switch (TypeConverter<T>.type_id)
                {
                    
                    case  0:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.KGId);
                            
                        }
                        break;
                    
                    case  1:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.KGId);
                            
                        }
                        break;
                    
                    case  2:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.KGId);
                            
                        }
                        break;
                    
                    case  3:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.KGId);
                            
                        }
                        break;
                    
                    case  4:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.KGId);
                            
                        }
                        break;
                    
                    case  5:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.KGId);
                            
                        }
                        break;
                    
                    case  6:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.KGId);
                            
                        }
                        break;
                    
                    default:
                        Throw.incompatible_with_cell();
                        break;
                }
            yield break;
        }
        
        private IEnumerable<T> _enumerate_from_Genres<T>()
        {
            
                switch (TypeConverter<T>.type_id)
                {
                    
                    case  0:
                        {
                            
                            {
                                
                                var element0 = this.Genres;
                                
                                foreach (var element1 in element0)
                                
                                {
                                    yield return TypeConverter<T>.ConvertFrom_string(element1);
                                }
                            }
                            
                        }
                        break;
                    
                    case  1:
                        {
                            
                            {
                                
                                var element0 = this.Genres;
                                
                                foreach (var element1 in element0)
                                
                                {
                                    yield return TypeConverter<T>.ConvertFrom_string(element1);
                                }
                            }
                            
                        }
                        break;
                    
                    case  2:
                        {
                            
                            {
                                
                                var element0 = this.Genres;
                                
                                foreach (var element1 in element0)
                                
                                {
                                    yield return TypeConverter<T>.ConvertFrom_string(element1);
                                }
                            }
                            
                        }
                        break;
                    
                    case  3:
                        {
                            
                            {
                                
                                var element0 = this.Genres;
                                
                                foreach (var element1 in element0)
                                
                                {
                                    yield return TypeConverter<T>.ConvertFrom_string(element1);
                                }
                            }
                            
                        }
                        break;
                    
                    case  4:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_List_string(this.Genres);
                            
                        }
                        break;
                    
                    case  5:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_List_string(this.Genres);
                            
                        }
                        break;
                    
                    case  6:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_List_string(this.Genres);
                            
                        }
                        break;
                    
                    default:
                        Throw.incompatible_with_cell();
                        break;
                }
            yield break;
        }
        
        private IEnumerable<T> _enumerate_from_Artists<T>()
        {
            
                switch (TypeConverter<T>.type_id)
                {
                    
                    case  1:
                        {
                            
                            {
                                
                                var element0 = this.Artists;
                                
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
                                
                                var element0 = this.Artists;
                                
                                foreach (var element1 in element0)
                                
                                {
                                    yield return TypeConverter<T>.ConvertFrom_long(element1);
                                }
                            }
                            
                        }
                        break;
                    
                    case  4:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_List_long(this.Artists);
                            
                        }
                        break;
                    
                    case  5:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_List_long(this.Artists);
                            
                        }
                        break;
                    
                    case  6:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_List_long(this.Artists);
                            
                        }
                        break;
                    
                    default:
                        Throw.incompatible_with_cell();
                        break;
                }
            yield break;
        }
        
        private IEnumerable<T> _enumerate_from_Directors<T>()
        {
            
                switch (TypeConverter<T>.type_id)
                {
                    
                    case  1:
                        {
                            
                            {
                                
                                var element0 = this.Directors;
                                
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
                                
                                var element0 = this.Directors;
                                
                                foreach (var element1 in element0)
                                
                                {
                                    yield return TypeConverter<T>.ConvertFrom_long(element1);
                                }
                            }
                            
                        }
                        break;
                    
                    case  4:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_List_long(this.Directors);
                            
                        }
                        break;
                    
                    case  5:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_List_long(this.Directors);
                            
                        }
                        break;
                    
                    case  6:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_List_long(this.Directors);
                            
                        }
                        break;
                    
                    default:
                        Throw.incompatible_with_cell();
                        break;
                }
            yield break;
        }
        
        private IEnumerable<T> _enumerate_from_Characters<T>()
        {
            
                switch (TypeConverter<T>.type_id)
                {
                    
                    case  0:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Characters);
                            
                        }
                        break;
                    
                    case  1:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Characters);
                            
                        }
                        break;
                    
                    case  2:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Characters);
                            
                        }
                        break;
                    
                    case  3:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Characters);
                            
                        }
                        break;
                    
                    case  4:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Characters);
                            
                        }
                        break;
                    
                    case  5:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Characters);
                            
                        }
                        break;
                    
                    case  6:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Characters);
                            
                        }
                        break;
                    
                    default:
                        Throw.incompatible_with_cell();
                        break;
                }
            yield break;
        }
        
        private IEnumerable<T> _enumerate_from_Performance<T>()
        {
            
                switch (TypeConverter<T>.type_id)
                {
                    
                    case  0:
                        {
                            
                            {
                                
                                var element0 = this.Performance;
                                
                                foreach (var element1 in element0)
                                
                                {
                                    yield return TypeConverter<T>.ConvertFrom_string(element1);
                                }
                            }
                            
                        }
                        break;
                    
                    case  1:
                        {
                            
                            {
                                
                                var element0 = this.Performance;
                                
                                foreach (var element1 in element0)
                                
                                {
                                    yield return TypeConverter<T>.ConvertFrom_string(element1);
                                }
                            }
                            
                        }
                        break;
                    
                    case  2:
                        {
                            
                            {
                                
                                var element0 = this.Performance;
                                
                                foreach (var element1 in element0)
                                
                                {
                                    yield return TypeConverter<T>.ConvertFrom_string(element1);
                                }
                            }
                            
                        }
                        break;
                    
                    case  3:
                        {
                            
                            {
                                
                                var element0 = this.Performance;
                                
                                foreach (var element1 in element0)
                                
                                {
                                    yield return TypeConverter<T>.ConvertFrom_string(element1);
                                }
                            }
                            
                        }
                        break;
                    
                    case  4:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_List_string(this.Performance);
                            
                        }
                        break;
                    
                    case  5:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_List_string(this.Performance);
                            
                        }
                        break;
                    
                    case  6:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_List_string(this.Performance);
                            
                        }
                        break;
                    
                    default:
                        Throw.incompatible_with_cell();
                        break;
                }
            yield break;
        }
        
        private IEnumerable<T> _enumerate_from_Distributors<T>()
        {
            
                switch (TypeConverter<T>.type_id)
                {
                    
                    case  0:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Distributors);
                            
                        }
                        break;
                    
                    case  1:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Distributors);
                            
                        }
                        break;
                    
                    case  2:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Distributors);
                            
                        }
                        break;
                    
                    case  3:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Distributors);
                            
                        }
                        break;
                    
                    case  4:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Distributors);
                            
                        }
                        break;
                    
                    case  5:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Distributors);
                            
                        }
                        break;
                    
                    case  6:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Distributors);
                            
                        }
                        break;
                    
                    default:
                        Throw.incompatible_with_cell();
                        break;
                }
            yield break;
        }
        
        private IEnumerable<T> _enumerate_from_Channels<T>()
        {
            
                switch (TypeConverter<T>.type_id)
                {
                    
                    case  0:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Channels);
                            
                        }
                        break;
                    
                    case  1:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Channels);
                            
                        }
                        break;
                    
                    case  2:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Channels);
                            
                        }
                        break;
                    
                    case  3:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Channels);
                            
                        }
                        break;
                    
                    case  4:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Channels);
                            
                        }
                        break;
                    
                    case  5:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Channels);
                            
                        }
                        break;
                    
                    case  6:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Channels);
                            
                        }
                        break;
                    
                    default:
                        Throw.incompatible_with_cell();
                        break;
                }
            yield break;
        }
        
        private IEnumerable<T> _enumerate_from_Albums<T>()
        {
            
                switch (TypeConverter<T>.type_id)
                {
                    
                    case  0:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Albums);
                            
                        }
                        break;
                    
                    case  1:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Albums);
                            
                        }
                        break;
                    
                    case  2:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Albums);
                            
                        }
                        break;
                    
                    case  3:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Albums);
                            
                        }
                        break;
                    
                    case  4:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Albums);
                            
                        }
                        break;
                    
                    case  5:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Albums);
                            
                        }
                        break;
                    
                    case  6:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Albums);
                            
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
        
        private IEnumerable<T> _enumerate_from_Alias<T>()
        {
            
                switch (TypeConverter<T>.type_id)
                {
                    
                    case  0:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Alias);
                            
                        }
                        break;
                    
                    case  1:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Alias);
                            
                        }
                        break;
                    
                    case  2:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Alias);
                            
                        }
                        break;
                    
                    case  3:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Alias);
                            
                        }
                        break;
                    
                    case  4:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Alias);
                            
                        }
                        break;
                    
                    case  5:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Alias);
                            
                        }
                        break;
                    
                    case  6:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Alias);
                            
                        }
                        break;
                    
                    default:
                        Throw.incompatible_with_cell();
                        break;
                }
            yield break;
        }
        
        private IEnumerable<T> _enumerate_from_Description<T>()
        {
            
                switch (TypeConverter<T>.type_id)
                {
                    
                    case  0:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Description);
                            
                        }
                        break;
                    
                    case  1:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Description);
                            
                        }
                        break;
                    
                    case  2:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Description);
                            
                        }
                        break;
                    
                    case  3:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Description);
                            
                        }
                        break;
                    
                    case  4:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Description);
                            
                        }
                        break;
                    
                    case  5:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Description);
                            
                        }
                        break;
                    
                    case  6:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Description);
                            
                        }
                        break;
                    
                    default:
                        Throw.incompatible_with_cell();
                        break;
                }
            yield break;
        }
        
        private IEnumerable<T> _enumerate_from_Segments<T>()
        {
            
                switch (TypeConverter<T>.type_id)
                {
                    
                    case  0:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Segments);
                            
                        }
                        break;
                    
                    case  1:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Segments);
                            
                        }
                        break;
                    
                    case  2:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Segments);
                            
                        }
                        break;
                    
                    case  3:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Segments);
                            
                        }
                        break;
                    
                    case  4:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Segments);
                            
                        }
                        break;
                    
                    case  5:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Segments);
                            
                        }
                        break;
                    
                    case  6:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Segments);
                            
                        }
                        break;
                    
                    default:
                        Throw.incompatible_with_cell();
                        break;
                }
            yield break;
        }
        
        private IEnumerable<T> _enumerate_from_Categories<T>()
        {
            
                switch (TypeConverter<T>.type_id)
                {
                    
                    case  0:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Categories);
                            
                        }
                        break;
                    
                    case  1:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Categories);
                            
                        }
                        break;
                    
                    case  2:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Categories);
                            
                        }
                        break;
                    
                    case  3:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Categories);
                            
                        }
                        break;
                    
                    case  4:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Categories);
                            
                        }
                        break;
                    
                    case  5:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Categories);
                            
                        }
                        break;
                    
                    case  6:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Categories);
                            
                        }
                        break;
                    
                    default:
                        Throw.incompatible_with_cell();
                        break;
                }
            yield break;
        }
        
        private IEnumerable<T> _enumerate_from_IntEmbeddedFilters<T>()
        {
            
                switch (TypeConverter<T>.type_id)
                {
                    
                    case  0:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.IntEmbeddedFilters);
                            
                        }
                        break;
                    
                    case  1:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.IntEmbeddedFilters);
                            
                        }
                        break;
                    
                    case  2:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.IntEmbeddedFilters);
                            
                        }
                        break;
                    
                    case  3:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.IntEmbeddedFilters);
                            
                        }
                        break;
                    
                    case  4:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.IntEmbeddedFilters);
                            
                        }
                        break;
                    
                    case  5:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.IntEmbeddedFilters);
                            
                        }
                        break;
                    
                    case  6:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.IntEmbeddedFilters);
                            
                        }
                        break;
                    
                    default:
                        Throw.incompatible_with_cell();
                        break;
                }
            yield break;
        }
        
        private IEnumerable<T> _enumerate_from_NumberOfWantToWatch<T>()
        {
            
                switch (TypeConverter<T>.type_id)
                {
                    
                    case  0:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.NumberOfWantToWatch);
                            
                        }
                        break;
                    
                    case  1:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.NumberOfWantToWatch);
                            
                        }
                        break;
                    
                    case  2:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.NumberOfWantToWatch);
                            
                        }
                        break;
                    
                    case  3:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.NumberOfWantToWatch);
                            
                        }
                        break;
                    
                    case  4:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.NumberOfWantToWatch);
                            
                        }
                        break;
                    
                    case  5:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.NumberOfWantToWatch);
                            
                        }
                        break;
                    
                    case  6:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.NumberOfWantToWatch);
                            
                        }
                        break;
                    
                    default:
                        Throw.incompatible_with_cell();
                        break;
                }
            yield break;
        }
        
        private IEnumerable<T> _enumerate_from_Rating<T>()
        {
            
                switch (TypeConverter<T>.type_id)
                {
                    
                    case  0:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Rating);
                            
                        }
                        break;
                    
                    case  1:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Rating);
                            
                        }
                        break;
                    
                    case  2:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Rating);
                            
                        }
                        break;
                    
                    case  3:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Rating);
                            
                        }
                        break;
                    
                    case  4:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Rating);
                            
                        }
                        break;
                    
                    case  5:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Rating);
                            
                        }
                        break;
                    
                    case  6:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Rating);
                            
                        }
                        break;
                    
                    default:
                        Throw.incompatible_with_cell();
                        break;
                }
            yield break;
        }
        
        private IEnumerable<T> _enumerate_from_NumberOfShortReview<T>()
        {
            
                switch (TypeConverter<T>.type_id)
                {
                    
                    case  0:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.NumberOfShortReview);
                            
                        }
                        break;
                    
                    case  1:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.NumberOfShortReview);
                            
                        }
                        break;
                    
                    case  2:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.NumberOfShortReview);
                            
                        }
                        break;
                    
                    case  3:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.NumberOfShortReview);
                            
                        }
                        break;
                    
                    case  4:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.NumberOfShortReview);
                            
                        }
                        break;
                    
                    case  5:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.NumberOfShortReview);
                            
                        }
                        break;
                    
                    case  6:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.NumberOfShortReview);
                            
                        }
                        break;
                    
                    default:
                        Throw.incompatible_with_cell();
                        break;
                }
            yield break;
        }
        
        private IEnumerable<T> _enumerate_from_ReviewCount<T>()
        {
            
                switch (TypeConverter<T>.type_id)
                {
                    
                    case  0:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.ReviewCount);
                            
                        }
                        break;
                    
                    case  1:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.ReviewCount);
                            
                        }
                        break;
                    
                    case  2:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.ReviewCount);
                            
                        }
                        break;
                    
                    case  3:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.ReviewCount);
                            
                        }
                        break;
                    
                    case  4:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.ReviewCount);
                            
                        }
                        break;
                    
                    case  5:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.ReviewCount);
                            
                        }
                        break;
                    
                    case  6:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.ReviewCount);
                            
                        }
                        break;
                    
                    default:
                        Throw.incompatible_with_cell();
                        break;
                }
            yield break;
        }
        
        private IEnumerable<T> _enumerate_from_NumberOfWatched<T>()
        {
            
                switch (TypeConverter<T>.type_id)
                {
                    
                    case  0:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.NumberOfWatched);
                            
                        }
                        break;
                    
                    case  1:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.NumberOfWatched);
                            
                        }
                        break;
                    
                    case  2:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.NumberOfWatched);
                            
                        }
                        break;
                    
                    case  3:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.NumberOfWatched);
                            
                        }
                        break;
                    
                    case  4:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.NumberOfWatched);
                            
                        }
                        break;
                    
                    case  5:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.NumberOfWatched);
                            
                        }
                        break;
                    
                    case  6:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.NumberOfWatched);
                            
                        }
                        break;
                    
                    default:
                        Throw.incompatible_with_cell();
                        break;
                }
            yield break;
        }
        
        private IEnumerable<T> _enumerate_from_NumberOfReviewer<T>()
        {
            
                switch (TypeConverter<T>.type_id)
                {
                    
                    case  0:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.NumberOfReviewer);
                            
                        }
                        break;
                    
                    case  1:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.NumberOfReviewer);
                            
                        }
                        break;
                    
                    case  2:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.NumberOfReviewer);
                            
                        }
                        break;
                    
                    case  3:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.NumberOfReviewer);
                            
                        }
                        break;
                    
                    case  4:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.NumberOfReviewer);
                            
                        }
                        break;
                    
                    case  5:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.NumberOfReviewer);
                            
                        }
                        break;
                    
                    case  6:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.NumberOfReviewer);
                            
                        }
                        break;
                    
                    default:
                        Throw.incompatible_with_cell();
                        break;
                }
            yield break;
        }
        
        private IEnumerable<T> _enumerate_from_PublishDate<T>()
        {
            
                switch (TypeConverter<T>.type_id)
                {
                    
                    case  0:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.PublishDate);
                            
                        }
                        break;
                    
                    case  1:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.PublishDate);
                            
                        }
                        break;
                    
                    case  2:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.PublishDate);
                            
                        }
                        break;
                    
                    case  3:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.PublishDate);
                            
                        }
                        break;
                    
                    case  4:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.PublishDate);
                            
                        }
                        break;
                    
                    case  5:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.PublishDate);
                            
                        }
                        break;
                    
                    case  6:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.PublishDate);
                            
                        }
                        break;
                    
                    default:
                        Throw.incompatible_with_cell();
                        break;
                }
            yield break;
        }
        
        private IEnumerable<T> _enumerate_from_Length<T>()
        {
            
                switch (TypeConverter<T>.type_id)
                {
                    
                    case  0:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Length);
                            
                        }
                        break;
                    
                    case  1:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Length);
                            
                        }
                        break;
                    
                    case  2:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Length);
                            
                        }
                        break;
                    
                    case  3:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Length);
                            
                        }
                        break;
                    
                    case  4:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Length);
                            
                        }
                        break;
                    
                    case  5:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Length);
                            
                        }
                        break;
                    
                    case  6:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Length);
                            
                        }
                        break;
                    
                    default:
                        Throw.incompatible_with_cell();
                        break;
                }
            yield break;
        }
        
        private IEnumerable<T> _enumerate_from_Country<T>()
        {
            
                switch (TypeConverter<T>.type_id)
                {
                    
                    case  0:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Country);
                            
                        }
                        break;
                    
                    case  1:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Country);
                            
                        }
                        break;
                    
                    case  2:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Country);
                            
                        }
                        break;
                    
                    case  3:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Country);
                            
                        }
                        break;
                    
                    case  4:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Country);
                            
                        }
                        break;
                    
                    case  5:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Country);
                            
                        }
                        break;
                    
                    case  6:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Country);
                            
                        }
                        break;
                    
                    default:
                        Throw.incompatible_with_cell();
                        break;
                }
            yield break;
        }
        
        private IEnumerable<T> _enumerate_from_Language<T>()
        {
            
                switch (TypeConverter<T>.type_id)
                {
                    
                    case  0:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Language);
                            
                        }
                        break;
                    
                    case  1:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Language);
                            
                        }
                        break;
                    
                    case  2:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Language);
                            
                        }
                        break;
                    
                    case  3:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Language);
                            
                        }
                        break;
                    
                    case  4:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Language);
                            
                        }
                        break;
                    
                    case  5:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Language);
                            
                        }
                        break;
                    
                    case  6:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Language);
                            
                        }
                        break;
                    
                    default:
                        Throw.incompatible_with_cell();
                        break;
                }
            yield break;
        }
        
        private IEnumerable<T> _enumerate_from_SourceUrls<T>()
        {
            
                switch (TypeConverter<T>.type_id)
                {
                    
                    case  0:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.SourceUrls);
                            
                        }
                        break;
                    
                    case  1:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.SourceUrls);
                            
                        }
                        break;
                    
                    case  2:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.SourceUrls);
                            
                        }
                        break;
                    
                    case  3:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.SourceUrls);
                            
                        }
                        break;
                    
                    case  4:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.SourceUrls);
                            
                        }
                        break;
                    
                    case  5:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.SourceUrls);
                            
                        }
                        break;
                    
                    case  6:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.SourceUrls);
                            
                        }
                        break;
                    
                    default:
                        Throw.incompatible_with_cell();
                        break;
                }
            yield break;
        }
        
        private IEnumerable<T> _enumerate_from_ImageUrls<T>()
        {
            
                switch (TypeConverter<T>.type_id)
                {
                    
                    case  0:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.ImageUrls);
                            
                        }
                        break;
                    
                    case  1:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.ImageUrls);
                            
                        }
                        break;
                    
                    case  2:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.ImageUrls);
                            
                        }
                        break;
                    
                    case  3:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.ImageUrls);
                            
                        }
                        break;
                    
                    case  4:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.ImageUrls);
                            
                        }
                        break;
                    
                    case  5:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.ImageUrls);
                            
                        }
                        break;
                    
                    case  6:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.ImageUrls);
                            
                        }
                        break;
                    
                    default:
                        Throw.incompatible_with_cell();
                        break;
                }
            yield break;
        }
        
        private IEnumerable<T> _enumerate_from_OfficialSite<T>()
        {
            
                switch (TypeConverter<T>.type_id)
                {
                    
                    case  0:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.OfficialSite);
                            
                        }
                        break;
                    
                    case  1:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.OfficialSite);
                            
                        }
                        break;
                    
                    case  2:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.OfficialSite);
                            
                        }
                        break;
                    
                    case  3:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.OfficialSite);
                            
                        }
                        break;
                    
                    case  4:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.OfficialSite);
                            
                        }
                        break;
                    
                    case  5:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.OfficialSite);
                            
                        }
                        break;
                    
                    case  6:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.OfficialSite);
                            
                        }
                        break;
                    
                    default:
                        Throw.incompatible_with_cell();
                        break;
                }
            yield break;
        }
        
        private IEnumerable<T> _enumerate_from_EntityContainer<T>()
        {
            
                switch (TypeConverter<T>.type_id)
                {
                    
                    case  0:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.EntityContainer);
                            
                        }
                        break;
                    
                    case  1:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.EntityContainer);
                            
                        }
                        break;
                    
                    case  2:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.EntityContainer);
                            
                        }
                        break;
                    
                    case  3:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.EntityContainer);
                            
                        }
                        break;
                    
                    case  4:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.EntityContainer);
                            
                        }
                        break;
                    
                    case  5:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.EntityContainer);
                            
                        }
                        break;
                    
                    case  6:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.EntityContainer);
                            
                        }
                        break;
                    
                    default:
                        Throw.incompatible_with_cell();
                        break;
                }
            yield break;
        }
        
        private IEnumerable<T> _enumerate_from_Logo<T>()
        {
            
                switch (TypeConverter<T>.type_id)
                {
                    
                    case  0:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Logo);
                            
                        }
                        break;
                    
                    case  1:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Logo);
                            
                        }
                        break;
                    
                    case  2:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Logo);
                            
                        }
                        break;
                    
                    case  3:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Logo);
                            
                        }
                        break;
                    
                    case  4:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Logo);
                            
                        }
                        break;
                    
                    case  5:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Logo);
                            
                        }
                        break;
                    
                    case  6:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Logo);
                            
                        }
                        break;
                    
                    default:
                        Throw.incompatible_with_cell();
                        break;
                }
            yield break;
        }
        
        private IEnumerable<T> _enumerate_from_QueryRank<T>()
        {
            
                switch (TypeConverter<T>.type_id)
                {
                    
                    case  0:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.QueryRank);
                            
                        }
                        break;
                    
                    case  1:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.QueryRank);
                            
                        }
                        break;
                    
                    case  2:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.QueryRank);
                            
                        }
                        break;
                    
                    case  3:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.QueryRank);
                            
                        }
                        break;
                    
                    case  4:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.QueryRank);
                            
                        }
                        break;
                    
                    case  5:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.QueryRank);
                            
                        }
                        break;
                    
                    case  6:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.QueryRank);
                            
                        }
                        break;
                    
                    default:
                        Throw.incompatible_with_cell();
                        break;
                }
            yield break;
        }
        
        private static StringLookupTable s_field_attribute_id_table = new StringLookupTable(
            
            "Index"
            ,
            "GraphEdge"
            
            );
        #endregion
        public IEnumerable<T> EnumerateField<T>(string fieldName)
        {
            switch (FieldLookupTable.Lookup(fieldName))
            {
                
                case 0:
                    return _enumerate_from_Key<T>();
                
                case 1:
                    return _enumerate_from_KGId<T>();
                
                case 2:
                    return _enumerate_from_Genres<T>();
                
                case 3:
                    return _enumerate_from_Artists<T>();
                
                case 4:
                    return _enumerate_from_Directors<T>();
                
                case 5:
                    return _enumerate_from_Characters<T>();
                
                case 6:
                    return _enumerate_from_Performance<T>();
                
                case 7:
                    return _enumerate_from_Distributors<T>();
                
                case 8:
                    return _enumerate_from_Channels<T>();
                
                case 9:
                    return _enumerate_from_Albums<T>();
                
                case 10:
                    return _enumerate_from_Name<T>();
                
                case 11:
                    return _enumerate_from_Alias<T>();
                
                case 12:
                    return _enumerate_from_Description<T>();
                
                case 13:
                    return _enumerate_from_Segments<T>();
                
                case 14:
                    return _enumerate_from_Categories<T>();
                
                case 15:
                    return _enumerate_from_IntEmbeddedFilters<T>();
                
                case 16:
                    return _enumerate_from_NumberOfWantToWatch<T>();
                
                case 17:
                    return _enumerate_from_Rating<T>();
                
                case 18:
                    return _enumerate_from_NumberOfShortReview<T>();
                
                case 19:
                    return _enumerate_from_ReviewCount<T>();
                
                case 20:
                    return _enumerate_from_NumberOfWatched<T>();
                
                case 21:
                    return _enumerate_from_NumberOfReviewer<T>();
                
                case 22:
                    return _enumerate_from_PublishDate<T>();
                
                case 23:
                    return _enumerate_from_Length<T>();
                
                case 24:
                    return _enumerate_from_Country<T>();
                
                case 25:
                    return _enumerate_from_Language<T>();
                
                case 26:
                    return _enumerate_from_SourceUrls<T>();
                
                case 27:
                    return _enumerate_from_ImageUrls<T>();
                
                case 28:
                    return _enumerate_from_OfficialSite<T>();
                
                case 29:
                    return _enumerate_from_EntityContainer<T>();
                
                case 30:
                    return _enumerate_from_Logo<T>();
                
                case 31:
                    return _enumerate_from_QueryRank<T>();
                
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
                
                foreach (var val in _enumerate_from_Key<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_KGId<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_Genres<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_Artists<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_Directors<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_Characters<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_Performance<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_Distributors<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_Channels<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_Albums<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_Name<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_Alias<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_Description<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_Segments<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_Categories<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_IntEmbeddedFilters<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_NumberOfWantToWatch<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_Rating<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_NumberOfShortReview<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_ReviewCount<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_NumberOfWatched<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_NumberOfReviewer<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_PublishDate<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_Length<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_Country<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_Language<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_SourceUrls<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_ImageUrls<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_OfficialSite<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_EntityContainer<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_Logo<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_QueryRank<T>())
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
                            
                        }
                        
                        {
                            
                            {
                                if (attributeValue == null || attributeValue == "")
                                {
                                    foreach (var val in _enumerate_from_Genres<T>())
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
                        
                        {
                            
                        }
                        
                        break;
                    
                    case  1:
                        
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
                                    foreach (var val in _enumerate_from_Artists<T>())
                                        yield return val;
                                }
                            }
                            
                        }
                        
                        {
                            
                            {
                                if (attributeValue == null || attributeValue == "")
                                {
                                    foreach (var val in _enumerate_from_Directors<T>())
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
            get { return StorageSchema.s_cellTypeName_Movie; }
        }
        Type ITypeDescriptor.Type
        {
            get { return StorageSchema.s_cellType_Movie; }
        }
        bool ITypeDescriptor.IsOfType<T>()
        {
            return typeof(T) == StorageSchema.s_cellType_Movie;
        }
        bool ITypeDescriptor.IsList()
        {
            return false;
        }
        IEnumerable<IFieldDescriptor> ICellDescriptor.GetFieldDescriptors()
        {
            return StorageSchema.Movie.GetFieldDescriptors();
        }
        IAttributeCollection ICellDescriptor.GetFieldAttributes(string fieldName)
        {
            return StorageSchema.Movie.GetFieldAttributes(fieldName);
        }
        string IAttributeCollection.GetAttributeValue(string attributeKey)
        {
            return StorageSchema.Movie.GetAttributeValue(attributeKey);
        }
        IReadOnlyDictionary<string, string> IAttributeCollection.Attributes
        {
            get { return StorageSchema.Movie.Attributes; }
        }
        IEnumerable<string> ICellDescriptor.GetFieldNames()
        {
            
            {
                yield return "Key";
            }
            
            {
                yield return "KGId";
            }
            
            {
                yield return "Genres";
            }
            
            {
                yield return "Artists";
            }
            
            {
                yield return "Directors";
            }
            
            {
                yield return "Characters";
            }
            
            {
                yield return "Performance";
            }
            
            {
                yield return "Distributors";
            }
            
            {
                yield return "Channels";
            }
            
            {
                yield return "Albums";
            }
            
            {
                yield return "Name";
            }
            
            {
                yield return "Alias";
            }
            
            {
                yield return "Description";
            }
            
            {
                yield return "Segments";
            }
            
            {
                yield return "Categories";
            }
            
            {
                yield return "IntEmbeddedFilters";
            }
            
            {
                yield return "NumberOfWantToWatch";
            }
            
            {
                yield return "Rating";
            }
            
            {
                yield return "NumberOfShortReview";
            }
            
            {
                yield return "ReviewCount";
            }
            
            {
                yield return "NumberOfWatched";
            }
            
            {
                yield return "NumberOfReviewer";
            }
            
            {
                yield return "PublishDate";
            }
            
            {
                yield return "Length";
            }
            
            {
                yield return "Country";
            }
            
            {
                yield return "Language";
            }
            
            {
                yield return "SourceUrls";
            }
            
            {
                yield return "ImageUrls";
            }
            
            {
                yield return "OfficialSite";
            }
            
            {
                yield return "EntityContainer";
            }
            
            {
                yield return "Logo";
            }
            
            {
                yield return "QueryRank";
            }
            
        }
        ushort ICellDescriptor.CellType
        {
            get
            {
                return (ushort)CellType.Movie;
            }
        }
        #endregion
    }
    /// <summary>
    /// Provides in-place operations of Movie defined in TSL.
    /// </summary>
    public unsafe partial class Movie_Accessor : ICellAccessor
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
                            eResult                   = Global.LocalStorage.AddOrUse(cellId, defaultContent, ref size, (ushort)CellType.Movie, out cellPtr, out cellEntryIndex);
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
                        if (cellType != (ushort)CellType.Movie)
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
        internal static Movie_Accessor s_accessor = null;
        internal static Movie_Accessor New(long CellID, CellAccessOptions options)
        {
            Movie_Accessor ret = null;
            if (s_accessor != (Movie_Accessor)null)
            {
                ret = s_accessor;
                ret.Initialize(CellID, options);
                s_accessor = null;
            }
            else
            {
                ret = new Movie_Accessor(CellID, options);
            }
            if (ret.CellPtr == null)
            {
                s_accessor = ret;
                ret        = null;
            }
            return ret;
        }
        internal static Movie_Accessor New(long CellId, byte* cellPtr, int entryIndex, CellAccessOptions options)
        {
            Movie_Accessor ret = null;
            if (s_accessor != (Movie_Accessor)null)
            {
                ret = s_accessor;
                s_accessor = null;
                ret.CellPtr = cellPtr;
            }
            else
            {
                ret = new Movie_Accessor(cellPtr);
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
        internal static Movie_Accessor AllocIterativeAccessor(CellInfo info)
        {
            Movie_Accessor ret = null;
            if (s_accessor != (Movie_Accessor)null)
            {
                ret                = s_accessor;
                ret.CellPtr        = info.CellPtr;
                s_accessor         = null;
            }
            else
            {
                ret                = new Movie_Accessor(info.CellPtr);
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
        /// If write-ahead-log behavior is specified on <see cref="GraphEngineServer.StorageExtension_Movie.UseMovie"/>,
        /// the changes will be committed to the write-ahead log.
        /// </summary>
        public void Dispose()
        {
            if (CellEntryIndex >= 0)
            {
                if ((m_options & c_WALFlags) != 0)
                {
                    LocalMemoryStorage.CWriteAheadLog(this.CellID.Value, this.CellPtr, this.CellSize, (ushort)CellType.Movie, m_options);
                }
                if (!m_IsIterator)
                {
                    Global.LocalStorage.ReleaseCellLock(CellID.Value, CellEntryIndex);
                }
                if (s_accessor == (Movie_Accessor)null)
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
        /// <summary>Converts a Movie_Accessor to its string representation, in JSON format.</summary>
        /// <returns>A string representation of the Movie.</returns>
        public override string ToString()
        {
            return Serializer.ToString(this);
        }
        #endregion
        internal static StringLookupTable FieldLookupTable = new StringLookupTable(
            
            "Key"
            ,
            "KGId"
            ,
            "Genres"
            ,
            "Artists"
            ,
            "Directors"
            ,
            "Characters"
            ,
            "Performance"
            ,
            "Distributors"
            ,
            "Channels"
            ,
            "Albums"
            ,
            "Name"
            ,
            "Alias"
            ,
            "Description"
            ,
            "Segments"
            ,
            "Categories"
            ,
            "IntEmbeddedFilters"
            ,
            "NumberOfWantToWatch"
            ,
            "Rating"
            ,
            "NumberOfShortReview"
            ,
            "ReviewCount"
            ,
            "NumberOfWatched"
            ,
            "NumberOfReviewer"
            ,
            "PublishDate"
            ,
            "Length"
            ,
            "Country"
            ,
            "Language"
            ,
            "SourceUrls"
            ,
            "ImageUrls"
            ,
            "OfficialSite"
            ,
            "EntityContainer"
            ,
            "Logo"
            ,
            "QueryRank"
            
            );
        static HashSet<string> AppendToFieldRerouteSet = new HashSet<string>()
        {
            
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
                    return TypeConverter<T>.ConvertFrom_string(this.Key);
                
                case 1:
                    return TypeConverter<T>.ConvertFrom_string(this.KGId);
                
                case 2:
                    return TypeConverter<T>.ConvertFrom_List_string(this.Genres);
                
                case 3:
                    return TypeConverter<T>.ConvertFrom_List_long(this.Artists);
                
                case 4:
                    return TypeConverter<T>.ConvertFrom_List_long(this.Directors);
                
                case 5:
                    return TypeConverter<T>.ConvertFrom_string(this.Characters);
                
                case 6:
                    return TypeConverter<T>.ConvertFrom_List_string(this.Performance);
                
                case 7:
                    return TypeConverter<T>.ConvertFrom_string(this.Distributors);
                
                case 8:
                    return TypeConverter<T>.ConvertFrom_string(this.Channels);
                
                case 9:
                    return TypeConverter<T>.ConvertFrom_string(this.Albums);
                
                case 10:
                    return TypeConverter<T>.ConvertFrom_string(this.Name);
                
                case 11:
                    return TypeConverter<T>.ConvertFrom_string(this.Alias);
                
                case 12:
                    return TypeConverter<T>.ConvertFrom_string(this.Description);
                
                case 13:
                    return TypeConverter<T>.ConvertFrom_string(this.Segments);
                
                case 14:
                    return TypeConverter<T>.ConvertFrom_string(this.Categories);
                
                case 15:
                    return TypeConverter<T>.ConvertFrom_string(this.IntEmbeddedFilters);
                
                case 16:
                    return TypeConverter<T>.ConvertFrom_string(this.NumberOfWantToWatch);
                
                case 17:
                    return TypeConverter<T>.ConvertFrom_string(this.Rating);
                
                case 18:
                    return TypeConverter<T>.ConvertFrom_string(this.NumberOfShortReview);
                
                case 19:
                    return TypeConverter<T>.ConvertFrom_string(this.ReviewCount);
                
                case 20:
                    return TypeConverter<T>.ConvertFrom_string(this.NumberOfWatched);
                
                case 21:
                    return TypeConverter<T>.ConvertFrom_string(this.NumberOfReviewer);
                
                case 22:
                    return TypeConverter<T>.ConvertFrom_string(this.PublishDate);
                
                case 23:
                    return TypeConverter<T>.ConvertFrom_string(this.Length);
                
                case 24:
                    return TypeConverter<T>.ConvertFrom_string(this.Country);
                
                case 25:
                    return TypeConverter<T>.ConvertFrom_string(this.Language);
                
                case 26:
                    return TypeConverter<T>.ConvertFrom_string(this.SourceUrls);
                
                case 27:
                    return TypeConverter<T>.ConvertFrom_string(this.ImageUrls);
                
                case 28:
                    return TypeConverter<T>.ConvertFrom_string(this.OfficialSite);
                
                case 29:
                    return TypeConverter<T>.ConvertFrom_string(this.EntityContainer);
                
                case 30:
                    return TypeConverter<T>.ConvertFrom_string(this.Logo);
                
                case 31:
                    return TypeConverter<T>.ConvertFrom_string(this.QueryRank);
                
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
                        string conversion_result = TypeConverter<T>.ConvertTo_string(value);
                        
            {
                this.Key = conversion_result;
            }
            
                    }
                    break;
                
                case 1:
                    {
                        string conversion_result = TypeConverter<T>.ConvertTo_string(value);
                        
            {
                this.KGId = conversion_result;
            }
            
                    }
                    break;
                
                case 2:
                    {
                        List<string> conversion_result = TypeConverter<T>.ConvertTo_List_string(value);
                        
            {
                this.Genres = conversion_result;
            }
            
                    }
                    break;
                
                case 3:
                    {
                        List<long> conversion_result = TypeConverter<T>.ConvertTo_List_long(value);
                        
            {
                this.Artists = conversion_result;
            }
            
                    }
                    break;
                
                case 4:
                    {
                        List<long> conversion_result = TypeConverter<T>.ConvertTo_List_long(value);
                        
            {
                this.Directors = conversion_result;
            }
            
                    }
                    break;
                
                case 5:
                    {
                        string conversion_result = TypeConverter<T>.ConvertTo_string(value);
                        
            {
                this.Characters = conversion_result;
            }
            
                    }
                    break;
                
                case 6:
                    {
                        List<string> conversion_result = TypeConverter<T>.ConvertTo_List_string(value);
                        
            {
                this.Performance = conversion_result;
            }
            
                    }
                    break;
                
                case 7:
                    {
                        string conversion_result = TypeConverter<T>.ConvertTo_string(value);
                        
            {
                this.Distributors = conversion_result;
            }
            
                    }
                    break;
                
                case 8:
                    {
                        string conversion_result = TypeConverter<T>.ConvertTo_string(value);
                        
            {
                this.Channels = conversion_result;
            }
            
                    }
                    break;
                
                case 9:
                    {
                        string conversion_result = TypeConverter<T>.ConvertTo_string(value);
                        
            {
                this.Albums = conversion_result;
            }
            
                    }
                    break;
                
                case 10:
                    {
                        string conversion_result = TypeConverter<T>.ConvertTo_string(value);
                        
            {
                this.Name = conversion_result;
            }
            
                    }
                    break;
                
                case 11:
                    {
                        string conversion_result = TypeConverter<T>.ConvertTo_string(value);
                        
            {
                this.Alias = conversion_result;
            }
            
                    }
                    break;
                
                case 12:
                    {
                        string conversion_result = TypeConverter<T>.ConvertTo_string(value);
                        
            {
                this.Description = conversion_result;
            }
            
                    }
                    break;
                
                case 13:
                    {
                        string conversion_result = TypeConverter<T>.ConvertTo_string(value);
                        
            {
                this.Segments = conversion_result;
            }
            
                    }
                    break;
                
                case 14:
                    {
                        string conversion_result = TypeConverter<T>.ConvertTo_string(value);
                        
            {
                this.Categories = conversion_result;
            }
            
                    }
                    break;
                
                case 15:
                    {
                        string conversion_result = TypeConverter<T>.ConvertTo_string(value);
                        
            {
                this.IntEmbeddedFilters = conversion_result;
            }
            
                    }
                    break;
                
                case 16:
                    {
                        string conversion_result = TypeConverter<T>.ConvertTo_string(value);
                        
            {
                this.NumberOfWantToWatch = conversion_result;
            }
            
                    }
                    break;
                
                case 17:
                    {
                        string conversion_result = TypeConverter<T>.ConvertTo_string(value);
                        
            {
                this.Rating = conversion_result;
            }
            
                    }
                    break;
                
                case 18:
                    {
                        string conversion_result = TypeConverter<T>.ConvertTo_string(value);
                        
            {
                this.NumberOfShortReview = conversion_result;
            }
            
                    }
                    break;
                
                case 19:
                    {
                        string conversion_result = TypeConverter<T>.ConvertTo_string(value);
                        
            {
                this.ReviewCount = conversion_result;
            }
            
                    }
                    break;
                
                case 20:
                    {
                        string conversion_result = TypeConverter<T>.ConvertTo_string(value);
                        
            {
                this.NumberOfWatched = conversion_result;
            }
            
                    }
                    break;
                
                case 21:
                    {
                        string conversion_result = TypeConverter<T>.ConvertTo_string(value);
                        
            {
                this.NumberOfReviewer = conversion_result;
            }
            
                    }
                    break;
                
                case 22:
                    {
                        string conversion_result = TypeConverter<T>.ConvertTo_string(value);
                        
            {
                this.PublishDate = conversion_result;
            }
            
                    }
                    break;
                
                case 23:
                    {
                        string conversion_result = TypeConverter<T>.ConvertTo_string(value);
                        
            {
                this.Length = conversion_result;
            }
            
                    }
                    break;
                
                case 24:
                    {
                        string conversion_result = TypeConverter<T>.ConvertTo_string(value);
                        
            {
                this.Country = conversion_result;
            }
            
                    }
                    break;
                
                case 25:
                    {
                        string conversion_result = TypeConverter<T>.ConvertTo_string(value);
                        
            {
                this.Language = conversion_result;
            }
            
                    }
                    break;
                
                case 26:
                    {
                        string conversion_result = TypeConverter<T>.ConvertTo_string(value);
                        
            {
                this.SourceUrls = conversion_result;
            }
            
                    }
                    break;
                
                case 27:
                    {
                        string conversion_result = TypeConverter<T>.ConvertTo_string(value);
                        
            {
                this.ImageUrls = conversion_result;
            }
            
                    }
                    break;
                
                case 28:
                    {
                        string conversion_result = TypeConverter<T>.ConvertTo_string(value);
                        
            {
                this.OfficialSite = conversion_result;
            }
            
                    }
                    break;
                
                case 29:
                    {
                        string conversion_result = TypeConverter<T>.ConvertTo_string(value);
                        
            {
                this.EntityContainer = conversion_result;
            }
            
                    }
                    break;
                
                case 30:
                    {
                        string conversion_result = TypeConverter<T>.ConvertTo_string(value);
                        
            {
                this.Logo = conversion_result;
            }
            
                    }
                    break;
                
                case 31:
                    {
                        string conversion_result = TypeConverter<T>.ConvertTo_string(value);
                        
            {
                this.QueryRank = conversion_result;
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
                    
                case 8:
                    
                    return true;
                    
                case 9:
                    
                    return true;
                    
                case 10:
                    
                    return true;
                    
                case 11:
                    
                    return true;
                    
                case 12:
                    
                    return true;
                    
                case 13:
                    
                    return true;
                    
                case 14:
                    
                    return true;
                    
                case 15:
                    
                    return true;
                    
                case 16:
                    
                    return true;
                    
                case 17:
                    
                    return true;
                    
                case 18:
                    
                    return true;
                    
                case 19:
                    
                    return true;
                    
                case 20:
                    
                    return true;
                    
                case 21:
                    
                    return true;
                    
                case 22:
                    
                    return true;
                    
                case 23:
                    
                    return true;
                    
                case 24:
                    
                    return true;
                    
                case 25:
                    
                    return true;
                    
                case 26:
                    
                    return true;
                    
                case 27:
                    
                    return true;
                    
                case 28:
                    
                    return true;
                    
                case 29:
                    
                    return true;
                    
                case 30:
                    
                    return true;
                    
                case 31:
                    
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
                
                case 0:
                    
                    {
                        
                        this.Key += TypeConverter<T>.ConvertTo_string(value);
                    }
                    
                    break;
                
                case 1:
                    
                    {
                        
                        this.KGId += TypeConverter<T>.ConvertTo_string(value);
                    }
                    
                    break;
                
                case 2:
                    
                    {
                        
                        switch (TypeConverter<T>.GetConversionActionTo_List_string())
                        {
                            case TypeConversionAction.TC_ASSIGN:
                                foreach (var element in value as List<string>)
                                    this.Genres.Add(element);
                                break;
                            case TypeConversionAction.TC_CONVERTLIST:
                            case TypeConversionAction.TC_ARRAYTOLIST:
                                foreach (var element in TypeConverter<T>.Enumerate_string(value))
                                    this.Genres.Add(element);
                                break;
                            case TypeConversionAction.TC_WRAPINLIST:
                            case TypeConversionAction.TC_PARSESTRING:
                                this.Genres.Add(TypeConverter<T>.ConvertTo_string(value));
                                break;
                            default:
                                Throw.data_type_incompatible_with_list(typeof(T).ToString());
                                break;
                        }
                    }
                    
                    break;
                
                case 3:
                    
                    {
                        
                        switch (TypeConverter<T>.GetConversionActionTo_List_long())
                        {
                            case TypeConversionAction.TC_ASSIGN:
                                foreach (var element in value as List<long>)
                                    this.Artists.Add(element);
                                break;
                            case TypeConversionAction.TC_CONVERTLIST:
                            case TypeConversionAction.TC_ARRAYTOLIST:
                                foreach (var element in TypeConverter<T>.Enumerate_long(value))
                                    this.Artists.Add(element);
                                break;
                            case TypeConversionAction.TC_WRAPINLIST:
                            case TypeConversionAction.TC_PARSESTRING:
                                this.Artists.Add(TypeConverter<T>.ConvertTo_long(value));
                                break;
                            default:
                                Throw.data_type_incompatible_with_list(typeof(T).ToString());
                                break;
                        }
                    }
                    
                    break;
                
                case 4:
                    
                    {
                        
                        switch (TypeConverter<T>.GetConversionActionTo_List_long())
                        {
                            case TypeConversionAction.TC_ASSIGN:
                                foreach (var element in value as List<long>)
                                    this.Directors.Add(element);
                                break;
                            case TypeConversionAction.TC_CONVERTLIST:
                            case TypeConversionAction.TC_ARRAYTOLIST:
                                foreach (var element in TypeConverter<T>.Enumerate_long(value))
                                    this.Directors.Add(element);
                                break;
                            case TypeConversionAction.TC_WRAPINLIST:
                            case TypeConversionAction.TC_PARSESTRING:
                                this.Directors.Add(TypeConverter<T>.ConvertTo_long(value));
                                break;
                            default:
                                Throw.data_type_incompatible_with_list(typeof(T).ToString());
                                break;
                        }
                    }
                    
                    break;
                
                case 5:
                    
                    {
                        
                        this.Characters += TypeConverter<T>.ConvertTo_string(value);
                    }
                    
                    break;
                
                case 6:
                    
                    {
                        
                        switch (TypeConverter<T>.GetConversionActionTo_List_string())
                        {
                            case TypeConversionAction.TC_ASSIGN:
                                foreach (var element in value as List<string>)
                                    this.Performance.Add(element);
                                break;
                            case TypeConversionAction.TC_CONVERTLIST:
                            case TypeConversionAction.TC_ARRAYTOLIST:
                                foreach (var element in TypeConverter<T>.Enumerate_string(value))
                                    this.Performance.Add(element);
                                break;
                            case TypeConversionAction.TC_WRAPINLIST:
                            case TypeConversionAction.TC_PARSESTRING:
                                this.Performance.Add(TypeConverter<T>.ConvertTo_string(value));
                                break;
                            default:
                                Throw.data_type_incompatible_with_list(typeof(T).ToString());
                                break;
                        }
                    }
                    
                    break;
                
                case 7:
                    
                    {
                        
                        this.Distributors += TypeConverter<T>.ConvertTo_string(value);
                    }
                    
                    break;
                
                case 8:
                    
                    {
                        
                        this.Channels += TypeConverter<T>.ConvertTo_string(value);
                    }
                    
                    break;
                
                case 9:
                    
                    {
                        
                        this.Albums += TypeConverter<T>.ConvertTo_string(value);
                    }
                    
                    break;
                
                case 10:
                    
                    {
                        
                        this.Name += TypeConverter<T>.ConvertTo_string(value);
                    }
                    
                    break;
                
                case 11:
                    
                    {
                        
                        this.Alias += TypeConverter<T>.ConvertTo_string(value);
                    }
                    
                    break;
                
                case 12:
                    
                    {
                        
                        this.Description += TypeConverter<T>.ConvertTo_string(value);
                    }
                    
                    break;
                
                case 13:
                    
                    {
                        
                        this.Segments += TypeConverter<T>.ConvertTo_string(value);
                    }
                    
                    break;
                
                case 14:
                    
                    {
                        
                        this.Categories += TypeConverter<T>.ConvertTo_string(value);
                    }
                    
                    break;
                
                case 15:
                    
                    {
                        
                        this.IntEmbeddedFilters += TypeConverter<T>.ConvertTo_string(value);
                    }
                    
                    break;
                
                case 16:
                    
                    {
                        
                        this.NumberOfWantToWatch += TypeConverter<T>.ConvertTo_string(value);
                    }
                    
                    break;
                
                case 17:
                    
                    {
                        
                        this.Rating += TypeConverter<T>.ConvertTo_string(value);
                    }
                    
                    break;
                
                case 18:
                    
                    {
                        
                        this.NumberOfShortReview += TypeConverter<T>.ConvertTo_string(value);
                    }
                    
                    break;
                
                case 19:
                    
                    {
                        
                        this.ReviewCount += TypeConverter<T>.ConvertTo_string(value);
                    }
                    
                    break;
                
                case 20:
                    
                    {
                        
                        this.NumberOfWatched += TypeConverter<T>.ConvertTo_string(value);
                    }
                    
                    break;
                
                case 21:
                    
                    {
                        
                        this.NumberOfReviewer += TypeConverter<T>.ConvertTo_string(value);
                    }
                    
                    break;
                
                case 22:
                    
                    {
                        
                        this.PublishDate += TypeConverter<T>.ConvertTo_string(value);
                    }
                    
                    break;
                
                case 23:
                    
                    {
                        
                        this.Length += TypeConverter<T>.ConvertTo_string(value);
                    }
                    
                    break;
                
                case 24:
                    
                    {
                        
                        this.Country += TypeConverter<T>.ConvertTo_string(value);
                    }
                    
                    break;
                
                case 25:
                    
                    {
                        
                        this.Language += TypeConverter<T>.ConvertTo_string(value);
                    }
                    
                    break;
                
                case 26:
                    
                    {
                        
                        this.SourceUrls += TypeConverter<T>.ConvertTo_string(value);
                    }
                    
                    break;
                
                case 27:
                    
                    {
                        
                        this.ImageUrls += TypeConverter<T>.ConvertTo_string(value);
                    }
                    
                    break;
                
                case 28:
                    
                    {
                        
                        this.OfficialSite += TypeConverter<T>.ConvertTo_string(value);
                    }
                    
                    break;
                
                case 29:
                    
                    {
                        
                        this.EntityContainer += TypeConverter<T>.ConvertTo_string(value);
                    }
                    
                    break;
                
                case 30:
                    
                    {
                        
                        this.Logo += TypeConverter<T>.ConvertTo_string(value);
                    }
                    
                    break;
                
                case 31:
                    
                    {
                        
                        this.QueryRank += TypeConverter<T>.ConvertTo_string(value);
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
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Key, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Key", TypeConverter<T>.ConvertFrom_string(this.Key));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.KGId, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("KGId", TypeConverter<T>.ConvertFrom_string(this.KGId));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Characters, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Characters", TypeConverter<T>.ConvertFrom_string(this.Characters));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Distributors, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Distributors", TypeConverter<T>.ConvertFrom_string(this.Distributors));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Channels, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Channels", TypeConverter<T>.ConvertFrom_string(this.Channels));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Albums, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Albums", TypeConverter<T>.ConvertFrom_string(this.Albums));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Name, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Name", TypeConverter<T>.ConvertFrom_string(this.Name));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Alias, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Alias", TypeConverter<T>.ConvertFrom_string(this.Alias));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Description, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Description", TypeConverter<T>.ConvertFrom_string(this.Description));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Segments, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Segments", TypeConverter<T>.ConvertFrom_string(this.Segments));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Categories, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Categories", TypeConverter<T>.ConvertFrom_string(this.Categories));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.IntEmbeddedFilters, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("IntEmbeddedFilters", TypeConverter<T>.ConvertFrom_string(this.IntEmbeddedFilters));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.NumberOfWantToWatch, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("NumberOfWantToWatch", TypeConverter<T>.ConvertFrom_string(this.NumberOfWantToWatch));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Rating, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Rating", TypeConverter<T>.ConvertFrom_string(this.Rating));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.NumberOfShortReview, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("NumberOfShortReview", TypeConverter<T>.ConvertFrom_string(this.NumberOfShortReview));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.ReviewCount, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("ReviewCount", TypeConverter<T>.ConvertFrom_string(this.ReviewCount));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.NumberOfWatched, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("NumberOfWatched", TypeConverter<T>.ConvertFrom_string(this.NumberOfWatched));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.NumberOfReviewer, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("NumberOfReviewer", TypeConverter<T>.ConvertFrom_string(this.NumberOfReviewer));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.PublishDate, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("PublishDate", TypeConverter<T>.ConvertFrom_string(this.PublishDate));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Length, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Length", TypeConverter<T>.ConvertFrom_string(this.Length));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Country, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Country", TypeConverter<T>.ConvertFrom_string(this.Country));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Language, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Language", TypeConverter<T>.ConvertFrom_string(this.Language));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.SourceUrls, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("SourceUrls", TypeConverter<T>.ConvertFrom_string(this.SourceUrls));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.ImageUrls, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("ImageUrls", TypeConverter<T>.ConvertFrom_string(this.ImageUrls));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.OfficialSite, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("OfficialSite", TypeConverter<T>.ConvertFrom_string(this.OfficialSite));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.EntityContainer, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("EntityContainer", TypeConverter<T>.ConvertFrom_string(this.EntityContainer));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Logo, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Logo", TypeConverter<T>.ConvertFrom_string(this.Logo));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.QueryRank, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("QueryRank", TypeConverter<T>.ConvertFrom_string(this.QueryRank));
                    
                    break;
                
                case  1:
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Key, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Key", TypeConverter<T>.ConvertFrom_string(this.Key));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.KGId, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("KGId", TypeConverter<T>.ConvertFrom_string(this.KGId));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Characters, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Characters", TypeConverter<T>.ConvertFrom_string(this.Characters));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Distributors, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Distributors", TypeConverter<T>.ConvertFrom_string(this.Distributors));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Channels, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Channels", TypeConverter<T>.ConvertFrom_string(this.Channels));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Albums, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Albums", TypeConverter<T>.ConvertFrom_string(this.Albums));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Name, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Name", TypeConverter<T>.ConvertFrom_string(this.Name));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Alias, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Alias", TypeConverter<T>.ConvertFrom_string(this.Alias));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Description, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Description", TypeConverter<T>.ConvertFrom_string(this.Description));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Segments, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Segments", TypeConverter<T>.ConvertFrom_string(this.Segments));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Categories, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Categories", TypeConverter<T>.ConvertFrom_string(this.Categories));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.IntEmbeddedFilters, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("IntEmbeddedFilters", TypeConverter<T>.ConvertFrom_string(this.IntEmbeddedFilters));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.NumberOfWantToWatch, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("NumberOfWantToWatch", TypeConverter<T>.ConvertFrom_string(this.NumberOfWantToWatch));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Rating, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Rating", TypeConverter<T>.ConvertFrom_string(this.Rating));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.NumberOfShortReview, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("NumberOfShortReview", TypeConverter<T>.ConvertFrom_string(this.NumberOfShortReview));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.ReviewCount, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("ReviewCount", TypeConverter<T>.ConvertFrom_string(this.ReviewCount));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.NumberOfWatched, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("NumberOfWatched", TypeConverter<T>.ConvertFrom_string(this.NumberOfWatched));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.NumberOfReviewer, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("NumberOfReviewer", TypeConverter<T>.ConvertFrom_string(this.NumberOfReviewer));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.PublishDate, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("PublishDate", TypeConverter<T>.ConvertFrom_string(this.PublishDate));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Length, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Length", TypeConverter<T>.ConvertFrom_string(this.Length));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Country, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Country", TypeConverter<T>.ConvertFrom_string(this.Country));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Language, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Language", TypeConverter<T>.ConvertFrom_string(this.Language));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.SourceUrls, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("SourceUrls", TypeConverter<T>.ConvertFrom_string(this.SourceUrls));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.ImageUrls, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("ImageUrls", TypeConverter<T>.ConvertFrom_string(this.ImageUrls));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.OfficialSite, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("OfficialSite", TypeConverter<T>.ConvertFrom_string(this.OfficialSite));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.EntityContainer, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("EntityContainer", TypeConverter<T>.ConvertFrom_string(this.EntityContainer));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Logo, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Logo", TypeConverter<T>.ConvertFrom_string(this.Logo));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.QueryRank, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("QueryRank", TypeConverter<T>.ConvertFrom_string(this.QueryRank));
                    
                    break;
                
                case  2:
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Key, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Key", TypeConverter<T>.ConvertFrom_string(this.Key));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.KGId, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("KGId", TypeConverter<T>.ConvertFrom_string(this.KGId));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Characters, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Characters", TypeConverter<T>.ConvertFrom_string(this.Characters));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Distributors, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Distributors", TypeConverter<T>.ConvertFrom_string(this.Distributors));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Channels, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Channels", TypeConverter<T>.ConvertFrom_string(this.Channels));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Albums, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Albums", TypeConverter<T>.ConvertFrom_string(this.Albums));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Name, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Name", TypeConverter<T>.ConvertFrom_string(this.Name));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Alias, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Alias", TypeConverter<T>.ConvertFrom_string(this.Alias));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Description, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Description", TypeConverter<T>.ConvertFrom_string(this.Description));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Segments, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Segments", TypeConverter<T>.ConvertFrom_string(this.Segments));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Categories, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Categories", TypeConverter<T>.ConvertFrom_string(this.Categories));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.IntEmbeddedFilters, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("IntEmbeddedFilters", TypeConverter<T>.ConvertFrom_string(this.IntEmbeddedFilters));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.NumberOfWantToWatch, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("NumberOfWantToWatch", TypeConverter<T>.ConvertFrom_string(this.NumberOfWantToWatch));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Rating, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Rating", TypeConverter<T>.ConvertFrom_string(this.Rating));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.NumberOfShortReview, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("NumberOfShortReview", TypeConverter<T>.ConvertFrom_string(this.NumberOfShortReview));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.ReviewCount, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("ReviewCount", TypeConverter<T>.ConvertFrom_string(this.ReviewCount));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.NumberOfWatched, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("NumberOfWatched", TypeConverter<T>.ConvertFrom_string(this.NumberOfWatched));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.NumberOfReviewer, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("NumberOfReviewer", TypeConverter<T>.ConvertFrom_string(this.NumberOfReviewer));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.PublishDate, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("PublishDate", TypeConverter<T>.ConvertFrom_string(this.PublishDate));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Length, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Length", TypeConverter<T>.ConvertFrom_string(this.Length));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Country, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Country", TypeConverter<T>.ConvertFrom_string(this.Country));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Language, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Language", TypeConverter<T>.ConvertFrom_string(this.Language));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.SourceUrls, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("SourceUrls", TypeConverter<T>.ConvertFrom_string(this.SourceUrls));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.ImageUrls, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("ImageUrls", TypeConverter<T>.ConvertFrom_string(this.ImageUrls));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.OfficialSite, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("OfficialSite", TypeConverter<T>.ConvertFrom_string(this.OfficialSite));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.EntityContainer, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("EntityContainer", TypeConverter<T>.ConvertFrom_string(this.EntityContainer));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Logo, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Logo", TypeConverter<T>.ConvertFrom_string(this.Logo));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.QueryRank, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("QueryRank", TypeConverter<T>.ConvertFrom_string(this.QueryRank));
                    
                    break;
                
                case  3:
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Key, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Key", TypeConverter<T>.ConvertFrom_string(this.Key));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.KGId, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("KGId", TypeConverter<T>.ConvertFrom_string(this.KGId));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Characters, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Characters", TypeConverter<T>.ConvertFrom_string(this.Characters));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Distributors, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Distributors", TypeConverter<T>.ConvertFrom_string(this.Distributors));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Channels, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Channels", TypeConverter<T>.ConvertFrom_string(this.Channels));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Albums, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Albums", TypeConverter<T>.ConvertFrom_string(this.Albums));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Name, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Name", TypeConverter<T>.ConvertFrom_string(this.Name));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Alias, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Alias", TypeConverter<T>.ConvertFrom_string(this.Alias));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Description, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Description", TypeConverter<T>.ConvertFrom_string(this.Description));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Segments, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Segments", TypeConverter<T>.ConvertFrom_string(this.Segments));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Categories, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Categories", TypeConverter<T>.ConvertFrom_string(this.Categories));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.IntEmbeddedFilters, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("IntEmbeddedFilters", TypeConverter<T>.ConvertFrom_string(this.IntEmbeddedFilters));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.NumberOfWantToWatch, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("NumberOfWantToWatch", TypeConverter<T>.ConvertFrom_string(this.NumberOfWantToWatch));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Rating, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Rating", TypeConverter<T>.ConvertFrom_string(this.Rating));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.NumberOfShortReview, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("NumberOfShortReview", TypeConverter<T>.ConvertFrom_string(this.NumberOfShortReview));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.ReviewCount, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("ReviewCount", TypeConverter<T>.ConvertFrom_string(this.ReviewCount));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.NumberOfWatched, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("NumberOfWatched", TypeConverter<T>.ConvertFrom_string(this.NumberOfWatched));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.NumberOfReviewer, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("NumberOfReviewer", TypeConverter<T>.ConvertFrom_string(this.NumberOfReviewer));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.PublishDate, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("PublishDate", TypeConverter<T>.ConvertFrom_string(this.PublishDate));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Length, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Length", TypeConverter<T>.ConvertFrom_string(this.Length));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Country, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Country", TypeConverter<T>.ConvertFrom_string(this.Country));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Language, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Language", TypeConverter<T>.ConvertFrom_string(this.Language));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.SourceUrls, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("SourceUrls", TypeConverter<T>.ConvertFrom_string(this.SourceUrls));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.ImageUrls, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("ImageUrls", TypeConverter<T>.ConvertFrom_string(this.ImageUrls));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.OfficialSite, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("OfficialSite", TypeConverter<T>.ConvertFrom_string(this.OfficialSite));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.EntityContainer, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("EntityContainer", TypeConverter<T>.ConvertFrom_string(this.EntityContainer));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Logo, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Logo", TypeConverter<T>.ConvertFrom_string(this.Logo));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.QueryRank, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("QueryRank", TypeConverter<T>.ConvertFrom_string(this.QueryRank));
                    
                    break;
                
                case  4:
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Key, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Key", TypeConverter<T>.ConvertFrom_string(this.Key));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.KGId, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("KGId", TypeConverter<T>.ConvertFrom_string(this.KGId));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Genres, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Genres", TypeConverter<T>.ConvertFrom_List_string(this.Genres));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Artists, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Artists", TypeConverter<T>.ConvertFrom_List_long(this.Artists));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Directors, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Directors", TypeConverter<T>.ConvertFrom_List_long(this.Directors));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Characters, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Characters", TypeConverter<T>.ConvertFrom_string(this.Characters));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Performance, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Performance", TypeConverter<T>.ConvertFrom_List_string(this.Performance));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Distributors, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Distributors", TypeConverter<T>.ConvertFrom_string(this.Distributors));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Channels, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Channels", TypeConverter<T>.ConvertFrom_string(this.Channels));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Albums, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Albums", TypeConverter<T>.ConvertFrom_string(this.Albums));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Name, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Name", TypeConverter<T>.ConvertFrom_string(this.Name));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Alias, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Alias", TypeConverter<T>.ConvertFrom_string(this.Alias));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Description, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Description", TypeConverter<T>.ConvertFrom_string(this.Description));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Segments, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Segments", TypeConverter<T>.ConvertFrom_string(this.Segments));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Categories, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Categories", TypeConverter<T>.ConvertFrom_string(this.Categories));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.IntEmbeddedFilters, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("IntEmbeddedFilters", TypeConverter<T>.ConvertFrom_string(this.IntEmbeddedFilters));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.NumberOfWantToWatch, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("NumberOfWantToWatch", TypeConverter<T>.ConvertFrom_string(this.NumberOfWantToWatch));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Rating, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Rating", TypeConverter<T>.ConvertFrom_string(this.Rating));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.NumberOfShortReview, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("NumberOfShortReview", TypeConverter<T>.ConvertFrom_string(this.NumberOfShortReview));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.ReviewCount, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("ReviewCount", TypeConverter<T>.ConvertFrom_string(this.ReviewCount));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.NumberOfWatched, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("NumberOfWatched", TypeConverter<T>.ConvertFrom_string(this.NumberOfWatched));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.NumberOfReviewer, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("NumberOfReviewer", TypeConverter<T>.ConvertFrom_string(this.NumberOfReviewer));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.PublishDate, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("PublishDate", TypeConverter<T>.ConvertFrom_string(this.PublishDate));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Length, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Length", TypeConverter<T>.ConvertFrom_string(this.Length));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Country, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Country", TypeConverter<T>.ConvertFrom_string(this.Country));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Language, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Language", TypeConverter<T>.ConvertFrom_string(this.Language));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.SourceUrls, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("SourceUrls", TypeConverter<T>.ConvertFrom_string(this.SourceUrls));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.ImageUrls, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("ImageUrls", TypeConverter<T>.ConvertFrom_string(this.ImageUrls));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.OfficialSite, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("OfficialSite", TypeConverter<T>.ConvertFrom_string(this.OfficialSite));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.EntityContainer, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("EntityContainer", TypeConverter<T>.ConvertFrom_string(this.EntityContainer));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Logo, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Logo", TypeConverter<T>.ConvertFrom_string(this.Logo));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.QueryRank, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("QueryRank", TypeConverter<T>.ConvertFrom_string(this.QueryRank));
                    
                    break;
                
                case  5:
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Key, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Key", TypeConverter<T>.ConvertFrom_string(this.Key));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.KGId, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("KGId", TypeConverter<T>.ConvertFrom_string(this.KGId));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Genres, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Genres", TypeConverter<T>.ConvertFrom_List_string(this.Genres));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Artists, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Artists", TypeConverter<T>.ConvertFrom_List_long(this.Artists));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Directors, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Directors", TypeConverter<T>.ConvertFrom_List_long(this.Directors));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Characters, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Characters", TypeConverter<T>.ConvertFrom_string(this.Characters));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Performance, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Performance", TypeConverter<T>.ConvertFrom_List_string(this.Performance));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Distributors, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Distributors", TypeConverter<T>.ConvertFrom_string(this.Distributors));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Channels, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Channels", TypeConverter<T>.ConvertFrom_string(this.Channels));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Albums, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Albums", TypeConverter<T>.ConvertFrom_string(this.Albums));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Name, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Name", TypeConverter<T>.ConvertFrom_string(this.Name));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Alias, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Alias", TypeConverter<T>.ConvertFrom_string(this.Alias));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Description, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Description", TypeConverter<T>.ConvertFrom_string(this.Description));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Segments, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Segments", TypeConverter<T>.ConvertFrom_string(this.Segments));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Categories, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Categories", TypeConverter<T>.ConvertFrom_string(this.Categories));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.IntEmbeddedFilters, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("IntEmbeddedFilters", TypeConverter<T>.ConvertFrom_string(this.IntEmbeddedFilters));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.NumberOfWantToWatch, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("NumberOfWantToWatch", TypeConverter<T>.ConvertFrom_string(this.NumberOfWantToWatch));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Rating, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Rating", TypeConverter<T>.ConvertFrom_string(this.Rating));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.NumberOfShortReview, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("NumberOfShortReview", TypeConverter<T>.ConvertFrom_string(this.NumberOfShortReview));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.ReviewCount, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("ReviewCount", TypeConverter<T>.ConvertFrom_string(this.ReviewCount));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.NumberOfWatched, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("NumberOfWatched", TypeConverter<T>.ConvertFrom_string(this.NumberOfWatched));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.NumberOfReviewer, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("NumberOfReviewer", TypeConverter<T>.ConvertFrom_string(this.NumberOfReviewer));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.PublishDate, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("PublishDate", TypeConverter<T>.ConvertFrom_string(this.PublishDate));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Length, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Length", TypeConverter<T>.ConvertFrom_string(this.Length));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Country, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Country", TypeConverter<T>.ConvertFrom_string(this.Country));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Language, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Language", TypeConverter<T>.ConvertFrom_string(this.Language));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.SourceUrls, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("SourceUrls", TypeConverter<T>.ConvertFrom_string(this.SourceUrls));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.ImageUrls, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("ImageUrls", TypeConverter<T>.ConvertFrom_string(this.ImageUrls));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.OfficialSite, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("OfficialSite", TypeConverter<T>.ConvertFrom_string(this.OfficialSite));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.EntityContainer, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("EntityContainer", TypeConverter<T>.ConvertFrom_string(this.EntityContainer));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Logo, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Logo", TypeConverter<T>.ConvertFrom_string(this.Logo));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.QueryRank, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("QueryRank", TypeConverter<T>.ConvertFrom_string(this.QueryRank));
                    
                    break;
                
                case  6:
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Key, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Key", TypeConverter<T>.ConvertFrom_string(this.Key));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.KGId, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("KGId", TypeConverter<T>.ConvertFrom_string(this.KGId));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Genres, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Genres", TypeConverter<T>.ConvertFrom_List_string(this.Genres));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Artists, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Artists", TypeConverter<T>.ConvertFrom_List_long(this.Artists));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Directors, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Directors", TypeConverter<T>.ConvertFrom_List_long(this.Directors));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Characters, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Characters", TypeConverter<T>.ConvertFrom_string(this.Characters));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Performance, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Performance", TypeConverter<T>.ConvertFrom_List_string(this.Performance));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Distributors, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Distributors", TypeConverter<T>.ConvertFrom_string(this.Distributors));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Channels, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Channels", TypeConverter<T>.ConvertFrom_string(this.Channels));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Albums, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Albums", TypeConverter<T>.ConvertFrom_string(this.Albums));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Name, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Name", TypeConverter<T>.ConvertFrom_string(this.Name));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Alias, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Alias", TypeConverter<T>.ConvertFrom_string(this.Alias));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Description, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Description", TypeConverter<T>.ConvertFrom_string(this.Description));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Segments, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Segments", TypeConverter<T>.ConvertFrom_string(this.Segments));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Categories, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Categories", TypeConverter<T>.ConvertFrom_string(this.Categories));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.IntEmbeddedFilters, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("IntEmbeddedFilters", TypeConverter<T>.ConvertFrom_string(this.IntEmbeddedFilters));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.NumberOfWantToWatch, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("NumberOfWantToWatch", TypeConverter<T>.ConvertFrom_string(this.NumberOfWantToWatch));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Rating, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Rating", TypeConverter<T>.ConvertFrom_string(this.Rating));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.NumberOfShortReview, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("NumberOfShortReview", TypeConverter<T>.ConvertFrom_string(this.NumberOfShortReview));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.ReviewCount, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("ReviewCount", TypeConverter<T>.ConvertFrom_string(this.ReviewCount));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.NumberOfWatched, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("NumberOfWatched", TypeConverter<T>.ConvertFrom_string(this.NumberOfWatched));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.NumberOfReviewer, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("NumberOfReviewer", TypeConverter<T>.ConvertFrom_string(this.NumberOfReviewer));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.PublishDate, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("PublishDate", TypeConverter<T>.ConvertFrom_string(this.PublishDate));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Length, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Length", TypeConverter<T>.ConvertFrom_string(this.Length));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Country, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Country", TypeConverter<T>.ConvertFrom_string(this.Country));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Language, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Language", TypeConverter<T>.ConvertFrom_string(this.Language));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.SourceUrls, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("SourceUrls", TypeConverter<T>.ConvertFrom_string(this.SourceUrls));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.ImageUrls, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("ImageUrls", TypeConverter<T>.ConvertFrom_string(this.ImageUrls));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.OfficialSite, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("OfficialSite", TypeConverter<T>.ConvertFrom_string(this.OfficialSite));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.EntityContainer, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("EntityContainer", TypeConverter<T>.ConvertFrom_string(this.EntityContainer));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.Logo, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("Logo", TypeConverter<T>.ConvertFrom_string(this.Logo));
                    
                    if (StorageSchema.Movie_descriptor.check_attribute(StorageSchema.Movie_descriptor.QueryRank, attributeKey, attributeValue))
                        
                            yield return new KeyValuePair<string, T>("QueryRank", TypeConverter<T>.ConvertFrom_string(this.QueryRank));
                    
                    break;
                
                default:
                    Throw.incompatible_with_cell();
                    break;
            }
            yield break;
        }
        #region enumerate value methods
        
        private IEnumerable<T> _enumerate_from_Key<T>()
        {
            
                switch (TypeConverter<T>.type_id)
                {
                    
                    case  0:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Key);
                            
                        }
                        break;
                    
                    case  1:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Key);
                            
                        }
                        break;
                    
                    case  2:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Key);
                            
                        }
                        break;
                    
                    case  3:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Key);
                            
                        }
                        break;
                    
                    case  4:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Key);
                            
                        }
                        break;
                    
                    case  5:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Key);
                            
                        }
                        break;
                    
                    case  6:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Key);
                            
                        }
                        break;
                    
                    default:
                        Throw.incompatible_with_cell();
                        break;
                }
            yield break;
        }
        
        private IEnumerable<T> _enumerate_from_KGId<T>()
        {
            
                switch (TypeConverter<T>.type_id)
                {
                    
                    case  0:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.KGId);
                            
                        }
                        break;
                    
                    case  1:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.KGId);
                            
                        }
                        break;
                    
                    case  2:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.KGId);
                            
                        }
                        break;
                    
                    case  3:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.KGId);
                            
                        }
                        break;
                    
                    case  4:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.KGId);
                            
                        }
                        break;
                    
                    case  5:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.KGId);
                            
                        }
                        break;
                    
                    case  6:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.KGId);
                            
                        }
                        break;
                    
                    default:
                        Throw.incompatible_with_cell();
                        break;
                }
            yield break;
        }
        
        private IEnumerable<T> _enumerate_from_Genres<T>()
        {
            
                switch (TypeConverter<T>.type_id)
                {
                    
                    case  0:
                        {
                            
                            {
                                
                                var element0 = this.Genres;
                                
                                foreach (var element1 in element0)
                                
                                {
                                    yield return TypeConverter<T>.ConvertFrom_string(element1);
                                }
                            }
                            
                        }
                        break;
                    
                    case  1:
                        {
                            
                            {
                                
                                var element0 = this.Genres;
                                
                                foreach (var element1 in element0)
                                
                                {
                                    yield return TypeConverter<T>.ConvertFrom_string(element1);
                                }
                            }
                            
                        }
                        break;
                    
                    case  2:
                        {
                            
                            {
                                
                                var element0 = this.Genres;
                                
                                foreach (var element1 in element0)
                                
                                {
                                    yield return TypeConverter<T>.ConvertFrom_string(element1);
                                }
                            }
                            
                        }
                        break;
                    
                    case  3:
                        {
                            
                            {
                                
                                var element0 = this.Genres;
                                
                                foreach (var element1 in element0)
                                
                                {
                                    yield return TypeConverter<T>.ConvertFrom_string(element1);
                                }
                            }
                            
                        }
                        break;
                    
                    case  4:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_List_string(this.Genres);
                            
                        }
                        break;
                    
                    case  5:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_List_string(this.Genres);
                            
                        }
                        break;
                    
                    case  6:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_List_string(this.Genres);
                            
                        }
                        break;
                    
                    default:
                        Throw.incompatible_with_cell();
                        break;
                }
            yield break;
        }
        
        private IEnumerable<T> _enumerate_from_Artists<T>()
        {
            
                switch (TypeConverter<T>.type_id)
                {
                    
                    case  1:
                        {
                            
                            {
                                
                                var element0 = this.Artists;
                                
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
                                
                                var element0 = this.Artists;
                                
                                foreach (var element1 in element0)
                                
                                {
                                    yield return TypeConverter<T>.ConvertFrom_long(element1);
                                }
                            }
                            
                        }
                        break;
                    
                    case  4:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_List_long(this.Artists);
                            
                        }
                        break;
                    
                    case  5:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_List_long(this.Artists);
                            
                        }
                        break;
                    
                    case  6:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_List_long(this.Artists);
                            
                        }
                        break;
                    
                    default:
                        Throw.incompatible_with_cell();
                        break;
                }
            yield break;
        }
        
        private IEnumerable<T> _enumerate_from_Directors<T>()
        {
            
                switch (TypeConverter<T>.type_id)
                {
                    
                    case  1:
                        {
                            
                            {
                                
                                var element0 = this.Directors;
                                
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
                                
                                var element0 = this.Directors;
                                
                                foreach (var element1 in element0)
                                
                                {
                                    yield return TypeConverter<T>.ConvertFrom_long(element1);
                                }
                            }
                            
                        }
                        break;
                    
                    case  4:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_List_long(this.Directors);
                            
                        }
                        break;
                    
                    case  5:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_List_long(this.Directors);
                            
                        }
                        break;
                    
                    case  6:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_List_long(this.Directors);
                            
                        }
                        break;
                    
                    default:
                        Throw.incompatible_with_cell();
                        break;
                }
            yield break;
        }
        
        private IEnumerable<T> _enumerate_from_Characters<T>()
        {
            
                switch (TypeConverter<T>.type_id)
                {
                    
                    case  0:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Characters);
                            
                        }
                        break;
                    
                    case  1:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Characters);
                            
                        }
                        break;
                    
                    case  2:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Characters);
                            
                        }
                        break;
                    
                    case  3:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Characters);
                            
                        }
                        break;
                    
                    case  4:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Characters);
                            
                        }
                        break;
                    
                    case  5:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Characters);
                            
                        }
                        break;
                    
                    case  6:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Characters);
                            
                        }
                        break;
                    
                    default:
                        Throw.incompatible_with_cell();
                        break;
                }
            yield break;
        }
        
        private IEnumerable<T> _enumerate_from_Performance<T>()
        {
            
                switch (TypeConverter<T>.type_id)
                {
                    
                    case  0:
                        {
                            
                            {
                                
                                var element0 = this.Performance;
                                
                                foreach (var element1 in element0)
                                
                                {
                                    yield return TypeConverter<T>.ConvertFrom_string(element1);
                                }
                            }
                            
                        }
                        break;
                    
                    case  1:
                        {
                            
                            {
                                
                                var element0 = this.Performance;
                                
                                foreach (var element1 in element0)
                                
                                {
                                    yield return TypeConverter<T>.ConvertFrom_string(element1);
                                }
                            }
                            
                        }
                        break;
                    
                    case  2:
                        {
                            
                            {
                                
                                var element0 = this.Performance;
                                
                                foreach (var element1 in element0)
                                
                                {
                                    yield return TypeConverter<T>.ConvertFrom_string(element1);
                                }
                            }
                            
                        }
                        break;
                    
                    case  3:
                        {
                            
                            {
                                
                                var element0 = this.Performance;
                                
                                foreach (var element1 in element0)
                                
                                {
                                    yield return TypeConverter<T>.ConvertFrom_string(element1);
                                }
                            }
                            
                        }
                        break;
                    
                    case  4:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_List_string(this.Performance);
                            
                        }
                        break;
                    
                    case  5:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_List_string(this.Performance);
                            
                        }
                        break;
                    
                    case  6:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_List_string(this.Performance);
                            
                        }
                        break;
                    
                    default:
                        Throw.incompatible_with_cell();
                        break;
                }
            yield break;
        }
        
        private IEnumerable<T> _enumerate_from_Distributors<T>()
        {
            
                switch (TypeConverter<T>.type_id)
                {
                    
                    case  0:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Distributors);
                            
                        }
                        break;
                    
                    case  1:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Distributors);
                            
                        }
                        break;
                    
                    case  2:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Distributors);
                            
                        }
                        break;
                    
                    case  3:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Distributors);
                            
                        }
                        break;
                    
                    case  4:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Distributors);
                            
                        }
                        break;
                    
                    case  5:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Distributors);
                            
                        }
                        break;
                    
                    case  6:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Distributors);
                            
                        }
                        break;
                    
                    default:
                        Throw.incompatible_with_cell();
                        break;
                }
            yield break;
        }
        
        private IEnumerable<T> _enumerate_from_Channels<T>()
        {
            
                switch (TypeConverter<T>.type_id)
                {
                    
                    case  0:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Channels);
                            
                        }
                        break;
                    
                    case  1:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Channels);
                            
                        }
                        break;
                    
                    case  2:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Channels);
                            
                        }
                        break;
                    
                    case  3:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Channels);
                            
                        }
                        break;
                    
                    case  4:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Channels);
                            
                        }
                        break;
                    
                    case  5:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Channels);
                            
                        }
                        break;
                    
                    case  6:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Channels);
                            
                        }
                        break;
                    
                    default:
                        Throw.incompatible_with_cell();
                        break;
                }
            yield break;
        }
        
        private IEnumerable<T> _enumerate_from_Albums<T>()
        {
            
                switch (TypeConverter<T>.type_id)
                {
                    
                    case  0:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Albums);
                            
                        }
                        break;
                    
                    case  1:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Albums);
                            
                        }
                        break;
                    
                    case  2:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Albums);
                            
                        }
                        break;
                    
                    case  3:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Albums);
                            
                        }
                        break;
                    
                    case  4:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Albums);
                            
                        }
                        break;
                    
                    case  5:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Albums);
                            
                        }
                        break;
                    
                    case  6:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Albums);
                            
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
        
        private IEnumerable<T> _enumerate_from_Alias<T>()
        {
            
                switch (TypeConverter<T>.type_id)
                {
                    
                    case  0:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Alias);
                            
                        }
                        break;
                    
                    case  1:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Alias);
                            
                        }
                        break;
                    
                    case  2:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Alias);
                            
                        }
                        break;
                    
                    case  3:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Alias);
                            
                        }
                        break;
                    
                    case  4:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Alias);
                            
                        }
                        break;
                    
                    case  5:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Alias);
                            
                        }
                        break;
                    
                    case  6:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Alias);
                            
                        }
                        break;
                    
                    default:
                        Throw.incompatible_with_cell();
                        break;
                }
            yield break;
        }
        
        private IEnumerable<T> _enumerate_from_Description<T>()
        {
            
                switch (TypeConverter<T>.type_id)
                {
                    
                    case  0:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Description);
                            
                        }
                        break;
                    
                    case  1:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Description);
                            
                        }
                        break;
                    
                    case  2:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Description);
                            
                        }
                        break;
                    
                    case  3:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Description);
                            
                        }
                        break;
                    
                    case  4:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Description);
                            
                        }
                        break;
                    
                    case  5:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Description);
                            
                        }
                        break;
                    
                    case  6:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Description);
                            
                        }
                        break;
                    
                    default:
                        Throw.incompatible_with_cell();
                        break;
                }
            yield break;
        }
        
        private IEnumerable<T> _enumerate_from_Segments<T>()
        {
            
                switch (TypeConverter<T>.type_id)
                {
                    
                    case  0:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Segments);
                            
                        }
                        break;
                    
                    case  1:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Segments);
                            
                        }
                        break;
                    
                    case  2:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Segments);
                            
                        }
                        break;
                    
                    case  3:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Segments);
                            
                        }
                        break;
                    
                    case  4:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Segments);
                            
                        }
                        break;
                    
                    case  5:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Segments);
                            
                        }
                        break;
                    
                    case  6:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Segments);
                            
                        }
                        break;
                    
                    default:
                        Throw.incompatible_with_cell();
                        break;
                }
            yield break;
        }
        
        private IEnumerable<T> _enumerate_from_Categories<T>()
        {
            
                switch (TypeConverter<T>.type_id)
                {
                    
                    case  0:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Categories);
                            
                        }
                        break;
                    
                    case  1:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Categories);
                            
                        }
                        break;
                    
                    case  2:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Categories);
                            
                        }
                        break;
                    
                    case  3:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Categories);
                            
                        }
                        break;
                    
                    case  4:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Categories);
                            
                        }
                        break;
                    
                    case  5:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Categories);
                            
                        }
                        break;
                    
                    case  6:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Categories);
                            
                        }
                        break;
                    
                    default:
                        Throw.incompatible_with_cell();
                        break;
                }
            yield break;
        }
        
        private IEnumerable<T> _enumerate_from_IntEmbeddedFilters<T>()
        {
            
                switch (TypeConverter<T>.type_id)
                {
                    
                    case  0:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.IntEmbeddedFilters);
                            
                        }
                        break;
                    
                    case  1:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.IntEmbeddedFilters);
                            
                        }
                        break;
                    
                    case  2:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.IntEmbeddedFilters);
                            
                        }
                        break;
                    
                    case  3:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.IntEmbeddedFilters);
                            
                        }
                        break;
                    
                    case  4:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.IntEmbeddedFilters);
                            
                        }
                        break;
                    
                    case  5:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.IntEmbeddedFilters);
                            
                        }
                        break;
                    
                    case  6:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.IntEmbeddedFilters);
                            
                        }
                        break;
                    
                    default:
                        Throw.incompatible_with_cell();
                        break;
                }
            yield break;
        }
        
        private IEnumerable<T> _enumerate_from_NumberOfWantToWatch<T>()
        {
            
                switch (TypeConverter<T>.type_id)
                {
                    
                    case  0:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.NumberOfWantToWatch);
                            
                        }
                        break;
                    
                    case  1:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.NumberOfWantToWatch);
                            
                        }
                        break;
                    
                    case  2:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.NumberOfWantToWatch);
                            
                        }
                        break;
                    
                    case  3:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.NumberOfWantToWatch);
                            
                        }
                        break;
                    
                    case  4:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.NumberOfWantToWatch);
                            
                        }
                        break;
                    
                    case  5:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.NumberOfWantToWatch);
                            
                        }
                        break;
                    
                    case  6:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.NumberOfWantToWatch);
                            
                        }
                        break;
                    
                    default:
                        Throw.incompatible_with_cell();
                        break;
                }
            yield break;
        }
        
        private IEnumerable<T> _enumerate_from_Rating<T>()
        {
            
                switch (TypeConverter<T>.type_id)
                {
                    
                    case  0:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Rating);
                            
                        }
                        break;
                    
                    case  1:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Rating);
                            
                        }
                        break;
                    
                    case  2:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Rating);
                            
                        }
                        break;
                    
                    case  3:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Rating);
                            
                        }
                        break;
                    
                    case  4:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Rating);
                            
                        }
                        break;
                    
                    case  5:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Rating);
                            
                        }
                        break;
                    
                    case  6:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Rating);
                            
                        }
                        break;
                    
                    default:
                        Throw.incompatible_with_cell();
                        break;
                }
            yield break;
        }
        
        private IEnumerable<T> _enumerate_from_NumberOfShortReview<T>()
        {
            
                switch (TypeConverter<T>.type_id)
                {
                    
                    case  0:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.NumberOfShortReview);
                            
                        }
                        break;
                    
                    case  1:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.NumberOfShortReview);
                            
                        }
                        break;
                    
                    case  2:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.NumberOfShortReview);
                            
                        }
                        break;
                    
                    case  3:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.NumberOfShortReview);
                            
                        }
                        break;
                    
                    case  4:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.NumberOfShortReview);
                            
                        }
                        break;
                    
                    case  5:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.NumberOfShortReview);
                            
                        }
                        break;
                    
                    case  6:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.NumberOfShortReview);
                            
                        }
                        break;
                    
                    default:
                        Throw.incompatible_with_cell();
                        break;
                }
            yield break;
        }
        
        private IEnumerable<T> _enumerate_from_ReviewCount<T>()
        {
            
                switch (TypeConverter<T>.type_id)
                {
                    
                    case  0:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.ReviewCount);
                            
                        }
                        break;
                    
                    case  1:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.ReviewCount);
                            
                        }
                        break;
                    
                    case  2:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.ReviewCount);
                            
                        }
                        break;
                    
                    case  3:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.ReviewCount);
                            
                        }
                        break;
                    
                    case  4:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.ReviewCount);
                            
                        }
                        break;
                    
                    case  5:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.ReviewCount);
                            
                        }
                        break;
                    
                    case  6:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.ReviewCount);
                            
                        }
                        break;
                    
                    default:
                        Throw.incompatible_with_cell();
                        break;
                }
            yield break;
        }
        
        private IEnumerable<T> _enumerate_from_NumberOfWatched<T>()
        {
            
                switch (TypeConverter<T>.type_id)
                {
                    
                    case  0:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.NumberOfWatched);
                            
                        }
                        break;
                    
                    case  1:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.NumberOfWatched);
                            
                        }
                        break;
                    
                    case  2:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.NumberOfWatched);
                            
                        }
                        break;
                    
                    case  3:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.NumberOfWatched);
                            
                        }
                        break;
                    
                    case  4:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.NumberOfWatched);
                            
                        }
                        break;
                    
                    case  5:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.NumberOfWatched);
                            
                        }
                        break;
                    
                    case  6:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.NumberOfWatched);
                            
                        }
                        break;
                    
                    default:
                        Throw.incompatible_with_cell();
                        break;
                }
            yield break;
        }
        
        private IEnumerable<T> _enumerate_from_NumberOfReviewer<T>()
        {
            
                switch (TypeConverter<T>.type_id)
                {
                    
                    case  0:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.NumberOfReviewer);
                            
                        }
                        break;
                    
                    case  1:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.NumberOfReviewer);
                            
                        }
                        break;
                    
                    case  2:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.NumberOfReviewer);
                            
                        }
                        break;
                    
                    case  3:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.NumberOfReviewer);
                            
                        }
                        break;
                    
                    case  4:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.NumberOfReviewer);
                            
                        }
                        break;
                    
                    case  5:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.NumberOfReviewer);
                            
                        }
                        break;
                    
                    case  6:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.NumberOfReviewer);
                            
                        }
                        break;
                    
                    default:
                        Throw.incompatible_with_cell();
                        break;
                }
            yield break;
        }
        
        private IEnumerable<T> _enumerate_from_PublishDate<T>()
        {
            
                switch (TypeConverter<T>.type_id)
                {
                    
                    case  0:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.PublishDate);
                            
                        }
                        break;
                    
                    case  1:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.PublishDate);
                            
                        }
                        break;
                    
                    case  2:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.PublishDate);
                            
                        }
                        break;
                    
                    case  3:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.PublishDate);
                            
                        }
                        break;
                    
                    case  4:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.PublishDate);
                            
                        }
                        break;
                    
                    case  5:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.PublishDate);
                            
                        }
                        break;
                    
                    case  6:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.PublishDate);
                            
                        }
                        break;
                    
                    default:
                        Throw.incompatible_with_cell();
                        break;
                }
            yield break;
        }
        
        private IEnumerable<T> _enumerate_from_Length<T>()
        {
            
                switch (TypeConverter<T>.type_id)
                {
                    
                    case  0:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Length);
                            
                        }
                        break;
                    
                    case  1:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Length);
                            
                        }
                        break;
                    
                    case  2:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Length);
                            
                        }
                        break;
                    
                    case  3:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Length);
                            
                        }
                        break;
                    
                    case  4:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Length);
                            
                        }
                        break;
                    
                    case  5:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Length);
                            
                        }
                        break;
                    
                    case  6:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Length);
                            
                        }
                        break;
                    
                    default:
                        Throw.incompatible_with_cell();
                        break;
                }
            yield break;
        }
        
        private IEnumerable<T> _enumerate_from_Country<T>()
        {
            
                switch (TypeConverter<T>.type_id)
                {
                    
                    case  0:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Country);
                            
                        }
                        break;
                    
                    case  1:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Country);
                            
                        }
                        break;
                    
                    case  2:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Country);
                            
                        }
                        break;
                    
                    case  3:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Country);
                            
                        }
                        break;
                    
                    case  4:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Country);
                            
                        }
                        break;
                    
                    case  5:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Country);
                            
                        }
                        break;
                    
                    case  6:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Country);
                            
                        }
                        break;
                    
                    default:
                        Throw.incompatible_with_cell();
                        break;
                }
            yield break;
        }
        
        private IEnumerable<T> _enumerate_from_Language<T>()
        {
            
                switch (TypeConverter<T>.type_id)
                {
                    
                    case  0:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Language);
                            
                        }
                        break;
                    
                    case  1:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Language);
                            
                        }
                        break;
                    
                    case  2:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Language);
                            
                        }
                        break;
                    
                    case  3:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Language);
                            
                        }
                        break;
                    
                    case  4:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Language);
                            
                        }
                        break;
                    
                    case  5:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Language);
                            
                        }
                        break;
                    
                    case  6:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Language);
                            
                        }
                        break;
                    
                    default:
                        Throw.incompatible_with_cell();
                        break;
                }
            yield break;
        }
        
        private IEnumerable<T> _enumerate_from_SourceUrls<T>()
        {
            
                switch (TypeConverter<T>.type_id)
                {
                    
                    case  0:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.SourceUrls);
                            
                        }
                        break;
                    
                    case  1:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.SourceUrls);
                            
                        }
                        break;
                    
                    case  2:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.SourceUrls);
                            
                        }
                        break;
                    
                    case  3:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.SourceUrls);
                            
                        }
                        break;
                    
                    case  4:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.SourceUrls);
                            
                        }
                        break;
                    
                    case  5:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.SourceUrls);
                            
                        }
                        break;
                    
                    case  6:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.SourceUrls);
                            
                        }
                        break;
                    
                    default:
                        Throw.incompatible_with_cell();
                        break;
                }
            yield break;
        }
        
        private IEnumerable<T> _enumerate_from_ImageUrls<T>()
        {
            
                switch (TypeConverter<T>.type_id)
                {
                    
                    case  0:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.ImageUrls);
                            
                        }
                        break;
                    
                    case  1:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.ImageUrls);
                            
                        }
                        break;
                    
                    case  2:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.ImageUrls);
                            
                        }
                        break;
                    
                    case  3:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.ImageUrls);
                            
                        }
                        break;
                    
                    case  4:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.ImageUrls);
                            
                        }
                        break;
                    
                    case  5:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.ImageUrls);
                            
                        }
                        break;
                    
                    case  6:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.ImageUrls);
                            
                        }
                        break;
                    
                    default:
                        Throw.incompatible_with_cell();
                        break;
                }
            yield break;
        }
        
        private IEnumerable<T> _enumerate_from_OfficialSite<T>()
        {
            
                switch (TypeConverter<T>.type_id)
                {
                    
                    case  0:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.OfficialSite);
                            
                        }
                        break;
                    
                    case  1:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.OfficialSite);
                            
                        }
                        break;
                    
                    case  2:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.OfficialSite);
                            
                        }
                        break;
                    
                    case  3:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.OfficialSite);
                            
                        }
                        break;
                    
                    case  4:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.OfficialSite);
                            
                        }
                        break;
                    
                    case  5:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.OfficialSite);
                            
                        }
                        break;
                    
                    case  6:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.OfficialSite);
                            
                        }
                        break;
                    
                    default:
                        Throw.incompatible_with_cell();
                        break;
                }
            yield break;
        }
        
        private IEnumerable<T> _enumerate_from_EntityContainer<T>()
        {
            
                switch (TypeConverter<T>.type_id)
                {
                    
                    case  0:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.EntityContainer);
                            
                        }
                        break;
                    
                    case  1:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.EntityContainer);
                            
                        }
                        break;
                    
                    case  2:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.EntityContainer);
                            
                        }
                        break;
                    
                    case  3:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.EntityContainer);
                            
                        }
                        break;
                    
                    case  4:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.EntityContainer);
                            
                        }
                        break;
                    
                    case  5:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.EntityContainer);
                            
                        }
                        break;
                    
                    case  6:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.EntityContainer);
                            
                        }
                        break;
                    
                    default:
                        Throw.incompatible_with_cell();
                        break;
                }
            yield break;
        }
        
        private IEnumerable<T> _enumerate_from_Logo<T>()
        {
            
                switch (TypeConverter<T>.type_id)
                {
                    
                    case  0:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Logo);
                            
                        }
                        break;
                    
                    case  1:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Logo);
                            
                        }
                        break;
                    
                    case  2:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Logo);
                            
                        }
                        break;
                    
                    case  3:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Logo);
                            
                        }
                        break;
                    
                    case  4:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Logo);
                            
                        }
                        break;
                    
                    case  5:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Logo);
                            
                        }
                        break;
                    
                    case  6:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.Logo);
                            
                        }
                        break;
                    
                    default:
                        Throw.incompatible_with_cell();
                        break;
                }
            yield break;
        }
        
        private IEnumerable<T> _enumerate_from_QueryRank<T>()
        {
            
                switch (TypeConverter<T>.type_id)
                {
                    
                    case  0:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.QueryRank);
                            
                        }
                        break;
                    
                    case  1:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.QueryRank);
                            
                        }
                        break;
                    
                    case  2:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.QueryRank);
                            
                        }
                        break;
                    
                    case  3:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.QueryRank);
                            
                        }
                        break;
                    
                    case  4:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.QueryRank);
                            
                        }
                        break;
                    
                    case  5:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.QueryRank);
                            
                        }
                        break;
                    
                    case  6:
                        {
                            
                            yield return TypeConverter<T>.ConvertFrom_string(this.QueryRank);
                            
                        }
                        break;
                    
                    default:
                        Throw.incompatible_with_cell();
                        break;
                }
            yield break;
        }
        
        private static StringLookupTable s_field_attribute_id_table = new StringLookupTable(
            
            "Index"
            ,
            "GraphEdge"
            
            );
        #endregion
        public IEnumerable<T> EnumerateField<T>(string fieldName)
        {
            switch (FieldLookupTable.Lookup(fieldName))
            {
                
                case 0:
                    return _enumerate_from_Key<T>();
                
                case 1:
                    return _enumerate_from_KGId<T>();
                
                case 2:
                    return _enumerate_from_Genres<T>();
                
                case 3:
                    return _enumerate_from_Artists<T>();
                
                case 4:
                    return _enumerate_from_Directors<T>();
                
                case 5:
                    return _enumerate_from_Characters<T>();
                
                case 6:
                    return _enumerate_from_Performance<T>();
                
                case 7:
                    return _enumerate_from_Distributors<T>();
                
                case 8:
                    return _enumerate_from_Channels<T>();
                
                case 9:
                    return _enumerate_from_Albums<T>();
                
                case 10:
                    return _enumerate_from_Name<T>();
                
                case 11:
                    return _enumerate_from_Alias<T>();
                
                case 12:
                    return _enumerate_from_Description<T>();
                
                case 13:
                    return _enumerate_from_Segments<T>();
                
                case 14:
                    return _enumerate_from_Categories<T>();
                
                case 15:
                    return _enumerate_from_IntEmbeddedFilters<T>();
                
                case 16:
                    return _enumerate_from_NumberOfWantToWatch<T>();
                
                case 17:
                    return _enumerate_from_Rating<T>();
                
                case 18:
                    return _enumerate_from_NumberOfShortReview<T>();
                
                case 19:
                    return _enumerate_from_ReviewCount<T>();
                
                case 20:
                    return _enumerate_from_NumberOfWatched<T>();
                
                case 21:
                    return _enumerate_from_NumberOfReviewer<T>();
                
                case 22:
                    return _enumerate_from_PublishDate<T>();
                
                case 23:
                    return _enumerate_from_Length<T>();
                
                case 24:
                    return _enumerate_from_Country<T>();
                
                case 25:
                    return _enumerate_from_Language<T>();
                
                case 26:
                    return _enumerate_from_SourceUrls<T>();
                
                case 27:
                    return _enumerate_from_ImageUrls<T>();
                
                case 28:
                    return _enumerate_from_OfficialSite<T>();
                
                case 29:
                    return _enumerate_from_EntityContainer<T>();
                
                case 30:
                    return _enumerate_from_Logo<T>();
                
                case 31:
                    return _enumerate_from_QueryRank<T>();
                
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
                
                foreach (var val in _enumerate_from_Key<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_KGId<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_Genres<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_Artists<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_Directors<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_Characters<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_Performance<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_Distributors<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_Channels<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_Albums<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_Name<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_Alias<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_Description<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_Segments<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_Categories<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_IntEmbeddedFilters<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_NumberOfWantToWatch<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_Rating<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_NumberOfShortReview<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_ReviewCount<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_NumberOfWatched<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_NumberOfReviewer<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_PublishDate<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_Length<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_Country<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_Language<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_SourceUrls<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_ImageUrls<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_OfficialSite<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_EntityContainer<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_Logo<T>())
                    yield return val;
                
                foreach (var val in _enumerate_from_QueryRank<T>())
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
                            
                        }
                        
                        {
                            
                            {
                                if (attributeValue == null || attributeValue == "")
                                {
                                    foreach (var val in _enumerate_from_Genres<T>())
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
                        
                        {
                            
                        }
                        
                        break;
                    
                    case  1:
                        
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
                                    foreach (var val in _enumerate_from_Artists<T>())
                                        yield return val;
                                }
                            }
                            
                        }
                        
                        {
                            
                            {
                                if (attributeValue == null || attributeValue == "")
                                {
                                    foreach (var val in _enumerate_from_Directors<T>())
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
                yield return "Key";
            }
            
            {
                yield return "KGId";
            }
            
            {
                yield return "Genres";
            }
            
            {
                yield return "Artists";
            }
            
            {
                yield return "Directors";
            }
            
            {
                yield return "Characters";
            }
            
            {
                yield return "Performance";
            }
            
            {
                yield return "Distributors";
            }
            
            {
                yield return "Channels";
            }
            
            {
                yield return "Albums";
            }
            
            {
                yield return "Name";
            }
            
            {
                yield return "Alias";
            }
            
            {
                yield return "Description";
            }
            
            {
                yield return "Segments";
            }
            
            {
                yield return "Categories";
            }
            
            {
                yield return "IntEmbeddedFilters";
            }
            
            {
                yield return "NumberOfWantToWatch";
            }
            
            {
                yield return "Rating";
            }
            
            {
                yield return "NumberOfShortReview";
            }
            
            {
                yield return "ReviewCount";
            }
            
            {
                yield return "NumberOfWatched";
            }
            
            {
                yield return "NumberOfReviewer";
            }
            
            {
                yield return "PublishDate";
            }
            
            {
                yield return "Length";
            }
            
            {
                yield return "Country";
            }
            
            {
                yield return "Language";
            }
            
            {
                yield return "SourceUrls";
            }
            
            {
                yield return "ImageUrls";
            }
            
            {
                yield return "OfficialSite";
            }
            
            {
                yield return "EntityContainer";
            }
            
            {
                yield return "Logo";
            }
            
            {
                yield return "QueryRank";
            }
            
        }
        IAttributeCollection ICellDescriptor.GetFieldAttributes(string fieldName)
        {
            return StorageSchema.Movie.GetFieldAttributes(fieldName);
        }
        IEnumerable<IFieldDescriptor> ICellDescriptor.GetFieldDescriptors()
        {
            return StorageSchema.Movie.GetFieldDescriptors();
        }
        string ITypeDescriptor.TypeName
        {
            get { return StorageSchema.s_cellTypeName_Movie; }
        }
        Type ITypeDescriptor.Type
        {
            get { return StorageSchema.s_cellType_Movie; }
        }
        bool ITypeDescriptor.IsOfType<T>()
        {
            return typeof(T) == StorageSchema.s_cellType_Movie;
        }
        bool ITypeDescriptor.IsList()
        {
            return false;
        }
        IReadOnlyDictionary<string, string> IAttributeCollection.Attributes
        {
            get { return StorageSchema.Movie.Attributes; }
        }
        string IAttributeCollection.GetAttributeValue(string attributeKey)
        {
            return StorageSchema.Movie.GetAttributeValue(attributeKey);
        }
        ushort ICellDescriptor.CellType
        {
            get
            {
                return (ushort)CellType.Movie;
            }
        }
        #endregion
    }
    
}

#pragma warning restore 162,168,649,660,661,1522

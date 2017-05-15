#pragma warning disable 162,168,649,660,661,1522

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Trinity;
using Trinity.TSL;
using Trinity.TSL.Lib;
namespace GraphEngineServer
{
    /// <summary>
    /// Provides facilities for serializing data to Json strings.
    /// </summary>
    public class Serializer
    {
        [ThreadStatic]
        static StringBuilder s_stringBuilder;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void s_ensure_string_builder()
        {
            if (s_stringBuilder == null)
                s_stringBuilder = new StringBuilder();
            else
                s_stringBuilder.Clear();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <summary>
        /// Serializes a byte object to Json string.
        /// </summary>
        /// <param name="value">The target object to be serialized.</param>
        /// <returns>The serialized Json string.</returns>
        public static string ToString(byte value)
        {
            s_ensure_string_builder();
            ToString_impl(value, s_stringBuilder, in_json: false);
            return s_stringBuilder.ToString();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <summary>
        /// Serializes a bool object to Json string.
        /// </summary>
        /// <param name="value">The target object to be serialized.</param>
        /// <returns>The serialized Json string.</returns>
        public static string ToString(bool value)
        {
            s_ensure_string_builder();
            ToString_impl(value, s_stringBuilder, in_json: false);
            return s_stringBuilder.ToString();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <summary>
        /// Serializes a int object to Json string.
        /// </summary>
        /// <param name="value">The target object to be serialized.</param>
        /// <returns>The serialized Json string.</returns>
        public static string ToString(int value)
        {
            s_ensure_string_builder();
            ToString_impl(value, s_stringBuilder, in_json: false);
            return s_stringBuilder.ToString();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <summary>
        /// Serializes a long object to Json string.
        /// </summary>
        /// <param name="value">The target object to be serialized.</param>
        /// <returns>The serialized Json string.</returns>
        public static string ToString(long value)
        {
            s_ensure_string_builder();
            ToString_impl(value, s_stringBuilder, in_json: false);
            return s_stringBuilder.ToString();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <summary>
        /// Serializes a string object to Json string.
        /// </summary>
        /// <param name="value">The target object to be serialized.</param>
        /// <returns>The serialized Json string.</returns>
        public static string ToString(string value)
        {
            s_ensure_string_builder();
            ToString_impl(value, s_stringBuilder, in_json: false);
            return s_stringBuilder.ToString();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <summary>
        /// Serializes a List<long> object to Json string.
        /// </summary>
        /// <param name="value">The target object to be serialized.</param>
        /// <returns>The serialized Json string.</returns>
        public static string ToString(List<long> value)
        {
            s_ensure_string_builder();
            ToString_impl(value, s_stringBuilder, in_json: false);
            return s_stringBuilder.ToString();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <summary>
        /// Serializes a List<string> object to Json string.
        /// </summary>
        /// <param name="value">The target object to be serialized.</param>
        /// <returns>The serialized Json string.</returns>
        public static string ToString(List<string> value)
        {
            s_ensure_string_builder();
            ToString_impl(value, s_stringBuilder, in_json: false);
            return s_stringBuilder.ToString();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <summary>
        /// Serializes a __type_injection__ object to Json string.
        /// </summary>
        /// <param name="value">The target object to be serialized.</param>
        /// <returns>The serialized Json string.</returns>
        public static string ToString(__type_injection__ value)
        {
            s_ensure_string_builder();
            ToString_impl(value, s_stringBuilder, in_json: false);
            return s_stringBuilder.ToString();
        }
        
        /// <summary>
        /// Serializes a Movie object to Json string.
        /// </summary>
        /// <param name="value">The target cell object to be serialized.</param>
        /// <returns>The serialized Json string.</returns>
        public static string ToString(Movie cell)
        {
            s_ensure_string_builder();
            s_stringBuilder.Append('{');
            s_stringBuilder.AppendFormat("\"CellID\":{0}", cell.CellID);
            
            {
                
                if (cell.Key != null)
                {
                    
                    s_stringBuilder.Append(',');
                    s_stringBuilder.Append("\"Key\":");
                    ToString_impl(cell.Key, s_stringBuilder, in_json: true);
                    
                }
                
            }
            
            {
                
                if (cell.KGId != null)
                {
                    
                    s_stringBuilder.Append(',');
                    s_stringBuilder.Append("\"KGId\":");
                    ToString_impl(cell.KGId, s_stringBuilder, in_json: true);
                    
                }
                
            }
            
            {
                
                if (cell.Genres != null)
                {
                    
                    s_stringBuilder.Append(',');
                    s_stringBuilder.Append("\"Genres\":");
                    ToString_impl(cell.Genres, s_stringBuilder, in_json: true);
                    
                }
                
            }
            
            {
                
                if (cell.Artists != null)
                {
                    
                    s_stringBuilder.Append(',');
                    s_stringBuilder.Append("\"Artists\":");
                    ToString_impl(cell.Artists, s_stringBuilder, in_json: true);
                    
                }
                
            }
            
            {
                
                if (cell.Directors != null)
                {
                    
                    s_stringBuilder.Append(',');
                    s_stringBuilder.Append("\"Directors\":");
                    ToString_impl(cell.Directors, s_stringBuilder, in_json: true);
                    
                }
                
            }
            
            {
                
                if (cell.Characters != null)
                {
                    
                    s_stringBuilder.Append(',');
                    s_stringBuilder.Append("\"Characters\":");
                    ToString_impl(cell.Characters, s_stringBuilder, in_json: true);
                    
                }
                
            }
            
            {
                
                if (cell.Performance != null)
                {
                    
                    s_stringBuilder.Append(',');
                    s_stringBuilder.Append("\"Performance\":");
                    ToString_impl(cell.Performance, s_stringBuilder, in_json: true);
                    
                }
                
            }
            
            {
                
                if (cell.Distributors != null)
                {
                    
                    s_stringBuilder.Append(',');
                    s_stringBuilder.Append("\"Distributors\":");
                    ToString_impl(cell.Distributors, s_stringBuilder, in_json: true);
                    
                }
                
            }
            
            {
                
                if (cell.Channels != null)
                {
                    
                    s_stringBuilder.Append(',');
                    s_stringBuilder.Append("\"Channels\":");
                    ToString_impl(cell.Channels, s_stringBuilder, in_json: true);
                    
                }
                
            }
            
            {
                
                if (cell.Albums != null)
                {
                    
                    s_stringBuilder.Append(',');
                    s_stringBuilder.Append("\"Albums\":");
                    ToString_impl(cell.Albums, s_stringBuilder, in_json: true);
                    
                }
                
            }
            
            {
                
                if (cell.Name != null)
                {
                    
                    s_stringBuilder.Append(',');
                    s_stringBuilder.Append("\"Name\":");
                    ToString_impl(cell.Name, s_stringBuilder, in_json: true);
                    
                }
                
            }
            
            {
                
                if (cell.Alias != null)
                {
                    
                    s_stringBuilder.Append(',');
                    s_stringBuilder.Append("\"Alias\":");
                    ToString_impl(cell.Alias, s_stringBuilder, in_json: true);
                    
                }
                
            }
            
            {
                
                if (cell.Description != null)
                {
                    
                    s_stringBuilder.Append(',');
                    s_stringBuilder.Append("\"Description\":");
                    ToString_impl(cell.Description, s_stringBuilder, in_json: true);
                    
                }
                
            }
            
            {
                
                if (cell.Segments != null)
                {
                    
                    s_stringBuilder.Append(',');
                    s_stringBuilder.Append("\"Segments\":");
                    ToString_impl(cell.Segments, s_stringBuilder, in_json: true);
                    
                }
                
            }
            
            {
                
                if (cell.Categories != null)
                {
                    
                    s_stringBuilder.Append(',');
                    s_stringBuilder.Append("\"Categories\":");
                    ToString_impl(cell.Categories, s_stringBuilder, in_json: true);
                    
                }
                
            }
            
            {
                
                if (cell.IntEmbeddedFilters != null)
                {
                    
                    s_stringBuilder.Append(',');
                    s_stringBuilder.Append("\"IntEmbeddedFilters\":");
                    ToString_impl(cell.IntEmbeddedFilters, s_stringBuilder, in_json: true);
                    
                }
                
            }
            
            {
                
                if (cell.NumberOfWantToWatch != null)
                {
                    
                    s_stringBuilder.Append(',');
                    s_stringBuilder.Append("\"NumberOfWantToWatch\":");
                    ToString_impl(cell.NumberOfWantToWatch, s_stringBuilder, in_json: true);
                    
                }
                
            }
            
            {
                
                if (cell.Rating != null)
                {
                    
                    s_stringBuilder.Append(',');
                    s_stringBuilder.Append("\"Rating\":");
                    ToString_impl(cell.Rating, s_stringBuilder, in_json: true);
                    
                }
                
            }
            
            {
                
                if (cell.NumberOfShortReview != null)
                {
                    
                    s_stringBuilder.Append(',');
                    s_stringBuilder.Append("\"NumberOfShortReview\":");
                    ToString_impl(cell.NumberOfShortReview, s_stringBuilder, in_json: true);
                    
                }
                
            }
            
            {
                
                if (cell.ReviewCount != null)
                {
                    
                    s_stringBuilder.Append(',');
                    s_stringBuilder.Append("\"ReviewCount\":");
                    ToString_impl(cell.ReviewCount, s_stringBuilder, in_json: true);
                    
                }
                
            }
            
            {
                
                if (cell.NumberOfWatched != null)
                {
                    
                    s_stringBuilder.Append(',');
                    s_stringBuilder.Append("\"NumberOfWatched\":");
                    ToString_impl(cell.NumberOfWatched, s_stringBuilder, in_json: true);
                    
                }
                
            }
            
            {
                
                if (cell.NumberOfReviewer != null)
                {
                    
                    s_stringBuilder.Append(',');
                    s_stringBuilder.Append("\"NumberOfReviewer\":");
                    ToString_impl(cell.NumberOfReviewer, s_stringBuilder, in_json: true);
                    
                }
                
            }
            
            {
                
                if (cell.PublishDate != null)
                {
                    
                    s_stringBuilder.Append(',');
                    s_stringBuilder.Append("\"PublishDate\":");
                    ToString_impl(cell.PublishDate, s_stringBuilder, in_json: true);
                    
                }
                
            }
            
            {
                
                if (cell.Length != null)
                {
                    
                    s_stringBuilder.Append(',');
                    s_stringBuilder.Append("\"Length\":");
                    ToString_impl(cell.Length, s_stringBuilder, in_json: true);
                    
                }
                
            }
            
            {
                
                if (cell.Country != null)
                {
                    
                    s_stringBuilder.Append(',');
                    s_stringBuilder.Append("\"Country\":");
                    ToString_impl(cell.Country, s_stringBuilder, in_json: true);
                    
                }
                
            }
            
            {
                
                if (cell.Language != null)
                {
                    
                    s_stringBuilder.Append(',');
                    s_stringBuilder.Append("\"Language\":");
                    ToString_impl(cell.Language, s_stringBuilder, in_json: true);
                    
                }
                
            }
            
            {
                
                if (cell.SourceUrls != null)
                {
                    
                    s_stringBuilder.Append(',');
                    s_stringBuilder.Append("\"SourceUrls\":");
                    ToString_impl(cell.SourceUrls, s_stringBuilder, in_json: true);
                    
                }
                
            }
            
            {
                
                if (cell.ImageUrls != null)
                {
                    
                    s_stringBuilder.Append(',');
                    s_stringBuilder.Append("\"ImageUrls\":");
                    ToString_impl(cell.ImageUrls, s_stringBuilder, in_json: true);
                    
                }
                
            }
            
            {
                
                if (cell.OfficialSite != null)
                {
                    
                    s_stringBuilder.Append(',');
                    s_stringBuilder.Append("\"OfficialSite\":");
                    ToString_impl(cell.OfficialSite, s_stringBuilder, in_json: true);
                    
                }
                
            }
            
            {
                
                if (cell.EntityContainer != null)
                {
                    
                    s_stringBuilder.Append(',');
                    s_stringBuilder.Append("\"EntityContainer\":");
                    ToString_impl(cell.EntityContainer, s_stringBuilder, in_json: true);
                    
                }
                
            }
            
            {
                
                if (cell.Logo != null)
                {
                    
                    s_stringBuilder.Append(',');
                    s_stringBuilder.Append("\"Logo\":");
                    ToString_impl(cell.Logo, s_stringBuilder, in_json: true);
                    
                }
                
            }
            
            {
                
                if (cell.QueryRank != null)
                {
                    
                    s_stringBuilder.Append(',');
                    s_stringBuilder.Append("\"QueryRank\":");
                    ToString_impl(cell.QueryRank, s_stringBuilder, in_json: true);
                    
                }
                
            }
            
            s_stringBuilder.Append('}');
            return s_stringBuilder.ToString();
        }
        
        /// <summary>
        /// Serializes a Person object to Json string.
        /// </summary>
        /// <param name="value">The target cell object to be serialized.</param>
        /// <returns>The serialized Json string.</returns>
        public static string ToString(Person cell)
        {
            s_ensure_string_builder();
            s_stringBuilder.Append('{');
            s_stringBuilder.AppendFormat("\"CellID\":{0}", cell.CellID);
            
            {
                
                    s_stringBuilder.Append(',');
                    s_stringBuilder.Append("\"age\":");
                    ToString_impl(cell.age, s_stringBuilder, in_json: true);
                    
            }
            
            {
                
                    s_stringBuilder.Append(',');
                    s_stringBuilder.Append("\"parent\":");
                    ToString_impl(cell.parent, s_stringBuilder, in_json: true);
                    
            }
            
            {
                
                if (cell.Name != null)
                {
                    
                    s_stringBuilder.Append(',');
                    s_stringBuilder.Append("\"Name\":");
                    ToString_impl(cell.Name, s_stringBuilder, in_json: true);
                    
                }
                
            }
            
            {
                
                    s_stringBuilder.Append(',');
                    s_stringBuilder.Append("\"Gender\":");
                    ToString_impl(cell.Gender, s_stringBuilder, in_json: true);
                    
            }
            
            {
                
                    s_stringBuilder.Append(',');
                    s_stringBuilder.Append("\"Married\":");
                    ToString_impl(cell.Married, s_stringBuilder, in_json: true);
                    
            }
            
            {
                
                    s_stringBuilder.Append(',');
                    s_stringBuilder.Append("\"Spouse\":");
                    ToString_impl(cell.Spouse, s_stringBuilder, in_json: true);
                    
            }
            
            {
                
                if (cell.Act != null)
                {
                    
                    s_stringBuilder.Append(',');
                    s_stringBuilder.Append("\"Act\":");
                    ToString_impl(cell.Act, s_stringBuilder, in_json: true);
                    
                }
                
            }
            
            {
                
                if (cell.Direct != null)
                {
                    
                    s_stringBuilder.Append(',');
                    s_stringBuilder.Append("\"Direct\":");
                    ToString_impl(cell.Direct, s_stringBuilder, in_json: true);
                    
                }
                
            }
            
            s_stringBuilder.Append('}');
            return s_stringBuilder.ToString();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ToString_impl(byte value, StringBuilder str_builder, bool in_json)
        {
            
            {
                
                {
                    str_builder.Append(value);
                }
                
            }
            
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ToString_impl(bool value, StringBuilder str_builder, bool in_json)
        {
            
            {
                
                {
                    str_builder.Append(value.ToString().ToLowerInvariant());
                }
                
            }
            
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ToString_impl(int value, StringBuilder str_builder, bool in_json)
        {
            
            {
                
                {
                    str_builder.Append(value);
                }
                
            }
            
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ToString_impl(long value, StringBuilder str_builder, bool in_json)
        {
            
            {
                
                {
                    str_builder.Append(value);
                }
                
            }
            
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ToString_impl(string value, StringBuilder str_builder, bool in_json)
        {
            
            if (in_json)
            {
                str_builder.Append(JsonStringProcessor.escape(value));
            }
            else
            {
                str_builder.Append(value);
            }
            
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ToString_impl(List<long> value, StringBuilder str_builder, bool in_json)
        {
            
            {
                str_builder.Append('[');
                bool first = true;
                foreach (var element in value)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        str_builder.Append(',');
                    }
                    ToString_impl(element, str_builder, in_json:true);
                }
                str_builder.Append(']');
            }
            
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ToString_impl(List<string> value, StringBuilder str_builder, bool in_json)
        {
            
            {
                str_builder.Append('[');
                bool first = true;
                foreach (var element in value)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        str_builder.Append(',');
                    }
                    ToString_impl(element, str_builder, in_json:true);
                }
                str_builder.Append(']');
            }
            
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ToString_impl(__type_injection__ value, StringBuilder str_builder, bool in_json)
        {
            
            {
                
                str_builder.Append('{');
                bool first_field = true;
                
                {
                    
                        if(first_field)
                            first_field = false;
                        else
                            str_builder.Append(',');
                        str_builder.Append("\"f1\":");
                        
                        ToString_impl(value.f1, str_builder, in_json: true);
                        
                }
                
                {
                    
                    if (value.f2 != null)
                    
                    {
                        
                        if(first_field)
                            first_field = false;
                        else
                            str_builder.Append(',');
                        str_builder.Append("\"f2\":");
                        
                        ToString_impl(value.f2, str_builder, in_json: true);
                        
                    }
                    
                }
                
                str_builder.Append('}');
            }
            
        }
        
        #region mute
        
        #endregion
    }
}

#pragma warning restore 162,168,649,660,661,1522

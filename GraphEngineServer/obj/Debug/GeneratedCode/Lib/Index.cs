#pragma warning disable 162,168,649,660,661,1522

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity;
using Trinity.TSL;
using Trinity.TSL.Lib;
using Trinity.Storage;
using Trinity.Daemon;
using Trinity.Diagnostics;

using GraphEngineServer.InvertedIndex;

using GraphEngineServer.Linq;
namespace GraphEngineServer
{
    /// <summary>
    /// Provides indexing capabilities on <see cref="Trinity.Storage.LocalMemoryStorage"/>.
    /// The target field to query on is specified with <paramref name="query"/>, <seealso cref="GraphEngineServer.IndexIdentifier"/>.
    /// </summary>
    public class Index
    {
        static Index()
        {
            BackgroundIndexUpdater();
            BackgroundThread.AddBackgroundTask(new BackgroundTask(BackgroundIndexUpdater, s_BackgroundIndexUpdateInterval));
        }
        static          readonly    int                         s_BackgroundIndexUpdateInterval      = 60*1000*10;
        static          readonly    object                      s_IndexLock                          = new object();
        static internal readonly    Dictionary<string,string>   s_AccessorSubstringIndexAccessMethod = new Dictionary<string, string>
        {
            
            { "Movie.Genres", "ElementContainsSubstring"}
            ,
            { "Movie.Name", "Contains"}
            ,
            { "Person.Name", "Contains"}
            
        };
        static internal readonly    Dictionary<string,SubstringQueryDelegate>
                                                                s_AccessorSubstringQueryMethodTable = 
            new Dictionary<string, SubstringQueryDelegate>
        {
            
            { "Movie.Genres", Movie_Genres_SubstringQuery }
            ,
            { "Movie.Name", Movie_Name_SubstringQuery }
            ,
            { "Person.Name", Person_Name_SubstringQuery }
            
        };
        static internal readonly    Dictionary<string,SubstringWildcardQueryDelegate>
                                                                s_AccessorSubstringWildcardQueryMethodTable = 
            new Dictionary<string, SubstringWildcardQueryDelegate>
        {
            
            { "Movie.Genres", Movie_Genres_SubstringQuery }
            ,
            { "Movie.Name", Movie_Name_SubstringQuery }
            ,
            { "Person.Name", Person_Name_SubstringQuery }
            
        };
        static internal readonly    Dictionary<string,string>
                                                                s_CellSubstringIndexAccessMethod = 
            new Dictionary<string, string>
        {
            
            { "Movie.Genres", "ElementContainsSubstring"}
            ,
            { "Movie.Name", "Contains"}
            ,
            { "Person.Name", "Contains"}
            
        };
        static internal readonly    Dictionary<string,SubstringQueryDelegate>
                                                                s_CellSubstringQueryMethodTable = 
            new Dictionary<string, SubstringQueryDelegate>
        {
            
            { "Movie.Genres", Movie_Genres_SubstringQuery }
            ,
            { "Movie.Name", Movie_Name_SubstringQuery }
            ,
            { "Person.Name", Person_Name_SubstringQuery }
            
        };
        static internal readonly    Dictionary<string,SubstringWildcardQueryDelegate>
                                                                s_CellSubstringWildcardQueryMethodTable = 
            new Dictionary<string, SubstringWildcardQueryDelegate>
        {
            
            { "Movie.Genres", Movie_Genres_SubstringQuery }
            ,
            { "Movie.Name", Movie_Name_SubstringQuery }
            ,
            { "Person.Name", Person_Name_SubstringQuery }
            
        };
        
        /// <summary>
        /// Performs a substring search.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public static List<long> Movie_Genres_SubstringQuery(string value)
        {
            InvertedBigramSearcher searcher;
            lock (s_IndexLock)
            {
                searcher = Movie_Genres_substring_searcher;
            }
            if (searcher == null)
            {
                Log.WriteLine(LogLevel.Warning, "Index: Substring index for Movie_Genres is not yet ready. Returning an empty query result set.");
                return new List<long>();
            }
            return Movie_Genres_substring_searcher.SubstringSearch(value);
        }
        /// <summary>
        /// Performs a substring search for a list of keywords. The match pattern is:
        /// keywords[0]*keywords[1]..., where * is the wildcard symbol.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static List<long> Movie_Genres_SubstringQuery(List<string> values)
        {
            InvertedBigramSearcher searcher;
            lock (s_IndexLock)
            {
                searcher = Movie_Genres_substring_searcher;
            }
            if (searcher == null)
            {
                Log.WriteLine(LogLevel.Warning, "Index: Substring index for Movie_Genres is not yet ready. Returning an empty query result set.");
                return new List<long>();
            }
            return Movie_Genres_substring_searcher.SubstringSearch(values);
        }
        /// <summary>
        /// Performs a substring search using the specified keywords. The match pattern is:
        /// keywords[0]*keywords[1]..., where * is the wildcard symbol.
        /// </summary>
        /// <param name="keywords">A list of keywords.</param>
        /// <returns>A list of matched cell Ids.</returns>
        public static List<long> Movie_Genres_SubstringQuery(params string[] values)
        {
            InvertedBigramSearcher searcher;
            lock (s_IndexLock)
            {
                searcher = Movie_Genres_substring_searcher;
            }
            if (searcher == null)
            {
                Log.WriteLine(LogLevel.Warning, "Index: Substring index for Movie_Genres is not yet ready. Returning an empty query result set.");
                return new List<long>();
            }
            return Movie_Genres_substring_searcher.SubstringSearch(values);
        }
        
        /// <summary>
        /// Performs a substring search.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public static List<long> Movie_Name_SubstringQuery(string value)
        {
            InvertedBigramSearcher searcher;
            lock (s_IndexLock)
            {
                searcher = Movie_Name_substring_searcher;
            }
            if (searcher == null)
            {
                Log.WriteLine(LogLevel.Warning, "Index: Substring index for Movie_Name is not yet ready. Returning an empty query result set.");
                return new List<long>();
            }
            return Movie_Name_substring_searcher.SubstringSearch(value);
        }
        /// <summary>
        /// Performs a substring search for a list of keywords. The match pattern is:
        /// keywords[0]*keywords[1]..., where * is the wildcard symbol.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static List<long> Movie_Name_SubstringQuery(List<string> values)
        {
            InvertedBigramSearcher searcher;
            lock (s_IndexLock)
            {
                searcher = Movie_Name_substring_searcher;
            }
            if (searcher == null)
            {
                Log.WriteLine(LogLevel.Warning, "Index: Substring index for Movie_Name is not yet ready. Returning an empty query result set.");
                return new List<long>();
            }
            return Movie_Name_substring_searcher.SubstringSearch(values);
        }
        /// <summary>
        /// Performs a substring search using the specified keywords. The match pattern is:
        /// keywords[0]*keywords[1]..., where * is the wildcard symbol.
        /// </summary>
        /// <param name="keywords">A list of keywords.</param>
        /// <returns>A list of matched cell Ids.</returns>
        public static List<long> Movie_Name_SubstringQuery(params string[] values)
        {
            InvertedBigramSearcher searcher;
            lock (s_IndexLock)
            {
                searcher = Movie_Name_substring_searcher;
            }
            if (searcher == null)
            {
                Log.WriteLine(LogLevel.Warning, "Index: Substring index for Movie_Name is not yet ready. Returning an empty query result set.");
                return new List<long>();
            }
            return Movie_Name_substring_searcher.SubstringSearch(values);
        }
        
        /// <summary>
        /// Performs a substring search.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public static List<long> Person_Name_SubstringQuery(string value)
        {
            InvertedBigramSearcher searcher;
            lock (s_IndexLock)
            {
                searcher = Person_Name_substring_searcher;
            }
            if (searcher == null)
            {
                Log.WriteLine(LogLevel.Warning, "Index: Substring index for Person_Name is not yet ready. Returning an empty query result set.");
                return new List<long>();
            }
            return Person_Name_substring_searcher.SubstringSearch(value);
        }
        /// <summary>
        /// Performs a substring search for a list of keywords. The match pattern is:
        /// keywords[0]*keywords[1]..., where * is the wildcard symbol.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static List<long> Person_Name_SubstringQuery(List<string> values)
        {
            InvertedBigramSearcher searcher;
            lock (s_IndexLock)
            {
                searcher = Person_Name_substring_searcher;
            }
            if (searcher == null)
            {
                Log.WriteLine(LogLevel.Warning, "Index: Substring index for Person_Name is not yet ready. Returning an empty query result set.");
                return new List<long>();
            }
            return Person_Name_substring_searcher.SubstringSearch(values);
        }
        /// <summary>
        /// Performs a substring search using the specified keywords. The match pattern is:
        /// keywords[0]*keywords[1]..., where * is the wildcard symbol.
        /// </summary>
        /// <param name="keywords">A list of keywords.</param>
        /// <returns>A list of matched cell Ids.</returns>
        public static List<long> Person_Name_SubstringQuery(params string[] values)
        {
            InvertedBigramSearcher searcher;
            lock (s_IndexLock)
            {
                searcher = Person_Name_substring_searcher;
            }
            if (searcher == null)
            {
                Log.WriteLine(LogLevel.Warning, "Index: Substring index for Person_Name is not yet ready. Returning an empty query result set.");
                return new List<long>();
            }
            return Person_Name_substring_searcher.SubstringSearch(values);
        }
        
        static int BackgroundIndexUpdater()
        {
            
            {
                UpdateSubstringQueryIndex(Movie.Genres);
            }
            
            {
                UpdateSubstringQueryIndex(Movie.Name);
            }
            
            {
                UpdateSubstringQueryIndex(Person.Name);
            }
            
            return s_BackgroundIndexUpdateInterval;
        }
        #region Fields
        
        internal static object                  Movie_Genres_substring_index_lock = new object();
        internal static InvertedBigramIndexer   Movie_Genres_substring_index;
        internal static InvertedBigramSearcher  Movie_Genres_substring_searcher;
        internal static ulong                   Movie_Genres_substring_index_version = 0;
        
        internal static object                  Movie_Name_substring_index_lock = new object();
        internal static InvertedBigramIndexer   Movie_Name_substring_index;
        internal static InvertedBigramSearcher  Movie_Name_substring_searcher;
        internal static ulong                   Movie_Name_substring_index_version = 0;
        
        internal static object                  Person_Name_substring_index_lock = new object();
        internal static InvertedBigramIndexer   Person_Name_substring_index;
        internal static InvertedBigramSearcher  Person_Name_substring_searcher;
        internal static ulong                   Person_Name_substring_index_version = 0;
        
        #endregion
        #region Index interfaces
        internal static List<long> SubstringQuery(string index_id, string query)
        {
            SubstringQueryDelegate query_method;
            if (!s_AccessorSubstringQueryMethodTable.TryGetValue(index_id, out query_method))
                throw new Exception("Unrecognized index id.");
            return query_method(query);
        }
        
        /// <summary>
        /// Performs a substring query on <see cref="Trinity.Global.LocalStorage"/>.
        /// The target field to query on is specified with <paramref name="query"/>, <seealso cref="GraphEngineServer.IndexIdentifier"/>.
        /// </summary>
        /// <param name="index_id">The identifier of the field that the query should be performed on.</param>
        /// <param name="query">The query string.</param>
        /// <returns>
        /// A list of cell ids, of which the given query string is a substring of the field, or a substring of
        /// an element if the target field is a container of strings.
        /// </returns>
        public static List<long> SubstringQuery(IndexIdentifier index_id, string query)
        {
            switch (index_id.id)
            {
                
                case 0:
                    {
                        return Movie_Genres_SubstringQuery(query);
                    }
                
                case 1:
                    {
                        return Movie_Name_SubstringQuery(query);
                    }
                
                case 2:
                    {
                        return Person_Name_SubstringQuery(query);
                    }
                
                default:
                    throw new Exception("The target field does not have a substring index.");
            }
        }
        /// <summary>
        /// Performs a substring sequence query on <see cref="Trinity.Global.LocalStorage"/>. The semantics of
        /// this type of query is that, given a sequence of query strings <c>q_1, q_2,...,q_n</c> in 
        /// <paramref name="query"/>, the match condition on a target string is that all the strings in the sequence
        /// are the substrings of the target string, and the order of the substring occurances should correspond
        /// strictly to that given by <paramref name="query"/>. For example (let <c>S</c> denote the target string):
        /// <example>
        /// <list type="bullet">
        /// <item>
        /// <c>S=..q1....q2..q3{and so on}...qn...</c> is a match.
        /// </item>
        /// <item>
        /// <c>S=.....q1q2..q4{and there are missing substrings in the sequence}...qn...</c> is not match.
        /// </item>
        /// <item>
        /// <c>S=..q5..q3.q1{all the substrings in the sequence are present, but never in the corresponding order}...qn...</c> is not match.
        /// </item>
        /// </list>
        /// </example>
        /// The target field to query on is specified with <paramref name="query"/>, <seealso cref="GraphEngineServer.IndexIdentifier"/>.
        /// </summary>
        /// <param name="index_id">The identifier of the field that the query should be performed on.</param>
        /// <param name="query">The sequence of query strings.</param>
        /// <returns>
        /// A list of cell ids, of which the given query string sequence is matched on the target field, or
        /// an element it if it is a container of strings.
        /// </returns>
        public static List<long> SubstringQuery(IndexIdentifier index_id, List<string> query)
        {
            switch (index_id.id)
            {
                
                case 0:
                    {
                        return Movie_Genres_SubstringQuery(query);
                    }
                
                case 1:
                    {
                        return Movie_Name_SubstringQuery(query);
                    }
                
                case 2:
                    {
                        return Person_Name_SubstringQuery(query);
                    }
                
                default:
                    throw new Exception("The target field does not have a substring index.");
            }
        }
        /// <summary>
        /// Performs a substring sequence query on <see cref="Trinity.Global.LocalStorage"/>. The semantics of
        /// this type of query is that, given a sequence of query strings <c>q_1, q_2,...,q_n</c> in 
        /// <paramref name="query"/>, the match condition on a target string is that all the strings in the sequence
        /// are the substrings of the target string, and the order of the substring occurances should correspond
        /// strictly to that given by <paramref name="query"/>. For example (let <c>S</c> denote the target string):
        /// <example>
        /// <list type="bullet">
        /// <item>
        /// <c>S=..q1....q2..q3{and so on}...qn...</c> is a match.
        /// </item>
        /// <item>
        /// <c>S=.....q1q2..q4{and there are missing substrings in the sequence}...qn...</c> is not match.
        /// </item>
        /// <item>
        /// <c>S=..q5..q3.q1{all the substrings in the sequence are present, but never in the corresponding order}...qn...</c> is not match.
        /// </item>
        /// </list>
        /// </example>
        /// The target field to query on is specified with <paramref name="query"/>, <seealso cref="GraphEngineServer.IndexIdentifier"/>.
        /// </summary>
        /// <param name="index_id">The identifier of the field that the query should be performed on.</param>
        /// <param name="query">The sequence of query strings.</param>
        /// <returns>
        /// A list of cell ids, of which the given query string sequence is matched on the target field, or
        /// an element it if it is a container of strings.
        /// </returns>
        public static List<long> SubstringQuery(IndexIdentifier index_id, params string[] query)
        {
            switch (index_id.id)
            {
                
                case 0:
                    {
                        return Movie_Genres_SubstringQuery(query);
                    }
                
                case 1:
                    {
                        return Movie_Name_SubstringQuery(query);
                    }
                
                case 2:
                    {
                        return Person_Name_SubstringQuery(query);
                    }
                
                default:
                    throw new Exception("The target field does not have a substring index.");
            }
        }
        
        /// <summary>
        /// Updates the index on the given field.
        /// </summary>
        /// <param name="index_id">The identifier of the field whose index should be rebuilt.</param>
        public static void UpdateSubstringQueryIndex(IndexIdentifier index_id)
        {
            switch (index_id.id)
            {
                
                case 0:
                    {
                        Log.WriteLine(LogLevel.Info, "Index: updating substring index of Movie_Genres");
                        System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
                        InvertedBigramIndexer  new_index;
                        InvertedBigramSearcher new_searcher;
                        ulong                  old_version = Movie_Genres_substring_index_version;
                        /**
                         *  Lock the original index to prevent multiple updates
                         *  to be executed simultaneously.
                         */
                        lock (Movie_Genres_substring_index_lock)
                        {
                            /*
                             * If the index version has changed, it means that someone else
                             * has updated the index while we're blocked on the old index object.
                             */
                            if (Movie_Genres_substring_index_version != old_version)
                            {
                                Log.WriteLine(LogLevel.Info, "Index: Substring index of Movie_Genres is already updated, stopping current action.");
                                break;
                            }
                            new_index = new InvertedBigramIndexer("Movie_Genres");
                            foreach (var accessor in Global.LocalStorage.Movie_Accessor_Selector())
                            {
                                bool optional_not_exist = true;
                                
                                {
                                    
                                    foreach (var element_0 in accessor.Genres)
                                    
                                    {
                                        new_index.AddItem((string)element_0, accessor.CellID.Value);
                                    }
                                    
                                }
                                
                            }
                            new_index.BuildIndex();
                            new_searcher = new InvertedBigramSearcher(true, "Movie_Genres");
                            ++Movie_Genres_substring_index_version;
                        }
                        /*  Update the index objects now.  */
                        lock (s_IndexLock)
                        {
                            Movie_Genres_substring_index    = new_index;
                            Movie_Genres_substring_searcher = new_searcher;
                        }
                        sw.Stop();
                        Log.WriteLine(LogLevel.Info, "Index: Finished updating the substring index of Movie_Genres. Time = {0}ms.", sw.ElapsedMilliseconds);
                        break;
                    }
                
                case 1:
                    {
                        Log.WriteLine(LogLevel.Info, "Index: updating substring index of Movie_Name");
                        System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
                        InvertedBigramIndexer  new_index;
                        InvertedBigramSearcher new_searcher;
                        ulong                  old_version = Movie_Name_substring_index_version;
                        /**
                         *  Lock the original index to prevent multiple updates
                         *  to be executed simultaneously.
                         */
                        lock (Movie_Name_substring_index_lock)
                        {
                            /*
                             * If the index version has changed, it means that someone else
                             * has updated the index while we're blocked on the old index object.
                             */
                            if (Movie_Name_substring_index_version != old_version)
                            {
                                Log.WriteLine(LogLevel.Info, "Index: Substring index of Movie_Name is already updated, stopping current action.");
                                break;
                            }
                            new_index = new InvertedBigramIndexer("Movie_Name");
                            foreach (var accessor in Global.LocalStorage.Movie_Accessor_Selector())
                            {
                                bool optional_not_exist = true;
                                
                                {
                                    new_index.AddItem(accessor.Name, accessor.CellID.Value);
                                }
                                
                            }
                            new_index.BuildIndex();
                            new_searcher = new InvertedBigramSearcher(true, "Movie_Name");
                            ++Movie_Name_substring_index_version;
                        }
                        /*  Update the index objects now.  */
                        lock (s_IndexLock)
                        {
                            Movie_Name_substring_index    = new_index;
                            Movie_Name_substring_searcher = new_searcher;
                        }
                        sw.Stop();
                        Log.WriteLine(LogLevel.Info, "Index: Finished updating the substring index of Movie_Name. Time = {0}ms.", sw.ElapsedMilliseconds);
                        break;
                    }
                
                case 2:
                    {
                        Log.WriteLine(LogLevel.Info, "Index: updating substring index of Person_Name");
                        System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
                        InvertedBigramIndexer  new_index;
                        InvertedBigramSearcher new_searcher;
                        ulong                  old_version = Person_Name_substring_index_version;
                        /**
                         *  Lock the original index to prevent multiple updates
                         *  to be executed simultaneously.
                         */
                        lock (Person_Name_substring_index_lock)
                        {
                            /*
                             * If the index version has changed, it means that someone else
                             * has updated the index while we're blocked on the old index object.
                             */
                            if (Person_Name_substring_index_version != old_version)
                            {
                                Log.WriteLine(LogLevel.Info, "Index: Substring index of Person_Name is already updated, stopping current action.");
                                break;
                            }
                            new_index = new InvertedBigramIndexer("Person_Name");
                            foreach (var accessor in Global.LocalStorage.Person_Accessor_Selector())
                            {
                                bool optional_not_exist = true;
                                
                                {
                                    new_index.AddItem(accessor.Name, accessor.CellID.Value);
                                }
                                
                            }
                            new_index.BuildIndex();
                            new_searcher = new InvertedBigramSearcher(true, "Person_Name");
                            ++Person_Name_substring_index_version;
                        }
                        /*  Update the index objects now.  */
                        lock (s_IndexLock)
                        {
                            Person_Name_substring_index    = new_index;
                            Person_Name_substring_searcher = new_searcher;
                        }
                        sw.Stop();
                        Log.WriteLine(LogLevel.Info, "Index: Finished updating the substring index of Person_Name. Time = {0}ms.", sw.ElapsedMilliseconds);
                        break;
                    }
                
                default:
                    throw new Exception("The target field does not have a substring index.");
            }
        }
        #endregion
        #region Index identifiers
        
        /// <summary>
        /// The index identifier representing <see cref="GraphEngineServer.Person"/>.
        /// </summary>
        public class Person : IndexIdentifier
        {
            
            /// <summary>
            /// The index identifier representing <see cref="GraphEngineServer.Person.Name"/>.
            /// </summary>
            public static readonly IndexIdentifier Name = 2;
            
        }
        
        /// <summary>
        /// The index identifier representing <see cref="GraphEngineServer.Movie"/>.
        /// </summary>
        public class Movie : IndexIdentifier
        {
            
            /// <summary>
            /// The index identifier representing <see cref="GraphEngineServer.Movie.Genres"/>.
            /// </summary>
            public static readonly IndexIdentifier Genres = 0;
            
            /// <summary>
            /// The index identifier representing <see cref="GraphEngineServer.Movie.Name"/>.
            /// </summary>
            public static readonly IndexIdentifier Name = 1;
            
        }
        
        /// <summary>
        /// The base class of index identifiers. When performing index queries, an index identifier should be provided
        /// to the query interface. All the indexed fields defined in the TSL have a corresponding static index identifier
        /// instance, accessible through GraphEngineServer.Index.Target_Cell_Name.Target_Field_Name.
        /// </summary>
        public class IndexIdentifier 
        {
            /// <summary>
            /// For internal use only.
            /// </summary>
            /// <param name="value">An 32-bit unsigned integer that is assigned to the target index identifier. </param>
            /// <returns></returns>
            public static implicit operator IndexIdentifier(uint value)
            {
                return new IndexIdentifier(value);
            }
            protected IndexIdentifier(uint value)
            {
                this.id = value;
            }
            protected IndexIdentifier()
            {
                this.id = uint.MaxValue;
            }
            internal uint id;
        }
        #endregion
    }
    #region Index extension methods
    
    /// <summary>
    /// Provides interfaces to be translated to index queires in Linq expressions.
    /// </summary>
    public static class SubstringQueryExtension
    {
        
        public static bool Contains(this string @string, IEnumerable<string> substrings)
        {
            if (substrings == null || substrings.Count() == 0)
                return true;
            if (@string == null)
                throw new ArgumentNullException("string");
            int idx = 0;
            foreach (var substr in substrings)
            {
                if (substr == null)
                    continue;
                idx = @string.IndexOf(substr, idx);
                if (idx == -1)
                {
                    return false;
                }
            }
            return true;
        }
        public static bool Contains(this string @string, params string[] substrings)
        {
            return Contains(@string, substrings as IEnumerable<string>);
        }
        public static bool Contains(this StringAccessor @string, IEnumerable<string> substrings)
        {
            if (substrings == null || substrings.Count() == 0)
                return true;
            if (@string == (object)null)
                throw new ArgumentNullException("string");
            return Contains((string)@string, substrings);
        }
        public static bool Contains(this StringAccessor @string, params string[] substrings)
        {
            return Contains((string)@string, substrings as IEnumerable<string>);
        }
        
        public static bool ElementContainsSubstring(this List<string>list, IEnumerable<string> substrings)
        {
            if (substrings == null || substrings.Count() == 0)
                return true;
            if (list == null)
                throw new ArgumentNullException("list");
            
                foreach (string str in list)
                {
                    if (str.Contains(substrings))
                        return true;
                }
                
            return false;
        }
        public static bool ElementContainsSubstring(this List<string> list, params string[] substrings)
        {
            return ElementContainsSubstring(list, substrings as IEnumerable<string>);
        }
        
        public static bool ElementContainsSubstring(this StringAccessorListAccessor/*_*/list, IEnumerable<string> substrings)
        {
            if (substrings == null || substrings.Count() == 0)
                return true;
            if (list == null)
                throw new ArgumentNullException("list");
            
                foreach (string str in list)
                {
                    if (str.Contains(substrings))
                        return true;
                }
                
            return false;
        }
        public static bool ElementContainsSubstring(this StringAccessorListAccessor/*_*/list, params string[] substrings)
        {
            return ElementContainsSubstring(list, substrings as IEnumerable<string>);
        }
        
    }
    #endregion
}

#pragma warning restore 162,168,649,660,661,1522

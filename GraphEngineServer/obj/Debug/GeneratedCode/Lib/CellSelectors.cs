#pragma warning disable 162,168,649,660,661,1522

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity;
using Trinity.TSL;
using Trinity.TSL.Lib;
using Trinity.Storage;
using System.Linq.Expressions;
using GraphEngineServer.Linq;
namespace GraphEngineServer
{
    
    #region Internal
    /**
     * <summary>
     * Accepts transformation from Movie_Accessor to T.
     * </summary>
     */
    internal class Movie_Accessor_local_projector<T> : IQueryable<T>
    {
        private         Expression                                   query_expression;
        private         Movie_Accessor_local_query_provider    query_provider;
        internal Movie_Accessor_local_projector(Movie_Accessor_local_query_provider provider, Expression expression)
        {
            this.query_expression              = expression;
            this.query_provider                = provider;
        }
        public IEnumerator<T> GetEnumerator()
        {
            return Provider.Execute<IEnumerator<T>>(query_expression);
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        public Type ElementType
        {
            get { return typeof(T); }
        }
        public Expression Expression
        {
            get { return query_expression; }
        }
        public IQueryProvider Provider
        {
            get { return query_provider; }
        }
    }
    /**
     * Accepts transformation from Movie to T.
     */
    internal class Movie_local_projector<T> : IQueryable<T>
    {
        private         Expression                                   query_expression;
        private         Movie_local_query_provider             query_provider;
        internal Movie_local_projector(Movie_local_query_provider provider, Expression expression)
        {
            this.query_expression              = expression;
            this.query_provider                = provider;
        }
        public IEnumerator<T> GetEnumerator()
        {
            return Provider.Execute<IEnumerator<T>>(query_expression);
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        public Type ElementType
        {
            get { return typeof(T); }
        }
        public Expression Expression
        {
            get { return query_expression; }
        }
        public IQueryProvider Provider
        {
            get { return query_provider; }
        }
    }
    internal class Movie_AccessorEnumerable : IEnumerable<Movie_Accessor>
    {
        private     LocalMemoryStorage              m_storage;
        private     HashSet<long>                   m_filter_set;
        private     bool                            m_is_positive_filtering;
        private     Func<Movie_Accessor,bool> m_filter_predicate;
        internal Movie_AccessorEnumerable(LocalMemoryStorage storage)
        {
            this.m_storage     = storage;
            m_filter_set       = null;
            m_filter_predicate = null;
        }
        internal void SetPositiveFiltering(HashSet<long> set)
        {
            this.m_filter_set       = set;
            m_is_positive_filtering = true;
        }
        internal void SetNegativeFiltering(HashSet<long> set)
        {
            this.m_filter_set       = set;
            m_is_positive_filtering = false;
        }
        public IEnumerator<Movie_Accessor> GetEnumerator()
        {
            if (m_filter_set == null)
            {
                if (m_filter_predicate == null)
                    foreach (var cellInfo in m_storage)
                    {
                        if (cellInfo.CellType == (ushort)CellType.Movie)
                        {
                            var accessor = Movie_Accessor.AllocIterativeAccessor(cellInfo);
                            yield return accessor;
                            accessor.Dispose();
                        }
                    }
                else
                    foreach (var cellInfo in m_storage)
                    {
                        if (cellInfo.CellType == (ushort)CellType.Movie)
                        {
                            var accessor = Movie_Accessor.AllocIterativeAccessor(cellInfo);
                            if (m_filter_predicate(accessor))
                                yield return accessor;
                            accessor.Dispose();
                        }
                    }
            }
            else if (m_is_positive_filtering)
            {
                if (m_filter_predicate == null)
                    foreach (var cellID in m_filter_set)
                    {
                        using (var accessor = m_storage.UseMovie(cellID))
                        {
                            yield return accessor;
                        }
                    }
                else
                    foreach (var cellID in m_filter_set)
                    {
                        using (var accessor = m_storage.UseMovie(cellID))
                        {
                            if (m_filter_predicate(accessor))
                                yield return accessor;
                        }
                    }
            }
            else
            {
                throw new NotImplementedException("Negative filtering not supported.");
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        internal void SetPredicate(Expression aggregated_where_clause, ParameterExpression parameter)
        {
            m_filter_predicate = Expression.Lambda<Func<Movie_Accessor, bool>>(
                aggregated_where_clause,
                parameter
                ).Compile();
        }
    }
    internal class Movie_Enumerable : IEnumerable<Movie>
    {
        private     LocalMemoryStorage              m_storage;
        private     HashSet<long>                   m_filter_set;
        private     bool                            m_is_positive_filtering;
        private     Func<Movie,bool>          m_filter_predicate;
        private     static Type                     m_cell_type = typeof(Movie);
        internal Movie_Enumerable(LocalMemoryStorage storage)
        {
            this.m_storage     = storage;
            m_filter_set       = null;
            m_filter_predicate = null;
        }
        internal void SetPositiveFiltering(HashSet<long> set)
        {
            this.m_filter_set       = set;
            m_is_positive_filtering = true;
        }
        internal void SetNegativeFiltering(HashSet<long> set)
        {
            this.m_filter_set       = set;
            m_is_positive_filtering = false;
        }
        public IEnumerator<Movie> GetEnumerator()
        {
            if (m_filter_set == null)
            {
                if (m_filter_predicate == null)
                    foreach (var cellInfo in m_storage)
                    {
                        if (cellInfo.CellType == (ushort)CellType.Movie)
                        {
                            var accessor = Movie_Accessor.AllocIterativeAccessor(cellInfo);
                            yield return accessor;
                            accessor.Dispose();
                        }
                    }
                else
                    foreach (var cellInfo in m_storage)
                    {
                        if (cellInfo.CellType == (ushort)CellType.Movie)
                        {
                            var accessor = Movie_Accessor.AllocIterativeAccessor(cellInfo);
                            if (m_filter_predicate(accessor))
                                yield return accessor;
                            accessor.Dispose();
                        }
                    }
            }
            else if (m_is_positive_filtering)
            {
                if (m_filter_predicate == null)
                    foreach (var cellID in m_filter_set)
                    {
                        using (var accessor = m_storage.UseMovie(cellID))
                        {
                            yield return accessor;
                        }
                    }
                else
                    foreach (var cellID in m_filter_set)
                    {
                        using (var accessor = m_storage.UseMovie(cellID))
                        {
                            if (m_filter_predicate(accessor))
                                yield return accessor;
                        }
                    }
            }
            else
            {
                throw new NotImplementedException("Negative filtering not supported.");
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        internal void SetPredicate(Expression aggregated_where_clause, ParameterExpression parameter)
        {
            m_filter_predicate = Expression.Lambda<Func<Movie, bool>>(
                aggregated_where_clause,
                parameter
                ).Compile();
        }
    }
    internal class Movie_Accessor_local_query_provider : IQueryProvider
    {
        private static  Type                             s_accessor_type    = typeof(Movie_Accessor);
        private static  Type                             s_cell_type        = typeof(Movie);
        private static  Type                             s_ienumerable_type = typeof(IEnumerable<>);
        private         Movie_AccessorEnumerable   m_accessor_enumerable;
        internal Movie_Accessor_local_query_provider(LocalMemoryStorage storage)
        {
            m_accessor_enumerable = new Movie_AccessorEnumerable(storage);
        }
        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            if (typeof(TElement) == s_accessor_type)
            {
                return (IQueryable<TElement>)new Movie_Accessor_local_selector(this, expression);
            }
            else
            {
                return new Movie_Accessor_local_projector<TElement>(this, expression);
            }
        }
        public TResult Execute<TResult>(Expression expression)
        {
            var  visitor              = new RewrittableWhereCaluseVisitor<Movie_Accessor>(expression);
            var  where_clauses        = visitor.RewrittableWhereClauses;
            var  queryable            = m_accessor_enumerable.AsQueryable<Movie_Accessor>();
            var  trimmed_expression   = visitor.InjectEnumerator(expression, queryable, typeof(Movie_Accessor_local_selector));
            if (where_clauses.Count != 0)
            {
                var subject_rewritter           = new PredicateSubjectRewritter<Movie_Accessor>();
                Expression aggregated_predicate = subject_rewritter.Visit(where_clauses.First().Body);
                foreach (var where_clause in where_clauses.Skip(1))
                {
                    Expression predicate = where_clause.Body;
                    aggregated_predicate = Expression.AndAlso(aggregated_predicate, subject_rewritter.Visit(predicate));
                }
                IndexQueryTreeGenerator<Movie_Accessor> query_tree_gen       = new IndexQueryTreeGenerator<Movie_Accessor>("Movie", Index.s_AccessorSubstringIndexAccessMethod, is_cell: false);
                aggregated_predicate                                               = query_tree_gen.Visit(aggregated_predicate);
                var query_tree                                                     = query_tree_gen.QueryTree;
                if (query_tree != null)
                {
                    query_tree = query_tree.Optimize();
                    var query_tree_exec = new IndexQueryTreeExecutor(Index.s_AccessorSubstringQueryMethodTable, Index.s_AccessorSubstringWildcardQueryMethodTable);
                    m_accessor_enumerable.SetPositiveFiltering(query_tree_exec.Execute(query_tree));
                }
                m_accessor_enumerable.SetPredicate(aggregated_predicate, subject_rewritter.Parameter);
            }
            if (trimmed_expression.NodeType == ExpressionType.Constant)
            {
                return (TResult)m_accessor_enumerable.GetEnumerator();
            }
            Type result_type          = typeof(TResult);
            bool result_is_enumerable = (result_type.GenericTypeArguments.Count() == 1);
            Type element_type         = result_is_enumerable ? result_type.GenericTypeArguments[0] : result_type;
            if (result_is_enumerable)
            {
                var  enumerator_type      = s_ienumerable_type.MakeGenericType(element_type);
                var  enumerator_extractor = Expression.Call(trimmed_expression, enumerator_type.GetMethod("GetEnumerator"));
                var  lambda               = Expression.Lambda<Func<TResult>>(enumerator_extractor);
                var  func                 = (lambda).Compile();
                var  result               = func();
                return result;
            }
            else
            {
                var  lambda               = Expression.Lambda<Func<TResult>>(trimmed_expression);
                var  func                 = (lambda).Compile();
                var  result               = func();
                return result;
            }
        }
        #region Not implemented
        public IQueryable CreateQuery(Expression expression)
        {
            throw new NotImplementedException();
        }
        public object Execute(Expression expression)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
    internal class Movie_local_query_provider : IQueryProvider
    {
        private static  Type                             s_cell_type        = typeof(Movie);
        private static  Type                             s_ienumerable_type = typeof(IEnumerable<>);
        private         Movie_Enumerable           s_cell_enumerable;
        internal Movie_local_query_provider(LocalMemoryStorage storage)
        {
            s_cell_enumerable = new Movie_Enumerable(storage);
        }
        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            if (typeof(TElement) == s_cell_type)
            {
                return (IQueryable<TElement>)new Movie_local_selector(this, expression);
            }
            else
            {
                return new Movie_local_projector<TElement>(this, expression);
            }
        }
        public TResult Execute<TResult>(Expression expression)
        {
            var  visitor              = new RewrittableWhereCaluseVisitor<Movie>(expression);
            var  where_clauses        = visitor.RewrittableWhereClauses;
            var  queryable            = s_cell_enumerable.AsQueryable<Movie>();
            var  trimmed_expression   = visitor.InjectEnumerator(expression, queryable, typeof(Movie_local_selector));
            if (where_clauses.Count != 0)
            {
                var subject_rewritter           = new PredicateSubjectRewritter<Movie>();
                Expression aggregated_predicate = subject_rewritter.Visit(where_clauses.First().Body);
                foreach (var where_clause in where_clauses.Skip(1))
                {
                    Expression predicate = where_clause.Body;
                    aggregated_predicate = Expression.AndAlso(aggregated_predicate, subject_rewritter.Visit(predicate));
                }
                IndexQueryTreeGenerator<Movie> query_tree_gen       = new IndexQueryTreeGenerator<Movie>("Movie", Index.s_CellSubstringIndexAccessMethod, is_cell: true);
                aggregated_predicate                                      = query_tree_gen.Visit(aggregated_predicate);
                var query_tree                                            = query_tree_gen.QueryTree;
                if (query_tree != null)
                {
                    query_tree = query_tree.Optimize();
                    var query_tree_exec = new IndexQueryTreeExecutor(Index.s_CellSubstringQueryMethodTable, Index.s_CellSubstringWildcardQueryMethodTable);
                    s_cell_enumerable.SetPositiveFiltering(query_tree_exec.Execute(query_tree));
                }
                s_cell_enumerable.SetPredicate(aggregated_predicate, subject_rewritter.Parameter);
            }
            if (trimmed_expression.NodeType == ExpressionType.Constant)
            {
                return (TResult)s_cell_enumerable.GetEnumerator();
            }
            Type result_type          = typeof(TResult);
            bool result_is_enumerable = (result_type.GenericTypeArguments.Count() == 1);
            Type element_type         = result_is_enumerable ? result_type.GenericTypeArguments[0] : result_type;
            if (result_is_enumerable)
            {
                var  enumerator_type      = s_ienumerable_type.MakeGenericType(element_type);
                var  enumerator_extractor = Expression.Call(trimmed_expression, enumerator_type.GetMethod("GetEnumerator"));
                var  lambda               = Expression.Lambda<Func<TResult>>(enumerator_extractor);
                var  func                 = (lambda).Compile();
                var  result               = func();
                return result;
            }
            else
            {
                var  lambda               = Expression.Lambda<Func<TResult>>(trimmed_expression);
                var  func                 = (lambda).Compile();
                var  result               = func();
                return result;
            }
        }
        #region Not implemented
        public IQueryable CreateQuery(Expression expression)
        {
            throw new NotImplementedException();
        }
        public object Execute(Expression expression)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
    #endregion
    #region Public
    /// <summary>
    /// Implements System.Linq.IQueryable{Movie_Accessor} and accepts LINQ
    /// queries on <see cref="Trinity.Global.LocalStorage"/>.
    /// </summary>
    public class Movie_Accessor_local_selector : IQueryable<Movie_Accessor>
    {
        private         Expression                                   query_expression;
        private         Movie_Accessor_local_query_provider    query_provider;
        private Movie_Accessor_local_selector() { /* nobody should reach this method */ }
        internal Movie_Accessor_local_selector(Trinity.Storage.LocalMemoryStorage storage)
        {
            this.query_expression              = Expression.Constant(this);
            this.query_provider                = new Movie_Accessor_local_query_provider(storage);
        }
        internal unsafe Movie_Accessor_local_selector(Movie_Accessor_local_query_provider query_provider, Expression query_expression)
        {
            this.query_expression              = query_expression;
            this.query_provider                = query_provider;
        }
        #region IQueryable<CellAccessor> interfaces
        public IEnumerator<Movie_Accessor> GetEnumerator()
        {
            return Provider.Execute<IEnumerator<Movie_Accessor>>(query_expression);
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        public Type ElementType
        {
            get { return typeof(Movie_Accessor); }
        }
        public Expression Expression
        {
            get { return this.query_expression; }
        }
        public IQueryProvider Provider
        {
            get { return query_provider; }
        }
        #endregion
        #region PLINQ Wrapper
        public PLINQWrapper<Movie_Accessor> AsParallel()
        {
            return new PLINQWrapper<Movie_Accessor>(this);
        }
        #endregion
    }
    /// <summary>
    /// Implements System.Linq.IQueryable{Movie} and accepts LINQ
    /// queries on <see cref="Trinity.Global.LocalStorage"/>.
    /// </summary>
    public class Movie_local_selector : IQueryable<Movie>, IOrderedQueryable<Movie>
    {
        private         Expression                                   query_expression;
        private         Movie_local_query_provider             query_provider;
        private Movie_local_selector() { /* nobody should reach this method */ }
        internal Movie_local_selector(Trinity.Storage.LocalMemoryStorage storage)
        {
            this.query_expression              = Expression.Constant(this);
            this.query_provider                = new Movie_local_query_provider(storage);
        }
        internal unsafe Movie_local_selector(Movie_local_query_provider query_provider, Expression query_expression)
        {
            this.query_expression              = query_expression;
            this.query_provider                = query_provider;
        }
        #region IQueryable<Cell> interfaces
        public IEnumerator<Movie> GetEnumerator()
        {
            return Provider.Execute<IEnumerator<Movie>>(query_expression);
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator<Movie>)this.GetEnumerator();
        }
        public Type ElementType
        {
            get { return typeof(Movie); }
        }
        public Expression Expression
        {
            get { return this.query_expression; }
        }
        public IQueryProvider Provider
        {
            get { return query_provider; }
        }
        #endregion
    }
    #endregion
    
    #region Internal
    /**
     * <summary>
     * Accepts transformation from Celebrity_Accessor to T.
     * </summary>
     */
    internal class Celebrity_Accessor_local_projector<T> : IQueryable<T>
    {
        private         Expression                                   query_expression;
        private         Celebrity_Accessor_local_query_provider    query_provider;
        internal Celebrity_Accessor_local_projector(Celebrity_Accessor_local_query_provider provider, Expression expression)
        {
            this.query_expression              = expression;
            this.query_provider                = provider;
        }
        public IEnumerator<T> GetEnumerator()
        {
            return Provider.Execute<IEnumerator<T>>(query_expression);
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        public Type ElementType
        {
            get { return typeof(T); }
        }
        public Expression Expression
        {
            get { return query_expression; }
        }
        public IQueryProvider Provider
        {
            get { return query_provider; }
        }
    }
    /**
     * Accepts transformation from Celebrity to T.
     */
    internal class Celebrity_local_projector<T> : IQueryable<T>
    {
        private         Expression                                   query_expression;
        private         Celebrity_local_query_provider             query_provider;
        internal Celebrity_local_projector(Celebrity_local_query_provider provider, Expression expression)
        {
            this.query_expression              = expression;
            this.query_provider                = provider;
        }
        public IEnumerator<T> GetEnumerator()
        {
            return Provider.Execute<IEnumerator<T>>(query_expression);
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        public Type ElementType
        {
            get { return typeof(T); }
        }
        public Expression Expression
        {
            get { return query_expression; }
        }
        public IQueryProvider Provider
        {
            get { return query_provider; }
        }
    }
    internal class Celebrity_AccessorEnumerable : IEnumerable<Celebrity_Accessor>
    {
        private     LocalMemoryStorage              m_storage;
        private     HashSet<long>                   m_filter_set;
        private     bool                            m_is_positive_filtering;
        private     Func<Celebrity_Accessor,bool> m_filter_predicate;
        internal Celebrity_AccessorEnumerable(LocalMemoryStorage storage)
        {
            this.m_storage     = storage;
            m_filter_set       = null;
            m_filter_predicate = null;
        }
        internal void SetPositiveFiltering(HashSet<long> set)
        {
            this.m_filter_set       = set;
            m_is_positive_filtering = true;
        }
        internal void SetNegativeFiltering(HashSet<long> set)
        {
            this.m_filter_set       = set;
            m_is_positive_filtering = false;
        }
        public IEnumerator<Celebrity_Accessor> GetEnumerator()
        {
            if (m_filter_set == null)
            {
                if (m_filter_predicate == null)
                    foreach (var cellInfo in m_storage)
                    {
                        if (cellInfo.CellType == (ushort)CellType.Celebrity)
                        {
                            var accessor = Celebrity_Accessor.AllocIterativeAccessor(cellInfo);
                            yield return accessor;
                            accessor.Dispose();
                        }
                    }
                else
                    foreach (var cellInfo in m_storage)
                    {
                        if (cellInfo.CellType == (ushort)CellType.Celebrity)
                        {
                            var accessor = Celebrity_Accessor.AllocIterativeAccessor(cellInfo);
                            if (m_filter_predicate(accessor))
                                yield return accessor;
                            accessor.Dispose();
                        }
                    }
            }
            else if (m_is_positive_filtering)
            {
                if (m_filter_predicate == null)
                    foreach (var cellID in m_filter_set)
                    {
                        using (var accessor = m_storage.UseCelebrity(cellID))
                        {
                            yield return accessor;
                        }
                    }
                else
                    foreach (var cellID in m_filter_set)
                    {
                        using (var accessor = m_storage.UseCelebrity(cellID))
                        {
                            if (m_filter_predicate(accessor))
                                yield return accessor;
                        }
                    }
            }
            else
            {
                throw new NotImplementedException("Negative filtering not supported.");
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        internal void SetPredicate(Expression aggregated_where_clause, ParameterExpression parameter)
        {
            m_filter_predicate = Expression.Lambda<Func<Celebrity_Accessor, bool>>(
                aggregated_where_clause,
                parameter
                ).Compile();
        }
    }
    internal class Celebrity_Enumerable : IEnumerable<Celebrity>
    {
        private     LocalMemoryStorage              m_storage;
        private     HashSet<long>                   m_filter_set;
        private     bool                            m_is_positive_filtering;
        private     Func<Celebrity,bool>          m_filter_predicate;
        private     static Type                     m_cell_type = typeof(Celebrity);
        internal Celebrity_Enumerable(LocalMemoryStorage storage)
        {
            this.m_storage     = storage;
            m_filter_set       = null;
            m_filter_predicate = null;
        }
        internal void SetPositiveFiltering(HashSet<long> set)
        {
            this.m_filter_set       = set;
            m_is_positive_filtering = true;
        }
        internal void SetNegativeFiltering(HashSet<long> set)
        {
            this.m_filter_set       = set;
            m_is_positive_filtering = false;
        }
        public IEnumerator<Celebrity> GetEnumerator()
        {
            if (m_filter_set == null)
            {
                if (m_filter_predicate == null)
                    foreach (var cellInfo in m_storage)
                    {
                        if (cellInfo.CellType == (ushort)CellType.Celebrity)
                        {
                            var accessor = Celebrity_Accessor.AllocIterativeAccessor(cellInfo);
                            yield return accessor;
                            accessor.Dispose();
                        }
                    }
                else
                    foreach (var cellInfo in m_storage)
                    {
                        if (cellInfo.CellType == (ushort)CellType.Celebrity)
                        {
                            var accessor = Celebrity_Accessor.AllocIterativeAccessor(cellInfo);
                            if (m_filter_predicate(accessor))
                                yield return accessor;
                            accessor.Dispose();
                        }
                    }
            }
            else if (m_is_positive_filtering)
            {
                if (m_filter_predicate == null)
                    foreach (var cellID in m_filter_set)
                    {
                        using (var accessor = m_storage.UseCelebrity(cellID))
                        {
                            yield return accessor;
                        }
                    }
                else
                    foreach (var cellID in m_filter_set)
                    {
                        using (var accessor = m_storage.UseCelebrity(cellID))
                        {
                            if (m_filter_predicate(accessor))
                                yield return accessor;
                        }
                    }
            }
            else
            {
                throw new NotImplementedException("Negative filtering not supported.");
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        internal void SetPredicate(Expression aggregated_where_clause, ParameterExpression parameter)
        {
            m_filter_predicate = Expression.Lambda<Func<Celebrity, bool>>(
                aggregated_where_clause,
                parameter
                ).Compile();
        }
    }
    internal class Celebrity_Accessor_local_query_provider : IQueryProvider
    {
        private static  Type                             s_accessor_type    = typeof(Celebrity_Accessor);
        private static  Type                             s_cell_type        = typeof(Celebrity);
        private static  Type                             s_ienumerable_type = typeof(IEnumerable<>);
        private         Celebrity_AccessorEnumerable   m_accessor_enumerable;
        internal Celebrity_Accessor_local_query_provider(LocalMemoryStorage storage)
        {
            m_accessor_enumerable = new Celebrity_AccessorEnumerable(storage);
        }
        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            if (typeof(TElement) == s_accessor_type)
            {
                return (IQueryable<TElement>)new Celebrity_Accessor_local_selector(this, expression);
            }
            else
            {
                return new Celebrity_Accessor_local_projector<TElement>(this, expression);
            }
        }
        public TResult Execute<TResult>(Expression expression)
        {
            var  visitor              = new RewrittableWhereCaluseVisitor<Celebrity_Accessor>(expression);
            var  where_clauses        = visitor.RewrittableWhereClauses;
            var  queryable            = m_accessor_enumerable.AsQueryable<Celebrity_Accessor>();
            var  trimmed_expression   = visitor.InjectEnumerator(expression, queryable, typeof(Celebrity_Accessor_local_selector));
            if (where_clauses.Count != 0)
            {
                var subject_rewritter           = new PredicateSubjectRewritter<Celebrity_Accessor>();
                Expression aggregated_predicate = subject_rewritter.Visit(where_clauses.First().Body);
                foreach (var where_clause in where_clauses.Skip(1))
                {
                    Expression predicate = where_clause.Body;
                    aggregated_predicate = Expression.AndAlso(aggregated_predicate, subject_rewritter.Visit(predicate));
                }
                IndexQueryTreeGenerator<Celebrity_Accessor> query_tree_gen       = new IndexQueryTreeGenerator<Celebrity_Accessor>("Celebrity", Index.s_AccessorSubstringIndexAccessMethod, is_cell: false);
                aggregated_predicate                                               = query_tree_gen.Visit(aggregated_predicate);
                var query_tree                                                     = query_tree_gen.QueryTree;
                if (query_tree != null)
                {
                    query_tree = query_tree.Optimize();
                    var query_tree_exec = new IndexQueryTreeExecutor(Index.s_AccessorSubstringQueryMethodTable, Index.s_AccessorSubstringWildcardQueryMethodTable);
                    m_accessor_enumerable.SetPositiveFiltering(query_tree_exec.Execute(query_tree));
                }
                m_accessor_enumerable.SetPredicate(aggregated_predicate, subject_rewritter.Parameter);
            }
            if (trimmed_expression.NodeType == ExpressionType.Constant)
            {
                return (TResult)m_accessor_enumerable.GetEnumerator();
            }
            Type result_type          = typeof(TResult);
            bool result_is_enumerable = (result_type.GenericTypeArguments.Count() == 1);
            Type element_type         = result_is_enumerable ? result_type.GenericTypeArguments[0] : result_type;
            if (result_is_enumerable)
            {
                var  enumerator_type      = s_ienumerable_type.MakeGenericType(element_type);
                var  enumerator_extractor = Expression.Call(trimmed_expression, enumerator_type.GetMethod("GetEnumerator"));
                var  lambda               = Expression.Lambda<Func<TResult>>(enumerator_extractor);
                var  func                 = (lambda).Compile();
                var  result               = func();
                return result;
            }
            else
            {
                var  lambda               = Expression.Lambda<Func<TResult>>(trimmed_expression);
                var  func                 = (lambda).Compile();
                var  result               = func();
                return result;
            }
        }
        #region Not implemented
        public IQueryable CreateQuery(Expression expression)
        {
            throw new NotImplementedException();
        }
        public object Execute(Expression expression)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
    internal class Celebrity_local_query_provider : IQueryProvider
    {
        private static  Type                             s_cell_type        = typeof(Celebrity);
        private static  Type                             s_ienumerable_type = typeof(IEnumerable<>);
        private         Celebrity_Enumerable           s_cell_enumerable;
        internal Celebrity_local_query_provider(LocalMemoryStorage storage)
        {
            s_cell_enumerable = new Celebrity_Enumerable(storage);
        }
        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            if (typeof(TElement) == s_cell_type)
            {
                return (IQueryable<TElement>)new Celebrity_local_selector(this, expression);
            }
            else
            {
                return new Celebrity_local_projector<TElement>(this, expression);
            }
        }
        public TResult Execute<TResult>(Expression expression)
        {
            var  visitor              = new RewrittableWhereCaluseVisitor<Celebrity>(expression);
            var  where_clauses        = visitor.RewrittableWhereClauses;
            var  queryable            = s_cell_enumerable.AsQueryable<Celebrity>();
            var  trimmed_expression   = visitor.InjectEnumerator(expression, queryable, typeof(Celebrity_local_selector));
            if (where_clauses.Count != 0)
            {
                var subject_rewritter           = new PredicateSubjectRewritter<Celebrity>();
                Expression aggregated_predicate = subject_rewritter.Visit(where_clauses.First().Body);
                foreach (var where_clause in where_clauses.Skip(1))
                {
                    Expression predicate = where_clause.Body;
                    aggregated_predicate = Expression.AndAlso(aggregated_predicate, subject_rewritter.Visit(predicate));
                }
                IndexQueryTreeGenerator<Celebrity> query_tree_gen       = new IndexQueryTreeGenerator<Celebrity>("Celebrity", Index.s_CellSubstringIndexAccessMethod, is_cell: true);
                aggregated_predicate                                      = query_tree_gen.Visit(aggregated_predicate);
                var query_tree                                            = query_tree_gen.QueryTree;
                if (query_tree != null)
                {
                    query_tree = query_tree.Optimize();
                    var query_tree_exec = new IndexQueryTreeExecutor(Index.s_CellSubstringQueryMethodTable, Index.s_CellSubstringWildcardQueryMethodTable);
                    s_cell_enumerable.SetPositiveFiltering(query_tree_exec.Execute(query_tree));
                }
                s_cell_enumerable.SetPredicate(aggregated_predicate, subject_rewritter.Parameter);
            }
            if (trimmed_expression.NodeType == ExpressionType.Constant)
            {
                return (TResult)s_cell_enumerable.GetEnumerator();
            }
            Type result_type          = typeof(TResult);
            bool result_is_enumerable = (result_type.GenericTypeArguments.Count() == 1);
            Type element_type         = result_is_enumerable ? result_type.GenericTypeArguments[0] : result_type;
            if (result_is_enumerable)
            {
                var  enumerator_type      = s_ienumerable_type.MakeGenericType(element_type);
                var  enumerator_extractor = Expression.Call(trimmed_expression, enumerator_type.GetMethod("GetEnumerator"));
                var  lambda               = Expression.Lambda<Func<TResult>>(enumerator_extractor);
                var  func                 = (lambda).Compile();
                var  result               = func();
                return result;
            }
            else
            {
                var  lambda               = Expression.Lambda<Func<TResult>>(trimmed_expression);
                var  func                 = (lambda).Compile();
                var  result               = func();
                return result;
            }
        }
        #region Not implemented
        public IQueryable CreateQuery(Expression expression)
        {
            throw new NotImplementedException();
        }
        public object Execute(Expression expression)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
    #endregion
    #region Public
    /// <summary>
    /// Implements System.Linq.IQueryable{Celebrity_Accessor} and accepts LINQ
    /// queries on <see cref="Trinity.Global.LocalStorage"/>.
    /// </summary>
    public class Celebrity_Accessor_local_selector : IQueryable<Celebrity_Accessor>
    {
        private         Expression                                   query_expression;
        private         Celebrity_Accessor_local_query_provider    query_provider;
        private Celebrity_Accessor_local_selector() { /* nobody should reach this method */ }
        internal Celebrity_Accessor_local_selector(Trinity.Storage.LocalMemoryStorage storage)
        {
            this.query_expression              = Expression.Constant(this);
            this.query_provider                = new Celebrity_Accessor_local_query_provider(storage);
        }
        internal unsafe Celebrity_Accessor_local_selector(Celebrity_Accessor_local_query_provider query_provider, Expression query_expression)
        {
            this.query_expression              = query_expression;
            this.query_provider                = query_provider;
        }
        #region IQueryable<CellAccessor> interfaces
        public IEnumerator<Celebrity_Accessor> GetEnumerator()
        {
            return Provider.Execute<IEnumerator<Celebrity_Accessor>>(query_expression);
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        public Type ElementType
        {
            get { return typeof(Celebrity_Accessor); }
        }
        public Expression Expression
        {
            get { return this.query_expression; }
        }
        public IQueryProvider Provider
        {
            get { return query_provider; }
        }
        #endregion
        #region PLINQ Wrapper
        public PLINQWrapper<Celebrity_Accessor> AsParallel()
        {
            return new PLINQWrapper<Celebrity_Accessor>(this);
        }
        #endregion
    }
    /// <summary>
    /// Implements System.Linq.IQueryable{Celebrity} and accepts LINQ
    /// queries on <see cref="Trinity.Global.LocalStorage"/>.
    /// </summary>
    public class Celebrity_local_selector : IQueryable<Celebrity>, IOrderedQueryable<Celebrity>
    {
        private         Expression                                   query_expression;
        private         Celebrity_local_query_provider             query_provider;
        private Celebrity_local_selector() { /* nobody should reach this method */ }
        internal Celebrity_local_selector(Trinity.Storage.LocalMemoryStorage storage)
        {
            this.query_expression              = Expression.Constant(this);
            this.query_provider                = new Celebrity_local_query_provider(storage);
        }
        internal unsafe Celebrity_local_selector(Celebrity_local_query_provider query_provider, Expression query_expression)
        {
            this.query_expression              = query_expression;
            this.query_provider                = query_provider;
        }
        #region IQueryable<Cell> interfaces
        public IEnumerator<Celebrity> GetEnumerator()
        {
            return Provider.Execute<IEnumerator<Celebrity>>(query_expression);
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator<Celebrity>)this.GetEnumerator();
        }
        public Type ElementType
        {
            get { return typeof(Celebrity); }
        }
        public Expression Expression
        {
            get { return this.query_expression; }
        }
        public IQueryProvider Provider
        {
            get { return query_provider; }
        }
        #endregion
    }
    #endregion
    
    public static class LocalStorageCellSelectorExternsion
    {
        
        /// <summary>
        /// Enumerates all the Movie within the local memory storage.
        /// </summary>
        /// <param name="storage">A <see cref="Trinity.Storage.LocalMemoryStorage"/> object.</param>
        /// <returns>All the Movie within the local memory storage.</returns>
        public static Movie_local_selector/*_*/Movie_Selector(this LocalMemoryStorage storage)
        {
            return new Movie_local_selector(storage);
        }
        /// <summary>
        /// Enumerates all the Movie_Accessor within the local memory storage.
        /// </summary>
        /// <param name="storage">A <see cref="Trinity.Storage.LocalMemoryStorage"/> object.</param>
        /// <returns>All the Movie_Accessor within the local memory storage.</returns>
        public static Movie_Accessor_local_selector/*_*/Movie_Accessor_Selector(this LocalMemoryStorage storage)
        {
            return new Movie_Accessor_local_selector(storage);
        }
        
        /// <summary>
        /// Enumerates all the Celebrity within the local memory storage.
        /// </summary>
        /// <param name="storage">A <see cref="Trinity.Storage.LocalMemoryStorage"/> object.</param>
        /// <returns>All the Celebrity within the local memory storage.</returns>
        public static Celebrity_local_selector/*_*/Celebrity_Selector(this LocalMemoryStorage storage)
        {
            return new Celebrity_local_selector(storage);
        }
        /// <summary>
        /// Enumerates all the Celebrity_Accessor within the local memory storage.
        /// </summary>
        /// <param name="storage">A <see cref="Trinity.Storage.LocalMemoryStorage"/> object.</param>
        /// <returns>All the Celebrity_Accessor within the local memory storage.</returns>
        public static Celebrity_Accessor_local_selector/*_*/Celebrity_Accessor_Selector(this LocalMemoryStorage storage)
        {
            return new Celebrity_Accessor_local_selector(storage);
        }
        
    }
}

#pragma warning restore 162,168,649,660,661,1522

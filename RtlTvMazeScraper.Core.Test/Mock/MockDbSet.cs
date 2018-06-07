// <copyright file="MockDbSet.cs" company="Hans Kesting">
// Copyright (c) Hans Kesting. All rights reserved.
// </copyright>


namespace RtlTvMazeScraper.Core.Test.Mock
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    public class MockDbSet<TEntity> : DbSet<TEntity>, IQueryable, IEnumerable<TEntity>, IDbAsyncEnumerable<TEntity>
        where TEntity : class
    {
        // http://msdn.microsoft.com/en-us/data/dn314431#doubles

        readonly ObservableCollection<TEntity> _data;
        readonly IQueryable _query;

        public MockDbSet()
        {
            this._data = new ObservableCollection<TEntity>();
            this._query = this._data.AsQueryable();
        }

        public override TEntity Add(TEntity item)
        {
            this._data.Add(item);
            return item;
        }

        public override TEntity Remove(TEntity item)
        {
            this._data.Remove(item);
            return item;
        }

        public override TEntity Attach(TEntity item)
        {
            this._data.Add(item);
            return item;
        }

        public override TEntity Create()
        {
            return Activator.CreateInstance<TEntity>();
        }

        public override TDerivedEntity Create<TDerivedEntity>()
        {
            return Activator.CreateInstance<TDerivedEntity>();
        }

        public override ObservableCollection<TEntity> Local
        {
            get { return this._data; }
        }

        Type IQueryable.ElementType
        {
            get { return this._query.ElementType; }
        }

        Expression IQueryable.Expression
        {
            get { return this._query.Expression; }
        }

        IQueryProvider IQueryable.Provider
        {
            get { return new MockDbAsyncQueryProvider<TEntity>(this._query.Provider); }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this._data.GetEnumerator();
        }

        IEnumerator<TEntity> IEnumerable<TEntity>.GetEnumerator()
        {
            return this._data.GetEnumerator();
        }

        IDbAsyncEnumerator<TEntity> IDbAsyncEnumerable<TEntity>.GetAsyncEnumerator()
        {
            return new MockDbAsyncEnumerator<TEntity>(this._data.GetEnumerator());
        }
    }

    internal class MockDbAsyncQueryProvider<TEntity> : IDbAsyncQueryProvider
    {
        private readonly IQueryProvider _inner;

        internal MockDbAsyncQueryProvider(IQueryProvider inner)
        {
            this._inner = inner;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return new MockDbAsyncEnumerable<TEntity>(expression);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new MockDbAsyncEnumerable<TElement>(expression);
        }

        public object Execute(Expression expression)
        {
            return this._inner.Execute(expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return this._inner.Execute<TResult>(expression);
        }

        public Task<object> ExecuteAsync(Expression expression, CancellationToken cancellationToken)
        {
            return Task.FromResult(Execute(expression));
        }

        public Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
        {
            return Task.FromResult(Execute<TResult>(expression));
        }
    }

    internal class MockDbAsyncEnumerable<T> : EnumerableQuery<T>, IDbAsyncEnumerable<T>, IQueryable<T>
    {
        public MockDbAsyncEnumerable(IEnumerable<T> enumerable)
            : base(enumerable)
        { }

        public MockDbAsyncEnumerable(Expression expression)
            : base(expression)
        { }

        public IDbAsyncEnumerator<T> GetAsyncEnumerator()
        {
            return new MockDbAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
        }

        IDbAsyncEnumerator IDbAsyncEnumerable.GetAsyncEnumerator()
        {
            return GetAsyncEnumerator();
        }

        IQueryProvider IQueryable.Provider
        {
            get { return new MockDbAsyncQueryProvider<T>(this); }
        }
    }

    internal class MockDbAsyncEnumerator<T> : IDbAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _inner;

        public MockDbAsyncEnumerator(IEnumerator<T> inner)
        {
            this._inner = inner;
        }

        public void Dispose()
        {
            this._inner.Dispose();
        }

        public Task<bool> MoveNextAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(this._inner.MoveNext());
        }

        public T Current
        {
            get { return this._inner.Current; }
        }

        object IDbAsyncEnumerator.Current
        {
            get { return Current; }
        }
    }
}

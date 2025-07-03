using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace IncidentsAppApiTest.Helpers
{
    public class TestAsyncEnumerableHelper<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
    {
        public TestAsyncEnumerableHelper(IEnumerable<T> enumerable) : base(enumerable) { }
        public TestAsyncEnumerableHelper(Expression expression) : base(expression) { }
        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken token = default)
            => new TestAsyncEnumeratorHelper<T>(this.AsEnumerable().GetEnumerator());
    }
}

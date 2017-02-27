using System;
using System.Diagnostics;
using System.Linq.Expressions;

namespace TinyBDD
{
    public static class Extensions
    {
        public static TestClause<T> Given<T>(this T t, Expression<Action<T>> expression)
            where T : class, new()
        {
            var frame = new StackFrame(1);
            var method = frame.GetMethod();
            return new TestClause<T>(t, method.Name, expression);
        }
    }
}

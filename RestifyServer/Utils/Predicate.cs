using System.Linq.Expressions;

namespace RestifyServer.Utils;

public static class Predicate
{
    public static Expression<Func<T, bool>> True<T>() => _ => true;

    public static Expression<Func<T, bool>> And<T>(
        this Expression<Func<T, bool>> left,
        Expression<Func<T, bool>> right)
    {
        var param = left.Parameters[0];
        var body = Expression.AndAlso(
            left.Body,
            Expression.Invoke(right, param)
        );
        return Expression.Lambda<Func<T, bool>>(body, param);
    }
}

using System;
using System.Linq.Expressions;

namespace ModelViewViewModel
{
    public static class PropertyName
    {
        public static string For<T>(Expression<Func<T, object>> propertyExpression)
        {
            return For<T, object>(propertyExpression);
        }

        public static string For<TClass, TProp>(Expression<Func<TClass, TProp>> propertyExpression)
        {
            return propertyExpression.Body is UnaryExpression
                ? ((MemberExpression)((UnaryExpression)propertyExpression.Body).Operand).Member.Name
                : ((MemberExpression)propertyExpression.Body).Member.Name;
        }
    }

    public class PropertyNameInfo<T>
    {
        public string Of(Expression<Func<T, object>> propertyExpression)
        {
            return PropertyName.For(propertyExpression);
        }

        public string Of<TProp>(Expression<Func<T, TProp>> propertyExpression)
        {
            return PropertyName.For(propertyExpression);
        }
    }
}

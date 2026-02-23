using System.Linq.Expressions;
using Newtonsoft.Json;

namespace Mamey.Types;

public static class Extensions
{
    public static UpdatePropertiesResults<T> UpdateProperties<T>(this T current, T updated)
        where T : class, new()
    {
        if (current is null)
        {
            throw new ArgumentNullException(nameof(current));
        }

        if (updated is null)
        {
            throw new ArgumentNullException(nameof(updated));
        }

        var oldValues = new Dictionary<string, object>();
        var newValues = new Dictionary<string, object>();

        var properties = typeof(T).GetProperties();

        foreach (var property in properties)
        {
            var currentValue = property.GetValue(current);
            var updatedValue = property.GetValue(updated);

            if (!Equals(currentValue, updatedValue))
            {
                oldValues[property.Name] = currentValue;
                newValues[property.Name] = updatedValue;
                property.SetValue(current, updatedValue);
            }
        }
        
        if (oldValues.Any())
        {
            Console.WriteLine("Profile updated:");
            Console.WriteLine(JsonConvert.SerializeObject(oldValues) + " -> " + JsonConvert.SerializeObject(newValues));
        }
        else
        {
            Console.WriteLine("Profile not updated");
        }
        var result = new UpdatePropertiesResults<T>(current, oldValues, newValues);
        return result;
    }
    
    public static string GetMemberName<T>(this Expression<Func<T, object>> expression)
    {
        return GetMemberName(expression.Body);
    }
    private static string GetMemberName(this Expression expression)
    {
        if (expression == null)
        {
            throw new ArgumentException("Expression cannot be null.");
        }
        if (expression is MemberExpression)
        {
            // Reference22 type property or field
            var memberExpression = (MemberExpression)expression;
            return memberExpression.Member.Name;
        }
        if (expression is MethodCallExpression)
        {
            // Reference22 type method
            var methodCallExpression = (MethodCallExpression)expression;
            return methodCallExpression.Method.Name;
        }
        if (expression is UnaryExpression)
        {
            // Property, field of method returning value type
            var unaryExpression = (UnaryExpression)expression;
            return GetMemberName(unaryExpression);
        }
        throw new ArgumentException("Expression is invalid");
    }
    private static string GetMemberName(this UnaryExpression unaryExpression)
    {
        if (unaryExpression.Operand is MethodCallExpression)
        {
            var methodExpression = (MethodCallExpression)unaryExpression.Operand;
            return methodExpression.Method.Name;
        }
        return ((MemberExpression)unaryExpression.Operand).Member.Name;
    }
}
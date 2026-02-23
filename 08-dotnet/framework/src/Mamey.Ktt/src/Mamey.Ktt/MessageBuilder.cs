using System.Reflection;
using System.Text.RegularExpressions;
using Mamey.Ktt.Attributes;

namespace Mamey.Ktt;

public class MessageBuilder
{
    /// <summary>
    /// Builds a message by replacing placeholders in the template with values from the given model.
    /// </summary>
    /// <typeparam name="T">Type of the model.</typeparam>
    /// <param name="template">The message template containing placeholders.</param>
    /// <param name="model">The model object with data to fill the template.</param>
    /// <returns>A formatted message with placeholders replaced.</returns>
    public static string BuildMessage<T>(string template, T model)
    {
        if (string.IsNullOrEmpty(template) || model == null)
            throw new ArgumentException("Template or model cannot be null.");

        Type modelType = model.GetType();
        PropertyInfo[] properties = modelType.GetProperties();

        // Replace each placeholder with the corresponding property value
        foreach (var property in properties)
        {
            var fieldTagAttr = property.GetCustomAttribute<FieldTagAttribute>();
            var fieldNameAttr = property.GetCustomAttribute<FieldNameAttribute>();
            if (fieldTagAttr != null && fieldNameAttr != null)
            {
                string placeholder = $"{{{{{fieldTagAttr.Tag}}}}}";
                var value = property.GetValue(model)?.ToString() ?? string.Empty;

                // Replace the placeholder in the template with the actual value
                template = template.Replace(placeholder, value);
            }
        }

        // Clean up any remaining placeholders that have not been replaced
        template = Regex.Replace(template, @"{{.*?}}", string.Empty);

        return template;
    }
}
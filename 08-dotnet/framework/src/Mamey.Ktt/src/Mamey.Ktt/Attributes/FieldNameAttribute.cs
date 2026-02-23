namespace Mamey.Ktt.Attributes;

/// <summary>
/// Attribute to specify the field name for a SWIFT message field.
/// </summary>
[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public sealed class FieldNameAttribute : Attribute
{
    /// <summary>
    /// Gets the name of the SWIFT field.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="FieldNameAttribute"/> class.
    /// </summary>
    /// <param name="name">The field name (e.g., "Transaction Reference Number").</param>
    public FieldNameAttribute(string name)
    {
        Name = name;
    }
}
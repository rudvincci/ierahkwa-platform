namespace Mamey.Ktt.Attributes;

/// <summary>
/// Attribute to specify the field tag for a SWIFT message field.
/// </summary>
[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public sealed class FieldTagAttribute : Attribute
{
    /// <summary>
    /// Gets the SWIFT field tag.
    /// </summary>
    public string Tag { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="FieldTagAttribute"/> class.
    /// </summary>
    /// <param name="tag">The SWIFT field tag (e.g., "20", "23B").</param>
    public FieldTagAttribute(string tag)
    {
        Tag = tag;
    }
}
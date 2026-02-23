public class Product
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public bool Active { get; set; }
    public DateTime Created { get; set; }
    public Dictionary<string, string> Metadata { get; set; }
    // Additional properties for URLs, images, and categorization can be included as needed.
}

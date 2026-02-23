namespace Mamey.Types;

public interface ISoftDeletable
{
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
}

public class IsSoftDeletable : ISoftDeletable
{
    public IsSoftDeletable()
    {
        
    }
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; } = null;
}
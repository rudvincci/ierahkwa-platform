namespace Ierahkwa.PostalMapsService.Domain;

public class MapTile
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = "Active";
    public string CreatedBy { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty;
    public int ZoomLevel { get; set; }
    public int TileX { get; set; }
    public int TileY { get; set; }
    public string TileUrl { get; set; } = string.Empty;
}

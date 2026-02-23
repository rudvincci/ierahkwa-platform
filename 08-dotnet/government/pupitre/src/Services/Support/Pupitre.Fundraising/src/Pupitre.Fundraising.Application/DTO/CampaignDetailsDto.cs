using System.Text.Json.Serialization;
using Mamey.Types;

namespace Pupitre.Fundraising.Application.DTO;

internal class CampaignDetailsDto : CampaignDto
{
    public CampaignDetailsDto(Guid id, string name, IEnumerable<string> tags, DateTime createdAt, DateTime? modifiedAt)
        : base(id, name, tags)
    {
        CreatedAt = createdAt;
        ModifiedAt = modifiedAt;
    }
    public CampaignDetailsDto(CampaignDto campaignDto, DateTime createdAt, DateTime? modifiedAt)
        : base(campaignDto.Id, campaignDto.Name, campaignDto.Tags)
    {
        CreatedAt = createdAt;
        ModifiedAt = modifiedAt;
    }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
}

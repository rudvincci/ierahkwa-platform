using Pupitre.Fundraising.Application.DTO;
using Pupitre.Fundraising.Domain.Entities;
using Mamey.Types;
using Mamey;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Pupitre.Fundraising.Tests.Integration.Async")]
namespace Pupitre.Fundraising.Infrastructure.Mongo.Documents;

internal class CampaignDocument : IIdentifiable<Guid>
{
    public CampaignDocument()
    {

    }

    public CampaignDocument(Campaign campaign)
    {
        if (campaign is null)
        {
            throw new NullReferenceException();
        }

        Id = campaign.Id.Value;
        Name = campaign.Name;
        CreatedAt = campaign.CreatedAt.ToUnixTimeMilliseconds();
        ModifiedAt = campaign.ModifiedAt?.ToUnixTimeMilliseconds();
        Tags = campaign.Tags;
        Version = campaign.Version;
    }

    public Guid Id { get; set; }
    public string Name { get; set; }
    public long CreatedAt { get; set; }
    public long? ModifiedAt { get; set; }
    public IEnumerable<string> Tags { get; set; }
    public int Version { get; set; }

    public Campaign AsEntity()
        => new(Id, Name, CreatedAt.GetDate(), ModifiedAt?.GetDate(), Tags, Version);

    public CampaignDto AsDto()
        => new CampaignDto(Id, Name, Tags);
    public CampaignDetailsDto AsDetailsDto()
        => new CampaignDetailsDto(this.AsDto(), CreatedAt.GetDate(), ModifiedAt?.GetDate());
}


using System.ComponentModel;
using System.Runtime.CompilerServices;
using Pupitre.Fundraising.Domain.Events;
using Pupitre.Fundraising.Domain.Exceptions;
using Mamey.Types;

[assembly: InternalsVisibleTo("Pupitre.Fundraising.Tests.Unit.Core.Entities")]
namespace Pupitre.Fundraising.Domain.Entities;


internal class Campaign : AggregateRoot<CampaignId>
{
    #region Fields
    private ISet<string> _tags = new HashSet<string>();
    #endregion


    public Campaign(CampaignId id, string name, DateTime createdAt,
        DateTime? modifiedAt = null, IEnumerable<string>? tags = null, int version = 0)
        : base(id, version)
    {
        Name = name;
        CreatedAt = createdAt;
        ModifiedAt = modifiedAt;
        Tags = tags ?? Enumerable.Empty<string>();
    }

    #region Properties

    /// <summary>
    /// A name for the campaign.
    /// </summary>
    [Description("The campaign's name")]
    public string Name { get; private set; }

    /// <summary>
    /// Date and time the record was created.
    /// </summary>
    [Description("Date and time the record was created.")]
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Date and time the record was modified.
    /// </summary>
    [Description("Date and time the record was modified.")]
    public DateTime? ModifiedAt { get; private set; }

    /// <summary>
    /// Collection of Campaign tags.
    /// </summary>
    [Description("Collection of Campaign tags.")]
    public IEnumerable<string> Tags
    {
        get => _tags;
        private set => _tags = new HashSet<string>(value);
    }
    #endregion

    public static Campaign Create(Guid id, string name, IEnumerable<string>? tags)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new MissingCampaignNameException();
        }

        var campaign = new Campaign(id, name, DateTime.UtcNow, tags: tags);
        campaign.AddEvent(new CampaignCreated(campaign));
        return campaign;
    }

    public void Update(string name, IEnumerable<string> tags)
    {
        Name = name;
        Tags = tags;
        this.AddEvent(new CampaignModified(this));
    }

    private static void ValidateTags(IEnumerable<string> tags)
    {
        if (tags.Any(string.IsNullOrWhiteSpace))
        {
            throw new InvalidCampaignTagsException();
        }
    }
}


using System;
using Mamey.CQRS.Queries;
using Pupitre.Fundraising.Application.DTO;

namespace Pupitre.Fundraising.Application.Queries;

internal record GetCampaign(Guid Id) : IQuery<CampaignDetailsDto>;

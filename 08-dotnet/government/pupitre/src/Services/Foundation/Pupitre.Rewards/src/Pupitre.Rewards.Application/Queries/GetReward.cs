using System;
using Mamey.CQRS.Queries;
using Pupitre.Rewards.Application.DTO;

namespace Pupitre.Rewards.Application.Queries;

internal record GetReward(Guid Id) : IQuery<RewardDetailsDto>;

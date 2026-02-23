using System;
using Mamey.CQRS.Queries;
using Pupitre.AIContent.Application.DTO;

namespace Pupitre.AIContent.Application.Queries;

internal record GetContentGeneration(Guid Id) : IQuery<ContentGenerationDetailsDto>;

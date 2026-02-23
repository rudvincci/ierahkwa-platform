using System;
using Mamey.CQRS.Queries;
using Pupitre.AITranslation.Application.DTO;

namespace Pupitre.AITranslation.Application.Queries;

internal record GetTranslationRequest(Guid Id) : IQuery<TranslationRequestDetailsDto>;

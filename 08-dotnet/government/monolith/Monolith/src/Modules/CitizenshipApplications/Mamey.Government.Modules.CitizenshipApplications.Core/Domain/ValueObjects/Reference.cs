using Mamey.Types;

namespace Mamey.Government.Modules.CitizenshipApplications.Core.Domain.ValueObjects;

internal record Reference(Name Name, Address Address, Phone Phone, string Relationship);

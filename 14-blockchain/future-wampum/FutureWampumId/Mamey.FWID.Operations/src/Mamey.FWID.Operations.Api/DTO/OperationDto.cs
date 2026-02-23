using System;
using Mamey.FWID.Operations.Api.Types;

namespace Mamey.FWID.Operations.Api.DTO;

public class OperationDto
{
    public string Id { get; set; }
    public string UserId { get; set; }
    public string Name { get; set; }
    public OperationState State { get; set; }
    public string Code { get; set; }
    public string Reason { get; set; }
}




using MameyNode.Portals.Mocks.Models;

namespace MameyNode.Portals.Mocks.Interfaces;

public interface IODLClient
{
    Task<List<ODLInfo>> GetODLOperationsAsync();
    Task<ODLInfo?> GetODLOperationAsync(string odlId);
}


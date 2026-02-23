namespace INKG.CitizenPortal.ApiGataway.Infrastructure;

internal interface IAnonymousRouteValidator
{
    bool HasAccess(string path);
}

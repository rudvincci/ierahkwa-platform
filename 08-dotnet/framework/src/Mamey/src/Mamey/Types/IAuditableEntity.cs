namespace Mamey.Types;

public interface IAuditableEntity: IEntity<UserId>, IAuditable
{
    void UpdateModified(UserId userId);
}
namespace Pupitre.Contracts.Queries;

/// <summary>
/// Marker interface for all Pupitre queries.
/// Queries request information without modifying system state.
/// </summary>
public interface IQuery
{
}

/// <summary>
/// Query that returns a result of type T.
/// </summary>
/// <typeparam name="T">The type of result returned by the query.</typeparam>
public interface IQuery<T> : IQuery
{
}

namespace Pupitre.Contracts.Commands;

/// <summary>
/// Marker interface for all Pupitre commands.
/// Commands represent requests to change the system state.
/// </summary>
public interface ICommand
{
}

/// <summary>
/// Command that returns a result of type T.
/// </summary>
/// <typeparam name="T">The type of result returned by the command.</typeparam>
public interface ICommand<T> : ICommand
{
}

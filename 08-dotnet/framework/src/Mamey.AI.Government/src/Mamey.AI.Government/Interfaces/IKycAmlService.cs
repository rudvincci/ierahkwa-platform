namespace Mamey.AI.Government.Interfaces;

public interface IKycAmlService
{
    Task<bool> ScreenPersonAsync(string fullName, DateTime dateOfBirth, string nationality, CancellationToken cancellationToken = default);
    Task<double> CalculateRiskScoreAsync(object personData, CancellationToken cancellationToken = default);
}

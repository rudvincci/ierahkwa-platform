using Mamey.Types;
using Mamey.Constants;

namespace Mamey.Mock;

public static class TypeMockService
{
    public static async Task<List<Name>> GetNameMockListAsync(int count = 1, Gender? gender = null)
    {
        await Task.Delay(100); // Reduced delay for improved test UX

        return gender switch
        {
            Gender.Female => GenerateFemaleNames(count),
            Gender.Male => GenerateMaleNames(count),
            _ => GenerateNames(count)
        };
    }

    public static Name GenerateName(Gender? gender = null) =>
        gender switch
        {
            Gender.Female => new Name
            {
                FirstName = Names.FemaleFirstNames[Random.Shared.Next(Names.FemaleFirstNames.Length)],
                MiddleName = Names.FemaleMiddleNames[Random.Shared.Next(Names.FemaleMiddleNames.Length)],
                LastName = Names.LastNames[Random.Shared.Next(Names.LastNames.Length)],
                Nickname = Names.FemaleNicknames[Random.Shared.Next(Names.FemaleNicknames.Length)]
            },
            Gender.Male => new Name
            {
                FirstName = Names.MaleFirstNames[Random.Shared.Next(Names.MaleFirstNames.Length)],
                MiddleName = Names.MaleMiddleNames[Random.Shared.Next(Names.MaleMiddleNames.Length)],
                LastName = Names.LastNames[Random.Shared.Next(Names.LastNames.Length)],
                Nickname = Names.MaleNicknames[Random.Shared.Next(Names.MaleNicknames.Length)]
            },
            _ => GenerateName((Gender)Random.Shared.Next(0, 2))
        };

    private static List<Name> GenerateNames(int count)
        => Enumerable.Range(0, count)
            .Select(_ => GenerateName())
            .ToList();

    private static List<Name> GenerateFemaleNames(int count)
        => Enumerable.Range(0, count)
            .Select(_ => GenerateName(Gender.Female))
            .ToList();

    private static List<Name> GenerateMaleNames(int count)
        => Enumerable.Range(0, count)
            .Select(_ => GenerateName(Gender.Male))
            .ToList();

    public static string GenerateEmail(Name name)
        => $"{name.FirstName}.{name.LastName}@example.com".ToLowerInvariant();

    public static string GeneratePhoneNumber()
        => $"555-{Random.Shared.Next(100, 999)}-{Random.Shared.Next(1000, 9999)}";

    public static string GenerateRegion()
    {
        string[] regions =
        {
            "Mohawk Valley", "Seneca Plains", "Onondaga Hills", "Tuscarora Ridge",
            "Cayuga Lakes", "Oneida Forest", "Akwesasne Corridor", "Kahnawake Central"
        };
        return regions[Random.Shared.Next(regions.Length)];
    }

    public static string GenerateVehicleRegistration()
        => $"NY-{Random.Shared.Next(1000, 9999)}-{Random.Shared.Next(1000, 9999)}";

    public static string GenerateLicenseNumber()
        => $"DL-{Random.Shared.Next(100000, 999999)}";

    public static string GenerateKycDocumentUrl(Guid userId)
        => $"/documents/kyc-doc-{userId.ToString().Substring(0, 8)}.pdf";

    public static string GenerateProfileImageUrl(Guid userId)
        => $"/images/profiles/{userId.ToString().Substring(0, 8)}.jpg";

    public static DateTime GenerateDateOfBirth()
        => DateTime.UtcNow.AddYears(-Random.Shared.Next(21, 55)).AddDays(-Random.Shared.Next(0, 365));

    public static DateTime GenerateCreatedAt()
        => DateTime.UtcNow.AddMonths(-Random.Shared.Next(1, 12)).AddDays(-Random.Shared.Next(0, 28));

    public static DateTime GenerateUpdatedAt(DateTime createdAt)
        => createdAt.AddDays(Random.Shared.Next(1, 180));

    public static Gender InferGender(Name name)
    {
        if (Names.MaleFirstNames.Contains(name.FirstName))
            return Gender.Male;
        if (Names.FemaleFirstNames.Contains(name.FirstName))
            return Gender.Female;
        throw new Exception($"Unable to infer gender: {name.FirstName}");
    }

    public static List<string> GenerateAdoptionCertificateNumbers(int count)
        => Enumerable.Range(1, count)
            .Select(_ => AdoptionCertificateNumbers[Random.Shared.Next(AdoptionCertificateNumbers.Length)])
            .ToList();

    private static readonly string[] AdoptionCertificateNumbers = new[]
    {
        "INKMIA-CA-20200425-470", "INKMIA-CA-20181226-436", "INKMIA-CA-20181002-669",
        "INKMIA-CA-20190619-599", "INKMIA-CA-20170220-875", "INKMIA-CA-20221015-980",
        "INKMIA-CA-20230408-168", "INKMIA-CA-20161223-340", "INKMIA-CA-20201015-287",
        "INKMIA-CA-20241005-551", "INKMIA-CA-20180414-250", "INKMIA-CA-20200618-879",
        "INKMIA-CA-20221028-180", "INKMIA-CA-20240220-432", "INKMIA-CA-20180214-994",
        "INKMIA-CA-20190115-606", "INKMIA-CA-20190606-619", "INKMIA-CA-20200226-188",
        "INKMIA-CA-20220716-287", "INKMIA-CA-20170101-016", "INKMIA-CA-20230518-614",
        "INKMIA-CA-20230211-774", "INKMIA-CA-20180323-360", "INKMIA-CA-20191222-994",
        "INKMIA-CA-20220915-523", "INKMIA-CA-20180501-094", "INKMIA-CA-20201117-182",
        "INKMIA-CA-20180901-984", "INKMIA-CA-20221010-518", "INKMIA-CA-20241204-325",
        "INKMIA-CA-20191126-973", "INKMIA-CA-20220702-905", "INKMIA-CA-20230912-730",
        "INKMIA-CA-20230925-356", "INKMIA-CA-20211006-922", "INKMIA-CA-20190526-619",
        "INKMIA-CA-20241126-787", "INKMIA-CA-20180426-334", "INKMIA-CA-20220909-951",
        "INKMIA-CA-20200410-422", "INKMIA-CA-20180514-935", "INKMIA-CA-20221228-229",
        "INKMIA-CA-20230223-163", "INKMIA-CA-20210903-850", "INKMIA-CA-20180721-485",
        "INKMIA-CA-20241107-181", "INKMIA-CA-20220918-737", "INKMIA-CA-20160325-876",
        "INKMIA-CA-20220223-682", "INKMIA-CA-20200724-077"
    };
}

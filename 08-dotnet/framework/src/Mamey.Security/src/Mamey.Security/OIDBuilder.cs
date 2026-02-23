namespace Mamey.Security;

public sealed class OIDBuilder
{
    private readonly string _organizationOid;
    private List<string> _oidParts;
    private readonly OidCollection _oidGenerator;

    public OIDBuilder(string organizationOid, OidCollection oidGenerator)
    {
        _organizationOid = organizationOid;
        _oidParts = new List<string> { _organizationOid };
        _oidGenerator = oidGenerator;
    }

    // Fluent method to handle multiple enums for OID construction
    public OIDBuilder WithOIDs(params Enum[] oidEnums)
    {
        foreach (var enumValue in oidEnums)
        {
            _oidParts.Add(Convert.ToInt32(enumValue).ToString());
        }
        return this;
    }

    // Generate the full OID and return it
    public string Build()
    {
        string branch = string.Join(".", _oidParts);
        return _oidGenerator.GenerateOID(branch);
    }
}
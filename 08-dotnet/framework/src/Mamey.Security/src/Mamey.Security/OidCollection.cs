namespace Mamey.Security;

public class OidCollection
{
    private readonly string _baseOid;
    private readonly Dictionary<string, int> _oidCounters;
    private readonly HashSet<string> _assignedOids;

    public OidCollection(string baseOid)
    {
        if (string.IsNullOrEmpty(baseOid))
        {
            throw new ArgumentException($"'{nameof(baseOid)}' cannot be null or empty.", nameof(baseOid));
        }

        _baseOid = baseOid;
        _oidCounters = new Dictionary<string, int>();
        _assignedOids = new HashSet<string>();
    }

    // Generate a new OID under a given branch
    public string GenerateOID(string branch = "")
    {
        string fullBranch = string.IsNullOrEmpty(branch) ? _baseOid : $"{_baseOid}.{branch}";

        // Increment the counter for the branch
        if (!_oidCounters.ContainsKey(fullBranch))
        {
            _oidCounters[fullBranch] = 1;
        }
        else
        {
            _oidCounters[fullBranch]++;
        }

        // Create the new OID
        string newOid = $"{fullBranch}.{_oidCounters[fullBranch]}";

        // Ensure uniqueness
        while (_assignedOids.Contains(newOid))
        {
            _oidCounters[fullBranch]++;
            newOid = $"{fullBranch}.{_oidCounters[fullBranch]}";
        }

        // Track the new OID
        _assignedOids.Add(newOid);
        return newOid;
    }

    // Assign an existing OID
    public void AssignOID(string oid)
    {
        if (!_assignedOids.Contains(oid))
        {
            _assignedOids.Add(oid);
        }
    }

    // Check if OID is available
    public bool IsAvailable(string oid)
    {
        return !_assignedOids.Contains(oid);
    }

    // Get all assigned OIDs
    public IEnumerable<string> GetAssignedOIDs()
    {
        return _assignedOids;
    }
}

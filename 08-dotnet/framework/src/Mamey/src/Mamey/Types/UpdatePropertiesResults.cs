using Newtonsoft.Json;

namespace Mamey.Types;

public class UpdatePropertiesResults<T>
    where T : class, new()
{
    public UpdatePropertiesResults(T current, Dictionary<string, object> oldValues,
        Dictionary<string, object> newValues)
    {
        HasChanges = oldValues.Any();
        OldValues = oldValues;
        NewValues = newValues;
        Value = current;
    }
    public Dictionary<string, object> OldValues { get; }
    public Dictionary<string, object> NewValues { get; }
    public bool HasChanges { get; }
    public T Value { get; set; }

    public string Changes => HasChanges ?
        $"Profile updated: {JsonConvert.SerializeObject(OldValues)} -> {JsonConvert.SerializeObject(NewValues)}"
        : $"{nameof(T)} not updated";

}
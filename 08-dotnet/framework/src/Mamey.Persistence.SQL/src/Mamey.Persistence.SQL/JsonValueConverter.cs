using System.Text.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Mamey.Persistence.SQL;

public sealed class JsonValueConverter<TObject> : ValueConverter<TObject, string>
{
    public JsonValueConverter() : base(
        convertToProviderExpression: @object => Serialize(@object), 
        convertFromProviderExpression: json => Deserialize(json)) { }
    
    private static TObject Deserialize(string value) => JsonSerializer.Deserialize<TObject>(value)!;
    
    private static string Serialize(TObject value) => JsonSerializer.Serialize(value);
}
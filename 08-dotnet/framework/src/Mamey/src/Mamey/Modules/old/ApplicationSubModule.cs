//using Newtonsoft.Json;

//namespace Mamey.Modules;

//public class ApplicationSubModule<TApplicationSubModuleType> : IEquatable<ApplicationSubModule<TApplicationSubModuleType>>
//    where TApplicationSubModuleType : Enum
//{
//    //public ApplicationSubModule()
//    //    : base()
//    //{

//    //}
//    public ApplicationSubModule(string type,
//        IEnumerable<string>? subModules = null)
//        : this(null, type, subModules, null)
//    { }

//    public ApplicationSubModule(string? name, string type,
//        IEnumerable<string>? subModules = null)
//        : this(name, type, subModules, null)
//    { }

//    [JsonConstructor]
//    public ApplicationSubModule(string? name, string type, IEnumerable<string>? subModules = null,
//        string? url = null)
//    {
//        var submoduleType = type.ToEnum<TApplicationSubModuleType>();
//        Name = string.IsNullOrEmpty(name) ? $"{submoduleType.GetEnumDescriptionAttribute()} Application" : name;
//        Type = type.ToEnum<TApplicationSubModuleType>();
//        SubModules = subModules ?? Enumerable.Empty<string>();
//        Url = url;
//    }

//    public Guid Id { get; private set; } = Guid.NewGuid();

//    [JsonProperty("name")]
//    public string Name { get; set; }

//    [JsonProperty("type")]
//    public TApplicationSubModuleType Type { get; set; }

//    [JsonProperty("url")]
//    public string? Url { get; set; }

//    [JsonProperty("subModules")]
//    public IEnumerable<string> SubModules { get; set; }

//    public bool Active { get; protected set; } = false;
//    public bool Deployed { get; protected set; } = false;

//    protected void Activate()
//    {
//        Active = true;
//    }
//    public void Deactivate()
//    {
//        Active = false;
//    }

//    protected void SetDeployed()
//    {
//        Deployed = true;
//    }

//    public override string ToString()
//    {
//        var value = $"Application Module: '{Name}', Module Type: '{Type.GetEnumDescriptionAttribute()}'{Environment.NewLine}";
//        var subModulesValue = string.Empty;
//        if (SubModules is not null && SubModules.Any())
//        {
//            subModulesValue = $"Sub-Modules:";
//            foreach (var subModule in SubModules)
//            {
//                subModulesValue += $"{Environment.NewLine}Sub Module: '{subModule}'";
//            }
//            value += $"{value}{Environment.NewLine}{subModulesValue}";
//        }
//        return value;
//    }

//    public string ToJson()
//    {
//        return JsonConvert.SerializeObject(this);
//    }

//    #region IEquatable Implementation

//    public bool Equals(ApplicationSubModule<TApplicationSubModuleType>? other)
//    {
//        if (ReferenceEquals(null, other)) return false;
//        return ReferenceEquals(this, other) || (
//            Name.Equals(other.Name) &&
//            Type.Equals(other.Type) &&
//            Url.Equals(other?.Url) &&
//            SubModules.SequenceEqual(other.SubModules)
//            );
//    }

//    public override bool Equals(object? obj)
//    {
//        if (ReferenceEquals(null, obj)) return false;
//        if (ReferenceEquals(this, obj)) return true;
//        return obj.GetType() == GetType() && Equals((ApplicationSubModule<TApplicationSubModuleType>)obj);
//    }

//    public override int GetHashCode() => HashCode.Combine(Name, Type, Url, SubModules);
//    #endregion

//    #region Operators

//    public static bool operator ==(ApplicationSubModule<TApplicationSubModuleType> a, ApplicationSubModule<TApplicationSubModuleType> b)
//     => a.Equals(b);

//    public static bool operator !=(ApplicationSubModule<TApplicationSubModuleType> a, ApplicationSubModule<TApplicationSubModuleType> b)
//        => !(a == b);
//    #endregion
//}
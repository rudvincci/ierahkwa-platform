using Newtonsoft.Json;

namespace Mamey.Modules;

//public class ApplicationModule<TApplicationModuleType, TApplicationModuleType>
//    : IEquatable<ApplicationModule<TApplicationModuleType>>
//    where TApplicationModuleType : Enum
//{
//    public ApplicationModule()
//        : this("")
//    {
//    }

//    public ApplicationModule(string type)
//        : this(null, type)
//    {
//    }
//    public ApplicationModule(string name, string type)
//        : this(name, type, null)
//    {
//    }

//    [JsonConstructor]
//    public ApplicationModule(string name, string type, string? url = null)
//    {
//        Type = type.ToEnum<TApplicationModuleType>();
//        if (string.IsNullOrEmpty(name))
//        {
//            var descriptionAttr = Type.GetEnumDescriptionAttribute();
//            Name = descriptionAttr;
//        }
//        else
//        {
//            Name = name;
//        }
//        Url = url;
//    }

//    #region Properties

//    [JsonProperty("name")]
//    public string Name { get; set; }

//    [JsonProperty("type")]
//    public TApplicationModuleType Type { get; set; }

//    [JsonProperty("url")]
//    public string? Url { get; set; }
//    [JsonProperty("subModules")]
//    public IEnumerable<ApplicationModule<TApplicationSubModuleType>> SubModules { get; set; }
//    #endregion

//    public void SetUrl(string? url)
//    {
//        // 
//        Url = url;
//    }

//    public virtual string ToJson()
//    {
//        return JsonConvert.SerializeObject(this);
//    }
//    public override string ToString()
//    {
//        return $"Application Module: '{Name}', Module Type: '{Type.GetEnumDescriptionAttribute()}'";
//    }

//    #region IEquatable Implementation

//    public bool Equals(ApplicationModule<TApplicationModuleType>? other)
//    {
//        if (ReferenceEquals(null, other)) return false;
//        return ReferenceEquals(this, other) || (
//            Name.Equals(other.Name) &&
//            Type.Equals(other.Type) &&
//            Url.Equals(other.Url)
//            );
//    }

//    public override bool Equals(object obj)
//    {
//        if (ReferenceEquals(null, obj)) return false;
//        if (ReferenceEquals(this, obj)) return true;
//        return obj.GetType() == GetType() && Equals((ApplicationModule<TApplicationModuleType>)obj);
//    }

//    public override int GetHashCode() => HashCode.Combine(Name, Type, Url);
//    #endregion

//    #region Operator Overides

//    public static bool operator ==(ApplicationModule<TApplicationModuleType> a, ApplicationModule<TApplicationModuleType> b)
//    {
//        return a.Equals(b);
//    }

//    public static bool operator !=(ApplicationModule<TApplicationModuleType> a, ApplicationModule<TApplicationModuleType> b) => !(a == b);

//    #endregion
//}

public class ApplicationModule<TApplicationModuleType, TApplicationSubModuleType>
    //: ApplicationModule<TApplicationModuleType>, IEquatable<ApplicationModule<TApplicationModuleType, TApplicationSubModuleType>>
    where TApplicationModuleType : Enum
    where TApplicationSubModuleType : Enum
{
    public ApplicationModule()
        : base()
    {
        
    }
    public ApplicationModule(TApplicationModuleType type,
        IEnumerable<ApplicationSubModule<TApplicationSubModuleType>>? subModules = null)
        : this(null, type, null, null)
    { }

    public ApplicationModule(string? name, TApplicationModuleType type,
        IEnumerable<ApplicationSubModule<TApplicationSubModuleType>>? subModules = null)
        : this(name, type, null, null)
    { }

    [Newtonsoft.Json.JsonConstructor]
    public ApplicationModule(string? name, TApplicationModuleType type, IEnumerable<ApplicationSubModule<TApplicationSubModuleType>>? subModules = null,
        string? url = null)
        => (Name, Type, SubModules, Url) =
            (name, type,
            subModules ?? Enumerable.Empty<ApplicationSubModule<TApplicationSubModuleType>>(),
            url);

    #region Properties

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("type")]
    public TApplicationModuleType Type { get; set; }

    [JsonProperty("url")]
    public string? Url { get; set; }

    [JsonProperty("subModules")]
    public IEnumerable<ApplicationSubModule<TApplicationSubModuleType>> SubModules { get; set; }
    #endregion

    public override string ToString()
    {
        var value = $"Application Module: '{Name}', Module Type: '{Type.GetEnumDescriptionAttribute()}'{Environment.NewLine}";
        var subModulesValue = string.Empty;
        if (SubModules is not null && SubModules.Any())
        {
            subModulesValue = $"Sub-Modules:";
            foreach (var subModule in SubModules)
            {
                subModulesValue += $"{Environment.NewLine}Sub Module: {subModule.Name}, Sub Module Type: '{Type.GetEnumDescriptionAttribute()}'";
            }
            value += $"{value}{Environment.NewLine}{subModulesValue}";
        }
        return value;
    }

    public string ToJson()
    {
        return JsonConvert.SerializeObject(this);
    }

    #region IEquatable Implementation

    public bool Equals(ApplicationModule<TApplicationModuleType, TApplicationSubModuleType>? other)
    {
        if (ReferenceEquals(null, other)) return false;
        return ReferenceEquals(this, other) || (
            Name.Equals(other.Name) &&
            Type.Equals(other.Type) &&
            Url.Equals(other.Url) &&
            SubModules.SequenceEqual(other.SubModules)
            );
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((ApplicationModule<TApplicationModuleType, TApplicationSubModuleType>)obj);
    }

    public override int GetHashCode() => HashCode.Combine(Name, Type, Url, SubModules);
    #endregion

    #region Operators

    public static bool operator ==(ApplicationModule<TApplicationModuleType, TApplicationSubModuleType> a, ApplicationModule<TApplicationModuleType, TApplicationSubModuleType> b)
     => a.Equals(b);

    public static bool operator !=(ApplicationModule<TApplicationModuleType, TApplicationSubModuleType> a, ApplicationModule<TApplicationModuleType, TApplicationSubModuleType> b)
        => !(a == b);
    #endregion
}
public class ApplicationSubModule<TApplicationSubModuleType> : IEquatable<ApplicationSubModule<TApplicationSubModuleType>>
    where TApplicationSubModuleType : Enum
{
    public ApplicationSubModule()
        : base()
    {

    }
    public ApplicationSubModule(TApplicationSubModuleType type,
        IEnumerable<string>? subModules = null)
        : this(null, type, subModules, null)
    { }

    public ApplicationSubModule(string? name, TApplicationSubModuleType type,
        IEnumerable<string>? subModules = null)
        : this(name, type, subModules, null)
    { }

    [Newtonsoft.Json.JsonConstructor]
    public ApplicationSubModule(string? name, TApplicationSubModuleType type, IEnumerable<string>? subModules = null,
        string? url = null)
        => (Name, Type, SubModules, Url) =
            (name, type,
            subModules ?? Enumerable.Empty<string>(),
            url);

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("type")]
    public TApplicationSubModuleType Type { get; set; }

    [JsonProperty("url")]
    public string? Url { get; set; }

    [JsonProperty("subModules")]
    public IEnumerable<string> SubModules { get; set; }

    public override string ToString()
    {
        var value = $"Application Module: '{Name}', Module Type: '{Type.GetEnumDescriptionAttribute()}'{Environment.NewLine}";
        var subModulesValue = string.Empty;
        if (SubModules is not null && SubModules.Any())
        {
            subModulesValue = $"Sub-Modules:";
            foreach (var subModule in SubModules)
            {
                subModulesValue += $"{Environment.NewLine}Sub Module: '{subModule}'";
            }
            value += $"{value}{Environment.NewLine}{subModulesValue}";
        }
        return value;
    }

    public string ToJson()
    {
        return JsonConvert.SerializeObject(this);
    }

    #region IEquatable Implementation

    public bool Equals(ApplicationSubModule<TApplicationSubModuleType>? other)
    {
        if (ReferenceEquals(null, other)) return false;
        return ReferenceEquals(this, other) || (
            Name.Equals(other.Name) &&
            Type.Equals(other.Type) &&
            Url.Equals(other.Url) &&
            SubModules.SequenceEqual(other.SubModules)
            );
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((ApplicationSubModule<TApplicationSubModuleType>)obj);
    }

    public override int GetHashCode() => HashCode.Combine(Name, Type, Url, SubModules);
    #endregion

    #region Operators

    public static bool operator ==(ApplicationSubModule<TApplicationSubModuleType> a, ApplicationSubModule<TApplicationSubModuleType> b)
     => a.Equals(b);

    public static bool operator !=(ApplicationSubModule<TApplicationSubModuleType> a, ApplicationSubModule<TApplicationSubModuleType> b)
        => !(a == b);
    #endregion
}
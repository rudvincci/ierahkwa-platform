namespace Mamey.Modules;

public class MameyApplicationModule
    : ApplicationModule<MameyApplicationType, MameyAppModuleType>
    , IEquatable<MameyApplicationModule>
{
    public MameyApplicationModule()
        : base(MameyApplicationType.None)
    {

    }
    public MameyApplicationModule(MameyApplicationType type, IEnumerable<ApplicationSubModule<MameyAppModuleType>>? subModules = null)
        : base(type, subModules)
    {

    }

    [Newtonsoft.Json.JsonConstructor]
    public MameyApplicationModule(string? name, MameyApplicationType type,
        IEnumerable<ApplicationSubModule<MameyAppModuleType>>? subModules,
        string? url = null)
        : base(name, type, subModules, url)
    {
        
    }

    #region IEquatable Implementation

    public bool Equals(MameyApplicationModule? other)
    {
        if (ReferenceEquals(null, other)) return false;
        return ReferenceEquals(this, other);
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((MameyApplicationModule)obj);
    }

    public override int GetHashCode() => base.GetHashCode();
    #endregion

    #region Operator Overides

    public static bool operator ==(MameyApplicationModule a, MameyApplicationModule b)
     => a.Equals(b);

    public static bool operator !=(MameyApplicationModule a, MameyApplicationModule b)
        => !(a == b);
    
    #endregion
}
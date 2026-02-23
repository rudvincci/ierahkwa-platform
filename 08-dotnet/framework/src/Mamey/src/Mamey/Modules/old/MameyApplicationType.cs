namespace Mamey.Modules;

[Flags]
public enum MameyApplicationType : long
{
    [Description("Portal")]
    None = 1 << 0,
    [Description("Portal")]
    Portal = 1 << 1,
    [Description("Web API")]
    WebApi = 1 << 2,
    [Description("Website")]
    Website = 1 << 3,
    [Description("Daemon")]
    Daemon =  1 << 4,
    [Description("Mobile")]
    Mobile = 1 << 5,
    [Description("Pupitre K-12")]
    PupitreK12 = 1 << 6,
    [Description("Pupitre University")]
    PupitreU = 1 << 7,
    [Description("Pupitre Enterprise")]
    PupitreE = 1 << 8,
    [Description("Woofy")]
    Woofy = 1 << 9,
    [Description("IntelliCare")]
    IntelliCare = 1 << 10,
    [Description("ATM Machine")]
    ATMMachine = 1 << 11,
}
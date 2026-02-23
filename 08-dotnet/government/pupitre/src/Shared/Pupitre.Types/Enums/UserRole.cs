namespace Pupitre.Types.Enums;

/// <summary>
/// User roles in the Pupitre platform.
/// </summary>
public enum UserRole
{
    /// <summary>Student - primary learner</summary>
    Student = 1,

    /// <summary>Parent or guardian of a student</summary>
    Parent = 2,

    /// <summary>Legal guardian (non-parent)</summary>
    Guardian = 3,

    /// <summary>Teacher or educator</summary>
    Educator = 4,

    /// <summary>Platform administrator</summary>
    Admin = 5,

    /// <summary>Ministry or government official</summary>
    Ministry = 6
}

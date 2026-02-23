namespace Mamey.Identity.AspNetCore.ValueObjects;

/// <summary>
/// Represents how a user authenticated (e.g., Password, OTP, Certificate).
/// </summary>
public readonly record struct AuthenticationMethod(string Value)
{
    public static AuthenticationMethod Password    => new("Password");
    public static AuthenticationMethod OTP         => new("OTP");
    public static AuthenticationMethod Certificate => new("Certificate");
    public static AuthenticationMethod External    => new("External");

    public override string ToString() => Value;
    public static implicit operator string(AuthenticationMethod am) => am.Value;
}


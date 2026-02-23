namespace Mamey.Exceptions;

public class UnsupportedNationalityException : MameyException
{
    public string Nationality { get; }

    public UnsupportedNationalityException(string nationality) : base($"Nationality: '{nationality}' is unsupported.")
    {
        Nationality = nationality;
    }
}
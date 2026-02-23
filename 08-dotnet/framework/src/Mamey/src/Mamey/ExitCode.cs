namespace Mamey;

public enum ExitCode : int
{
    Success = 0,
    InvalidLogin = 1,
    InvalidFilename = 2,
    AppConfigError = 3,
    UnknownError = 10
}


using System.ComponentModel.DataAnnotations;

namespace Mamey.Auth.Jwt.BlazorWasm.Requests;

public class ResetPasswordRequest
{
    [Required]
    public string Token { get; set; }

    [Required]
    public string Password { get; set; }
}

using System.ComponentModel.DataAnnotations;

namespace Mamey.Auth.Jwt.BlazorWasm.Requests;
public class RecoverPasswordRequest
{
    [Required]
    public string Email { get; set; }
}

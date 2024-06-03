using System.ComponentModel.DataAnnotations;

namespace JwtAuth_NetCode_Hub.Data.Dtos
{
    public class LoginDto
    {

        [Required(ErrorMessage = "UserNameIs Required")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "Password Is Required")]
        public string? Password { get; set; }
    }
}

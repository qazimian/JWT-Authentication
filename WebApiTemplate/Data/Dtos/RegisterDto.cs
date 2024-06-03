using System.ComponentModel.DataAnnotations;

namespace JwtAuth_NetCode_Hub.Data.Dtos
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "UserNameIs Required")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "UserNameIs Required")]
        public string? LastName { get; set; }

        [Required (ErrorMessage ="UserNameIs Required")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "Email Is Required")]
        public string? Email{ get; set; }

        [Required(ErrorMessage = "Password Is Required")]
        public string? Password { get; set; }

    }
}

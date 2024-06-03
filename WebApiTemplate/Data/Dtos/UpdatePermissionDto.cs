using System.ComponentModel.DataAnnotations;

namespace JwtAuth_NetCode_Hub.Data.Dtos
{
    public class UpdatePermissionDto
    {

        [Required(ErrorMessage = "UserNameIs Required")]
        public string? UserName { get; set; }

    }
}

using Microsoft.AspNetCore.Identity;

namespace JwtAuth_NetCode_Hub.Data
{
    public class ApplicationUser : IdentityUser
    {

        public string? FirstName{ get; set; }
        public string? LastName { get; set; }
    }
}

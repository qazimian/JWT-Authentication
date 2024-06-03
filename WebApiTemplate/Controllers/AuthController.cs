using Azure.Core;
using JwtAuth_NetCode_Hub.Data;
using JwtAuth_NetCode_Hub.Data.Dtos;
using JwtAuth_NetCode_Hub.Data.OtherObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JwtAuth_NetCode_Hub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        public AuthController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        //Route for seeding my roles to DB
        [HttpPost]
        [Route("Seeds-roles")]
        public async Task<IActionResult> SeedRoles()
        {
            bool isOwnerRolesExists = await _roleManager.RoleExistsAsync(StaticUserRoles.OWNER);
            bool isAdminRolesExists = await _roleManager.RoleExistsAsync(StaticUserRoles.ADMIN);
            bool isUserRolesExists = await _roleManager.RoleExistsAsync(StaticUserRoles.USER);
            if (isOwnerRolesExists && isAdminRolesExists && isUserRolesExists)
            {
                return Ok("roles seeding is already done");
            }
            else
            {
                await _roleManager.CreateAsync(new IdentityRole(StaticUserRoles.USER));
                await _roleManager.CreateAsync(new IdentityRole(StaticUserRoles.ADMIN));
                await _roleManager.CreateAsync(new IdentityRole(StaticUserRoles.OWNER));
                return Ok("Role Seeding Done successfully ");
            }


        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var isExistsUser = await _userManager.FindByNameAsync(registerDto.UserName);
            if (isExistsUser != null)
            {
                return BadRequest("Username already exists");
            }

            ApplicationUser newUser = new ApplicationUser()
            {
                FirstName = registerDto.FirstName, 
                LastName = registerDto.LastName,
                Email = registerDto.Email,
                UserName = registerDto.UserName,
                SecurityStamp = Guid.NewGuid().ToString(),
            };
            var createUserResult = await _userManager.CreateAsync(newUser, registerDto.Password);
            if (!createUserResult.Succeeded)
            {
                var errorString = "User creation failed because:";
                foreach (var error in createUserResult.Errors)
                {
                    errorString += " # " + error.Description;
                }
                return BadRequest(errorString);
            }

            await _userManager.AddToRoleAsync(newUser, StaticUserRoles.USER);
            return Ok("User created successfully");
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var user = await _userManager.FindByNameAsync(loginDto.UserName);
            if (user == null)
                return Unauthorized("Invaluid Credentials");

            var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (isPasswordCorrect == null)
                return Unauthorized("Invaluid Credentials");

            var userRoles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.UserName),
                new Claim("JWTID",Guid.NewGuid().ToString()),
            };
            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var token = GenerateNewJsonWebToken(authClaims);
            return Ok(token);
        }
        private string GenerateNewJsonWebToken(List<Claim> claims)
        {
            var authSecret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]));
            var tokenObject = new JwtSecurityToken(
                issuer: _configuration["Jwt:ValidIssuer"],
                audience: _configuration["Jwt:ValidAudience"],
                expires: DateTime.Now.AddHours(1),
                claims: claims,
                signingCredentials: new SigningCredentials(authSecret, SecurityAlgorithms.HmacSha256)
                );

            string token = new JwtSecurityTokenHandler().WriteToken(tokenObject);
            return token;

        }
    }
}

//ValidateIssuer = true,
//            ValidateAudience = true,
//            ValidIssuer = builder.Configuration["Jwt:Issuer"],
//            ValidAudience = builder.Configuration["Jwt:Audience"],
//            ValidateIssuerSigningKey = true,
//            ValidateLifetime = true,
//            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!))

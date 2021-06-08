using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MovieTheater.DTOs;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MovieTheater.Controllers
{
    [ApiController]
    [Route("api/accounts")]
    public class AccountsController : CustomBaseController
    {
        private readonly MovieTheaterDbContext context;
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly IConfiguration configuration;

        public AccountsController(
            MovieTheaterDbContext context, 
            IMapper mapper,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IConfiguration configuration)
            : base(context, mapper)
        {
            this.context = context;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.configuration = configuration;
        }

        [HttpPost("create")]
        public async Task<ActionResult<UserTokenDTO>> CreateUser([FromBody] UserInfoDTO userInfo)
        {
            var user = new IdentityUser { UserName = userInfo.Email, Email = userInfo.Email };
            var result = await userManager.CreateAsync(user, userInfo.Password);
            if (!result.Succeeded) return BadRequest(result.Errors);
            return await BuildToken(userInfo);
        }
        [HttpPost("login")]
        public async Task<ActionResult<UserTokenDTO>> Login([FromBody] UserInfoDTO userInfo)
        {
            var result = await signInManager.PasswordSignInAsync(userInfo.Email,
                userInfo.Password, isPersistent: false, lockoutOnFailure: false);
            if (!result.Succeeded) return BadRequest("Invalid login attempt");
            return await BuildToken(userInfo);

        }

        [HttpGet("users")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult<List<UserDTO>>> Get([FromQuery] PaginationDTO pagination)
        {
            return await Get<IdentityUser, UserDTO>(pagination);
        }

        [HttpGet("roles")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult<List<string>>> GetRoles()
        {
            return await context.Roles.Select(u => u.Name).ToListAsync();
        }

        [HttpPost("assignRole")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> AssignRole(RoleEditDTO roleEditDTO)
        {
            var user = await userManager.FindByIdAsync(roleEditDTO.UserId);
            if (user == null) return NotFound();
            await userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, roleEditDTO.RoleName));
            return NoContent();
        }

        [HttpPost("removeRole")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> RemoveRole(RoleEditDTO roleEditDTO)
        {
            var user = await userManager.FindByIdAsync(roleEditDTO.UserId);
            if (user == null) return NotFound();
            await userManager.RemoveClaimAsync(user, new Claim(ClaimTypes.Role, roleEditDTO.RoleName));
            return NoContent();
        }

        [HttpPost("renovateToken")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<UserTokenDTO>> Renovate()
        {
            var userInfo = new UserInfoDTO { Email = HttpContext.User.Identity.Name };
            return await BuildToken(userInfo);
        }

        private async Task<UserTokenDTO> BuildToken(UserInfoDTO userInfo)
        {
            var claims = new List<Claim> 
            { 
                new Claim(ClaimTypes.Name, userInfo.Email),
                new Claim(ClaimTypes.Email, userInfo.Email),
            };
            var user = await userManager.FindByEmailAsync(userInfo.Email);
            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
            var claimsDb = await userManager.GetClaimsAsync(user);
            claims.AddRange(claimsDb);
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["jwt:key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddYears(1);
            var token = new JwtSecurityToken(issuer: null, audience: null, claims: claims, expires: expiration, signingCredentials: creds);
            return new UserTokenDTO
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expiration
            };
        }
    }
}

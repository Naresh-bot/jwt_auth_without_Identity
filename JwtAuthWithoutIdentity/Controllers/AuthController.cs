using JwtAuthWithoutIdentity.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JwtAuthWithoutIdentity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public readonly IConfiguration _configuration;
        public readonly AppDbContext _dbContext;
        public AuthController(IConfiguration configuration, AppDbContext dbContext)
        {
            _configuration = configuration;
            _dbContext = dbContext;
            if(!_dbContext.Users.Any(x=>x.Id == 1))
            {
                _dbContext.Users.Add(new Models.User
                {
                    Id = 1,
                    Name = "TestUser",
                    Password = "google",
                    Role = "User"
                });
                _dbContext.SaveChanges();
            }
            
        }

        
        [HttpPost("/Register")]
        public IActionResult Register(User user)
        {
            if (!ModelState.IsValid) {
                return BadRequest("Insufficient data");
            }
            if(ModelState.IsValid &&  _dbContext.Users.Any(u => u.Id == user.Id))
            {
                return BadRequest("User already exist");
            }

            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();

            return Ok(new {message="User registered successfully"});
        }

        [HttpPost("/Login")]
        public IActionResult Login(User user)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest();
            //}

            var userList = _dbContext.Users;

            if(_dbContext.Users
                .Any(x => x.Name == user.Name && x.Password == user.Password))
            {
                user = _dbContext.Users.Where(x => x.Name == user.Name && x.Password == user.Password).First();

                Claim[] authClaims = new Claim[]{
                    new Claim(ClaimTypes.NameIdentifier, user.Name),
                    new Claim(ClaimTypes.Role, user.Role)
                };

                var token = new JwtSecurityToken(
                    issuer: _configuration["JWT:ValidIssuer"],
                    audience: _configuration["JWT:ValidAudience"],
                    expires: DateTime.Now.AddMinutes(double.Parse(_configuration["JWT:ExpiryMinutes"]!)),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]!)), SecurityAlgorithms.HmacSha256) 
                );

                return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
            }
              
            return BadRequest("Invalid credentials..!");  
        }

    }
}

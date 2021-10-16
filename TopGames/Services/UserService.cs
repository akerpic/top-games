using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TopGames.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
namespace TopGames.Services
{
    public interface IUserService
    {
        AuthenticateResponse Authenticate(AuthenticateRequest model);
    }

    public class UserService : IUserService
    {
        public List<User> testUsers = new List<User> { new User() { Username = "Test", Password = "Test", Email = "test@test.com" } };
        public IConfiguration Configuration { get; }

        public UserService(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private (bool result, User user) IsAuthenticated(AuthenticateRequest model)
        {
            foreach(var testUser in testUsers)
            {
                if (testUser.Username == model.Username && testUser.Password == model.Password)
                    return (result: true, user: testUser);
            }
            return (result:false, user:null);
        }

        public AuthenticateResponse Authenticate(AuthenticateRequest model)
        {

            var isAuthenticated = IsAuthenticated(model);
            if (isAuthenticated.result)
            {
                var token = GenerateJwtToken(isAuthenticated.user);
                return new AuthenticateResponse(isAuthenticated.user, token);
            }

            return null;
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Configuration.GetSection("AppSettings").GetSection("Secret").Value);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("Username", user.Username), new Claim("Email", user.Email) }),
                Expires = DateTime.UtcNow.AddMinutes(30),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}

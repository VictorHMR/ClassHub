using ClassHub.ClassHubContext.Models;
using ClassHub.Dtos.Users;
using ClassHub.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ClassHub.ClassHubContext.Services
{
    public class UserService
    {
        private readonly ClassHubDbContext _db;
        private readonly PasswordHasher<User> _hasher;
        private readonly IConfiguration _configuration;


        public UserService(ClassHubDbContext db, IConfiguration configuration)
        {
            _db = db;
            _hasher = new PasswordHasher<User>();
            _configuration = configuration;


        }

        public async Task<User> CriarUserAsync(CreateUserRequestDTO newUser)
        {
            var usuario = new User
            {
                Nome = newUser.Nome,
                Email = newUser.Email,
                CPF = newUser.CPF,
                Role = newUser.Role
            };

            usuario.Senha = _hasher.HashPassword(usuario, newUser.Senha);
            usuario.RA = $"{DateTime.Now.Year}{usuario.Id:D3}";
            _db.Users.Add(usuario);

            await _db.SaveChangesAsync();

            return usuario;
        }

        public async Task<User?> ObterUserAsync(LoginRequestDTO LoginRequest)
        {
            var usuario = await _db.Users
                .Where(u => u.Email == LoginRequest.Login|| u.RA == LoginRequest.Login)
                .FirstOrDefaultAsync();

            if (usuario == null) return null;

            var result = _hasher.VerifyHashedPassword(usuario, usuario.Senha, LoginRequest.Senha);
            return result == PasswordVerificationResult.Success ? usuario : null;
        }

        public string GerarToken(User user)
        {
            var jwtSection = _configuration.GetSection("Jwt");
            var key = Encoding.ASCII.GetBytes(jwtSection["Key"]);
            var expireHours = Convert.ToDouble(jwtSection["ExpireHours"]);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Nome),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(expireHours),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}

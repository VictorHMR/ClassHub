using ClassHub.ClassHubContext.Models;
using ClassHub.Dtos.Usuario;
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
    public class UsuarioService
    {
        private readonly ClassHubDbContext _db;
        private readonly PasswordHasher<Usuario> _hasher;
        private readonly IConfiguration _configuration;


        public UsuarioService(ClassHubDbContext db, IConfiguration configuration)
        {
            _db = db;
            _hasher = new PasswordHasher<Usuario>();
            _configuration = configuration;
        }

        public async Task<Usuario> CriarUsuarioAsync(CriarUsuarioRequestDTO novoUsuario)
        {
            var usuario = new Usuario
            {
                Nome = novoUsuario.Nome,
                Email = novoUsuario.Email,
                CPF = novoUsuario.CPF,
                TipoUsuario = novoUsuario.TipoUsuario,
                RA = string.Empty
            };

            usuario.Senha = _hasher.HashPassword(usuario, novoUsuario.Senha);
            _db.Usuarios.Add(usuario);
            await _db.SaveChangesAsync();
            usuario.RA = usuario.TipoUsuario != TipoUsuario.Aluno ? usuario.Email : $"{DateTime.Now.Year}{usuario.Id:D3}";
            await _db.SaveChangesAsync();


            return usuario;
        }

        public async Task<Usuario?> ObterUsuarioAsync(LoginRequestDTO LoginRequest)
        {
            var usuario = await _db.Usuarios
                .Where(u => u.Email == LoginRequest.Login|| u.RA == LoginRequest.Login)
                .FirstOrDefaultAsync();

            if (usuario == null) return null;

            var result = _hasher.VerifyHashedPassword(usuario, usuario.Senha, LoginRequest.Senha);
            return result == PasswordVerificationResult.Success ? usuario : null;
        }

        public string GerarToken(Usuario usuario)
        {
            var jwtSection = _configuration.GetSection("Jwt");
            var key = Encoding.ASCII.GetBytes(jwtSection["Key"]);
            var expireHours = Convert.ToDouble(jwtSection["ExpireHours"]);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.Nome),
                new Claim(ClaimTypes.Role, usuario.TipoUsuario.ToString())
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

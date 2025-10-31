using ClassHub.ClassHubContext.Models;
using ClassHub.Dtos.Turma;
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

        public async Task<LoginResponseDTO?> ObterUsuarioAsync(LoginRequestDTO LoginRequest)
        {
            var usuario = await _db.Usuarios
                .Where(u => u.Email == LoginRequest.Login|| u.RA == LoginRequest.Login)
                .FirstOrDefaultAsync();

            if (usuario == null) return null;

            var result = _hasher.VerifyHashedPassword(usuario, usuario.Senha, LoginRequest.Senha);

            return result == PasswordVerificationResult.Success ? new LoginResponseDTO
            {
                IdUsuario = usuario.Id,
                Token = GerarToken(usuario),
                Nome = usuario.Nome,
                CPF = usuario.CPF,
                Email = usuario.Email,
                TipoUsuario = usuario.TipoUsuario
            } : null;
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

        public async Task<Usuario?> LoginAsync(string login, string senha)
        {
            var usuario = await _db.Usuarios.FirstOrDefaultAsync(u => u.Email == login || u.RA == login);

            if (usuario == null)
                return null;

            var result = _hasher.VerifyHashedPassword(usuario, usuario.Senha, senha);
            return result == PasswordVerificationResult.Success ? usuario : null;
        }

        public async Task<PaginacaoResult<UsuarioDTO>> ListarUsuarios(ListarUsuarioRequestDTO request)
        {
            if (request.nrPagina < 1) request.nrPagina = 1;
            if (request.qtRegistros < 1) request.qtRegistros = 10;
            var query = _db.Usuarios
                .Include(x => x.Matriculas)
                .AsQueryable();

            if(request.idTurma != null)
                query = query.Where(u => u.Matriculas.Any(m => m.IdTurma == request.idTurma));

            if (!string.IsNullOrEmpty(request.pesquisa))
                query = query.Where(u => u.Nome.StartsWith(request.pesquisa));

            if(request.tipoUsuario != null)
                query = query.Where(u => u.TipoUsuario == request.tipoUsuario);

            if (request.ordenacao == Filtros.Ordenacao.Ascendente)
                query = query.OrderBy(t => t.Nome);
            else
                query = query.OrderByDescending(t => t.Nome);

            var total = await query.CountAsync();

            var usuarios = await query
                .Skip((request.nrPagina - 1) * request.qtRegistros)
                .Take(request.qtRegistros)
                .Select(t => new UsuarioDTO
                {
                    IdUsuario = t.Id,
                    Nome = t.Nome,
                    Email = t.Email,
                    RA = t.RA,
                    TipoUsuario = t.TipoUsuario
                }).ToListAsync();

            return new PaginacaoResult<UsuarioDTO>
            {
                Itens = usuarios,
                PaginaAtual = request.nrPagina,
                TamanhoPagina = request.qtRegistros,
                TotalItens = total,
                TotalPaginas = (int)Math.Ceiling(total / (double)request.qtRegistros)
            };
        }

    }
}

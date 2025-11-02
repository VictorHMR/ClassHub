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

        public async Task<int> CriarUsuarioAsync(CriarUsuarioRequestDTO novoUsuario)
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


            return usuario.Id;
        }

        public async Task<LoginResponseDTO?> ObterUsuarioAsync(LoginRequestDTO LoginRequest)
        {
            var usuario = await _db.Usuarios
                .Where(u => u.Email == LoginRequest.Login || u.RA == LoginRequest.Login)
                .FirstOrDefaultAsync();

            if (usuario == null || string.IsNullOrEmpty(usuario.RA))
            {
                return null;
            }

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
            if (usuario == null || string.IsNullOrEmpty(usuario.RA))
            {
                throw new ArgumentException("O RA do usuário não pode ser nulo ou vazio.");
            }

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
                .Include(y=> y.TurmasLecionadas)
                .AsQueryable();

            if(request.idTurma != null)
            {

                query = query.Where(u => u.Matriculas.Any(m => m.IdTurma == request.idTurma) || u.TurmasLecionadas.Any(x=> x.Id == request.idTurma));
            }

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

        public async Task DeletarUsuario(int idUsuario)
        {
            var usuario = await _db.Usuarios.Include(u => u.Matriculas).Include(u=> u.TurmasLecionadas).FirstOrDefaultAsync(u => u.Id == idUsuario);
            if (usuario.Matriculas.Any() || (usuario.TurmasLecionadas?.Any() ?? true))
                throw new Exception("Não é possivel remover o usuário, será necessário remover seu vinculo com suas turmas primeiro.");
            _db.Usuarios.Remove(usuario);
            await _db.SaveChangesAsync();
        }

        public async Task<List<UsuarioDTO>> ListarProfessores()
        {
            return await _db.Usuarios.Where(u => u.TipoUsuario == TipoUsuario.Professor).Select(u=> new UsuarioDTO { IdUsuario = u.Id, Nome= u.Nome, Email= u.Email, RA = u.RA, TipoUsuario = u.TipoUsuario}).ToListAsync();
        }

        public async Task<UsuarioDTO?> ObterUsuarioPorId(int idUsuario)
        {
            var usuario = await _db.Usuarios.FirstOrDefaultAsync(u => u.Id == idUsuario);

            if (usuario == null) return null;

            return new UsuarioDTO
            {
                IdUsuario = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                RA = usuario.RA,
                CPF = usuario.CPF,
                TipoUsuario = usuario.TipoUsuario
            };
        }

        public async Task EditarUsuario(EditarUsuarioRequestDTO editarUsuario)
        {
            var usuario =  await _db.Usuarios.FirstOrDefaultAsync(u => u.Id == editarUsuario.Id);
            if (usuario == null) throw new Exception("Usuário não encontrado.");
            usuario.Nome = editarUsuario.Nome;
            usuario.Email = editarUsuario.Email;
            usuario.CPF = editarUsuario.CPF;
            if (!string.IsNullOrEmpty(editarUsuario.Senha))
            {
                usuario.Senha = _hasher.HashPassword(usuario, editarUsuario.Senha);
            }
            await _db.SaveChangesAsync();
        }
    }
}

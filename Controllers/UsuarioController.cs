using ClassHub.ClassHubContext.Services;
using ClassHub.Dtos.Usuario;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClassHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class UsuarioController : ControllerBase
    {
        private readonly UsuarioService _usuarioService;
        public UsuarioController(UsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }
        /// <summary>
        /// Fornece um token de autenticação baseado no email e senha fornecidos.
        /// </summary>
        /// <param name="login">Informações de login do usuário</param>
        /// <returns>JWT Token do usuário</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO login)
        {
            var usuario = await _usuarioService.ObterUsuarioAsync(login);
            if (usuario == null) return Unauthorized("Login ou senha inválidos");

            return Ok(new LoginResponseDTO
            {
                Token = _usuarioService.GerarToken(usuario),
                Nome = usuario.Nome,
                CPF = usuario.CPF,
                Email = usuario.Email,
                TipoUsuario = usuario.TipoUsuario
            });
        }

        /// <summary>
        /// Cria um usuário a partir de informações recebidas, apenas admins podem criar
        /// </summary>
        /// <param name="novoUsuario">Informações do usuário a ser criado</param>
#if !DEBUG
        [Authorize(Roles = "Admin")]
#endif
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CriarUsuarioRequestDTO novoUsuario)
        {
            var usuario = await _usuarioService.CriarUsuarioAsync(novoUsuario);
            return Ok();
        }

    }
}

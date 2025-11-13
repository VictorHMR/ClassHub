using ClassHub.ClassHubContext.Models;
using ClassHub.ClassHubContext.Services;
using ClassHub.Dtos.Usuario;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
        [ProducesResponseType(typeof(LoginResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO login)
        {
            var usuario = await _usuarioService.ObterUsuarioAsync(login);
            if (usuario == null) return Unauthorized("Login ou senha inválidos");

            return Ok(usuario);
        }

        /// <summary>
        /// Cria um usuário a partir de informações recebidas, apenas admins podem criar
        /// </summary>
        /// <param name="novoUsuario">Informações do usuário a ser criado</param>
#if !DEBUG
        [Authorize(Roles = "Admin", AuthenticationSchemes =JwtBearerDefaults.AuthenticationScheme)]
#endif
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Create([FromBody] CriarUsuarioRequestDTO novoUsuario)
        {
            var usuario = await _usuarioService.CriarUsuarioAsync(novoUsuario);
            return Ok();
        }

        /// <summary>
        /// Lista usuários paginados conforme filtro recebido
        /// </summary>
        /// <param name="filtro">Informações do filtro para a listagem</param>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("listar")]
        [ProducesResponseType(typeof(PaginacaoResult<UsuarioDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Listar([FromBody] ListarUsuarioRequestDTO filtro)
        {
            var lstUsuarios = await _usuarioService.ListarUsuarios(filtro);
            return Ok(lstUsuarios);
        }

        /// <summary>
        /// Lista todos os professores do sistema
        /// </summary>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("listarProfessores")]
        [ProducesResponseType(typeof(List<UsuarioDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ListarProfessores()
        {
            var lstUsuarios = await _usuarioService.ListarProfessores();
            return Ok(lstUsuarios);
        }

        /// <summary>
        /// Remove um usuário
        /// </summary>
        /// <param name="idUsuario">Id do usuário a ser deletado</param>
        [Authorize(Roles ="Admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpDelete("deletar")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Deletar([FromQuery] int idUsuario)
        {
            try
            {
                await _usuarioService.DeletarUsuario(idUsuario);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Lista um usuário pelo Id
        /// </summary>
        /// <param name="idUsuario">Id do usuário a ser buscado</param>
        [Authorize(Roles = "Admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("obterusuario")]
        [ProducesResponseType(typeof(UsuarioDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ObterUsuario([FromQuery] int idUsuario)
        {
            var usuario = await _usuarioService.ObterUsuarioPorId(idUsuario);
            return Ok(usuario);
        }

        /// <summary>
        /// Realiza a edição de um usuário
        /// </summary>
        /// <param name="request">Novos dados do usuário</param>
        [Authorize(Roles = "Admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut("editar")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Editar([FromBody] EditarUsuarioRequestDTO request)
        {
            await _usuarioService.EditarUsuario(request);
            return Ok();
        }
    }
}

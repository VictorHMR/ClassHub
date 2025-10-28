using ClassHub.ClassHubContext.Services;
using ClassHub.Dtos.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClassHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        public UserController(UserService userService)
        {
            _userService = userService;
        }
        /// <summary>
        /// Fornece um token de autenticação baseado no email e senha fornecidos.
        /// </summary>
        /// <param name="login">Informações de login do usuário</param>
        /// <returns>JWT Token do usuário</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO login)
        {
            var user = await _userService.ObterUserAsync(login);
            if (user == null) return Unauthorized("Login ou senha inválidos");

            return Ok(new LoginResponseDTO
            {
                Token = _userService.GerarToken(user),
                Nome = user.Nome,
                CPF = user.CPF,
                Email = user.Email,
                Role = user.Role
            });
        }

        /// <summary>
        /// Cria um usuário a partir de informações recebidas, apenas admins podem criar
        /// </summary>
        /// <param name="newUser">Informações do usuário a ser criado</param>
        #if !DEBUG
        [Authorize(Roles = "Admin")]
        #endif
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateUserRequestDTO newUser)
        {
            var user = await _userService.CriarUserAsync(newUser);
            return Ok();
        }

    }
}

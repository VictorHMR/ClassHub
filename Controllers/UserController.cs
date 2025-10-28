using ClassHub.Dtos.Users;
using Microsoft.AspNetCore.Mvc;

namespace ClassHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class UserController : ControllerBase
    {
        /// <summary>
        /// Fornece um token de autenticação baseado no email e senha fornecidos.
        /// </summary>
        /// <param name="login">Informações de login do usuário</param>
        /// <returns>JWT Token do usuário</returns>
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequestDTO login)
        {
            // lógica de autenticação
            return Ok(new LoginResponseDTO() { Token = ""});
        }
    }
}

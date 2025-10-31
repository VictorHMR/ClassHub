using ClassHub.ClassHubContext.Services;
using ClassHub.Dtos.Nota;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClassHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    public class NotaController: ControllerBase
    {
        private readonly NotaService _notaService;
        public NotaController(NotaService notaService)
        {
            _notaService = notaService;
        }

        /// <summary>
        /// Realiza a listagem de notas de um aluno em uma turma.
        /// </summary>
        /// <param name="idAlunoTurma">Identificação de um aluno na turma</param>
        /// <returns>Ok</returns>
        [HttpGet("ListarNotasAluno")]
        [Authorize(Roles = "Admin,Professor")]
        public async Task<IActionResult> ListarNotasAluno([FromQuery] int idAlunoTurma)
        {
            var notas = await _notaService.ListarNotasAluno(idAlunoTurma);
            return Ok(notas);
        }

        /// <summary>
        /// Realiza o lançamento de uma nova nota para um aluno em uma turma.
        /// </summary>
        /// <param name="novoLancamento">Objeto contendo informações da nota a ser lançada</param>
        /// <returns>Ok</returns>
        [HttpPost("Lancar")]
        [Authorize(Roles = "Admin,Professor")]
        public async Task<IActionResult> Lancar([FromBody] LancarNotaRequestDTO novoLancamento)
        {
            await _notaService.InserirNotaAsync(novoLancamento);
            return Ok();
        }

        /// <summary>
        /// Realiza a edição do lançamento de uma nova nota para um aluno em uma turma.
        /// </summary>
        /// <param name="novoLancamento">Objeto contendo informações da nota a ser editada</param>
        /// <returns>Ok</returns>
        [HttpPut("Editar")]
        [Authorize(Roles = "Admin,Professor")]
        public async Task<IActionResult> Editar([FromBody] EditarNotaRequestDTO novoLancamento)
        {
            await _notaService.EditarNotaAsync(novoLancamento);
            return Ok();
        }

        /// <summary>
        /// Realiza a deleção do lançamento de uma nova nota para um aluno em uma turma.
        /// </summary>
        /// <param name="idNota">Identificação de uma nota lançada</param>
        /// <returns>Ok</returns>
        [HttpDelete("Deletar")]
        [Authorize(Roles = "Admin,Professor")]
        public async Task<IActionResult> Deletar([FromQuery] int idNota)
        {
            await _notaService.DeletarNotaAsync(idNota);
            return Ok();
        }
    }
}

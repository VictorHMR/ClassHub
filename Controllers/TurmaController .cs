using ClassHub.ClassHubContext.Services;
using ClassHub.Dtos.Turma;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClassHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class TurmaController: ControllerBase
    {
        private readonly TurmaService _turmaService;
        public TurmaController(TurmaService turmaService)
        {
            _turmaService = turmaService;
        }

        /// <summary>
        /// Realiza uma listagem paginada de turmas cadastradas no sistema.
        /// </summary>
        /// <param name="page">Página pesquisada</param>
        /// <param name="pageSize">Tamanho da página</param>
        /// <returns>Lista de turmas e outras informações para paginação</returns>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ListarTurmas([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _turmaService.ListarTurmasAsync(page, pageSize);

            return Ok(result);
        }

        /// <summary>
        /// Realiza a criação de uma nova turma no sistema.
        /// </summary>
        /// <param name="novaTurma">Objeto contendo informações da turma a ser criada</param>
        /// <returns>Ok</returns>
        [HttpPost("CriarTurma")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CriarTurma([FromBody] CriarTurmaRequestDTO novaTurma) 
        {
            var result = await _turmaService.CriarTurmaAsync(novaTurma);

            return Ok();
        }

        /// <summary>
        /// Realiza o vínculo de um aluno a uma turma.
        /// </summary>
        /// <param name="novoVinculo">Objeto contendo RA do aluno e da turma que deseja vincular</param>
        /// <returns>Ok</returns>
        [HttpPost("VincularAlunoTurma")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> VincularAlunoTurma([FromBody] VincularAlunoTurmaRequestDTO novoVinculo)
        {
            await _turmaService.VincularAlunoTurmaAsync(novoVinculo);

            return Ok();
        }

    }
}

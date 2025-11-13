using ClassHub.ClassHubContext.Models;
using ClassHub.ClassHubContext.Services;
using ClassHub.Dtos.Turma;
using ClassHub.Dtos.Usuario;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClassHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
        /// <param name="request">Filtros da requisição</param>
        /// <returns>Lista de turmas e outras informações para paginação</returns>
        [HttpPost("Listar")]
        [Authorize]
        [ProducesResponseType(typeof(PaginacaoResult<TurmaDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Listar([FromBody] ListarTurmaRequestDTO request)
        {
            var result = await _turmaService.ListarTurmasAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// Realiza a listagem dos dados de uma turma pelo id
        /// </summary>
        /// <param name="idTurma">Id da turma desejada</param>
        /// <returns>Detalhes da turma cujo id foi enviado</returns>
        [HttpGet("Obter")]
        [Authorize]
        [ProducesResponseType(typeof(TurmaDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ObterTurma([FromQuery] int idTurma)
        {
            var result = await _turmaService.ObterTurmaPorId(idTurma);
            return Ok(result);
        }

        /// <summary>
        /// Realiza a criação de uma nova turma no sistema.
        /// </summary>
        /// <param name="novaTurma">Objeto contendo informações da turma a ser criada</param>
        /// <returns>Ok</returns>
        [HttpPost("Criar")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Criar([FromBody] CriarTurmaRequestDTO novaTurma) 
        {
            var result = await _turmaService.CriarTurmaAsync(novaTurma);
            return Ok();
        }

        /// <summary>
        /// Realiza a edição de uma turma existente.
        /// </summary>
        /// <param name="turma">Objeto contendo informações da turma a ser editada</param>
        /// <returns>Ok</returns>
        [HttpPut("Editar")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Editar([FromBody] EditarTurmaRequestDTO turma)
        {
            await _turmaService.EditarTurmaAsync(turma);
            return Ok();
        }

        /// <summary>
        /// Realiza a deleção de uma turma existente caso a mesma não possua alunos vinculados.
        /// </summary>
        /// <param name="idTurma">Objeto contendo id da turma a ser deletada</param>
        /// <returns>Ok</returns>
        [HttpDelete("Deletar")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Deletar([FromQuery] int idTurma)
        {
            await _turmaService.DeletarTurmaAsync(idTurma);
            return Ok();
        }

        /// <summary>
        /// Realiza o vínculo ou a remoção do vinculo de um aluno a uma turma.
        /// </summary>
        /// <param name="novoVinculo">Objeto contendo RA do aluno, id da turma e flag de remoção do vinculo</param>
        /// <returns>Ok</returns>
        [HttpPost("VincularAluno")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> VincularAluno([FromBody] VincularAlunoTurmaRequestDTO novoVinculo)
        {
            await _turmaService.VincularAlunoTurmaAsync(novoVinculo);
            return Ok();
        }

    }
}

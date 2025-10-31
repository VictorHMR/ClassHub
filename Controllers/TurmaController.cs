using ClassHub.ClassHubContext.Services;
using ClassHub.Dtos.Turma;
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
        /// <param name="page">Página pesquisada</param>
        /// <param name="pageSize">Tamanho da página</param>
        /// <returns>Lista de turmas e outras informações para paginação</returns>
        [HttpGet("Listar")]
        [Authorize]
        public async Task<IActionResult> Listar([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _turmaService.ListarTurmasAsync(page, pageSize);
            return Ok(result);
        }


        /// <summary>
        /// Realiza a criação de uma nova turma no sistema.
        /// </summary>
        /// <param name="novaTurma">Objeto contendo informações da turma a ser criada</param>
        /// <returns>Ok</returns>
        [HttpPost("Criar")]
        [Authorize(Roles = "Admin")]
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
        public async Task<IActionResult> VincularAluno([FromBody] VincularAlunoTurmaRequestDTO novoVinculo)
        {
            await _turmaService.VincularAlunoTurmaAsync(novoVinculo);
            return Ok();
        }

    }
}

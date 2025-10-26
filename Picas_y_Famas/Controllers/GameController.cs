using Picas_y_Famas.DataTransferObjects;
using Picas_y_Famas.Services;
using Microsoft.AspNetCore.Mvc;


namespace NumberGuessGameApi.Controllers
{
    [ApiController]
    [Route("api/game/v1")]
    public class GameController : ControllerBase
    {
        private readonly IGameService _gameService;
        private readonly ILogger<GameController> _logger;

        public GameController(IGameService gameService, ILogger<GameController> logger)
        {
            _gameService = gameService;
            _logger = logger;
        }

        /// <summary>
        /// Registra un nuevo jugador.
        /// </summary>
        [HttpPost("register")]
        public async Task<ActionResult<RegisterPlayerResponse>> RegisterPlayer([FromBody] RegisterPlayerRequest request)
        {
            try
            {
                var response = await _gameService.RegisterPlayerAsync(request);
                return Ok(response);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error al registrar jugador");
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Inicia un nuevo juego para un jugador.
        /// </summary>
        [HttpPost("start")]
        public async Task<ActionResult<StartGameResponse>> StartGame([FromBody] StartGameRequest request)
        {
            try
            {
                var response = await _gameService.StartGameAsync(request);
                return Ok(response);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error al iniciar juego");
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Envía un intento de adivinanza.
        /// </summary>
        [HttpPost("guess")]
        public async Task<ActionResult<GuessNumberResponse>> GuessNumber([FromBody] GuessNumberRequest request)
        {
            try
            {
                var response = await _gameService.GuessNumberAsync(request);
                return Ok(response);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error al procesar intento");
                return BadRequest(ex.Message);
            }
        }
    }
}

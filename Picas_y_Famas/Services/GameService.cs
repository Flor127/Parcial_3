using GameCore;
using Microsoft.EntityFrameworkCore;
using Picas_y_Famas.Data;
using Picas_y_Famas.DataTransferObjects;
using Picas_y_Famas.Models;
using Picas_y_Famas.Services;
using System;

namespace Picas_y_Famas.Services
{
    public class GameService : IGameService
    {
        private readonly GameDbContext _context;
        private readonly ILogger<GameService> _logger;

        public GameService(GameDbContext context, ILogger<GameService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<RegisterPlayerResponse> RegisterPlayerAsync(RegisterPlayerRequest request)
        {
            var existing = await _context.Players
                .FirstOrDefaultAsync(p => p.Nombre == request.Nombre && p.Apellido == request.Apellido && p.Edad == request.Edad);

            if (existing != null)
            {
                _logger.LogInformation("Usuario ya registrado: {Nombre} {Apellido}", request.Nombre, request.Apellido);
                throw new Exception("El usuario ya se encuentra registrado.");
            }

            var player = new Player
            {
                Nombre = request.Nombre,
                Apellido = request.Apellido,
                Edad = request.Edad,
                RegisteredAt = DateTime.Now
            };

            _context.Players.Add(player);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Nuevo usuario registrado: {Id}", player.Id);

            return new RegisterPlayerResponse { PlayerId = player.Id };
        }

        public async Task<StartGameResponse> StartGameAsync(StartGameRequest request)
        {
            var player = await _context.Players.FindAsync(request.PlayerId);
            if (player == null)
            {
                _logger.LogWarning("Intento de iniciar juego con usuario inexistente: {Id}", request.PlayerId);
                throw new Exception("Usuario no registrado.");
            }

            var activeGame = await _context.Games
                .FirstOrDefaultAsync(g => g.PlayerId == request.PlayerId && !g.Fin);

            if (activeGame != null)
            {
                _logger.LogWarning("Usuario {Id} ya tiene un juego activo", request.PlayerId);
                throw new Exception("Ya tenés un juego activo.");
            }

            var secret = GenerateSecretNumber();
            var game = new Game
            {
                PlayerId = request.PlayerId,
                NumeroSecreto = secret,
                CreatedAt = DateTime.Now,
                Fin = false
            };

            _context.Games.Add(game);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Juego iniciado: {GameId} para jugador {PlayerId}", game.Id, request.PlayerId);

            return new StartGameResponse
            {
                GameId = game.Id,
                PlayerId = game.PlayerId,
                CreatedAt = game.CreatedAt
            };
        }

        public async Task<GuessNumberResponse> GuessNumberAsync(GuessNumberRequest request)
        {
            var game = await _context.Games.FindAsync(request.GameId);
            if (game == null)
            {
                _logger.LogWarning("Juego no encontrado: {GameId}", request.GameId);
                throw new Exception("Juego no encontrado.");
            }

            if (game.Fin)
            {
                _logger.LogInformation("Juego {GameId} ya finalizado", request.GameId);
                return new GuessNumberResponse
                {
                    GameId = game.Id,
                    IntentoNumero = request.IntentoNumero,
                    Mensaje = $"El juego {game.Id} ya ha finalizado."
                };
            }

            if (!IsValidNumber(request.IntentoNumero))
            {
                _logger.LogWarning("Número inválido: {Number}", request.IntentoNumero);
                throw new Exception("Número inválido. Debe tener 4 dígitos únicos.");
            }

            var result = Evaluator.Validate(game.NumeroSecreto, request.IntentoNumero);

            var attempt = new Attempt
            {
                GameId = game.Id,
                IntentoNumero= request.IntentoNumero,
                AttemptedAt = DateTime.Now
            };

            _context.Attempts.Add(attempt);

            if (result.Fama == 4)
            {
                game.Fin = true;
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("Intento registrado: {Number} - {Message}", request.IntentoNumero, result.Message);

            return new GuessNumberResponse
            {
                GameId = game.Id,
                IntentoNumero = request.IntentoNumero,
                Mensaje = result.Message
            };
        }

        private string GenerateSecretNumber()
        {
            var rnd = new Random();
            var digits = new HashSet<int>();
            while (digits.Count < 4)
                digits.Add(rnd.Next(0, 10));
            return string.Join("", digits);
        }

        private bool IsValidNumber(string number)
        {
            return number.Length == 4 && number.Distinct().Count() == 4 && number.All(char.IsDigit);
        }
    }
}

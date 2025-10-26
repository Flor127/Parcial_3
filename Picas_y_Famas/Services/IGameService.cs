using Picas_y_Famas.DataTransferObjects;

namespace Picas_y_Famas.Services
{
    public interface IGameService
    {
        Task<RegisterPlayerResponse> RegisterPlayerAsync(RegisterPlayerRequest request);
        Task<StartGameResponse> StartGameAsync(StartGameRequest request);
        Task<GuessNumberResponse> GuessNumberAsync(GuessNumberRequest request);
    }
}

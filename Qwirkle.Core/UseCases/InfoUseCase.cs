namespace Qwirkle.Core.UseCases;

public class InfoUseCase
{
    private readonly IRepository _repository;

    public InfoUseCase(IRepository repository)
    {
        _repository = repository;
    }



    public int GetUserId(int playerId) => _repository.GetUserId(playerId);
    public Player GetPlayer(int playerId) => _repository.GetPlayer(playerId);
}
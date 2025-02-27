namespace backend.Classes.Data
{
    public interface IGameRepository
    {
        int CreateGame(int playerOneId, int playerTwoId, string gameData);
    }
}
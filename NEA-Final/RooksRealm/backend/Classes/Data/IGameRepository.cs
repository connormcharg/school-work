namespace backend.Classes.Data
{
    public interface IGameRepository
    {
        bool CreateGame(int playerOneId, int playerTwoId, string gameData);
    }
}

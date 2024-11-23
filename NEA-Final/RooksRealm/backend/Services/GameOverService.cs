using backend.Classes.Data;
using backend.Classes.Utilities;

namespace backend.Services
{
    public class GameOverService : IHostedService, IDisposable
    {
        private Timer? timer;
        private readonly ChessService chessService;
        private readonly IServiceProvider serviceProvider;
        /*private readonly IUserRepository userRepository;*/

        public GameOverService(ChessService chessService/*, IUserRepository userRepository*/, IServiceProvider serviceProvider)
        {
            this.chessService = chessService;
            /*this.userRepository = userRepository;*/
            this.serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            timer = new Timer(CheckGames, null, TimeSpan.Zero, TimeSpan.FromSeconds(0.5));
            return Task.CompletedTask;
        }

        private async void CheckGames(object state)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
                var statisticsRepository = scope.ServiceProvider.GetRequiredService<IStatisticsRepository>();

                foreach (var game in chessService.GetAllGames())
                {
                    if (game.state.gameOver)
                    {
                        int winFlag = -1; // 0 for white won, 1 for black won, 2 for draw
                        string result = "";
                        string reason = "";
                        string ratingChange1 = "";
                        string ratingChange2 = "";
                        int userIdOne = -1;
                        int userIdTwo = -1;
                        string outcome1 = "";
                        string outcome2 = "";
                        if (game.state.checkMate)
                        {
                            if (game.state.whiteToMove)
                            {
                                result = "Checkmate!";
                                reason = "Black won by checkmate!";
                                winFlag = 1;
                            }
                            else
                            {
                                result = "Checkmate!";
                                reason = "White won by checkmate!";
                                winFlag = 0;
                            }
                        }
                        else if (game.state.whiteTime <= 0)
                        {
                            result = "Black Wins!";
                            reason = "White ran out of time!";
                            winFlag = 1;
                        }
                        else if (game.state.blackTime <= 0)
                        {
                            result = "White Wins!";
                            reason = "Black ran out of time!";
                            winFlag = 0;
                        }
                        else if (game.state.staleMate)
                        {
                            result = "Stalemate!";
                            reason = "No more possible moves!";
                            winFlag = 2;
                        }
                        else if (game.state.fiftyMoveCounter >= 100)
                        {
                            result = "Draw!";
                            reason = "Fifty moves without moving a pawn or capturing a piece!";
                            winFlag = 2;
                        }
                        else if (game.state.drawAgreed)
                        {
                            result = "Draw!";
                            reason = "Players agreed to a draw!";
                            winFlag = 2;
                        }
                        else if (game.state.playerResigned)
                        {
                            if (game.state.isWhiteResignation)
                            {
                                result = "Black Wins!";
                                reason = "Black won by white's resignation!";
                                winFlag = 1;
                            }
                            else
                            {
                                result = "White Wins!";
                                reason = "White won by black's resignation!";
                                winFlag = 0;
                            }
                        }

                        if (game.settings.isRated)
                        {
                            if (game.settings.isSinglePlayer)
                            {
                                var user = userRepository.GetUserByUsername(game.players[0].nickName);
                                if (user == null)
                                {
                                    break;
                                }
                                int newRating = 0;
                                userIdOne = user.id;
                                switch (winFlag)
                                {
                                    case 0:
                                        // win for white
                                        if (game.players[0].isWhite)
                                        {
                                            newRating = RatingUtilities.GetRatingChange(user.rating, 600, 1.0);
                                            userRepository.UpdateUserRating(user.username, user.rating + newRating);
                                            ratingChange1 = RatingUtilities.GetRatingChangeString(user.rating + newRating, newRating);
                                            outcome1 = "win";
                                        }
                                        else
                                        {
                                            newRating = RatingUtilities.GetRatingChange(user.rating, 600, 0.0);
                                            userRepository.UpdateUserRating(user.username, user.rating + newRating);
                                            ratingChange1 = RatingUtilities.GetRatingChangeString(user.rating + newRating, newRating);
                                            outcome1 = "loss";
                                        }
                                        break;
                                    case 1:
                                        // win for black
                                        if (!game.players[0].isWhite)
                                        {
                                            newRating = RatingUtilities.GetRatingChange(user.rating, 600, 1.0);
                                            userRepository.UpdateUserRating(user.username, user.rating + newRating);
                                            ratingChange1 = $"{user.username}: {RatingUtilities.GetRatingChangeString(user.rating + newRating, newRating)}";
                                            outcome1 = "win";
                                        }
                                        else
                                        {
                                            newRating = RatingUtilities.GetRatingChange(user.rating, 600, 0.0);
                                            userRepository.UpdateUserRating(user.username, user.rating + newRating);
                                            ratingChange1 = $"{user.username}: {RatingUtilities.GetRatingChangeString(user.rating + newRating, newRating)}";
                                            outcome1 = "loss";
                                        }
                                        break;
                                    case 2:
                                        // draw
                                        newRating = RatingUtilities.GetRatingChange(user.rating, 600, 0.5);
                                        userRepository.UpdateUserRating(user.username, user.rating + newRating);
                                        ratingChange1 = $"{user.username}: {RatingUtilities.GetRatingChangeString(user.rating + newRating, newRating)}";
                                        outcome1 = "draw";
                                        break;
                                    default:
                                        break;
                                }
                            }
                            else
                            {
                                var user1 = userRepository.GetUserByUsername(game.players[0].nickName);
                                var user2 = userRepository.GetUserByUsername(game.players[1].nickName);
                                if (user1 == null || user2 == null)
                                {
                                    break;
                                }
                                int newRating1 = 0;
                                int newRating2 = 0;
                                userIdOne = user1.id;
                                userIdTwo = user2.id;
                                switch (winFlag)
                                {
                                    case 0:
                                        // win for white
                                        if (game.players[0].isWhite)
                                        {
                                            newRating1 = RatingUtilities.GetRatingChange(user1.rating, user2.rating, 1.0);
                                            newRating2 = RatingUtilities.GetRatingChange(user2.rating, user1.rating, 0.0);
                                            userRepository.UpdateUserRating(user1.username, user1.rating + newRating1);
                                            userRepository.UpdateUserRating(user2.username, user2.rating + newRating2);
                                            ratingChange1 = $"{user1.username}: {RatingUtilities.GetRatingChangeString(user1.rating + newRating1, newRating1)}";
                                            ratingChange2 = $"{user2.username}: {RatingUtilities.GetRatingChangeString(user2.rating + newRating2, newRating2)}";
                                            outcome1 = "win";
                                            outcome2 = "loss";
                                        }
                                        else
                                        {
                                            newRating1 = RatingUtilities.GetRatingChange(user1.rating, user2.rating, 0.0);
                                            newRating2 = RatingUtilities.GetRatingChange(user2.rating, user1.rating, 1.0);
                                            userRepository.UpdateUserRating(user1.username, user1.rating + newRating1);
                                            userRepository.UpdateUserRating(user2.username, user2.rating + newRating2);
                                            ratingChange1 = $"{user1.username}: {RatingUtilities.GetRatingChangeString(user1.rating + newRating1, newRating1)}";
                                            ratingChange2 = $"{user2.username}: {RatingUtilities.GetRatingChangeString(user2.rating + newRating2, newRating2)}";
                                            outcome1 = "loss";
                                            outcome2 = "win";
                                        }
                                        break;
                                    case 1:
                                        // win for black
                                        if (!game.players[0].isWhite)
                                        {
                                            newRating1 = RatingUtilities.GetRatingChange(user1.rating, user2.rating, 1.0);
                                            newRating2 = RatingUtilities.GetRatingChange(user2.rating, user1.rating, 0.0);
                                            userRepository.UpdateUserRating(user1.username, user1.rating + newRating1);
                                            userRepository.UpdateUserRating(user2.username, user2.rating + newRating2);
                                            ratingChange1 = $"{user1.username}: {RatingUtilities.GetRatingChangeString(user1.rating + newRating1, newRating1)}";
                                            ratingChange2 = $"{user2.username}: {RatingUtilities.GetRatingChangeString(user2.rating + newRating2, newRating2)}";
                                            outcome1 = "win";
                                            outcome2 = "loss";
                                        }
                                        else
                                        {
                                            newRating1 = RatingUtilities.GetRatingChange(user1.rating, user2.rating, 0.0);
                                            newRating2 = RatingUtilities.GetRatingChange(user2.rating, user1.rating, 1.0);
                                            userRepository.UpdateUserRating(user1.username, user1.rating + newRating1);
                                            userRepository.UpdateUserRating(user2.username, user2.rating + newRating2);
                                            ratingChange1 = $"{user1.username}: {RatingUtilities.GetRatingChangeString(user1.rating + newRating1, newRating1)}";
                                            ratingChange2 = $"{user2.username}: {RatingUtilities.GetRatingChangeString(user2.rating + newRating2, newRating2)}";
                                            outcome1 = "loss";
                                            outcome2 = "win";
                                        }
                                        break;
                                    case 2:
                                        // draw
                                        newRating1 = RatingUtilities.GetRatingChange(user1.rating, user2.rating, 0.5);
                                        newRating2 = RatingUtilities.GetRatingChange(user2.rating, user1.rating, 0.5);
                                        userRepository.UpdateUserRating(user1.username, user1.rating + newRating1);
                                        userRepository.UpdateUserRating(user2.username, user2.rating + newRating2);
                                        ratingChange1 = $"{user1.username}: {RatingUtilities.GetRatingChangeString(user1.rating + newRating1, newRating1)}";
                                        ratingChange2 = $"{user2.username}: {RatingUtilities.GetRatingChangeString(user2.rating + newRating2, newRating2)}";
                                        outcome1 = "draw";
                                        outcome2 = "draw";
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                        if (ratingChange1 != "" && ratingChange2 != "")
                        {
                            await chessService.GameOver(game.id, result, reason, $"{ratingChange1}  ---  {ratingChange2}");
                        }
                        else if (ratingChange1 != "")
                        {
                            await chessService.GameOver(game.id, result, reason, ratingChange1);
                        }
                        else
                        {
                            await chessService.GameOver(game.id, result, reason);
                        }

                        // archive the game

                        // ADD LOGIC TO RUN CREATEGAME FROM GAMEREPOSITORY AND SAVE THE GAMEID OF THAT GAME


                        if (userIdOne != -1 && userIdTwo != -1 && outcome1 != "" && outcome2 != "")
                        {
                            statisticsRepository.CreateStatistic((int)(game.state.moveLog.Count / 2), userIdOne, -1, outcome1);
                            statisticsRepository.CreateStatistic((int)(game.state.moveLog.Count / 2), userIdTwo, -1, outcome2);

                        }
                        else if (userIdOne != -1 && outcome1 != "")
                        {
                            statisticsRepository.CreateStatistic((int)(game.state.moveLog.Count / 2), userIdOne, -1, outcome1);
                        }
                        chessService.RemoveGame(game.id);
                    }
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            timer?.Dispose();
        }
    }
}

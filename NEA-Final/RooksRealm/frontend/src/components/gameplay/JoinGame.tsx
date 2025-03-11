import { useToast } from "@chakra-ui/react";
import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";

interface GameDetails {
  title: string;
  id: string;
  playerCount: number;
  players: Array<any>;
  settings: any;
}

const JoinGame: React.FC = () => {
  const [games, setGames] = useState<Array<GameDetails> | null>(null); // List of public games
  const [selectedGame, setSelectedGame] = useState<any | null>(null); // Selected game details
  const [gameCode, setGameCode] = useState(""); // Entered game code
  const toast = useToast();
  const navigate = useNavigate();

  const refreshGames = async () => {
    try {
      const response = await fetch("/proxy/api/chess/public");
      const data = await response.json();
      setGames(data);
    } catch (error) {
      toast({
        title: "Failed to fetch games",
        status: "error",
        duration: 4000,
        isClosable: true,
        position: "bottom",
      });
    }
  };

  const fetchGameDetails = async (gameId: string) => {
    const game = games?.find((g) => g.id === gameId);
    if (game) {
      setSelectedGame(game);
    } else {
      await fetchPrivateGameDetails(gameId);
    }
  };

  const fetchPrivateGameDetails = async (gameId: string) => {
    try {
      const response = await fetch(`/proxy/api/chess/details?id=${gameId}`);
      if (response.ok) {
        const game = await response.json();
        setSelectedGame(game);
      } else {
        toast({
          title: "Failed to fetch game details",
          status: "error",
          duration: 4000,
          isClosable: true,
          position: "bottom",
        });
      }
    } catch (error) {
      toast({
        title: "Error fetching game details",
        status: "error",
        duration: 4000,
        isClosable: true,
        position: "bottom",
      });
    }
  };

  const joinGame = async () => {
    if (!selectedGame) {
      toast({
        title: "No game selected",
        status: "error",
        duration: 4000,
        isClosable: true,
        position: "bottom",
      });
      return;
    }

    try {
      const response = await fetch(
        `/proxy/api/chess/join?id=${selectedGame.id}`,
      );
      if (response.ok) {
        navigate(`/play/${selectedGame.id}`);
      } else {
        toast({
          title: "Error joining game. Please refresh.",
          status: "error",
          duration: 4000,
          isClosable: true,
          position: "bottom",
        });
      }
    } catch (error) {
      toast({
        title: "Error joining game. Please refresh.",
        status: "error",
        duration: 4000,
        isClosable: true,
        position: "bottom",
      });
    }
  };

  useEffect(() => {
    refreshGames();
  }, []);

  return (
    <div className="flex h-full flex-col bg-gray-200 p-4 text-gray-900">
      <h1 className="mb-4 text-2xl font-bold">Join a Game!</h1>

      {/* Top Row: Refresh and Game Code Input */}
      <div className="mb-4 flex items-center gap-4">
        <button
          onClick={refreshGames}
          className="rounded bg-blue-500 px-4 py-2 text-white hover:bg-blue-600"
        >
          Refresh
        </button>
        <div className="flex flex-1 items-center gap-2">
          <input
            type="text"
            placeholder="Enter game code"
            value={gameCode}
            onChange={(e) => setGameCode(e.target.value)}
            className="flex-1 rounded border border-gray-300 bg-white px-4 py-2"
          />
          <button
            onClick={() => fetchGameDetails(gameCode)}
            className="rounded bg-green-500 px-4 py-2 text-white hover:bg-green-600"
          >
            ✓
          </button>
        </div>
      </div>

      {/* Main Content */}
      <div className="flex flex-1 gap-4">
        {/* List of Games */}
        <div className="flex-1 overflow-auto rounded-lg border border-gray-300 bg-white">
          <h2 className="border-b border-gray-300 bg-gray-100 p-4 text-lg font-semibold">
            Public Games
          </h2>
          {games && games.length > 0 ? (
            <ul className="p-4">
              {games.map((game) => (
                <li
                  key={game.id}
                  className="mb-2 cursor-pointer rounded border-2 p-2 hover:bg-gray-200"
                  onClick={() => fetchGameDetails(game.id)}
                >
                  <p>
                    <strong>Title:</strong> {game.title}
                  </p>
                  <p>
                    <strong>ID:</strong> {game.id}
                  </p>
                  <p>
                    <strong>Player Count:</strong> {game.playerCount}
                  </p>
                </li>
              ))}
            </ul>
          ) : (
            <p className="p-4 text-gray-500">No public games available</p>
          )}
        </div>

        {/* Game Details */}
        <div className="h-max flex-1 rounded-lg border border-gray-300 bg-white">
          <h2 className="border-b border-gray-300 bg-gray-100 p-4 text-lg font-semibold">
            Game Details
          </h2>
          {selectedGame ? (
            <div className="p-4">
              <p>
                <strong>Title:</strong> {selectedGame.title}
              </p>
              <p>
                <strong>ID:</strong> {selectedGame.id}
              </p>
              <p>
                <strong>Host Starting Color:</strong>{" "}
                {selectedGame.settings.isStartingWhite ? "White" : "Black"}
              </p>
              <p>
                <strong>Timed Game:</strong>{" "}
                {selectedGame.settings.isTimed ? "Yes" : "No"}
              </p>
              <p>
                <strong>Rated Game:</strong>{" "}
                {selectedGame.settings.isRated ? "Yes" : "No"}
              </p>
              <p>
                <strong>Players:</strong> {selectedGame.players.join(" vs ")}
              </p>
            </div>
          ) : (
            <p className="p-4 text-gray-500">
              Game details view that populates if you click on a game in the
              list or if you enter a code above.
            </p>
          )}
        </div>
      </div>

      {/* Join Button */}
      <div className="mt-4 flex">
        <button
          className="flex-1 rounded bg-blue-500 px-4 py-2 text-white hover:bg-blue-600"
          onClick={joinGame}
        >
          Join
        </button>
      </div>
    </div>
  );
};

export default JoinGame;

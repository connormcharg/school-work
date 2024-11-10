import React, { useState } from "react";
import { useNavigate } from "react-router-dom";

const StartGame: React.FC = () => {
  const [mode, setMode] = useState("");
  const [colour, setColour] = useState("");
  const [time, setTime] = useState("");
  const [privacy, setPrivacy] = useState("");
  const [rated, setRated] = useState("");
  const [watchable, setWatchable] = useState("");
  const [title, setTitle] = useState("");
  const [errorMessage, setErrorMessage] = useState("");
  const navigate = useNavigate();

  const handleStartGame = async () => {
    setErrorMessage("");
    const data = {
      isSinglePlayer: mode === "single",
      isStartingWhite: colour === "white",
      isTimed: time === "timed",
      isPrivate: privacy === "private",
      isRated: rated === "rated",
      isWatchable: watchable === "yes",
      gameTitle: title,
    };

    try {
      const response = await fetch("/proxy/api/chess/start", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(data),
      });

      if (!response.ok) {
        const errorData = await response.json();
        throw new Error(errorData.message || "An error occurred");
      }

      const { id } = await response.json();
      navigate(`/play/${id}`);
    } catch (error: any) {
      setErrorMessage(error.message);
    }
  };

  return (
    <div>
      <h1 className="mb-6 text-xl font-semibold">Start a game!</h1>
      {errorMessage && <p className="mb-4 text-red-500">{errorMessage}</p>}
      <div className="flex">
        <div className="flex flex-1 flex-col items-center justify-center p-4">
          <select
            value={mode}
            onChange={(e) => setMode(e.target.value)}
            className="mb-4 w-full rounded border border-gray-300 p-2 md:w-60"
          >
            <option value="">Select Mode</option>
            <option value="single">Single Player</option>
            <option value="multi">Multiplayer</option>
          </select>

          <select
            value={colour}
            onChange={(e) => setColour(e.target.value)}
            className="mb-4 w-full rounded border border-gray-300 p-2 md:w-60"
          >
            <option value="">Select Starting Colour</option>
            <option value="white">White</option>
            <option value="black">Black</option>
          </select>

          <select
            value={time}
            onChange={(e) => setTime(e.target.value)}
            className="mb-4 w-full rounded border border-gray-300 p-2 md:w-60"
          >
            <option value="">Select Time Control</option>
            <option value="timed">Timed</option>
            <option value="not-timed">Correspondence</option>
          </select>

          <select
            value={privacy}
            onChange={(e) => setPrivacy(e.target.value)}
            className="mb-4 w-full rounded border border-gray-300 p-2 md:w-60"
          >
            <option value="">Select Visibility</option>
            <option value="public">Public</option>
            <option value="private">Private</option>
          </select>

          <select
            value={rated}
            onChange={(e) => setRated(e.target.value)}
            className="mb-4 w-full rounded border border-gray-300 p-2 md:w-60"
          >
            <option value="">Is It Rated?</option>
            <option value="rated">Rated</option>
            <option value="unrated">Unrated</option>
          </select>

          <select
            value={watchable}
            onChange={(e) => setWatchable(e.target.value)}
            className="mb-4 w-full rounded border border-gray-300 p-2 md:w-60"
          >
            <option value="">Can It Be Watched?</option>
            <option value="yes">Yes</option>
            <option value="no">No</option>
          </select>

          <input
            type="text"
            value={title}
            onChange={(e) => setTitle(e.target.value)}
            placeholder="Enter Game Title"
            className="mb-4 w-full rounded border border-gray-300 p-2 md:w-60"
          />

          <button
            onClick={handleStartGame}
            className="rounded bg-blue-500 px-4 py-2 text-white hover:bg-blue-600"
          >
            Start Game
          </button>
        </div>

        <div className="flex flex-1 items-center justify-center h-full">
          <img
            draggable="false"
            src="/images/static-game-board.png"
            alt="Chess Board Starting Position"
            className="max-h-full max-w-full object-cover"
          />
        </div>
      </div>
    </div>
  );
};

export default StartGame;

import React, { useState } from "react";
import { useNavigate } from "react-router-dom";

const StartGame: React.FC = () => {
  const [formData, setFormData] = useState({
    mode: "",
    colour: "",
    time: "",
    privacy: "",
    rated: "",
    watchable: "",
    title: "",
  });

  const [errorMessage, setErrorMessage] = useState<string | null>(null);
  const navigate = useNavigate();

  const handleInputChange = (
    e: React.ChangeEvent<HTMLSelectElement | HTMLInputElement>,
  ) => {
    const { name, value } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: value,
    }));
  };

  const isFormValid = () => {
    return (
      formData.mode &&
      formData.colour &&
      formData.time &&
      formData.privacy &&
      formData.rated &&
      formData.watchable
    );
  };

  const handleStartGame = async () => {
    if (!isFormValid()) {
      setErrorMessage("Please fill in all fields.");
      return;
    }

    setErrorMessage(null);
    const data = {
      isSinglePlayer: formData.mode === "single",
      isStartingWhite: formData.colour === "white",
      isTimed: formData.time === "timed",
      isPrivate: formData.privacy === "private",
      isRated: formData.rated === "rated",
      isWatchable: formData.watchable === "yes",
      gameTitle: formData.title,
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
            name="mode"
            value={formData.mode}
            onChange={handleInputChange}
            className="mb-4 w-full rounded border border-gray-300 p-2 md:w-60"
          >
            <option value="">Select Mode</option>
            <option value="single">Single Player</option>
            <option value="multi">Multiplayer</option>
          </select>

          <select
            name="colour"
            value={formData.colour}
            onChange={handleInputChange}
            className="mb-4 w-full rounded border border-gray-300 p-2 md:w-60"
          >
            <option value="">Select Starting Colour</option>
            <option value="white">White</option>
            <option value="black">Black</option>
          </select>

          <select
            name="time"
            value={formData.time}
            onChange={handleInputChange}
            className="mb-4 w-full rounded border border-gray-300 p-2 md:w-60"
          >
            <option value="">Select Time Control</option>
            <option value="timed">Timed</option>
            <option value="not-timed">Correspondence</option>
          </select>

          <select
            name="privacy"
            value={formData.privacy}
            onChange={handleInputChange}
            className="mb-4 w-full rounded border border-gray-300 p-2 md:w-60"
          >
            <option value="">Select Visibility</option>
            <option value="public">Public</option>
            <option value="private">Private</option>
          </select>

          <select
            name="rated"
            value={formData.rated}
            onChange={handleInputChange}
            className="mb-4 w-full rounded border border-gray-300 p-2 md:w-60"
          >
            <option value="">Is It Rated?</option>
            <option value="rated">Rated</option>
            <option value="unrated">Unrated</option>
          </select>

          <select
            name="watchable"
            value={formData.watchable}
            onChange={handleInputChange}
            className="mb-4 w-full rounded border border-gray-300 p-2 md:w-60"
          >
            <option value="">Can It Be Watched?</option>
            <option value="yes">Yes</option>
            <option value="no">No</option>
          </select>

          <input
            type="text"
            name="title"
            value={formData.title}
            onChange={handleInputChange}
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

        <div className="flex h-full flex-1 items-center justify-center">
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

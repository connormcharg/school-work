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
            mode,
            colour,
            time,
            privacy,
            rated,
            watchable,
            title
        };

        try {
            const response = await fetch("https://localhost:7204/api/chess/start", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify(data)
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
            <h1 className="text-xl font-semibold mb-6">Start a game!</h1>
            {errorMessage && <p className="text-red-500 mb-4">{errorMessage}</p>}
            <div className="flex">
                <div className="flex-1 flex flex-col items-center justify-center p-4">
                    <select
                        value={mode}
                        onChange={(e) => setMode(e.target.value)}
                        className="mb-4 border border-gray-300 rounded p-2 w-full md:w-60"
                    >
                        <option value="">Select Mode</option>
                        <option value="single">Single Player</option>
                        <option value="multi">Multiplayer</option>
                    </select>

                    <select
                        value={colour}
                        onChange={(e) => setColour(e.target.value)}
                        className="mb-4 border border-gray-300 rounded p-2 w-full md:w-60"
                    >
                        <option value="">Select Starting Colour</option>
                        <option value="white">White</option>
                        <option value="black">Black</option>
                        <option value="random">Random</option>
                    </select>

                    <select
                        value={time}
                        onChange={(e) => setTime(e.target.value)}
                        className="mb-4 border border-gray-300 rounded p-2 w-full md:w-60"
                    >
                        <option value="">Select Time Control</option>
                        <option value="timed">Timed</option>
                        <option value="not-timed">Correspondence</option>
                    </select>

                    <select
                        value={privacy}
                        onChange={(e) => setPrivacy(e.target.value)}
                        className="mb-4 border border-gray-300 rounded p-2 w-full md:w-60"
                    >
                        <option value="">Select Visibility</option>
                        <option value="public">Public</option>
                        <option value="private">Private</option>
                    </select>

                    <select
                        value={rated}
                        onChange={(e) => setRated(e.target.value)}
                        className="mb-4 border border-gray-300 rounded p-2 w-full md:w-60"
                    >
                        <option value="">Is It Rated?</option>
                        <option value="rated">Rated</option>
                        <option value="unrated">Unrated</option>
                    </select>

                    <select
                        value={watchable}
                        onChange={(e) => setWatchable(e.target.value)}
                        className="mb-4 border border-gray-300 rounded p-2 w-full md:w-60"
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
                        className="mb-4 border border-gray-300 rounded p-2 w-full md:w-60"
                    />
                    
                    <button
                        onClick={handleStartGame}
                        className="bg-blue-500 text-white py-2 px-4 rounded hover:bg-blue-600">
                        Start Game
                        </button>
                </div>

                <div className="flex-1 flex items-center justify-center">
                    <img src="/images/static-game-board.png" alt="Game Image" className="max-w-full max-h-full object-cover" />
                </div>
            </div>
        </div>
    );
}

export default StartGame;
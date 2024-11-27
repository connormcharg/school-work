import React from "react";
import { Link } from "react-router-dom";

const Play: React.FC = () => (
  <div className="flex flex-col items-center">
    <h1 className="mb-6 text-xl font-semibold">Play Chess!</h1>
    <div className="flex w-full max-w-3xl justify-center space-x-4">
      <Link
        to="/start"
        className="w-full max-w-80 flex-1 rounded-lg bg-rose-500 py-10 text-center font-bold text-white transition-colors hover:bg-rose-600"
      >
        Start Game
      </Link>
      <Link
        to="/join"
        className="w-full max-w-80 flex-1 rounded-lg bg-fuchsia-500 py-10 text-center font-bold text-white transition-colors hover:bg-fuchsia-600"
      >
        Join Game
      </Link>
    </div>
  </div>
);

export default Play;

import React from "react";
import { Link } from "react-router-dom";
import { isAuthenticated } from "../utils/auth";

const Play: React.FC = () => {    
    return (
        <div className="flex flex-col items-center">
            <h1 className="text-xl font-semibold mb-6">Play Chess!</h1>
            <div className="flex justify-center space-x-4 w-full max-w-3xl">
                <Link
                    to="/start"
                    className="flex-1 max-w-80 w-full py-10 text-center bg-rose-500 hover:bg-rose-600 text-white font-bold rounded-lg transition-colors">
                    Start Game
                </Link>
                <Link
                    to="/join"
                    className="flex-1 max-w-80 w-full py-10 text-center bg-fuchsia-500 hover:bg-fuchsia-600 text-white font-bold rounded-lg transition-colors">
                    Join Game
                </Link>
            </div>
        </div>
    );
}

export default Play;
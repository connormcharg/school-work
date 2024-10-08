import React from "react";
import { useParams } from "react-router-dom";

const PlayGame: React.FC = () => {    
    const { id } = useParams<{ id: string }>();

    return (
        <div className="">
            <h1 className="text-xl font-semibold mb-6">Play Game!</h1>
            <h1 className="text-xl font-mono">{id}</h1>
        </div>
    );
}

export default PlayGame;
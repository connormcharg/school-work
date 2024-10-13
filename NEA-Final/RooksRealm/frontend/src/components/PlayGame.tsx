import React from "react";
import { useParams } from "react-router-dom";
import ChessBoard from "./ChessBoard";
import MessageBox from "./MessageBox";

interface PlayGameProps {
    boardSize: number;
}

const PlayGame: React.FC<PlayGameProps> = ({ boardSize }) => {    
    const { id } = useParams<{ id: string }>();

    return (
        <div className="">
            <h1 className="text-xl font-semibold mb-6">Play Game!</h1>
            <div className="flex">
                <MessageBox boxSize={(96 - boardSize) / 2}/>
                <ChessBoard boardSize={boardSize}/>
                <MessageBox boxSize={(96 - boardSize) / 2}/>
            </div>
        </div>
    );
}

export default PlayGame;
import React, { useState } from "react";
import { useParams } from "react-router-dom";
import ChessBoard from "./ChessBoard";
import MessageBox from "./MessageBox";

interface PlayGameProps {
    boardSize: number;
}

const PlayGame: React.FC<PlayGameProps> = ({ boardSize }) => {    
    const { id } = useParams<{ id: string }>();
    const [board, setBoard] = useState<string[][]>([
        ["br", "bn", "bb", "bq", "bk", "bb", "bn", "br"],
        ["bp", "bp", "bp", "bp", "bp", "bp", "bp", "bp"],
        ["--", "--", "--", "--", "--", "--", "--", "--"],
        ["--", "--", "--", "--", "--", "--", "--", "--"],
        ["--", "--", "--", "--", "--", "--", "--", "--"],
        ["--", "--", "--", "--", "--", "--", "--", "--"],
        ["wp", "wp", "wp", "wp", "wp", "wp", "wp", "wp"],
        ["wr", "wn", "wb", "wq", "wk", "wb", "wn", "wr"]
    ]);

    return (
        <div>
            <h1 className="text-xl font-semibold mb-6">Play Game!</h1>
            <div className="flex">
                <MessageBox boxSize={(96 - boardSize) / 2}/>
                <ChessBoard boardSize={boardSize} isWhite={true} board={board}/>
                <MessageBox boxSize={(96 - boardSize) / 2}/>
            </div>
        </div>
    );
}

export default PlayGame;
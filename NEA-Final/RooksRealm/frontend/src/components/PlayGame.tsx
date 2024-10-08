import React from "react";
import { useParams } from "react-router-dom";
import ChessBoard from "./ChessBoard";

const PlayGame: React.FC = () => {
    const onMoveValidCheck = async (start: number[], end: number[]): Promise<boolean> => {
        return true;
    };
    const onMakeMove = async (start: number[], end: number[]): Promise<void> => {      
        const [startRow, startCol] = start;
        const [endRow, endCol] = end;
        const newBoard = board.map(row => [...row]);
        newBoard[endRow][endCol] = newBoard[startRow][startCol];
        newBoard[startRow][startCol] = '--';
        setBoard(newBoard);
    };
    
    const { id } = useParams<{ id: string }>();
    const initialBoardSetup = [
        ["wr", "wn", "wb", "wq", "wk", "wb", "wn", "wr"],
        ["wp", "wp", "wp", "wp", "wp", "wp", "wp", "wp"],
        ["", "", "", "", "", "", "", ""],
        ["", "", "", "", "", "", "", ""],
        ["", "", "", "", "", "", "", ""],
        ["", "", "", "", "", "", "", ""],
        ["bp", "bp", "bp", "bp", "bp", "bp", "bp", "bp"],
        ["br", "bn", "bb", "bq", "bk", "bb", "bn", "br"]
    ];
    const [board, setBoard] = React.useState<string[][]>(initialBoardSetup);

    return (
        <div className="">
            <h1 className="text-xl font-semibold mb-6">Play Game!</h1>
            <ChessBoard isWhite={true} board={board} onMakeMove={onMakeMove} onMoveValidCheck={onMoveValidCheck} />
        </div>
    );
}

export default PlayGame;
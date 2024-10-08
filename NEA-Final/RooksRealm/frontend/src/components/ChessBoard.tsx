import React, { useState, useRef, useEffect } from "react";
import "./ChessBoard.css";

interface ChessBoardProps {
    isWhite: boolean;
    board: string[][];
    onMoveValidCheck: (start: number[], end: number[]) => Promise<boolean>;
    onMakeMove: (start: number[], end: number[]) => Promise<void>;
}

const ChessBoard: React.FC<ChessBoardProps> = ({ isWhite, board, onMoveValidCheck, onMakeMove }) => {
    const [selectedPiece, setSelectedPiece] = useState<HTMLElement | null>(null);
    const [selectedPieceInitialRowCol, setSelectedPieceInitialRowCol] = useState<number[] | null>(null);
    const [state, setState] = useState<number>(1);
    const gridRef = useRef<HTMLDivElement>(null);

    return (
        <div>

        </div>
    )
}

export default ChessBoard;
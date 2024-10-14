import React, { useState, useEffect, useRef } from "react";
import "./ChessBoard.css";

interface ChessBoardProps {
    boardSize: number;
    isWhite: boolean;
    board: string[][];
}

const ChessBoard: React.FC<ChessBoardProps> = ({ boardSize, isWhite, board }) => {
    const boardRef = useRef<HTMLDivElement>(null);
    const [boardPixelSize, setBoardPixelSize] = useState<number>(0);

    useEffect(() => {
        const updateBoardSize = () => {
            if (boardRef.current) {
                const containerWidth = boardRef.current.offsetWidth;
                const newBoardPixelSize = (boardSize / 100) * containerWidth;
                setBoardPixelSize(newBoardPixelSize);
            }
        };

        const resizeObserver = new ResizeObserver(() => {
            updateBoardSize();
        });

        if (boardRef.current) {
            resizeObserver.observe(boardRef.current);
        }

        updateBoardSize();

        return () => {
            if (boardRef.current) {
                resizeObserver.unobserve(boardRef.current);
            }
        }
    }, [boardSize]);

    const getRank = (index: number) => isWhite ? 8 - index : index + 1;
    const getFile = (index: number) => String.fromCharCode(isWhite ? 97 + index : 97 + (7 - index));
    const getPiece = (row: number, col: number) => isWhite ? board[row][col] : board[7 - row][7 - col];
    const isGrabbable = (piece: string) => piece !== "--";
    
    return (
        <div
            id="board"
            ref={boardRef}
            className="chess-board border rounded-xl"
            style={{
                "--chess-board-size": `${boardSize}%`,
                "--chess-board-pixel-size": `${boardPixelSize}px`
            } as React.CSSProperties}>
            <div id="grid" className="chess-grid rounded-tl-lg">
                {Array.from({ length: 8 }).map((_, row) =>
                    Array.from({ length: 8 }).map((_, col) => {
                        const piece = getPiece(row, col);
                        if (piece !== "--") {
                            const grabbableClass = isGrabbable(piece) ? "" : "nograb";
                            const colourClass = piece[0] === "w" ? "white" : "black";
                            const pieceClass = piece.toLowerCase();
                            return (
                                <div
                                    key={`${row}-${col}`}
                                    className={`chess-piece ${grabbableClass} ${colourClass} ${pieceClass} square-${row}-${col}`}
                                    style={{}}
                                ></div>
                            )
                        }
                        return null;
                    })
                )}
                <div id="hover-square" className="hover-square" style={{ visibility: "hidden" }}>
                    <svg viewBox="0 0 100 100">
                        <rect x="0" y="0" width="100" height="100"
                            stroke="rgba(255, 255, 255, 0.65)"
                            strokeWidth="10" fill="none"></rect>
                    </svg>
                </div>
                <div id="prev-move-start" className="highlight" style={{ visibility: "hidden" }}>

                </div>
                <div id="prev-move-end" className="highlight" style={{ visibility: "hidden" }}>
                    
                </div>
                <div id="curr-move" className="highlight" style={{ visibility: "hidden" }}>
                    
                </div>
            </div>
            <div id="ranks" className="chess-ranks">
                {Array.from({ length: 8 }).map((_, k) => (
                    <div key={k} className="chess-label rank">
                        <text className="chess-label-text">{getRank(k)}</text>
                    </div>
                ))}
            </div>
            <div id="files" className="chess-files">
                {Array.from({ length: 8 }).map((_, k) => (
                    <div key={k} className="chess-label file">
                        <text className="chess-label-text">{getFile(k)}</text>
                    </div>
                ))}
            </div>
        </div>
    );
}

export default ChessBoard;
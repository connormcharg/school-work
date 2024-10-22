import React, { useState, useEffect, useRef, MouseEventHandler } from "react";
import "./ChessBoard.css";

interface ChessBoardProps {
    boardSize: number;
    isInteractive: boolean;
    board: string[][];
    isWhite: boolean;
    onMoveValidCheck: (start: number[], end: number[]) => Promise<boolean>;
    onMakeMove: (start: number[], end: number[]) => Promise<void>;
    onIsWhiteCheck: () => Promise<boolean>;
}

const ChessBoard: React.FC<ChessBoardProps> = ({ boardSize, isInteractive, board, isWhite, onMoveValidCheck, onMakeMove, onIsWhiteCheck }) => {
    // #region Scaling
    
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

    // #endregion

    const getRank = (index: number) => isWhite ? 8 - index : index + 1;
    const getFile = (index: number) => String.fromCharCode(isWhite ? 97 + index : 97 + (7 - index));
    const getPiece = (row: number, col: number) => isWhite ? board[row][col] : board[7 - row][7 - col];

    // #region Interactivity

    // variables
    var selectedPiece: HTMLDivElement | null = null;
    var selectedPieceStartRowCol: Array<number> | null = null;
    var pieceOffset = 0;
    var percentX = 0;
    var percentY = 0;
    var state = 1;
    // SOME IF NOT ALL OF THESE NEED TO USE STATE INSTEAD

    // constants
    const grid = useRef<HTMLDivElement>(null);
    const gridRect = grid.current?.getBoundingClientRect();
    const hoverSquare = useRef<HTMLDivElement>(null);
    const previousMoveStartSquare = useRef<HTMLDivElement>(null);
    const previousMoveEndSquare = useRef<HTMLDivElement>(null);
    const currentMoveSquare = useRef<HTMLDivElement>(null);

    // functions
    const isGrabbable = (piece: string) => piece !== "--";
    const clamp = (value: number) => Math.max(-50, Math.min(value, 800));

    function getSquareClass(element: HTMLDivElement) {
        let regex = /^square-\d-\d$/;
        let classList = Array.from(element.classList);
        let foundClass = classList.find(className => regex.test(className as string));
        return foundClass ?? "";
    }

    function removeSquareClass(element: HTMLDivElement | null) {
        if (element) {
            let squareClass = getSquareClass(element);
            if (squareClass) {
                element.classList.remove(squareClass);
            }
        }
    }

    function getRowColFromSquareClass(squareClass: string) {
        let parts = squareClass.split("-");
        return [Number(parts[1]), Number(parts[2])];
    }

    function getIndexFromPercent(percent: number) {
        return Math.trunc((percent + 50) / 100);
    }

    function getRowColFromPercents() {
        return [getIndexFromPercent(percentY), getIndexFromPercent(percentX)];
    }

    function getRowColFromMouseXY(x: number, y: number) {
        if (gridRect) {
            return [
                Math.trunc(((y - gridRect.top + window.scrollY) / gridRect.height) * 8),
                Math.trunc(((x - gridRect.left + window.scrollX) / gridRect.width) * 8)
            ];
        }
        return [-1, -1];
    }

    function arraysEqual(a: any[], b: any[]) {
        if (a === b) return true;
        if (a === null || b === null) return false;
        if (a.length !== b.length) return false;
        return a.every((val: any, idx: number) => val === b[idx]);
    }

    function update(e : React.MouseEvent<HTMLDivElement>) {
        if (state === 2 || state === 4) {
            if (gridRect) {
                percentX = clamp(((e.clientX - gridRect.left - pieceOffset + window.scrollX) / gridRect.width) * 800);
                percentY = clamp(((e.clientY - gridRect.top - pieceOffset + window.scrollY) / gridRect.height) * 800);
            }
            if (selectedPiece) {
                selectedPiece.style.transform = `translate(${percentX}%, ${percentY}%)`;
                selectedPiece.classList.add("dragging");
            }
            removeSquareClass(hoverSquare.current);
            hoverSquare.current?.classList.add(`square-${getIndexFromPercent(percentY)}-${getIndexFromPercent(percentX)}`);
            if (hoverSquare.current) {
                hoverSquare.current.style.cssText = "";
            }
        }
    }

    function noSelectedNoDragging(valid = false) {
        if (selectedPiece) {
            removeSquareClass(selectedPiece);
            if (valid && hoverSquare.current) {
                selectedPiece.classList.add(getSquareClass(hoverSquare.current));
            } else if (selectedPieceStartRowCol) {
                selectedPiece.classList.add(`square-${selectedPieceStartRowCol[0]}-${selectedPieceStartRowCol[1]}`)
            }
            selectedPiece.classList.remove("dragging");
            selectedPiece.style.cssText = "";
        }
        selectedPiece = null;
        selectedPieceStartRowCol = null;
        pieceOffset = 0;
        percentX = 0;
        percentY = 0;
        state = 1;
        if (currentMoveSquare.current)
            currentMoveSquare.current.style.visibility = "hidden";
        if (hoverSquare.current)
            hoverSquare.current.style.visibility = "hidden";
    }

    function selectedDraggingFirstTime(e: React.MouseEvent<HTMLDivElement>, p: HTMLDivElement) {
        selectedDragging(e, p);
        state = 2;
    }

    function selectedNoDragging() {
        state = 3;
        removeSquareClass(selectedPiece);
        if (selectedPieceStartRowCol)
            selectedPiece?.classList.add(`square-${selectedPieceStartRowCol[0]}-${selectedPieceStartRowCol[1]}`);
        selectedPiece?.classList.remove("dragging");
        if (selectedPiece)
            selectedPiece.style.cssText = "";
        if (hoverSquare.current)
            hoverSquare.current.style.visibility = "hidden";
    }

    function selectedDragging(e: React.MouseEvent<HTMLDivElement>, p: HTMLDivElement) {
        selectedPiece = p;
        selectedPieceStartRowCol = getRowColFromSquareClass(getSquareClass(selectedPiece));
        pieceOffset = selectedPiece.getBoundingClientRect().width / 2;
        state = 4;
        removeSquareClass(currentMoveSquare.current);
        if (currentMoveSquare.current) {
            currentMoveSquare.current.classList.add(`square-${selectedPieceStartRowCol[0]}-${selectedPieceStartRowCol[1]}`);
            currentMoveSquare.current.style.cssText = "";
        }
        update(e);
    }

    // event handlers
    const handlePieceMouseDown: MouseEventHandler<HTMLDivElement> = (e) => {
        // e.target is the element
        var piece = e.target as HTMLDivElement;
        if (!piece.classList.contains("nograb")) {
            if (state === 1) {
                selectedDraggingFirstTime(e, piece)
            } else if (state === 3 && piece !== selectedPiece) {
                selectedDraggingFirstTime(e, piece);
            } else if (state === 3 && piece === selectedPiece) {
                selectedDragging(e, piece);
            }
        }
    }

    const handleGridMouseDown: MouseEventHandler<HTMLDivElement> = async (e) => {
        if (e.button === 0) {
            if (state === 3) {
                if (selectedPieceStartRowCol) {
                    var endRowCol = getRowColFromMouseXY(e.clientX, e.clientY);
                    if (await onMoveValidCheck(
                        selectedPieceStartRowCol,
                        endRowCol
                    )) {
                        await onMakeMove(
                            selectedPieceStartRowCol,
                            endRowCol
                        )
                        noSelectedNoDragging();
                    }
                }
            } else if (state === 3) {
                await noSelectedNoDragging();
            }
        }
    }

    const handleGridMouseUp: MouseEventHandler<HTMLDivElement> = async (e) => {
        if (e.button === 0) {
            var rowColFromPercents = getRowColFromPercents();
            if (selectedPieceStartRowCol) {
                if (state === 2 && arraysEqual(
                    selectedPieceStartRowCol,
                    rowColFromPercents
                )) {
                    selectedNoDragging();
                } else if (state === 2 && await onMoveValidCheck(
                    selectedPieceStartRowCol,
                    rowColFromPercents
                )) {
                    await onMakeMove(
                        selectedPieceStartRowCol,
                        rowColFromPercents
                    );
                    noSelectedNoDragging(true);
                } else if (state === 2) {
                    selectedNoDragging();
                } else if (state === 4 && arraysEqual(
                    selectedPieceStartRowCol,
                    rowColFromPercents
                )) {
                    noSelectedNoDragging();
                } else if (state === 4 && await onMoveValidCheck(
                    selectedPieceStartRowCol,
                    rowColFromPercents
                )) {
                    await onMakeMove(
                        selectedPieceStartRowCol,
                        rowColFromPercents
                    );
                    noSelectedNoDragging(true);
                } else if (state === 4) {
                    selectedNoDragging();
                }
            }
        }
    }

    const handleGridMouseMove: MouseEventHandler<HTMLDivElement> = async (e) => {
        update(e);
    }
    // #endregion
    
    return (
        <div
            id="board"
            ref={boardRef}
            className="chess-board border rounded-xl"
            style={{
                "--chess-board-size": `${boardSize}%`,
                "--chess-board-pixel-size": `${boardPixelSize}px`
            } as React.CSSProperties}>
            <div
                id="grid"
                ref={grid}
                className="chess-grid rounded-tl-lg"
                onMouseDown={handleGridMouseDown}
                onMouseUp={handleGridMouseUp}
                onMouseMove={handleGridMouseMove}
            >
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
                                    onMouseDown={handlePieceMouseDown}
                                    style={{}}
                                ></div>
                            )
                        }
                        return null;
                    })
                )}
                <div
                    id="hover-square"
                    className="hover-square"
                    style={{ visibility: "hidden" }}
                    ref={hoverSquare}
                >
                    <svg viewBox="0 0 100 100">
                        <rect x="0" y="0" width="100" height="100"
                            stroke="rgba(255, 255, 255, 0.65)"
                            strokeWidth="10" fill="none"></rect>
                    </svg>
                </div>
                <div
                    id="prev-move-start"
                    className="highlight"
                    style={{ visibility: "hidden" }}
                    ref={previousMoveStartSquare}
                >

                </div>
                <div
                    id="prev-move-end"
                    className="highlight"
                    style={{ visibility: "hidden" }}
                    ref={previousMoveEndSquare}
                >

                </div>
                <div
                    id="curr-move"
                    className="highlight"
                    style={{ visibility: "hidden" }}
                    ref={currentMoveSquare}
                >

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
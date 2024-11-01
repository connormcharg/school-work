import React, { useState, useEffect, useRef, MouseEventHandler } from "react";
import "./ChessBoard.css";

interface ChessBoardProps {
  boardSize: number;
  isInteractive: boolean;
  board: string[][];
  isWhite: boolean;
  onMoveValidCheck: (start: number[], end: number[]) => Promise<boolean>;
  onMakeMove: (start: number[], end: number[]) => Promise<void>;
  onHighlightSquares: () => HighlightData;
}

interface HighlightData {
  "prev-start": [number, number] | null;
  "prev-end": [number, number] | null;
  "check": [number, number] | null;
}

const ChessBoard: React.FC<ChessBoardProps> = ({
  boardSize,
  isInteractive,
  board,
  isWhite,
  onMoveValidCheck,
  onMakeMove,
  onHighlightSquares
}) => {
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
    };
  }, [boardSize]);

  // #endregion

  const getRank = (index: number) => (isWhite ? 8 - index : index + 1);
  const getFile = (index: number) =>
    String.fromCharCode(isWhite ? 97 + index : 97 + (7 - index));
  const getPiece = (row: number, col: number) =>
    isWhite ? board[row][col] : board[7 - row][7 - col];

  // #region Interactivity

  // variables
  // const [selectedPiece, setSelectedPiece] = useState<HTMLDivElement | null>(null);
  // const [selectedPieceStartRowCol, setSelectedPieceStartRowCol] = useState<Array<number> | null>(null);
  const selectedPiece = useRef<HTMLDivElement | null>(null);
  const selectedPieceStartRowCol = useRef<Array<number> | null>(null);
  const pieceOffset = useRef(0);
  const percentX = useRef(0);
  const percentY = useRef(0);
  const state = useRef(1);
  const selectedPieceString = useRef("");
  // var selectedPiece: HTMLDivElement | null = null;
  // var selectedPieceStartRowCol: Array<number> | null = null;
  // var pieceOffset = 0;
  // var percentX = 0;
  // var percentY = 0;
  // var state = 1;

  // effects
  useEffect(() => {
    // reset if board changed whilst picked up
    if (selectedPiece.current && board) {
      const isPieceChanged = checkIfChanged(
        getRowColFromSquareClass(getSquareClass(selectedPiece.current)),
        selectedPieceString.current);
      if (isPieceChanged) {
        // resetting code to restore board back to normal
        selectedPiece.current.style.cssText = "";
        selectedPiece.current.classList.remove("dragging");
        if (currentMoveSquare.current) currentMoveSquare.current.style.visibility = "hidden";
        if (hoverSquare.current) hoverSquare.current.style.visibility = "hidden";

        selectedPiece.current = null;
        selectedPieceStartRowCol.current = null;
        pieceOffset.current = 0;
        percentX.current = 0;
        percentY.current = 0;
        state.current = 1;
        selectedPieceString.current = "";
      }
    }

    // update highlighting squares (prev move, check)
    if (previousMoveStartSquare.current) {
      previousMoveStartSquare.current.style.visibility = "hidden";
      removeSquareClass(previousMoveStartSquare.current);
    }
    if (previousMoveEndSquare.current) {
      previousMoveEndSquare.current.style.visibility = "hidden";
      removeSquareClass(previousMoveEndSquare.current);
    }
    if (checkSquare.current) {
      checkSquare.current.style.visibility = "hidden";
      removeSquareClass(checkSquare.current);
    }

    const highlightData: HighlightData = onHighlightSquares();
    if (highlightData["prev-start"]) {
      if (previousMoveStartSquare.current) {
        previousMoveStartSquare.current.classList.add(
          `square-${highlightData["prev-start"][0]}-${highlightData["prev-start"][1]}`);
        previousMoveStartSquare.current.style.cssText = "";
      } 
    }
    if (highlightData["prev-end"]) {
      if (previousMoveEndSquare.current) {
        previousMoveEndSquare.current.classList.add(
          `square-${highlightData["prev-end"][0]}-${highlightData["prev-end"][1]}`);
        previousMoveEndSquare.current.style.cssText = "";
      } 
    }
    if (highlightData["check"]) {
      if (checkSquare.current) {
        checkSquare.current.classList.add(
          `square-${highlightData["check"][0]}-${highlightData["check"][1]}`);
        checkSquare.current.style.cssText = "";
      } 
    }

  }, [board]);

  // constants
  const grid = useRef<HTMLDivElement>(null);
  const gridRect = grid.current?.getBoundingClientRect();
  const hoverSquare = useRef<HTMLDivElement>(null);
  const previousMoveStartSquare = useRef<HTMLDivElement>(null);
  const previousMoveEndSquare = useRef<HTMLDivElement>(null);
  const currentMoveSquare = useRef<HTMLDivElement>(null);
  const checkSquare = useRef<HTMLDivElement>(null);

  // functions
  const isGrabbable = (piece: string) => piece !== "--" && isInteractive && ((piece[0] === "w" && isWhite) || (piece[0] === "b" && !isWhite));
  const clamp = (value: number) => Math.max(-50, Math.min(value, 800));

  function checkIfChanged(rowCol: Array<number>, piece: string) {
    if (board[rowCol[0]][rowCol[1]].toLowerCase() !== piece) {
      return true;
    }
    return false;
  }

  function getPieceClass(element: HTMLDivElement) {
    let regex = /^(w|b)[a-z]/;
    let classList = Array.from(element.classList);
    let foundClass = classList.find((name) => regex.test(name as string));
    return foundClass ?? "";
  }

  function getSquareClass(element: HTMLDivElement) {
    let regex = /^square-\d-\d$/;
    let classList = Array.from(element.classList);
    let foundClass = classList.find((className) =>
      regex.test(className as string),
    );
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
    return [getIndexFromPercent(percentY.current), getIndexFromPercent(percentX.current)];
  }

  function getRowColFromMouseXY(x: number, y: number) {
    if (gridRect) {
      return [
        Math.trunc(((y - gridRect.top + window.scrollY) / gridRect.height) * 8),
        Math.trunc(((x - gridRect.left + window.scrollX) / gridRect.width) * 8),
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

  function update(e: React.MouseEvent<HTMLDivElement>) {
    if (state.current === 2 || state.current === 4) {
      if (gridRect) {
        percentX.current = clamp(
          ((e.clientX - gridRect.left - pieceOffset.current + window.scrollX) /
            gridRect.width) *
            800,
        );
        percentY.current = clamp(
          ((e.clientY - gridRect.top - pieceOffset.current + window.scrollY) /
            gridRect.height) *
            800,
        );
      }
      if (selectedPiece.current) {
        selectedPiece.current.style.cssText = `transform: translate(${percentX.current}%, ${percentY.current}%)`;
        selectedPiece.current.classList.add("dragging");
      }
      removeSquareClass(hoverSquare.current);
      hoverSquare.current?.classList.add(
        `square-${getIndexFromPercent(percentY.current)}-${getIndexFromPercent(percentX.current)}`,
      );
      if (hoverSquare.current) {
        hoverSquare.current.style.cssText = "";
      }
    }
  }

  function noSelectedNoDragging(valid = false) {
    if (selectedPiece.current) {
      removeSquareClass(selectedPiece.current);
      if (valid && hoverSquare.current) {
        selectedPiece.current.classList.add(getSquareClass(hoverSquare.current));
      } else if (selectedPieceStartRowCol.current) {
        selectedPiece.current.classList.add(
          `square-${selectedPieceStartRowCol.current[0]}-${selectedPieceStartRowCol.current[1]}`,
        );
      }
      selectedPiece.current.classList.remove("dragging");
      selectedPiece.current.style.cssText = "";
    }
    selectedPiece.current = null;
    selectedPieceStartRowCol.current = null;
    pieceOffset.current = 0;
    percentX.current = 0;
    percentY.current = 0;
    state.current = 1;
    if (currentMoveSquare.current)
      currentMoveSquare.current.style.visibility = "hidden";
    if (hoverSquare.current) hoverSquare.current.style.visibility = "hidden";
  }

  function selectedDraggingFirstTime(
    e: React.MouseEvent<HTMLDivElement>,
    p: HTMLDivElement,
  ) {
    selectedDragging(e, p);
    state.current = 2;
  }

  function selectedNoDragging() {
    state.current = 3;
    removeSquareClass(selectedPiece.current);
    if (selectedPieceStartRowCol.current)
      selectedPiece.current?.classList.add(
        `square-${selectedPieceStartRowCol.current[0]}-${selectedPieceStartRowCol.current[1]}`,
      );
    selectedPiece.current?.classList.remove("dragging");
    if (selectedPiece.current) selectedPiece.current.style.cssText = "";
    if (hoverSquare.current) hoverSquare.current.style.visibility = "hidden";
  }

  function selectedDragging(
    e: React.MouseEvent<HTMLDivElement>,
    p: HTMLDivElement,
  ) {
    selectedPiece.current = p;
    selectedPieceString.current = getPieceClass(selectedPiece.current);
    if (!selectedPiece.current) {
      return;
    }
    selectedPieceStartRowCol.current = getRowColFromSquareClass(
      getSquareClass(selectedPiece.current),
    );
    pieceOffset.current = selectedPiece.current.getBoundingClientRect().width / 2;
    state.current = 4;
    removeSquareClass(currentMoveSquare.current);
    if (currentMoveSquare.current && selectedPieceStartRowCol.current) {
      currentMoveSquare.current.classList.add(
        `square-${selectedPieceStartRowCol.current[0]}-${selectedPieceStartRowCol.current[1]}`,
      );
      currentMoveSquare.current.style.cssText = "";
    }
    update(e);
  }

  // event handlers
  const handlePieceMouseDown: MouseEventHandler<HTMLDivElement> = (e) => {
    // e.target is the element
    var piece = e.target as HTMLDivElement;
    if (!piece.classList.contains("nograb")) {
      if (state.current === 1) {
        selectedDraggingFirstTime(e, piece);
      } else if (state.current === 3 && piece !== selectedPiece.current) {
        selectedDraggingFirstTime(e, piece);
      } else if (state.current === 3 && piece === selectedPiece.current) {
        selectedDragging(e, piece);
      }
    }
  };

  const handleGridMouseDown: MouseEventHandler<HTMLDivElement> = async (e) => {
    if (e.button === 0) {
      if (state.current === 3) {
        if (selectedPieceStartRowCol.current) {
          var endRowCol = getRowColFromMouseXY(e.clientX, e.clientY);
          if (await onMoveValidCheck(selectedPieceStartRowCol.current, endRowCol)) {
            await onMakeMove(selectedPieceStartRowCol.current, endRowCol);
            noSelectedNoDragging();
          }
        }
      } else if (state.current === 3) {
        await noSelectedNoDragging();
      }
    }
  };

  const handleGridMouseUp: MouseEventHandler<HTMLDivElement> = async (e) => {
    if (e.button === 0) {
      var rowColFromPercents = getRowColFromPercents();
      if (selectedPieceStartRowCol.current) {
        if (
          state.current === 2 &&
          arraysEqual(selectedPieceStartRowCol.current, rowColFromPercents)
        ) {
          selectedNoDragging();
        } else if (
          state.current === 2 &&
          (await onMoveValidCheck(selectedPieceStartRowCol.current, rowColFromPercents))
        ) {
          await onMakeMove(selectedPieceStartRowCol.current, rowColFromPercents);
          noSelectedNoDragging(true);
        } else if (state.current === 2) {
          selectedNoDragging();
        } else if (
          state.current === 4 &&
          arraysEqual(selectedPieceStartRowCol.current, rowColFromPercents)
        ) {
          noSelectedNoDragging();
        } else if (
          state.current === 4 &&
          (await onMoveValidCheck(selectedPieceStartRowCol.current, rowColFromPercents))
        ) {
          await onMakeMove(selectedPieceStartRowCol.current, rowColFromPercents);
          noSelectedNoDragging(true);
        } else if (state.current === 4) {
          selectedNoDragging();
        }
      }
    }
  };

  const handleGridMouseMove: MouseEventHandler<HTMLDivElement> = async (e) => {
    update(e);
  };
  // #endregion

  return (
    <div
      id="board"
      ref={boardRef}
      className="chess-board rounded-xl border"
      style={
        {
          "--chess-board-size": `${boardSize}%`,
          "--chess-board-pixel-size": `${boardPixelSize}px`,
        } as React.CSSProperties
      }
    >
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
                  className={`chess-piece ${grabbableClass} ${pieceClass} ${colourClass} square-${row}-${col}`}
                  onMouseDown={handlePieceMouseDown}
                  style={{}}
                ></div>
              );
            }
            return null;
          }),
        )}
        <div
          id="hover-square"
          className="hover-square"
          style={{ visibility: "hidden" }}
          ref={hoverSquare}
        >
          <svg viewBox="0 0 100 100">
            <rect
              x="0"
              y="0"
              width="100"
              height="100"
              stroke="rgba(255, 255, 255, 0.65)"
              strokeWidth="10"
              fill="none"
            ></rect>
          </svg>
        </div>
        <div
          id="prev-move-start"
          className="highlight bg-blue-500"
          style={{ visibility: "hidden" }}
          ref={previousMoveStartSquare}
        ></div>
        <div
          id="prev-move-end"
          className="highlight bg-blue-500"
          style={{ visibility: "hidden" }}
          ref={previousMoveEndSquare}
        ></div>
        <div
          id="curr-move"
          className="highlight bg-blue-500"
          style={{ visibility: "hidden" }}
          ref={currentMoveSquare}
        ></div>
        <div
          id="check"
          className="highlight bg-red-500"
          style={{ visibility: "hidden" }}
          ref={checkSquare}
        ></div>
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
};

export default ChessBoard;
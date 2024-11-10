import React, { useState, useEffect, useCallback, useRef } from "react";
import { useParams } from "react-router-dom";
import {
  HubConnectionBuilder,
  HubConnection
} from "@microsoft/signalr";
import ChessBoard from "./ChessBoard";
import MessageBox from "./MessageBox";
import { useAuth } from "../../contexts/AuthProvider";
import ControlStack from "./ControlStack";

interface PlayGameProps {
  boardSize: number;
}

interface HighlightData {
  "prev-start": [number, number] | null;
  "prev-end": [number, number] | null;
  "check": [number, number] | null;
}

interface Player {
  nickname: string;
  isWhite: boolean;
  timeLeft: number;
}

const PlayGame: React.FC<PlayGameProps> = ({ boardSize }) => {
  const { id } = useParams<{ id: string }>();
  const [board, setBoard] = useState<string[][]>([
    ["--", "--", "--", "--", "--", "--", "--", "--"],
    ["--", "--", "--", "--", "--", "--", "--", "--"],
    ["--", "--", "--", "--", "--", "--", "--", "--"],
    ["--", "--", "--", "--", "--", "--", "--", "--"],
    ["--", "--", "--", "--", "--", "--", "--", "--"],
    ["--", "--", "--", "--", "--", "--", "--", "--"],
    ["--", "--", "--", "--", "--", "--", "--", "--"],
    ["--", "--", "--", "--", "--", "--", "--", "--"],
  ]);
  const [playerData, setPlayerData] = useState<Array<Player> | null>(null);
  const [moveLogData, setMoveLogData] = useState<Array<string> | null>(null);

  const { isLoggedIn, token } = useAuth();

  // #region SignalR Connection
  const [data, setData] = useState<any>(null);
  const [connection, setConnection] = useState<HubConnection | null>(null);
  const [isConnected, setIsConnected] = useState(false);

  useEffect(() => {
    const getJwtToken = () => {
      if (isLoggedIn) {
        return token;
      }
      throw new Error("User not logged in!");
    }
    
    const newConnection = new HubConnectionBuilder()
      .withUrl("/proxy/chesshub", {
        accessTokenFactory: () => getJwtToken(),
      })
      .withAutomaticReconnect()
      .build();

    setConnection(newConnection);

    newConnection
      .start()
      .then(() => {
        console.log("Connection started.");
        setIsConnected(true);
        newConnection.on("ReceiveGame", (eventData: any) => {
          setData(JSON.parse(eventData));
        });
      })
      .catch((err: any) => console.log("Error while starting connection: ", err));

    return () => {
      if (newConnection) {
        newConnection.stop().then(() => {
          console.log("Connection stopped.");
        });
        newConnection.off("ReceiveGame");
      }
    };
  }, []);

  useEffect(() => {
    if (connection && isConnected) {
      connection
        .send("JoinGameAsPlayer", id)
        .then(() => {
          console.log("Joined game with ID: ", id);
        })
        .catch((err: any) => console.log("Error joining game: ", err));
    }
  }, [connection, isConnected, id]);

  useEffect(() => {
    if (data && data.state && data.state.board) setBoard(data.state.board);
    if (data && data.settings) {
      if (data.settings.isSinglePlayer) {
        if (isWhite()) {
          setPlayerData([
            {
              nickname: "Computer Player",
              isWhite: false,
              timeLeft: data.settings.isTimed ? data.state.blackTime : -1,
            },
            {
              nickname: data.players[0].nickName,
              isWhite: true,
              timeLeft: data.settings.isTimed ? data.state.whiteTime : -1,
            },
          ]);
        } else {
          setPlayerData([
            {
              nickname: "Computer Player",
              isWhite: true,
              timeLeft: data.settings.isTimed ? data.state.whiteTime : -1,
            },
            {
              nickname: data.players[0].nickName,
              isWhite: false,
              timeLeft: data.settings.isTimed ? data.state.blackTime : -1,
            },
          ]);
        }
      } else {
        if (isWhite()) {
          var p = data.players.find((player: any) => player.connectionId === connection?.connectionId);
          var p2 = (data.players.length === 2) ? (data.players[(data.players.indexOf(p) === 0 ? 1 : 0)]) : null;
          setPlayerData([
            {
              nickname: p2 ? p2.nickName : "Waiting...",
              isWhite: false,
              timeLeft: data.settings.isTimed ? data.state.blackTime : -1,
            },
            {
              nickname: p.nickName,
              isWhite: true,
              timeLeft: data.settings.isTimed ? data.state.whiteTime : -1,
            },
          ]);
        } else {
          var p = data.players.find((player: any) => player.connectionId === connection?.connectionId);
          var p2 = (data.players.length === 2) ? (data.players[(data.players.indexOf(p) === 0 ? 1 : 0)]) : null;
          setPlayerData([
            {
              nickname: p2 ? p2.nickName : "Waiting...",
              isWhite: true,
              timeLeft: data.settings.isTimed ? data.state.whiteTime : -1,
            },
            {
              nickname: p.nickName,
              isWhite: false,
              timeLeft: data.settings.isTimed ? data.state.blackTime : -1,
            },
          ]);
        }
      }
    }
    if (data && data.state && data.state.moveLog) {
      setMoveLogData(getNotation(data.state.moveLog));
    }
  }, [data]);

  const getNotation = (moveLog: Array<any>) => {
    const colToFile = (col: number) => {
      if (col == 0) return "a";
      if (col == 1) return "b";
      if (col == 2) return "c";
      if (col == 3) return "d";
      if (col == 4) return "e";
      if (col == 5) return "f";
      if (col == 6) return "g";
      if (col == 7) return "h";
      return "";
    }
    const rowToRank = (row: number) => {
      if (row == 0) return "1";
      if (row == 1) return "2";
      if (row == 2) return "3";
      if (row == 3) return "4";
      if (row == 4) return "5";
      if (row == 5) return "6";
      if (row == 6) return "7";
      if (row == 7) return "8";
      return "";
    }
    const pieceToUnicode = (piece: string, isWhite: boolean) => {
      switch (piece.toUpperCase()) {
        case "R":
          return isWhite ? "♖" : "♜"; // Rook
        case "N":
          return isWhite ? "♘" : "♞"; // Knight
        case "B":
          return isWhite ? "♗" : "♝"; // Bishop
        case "Q":
          return isWhite ? "♕" : "♛"; // Queen
        case "K":
          return isWhite ? "♔" : "♚"; // King
        case "P":
          return isWhite ? "♙" : "♟"; // Pawn
        default:
          return ""; // Return an empty string if the input is not a valid piece letter
      }
    };
    
    const result = new Array<string>;

    moveLog.forEach(move => {
      var notation = "";
      if (move) {
        if (move.isCastleMove) {
          // kingside
          if (move.endCol === 6) {
            result.push("O-O");
            return;
          } else {
            result.push("O-O-O");
            return;
          }
        }

        var isWhite = move.pieceMoved[0] === "w";
        var piece = move.pieceMoved[1];


        // 1. Add piece letter if it's not a pawn
        if (piece !== "P") {
          notation += pieceToUnicode(piece, isWhite);
        }

        // 2. Disambiguate if necessary
        // const disambiguousMoves = moveLog.filter(
        //   m => m.pieceMoved === move.pieceMoved && m.endRow === move.endRow && m.endCol === move.endCol
        // );
        // if (disambiguousMoves.length > 1) {
        //   console.log(disambiguousMoves);
        // }

        notation += colToFile(move.startCol);
        notation += rowToRank(move.startRow);

        // 3. Handle captures
        if (move.pieceCaptured !== "--") {
          // if piece is pawn: add start file if we havent already
          notation += "x";
        }

        // 4. Add destination square
        notation += colToFile(move.endCol) + rowToRank(move.endRow);

        // 5. Handle promotions
        if (move.pawnPromotion) {
          notation += "=Q";
        }

        // 6. Add check or checkmate symbols if applicable
        // if move results in checkmate then add "#"
        // else if move results in check then add "+"

        result.push(notation);
        return;
      }
    });
    
    return result;
  }
  // #endregion

  const chessBoardRef = useRef<HTMLDivElement | null>(null);
  const [maxHeight, setMaxHeight] = useState<string>("10rem");

  useEffect(() => {
    // Update the maxHeight when the component mounts or when the ChessBoard size changes
    if (chessBoardRef.current) {
      const handleResize = () => {
        const chessBoardHeight = chessBoardRef.current?.clientHeight || 0;
        setMaxHeight(`${chessBoardHeight}px`);
      };

      // Initial call to set height
      handleResize();

      // Add a resize event listener to adjust height dynamically
      window.addEventListener("resize", handleResize);

      // Cleanup event listener on unmount
      return () => window.removeEventListener("resize", handleResize);
    }
  }, []);

  async function sendMove(start: number[], end: number[]) {
    try {
      const move = {
        startRow: start[0],
        startCol: start[1],
        endRow: end[0],
        endCol: end[1],
        pieceMoved: "",
        pieceCaptured: "",
        enPassant: false,
        pawnPromotion: false,
        isCastleMove: false,
      };
      const jsonString = JSON.stringify(move);
      await connection?.send("SendMove", jsonString);
    } catch (e) {
      console.error("Error sending move: ", e);
    }
  }

  // #region Callback
  const onMoveValidCheck = useCallback(    
    async (start: number[], end: number[]) => {
      const startCopy = [...start];
      const endCopy = [...end];
      if (!isWhite()) {
        startCopy[0] = 7 - start[0];
        startCopy[1] = 7 - start[1];
        endCopy[0] = 7 - end[0];
        endCopy[1] = 7 - end[1];
      }
      const squaresToCode = (s: number[], e: number[]) => s[0] * 1000 + s[1] * 100 + e[0] * 10 + e[1];
      const code = squaresToCode(startCopy, endCopy);
      if (data && data.currentValidMoves) {
        if (data.currentValidMoves.some((m: any) => m.moveID === code)) {
          return true;
        }
        return false;
      }
      throw new Error("no currentValidMoves found")
    },
    [data],
  );

  const onMakeMove = useCallback(
    async (start: number[], end: number[]) => {
      const startCopy = [...start];
      const endCopy = [...end];
      if (!isWhite()) {
        startCopy[0] = 7 - start[0];
        startCopy[1] = 7 - start[1];
        endCopy[0] = 7 - end[0];
        endCopy[1] = 7 - end[1];
      }
      await sendMove(startCopy, endCopy);
    },
    [connection, data],
  );

  const onHighlightSquares = useCallback(
    (): HighlightData => {
      let prevStart: [number, number] | null = null;
      let prevEnd: [number, number] | null = null;
      let check: [number, number] | null = null;
      if (data && data.state && data.state.moveLog && data.state.moveLog.length > 0) {
        prevStart = [
          data.state.moveLog[data.state.moveLog.length - 1].startRow,
          data.state.moveLog[data.state.moveLog.length - 1].startCol
        ];
        prevEnd = [
          data.state.moveLog[data.state.moveLog.length - 1].endRow,
          data.state.moveLog[data.state.moveLog.length - 1].endCol
        ];

        if (data.state.inCheck) {
          if (data.state.whiteToMove) {
            check = data.state.whiteKingLocation;
          } else {
            check = data.state.blackKingLocation;
          }
        }
      }
      
      return {
        "prev-start": prevStart,
        "prev-end": prevEnd,
        "check": check
      };
    },
    [data],
  );

  const isWhite = useCallback(() => {
    if (!data) {
      return true;
    }
    if (data.players) {
      var p = data.players.find((player: any) => player.connectionId === connection?.connectionId);
      if (p) {
        return p.isWhite;
      }
      throw new Error("player not found in data")
    }
    throw new Error("no players data found in data")
  }, [data]);

  const isSinglePlayer = useCallback(() => {
    if (!data) {
      return false;
    }
    return data.settings.isSinglePlayer;
  }, [data])


  const [suggestedMoveSquares, setSuggestedMoveSquares] = useState<Array<Array<number>> | null>(null);
  const displaySuggestedMove = useCallback(() => {
    const moveIdToRowsAndCols = (moveId: number) => {
      const s = moveId.toString().padStart(4, "0");
      return [
        [Number(s[0]), Number(s[1])],
        [Number(s[2]), Number(s[3])]
      ];
    }
    if (data.players[0].isWhite == data.state.whiteToMove && !data.state.checkMate && !data.state.staleMate) {
      setSuggestedMoveSquares(moveIdToRowsAndCols(data.suggestedMoveId));
    }
  }, [data])
  // #endregion

  return (
    <div>
      <h1 className="mb-6 text-xl font-semibold">Play Game!</h1>
      <div className="flex">
        <MessageBox boxSize={(96 - boardSize) / 2} />
        <ChessBoard
          boardSize={boardSize}
          isWhite={isWhite}
          board={board}
          suggestedMoveSquares={suggestedMoveSquares}
          isInteractive={true}
          onMoveValidCheck={onMoveValidCheck}
          onMakeMove={onMakeMove}
          onHighlightSquares={onHighlightSquares}
        />
        <ControlStack
          boxSize={(96 - boardSize) / 2}
          suggestedMoveButton={isSinglePlayer}
          displaySuggestedMove={displaySuggestedMove}
          playerData={playerData}
          moveLogData={moveLogData}
          maxHeight={maxHeight}
        />
      </div>
    </div>
  );
};

export default PlayGame;

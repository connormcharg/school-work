import React, { useState, useEffect, useCallback } from "react";
import { useParams } from "react-router-dom";
import { HubConnectionBuilder, HubConnection } from "@microsoft/signalr";
import ChessBoard from "./ChessBoard";
import MessageBox from "./MessageBox";
import { useAuth } from "../../contexts/AuthProvider";
import ControlStack from "./ControlStack";
import { useNavigate } from "react-router-dom";

interface PlayGameProps {
  boardSize: number;
}

interface HighlightData {
  "prev-start": [number, number] | null;
  "prev-end": [number, number] | null;
  check: [number, number] | null;
}

interface Player {
  nickname: string;
  isWhite: boolean;
  timeLeft: number;
  rating: number;
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
  const [isEndOfGame, setIsEndOfGame] = useState(false);
  const [gameResult, setGameResult] = useState("");
  const [gameReason, setGameReason] = useState("");
  const [gameRatingChange, setGameRatingChange] = useState<string | null>(null);
  const [maxHeight, _] = useState<string>("10rem");
  const navigate = useNavigate();

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
    };

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
        newConnection.on("ReceiveGameOver", (eventData: any) => {
          const gameOverData = JSON.parse(eventData);
          setGameResult(gameOverData.result);
          setGameReason(gameOverData.reason);
          if (gameOverData.ratingChange) {
            setGameRatingChange(gameOverData.ratingChange);
          }
          setIsEndOfGame(true);
        });
      })
      .catch((err: any) =>
        console.log("Error while starting connection: ", err),
      );

    return () => {
      if (newConnection) {
        newConnection.stop().then(() => {
          console.log("Connection stopped.");
        });
        newConnection.off("ReceiveGame");
        newConnection.off("ReceiveGameOver");
      }
    };
  }, []);

  useEffect(() => {
    if (connection && isConnected) {
      connection
        .invoke("JoinGameAsPlayer", id)
        .then(() => {
          console.log("Joined game with ID: ", id);
        })
        .catch((err: any) => {
          console.log("Error joining game: ", err);
          if (err.message.includes("InvalidGameId")) {
            handleGoHome();
          }
        });
    }
  }, [connection, isConnected, id]);

  useEffect(() => {
    if (data && data.state && data.state.board) setBoard(data.state.board);
    if (data?.settings) {
      if (data.settings.isSinglePlayer) {
        if (isWhite()) {
          setPlayerData([
            {
              nickname: "Computer Player",
              isWhite: false,
              timeLeft: data.settings.isTimed ? data.state.blackTime : -1,
              rating: -1,
            },
            {
              nickname: data.players[0].nickName,
              isWhite: true,
              timeLeft: data.settings.isTimed ? data.state.whiteTime : -1,
              rating: data.players[0].rating,
            },
          ]);
        } else {
          setPlayerData([
            {
              nickname: "Computer Player",
              isWhite: true,
              timeLeft: data.settings.isTimed ? data.state.whiteTime : -1,
              rating: -1,
            },
            {
              nickname: data.players[0].nickName,
              isWhite: false,
              timeLeft: data.settings.isTimed ? data.state.blackTime : -1,
              rating: data.players[0].rating,
            },
          ]);
        }
      } else {
        if (isWhite()) {
          var p = data.players.find(
            (player: any) => player.connectionId === connection?.connectionId,
          );
          var p2 =
            data.players.length === 2
              ? data.players[data.players.indexOf(p) === 0 ? 1 : 0]
              : null;
          setPlayerData([
            {
              nickname: p2 ? p2.nickName : "Waiting...",
              isWhite: false,
              timeLeft: data.settings.isTimed ? data.state.blackTime : -1,
              rating: p2 ? p2.rating : -1,
            },
            {
              nickname: p.nickName,
              isWhite: true,
              timeLeft: data.settings.isTimed ? data.state.whiteTime : -1,
              rating: p.rating,
            },
          ]);
        } else {
          var p = data.players.find(
            (player: any) => player.connectionId === connection?.connectionId,
          );
          var p2 =
            data.players.length === 2
              ? data.players[data.players.indexOf(p) === 0 ? 1 : 0]
              : null;
          setPlayerData([
            {
              nickname: p2 ? p2.nickName : "Waiting...",
              isWhite: true,
              timeLeft: data.settings.isTimed ? data.state.whiteTime : -1,
              rating: p2 ? p2.rating : -1,
            },
            {
              nickname: p.nickName,
              isWhite: false,
              timeLeft: data.settings.isTimed ? data.state.blackTime : -1,
              rating: p.rating,
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
    };
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
    };
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

    const result = new Array<string>();

    moveLog.forEach((move) => {
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
  };

  const handleGoHome = () => {
    navigate("/");
  };
  // #endregion

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
      await connection
        ?.invoke("SendMove", jsonString)
        .then(() => null)
        .catch((err: any) => {
          console.log("Error sending move: ", err);
        });
    } catch (e) {
      console.error("Error sending move: ", e);
    }
  }

  // #region Callback
  const onMoveValidCheck = useCallback(
    async (start: number[], end: number[]) => {
      if (isPaused()) {
        return false;
      }
      const startCopy = [...start];
      const endCopy = [...end];
      if (!isWhite()) {
        startCopy[0] = 7 - start[0];
        startCopy[1] = 7 - start[1];
        endCopy[0] = 7 - end[0];
        endCopy[1] = 7 - end[1];
      }
      const squaresToCode = (s: number[], e: number[]) =>
        s[0] * 1000 + s[1] * 100 + e[0] * 10 + e[1];
      const code = squaresToCode(startCopy, endCopy);
      if (data && data.currentValidMoves) {
        if (data.currentValidMoves.some((m: any) => m.moveID === code)) {
          return true;
        }
        return false;
      }
      throw new Error("no currentValidMoves found");
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

  const onHighlightSquares = useCallback((): HighlightData => {
    let prevStart: [number, number] | null = null;
    let prevEnd: [number, number] | null = null;
    let check: [number, number] | null = null;
    if (
      data &&
      data.state &&
      data.state.moveLog &&
      data.state.moveLog.length > 0
    ) {
      prevStart = [
        data.state.moveLog[data.state.moveLog.length - 1].startRow,
        data.state.moveLog[data.state.moveLog.length - 1].startCol,
      ];
      prevEnd = [
        data.state.moveLog[data.state.moveLog.length - 1].endRow,
        data.state.moveLog[data.state.moveLog.length - 1].endCol,
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
      check: check,
    };
  }, [data]);

  const isWhite = useCallback(() => {
    if (!data) {
      return true;
    }
    if (data.players) {
      var p = data.players.find(
        (player: any) => player.connectionId === connection?.connectionId,
      );
      if (p) {
        return p.isWhite;
      }
      throw new Error("player not found in data");
    }
    throw new Error("no players data found in data");
  }, [data]);

  const isSinglePlayer = useCallback(() => {
    if (!data) {
      return false;
    }
    return data.settings.isSinglePlayer;
  }, [data]);

  const isPaused = useCallback(() => {
    if (!data) {
      return false;
    }
    return data.state.pauseAgreed;
  }, [data]);

  const findPauseRequestCount = useCallback(() => {
    if (!data) {
      return 0;
    }
    return data.state.pauseRequests.length;
  }, [data]);

  const findDrawOfferCount = useCallback(() => {
    if (!data) {
      return 0;
    }
    return data.state.drawOffers.length;
  }, [data]);

  const onValidMovesData = useCallback(
    (pieceRow: number, pieceCol: number) => {
      const rowColToSquareClass = (row: number, col: number) => {
        const rowCopy = isWhite() ? row : 7 - row;
        const colCopy = isWhite() ? col : 7 - col;
        return `square-${rowCopy}-${colCopy}`;
      };

      const pieceRowCopy = isWhite() ? pieceRow : 7 - pieceRow;
      const pieceColCopy = isWhite() ? pieceCol : 7 - pieceCol;

      var p = data.players.find(
        (player: any) => player.connectionId === connection?.connectionId,
      );
      if (
        data &&
        p &&
        data.currentValidMoves &&
        data.state.whiteToMove === p.isWhite
      ) {
        var r = (data.currentValidMoves as Array<any>)
          .filter(
            (move) =>
              move.startRow === pieceRowCopy && move.startCol === pieceColCopy,
          )
          .map((move) => rowColToSquareClass(move.endRow, move.endCol));
        return r;
      }
      return new Array<string>();
    },
    [data, connection],
  );

  const [suggestedMoveSquares, setSuggestedMoveSquares] = useState<Array<
    Array<number>
  > | null>(null);
  const displaySuggestedMove = useCallback(() => {
    const moveIdToRowsAndCols = (moveId: number) => {
      const s = moveId.toString().padStart(4, "0");
      return [
        [Number(s[0]), Number(s[1])],
        [Number(s[2]), Number(s[3])],
      ];
    };
    if (
      data.players[0].isWhite == data.state.whiteToMove &&
      !data.state.checkMate &&
      !data.state.staleMate
    ) {
      setSuggestedMoveSquares(moveIdToRowsAndCols(data.suggestedMoveId));
    }
  }, [data]);

  const getBoardTheme = useCallback(async () => {
    try {
      const res = await fetch("/proxy/api/auth/details", {
        method: "GET",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
      });
      if (!res.ok) {
        throw new Error("Failed to get user details");
      }
      const data = await res.json();
      if (data) {
        return data.boardTheme;
      }
    } catch {
      return "blue";
    }
  }, [token, isLoggedIn]);

  const sendResignation = useCallback(() => {
    if (connection && isConnected) {
      connection
        .send("SendResignation")
        .then(() => {
          console.log("Client sent resignation.");
        })
        .catch((err: any) => console.log("Error resigning from game: ", err));
    }
  }, [connection, isConnected, id]);

  const sendPauseRequest = useCallback(() => {
    if (connection && isConnected) {
      connection
        .send("SendPauseRequest")
        .then(() => {
          console.log("Client sent pause request.");
        })
        .catch((err: any) =>
          console.log("Error sending pause request to game: ", err),
        );
    }
  }, [connection, isConnected, id]);

  const sendDrawOffer = useCallback(() => {
    if (connection && isConnected) {
      connection
        .send("SendDrawOffer")
        .then(() => {
          console.log("Client sent draw offer.");
        })
        .catch((err: any) =>
          console.log("Error sending draw offer to game: ", err),
        );
    }
  }, [connection, isConnected, id]);
  // #endregion

  return (
    <div className="relative flex flex-col">
      {isEndOfGame && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black bg-opacity-50">
          <div className="rounded-lg bg-white p-6 text-center">
            <h2 className="mb-4 text-2xl font-bold">{gameResult}</h2>
            <p className="mb-6">{gameReason}</p>
            {gameRatingChange && <p className="mb-6">{gameRatingChange}</p>}
            <button
              onClick={handleGoHome}
              className="rounded bg-blue-500 px-4 py-2 text-white hover:bg-blue-600"
            >
              Go to Home
            </button>
          </div>
        </div>
      )}
      <h1 className="mb-6 text-xl font-semibold">Play Game!</h1>
      <div className="flex">
        {/* <MessageBox boxSize={(96 - boardSize) / 2} /> */}
        <ChessBoard
          boardSize={boardSize}
          isWhite={isWhite}
          board={board}
          suggestedMoveSquares={suggestedMoveSquares}
          isInteractive={true}
          onMoveValidCheck={onMoveValidCheck}
          onMakeMove={onMakeMove}
          onHighlightSquares={onHighlightSquares}
          onValidMovesData={onValidMovesData}
          getBoardTheme={getBoardTheme}
        />
        <ControlStack
          boxSize={(96 - boardSize)}
          suggestedMoveButton={isSinglePlayer}
          displaySuggestedMove={displaySuggestedMove}
          playerData={playerData}
          moveLogData={moveLogData}
          maxHeight={maxHeight}
          sendPauseRequest={sendPauseRequest}
          sendResignation={sendResignation}
          sendDrawOffer={sendDrawOffer}
          isPaused={isPaused}
          findPauseRequestCount={findPauseRequestCount}
          findDrawOfferCount={findDrawOfferCount}
        />
      </div>
    </div>
  );
};

export default PlayGame;

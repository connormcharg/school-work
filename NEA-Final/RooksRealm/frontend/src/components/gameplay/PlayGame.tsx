import React, { useState, useEffect, useCallback } from "react";
import { useParams } from "react-router-dom";
import {
  HubConnectionBuilder,
  HubConnection
} from "@microsoft/signalr";
import ChessBoard from "./ChessBoard";
import MessageBox from "./MessageBox";
import { useAuth } from "../../contexts/AuthProvider";

interface PlayGameProps {
  boardSize: number;
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
      .withUrl("https://localhost:7204/chesshub", {
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
  }, [data]);
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
      await connection?.send("SendMove", jsonString);
    } catch (e) {
      console.error("Error sending move: ", e);
    }
  }

  // #region Callback
  const onMoveValidCheck = useCallback(    
    async (start: number[], end: number[]) => {
      const squaresToCode = (s: number[], e: number[]) => s[0] * 1000 + s[1] * 100 + e[0] * 10 + e[1];
      const code = squaresToCode(start, end);
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
      await sendMove(start, end);
    },
    [connection],
  );

  const isWhite = () => {
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
  }

  const onIsWhiteCheck = useCallback(async () => {
    return isWhite();
  }, [data]);
  // #endregion

  return (
    <div>
      <h1 className="mb-6 text-xl font-semibold">Play Game!</h1>
      <div className="flex">
        <MessageBox boxSize={(96 - boardSize) / 2} />
        <ChessBoard
          boardSize={boardSize}
          isWhite={isWhite()}
          board={board}
          isInteractive={true}
          onMoveValidCheck={onMoveValidCheck}
          onMakeMove={onMakeMove}
          onIsWhiteCheck={onIsWhiteCheck}
        />
        <MessageBox boxSize={(96 - boardSize) / 2} />
      </div>
    </div>
  );
};

export default PlayGame;

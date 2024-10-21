import React, { useState, useEffect, useCallback } from "react";
import { useParams } from "react-router-dom";
import { HubConnectionBuilder, HubConnection, JsonHubProtocol } from "@microsoft/signalr";
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
        ["--", "--", "--", "--", "bq", "--", "--", "--"],
        ["--", "--", "--", "--", "--", "--", "--", "--"],
        ["wp", "wp", "wp", "wp", "wp", "wp", "wp", "wp"],
        ["wr", "wn", "wb", "wq", "wk", "wb", "wn", "wr"]
    ]);
    
    // #region SignalR Connection
    const [data, setData] = useState<any>(null);
    const [connection, setConnection] = useState<HubConnection | null>(null);
    const [isConnected, setIsConnected] = useState(false);

    useEffect(() => {
        const getJwtToken = () => {
            return "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiIzIiwidW5pcXVlX25hbWUiOiJ0ZXN0dXNlciIsIm5iZiI6MTcyOTQ0NTAxNSwiZXhwIjoxNzI5NDQ2ODE1LCJpYXQiOjE3Mjk0NDUwMTV9.F_8o9peiQMsa-vI7h6cXF-9DUiyq1X_Of8xRLFx5JBM";
        }

        const newConnection = new HubConnectionBuilder()
            .withUrl("https://localhost:7204/chesshub", {
                accessTokenFactory: () => getJwtToken(),
            })
            .withAutomaticReconnect()
            .build();
        
        setConnection(newConnection);

        newConnection.start()
            .then(() => {
                console.log("Connection started.");
                setIsConnected(true);
                newConnection.on("ReceiveGame", (eventData: any) => {
                    setData(JSON.parse(eventData));
                });
            })
            .catch((err) => console.log("Error while starting connection: ", err));
        
        return () => {
            if (newConnection) {
                newConnection.stop().then(() => {
                    console.log("Connection stopped.");
                });
                newConnection.off("ReceiveGame");
            }
        }
    }, []);

    useEffect(() => {
        if (connection && isConnected) {
            connection.send("JoinGameAsPlayer", id)
                .then(() => {
                    console.log("Joined game with ID: ", id);
                })
                .catch(err => console.log("Error joining game: ", err));
        }
    }, [connection, isConnected, id]);

    useEffect(() => {
        if (data && data.state && data.state.board)
            setBoard(data.state.board);
    }, [data])
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
                isCastleMove: false
            }
            const jsonString = JSON.stringify(move);
            await connection?.send("SendMove", jsonString)
        } catch (e) {
            console.error("Error sending move: ", e);
        }
    }

    // #region Callback
    const onMoveValidCheck = useCallback(async (start: number[], end: number[]) => {
        return true;
    }, []);

    const onMakeMove = useCallback(async (start: number[], end: number[]) => {
        await sendMove(start, end);
    }, [connection]);

    const onIsWhiteCheck = useCallback(async () => {
        return true;
    }, []);
    // #endregion

    return (
        <div>
            <h1 className="text-xl font-semibold mb-6">Play Game!</h1>
            <button onClick={() => sendMove([6, 4], [4, 4])}>click me!</button>
            <div className="flex">
                <MessageBox boxSize={(96 - boardSize) / 2} />
                <ChessBoard
                    boardSize={boardSize}
                    isWhite={true}
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

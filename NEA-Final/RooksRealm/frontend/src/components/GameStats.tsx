import React, { useEffect, useState } from "react";
import { useAuth } from "../contexts/AuthProvider";

interface Statistic {
  id: number;
  avgMoveTime: number;
  numberOfMoves: number;
  outcome: string;
  datetime: string;
  playerOneUsername: string;
  playerTwoUsername: string | null;
}

const GameStatsBoard: React.FC = () => {
  const [statistics, setStatistics] = useState<Statistic[]>([]);
  const daysAgo = 5;
  const { token, isLoggedIn } = useAuth();

  const fetchStatistics = async () => {
    try {
      if (!isLoggedIn) {
        throw new Error("Not Logged in!");
      }

      const res = await fetch(`/proxy/api/stats?daysAgo=${daysAgo}`, {
        method: "GET",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
      });
      const data = await res.json();
      setStatistics(data.statistics);
    } catch (error) {
      console.error("Failed to fetch statistics:", error);
    }
  };

  useEffect(() => {
    fetchStatistics();
  }, []);

  const parseTimestamp = (timestamp: string): string => {
    const normalizedTimestamp = timestamp.split(".")[0];
    return new Date(normalizedTimestamp).toLocaleString();
  };

  return (
    <div className="flex h-full flex-col bg-gray-200 p-4 text-gray-900">
      <h1 className="mb-4 text-2xl font-bold">Statistics</h1>
      <div className="flex-1 overflow-auto rounded-lg bg-white shadow">
        {statistics.length > 0 ? (
          <div className="space-y-4">
            {statistics
              .sort(
                (a, b) =>
                  new Date(b.datetime).getTime() -
                  new Date(a.datetime).getTime(),
              )
              .map((statistic) => (
                <div
                  key={statistic.id}
                  className="m-3 flex flex-col gap-4 rounded-lg border-2 border-gray-300 p-4 hover:bg-gray-50"
                >
                  <div className="flex items-center justify-between">
                    <h2 className="text-lg font-bold">
                      {statistic.playerOneUsername}
                      {statistic.playerTwoUsername ? (
                        <span> vs {statistic.playerTwoUsername}</span>
                      ) : (
                        <span> vs Computer Player</span>
                      )}
                    </h2>
                    <span className="text-sm text-gray-500">
                      {parseTimestamp(statistic.datetime)}
                    </span>
                  </div>
                  <div className="grid grid-cols-2 gap-4 text-gray-700">
                    <p>
                      <span className="font-medium">Moves:</span>{" "}
                      {statistic.numberOfMoves}
                    </p>
                    <p>
                      <span className="font-medium">Outcome:</span>{" "}
                      {statistic.outcome}
                    </p>
                  </div>
                </div>
              ))}
          </div>
        ) : (
          <p className="text-center text-gray-500">No statistics available</p>
        )}
      </div>
    </div>
  );
};

export default GameStatsBoard;

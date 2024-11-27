import React, { useEffect, useState } from "react";
import { useAuth } from "../contexts/AuthProvider";

interface Statistic {
  id: number;
  username: string;
  title: string;
  content: string;
  datetime: string;
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
    <div className="flex h-full flex-col items-center">
      <h1 className="py-4 text-2xl font-bold">Statistics</h1>
      <div className="w-full max-w-4xl flex-grow space-y-4 overflow-y-auto rounded bg-gray-800 p-4 text-gray-300 shadow">
        {statistics.length > 0 ? (
          statistics
            .sort(
              (a, b) =>
                new Date(b.datetime).getTime() - new Date(a.datetime).getTime(),
            )
            .map((statistic) => (
              <div
                className="rounded-lg border border-gray-500 bg-gray-600 p-4 shadow"
                key={statistic.id}
              >
                <div className="mb-2 flex items-center justify-between">
                  <span className="text-lg font-semibold text-white">
                    {statistic.title}
                  </span>
                  <span className="text-sm">
                    {parseTimestamp(statistic.datetime)}
                  </span>
                </div>
                <div className="mb-4 text-base text-white">
                  {statistic.content}
                </div>
              </div>
            ))
        ) : (
          <p className="text-center text-lg text-gray-500">
            No statistics available
          </p>
        )}
      </div>
    </div>
  );
};

export default GameStatsBoard;

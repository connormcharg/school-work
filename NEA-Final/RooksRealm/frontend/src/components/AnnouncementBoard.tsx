import React, { useEffect, useState } from "react";

interface Message {
  username: string;
  title: string;
  content: string;
  datetime: string;
}

const AnnouncementBoard: React.FC = () => {
  const [messages, setMessages] = useState<Message[]>([]);
  const daysAgo = 5;

  useEffect(() => {
    const fetchMessages = async () => {
      try {
        const response = await fetch(`/proxy/api/message?daysAgo=${daysAgo}`);
        const data = await response.json();
        setMessages(data.messages);
      } catch (error) {
        console.error("Failed to fetch messages:", error);
      }
    };

    fetchMessages();
  }, []);

  const parseTimestamp = (timestamp: string): string => {
    const normalizedTimestamp = timestamp.split(".")[0];
    return new Date(normalizedTimestamp).toLocaleString();
  };

  return (
    <div className="flex h-full flex-col items-center">
      <h1 className="py-4 text-2xl font-bold">Announcements</h1>
      <div className="w-full max-w-4xl flex-grow space-y-4 overflow-y-auto rounded bg-gray-800 p-4 text-gray-300 shadow">
        {messages.length > 0 ? (
          messages
            .sort(
              (a, b) =>
                new Date(b.datetime).getTime() - new Date(a.datetime).getTime(),
            )
            .map((message) => (
              <div
                className="rounded-lg border border-gray-500 bg-gray-600 p-4 shadow"
                key={message.username + message.title}
              >
                <div className="mb-2 flex items-center justify-between">
                  <span className="text-lg font-semibold text-white">
                    {message.title}
                  </span>
                  <span className="text-sm">
                    {parseTimestamp(message.datetime)}
                  </span>
                </div>
                <div className="mb-4 text-base text-white">
                  {message.content}
                </div>
                <div className="text-right text-sm">
                  Posted by: {message.username}
                </div>
              </div>
            ))
        ) : (
          <p className="text-center text-lg text-gray-600">
            No announcements available
          </p>
        )}
      </div>
    </div>
  );
};

export default AnnouncementBoard;

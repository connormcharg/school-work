import React, { useEffect, useState } from "react";
import { useAuth } from "../contexts/AuthProvider";

interface Statistic {
  id: number;
  username: string;
  title: string;
  content: string;
  datetime: string;
}

const AnnouncementBoard: React.FC = () => {
  const [statistics, setStatistics] = useState<Statistic[]>([]);
  const daysAgo = 5;
  const [showPopup, setShowPopup] = useState(false);
  const [role, setRole] = useState("user");
  const { token, isLoggedIn } = useAuth();

  const fetchMessages = async () => {
    try {
      const response = await fetch(`/proxy/api/message?daysAgo=${daysAgo}`);
      const data = await response.json();
      setMessages(data.messages);
    } catch (error) {
      console.error("Failed to fetch messages:", error);
    }
  };

  const fetchRole = async () => {
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
        setRole(data.role);
      }
    } catch {
      console.error("Failed to load user details");
    }
  };

  useEffect(() => {
    fetchMessages();
    fetchRole();
  }, []);

  const parseTimestamp = (timestamp: string): string => {
    const normalizedTimestamp = timestamp.split(".")[0];
    return new Date(normalizedTimestamp).toLocaleString();
  };

  const togglePopup = () => {
    setShowPopup(!showPopup);
  };

  useEffect(() => {
    // Close the popup if ESC is pressed
    function handleKeydown(event: KeyboardEvent) {
      if (event.key === "Escape") {
        setShowPopup(false);
      }
    }

    window.addEventListener("keydown", handleKeydown);

    return () => {
      window.removeEventListener("keydown", handleKeydown);
    };
  }, []);

  const closePopup = () => {
    setShowPopup(false);
  };

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();

    const title = e.currentTarget.elements.namedItem(
      "title",
    ) as HTMLInputElement;
    const content = e.currentTarget.elements.namedItem(
      "content",
    ) as HTMLInputElement;

    try {
      if (!isLoggedIn) {
        throw Error("not logged in");
      }

      const response = await fetch("/proxy/api/message/create", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify({ title: title.value, content: content.value }),
      });

      if (response.ok) {
        setShowPopup(false);
        await fetchMessages();
      } else {
        console.error("Failed to create message");
      }
    } catch (error) {
      console.error("Error:", error);
    }
  };

  const handleDeleteMessage = async (id: number) => {
    try {
      const response = await fetch(`/proxy/api/message/delete?id=${id}`, {
        method: "POST",
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });

      if (response.ok) {
        await fetchMessages();
      } else {
        console.error("Failed to delete message");
      }
    } catch (error) {
      console.error("Error deleting message:", error);
    }
  };

  return (
    <div className="flex h-full flex-col items-center">
      <h1 className="py-4 text-2xl font-bold">Announcements</h1>
      {role === "admin" && (
        <button
          onClick={togglePopup}
          disabled={role !== "admin"}
          className="mb-4 rounded bg-blue-500 px-4 py-2 text-white hover:bg-blue-700"
        >
          Create Post
        </button>
      )}
      {showPopup && (
        <div className="fixed inset-0 flex items-center justify-center shadow-lg">
          <div className="relative w-full max-w-md rounded-lg bg-white p-8 shadow-lg">
            <button
              onClick={closePopup}
              className="absolute right-2 top-2 rounded-full bg-gray-300 px-1 py-1 text-gray-700 hover:bg-gray-400"
            >
              <XMarkIcon className="h-6 w-6" />
            </button>
            <h2 className="mb-4 text-xl font-bold">Create Post</h2>
            <form onSubmit={handleSubmit}>
              <div className="mb-4">
                <label
                  htmlFor="title"
                  className="block text-sm font-medium text-gray-700"
                >
                  Title
                </label>
                <input
                  type="text"
                  id="title"
                  name="title"
                  required
                  className="mt-1 block w-full rounded-md border p-2 shadow-sm focus:border-blue-500 focus:outline-none focus:ring-blue-500 sm:text-sm"
                />
              </div>
              <div className="mb-4">
                <label
                  htmlFor="content"
                  className="block text-sm font-medium text-gray-700"
                >
                  Content
                </label>
                <textarea
                  id="content"
                  name="content"
                  rows={4}
                  required
                  className="mt-1 block w-full rounded-md border p-2 shadow-sm focus:border-blue-500 focus:outline-none focus:ring-blue-500 sm:text-sm"
                />
              </div>
              <button
                type="submit"
                className="mt-2 rounded bg-blue-500 px-4 py-2 text-white hover:bg-blue-700"
              >
                Submit
              </button>
            </form>
          </div>
        </div>
      )}
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
                key={message.id}
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
                <div className="flex items-center justify-between">
                  {role === "admin" && (
                    <button
                      onClick={() => handleDeleteMessage(message.id)}
                      className="mt-2 rounded bg-red-500 px-4 py-2 text-white hover:bg-red-700"
                    >
                      <TrashIcon className="h-6 w-6" />
                    </button>
                  )}
                  <span className="text-sm">Posted by: {message.username}</span>
                </div>
              </div>
            ))
        ) : (
          <p className="text-center text-lg text-gray-500">
            No announcements available
          </p>
        )}
      </div>
    </div>
  );
};

export default AnnouncementBoard;

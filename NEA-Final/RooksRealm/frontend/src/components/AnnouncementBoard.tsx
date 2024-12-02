import React, { useEffect, useState } from "react";
import { XMarkIcon, TrashIcon } from "@heroicons/react/24/solid";
import { useAuth } from "../contexts/AuthProvider";

interface Message {
  id: number;
  username: string;
  title: string;
  content: string;
  datetime: string;
}

const AnnouncementBoard: React.FC = () => {
  const [messages, setMessages] = useState<Message[]>([]);
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
    <div className="flex h-full flex-col bg-gray-200 p-4 text-gray-900">
      <h1 className="mb-4 text-2xl font-bold">Messages</h1>
      {role === "admin" && (
        <button
          onClick={togglePopup}
          className="mb-4 rounded bg-blue-500 px-4 py-2 text-white hover:bg-blue-600"
        >
          Create Post
        </button>
      )}
      {showPopup && (
        <div className="fixed inset-0 flex items-center justify-center bg-black bg-opacity-50">
          <div className="relative w-full max-w-lg rounded-lg bg-white p-6 shadow-lg">
            <button
              onClick={closePopup}
              className="absolute right-2 top-2 text-gray-500 hover:text-gray-800"
            >
              <XMarkIcon className="h-6 w-6" />
            </button>
            <h2 className="mb-4 text-xl font-semibold">Create Post</h2>
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
                  className="mt-1 block w-full rounded border border-gray-300 p-2 focus:border-blue-500 focus:ring-blue-500"
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
                  className="mt-1 block w-full rounded border border-gray-300 p-2 focus:border-blue-500 focus:ring-blue-500"
                />
              </div>
              <button
                type="submit"
                className="w-full rounded bg-green-500 px-4 py-2 text-white hover:bg-green-600"
              >
                Submit
              </button>
            </form>
          </div>
        </div>
      )}
      <div className="flex-1 overflow-auto rounded-lg bg-white shadow">
        {messages.length > 0 ? (
          messages
            .sort(
              (a, b) =>
                new Date(b.datetime).getTime() - new Date(a.datetime).getTime(),
            )
            .map((message) => (
              <div
                key={message.id}
                className="border-b border-gray-300 p-4 hover:bg-gray-100"
              >
                <div className="flex items-center justify-between">
                  <h3 className="text-lg font-bold">{message.title}</h3>
                  <span className="text-sm text-gray-500">
                    {parseTimestamp(message.datetime)}
                  </span>
                </div>
                <p className="mt-2 text-gray-700">{message.content}</p>
                <div className="mt-4 flex items-center justify-between">
                  <span className="text-sm text-gray-500">
                    Posted by: {message.username}
                  </span>
                  {role === "admin" && (
                    <button
                      onClick={() => handleDeleteMessage(message.id)}
                      className="rounded bg-red-500 px-4 py-2 text-white hover:bg-red-600"
                    >
                      <TrashIcon className="h-5 w-5" />
                    </button>
                  )}
                </div>
              </div>
            ))
        ) : (
          <p className="text-center text-gray-500">No messages available</p>
        )}
      </div>
    </div>
  );
};

export default AnnouncementBoard;

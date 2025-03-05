import React, { useState, useEffect } from "react";
import { useAuth } from "../../contexts/AuthProvider";
import { useToast } from "@chakra-ui/react";
import DOMPurify from "dompurify";
interface UserDetails {
  username: string;
  email: string;
  boardTheme: string;
  rating: number;
}

const Account: React.FC = () => {
  const {
    token,
    isLoggedIn,
    changeEmail,
    changePassword,
    changeUsername,
    deleteAccount,
    changeTheme,
  } = useAuth();
  const [userDetails, setUserDetails] = useState<UserDetails | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [activeTab, setActiveTab] = useState("profile");
  const toast = useToast();

  const fetchUserDetails = async () => {
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
      setUserDetails(data);
      if (data) {
        setUsername(data.username);
        setEmail(data.email);
        setTheme(data.boardTheme);
        setThemePreview(`/images/boards/${data.boardTheme}.png`);
        setRating(data.rating);
      }
    } catch {
      setError("Failed to load user details");
    } finally {
      setLoading(false);
    }
  };


  useEffect(() => {
    if (isLoggedIn) {
      fetchUserDetails();
    } else {
      setError("No Authorization token found");
      setLoading(false);
    }
  }, [token, isLoggedIn]);

  // Validation functions
  const validateEmail = (email: string) =>
    /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email);
  const validatePassword = (password: string) => password.length >= 8;
  // const validateTheme = async (t: string) => {
  //   try {
  //     const res = await fetch(`/images/boards/${t}.png`);
  //     return res.ok;
  //   } catch (error: any) {
  //     console.error("Error checking file existence: ", error);
  //     return false;
  //   }
  // };
  const validateTheme = (t: string) => validThemes.includes(t);

  // Constants
  const validThemes = [
    "8-bit",
    "blue",
    "brown",
    "bubblegum",
    "checkers",
    "green",
    "light",
    "orange",
    "purple",
    "red",
    "sky",
    "tan",
  ];

  // State
  const [username, setUsername] = useState("");
  const [email, setEmail] = useState("");
  const [theme, setTheme] = useState("");
  const [themePreview, setThemePreview] = useState("");
  const [rating, setRating] = useState("");
  const [oldPassword, setOldPassword] = useState("");
  const [newPassword, setNewPassword] = useState("");

  const handleThemeChange = async (e: React.ChangeEvent<HTMLSelectElement>) => {
    const selectedTheme = e.target.value;
    setTheme(selectedTheme);
    const isValidTheme = await validateTheme(selectedTheme);
    if (isValidTheme) {
      setThemePreview(
        DOMPurify.sanitize(`/images/boards/${selectedTheme}.png`),
      );
    } else {
      setThemePreview("");
    }
  };

  const renderTabContent = () => {
    if (activeTab === "profile" && userDetails) {
      return (
        <form
          onSubmit={async (e) => {
            e.preventDefault();
            changeUsername(username);
            if (!validateTheme(theme)) {
              toast({
                title: "Invalid theme",
                status: "error",
                duration: 4000,
                isClosable: true,
                position: "bottom",
              });
              return;
            }
            changeTheme(theme);
            await new Promise((resolve) => setTimeout(resolve, 100));
            fetchUserDetails();
          }}
          className="space-y-4"
        >
          <input
            type="text"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            className="w-full rounded-md border border-gray-300 p-2 focus:border-blue-500 focus:outline-none"
            placeholder="Enter new username"
          />
          <div className="flex flex-col items-center gap-4 md:flex-row">
            <select
              value={theme}
              onChange={handleThemeChange}
              className="w-full rounded-md border border-gray-300 bg-white p-2 focus:border-blue-500 focus:outline-none"
            >
              <option value="">Select theme</option>
              <option value="8-bit">8 Bit</option>
              <option value="blue">Blue</option>
              <option value="brown">Brown</option>
              <option value="bubblegum">Bubblegum</option>
              <option value="checkers">Checkers</option>
              <option value="green">Green</option>
              <option value="light">Light</option>
              <option value="orange">Orange</option>
              <option value="purple">Purple</option>
              <option value="red">Red</option>
              <option value="sky">Sky</option>
              <option value="tan">Tan</option>
            </select>
            {themePreview && (
              <div className="h-16 w-16 flex-shrink-0 overflow-hidden rounded-md">
                <img
                  src={themePreview}
                  alt="Board Preview"
                  className="h-64 w-64 object-cover"
                  style={{ objectPosition: "0 0" }}
                />
              </div>
            )}
          </div>
          <input
            type="text"
            value={`Rating: ${rating}`}
            disabled
            className="w-full rounded-md border border-gray-300 bg-gray-100 p-2 text-gray-600"
            readOnly
          />
          <button
            type="submit"
            className="w-full rounded-md bg-blue-500 py-2 text-white transition hover:bg-blue-600"
          >
            Update Profile
          </button>
        </form>
      );
    }

    if (activeTab === "email" && userDetails) {
      return (
        <form
          onSubmit={(e) => {
            e.preventDefault();
            if (!validateEmail(email)) {
              toast({
                title: "Invalid email",
                status: "error",
                duration: 4000,
                isClosable: true,
                position: "bottom",
              });
              return;
            }
            changeEmail(email);
          }}
          className="space-y-4"
        >
          <input
            type="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            className="w-full rounded-md border border-gray-300 p-2 focus:border-blue-500 focus:outline-none"
            placeholder="Enter new email"
          />
          <button
            type="submit"
            className="w-full rounded-md bg-blue-500 py-2 text-white transition hover:bg-blue-600"
          >
            Update Email
          </button>
        </form>
      );
    }

    if (activeTab === "password") {
      return (
        <form
          onSubmit={(e) => {
            e.preventDefault();
            if (!validatePassword(newPassword)) {
              toast({
                title: "Invalid password",
                status: "error",
                duration: 4000,
                isClosable: true,
                position: "bottom",
              });
              return;
            }
            changePassword(oldPassword, newPassword);
            setOldPassword("");
            setNewPassword("");
          }}
          className="space-y-4"
        >
          <input
            type="password"
            placeholder="Old Password"
            value={oldPassword}
            onChange={(e) => setOldPassword(e.target.value)}
            className="w-full rounded-md border border-gray-300 p-2 focus:border-blue-500 focus:outline-none"
          />
          <input
            type="password"
            placeholder="New Password"
            value={newPassword}
            onChange={(e) => setNewPassword(e.target.value)}
            className="w-full rounded-md border border-gray-300 p-2 focus:border-blue-500 focus:outline-none"
          />
          <button
            type="submit"
            className="w-full rounded-md bg-blue-500 py-2 text-white transition hover:bg-blue-600"
          >
            Change Password
          </button>
        </form>
      );
    }

    if (activeTab === "delete") {
      return (
        <button
          onClick={() => deleteAccount()}
          className="w-full rounded-md bg-red-500 py-2 text-white transition hover:bg-red-600"
        >
          Delete Account
        </button>
      );
    }

    return null;
  };

  return (
    <div className="mx-auto w-full md:w-80">
      <h1 className="mb-6 text-center text-xl font-semibold">Account</h1>
      <div className="mb-4 flex justify-around border-b border-gray-300">
        {["profile", "email", "password", "delete"].map((tab) => (
          <button
            key={tab}
            className={`pb-2 ${
              activeTab === tab
                ? "border-b-2 border-blue-500 text-blue-500"
                : "text-gray-500 transition hover:text-blue-500"
            }`}
            onClick={() => setActiveTab(tab)}
          >
            {tab.charAt(0).toUpperCase() + tab.slice(1)}
          </button>
        ))}
      </div>
      <div className="w-full space-y-4 md:w-80">
        {loading ? (
          <p className="text-center text-gray-500">Loading...</p>
        ) : (
          renderTabContent()
        )}
        {error && <p className="text-center text-red-500">{error}</p>}
      </div>
    </div>
  );
};

export default Account;

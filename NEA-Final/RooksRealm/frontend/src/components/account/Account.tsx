import React, { useState, useEffect } from "react";
import { useAuth } from "../../contexts/AuthProvider";
import { useToast } from "@chakra-ui/react";

interface UserDetails {
  username: string;
  email: string;
  boardTheme: string;
}

const Account: React.FC = () => {
  const { token, isLoggedIn, changeEmail, changePassword, changeUsername, deleteAccount, changeTheme } = useAuth();
  const [userDetails, setUserDetails] = useState<UserDetails | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [activeTab, setActiveTab] = useState("profile");
  const toast = useToast();

  useEffect(() => {
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
        }
      } catch {
        setError("Failed to load user details");
      } finally {
        setLoading(false);
      }
    };

    if (isLoggedIn) {
      fetchUserDetails();
    } else {
      setError("No Authorization token found");
      setLoading(false);
    }
  }, [token, isLoggedIn]);

  // Validation functions
  const validateEmail = (email: string) => /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email);
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
  const [oldPassword, setOldPassword] = useState("");
  const [newPassword, setNewPassword] = useState("");

  const handleThemeChange = async (e: React.ChangeEvent<HTMLSelectElement>) => {
    const selectedTheme = e.target.value;
    setTheme(selectedTheme);
    const isValidTheme = await validateTheme(selectedTheme);
    if (isValidTheme) {
      setThemePreview(`/images/boards/${selectedTheme}.png`);
    } else {
      setThemePreview("");
    }
  };

  const renderTabContent = () => {
    if (activeTab === "profile" && userDetails) {
      return (
        <form
          onSubmit={(e) => {
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
          }}
          className="space-y-4"
        >
          <input
            type="text"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            className="w-full p-2 border border-gray-300 rounded-md focus:outline-none focus:border-blue-500"
            placeholder="Enter new username"
          />
          <div className="flex flex-col md:flex-row items-center gap-4">
            <select
              value={theme}
              onChange={handleThemeChange}
              className="w-full p-2 border border-gray-300 bg-white rounded-md focus:outline-none focus:border-blue-500"
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
              <div className="w-16 h-16 overflow-hidden rounded-md flex-shrink-0">
                <img
                  src={themePreview}
                  alt="Board Preview"
                  className="w-64 h-64 object-cover"
                  style={{ objectPosition: '0 0' }}
                />
              </div>
            )}
          </div>
          <input
            type="text"
            value="Rating: 1200"
            disabled
            className="w-full p-2 border border-gray-300 rounded-md bg-gray-100 text-gray-600"
            readOnly
          />
          <button
            type="submit"
            className="w-full bg-blue-500 text-white py-2 rounded-md hover:bg-blue-600 transition"
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
            className="w-full p-2 border border-gray-300 rounded-md focus:outline-none focus:border-blue-500"
            placeholder="Enter new email"
          />
          <button
            type="submit"
            className="w-full bg-blue-500 text-white py-2 rounded-md hover:bg-blue-600 transition"
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
            className="w-full p-2 border border-gray-300 rounded-md focus:outline-none focus:border-blue-500"
          />
          <input
            type="password"
            placeholder="New Password"
            value={newPassword}
            onChange={(e) => setNewPassword(e.target.value)}
            className="w-full p-2 border border-gray-300 rounded-md focus:outline-none focus:border-blue-500"
          />
          <button
            type="submit"
            className="w-full bg-blue-500 text-white py-2 rounded-md hover:bg-blue-600 transition"
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
          className="w-full bg-red-500 text-white py-2 rounded-md hover:bg-red-600 transition"
        >
          Delete Account
        </button>
      );
    }
  
    return null;
  };

  return (
    <div className="w-full md:w-80 mx-auto">
      <h1 className="text-xl font-semibold mb-6 text-center">Account</h1>
      <div className="flex justify-around mb-4 border-b border-gray-300">
        {["profile", "email", "password", "delete"].map((tab) => (
          <button
            key={tab}
            className={`pb-2 ${
              activeTab === tab
                ? "border-b-2 border-blue-500 text-blue-500"
                : "text-gray-500 hover:text-blue-500 transition"
            }`}
            onClick={() => setActiveTab(tab)}
          >
            {tab.charAt(0).toUpperCase() + tab.slice(1)}
          </button>
        ))}
      </div>
      <div className="w-full md:w-80 space-y-4">
        {loading ? (
          <p className="text-center text-gray-500">Loading...</p>
        ) : (
          renderTabContent()
        )}
        {error && <p className="text-red-500 text-center">{error}</p>}
      </div>
    </div>
  );
};

export default Account;

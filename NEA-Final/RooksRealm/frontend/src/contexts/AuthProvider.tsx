import {
  FC,
  PropsWithChildren,
  createContext,
  useContext,
  useEffect,
} from "react";
import { useLocalStorage } from "@uidotdev/usehooks";
import { ToastPosition, useToast } from "@chakra-ui/react";
import { jwtDecode } from "jwt-decode";

interface AuthContextType {
  token: string;
  isLoggedIn: boolean;
  logout: () => void;
  login: (email: string, password: string) => Promise<void>;
  register: (email: string, password: string) => Promise<void>;
  changeEmail: (newEmail: string) => Promise<void>;
  changeUsername: (newUsername: string) => Promise<void>;
  changePassword: (oldPassword: string, newPassword: string) => Promise<void>;
  deleteAccount: () => Promise<void>;
  changeTheme: (newTheme: string) => Promise<void>;
}

interface DecodedToken {
  exp: number;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider: FC<PropsWithChildren> = ({ children }) => {
  const [token, setToken] = useLocalStorage("token", "");
  const toast = useToast();

  const toastPosition: ToastPosition = "bottom";
  const toastDuration: number = 4000;

  const validateToken = async () => {
    if (!token) {
      return false;
    }

    try {
      const decoded: DecodedToken = jwtDecode(token);
      const currentTime = Math.floor(Date.now() / 1000);

      if (decoded.exp < currentTime) {
        logout(true);
        return false;
      } else if (decoded.exp - currentTime < 300) {
        await refreshToken();
      }
      return true;
    } catch {
      logout(true);
      return false;
    }
  };

  useEffect(() => {
    const interval = setInterval(() => {
      if (!validateToken()) {
        clearInterval(interval);
      }
    }, 60000);

    return () => clearInterval(interval);
  }, [token]);

  const logout = (tokenExpired: boolean = false) => {
    setToken("");
    toast({
      title: "Logged out.",
      description: tokenExpired
        ? "Your session has expired. Please log in again."
        : "You have successfully logged out.",
      status: "success",
      duration: toastDuration,
      isClosable: true,
      position: toastPosition,
    });
  };

  const refreshToken = async () => {
    try {
      const res = await fetch("/proxy/api/auth/refresh", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
      });

      if (!res.ok) {
        throw new Error("Failed to refresh token");
      }

      const newToken = await res.json();
      setToken(newToken["token"]);
    } catch (error) {
      console.error("Error refreshing token:", error);
      logout();
    }
  };

  const handleLoginResponse = async (res: Response) => {
    if (!res.ok) {
      throw new Error((await res.json()).message || "An error occurred.");
    }

    const data = await res.json();
    setToken(data["token"]);
    toast({
      title: "Logged in.",
      description: "You have successfully logged in.",
      status: "success",
      duration: toastDuration,
      isClosable: true,
      position: toastPosition,
    });
  };

  const handleResponseError = (error: any) => {
    toast({
      title: "Error Occurred",
      description: error.message || "An error occurred.",
      status: "error",
      duration: toastDuration,
      isClosable: true,
      position: toastPosition,
    });
  };

  const handleLogin = async (email: string, password: string) => {
    try {
      const res = await fetch("/proxy/api/auth/login", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          email,
          password,
        }),
      });
      await handleLoginResponse(res);
    } catch (error) {
      handleResponseError(error);
    }
  };

  const handleRegister = async (email: string, password: string) => {
    try {
      const res = await fetch("/proxy/api/auth/register", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          email,
          password,
        }),
      });

      if (!res.ok) {
        throw new Error((await res.json()).message || "An error occurred.");
      }

      toast({
        title: "Registered.",
        description: "You have successfully registered.",
        status: "success",
        duration: toastDuration,
        isClosable: true,
        position: toastPosition,
      });
    } catch (error) {
      handleResponseError(error);
    }
  };

  const handleAccountAction = async (
    url: string,
    body: any,
    description: string,
  ) => {
    try {
      const res = await fetch(url, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify(body),
      });

      if (!res.ok) {
        throw new Error((await res.text()) || "An error occurred.");
      }

      toast({
        title: description,
        status: "success",
        duration: toastDuration,
        isClosable: true,
        position: toastPosition,
      });
    } catch (error) {
      handleResponseError(error);
    }
  };

  return (
    <AuthContext.Provider
      value={{
        token,
        isLoggedIn: !!token,
        logout,
        login: handleLogin,
        register: handleRegister,
        changeEmail: async (newEmail: string) =>
          await handleAccountAction(
            "/proxy/api/auth/changeEmail",
            { newEmail },
            "Email Updated",
          ),
        changeUsername: async (newUsername: string) =>
          await handleAccountAction(
            "/proxy/api/auth/changeUsername",
            { newUsername },
            "Username Updated",
          ),
        changePassword: async (oldPassword: string, newPassword: string) => {
          if (oldPassword && newPassword) {
            handleAccountAction(
              "/proxy/api/auth/changePassword",
              { oldPassword, newPassword },
              "Password Updated",
            );
            logout();
          } else {
            toast({
              title: "Error Occurred",
              description: "Old or new password(s) left blank.",
              status: "error",
              duration: toastDuration,
              isClosable: true,
              position: toastPosition,
            });
          }
        },
        deleteAccount: async () => {
          handleAccountAction("/proxy/api/auth/delete", {}, "Account Deleted");
          logout();
        },
        changeTheme: async (newTheme: string) =>
          await handleAccountAction(
            "/proxy/api/auth/changeTheme",
            { newTheme },
            "Theme Changed",
          ),
      }}
    >
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const i = useContext(AuthContext);
  if (i === undefined) {
    throw new Error("useAuth must be used within an AuthProvider");
  }
  return i;
};

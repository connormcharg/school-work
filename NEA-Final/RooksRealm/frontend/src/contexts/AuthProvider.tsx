import { FC, PropsWithChildren, createContext, useContext, useEffect } from "react";
import { useLocalStorage } from "@uidotdev/usehooks";
import { ToastPosition, useToast } from "@chakra-ui/react";
import { jwtDecode } from "jwt-decode";

interface AuthContextType {
  token: string;
  isLoggedIn: boolean;
  logout: () => void;
  login: (email: string, password: string) => Promise<void>;
  register: (email: string, password: string) => Promise<void>;
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
        refreshToken();
      }
      return true;
    } catch {
      logout(true);
      return false;
    }
  }

  useEffect(() => {
    const interval = setInterval(() => {
      if (!validateToken()) {
        clearInterval(interval);
      }
    }, 60000);

    return () => clearInterval(interval);
  }, [token])
  
  const logout = (tokenExpired: boolean = false) => {
    setToken("");
    toast({
      title: "Logged out.",
      description: tokenExpired ? "Your session has expired. Please log in again." : "You have successfully logged out.",
      status: "success",
      duration: toastDuration,
      isClosable: true,
      position: toastPosition,
    });
  }

  const refreshToken = async () => {
    try {
      const res = await fetch("https://localhost:7204/api/auth/refresh", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          "Authorization": `Bearer ${token}`
        }
      });

      if (!res.ok) {
        throw new Error("Failed to refresh token")
      }

      const newToken = await res.json();
      setToken(newToken["token"]);
    } catch (error) {
      console.error("Error refreshing token:", error);
      logout();
    }
  }

  return (
    <AuthContext.Provider
      value={{
        token,
        isLoggedIn: !!token,
        logout,
        login(email, password) {
          return new Promise((resolve, reject) => {
            fetch("https://localhost:7204/api/auth/login", {
              method: "POST",
              headers: {
                "Content-Type": "application/json",
              },
              body: JSON.stringify({
                email: email,
                password: password,
              }),
            }).then(async (res) => {
              if (!res.ok) {
                toast({
                  title: "Error occured logging in.",
                  description:
                    (await res.json()).message || "An error occurred.",
                  status: "error",
                  duration: toastDuration,
                  isClosable: true,
                  position: toastPosition,
                });
                reject();
              } else {
                setToken((await res.json())["token"]);
                toast({
                  title: "Logged in.",
                  description: "You have successfully logged in.",
                  status: "success",
                  duration: toastDuration,
                  isClosable: true,
                  position: toastPosition,
                });
                resolve();
              }
            }).catch(() => {
              toast({
                title: "Error occured logging in.",
                description: "An error occurred.",
                status: "error",
                duration: toastDuration,
                isClosable: true,
                position: toastPosition,
              });
              reject();
            });
          });
        },
        register(email, password) {
          return new Promise((resolve, reject) => {
            fetch("https://localhost:7204/api/auth/register", {
              method: "POST",
              headers: {
                "Content-Type": "application/json",
              },
              body: JSON.stringify({
                email: email,
                password: password,
              }),
            })
              .then(async (res) => {
                if (!res.ok) {
                  toast({
                    title: "Error occured registering.",
                    description:
                      (await res.json()).message || "An error occurred.",
                    status: "error",
                    duration: toastDuration,
                    isClosable: true,
                    position: toastPosition,
                  });
                  reject();
                } else {
                  // setToken(await res.json());
                  toast({
                    title: "Registered.",
                    description: "You have successfully registered.",
                    status: "success",
                    duration: toastDuration,
                    isClosable: true,
                    position: toastPosition,
                  });
                  resolve();
                }
              })
              .catch(() => {
                toast({
                  title: "Error occured logging in.",
                  description: "An error occurred.",
                  status: "error",
                  duration: toastDuration,
                  isClosable: true,
                  position: toastPosition,
                });
                reject();
              });
          });
        },
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

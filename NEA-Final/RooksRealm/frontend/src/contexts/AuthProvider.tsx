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
      const res = await fetch("/proxy/api/auth/refresh", {
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

  const changeEmail = async (newEmail: string) => {
    try {
      const res = await fetch("/proxy/api/auth/changeEmail", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          "Authorization": `Bearer ${token}`,
        },
        body: JSON.stringify({ newEmail }),
      });

      if (!res.ok) {
        throw new Error("Failed to change email");
      }

      toast({
        title: "Email Updated",
        description: "Your email has been successfully updated.",
        status: "success",
        duration: toastDuration,
        isClosable: true,
        position: toastPosition,
      });
    } catch (error: any) {
      toast({
        title: "Error Changing Email",
        description: error.message || "An error occurred.",
        status: "error",
        duration: toastDuration,
        isClosable: true,
        position: toastPosition,
      });
    }
  };

  const changeUsername = async (newUsername: string) => {
    try {
      const res = await fetch("/proxy/api/auth/changeUsername", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          "Authorization": `Bearer ${token}`,
        },
        body: JSON.stringify({ newUsername }),
      });

      if (!res.ok) {
        throw new Error("Failed to change username");
      }

      toast({
        title: "Username Updated",
        description: "Your username has been successfully updated.",
        status: "success",
        duration: toastDuration,
        isClosable: true,
        position: toastPosition,
      });
    } catch (error: any) {
      toast({
        title: "Error Changing Username",
        description: error.message || "An error occurred.",
        status: "error",
        duration: toastDuration,
        isClosable: true,
        position: toastPosition,
      });
    }
  };

  const changePassword = async (oldPassword: string, newPassword: string) => {
    try {
      const res = await fetch("/proxy/api/auth/changePassword", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          "Authorization": `Bearer ${token}`,
        },
        body: JSON.stringify({ oldPassword, newPassword }),
      });

      if (!res.ok) {
        throw new Error("Failed to change password");
      }

      toast({
        title: "Password Updated",
        description: "Your password has been successfully updated.",
        status: "success",
        duration: toastDuration,
        isClosable: true,
        position: toastPosition,
      });

      logout();
    } catch (error: any) {
      toast({
        title: "Error Changing Password",
        description: error.message || "An error occurred.",
        status: "error",
        duration: toastDuration,
        isClosable: true,
        position: toastPosition,
      });
    }
  };

  const deleteAccount = async () => {
    try {
      const res = await fetch("/proxy/api/auth/delete", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          "Authorization": `Bearer ${token}`,
        }
      });

      if (!res.ok) {
        throw new Error("Failed to delete account.");
      }

      toast({
        title: "Account Deleted",
        description: "Your account has been successfully deleted.",
        status: "success",
        duration: toastDuration,
        isClosable: true,
        position: toastPosition,
      });

      logout();
    } catch (error: any) {
      toast({
        title: "Error Deleting Account",
        description: error.message || "An error occurred.",
        status: "error",
        duration: toastDuration,
        isClosable: true,
        position: toastPosition,
      });
    }
  };

  const changeTheme = async (newTheme: string) => {
    try {
      const res = await fetch("/proxy/api/auth/changeTheme", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          "Authorization": `Bearer ${token}`,
        },
        body: JSON.stringify({ newTheme }),
      });

      if (!res.ok) {
        throw new Error("Failed to change theme.");
      }

      toast({
        title: "Theme Changed",
        description: "Your theme has been successfully changed.",
        status: "success",
        duration: toastDuration,
        isClosable: true,
        position: toastPosition,
      });
    } catch (error: any) {
      toast({
        title: "Error Changing Theme",
        description: error.message || "An error occurred.",
        status: "error",
        duration: toastDuration,
        isClosable: true,
        position: toastPosition,
      });
    }
  };

  return (
    <AuthContext.Provider
      value={{
        token,
        isLoggedIn: !!token,
        logout,
        login(email, password) {
          return new Promise((resolve, reject) => {
            fetch("/proxy/api/auth/login", {
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
            fetch("/proxy/api/auth/register", {
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
        changeEmail,
        changeUsername,
        changePassword,
        deleteAccount,
        changeTheme,
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

import { FC, PropsWithChildren, createContext, useContext } from "react";
import { useLocalStorage } from "@uidotdev/usehooks";
import { ToastPosition, useToast } from "@chakra-ui/react";

interface AuthContextType {
  token: string;
  isLoggedIn: boolean;
  logout: () => void;
  login: (email: string, password: string) => Promise<void>;
  register: (email: string, password: string) => Promise<void>;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider: FC<PropsWithChildren> = ({ children }) => {
	const [token, setToken] = useLocalStorage("token", "");
  const toast = useToast();

  const toastPosition: ToastPosition = "bottom";
  const toastDuration: number = 4000;

  return (
    <AuthContext.Provider
      value={{
        token,
        isLoggedIn: !!token,
        logout() {
          setToken("");
          toast({
            title: "Logged out.",
            description: "You have successfully logged out.",
            status: "success",
            duration: toastDuration,
            isClosable: true,
            position: toastPosition,
          });
        },
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
                setToken(await res.json());
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

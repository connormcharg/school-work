import React, { FormEventHandler, useEffect, useState } from "react";
import { useNavigate } from "react-router-dom"
import { useAuth } from "../../contexts/AuthProvider";
import { useToast } from "@chakra-ui/react";

const Register: React.FC = () => {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const navigate = useNavigate();
  const { isLoggedIn, register, login } = useAuth();
  const toast = useToast();

  const validateEmail = (email: string) => /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email);

  const handleRegister: FormEventHandler<HTMLFormElement> = async (e) => {
    try {
      e.preventDefault();
      if (validateEmail(email)) {
        await register(email, password);
        await login(email, password);
      } else {
        throw new Error("Email not valid.")
      }
    } catch (error) {
      toast({
        title: "Invalid Email Address",
        status: "error",
        duration: 4000,
        isClosable: true,
        position: "bottom",
      });
    }
  }

  useEffect(() => {
    if (isLoggedIn) {
      navigate("/");
    }
  }, [isLoggedIn, navigate])

  return (
    <div className="w-full md:w-80 mx-auto">
      <h1 className="text-xl font-semibold mb-6 text-center">Register</h1>
      <form
        className="flex-1 flex flex-col items-left justify-center p-4 w-full md:w-80"
        onSubmit={handleRegister}
      >
        <input
          type="text"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          placeholder="Enter email address"
          className="mb-4 border border-gray-300 rounded p-2 w-full"
        />
        <input
          type="password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          placeholder="Enter password"
          className="mb-4 border border-gray-300 rounded p-2 w-full"
        />
        <button
          type="submit"
          className="bg-blue-500 text-white py-2 px-4 rounded hover:bg-blue-600 w-[50%] self-center"
        >
          Register
        </button>
      </form>
    </div>
  );
}

export default Register;
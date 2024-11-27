import React, { FormEventHandler, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../../contexts/AuthProvider";
import { useToast } from "@chakra-ui/react";
import useInput from "../../hooks/useInput";

const RegisterForm: React.FC = () => {
  const navigate = useNavigate();
  const { isLoggedIn, register, login } = useAuth();
  const toast = useToast();

  const validateEmail = (email: string) =>
    /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email);

  const handleRegister: FormEventHandler<HTMLFormElement> = async (e) => {
    try {
      e.preventDefault();
      if (validateEmail(email.value)) {
        await register(email.value, password.value);
        await login(email.value, password.value);
      } else {
        throw new Error("Email not valid.");
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
  };

  useEffect(() => {
    if (isLoggedIn) {
      navigate("/");
    }
  }, [isLoggedIn, navigate]);

  const email = useInput("");
  const password = useInput("");

  return (
    <div className="mx-auto w-full md:w-80">
      <h1 className="mb-6 text-center text-xl font-semibold">Register</h1>
      <form
        className="items-left flex w-full flex-1 flex-col justify-center p-4 md:w-80"
        onSubmit={handleRegister}
      >
        <input
          type="email"
          value={email.value}
          onChange={email.onChange}
          placeholder="Enter email address"
          className="mb-4 w-full rounded border border-gray-300 p-2"
        />
        <input
          type="password"
          value={password.value}
          onChange={password.onChange}
          placeholder="Enter password"
          className="mb-4 w-full rounded border border-gray-300 p-2"
        />
        <button
          type="submit"
          className="w-[50%] self-center rounded bg-blue-500 px-4 py-2 text-white hover:bg-blue-600"
        >
          Register
        </button>
      </form>
    </div>
  );
};

export default RegisterForm;

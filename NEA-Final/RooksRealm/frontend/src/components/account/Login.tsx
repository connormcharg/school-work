import React, { FormEventHandler, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../../contexts/AuthProvider";
import useInput from "../../hooks/useInput";

const LoginForm: React.FC = () => {
  const navigate = useNavigate();
  const { isLoggedIn, login } = useAuth();

  const handleLogin: FormEventHandler<HTMLFormElement> = async (e) => {
    e.preventDefault();
    await login(email.value, password.value);
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
      <h1 className="mb-6 text-center text-xl font-semibold">Login</h1>
      <form
        className="items-left flex w-full flex-1 flex-col justify-center p-4 md:w-80"
        onSubmit={handleLogin}
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
          Login
        </button>
      </form>
    </div>
  );
};

export default LoginForm;

import React, { FormEventHandler, useEffect, useState } from "react";
import { useNavigate } from "react-router-dom"
import { useAuth } from "../../contexts/AuthProvider";

const Login: React.FC = () => {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const navigate = useNavigate();
  const { isLoggedIn, login } = useAuth();

  const handleLogin: FormEventHandler<HTMLFormElement> = async (e) => {
    e.preventDefault();
    await login(email, password);
  }

  useEffect(() => {
    if (isLoggedIn) {
      navigate("/");
    }
  }, [isLoggedIn, navigate])

  return (
    <div>
      <h1 className="text-xl font-semibold mb-6">Login</h1>
      <form
        className="flex-1 flex flex-col items-left justify-center p-4 w-full md:w-80"
        onSubmit={handleLogin}
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
          Login
        </button>
      </form>
    </div>
  );
}

export default Login;
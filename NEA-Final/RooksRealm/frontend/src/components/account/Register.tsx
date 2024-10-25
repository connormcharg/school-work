import React, { FormEventHandler, useEffect, useState } from "react";
import { useNavigate } from "react-router-dom"
import { useAuth } from "../../contexts/AuthProvider";

const Register: React.FC = () => {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const navigate = useNavigate();
  const { isLoggedIn, register, login } = useAuth();

  const handleRegister: FormEventHandler<HTMLFormElement> = async (e) => {
    try {
      e.preventDefault();
      await register(email, password);
      await login(email, password);
    } catch (error) {
      
    }
  }

  useEffect(() => {
    if (isLoggedIn) {
      navigate("/");
    }
  }, [isLoggedIn, navigate])

  return (
    <div>
      <h1 className="text-xl font-semibold mb-6">Register</h1>
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
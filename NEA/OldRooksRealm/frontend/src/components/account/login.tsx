import React, { FormEventHandler, useState } from "react";
import { useNavigate } from "react-router-dom";

const Login: React.FC = () => {
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [errorMessage, setErrorMessage] = useState("");
    const navigate = useNavigate();

    const handleLogin: FormEventHandler<HTMLFormElement> = async (e) => {
        e.preventDefault();
        setErrorMessage("");
        const data = {
            email: email,
            password: password
        }

        try {
            const response = await fetch("https://localhost:7204/api/auth/login", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify(data)
            });

            if (!response.ok) {
                const errorData = await response.json();
                throw new Error(errorData.message || "An error occurred");
            }

            const { token } = await response.json();
            localStorage.setItem("token", token);

            navigate("/");
        } catch (error: any) {
            setErrorMessage(error.message);
        }
    }
    
    return (
        <div>
            <h1 className="text-xl font-semibold mb-6">Login</h1>
            {errorMessage && <p className="text-red-500 mb-4">{errorMessage}</p>}
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
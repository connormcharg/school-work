import React, { useState, useEffect } from "react";
import { useAuth } from "../../contexts/AuthProvider";

interface UserDetails {
  username: string,
  email: string
}

const Account: React.FC = () => {
  const { token, isLoggedIn } = useAuth();
  const [userDetails, setUserDetails] = useState<UserDetails | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    const fetchUserDetails = async () => {
      try {
        const res = await fetch("/proxy/api/auth/details", {
          method: "GET",
          headers: {
            "Content-Type": "application/json",
            "Authorization": `Bearer ${token}`
          }
        });
        if (!res.ok) {
          throw new Error("Failed to get user details");
        }
        setUserDetails(await res.json());
      } catch {
        setError("Failed to load user details");
      } finally {
        setLoading(false);
      }
    };

    if (isLoggedIn) {
      fetchUserDetails();
    } else {
      setError("No Authorization token found");
      setLoading(false);
    }
  }, [token]);

  return (
    <div>
      <h1 className="text-xl font-semibold mb-6">Account</h1>
      {userDetails ? (
        <div>
          <p>Username: {userDetails.username}</p>
          <p>Email: {userDetails.email}</p>
        </div>
      ) : (
          <div>
            <p>No user details available.</p>
        </div>
      )}
    </div>
  )
}

export default Account;
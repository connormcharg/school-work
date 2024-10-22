import React from "react";
import { isAuthenticated } from "../../utils/auth";

interface AuthorizeProps {
    authorized: React.ReactNode;
    unauthorized: React.ReactNode;
}

const Authorize: React.FC<AuthorizeProps> = ({ authorized, unauthorized }) => {
    const isLoggedIn = isAuthenticated();

    return (
        <>
            {isLoggedIn ? authorized : unauthorized}
        </>
    )
}

export default Authorize;
import React, { useState, useEffect } from "react";
import { PlayIcon, CodeBracketIcon, Bars3Icon, UserIcon, UserPlusIcon, UserCircleIcon, ArrowRightStartOnRectangleIcon } from "@heroicons/react/24/solid";
import SidebarItem from "./SidebarItem";
import { Link } from "react-router-dom";
import Authorize from "./account/authorize";
import { useAuth } from "../context/AuthenticationState";

const Sidebar: React.FC = () => {
    const [isExpanded, setIsExpanded] = useState(true);
    const [showFullTitle, setShowFullTitle] = useState(true);
    const { isAuthenticated } = useAuth();

    const toggleSidebar = () => {
        setIsExpanded(!isExpanded);
    };

    useEffect(() => {
        if (!isExpanded) {
            setShowFullTitle(false);
        } else {
            const timeoutId = setTimeout(() => {
                setShowFullTitle(true);
            }, 300);

            return () => clearTimeout(timeoutId);
        }
    }, [isExpanded]);

    return (
        <div className={`relative flex flex-col bg-gray-800 transition-width duration-300 h-screen ${isExpanded ? "w-48" : "w-16"}`}>
            <button
                onClick={toggleSidebar}
                className={`text-gray-300 hover:text-rose-400 transition-all duration-300 self-start mt-4 ml-5 mb-4`}
            >
                <Bars3Icon className="w-6 h-6" />
            </button>
            
            <Link
                to="/"
                className={`text-2xl font-bold text-rose-400 mt-4 mb-4 self-center transition-opacity duration-300`}
            >
                {showFullTitle ? "Rook's Realm" : "RR"}
            </Link>

            <ul className="flex flex-col items-center mt-4 space-y-4">
                <SidebarItem icon={PlayIcon} text="Play" to="/" isExpanded={isExpanded} />
                {isAuthenticated ?
                    <>
                        <SidebarItem icon={UserCircleIcon} text="Account" to="/account" isExpanded={isExpanded} />
                        <SidebarItem icon={ArrowRightStartOnRectangleIcon} text="Logout" to="/logout" isExpanded={isExpanded} />
                    </> :
                    <>
                        <SidebarItem icon={UserIcon} text="Login" to="/login" isExpanded={isExpanded} />
                        <SidebarItem icon={UserPlusIcon} text="Register" to="/register" isExpanded={isExpanded} />
                    </>
                }
            </ul>
        </div>
    );
};

export default Sidebar;

import React, { useState, useEffect } from "react";
import { Link, useNavigate } from "react-router-dom";
import { useAuth } from "../contexts/AuthProvider";
import { ArrowRightStartOnRectangleIcon, Bars3Icon, PlayIcon, UserCircleIcon, UserIcon, UserPlusIcon } from "@heroicons/react/24/solid";

interface SidebarItemProps {
  icon: React.ElementType;
  text: string;
  to: string;
  isExpanded: boolean
}

const SidebarItem: React.FC<SidebarItemProps> = ({ icon: Icon, text, to, isExpanded }) => {
  return (
    <li
      className="flex items-center justify-center"
    >
      <Link
        to={to}
        className="flex items-center space-x-2 text-gray-300 hover:text-rose-400 transition-all duration-300"
      >
        <Icon className="w-6 h-6" />
        {isExpanded && <span className="font-semibold text-xl">{text}</span>}
      </Link>
    </li>
  )
}

interface SidebarButtonProps {
  icon: React.ElementType;
  text: string;
  isExpanded: boolean
}

const SidebarButton: React.FC<SidebarButtonProps> = ({ icon: Icon, text, isExpanded }) => {
  const { logout } = useAuth();
  const navigate = useNavigate();
  
  const handleClick = () => {
    logout();
    navigate("/");
  }
  
  return (
    <li
      className="flex items-center justify-center"
    >
      <button
        onClick={handleClick}
        className="flex items-center space-x-2 text-gray-300 hover:text-rose-400 transition-all duration-300"
      >
        <Icon className="w-6 h-6" />
        {isExpanded && <span className="font-semibold text-xl">{text}</span>}
      </button>
    </li>
  )
}

const Sidebar: React.FC = () => {
  const [isExpanded, setIsExpanded] = useState(true);
  const [showFullTitle, setShowFullTitle] = useState(true);
  const { isLoggedIn } = useAuth();

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
        className="text-gray-300 hover:text-rose-400 transition-all duration-300 self-start mt-4 ml-5 mb-4"
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
        {isLoggedIn ?
          <>
            <SidebarItem icon={UserCircleIcon} text="Account" to="/account" isExpanded={isExpanded} />
            <SidebarButton icon={ArrowRightStartOnRectangleIcon} text="Logout" isExpanded={isExpanded} />
          </> :
          <>
            <SidebarItem icon={UserIcon} text="Login" to="/login" isExpanded={isExpanded} />
            <SidebarItem icon={UserPlusIcon} text="Register" to="/register" isExpanded={isExpanded} />
          </>
        }
      </ul>
    </div>
  )
}

export default Sidebar;
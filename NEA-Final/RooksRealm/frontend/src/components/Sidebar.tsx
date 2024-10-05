import React from "react";
import { HomeIcon, PlayIcon, CodeBracketIcon } from "@heroicons/react/24/solid";
import "./Sidebar.css";
import SidebarItem from "./SidebarItem";

const Sidebar: React.FC = () => {
    return (
        <div className="flex flex-col items-center h-screen bg-gray-800 p-4 w-48">
            <h2 className="text-2xl font-bold text-rose-400 mb-8">Rook's Realm</h2>
            <ul>
                <SidebarItem icon={PlayIcon} text="Play" to="/" /> 
                <SidebarItem icon={CodeBracketIcon} text="Test" to="/test" /> 
            </ul>
        </div>
    )
}

export default Sidebar;
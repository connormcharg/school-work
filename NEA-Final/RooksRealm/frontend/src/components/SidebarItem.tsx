import React from "react";
import { Link } from "react-router-dom";

interface SidebarItemProps {
    icon: React.ElementType;
    text: string;
    to: string;
    isExpanded: boolean;
}

const SidebarItem: React.FC<SidebarItemProps> = ({ icon: Icon, text, to, isExpanded }) => {
    return (
        <li className="flex items-center justify-center">
            <Link
                to={to}
                className="flex items-center space-x-2 text-gray-300 hover:text-rose-400 transition-all duration-300"
            >
                <Icon className="w-6 h-6" />
                {isExpanded && <span className="font-semibold text-xl">{text}</span>}
            </Link>
        </li>
    );
};

export default SidebarItem;

import React from "react";
import { Link } from "react-router-dom";

interface SidebarItemProps {
    icon: React.ElementType;
    text: string;
    to: string;
}

const SidebarItem: React.FC<SidebarItemProps> = ({ icon: Icon, text, to}) => {
    return (
        <li className="w-full text-center">
            <div className="inline-flex items-center justify-center space-x-2 text-gray-300 hover:text-rose-400">
                <Icon className="size-6 transition-colors" />
                <Link className="transition-colors font-semibold text-xl" to={to}>{text}</Link>
            </div>
        </li>
    )
}

export default SidebarItem;
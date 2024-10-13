import React from "react";

interface MessageBoxProps {
    boxSize: number;
}

const MessageBox: React.FC<MessageBoxProps> = ({ boxSize }) => {
    return (
        <div className="flex flex-col bg-gray-800 text-white p-6 ml-2 mr-2 rounded-lg h-full" style={{ "width": `${boxSize}%`} as React.CSSProperties}>

        </div>
    );
}

export default MessageBox;
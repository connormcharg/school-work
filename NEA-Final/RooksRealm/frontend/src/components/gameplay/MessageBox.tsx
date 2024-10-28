import React from "react";

interface MessageBoxProps {
  boxSize: number;
}

const MessageBox: React.FC<MessageBoxProps> = ({ boxSize }) => {
  return (
    <div
      className="ml-2 mr-2 flex h-full flex-col rounded-lg bg-gray-800 p-6 text-white"
      style={{ width: `${boxSize}%` } as React.CSSProperties}
    ></div>
  );
};

export default MessageBox;

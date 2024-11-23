import React, { useEffect, useState, useRef } from "react";
import { FlagIcon, PauseIcon, ScaleIcon, PlayIcon } from "@heroicons/react/24/solid";

interface Player {
  nickname: string;
  isWhite: boolean;
  timeLeft: number;
  rating: number;
}

interface ControlStackProps {
  boxSize: number;
  suggestedMoveButton: () => boolean;
  displaySuggestedMove: () => void;
  playerData: Array<Player> | null;
  moveLogData: Array<string> | null;
  maxHeight: string;
  sendResignation: () => void;
  sendPauseRequest: () => void;
  sendDrawOffer: () => void;
  isPaused: () => boolean;
  findPauseRequestCount: () => number;
  findDrawOfferCount: () => number;
}

const ControlStack: React.FC<ControlStackProps> = ({
  boxSize,
  suggestedMoveButton,
  displaySuggestedMove,
  playerData,
  moveLogData,
  maxHeight,
  sendResignation,
  sendPauseRequest,
  sendDrawOffer,
  isPaused,
  findPauseRequestCount,
  findDrawOfferCount
}) => {
  const moveLogRef = useRef<HTMLDivElement | null>(null);
  const [isUserScrolled, setIsUserScrolled] = useState(false);

  // Detect if the user manually scrolls
  const handleScroll = () => {
    if (moveLogRef.current) {
      const { scrollTop, scrollHeight, clientHeight } = moveLogRef.current;
      setIsUserScrolled(scrollTop + clientHeight < scrollHeight - 10);
    }
  };

  // Scroll to the bottom of the move log if the user hasn't scrolled away
  useEffect(() => {
    if (moveLogRef.current && !isUserScrolled) {
      moveLogRef.current.scrollTop = moveLogRef.current.scrollHeight;
    }
  }, [moveLogData, isUserScrolled]);

  const handleSuggestedMoveClick = () => {
    displaySuggestedMove();
  };

  const formatTime = (totalSeconds: number) => {
    const minutes = Math.floor(totalSeconds / 60);
    const remainingSeconds = totalSeconds % 60;
    return `${String(minutes).padStart(2, "0")}:${String(remainingSeconds).padStart(2, "0")}`;
  };

  return (
    <div
      className="ml-2 mr-2 flex flex-col rounded-lg bg-gray-800 p-4 text-white h-full"
      style={{ width: `${boxSize}%` }}
    >
      {/* Player 1 Timer and Info */}
      <div className="font-semibold flex items-center justify-between bg-gray-600 rounded-lg p-4 mb-2">
        {playerData && playerData[0] && playerData[0].timeLeft !== -1 && (
          <div className="bg-gray-700 rounded-lg p-2 text-lg">
            {formatTime(playerData[0].timeLeft)}
          </div>
        )}
        <div className="bg-gray-700 rounded-lg p-2 text-lg">
          {playerData && playerData[0] && (
            <>
              {playerData[0].rating !== -1 && `[${playerData[0].rating}] `}
              {playerData[0].nickname}
            </>
          )}
        </div>
      </div>

      {/* Move Log */}
      <div
        className="flex flex-col bg-gray-600 rounded-lg p-4 mb-2 overflow-hidden justify-between"
        style={{ maxHeight }}
      >
        <div className="items-center w-fit font-semibold bg-gray-700 rounded-lg p-2 mb-2 text-lg">
          Move Log
        </div>
        {moveLogData && moveLogData.length > 0 && <div
          className="items-center w-fit bg-gray-700 rounded-lg p-2 text-lg overflow-y-auto"
          ref={moveLogRef}
          onScroll={handleScroll}
          style={{ maxHeight: "80%" }}
        >
          {moveLogData?.map((_, index) => {
            if (index % 2 === 0) {
              const moveNumber = Math.floor(index / 2) + 1;
              const whiteMove = moveLogData[index];
              const blackMove = moveLogData[index + 1] || "";

              return (
                <div key={moveNumber} className="flex justify-start items-center mb-1">
                  <span className="mr-3 text-gray-300">{moveNumber}.</span>
                  <span className="mr-6 text-white font-normal" style={{ width: "8ch" }}>
                    {whiteMove}
                  </span>
                  <span className="text-white font-normal" style={{ width: "8ch" }}>
                    {blackMove}
                  </span>
                </div>
              );
            }
            return null;
          })}
        </div>}
      </div>

      {/* Pause, Resign & Draw offer buttons */}
      <div className="flex items-center justify-center bg-gray-600 rounded-lg p-4 mb-2">        
        {playerData && playerData[0].timeLeft !== -1 &&
          <div className="relative">
            <button
              className="self-center bg-gray-700 rounded-lg p-2 mx-2 text-gray-300 hover:text-rose-400"
              onClick={sendPauseRequest}
            >
              {isPaused() && <PlayIcon className="h-6 w-6" />}
              {!isPaused() && <PauseIcon className="w-6 h-6" />}
            </button>
            {findPauseRequestCount() > 0 && (
              <span className="absolute top-0 right-0 bg-rose-600 text-white text-xs rounded-full h-4 w-4 flex items-center justify-center">
                {findPauseRequestCount()}
              </span>
            )}
          </div>
        }
        <button
          className="self-center bg-gray-700 rounded-lg p-2 mx-2 text-gray-300 hover:text-rose-400"
          onClick={sendResignation}
        >
          <FlagIcon className="w-6 h-6" />
        </button>
        {!suggestedMoveButton() &&
          <div className="relative">
            <button
              className="self-center bg-gray-700 rounded-lg p-2 mx-2 text-gray-300 hover:text-rose-400"
              onClick={sendDrawOffer}
            >
              <ScaleIcon className="w-6 h-6" />
            </button>
            {findDrawOfferCount() > 0 && (
              <span className="absolute top-0 right-0 bg-rose-600 text-white text-xs rounded-full h-4 w-4 flex items-center justify-center">
                {findDrawOfferCount()}
              </span>
            )}
          </div>
        }
      </div>

      {/* Player 2 Timer and Info */}
      <div className="font-semibold flex items-center justify-between bg-gray-600 rounded-lg p-4 mb-2">
        {playerData && playerData[1] && playerData[1].timeLeft !== -1 && (
          <div className="bg-gray-700 rounded-lg p-2 text-lg">
            {formatTime(playerData[1].timeLeft)}
          </div>
        )}
        <div className="bg-gray-700 rounded-lg p-2 text-lg">
          {playerData && playerData[1] && (
            <>
              {playerData[1].rating !== -1 && `[${playerData[1].rating}] `}
              {playerData[1].nickname}
            </>
          )}
        </div>
      </div>

      {suggestedMoveButton() && (
        <button
          className="bg-indigo-500 hover:bg-indigo-600 rounded-lg p-4 text-white font-bold transition-colors"
          onClick={handleSuggestedMoveClick}
        >
          Suggested Move!
        </button>
      )}
    </div>
  );
};

export default ControlStack;

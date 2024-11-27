import React, { useEffect, useState, useRef } from "react";
import {
  FlagIcon,
  PauseIcon,
  ScaleIcon,
  PlayIcon,
} from "@heroicons/react/24/solid";

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
  findDrawOfferCount,
}) => {
  const moveLogRef = useRef<HTMLDivElement | null>(null);
  const [isUserScrolled, setIsUserScrolled] = useState(false);

  const handleScroll = () => {
    if (moveLogRef.current) {
      const { scrollTop, scrollHeight, clientHeight } = moveLogRef.current;
      setIsUserScrolled(scrollTop + clientHeight < scrollHeight - 10);
    }
  };

  useEffect(() => {
    if (moveLogRef.current && !isUserScrolled) {
      moveLogRef.current.scrollTop = moveLogRef.current.scrollHeight;
    }
  }, [moveLogData, isUserScrolled]);

  const handleSuggestedMoveClick = () => {
    displaySuggestedMove();
  };

  const formatTime = (totalSeconds: number): string => {
    const minutes = Math.floor(totalSeconds / 60);
    const remainingSeconds = totalSeconds % 60;
    return `${String(minutes).padStart(2, "0")}:${String(remainingSeconds).padStart(2, "0")}`;
  };

  const renderPlayerInfo = (playerIndex: number): JSX.Element => {
    if (!playerData || playerData.length < playerIndex + 1) return <></>;

    const { nickname, rating, timeLeft } = playerData[playerIndex];
    const displayTimer = timeLeft !== -1;

    return (
      <div
        key={playerIndex}
        className="mb-2 flex items-center justify-between rounded-lg bg-gray-600 p-4 font-semibold"
      >
        {displayTimer && (
          <div className="rounded-lg bg-gray-700 p-2 text-lg">
            {formatTime(timeLeft)}
          </div>
        )}
        <div className="rounded-lg bg-gray-700 p-2 text-lg">
          {rating !== -1 && `[${rating}] `}
          {nickname}
        </div>
      </div>
    );
  };

  const renderMoveLog = (): JSX.Element => {
    if (!moveLogData || moveLogData.length === 0) return <></>;

    return (
      <div
        className="mb-2 flex flex-col justify-between overflow-hidden rounded-lg bg-gray-600 p-4"
        style={{ maxHeight }}
      >
        <div className="mb-2 w-fit items-center rounded-lg bg-gray-700 p-2 text-lg font-semibold">
          Move Log
        </div>
        <div
          ref={moveLogRef}
          onScroll={handleScroll}
          className="w-fit items-center overflow-y-auto rounded-lg bg-gray-700 p-2 text-lg"
          style={{ maxHeight: "80%" }}
        >
          {moveLogData.map((_, index) => {
            if (index % 2 === 0) {
              const moveNumber = Math.floor(index / 2) + 1;
              const whiteMove = moveLogData[index];
              const blackMove = moveLogData[index + 1] || "";

              return (
                <div
                  key={moveNumber}
                  className="mb-1 flex items-center justify-start"
                >
                  <span className="mr-3 text-gray-300">{moveNumber}.</span>
                  <span
                    className="mr-6 font-normal text-white"
                    style={{ width: "8ch" }}
                  >
                    {whiteMove}
                  </span>
                  <span
                    className="font-normal text-white"
                    style={{ width: "8ch" }}
                  >
                    {blackMove}
                  </span>
                </div>
              );
            }
            return null;
          })}
        </div>
      </div>
    );
  };

  const renderPauseButton = (): JSX.Element => {
    if (!playerData || playerData[0].timeLeft === -1) return <></>;

    const pauseCount = findPauseRequestCount();
    return (
      <button
        className="mx-2 self-center rounded-lg bg-gray-700 p-2 text-gray-300 hover:text-rose-400"
        onClick={sendPauseRequest}
      >
        {isPaused() && <PlayIcon className="h-6 w-6" />}
        {!isPaused() && <PauseIcon className="h-6 w-6" />}
        {pauseCount > 0 && (
          <span className="absolute right-0 top-0 flex h-4 w-4 items-center justify-center rounded-full bg-rose-600 text-xs text-white">
            {pauseCount}
          </span>
        )}
      </button>
    );
  };

  const renderDrawOfferButton = (): JSX.Element => {
    if (!playerData || playerData[0].timeLeft === -1) return <></>;

    const drawCount = findDrawOfferCount();
    return (
      <button
        className="mx-2 self-center rounded-lg bg-gray-700 p-2 text-gray-300 hover:text-rose-400"
        onClick={sendDrawOffer}
      >
        {drawCount > 0 && (
          <span className="absolute right-0 top-0 flex h-4 w-4 items-center justify-center rounded-full bg-rose-600 text-xs text-white">
            {drawCount}
          </span>
        )}
        <ScaleIcon className="h-6 w-6" />
      </button>
    );
  };

  const renderResignationButton = (): JSX.Element => {
    return (
      <button
        className="mx-2 self-center rounded-lg bg-gray-700 p-2 text-gray-300 hover:text-rose-400"
        onClick={sendResignation}
      >
        <FlagIcon className="h-6 w-6" />
      </button>
    );
  };

  return (
    <div
      className="ml-2 mr-2 flex h-full flex-col rounded-lg bg-gray-800 p-4 text-white max-w-lg"
      style={{ width: `${boxSize}%` }}
    >
      {renderPlayerInfo(0)}
      {renderMoveLog()}
      <div className="mb-2 flex items-center justify-center rounded-lg bg-gray-600 p-4">
        <div className="relative">{renderPauseButton()}</div>
        {renderResignationButton()}
        {!suggestedMoveButton() && (
          <div className="relative">{renderDrawOfferButton()}</div>
        )}
      </div>
      {renderPlayerInfo(1)}
      {suggestedMoveButton() && (
        <button
          className="rounded-lg bg-indigo-500 p-4 font-bold text-white transition-colors hover:bg-indigo-600"
          onClick={handleSuggestedMoveClick}
        >
          Suggested Move!
        </button>
      )}
    </div>
  );
};

export default ControlStack;

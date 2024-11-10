import React, {
  useEffect,
  useState,
  useRef
} from "react";

interface Player {
  nickname: string;
  isWhite: boolean;
  timeLeft: number;
}

interface ControlStackProps {
  boxSize: number;
  suggestedMoveButton: () => boolean,
  displaySuggestedMove: () => void;
  playerData: Array<Player> | null;
  moveLogData: Array<string> | null;
  maxHeight: string;
}

const ControlStack: React.FC<ControlStackProps> = ({
  boxSize,
  suggestedMoveButton,
  displaySuggestedMove,
  playerData,
  moveLogData,
  maxHeight
}) => {  
  const moveLogRef = useRef<HTMLDivElement | null>(null);
  const [isUserScrolled, setIsUserScrolled] = useState(false);

  // Detect if the user manually scrolls
  const handleScroll = () => {
    if (moveLogRef.current) {
      const { scrollTop, scrollHeight, clientHeight } = moveLogRef.current;
      // Check if the user has scrolled away from the bottom
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
  }

  const formatTime = (totalSeconds: number) => {
    const minutes = Math.floor(totalSeconds / 60);
    const remainingSeconds = totalSeconds % 60;
    const formattedMinutes = String(minutes).padStart(2, "0");
    const formattedSeconds = String(remainingSeconds).padStart(2, "0");
    return `${formattedMinutes}:${formattedSeconds}`;
  }
  
  return (
    <div
      className="ml-2 mr-2 flex h-full flex-col rounded-lg bg-gray-800 p-6 text-white"
      style={{ width: `${boxSize}%` } as React.CSSProperties}
    >
      {/* Player 1 Timer and Info */}
      <div className="flex-1 w-full mb-2 p-6 text-center bg-gray-600 font-bold rounded-lg transition-colors flex justify-between items-center">
        {playerData && playerData[0] &&(playerData[0].timeLeft !== -1) &&
          <>
            <div className="bg-gray-700 p-2 rounded-lg text-lg">
              {playerData && formatTime(playerData[0].timeLeft)}
            </div>
          </>
        }
        <div className="bg-gray-700 p-2 rounded-lg text-lg">
          {playerData && playerData[0] && playerData[0].nickname}
        </div>
      </div>

      {/* Move Log */}
      <div
        className="flex-1 w-full mb-2 p-6 bg-gray-600 font-bold rounded-lg overflow-y-auto"
        style={{ maxHeight }}
        ref={moveLogRef}
        onScroll={handleScroll}
      >
        {moveLogData &&
          moveLogData.map((move, index) => {
            if (index % 2 === 0) {
              const moveNumber = Math.floor(index / 2) + 1;
              const whiteMove = moveLogData[index];
              const blackMove = moveLogData[index + 1] || ""; // Leave blank if there's no black move

              const formatMove = (moveString: string) => {
                return moveString.padEnd(10, " ");
              };

              return (
                <div key={moveNumber} className="flex justify-start items-center mb-1">
                  {/* Move Number */}
                  <span className="mr-3 text-gray-300">{moveNumber}.</span>

                  {/* White Move */}
                  <span className="mr-6 text-white font-normal" style={{ width: "10ch" }}>
                    {formatMove(whiteMove)}
                  </span>

                  {/* Black Move */}
                  <span className="text-white font-normal" style={{ width: "10ch" }}>
                    {formatMove(blackMove)}
                  </span>
                </div>
              );
            }
            return null;
          })}
      </div>

      {/* Player 2 Timer and Info */}
      <div className="flex-1 w-full mb-2 p-6 text-center bg-gray-600 font-bold rounded-lg transition-colors flex justify-between items-center">
        {playerData && playerData[1] && (playerData[1].timeLeft !== -1) &&
          <>
            <div className="bg-gray-700 p-2 rounded-lg text-lg">
              {playerData && formatTime(playerData[1].timeLeft)}
            </div>
          </>
        }
        <div className="bg-gray-700 p-2 rounded-lg text-lg">
          {playerData && playerData[1] && playerData[1].nickname}
        </div>
      </div>

      {suggestedMoveButton() && <button
        className="flex-1 w-full p-10 text-center bg-indigo-500 hover:bg-indigo-600 text-white font-bold rounded-lg transition-colors"
        onClick={handleSuggestedMoveClick}
      >
        Suggested Move!
      </button>}
    </div>
  );
};

export default ControlStack;

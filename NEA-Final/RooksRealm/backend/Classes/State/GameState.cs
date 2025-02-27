﻿namespace backend.Classes.State
{
    using Newtonsoft.Json;

    public class GameState
    {
        public List<List<string>> board { get; set; }
        public bool whiteToMove { get; set; }
        public List<Move> moveLog { get; set; }
        public List<int> whiteKingLocation { get; set; }
        public List<int> blackKingLocation { get; set; }
        public bool inCheck { get; set; }
        public List<List<int>> pins { get; set; }
        public List<List<int>> checks { get; set; }
        public bool checkMate { get; set; }
        public bool staleMate { get; set; }
        public List<int>? enPassantPossible { get; set; }
        public CastleRights currentCastlingRight { get; set; }
        public List<CastleRights> castleRightsLog { get; set; }
        public int whiteTime { get; set; }
        public int blackTime { get; set; }
        public bool whiteTimeRunning { get; set; }
        public bool blackTimeRunning { get; set; }
        public bool gameOver { get; set; }
        public int fiftyMoveCounter { get; set; }
        public int threeFoldCounter { get; set; }
        public bool drawAgreed { get; set; }
        public bool playerResigned { get; set; }
        public bool isWhiteResignation { get; set; }
        public List<string> drawOffers { get; set; }
        public List<string> pauseRequests { get; set; }
        public bool pauseAgreed { get; set; }

        public GameState()
        {
            board = new List<List<string>>
            {
                new List<string> { "bR", "bN", "bB", "bQ", "bK", "bB", "bN", "bR" },
                new List<string> { "bP", "bP", "bP", "bP", "bP", "bP", "bP", "bP" },
                new List<string> { "--", "--", "--", "--", "--", "--", "--", "--" },
                new List<string> { "--", "--", "--", "--", "--", "--", "--", "--" },
                new List<string> { "--", "--", "--", "--", "--", "--", "--", "--" },
                new List<string> { "--", "--", "--", "--", "--", "--", "--", "--" },
                new List<string> { "wP", "wP", "wP", "wP", "wP", "wP", "wP", "wP" },
                new List<string> { "wR", "wN", "wB", "wQ", "wK", "wB", "wN", "wR" }
            };
            whiteToMove = true;
            moveLog = new List<Move>();
            whiteKingLocation = new List<int> { 7, 4 };
            blackKingLocation = new List<int> { 0, 4 };
            inCheck = false;
            pins = new List<List<int>>();
            checks = new List<List<int>>();
            checkMate = false;
            staleMate = false;
            enPassantPossible = null;
            currentCastlingRight = new CastleRights(true, true, true, true);
            castleRightsLog = new List<CastleRights>();
            whiteTime = 600;
            blackTime = 600;
            whiteTimeRunning = false;
            blackTimeRunning = false;
            gameOver = false;
            fiftyMoveCounter = 0;
            drawAgreed = false;
            playerResigned = false;
            isWhiteResignation = false;
            drawOffers = new List<string>();
            pauseRequests = new List<string>();
            pauseAgreed = false;
        }

        public GameState(GameState original)
        {
            this.board = original.board.Select(row => row.ToList()).ToList();
            this.whiteToMove = original.whiteToMove;
            this.moveLog = original.moveLog.Select(m => new Move(m)).ToList();
            this.whiteKingLocation = new List<int>(original.whiteKingLocation);
            this.blackKingLocation = new List<int>(original.blackKingLocation);
            this.inCheck = original.inCheck;
            this.pins = original.pins.Select(pin => new List<int>(pin)).ToList();
            this.checks = original.checks.Select(check => new List<int>(check)).ToList();
            this.checkMate = original.checkMate;
            this.staleMate = original.staleMate;
            this.enPassantPossible = original.enPassantPossible == null ? null : new List<int>(original.enPassantPossible);
            this.currentCastlingRight = new CastleRights(original.currentCastlingRight);
            this.castleRightsLog = original.castleRightsLog.Select(cr => new CastleRights(cr)).ToList();
            this.whiteTime = original.whiteTime;
            this.blackTime = original.blackTime;
            this.whiteTimeRunning = original.whiteTimeRunning;
            this.blackTimeRunning = original.blackTimeRunning;
            this.gameOver = original.gameOver;
            this.fiftyMoveCounter = original.fiftyMoveCounter;
            this.drawAgreed = original.drawAgreed;
            this.playerResigned = original.playerResigned;
            this.isWhiteResignation = original.isWhiteResignation;
            this.drawOffers = original.drawOffers;
            this.pauseRequests = original.pauseRequests;
            this.pauseAgreed = original.pauseAgreed;
        }

        [JsonConstructor]
        public GameState(List<List<string>> board, bool whiteToMove, List<Move> moveLog,
            List<int> whiteKingLocation, List<int> blackKingLocation, bool inCheck,
            List<List<int>> pins, List<List<int>> checks, bool checkMate, bool staleMate,
            List<int>? enPassantPossible, CastleRights currentCastlingRight,
            List<CastleRights> castleRightsLog, int whiteTime, int blackTime,
            bool whiteTimeRunning, bool blackTimeRunning, bool gameOver,
            int fiftyMoveCounter, bool drawAgreed, bool playerResigned,
            bool isWhiteResignation, List<string> drawOffers, List<string> pauseRequests,
            bool pauseAgreed)
        {
            this.board = board;
            this.whiteToMove = whiteToMove;
            this.moveLog = moveLog;
            this.whiteKingLocation = whiteKingLocation;
            this.blackKingLocation = blackKingLocation;
            this.inCheck = inCheck;
            this.pins = pins;
            this.checks = checks;
            this.checkMate = checkMate;
            this.staleMate = staleMate;
            this.enPassantPossible = enPassantPossible;
            this.currentCastlingRight = currentCastlingRight;
            this.castleRightsLog = castleRightsLog;
            this.whiteTime = whiteTime;
            this.blackTime = blackTime;
            this.blackTimeRunning = blackTimeRunning;
            this.whiteTimeRunning = whiteTimeRunning;
            this.gameOver = gameOver;
            this.fiftyMoveCounter = fiftyMoveCounter;
            this.drawAgreed = drawAgreed;
            this.playerResigned = playerResigned;
            this.isWhiteResignation = isWhiteResignation;
            this.drawOffers = drawOffers;
            this.pauseRequests = pauseRequests;
            this.pauseAgreed = pauseAgreed;
        }
    }
}
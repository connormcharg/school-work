using ChessForms.classes;
using ChessEngine;

namespace ChessForms
{
    public partial class ChessForms : Form
    {
        public ChessForms()
        {
            InitializeComponent();
        }

        List<ChessPiece>? pieces;
        GameState? gameState;

        List<int>? squareSelected;
        List<List<int>>? playerClicks;
        List<Move> validMoves;

        const int pictureSize = 56;
        const int boxSize = 64;

        private void ChessForms_Load(object sender, EventArgs e)
        {
            Setup();
        }

        private void Setup()
        {
            pieces = new List<ChessPiece>();
            pnlBoard.Controls.Clear();
            gameState = new GameState();
            squareSelected = null;
            playerClicks = null;

            this.KeyDown += ChessForms_KeyDown;

            pnlBoard.BackgroundImage = Image.FromFile("images/green-board.png");
            pnlBoard.BackgroundImageLayout = ImageLayout.Stretch;

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    var piece = new ChessPiece(i, j);
                    piece.Size = new Size(boxSize, boxSize);
                    piece.SizeMode = PictureBoxSizeMode.CenterImage;
                    piece.BackColor = Color.Transparent;
                    pieces.Add(piece);
                    pnlBoard.Controls.Add(piece);

                    piece.MouseClick += Piece_MouseClick;

                    piece.Location = new Point(j * boxSize, i * boxSize);
                }
            }

            UpdateImages();
        }

        private void ChessForms_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
            if (e.KeyCode == Keys.R)
            {
                gameState = new GameState();
                squareSelected = null;
                playerClicks = null;
                UpdateImages();
            }
            if (e.KeyCode == Keys.Z)
            {
                gameState.UndoMove();
                UpdateImages();
            }
        }

        private void Piece_MouseClick(object? sender, MouseEventArgs e)
        {
            ChessPiece piece = sender as ChessPiece;

            if (playerClicks == null)
            {
                playerClicks = new List<List<int>>();
            }

            if (squareSelected == null)
            {
                squareSelected = new List<int> { piece.row, piece.col };
                playerClicks.Add(squareSelected);
            }
            else
            {
                if (squareSelected[0] == piece.row && squareSelected[1] == piece.col)
                {
                    squareSelected = null;
                    playerClicks = null;
                }
                else
                {
                    squareSelected = new List<int> { piece.row, piece.col };
                    playerClicks.Add(squareSelected);
                }
            }

            validMoves = gameState.getValidMoves();

            if (!(playerClicks == null))
            {
                if (playerClicks.Count == 2)
                {
                    var move = new Move(playerClicks[0], playerClicks[1], gameState.board);
                    string moveString = move.GetChessNotation();
                    var moveMade = false;

                    for (int i = 0; i < validMoves.Count; i++)
                    {
                        if (move.Equals(validMoves[i]))
                        {
                            gameState.makeMove(validMoves[i]);
                            moveMade = true;
                            squareSelected = null;
                            playerClicks = null;
                            break;
                        }
                    }
                    if (!moveMade)
                    {
                        playerClicks = new List<List<int>> { squareSelected };
                    }
                }
            }
            UpdateImages();
        }

        private void UpdateImages()
        {
            foreach (var piece in pieces)
            {
                if (gameState.board[piece.row][piece.col] == "--")
                {
                    piece.Image = null;
                    continue;
                }
                var im = Image.FromFile($"images/{gameState.board[piece.row][piece.col]}.png");
                im = ResizeImage(im, new Size(pictureSize, pictureSize));
                piece.Image = im;
            }
            HighlightSquares();
            this.Invalidate();
        }

        private void HighlightSquares()
        {
            foreach (var piece in pieces)
            {
                piece.BackColor = Color.Transparent;
            }

            if (squareSelected != null)
            {
                int r = squareSelected[0];
                int c = squareSelected[1];

                if ((gameState.board[r][c][0] == 'w' && gameState.whiteToMove) ||
                    (gameState.board[r][c][0] == 'b' && !gameState.whiteToMove))
                {
                    HighlightThisSquare(r, c, Color.FromArgb(100, Color.Blue));
                    foreach (var move in validMoves)
                    {
                        if ((move.startRow == r) && (move.startCol == c))
                        {
                            HighlightThisSquare(move.endRow, move.endCol, Color.FromArgb(100, Color.Yellow));
                        }
                    }
                }
            }
            if (gameState.moveLog.Count > 0)
            {
                var move = gameState.moveLog[gameState.moveLog.Count - 1];
                HighlightThisSquare(move.startRow, move.startCol, Color.FromArgb(100, Color.Blue));
                HighlightThisSquare(move.endRow, move.endCol, Color.FromArgb(100, Color.Yellow));
            }
        }

        private void HighlightThisSquare(int r, int c, Color color)
        {
            foreach (var piece in pieces)
            {
                if (piece.row == r &&  (piece.col == c))
                {
                    piece.BackColor = color;
                }
            }
        }

        private static Image ResizeImage(Image original, Size size)
        {
            int sourceWidth = original.Width;
            int sourceHeight = original.Height;
            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = (float)size.Width / sourceWidth;
            nPercentH = (float)size.Height / sourceHeight;
            nPercent = Math.Min(nPercentH, nPercentW);

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap b = new Bitmap(destWidth, destHeight);

            Graphics g = Graphics.FromImage(b);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.DrawImage(original, 0, 0, destWidth, destHeight);
            g.Dispose();

            return (Image)b;
        }
    }
}

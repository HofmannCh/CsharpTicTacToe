using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TicTacToe
{
    public class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Program().Form);
        }

        public Form Form { get; set; }

        public const int CountItems = 3;

        private const int TotalPadding = 10;
        private const int TilePadding = 2;
        private const int TileSize = 50;

        public Tile[,] Tiles { get; set; }
        public int Turn { get; set; } = 0;

        public Program()
        {
            Form = new Form()
            {
                Visible = false,
            };
            
            Init();
        }

        public void Init()
        {
            this.Form.Controls.Clear();
            var size = CountItems * (TileSize + TilePadding) + TotalPadding * 2;
            this.Form.ClientSize = new Size(size, size);
            Tiles = new Tile[CountItems, CountItems];

            for (var y = 0; y < CountItems; y++)
                for (var x = 0; x < CountItems; x++)
                {
                    var tile = Tiles[y, x] = new Tile();
                    tile.Top = y * (TileSize + TilePadding) + TotalPadding;
                    tile.Left = x * (TileSize + TilePadding) + TotalPadding;
                    tile.Height = TileSize;
                    tile.Width = TileSize;
                    var y1 = y;
                    var x1 = x;
                    tile.Click += (sender, args) => HandleClick(y1, x1);
                    this.Form.Controls.Add(tile);
                }

            this.Turn = 0;
            this.Form.Visible = true;
        }

        private void HandleClick(int y, int x)
        {
            var isPlayerA = this.Turn % 2 == 0;

            if (!this.Tiles[y, x].Mark(isPlayerA))
            {
                MessageBox.Show(
                    "Kannst bereits markiertes Feld nicht mehr markieren. Nochmals spielen.",
                    "Info",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            var winner = GetWinner();
            if (winner != null)
            {
                string winnerName;
                switch (winner)
                {
                    case TileState.Empty:
                    case null:
                    default:
                        winnerName = "Nobody won";
                        break;
                    case TileState.PlayerA:
                        winnerName = "Player X won";
                        break;
                    case TileState.PlayerB:
                        winnerName = "Player O won";
                        break;
                }
                MessageBox.Show(
                    winnerName,
                    "Victory",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                Init();

                return;
            }

            this.Turn++;
        }

        private TileState? GetWinner()
        {
            bool won;
            TileState startPlayer;

            // Left to right
            for (var y = 0; y < CountItems; y++)
            {
                startPlayer = Tiles[y, 0].State;
                if (startPlayer == TileState.Empty) continue;
                won = true;
                for (var x = 1; x < CountItems; x++) won &= Tiles[y, x].State == startPlayer;
                if (won) return startPlayer;
            }

            // Top to bottom
            for (var x = 0; x < CountItems; x++)
            {
                startPlayer = Tiles[0, x].State;
                if (startPlayer == TileState.Empty) continue;
                won = true;
                for (var y = 1; y < CountItems; y++) won &= Tiles[y, x].State == startPlayer;
                if (won) return startPlayer;
            }

            // Diagonal topLeft to bottomRight
            startPlayer = Tiles[0, 0].State;
            if (startPlayer != TileState.Empty)
            {
                won = true;
                for (var i = 1; i < CountItems; i++) won &= Tiles[i, i].State == startPlayer;
                if (won) return startPlayer;
            }

            // Diagonal topRight to bottomLeft
            var lastIndex = CountItems - 1;
            startPlayer = Tiles[0, lastIndex].State;
            if (startPlayer != TileState.Empty)
            {
                won = true;
                for (var i = 1; i < CountItems; i++) won &= Tiles[i, lastIndex - i].State == startPlayer;
                if (won) return startPlayer;
            }

            won = true;
            for (var y = 0; y < CountItems; y++)
                for (var x = 0; x < CountItems; x++)
                    won &= Tiles[y, x].State != TileState.Empty;
            if (won) return TileState.Empty;

            return null;
        }

        public enum TileState
        {
            Empty,
            PlayerA,
            PlayerB,
        }

        public class Tile : Label
        {
            public TileState State { get; set; } = TileState.Empty;

            public Tile()
            {
                base.TextAlign = ContentAlignment.MiddleCenter;
                base.BackColor = Color.LightGray;
                base.Font = new Font(FontFamily.GenericMonospace, 20, FontStyle.Bold);
            }

            public bool Mark(bool isPlayerA)
            {
                if (State != TileState.Empty) return false;
                State = isPlayerA ? TileState.PlayerA : TileState.PlayerB;
                Text = isPlayerA ? "X" : "O";
                return true;
            }
        }
    }
}
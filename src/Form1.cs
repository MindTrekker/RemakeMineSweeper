using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RemakeMineSweeper
{
    public partial class Form1 : Form
    {
        //field
        private const int GridLength = 10;//<<<change these for bigger/smaller games
        private const int NumBombs = 15;//<<<<<

        private Cell[,] grid = new Cell[GridLength, GridLength];
        private Random rand = new();
        private bool firstClick = true;
        private Menu buttonMenu = new();

        private int wins = 0;
        private int losses = 0;
        private int totalTime = 0;

        //event handlers
        public EventHandler OnBombReveal;
        public EventHandler OnFirstClick;
        public EventHandler OnGameWon;

        public Form1()
        {
            InitializeComponent();
            InitializeGameGrid();
        }

        /// <summary>
        /// Preforms prep work for a new game.
        /// </summary>
        private void InitializeGameGrid()
        {
            ReadInfo();

            int yPadding = 32; //value used to make room for menu controls

            for (int row = 0; row < GridLength; row++)
            {
                for (int col = 0; col < GridLength; col++)
                {
                    Cell temp = new Cell(col, row);
                    grid[col, row] = temp;
                    grid[col, row].SetBounds(temp.Length * col, temp.Length * row + yPadding, temp.Length, temp.Length);
                    grid[col, row].CellClick += OnClick;
                    grid[col, row].CellClick += WinStatus;
                    this.Controls.Add(grid[col,row]);
                }
            }

            SetBombs(NumBombs);

            OnBombReveal += GameOver;
            OnGameWon += Victory;

            //sets bounds of the main window
            int heightOfGrid = grid[0, 0].Length * (GridLength + 1)+7;
            int widthOfGrid = grid[0, 0].Length * GridLength + 16;
            this.SetBounds(0, 0, widthOfGrid, heightOfGrid + 2*yPadding);

            SetButtonMenu(widthOfGrid,heightOfGrid);
        }

        /// <summary>
        /// performs setup for all menu items
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        private void SetButtonMenu(int width,int height)
        {
            buttonMenu.AddButton(ButtonType.Restart);
            buttonMenu.AddButton(ButtonType.Info);
            buttonMenu.AddButton(ButtonType.About);
            buttonMenu.AddButton(ButtonType.Stats);
            buttonMenu.AddButton(ButtonType.ClearStats);
            buttonMenu.AddButton(ButtonType.Exit);
            buttonMenu.OnStats += DisplayStats;
            buttonMenu.OnClearStats += ClearStats;
            OnFirstClick += buttonMenu.StartTimer;
            OnBombReveal += buttonMenu.StopTimer;
            buttonMenu.Location = this.Location;
            buttonMenu.ContainMenuBar(this.Size);
            buttonMenu.TimerStrip.Anchor = AnchorStyles.None;
            buttonMenu.TimerStrip.Location = new Point(0, height);
            this.Controls.Add(buttonMenu);
        }

        //event handlers
        ///
        /// 
        /// 
        /////////////////////////////////////////////////////////////////////////////////////////////////////////


        ///<summary>
        ///handles what to do during a left click
        /// </summary>
        private void OnClick(object sender, EventArgs e)
        {
            Cell cell = (Cell)sender;
            
            if (cell.BombIndicator != 9){ 
                CheckNear(cell);
            }
            else if (!firstClick) { OnBombReveal?.Invoke(this, EventArgs.Empty); }
            else if (firstClick) //subscribes handler that relocates bombs to first click if first cell is a bomb
            {
                OnFirstClick += FirstClickBomb;
            }
            if (firstClick) {
                OnFirstClick?.Invoke(cell, EventArgs.Empty);
                firstClick = !firstClick;
            }
        }

        private void GameOver(object sender, EventArgs e)
        {
            MessageBox.Show("You hit a mine.\nGame Over.");
            losses++;
            totalTime += buttonMenu.ElapsedTime;
            SaveInfo();
            Application.Restart();
            Environment.Exit(0);
        }

        private void Victory(object sender, EventArgs e)
        {
            MessageBox.Show("All mines swept!\nVictory!");
            wins++;
            totalTime += buttonMenu.ElapsedTime;
            SaveInfo();
            Application.Restart();
            Environment.Exit(0);
        }

        private void FirstClickBomb(object sender, EventArgs e)
        {
            Cell cell = (Cell)sender;
            //relocates a bomb if a bomb is the first cell clicked
            SetBombs(1);
            cell.BombIndicator = -1;
            OnBombReveal -= cell.RevealBomb;
            CheckNear(cell);
        }

        /// <summary>
        /// checks the status of the grid to see if win condition is met.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WinStatus(object sender, EventArgs e)
        {
            int revealedCells = 0;
            for (int col = 0; col < GridLength; col++)
            {
                for (int row = 0; row < GridLength; row++)
                {
                    if (!grid[col, row].MyButton.Visible)
                    {
                        revealedCells++;
                    }
                }
            }
            if (revealedCells == GridLength * GridLength - NumBombs) { 
                OnGameWon?.Invoke(this, EventArgs.Empty); 
            }
        }

        private void DisplayStats(object sender, EventArgs e)
        {
            int tempLosses = losses == 0 ? 1 : losses;
            MessageBox.Show($"Overall stats:\n\n" +
                            $"Wins: {wins}\n\n" +
                            $"Losses: {losses}\n\n" +
                            $"Win/Loss Ratio: {(float)wins/tempLosses}\n\n" +
                            $"Total Time of Completed Games: {totalTime}\n\n" +
                            $"Average Time Per Game: {totalTime/(wins+tempLosses)}");
        }

        private void ClearStats(object sender, EventArgs e)
        {
            wins = 0;
            losses = 0;
            totalTime = 0;
            SaveInfo();
        }



        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        private void ReadInfo()
        {
            if (File.Exists("Save.txt"))
            {
                try
                {
                    StreamReader reader = new StreamReader("Save.txt");
                    if (!int.TryParse(reader.ReadLine(), out wins)) wins = 0;
                    if (!int.TryParse(reader.ReadLine(), out losses)) losses = 0;
                    if (!int.TryParse(reader.ReadLine(), out totalTime)) totalTime = 0;
                    reader.Close();
                }
                catch (IOException) { }
            }
        }
        private void SaveInfo()
        {
            try
            {
                StreamWriter writer = new StreamWriter("Save.txt",false);
                writer.WriteLine($"{wins}");
                writer.WriteLine($"{losses}");
                writer.WriteLine($"{totalTime}");
                writer.Close();
            }
            catch(IOException) { }
        }

        /// <summary>
        /// Randomly sets bombs in "grid" equal to parameter
        /// </summary>
        /// <param name="maxBombs"></param>
        private void SetBombs(int maxBombs)
        {

            int currentBombs = 0;
            while (currentBombs < maxBombs && currentBombs!= GridLength*GridLength)
            {
                int col = rand.Next(0, GridLength);
                int row = rand.Next(0, GridLength);
                if (grid[col,row].BombIndicator!=9) { 
                     grid[col, row].BombIndicator=9;  //9 is used to indicate that a cell is a bomb.
                     OnBombReveal += grid[col, row].RevealBomb; //helps reveal all bombs when a bomb is clicked.
                     currentBombs++;
                }
            }
        }

       


        /// <summary>
        /// checks and counts bombs adjacent to "Cell check". Calls ClickNear() if no bombs are adjacent.
        /// </summary>
        private void CheckNear(Cell check)
        {
            if (check.BombIndicator == -1)
            {
                int bombCount = 0;
                for (int srow = -1; srow <= 1; srow++)
                {
                    for (int scol = -1; scol <= 1; scol++)
                    {
                        if(check.X+scol < 0 || check.X+scol >= GridLength || check.Y+srow < 0 || check.Y + srow >= GridLength)
                        {
                            continue;
                        }
                        if (grid[check.X + scol, check.Y + srow].BombIndicator == 9)
                        {
                            bombCount++;
                        }
                    }
                }
                check.BombIndicator = bombCount;
                check.SetNumber();
                if (bombCount == 0)
                {
                    ClickNear(check);
                }
            }
        }

        /// <summary>
        /// Invokes ButtonClick on cells surrounding "Cell center".
        /// </summary>
        /// <param name="center"></param>
        private void ClickNear(Cell center)
        {
            for (int srow = -1; srow < 2; srow++)
            {
                for (int scol = -1; scol < 2; scol++)
                {
                    if (center.X + scol < 0 || center.X + scol > GridLength - 1 || center.Y + srow < 0 || center.Y + srow > GridLength - 1)
                    {
                        continue;  //if statement ensures no selection is out of bounds
                    }
                    Cell current = grid[center.X + scol, center.Y + srow];
                    current.ButtonClick?.Invoke(current , EventArgs.Empty); 
                    //for technical reasons, ButtonClick is in no way tied to the button.Click event
                }
            }
        }
    }
}

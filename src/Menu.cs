using System;
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
    public enum ButtonType
    {
        Restart,
        Info,
        About,
        Exit,
        Stats,
        ClearStats
    }
    public partial class Menu : UserControl
    {
        public EventHandler OnStats;
        public EventHandler OnClearStats;

        private ToolStripContainer menuStripContainer = new ToolStripContainer();
        private ToolStrip menuBar= new ToolStrip();

        private StatusStrip timerStrip = new StatusStrip();
        private Timer menuTimer = new Timer();
        private int elapsedTime = 0;


        public StatusStrip TimerStrip { get => timerStrip; }
        public int ElapsedTime { get => elapsedTime; }

        public Menu()
        {
            InitializeComponent();

            this.Controls.Add(timerStrip);
            menuTimer.Interval = 1000;
            menuTimer.Tick += TimerUpdate;
            timerStrip.Items.Add($"Time: {elapsedTime}");
        }

        /// <summary>
        /// Adds an item to the Toolbar based on the provided enum ButtonType
        /// </summary>
        /// <param name="type"></param>
        public void AddButton(ButtonType type)
        {
            Button button = new();
            switch (type)
            {
                case ButtonType.Restart:
                    ToolStripItem restart = menuBar.Items.Add("Restart");
                    restart.Click += Restart;
                    break;
                case ButtonType.Info:
                    ToolStripItem info = menuBar.Items.Add("Info");
                    info.Click += Info;
                    break;
                case ButtonType.About:
                    ToolStripItem about = menuBar.Items.Add("About");
                    about.Click += About;
                    break;
                case ButtonType.Exit:
                    ToolStripItem exit = menuBar.Items.Add("Exit");
                    exit.Click += Exit;
                    break;
                case ButtonType.Stats:
                    ToolStripItem stats = menuBar.Items.Add("Stats");
                    stats.Click += Stats;
                    break;
                case ButtonType.ClearStats:
                    ToolStripItem clearStats = menuBar.Items.Add("Clear Stats");
                    clearStats.Click += ClearStats;
                    break;
            }
        }

        /// <summary>
        /// Runs some setup items. Could be rewritten and removed.
        /// </summary>
        /// <param name="size"></param>
        public void ContainMenuBar(Size size)
        {
            menuStripContainer.Size = new Size(size.Width, menuStripContainer.Size.Height);
            this.Size = size;
            menuBar.Stretch = true;
            menuStripContainer.TopToolStripPanel.Controls.Add(menuBar);
            this.Controls.Add(menuStripContainer);
        }

        private void Restart(object sender, EventArgs e)
        {
            Application.Restart();
            Environment.Exit(0);
        }

        private void Info(object sender, EventArgs e)
        {
            MessageBox.Show("Welcome to Minesweeper\n\n" +
                            "\tThe goal of this game is to locate all mines without setting them off." +
                            "To acheive this, all non-mine cells revealed will have a number indicating" +
                            "how many mines are within the 8 closest cells.\n\n" +
                            "\tUse left click on the mouse to reveal cells and use right click to flag" +
                            "potential mines. Good Luck!");
        }

        private void About(object sender, EventArgs e)
        {
            MessageBox.Show("Made by Grayson Mckenzie\n" +
                            "November, 2021\n" +
                            "as part of class CS 3020-001");
        }

        private void Exit(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void Stats(object sender, EventArgs e)
        {
            OnStats?.Invoke(sender, EventArgs.Empty);
        }

        private void ClearStats(object sender, EventArgs e)
        {
            OnClearStats?.Invoke(sender, EventArgs.Empty);
        }

        private void TimerUpdate(object sender, EventArgs e)
        {
            elapsedTime++;
            timerStrip.Items[0].Text=$"Time: {elapsedTime}";
        }

        public void StartTimer(object sender,EventArgs e)
        {
            menuTimer.Start();
        }

        public void StopTimer(object sender, EventArgs e)
        {
            menuTimer.Stop();
        }

    }
}

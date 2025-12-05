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
    public partial class Cell : UserControl
    {
        public EventHandler CellClick;
        public EventHandler ButtonClick;

        private Button myButton = new Button();
        private Panel myPanel = new Panel();
        private const int mySize = 32;
        private int x;
        private int y;
        private int bombIndicator = -1;
        private bool flagged = false;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int X { get => x; set => x = value; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int Y { get => y; set => y = value; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int BombIndicator { get => bombIndicator; set => bombIndicator = value; }
        public Button MyButton { get => myButton; }
        public int Length { get => mySize; }

        public Cell(int x, int y)
        {
            InitializeComponent();
            this.x = x;
            this.y = y;
            this.Padding = new Padding(0);
            Size = new Size(mySize, mySize);
            SetPanel();
            SetButton();
        }

        private void SetPanel()
        {
            myPanel.Location = this.Location;
            myPanel.Size = this.Size;
            myPanel.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(myPanel);
        }
        private void SetButton()
        {
            myButton.Location = this.Location;
            myButton.Size = this.Size;
            this.Controls.Add(myButton);
            myButton.BackColor = Color.DarkGray;
            myButton.FlatStyle = FlatStyle.Flat;
            myButton.MouseDown += OnMouseClick;
            ButtonClick += OnButtonClick;
            myButton.BringToFront();
        }

        public void SetNumber()
        {
            Label number = new Label();
            number.Font = new Font("Arial", 16, FontStyle.Bold);
            number.AutoSize = true;
            number.Text = BombIndicator != 0 ? $"{BombIndicator}":"";
            myPanel.Controls.Add(number);
        }

        public void RevealBomb(object sender, EventArgs e)
        {
            myButton.Visible = false;
            Label bomb = new Label();
            bomb.Font = new Font("Arial", 24, FontStyle.Bold);
            bomb.AutoSize = true;
            bomb.Text = "*";
            myPanel.Controls.Add(bomb);
        }

        private void FlagCell()
        {
            flagged = !flagged;
            myButton.BackColor = flagged ? Color.Red : Color.DarkGray;
        }

        /// <summary>
        /// Runs on events called on left clicks or program clicks
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnButtonClick(object sender, EventArgs e)
        {
            myButton.Visible = false;
            CellClick?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Runs on a mouse click event, EventArgs contain button information.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnMouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ButtonClick?.Invoke(this, EventArgs.Empty);
            }
            if(e.Button == MouseButtons.Right)
            {
                FlagCell();
            }
        }

    }
}

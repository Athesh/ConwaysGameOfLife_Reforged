using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Praus.ConwaysGameOfLife.Utils;
using Praus.ConwaysGameOfLife.Model.QTree;


namespace Praus.ConwaysGameOfLife {
    public partial class AppWindow : Form {

        private int generationCount;
        private QNode root;

        private void SetCell(int x, int y) {
            int max;
            do {
                max = Convert.ToInt32(Math.Pow(2, root.Level - 1));

                if (Math.Abs(x) > max || Math.Abs(y) > max) {
                    root.ExpandTree();
                }
            } while (Math.Abs(x) > max || Math.Abs(y) > max);
            root.SetCell(x, y);
        }

        private void NextGeneration() {
            while (root.Level < 3 ||
                   root.TopLeft.Population != root.TopLeft.BotRight.BotRight.Population ||
                   root.TopRight.Population != root.TopRight.BotLeft.BotLeft.Population ||
                   root.BotLeft.Population != root.BotLeft.TopRight.TopRight.Population ||
                   root.BotRight.Population != root.BotRight.TopLeft.TopLeft.Population) {
                root = root.ExpandTree();
            }
            root = root.NextGeneration();
            generationCount++;
        }

        public AppWindow() {
            InitializeComponent();
            root = QNode.Create();
        }

        protected override void OnPaint(PaintEventArgs e) {
            Graphics gfx = e.Graphics;
            Pen myPen = new Pen(Color.Gray);

            int squareSize = 8;
            var offset = new {
                Left = 100, 
                Right = 40, 
                Top = 20,
                Bottom = 40
            };

            gfx.DrawLine(myPen, offset.Left, offset.Top, Width - offset.Right, offset.Top);
            gfx.DrawLine(myPen, offset.Left, Height - offset.Bottom, Width - offset.Right, Height - offset.Bottom);

            gfx.DrawLine(myPen, offset.Left, offset.Top, offset.Left, Height - offset.Bottom);
            gfx.DrawLine(myPen, Width - offset.Right, offset.Top, Width - offset.Right, Height - offset.Bottom);

            var rows = (Height - offset.Top - offset.Bottom) / squareSize;
            for (var row = 0; row < rows; row++) {
                gfx.DrawLine(
                    myPen,
                    offset.Left,
                    row * squareSize + offset.Top,
                    Width - offset.Right,
                    row * squareSize + offset.Top);
                    
            }
            var cols = (Width - offset.Left - offset.Right) / squareSize;
            for (var col = 0; col < cols; col++) {
                gfx.DrawLine(
                    myPen,
                    col * squareSize + offset.Left,
                    offset.Top,
                    col * squareSize + offset.Left,
                    Height - offset.Bottom);
            }


        }

        private void start_Click(object sender, EventArgs e) {
            //timer1.Enabled = true;
            timer1.Start();
        }

        private void stop_Click(object sender, EventArgs e) {
            timer1.Stop();
        }

        private void AppWindow_Load(object sender, EventArgs e) {

        }

        private void nextGen_Click(object sender, EventArgs e) {
            NextGeneration();
        }

        private void timer1_Tick(object sender, EventArgs e) {
            MessageBox.Show("Test");
        }
    }
}

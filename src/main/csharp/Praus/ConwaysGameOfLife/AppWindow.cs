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
                    root = root.ExpandTree();
                }
            } while (Math.Abs(x) > max || Math.Abs(y) > max);
            root = root.SetCell(x, y);
        }

        private bool GetCell(int x, int y) {
            var max = Convert.ToInt32(Math.Pow(2, root.Level - 1));
            return (Math.Abs(x) > max || Math.Abs(y) > max) ? false : root.GetCell(x, y).IsAlive;
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

            SetCell(1, -1);
            SetCell(5, -1);
            SetCell(2, -1);
            SetCell(2, -3);
            SetCell(4, -2);
            SetCell(6, -1);
            SetCell(7, -1);

            SetCell(12, -12);
            SetCell(12, -13);
            SetCell(13, -12);
            SetCell(13, -13);

            SetCell(-12, 5);
            SetCell(-12, 6);
            SetCell(-12, 7);

        }

        protected override void OnPaint(PaintEventArgs e) {
            Graphics gfx = e.Graphics;
            int squareSize = 10;
            var offset = new {
                Left = 100, 
                Right = 40, 
                Top = 20,
                Bottom = 40
            };
            var dim = 32;
            for (var x = -dim; x <= dim; x++) {
                for (var y = -dim; y <= dim; y++) {
                    SolidBrush brush = new SolidBrush(Color.Black);
                    if (GetCell(x,y)) {
                        brush = new SolidBrush(Color.White);
                    } 
                    gfx.FillRectangle(
                        brush,
                        (x + dim) * squareSize + offset.Left + 1,
                        (y + dim) * squareSize + offset.Top + 1,
                        squareSize - 1,
                        squareSize - 1);
                    
                }
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
            Invalidate();
        }

        private void timer1_Tick(object sender, EventArgs e) {
            MessageBox.Show("Test");
        }
    }
}

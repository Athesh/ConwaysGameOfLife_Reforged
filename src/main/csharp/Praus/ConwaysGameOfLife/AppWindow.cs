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
        private CachedNode root;

        private void SetCell(int x, int y) {
            int max;
            do {
                max = Convert.ToInt32(Math.Pow(2, root.Level - 1));

                if (Math.Abs(x) > max || Math.Abs(y) > max) {
                    root.ExpandTree();
                }
            } while (Math.Abs(x) > max || Math.Abs(y) > max);
            root.SetLeaf(x, y);
        }

        public AppWindow() {
            InitializeComponent();
            root = CachedNode.Create();
        }

        protected override void OnPaint(PaintEventArgs e) {
            Graphics gfx = e.Graphics;
            Pen myPen = new Pen(Color.Gray);

            int squareSize = 4;
            var offset = new { LeftRight = 50, BottomTop = 100 };
            var gen = new { Rows = this.Width,
                            Columns = this.Height};
            

            for (int x = 0; x <= gen.Rows; x++) { //vykreslování mřížky svisle
                gfx.DrawLine(myPen, 0, x * squareSize, gen.Columns * squareSize, x * squareSize);
            }
            //squareSize -> vykreslování mřížky na základě velikosti čtverečku
            for (int y = 0; y <= gen.Columns; y++) { //vykreslování mřížky vodorovně
                gfx.DrawLine(myPen, y * squareSize, 0, y * squareSize, gen.Rows * squareSize);
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
            root = (CachedNode)root.NextGeneration();
        }

        private void timer1_Tick(object sender, EventArgs e) {
            MessageBox.Show("Test");
        }
    }
}

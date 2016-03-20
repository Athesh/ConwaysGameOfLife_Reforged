using System.Windows.Forms;
using System.Drawing;
using System;

namespace Praus.ConwaysGameOfLife {
    
    public class Display : Panel {
        public Func<int, int, bool> GetCell { get; set; }

        public Display() {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            UpdateStyles();
        }

        protected override void OnPaint(PaintEventArgs e) {
            Graphics gfx = e.Graphics;
            int squareSize = 9;
            var offset = new {
                Left = 0, 
                Right = 0, 
                Top = 0,
                Bottom = 0
            };
            var rows = 35;
            var cols = 46;
            for (var x = -cols; x <= cols; x++) {
                for (var y = -rows; y <= rows; y++) {
                    SolidBrush brush = new SolidBrush(Color.Black);
                    if (GetCell(x,y)) {
                        brush = new SolidBrush(Color.White);
                    } 
                    gfx.FillRectangle(
                        brush,
                        (x + cols) * squareSize + offset.Left + 1,
                        (y + rows) * squareSize + offset.Top + 1,
                        squareSize - 1,
                        squareSize - 1);

                }
            }
        }
    }
}
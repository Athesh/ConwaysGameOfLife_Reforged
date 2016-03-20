using System.Windows.Forms;
using System.Drawing;
using System;

namespace Praus.ConwaysGameOfLife {
    
    public class Display : Panel {
        public Func<int, int, bool> GetCell { get; set; } = (x, y) => false;
        public Action<int, int> SetCell { get; set; } 
        public int Rows { get; set; } = 35;
        public int Cols { get; set; } = 46;
        public int SquareSize { get; set; } = 9;

        public Display() {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            UpdateStyles();
        }

        protected override void OnMouseClick(MouseEventArgs e) {
            SetCell?.Invoke(e.X / SquareSize - Cols, e.Y / SquareSize - Rows);
            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e) {
            Graphics gfx = e.Graphics;
            var offset = new {
                Left = 0, 
                Right = 0, 
                Top = 0,
                Bottom = 0
            };
            for (var x = -Cols; x <= Cols; x++) {
                for (var y = -Rows; y <= Rows; y++) {
                    SolidBrush brush = new SolidBrush(Color.Black);
                    if (GetCell(x,y)) {
                        brush = new SolidBrush(Color.White);
                    } 
                    gfx.FillRectangle(
                        brush,
                        (x + Cols) * SquareSize + offset.Left + 1,
                        (y + Rows) * SquareSize + offset.Top + 1,
                        SquareSize - 1,
                        SquareSize - 1);

                }
            }
        }
    }
}
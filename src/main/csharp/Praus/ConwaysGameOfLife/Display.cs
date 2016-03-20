using System.Windows.Forms;
using System.Drawing;
using System;

namespace Praus.ConwaysGameOfLife {
    
    public class Display : Panel {

        /// <summary>
        /// Reference na fuknci, která zjistí jestli je buňka živá nebo mrtvá
        /// </summary>
        public Func<int, int, bool> GetCell { get; set; }
        /// <summary>
        /// Reference na fuknci, která nastaví živou buňku na souřadnicích
        /// </summary>
        public Action<int, int> SetCell { get; set; } 
        public int Rows { get; set; }   //počet řádků
        public int Cols { get; set; }   //počet sloupců
        public int SquareSize { get; set; } = 15;   //velikost čtverečku

        /// <summary>
        /// Konsktruktor prot nastavování vlastností panelu
        /// </summary>
        public Display() {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            UpdateStyles();
        }

        /// <summary>
        /// Výpočet zobrazovaných řádků a sloupců
        /// </summary>
        private void CalcRowsAndColumns() {
            Rows = (Height / SquareSize) / 2;
            Cols = (Width / SquareSize) / 2;
        }

        /// <summary>
        /// Nastaví živou buňku na souřadnicích X, Y
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseClick(MouseEventArgs e) {
            SetCell?.Invoke(e.X / SquareSize - Cols, e.Y / SquareSize - Rows);
            this.Invalidate();
        }

        /// <summary>
        /// Zoomuje
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseWheel(MouseEventArgs e) {
            var d = SquareSize + (e.Delta > 0 ? 1 : -1);
            if (d > 15) {
                SquareSize = 15;
            } else if (d < 3) { //3x3 pixely při odzoomování
                SquareSize = 3;
            } else {
                SquareSize = d;
            }
            this.Invalidate(); //překreslení
        }

        /// <summary>
        /// Chytne focus když se najede myší a může se zoomovat
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseHover(EventArgs e) {
            this.Focus();
        }

        /// <summary>
        /// Vykreslování
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e) {
            CalcRowsAndColumns();
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
                    if (GetCell?.Invoke(x, y) ?? false) {
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
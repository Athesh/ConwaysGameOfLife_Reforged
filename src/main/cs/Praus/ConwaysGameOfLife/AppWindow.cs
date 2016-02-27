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


namespace Praus.ConwaysGameOfLife {
    public partial class AppWindow : Form, ILogUtils {
        public AppWindow() {
            InitializeComponent();
            this.GetLogger().Debug("kafsjsla");
            this.GetLogger().Info("kafsjsla");
            this.GetLogger().Warn("kafsjsla");
            this.GetLogger().Error("kafsjsla");
            this.GetLogger().Fatal("kafsjsla");
        }
    }
}

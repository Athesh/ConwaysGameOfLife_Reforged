﻿using System;
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
    public partial class AppWindow : Form, ILogUtils {
        public AppWindow() {
            InitializeComponent();
        }
    }
}
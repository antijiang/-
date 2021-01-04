using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace 调试者
{
    public partial class 关于 : Form
    {
        public 关于()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void 关于_Load(object sender, EventArgs e)
        {
            label2.Text = "ShenZhen JiuZhou Greeble";
        }
    }
}

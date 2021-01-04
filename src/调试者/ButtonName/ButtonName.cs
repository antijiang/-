using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
//using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace 调试者
{
    public partial class ButtonName : Form
    {
        public int kit;
        public ButtonName()
        {
            InitializeComponent();
            try
            {
                textBox2.Text = TS调试者.IndexButton.Text;
            }
            catch
            {
                
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

            TS调试者.IndexButton.Text = textBox1.Text;
            this.Close();
                            
        }
    }
}

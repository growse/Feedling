using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Feedling
{
    public partial class PasswordForm : Form
    {
        private string givenpwd = "";

        public string Givenpwd
        {
            get { return givenpwd; }
        }

        public PasswordForm()
        {
            InitializeComponent();
        }

        private void closebtn_Click(object sender, EventArgs e)
        {
            givenpwd = pwdbox.Text;
            this.Close();
        }
    }
}

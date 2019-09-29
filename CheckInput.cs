using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.CSharp;

namespace Auto_Statistic
{
    public partial class CheckInput : Form
    {
        public CheckInput()
        {
            InitializeComponent();
            textBoxAlg.Text = Main.windowVars.checkAlgorithm;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            var res = Main.compileAlg(textBoxAlg.Text);
            if (res.Errors.Count == 0)
            {
                Close();
            }
            else
            {
                string errors = "";
                foreach (var error in res.Errors)
                {
                    errors += error + "\n";
                }
                MessageBox.Show("Не удалось скомпилировать код!\n" + errors, "Ошибка");
            }
        }

        private void buttonDefault_Click(object sender, EventArgs e)
        {
            textBoxAlg.Text = Main.defaultAlg;
        }
    }
}

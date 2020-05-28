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
            textBoxUsing.Text = Main.windowVars.checkAlgorithmUsingsText;
            textBoxAlg.Text = Main.windowVars.checkAlgorithmText;
            textBox_classes.Text = Main.windowVars.checkAlgorithmClassesText;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            try
            {
                Main.checkAlgorithm = new CheckAlg(textBoxUsing.Text, textBox_classes.Text, textBoxAlg.Text);
                Main.windowVars.checkAlgorithmText = textBoxAlg.Text;
                Main.windowVars.checkAlgorithmUsingsText = textBoxUsing.Text;
                Main.windowVars.checkAlgorithmClassesText = textBox_classes.Text;
                Close();
            }
            catch(Exception exc)
            {
                MessageBox.Show("Не удалось скомпилировать код!\n" + exc.Message, "Ошибка");
            }
        }

        private void buttonDefault_Click(object sender, EventArgs e)
        {
            textBoxAlg.Text = CheckAlg.defaultAlg;
        }
    }
}

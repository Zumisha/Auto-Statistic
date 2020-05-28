namespace Auto_Statistic
{
    partial class CheckInput
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.textBoxAlg = new System.Windows.Forms.TextBox();
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonDefault = new System.Windows.Forms.Button();
            this.textBoxUsing = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox_classes = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // textBoxAlg
            // 
            this.textBoxAlg.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxAlg.Location = new System.Drawing.Point(13, 342);
            this.textBoxAlg.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBoxAlg.Multiline = true;
            this.textBoxAlg.Name = "textBoxAlg";
            this.textBoxAlg.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxAlg.Size = new System.Drawing.Size(558, 412);
            this.textBoxAlg.TabIndex = 0;
            // 
            // buttonOk
            // 
            this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOk.Location = new System.Drawing.Point(459, 764);
            this.buttonOk.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(112, 35);
            this.buttonOk.TabIndex = 1;
            this.buttonOk.Text = "Применить";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonCancel.Location = new System.Drawing.Point(13, 764);
            this.buttonCancel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(112, 35);
            this.buttonCancel.TabIndex = 2;
            this.buttonCancel.Text = "Отмена";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 317);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(559, 20);
            this.label1.TabIndex = 3;
            this.label1.Text = "bool Check(String output, String expected, Dictionary<String, Object> storage)";
            // 
            // buttonDefault
            // 
            this.buttonDefault.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonDefault.Location = new System.Drawing.Point(133, 764);
            this.buttonDefault.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.buttonDefault.Name = "buttonDefault";
            this.buttonDefault.Size = new System.Drawing.Size(133, 35);
            this.buttonDefault.TabIndex = 4;
            this.buttonDefault.Text = "По умолчанию";
            this.buttonDefault.UseVisualStyleBackColor = true;
            this.buttonDefault.Click += new System.EventHandler(this.buttonDefault_Click);
            // 
            // textBoxUsing
            // 
            this.textBoxUsing.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxUsing.Location = new System.Drawing.Point(13, 34);
            this.textBoxUsing.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBoxUsing.Multiline = true;
            this.textBoxUsing.Name = "textBoxUsing";
            this.textBoxUsing.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxUsing.Size = new System.Drawing.Size(554, 93);
            this.textBoxUsing.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 20);
            this.label2.TabIndex = 6;
            this.label2.Text = "Includes";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 135);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 20);
            this.label3.TabIndex = 8;
            this.label3.Text = "Classes";
            // 
            // textBox_classes
            // 
            this.textBox_classes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_classes.Location = new System.Drawing.Point(13, 160);
            this.textBox_classes.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBox_classes.Multiline = true;
            this.textBox_classes.Name = "textBox_classes";
            this.textBox_classes.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox_classes.Size = new System.Drawing.Size(554, 143);
            this.textBox_classes.TabIndex = 7;
            // 
            // CheckInput
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 813);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBox_classes);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxUsing);
            this.Controls.Add(this.buttonDefault);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.textBoxAlg);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MinimumSize = new System.Drawing.Size(600, 200);
            this.Name = "CheckInput";
            this.Text = "Алгоритм проверки";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxAlg;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonDefault;
        private System.Windows.Forms.TextBox textBoxUsing;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox_classes;
    }
}
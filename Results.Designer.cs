namespace Auto_Statistic
{
    partial class Results
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
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Program = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Arguments = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.itNum = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ItCurr = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Time = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Variance = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MaxRam = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MaxCpu = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AvgCpu = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Status = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Program,
            this.Arguments,
            this.itNum,
            this.ItCurr,
            this.Time,
            this.Variance,
            this.MaxRam,
            this.MaxCpu,
            this.AvgCpu,
            this.Status});
            this.dataGridView1.Location = new System.Drawing.Point(12, 12);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.Size = new System.Drawing.Size(1176, 426);
            this.dataGridView1.TabIndex = 0;
            // 
            // Program
            // 
            this.Program.Frozen = true;
            this.Program.HeaderText = "Программа";
            this.Program.Name = "Program";
            this.Program.ReadOnly = true;
            // 
            // Arguments
            // 
            this.Arguments.Frozen = true;
            this.Arguments.HeaderText = "Аргументы";
            this.Arguments.Name = "Arguments";
            this.Arguments.ReadOnly = true;
            // 
            // itNum
            // 
            this.itNum.Frozen = true;
            this.itNum.HeaderText = "Количество Запусков";
            this.itNum.Name = "itNum";
            this.itNum.ReadOnly = true;
            // 
            // ItCurr
            // 
            this.ItCurr.Frozen = true;
            this.ItCurr.HeaderText = "Выполнено запусков";
            this.ItCurr.Name = "ItCurr";
            this.ItCurr.ReadOnly = true;
            // 
            // Time
            // 
            this.Time.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Time.Frozen = true;
            this.Time.HeaderText = "Время, с";
            this.Time.Name = "Time";
            this.Time.ReadOnly = true;
            this.Time.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Time.Width = 71;
            // 
            // Variance
            // 
            this.Variance.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Variance.Frozen = true;
            this.Variance.HeaderText = "Ср. кв. откл.";
            this.Variance.Name = "Variance";
            this.Variance.ReadOnly = true;
            this.Variance.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Variance.Width = 87;
            // 
            // MaxRam
            // 
            this.MaxRam.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.MaxRam.Frozen = true;
            this.MaxRam.HeaderText = "Макс. исп. ОЗУ, МБ";
            this.MaxRam.Name = "MaxRam";
            this.MaxRam.ReadOnly = true;
            this.MaxRam.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.MaxRam.Width = 108;
            // 
            // MaxCpu
            // 
            this.MaxCpu.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.MaxCpu.Frozen = true;
            this.MaxCpu.HeaderText = "Макс. исп. CPU, %";
            this.MaxCpu.Name = "MaxCpu";
            this.MaxCpu.ReadOnly = true;
            this.MaxCpu.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.MaxCpu.Width = 105;
            // 
            // AvgCpu
            // 
            this.AvgCpu.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.AvgCpu.Frozen = true;
            this.AvgCpu.HeaderText = "Ср. исп. CPU, %";
            this.AvgCpu.Name = "AvgCpu";
            this.AvgCpu.ReadOnly = true;
            this.AvgCpu.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.AvgCpu.Width = 92;
            // 
            // Status
            // 
            this.Status.Frozen = true;
            this.Status.HeaderText = "Статус";
            this.Status.Name = "Status";
            this.Status.ReadOnly = true;
            // 
            // Results
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 450);
            this.Controls.Add(this.dataGridView1);
            this.Name = "Results";
            this.Text = "Results";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Program;
        private System.Windows.Forms.DataGridViewTextBoxColumn Arguments;
        private System.Windows.Forms.DataGridViewTextBoxColumn itNum;
        private System.Windows.Forms.DataGridViewTextBoxColumn ItCurr;
        private System.Windows.Forms.DataGridViewTextBoxColumn Time;
        private System.Windows.Forms.DataGridViewTextBoxColumn Variance;
        private System.Windows.Forms.DataGridViewTextBoxColumn MaxRam;
        private System.Windows.Forms.DataGridViewTextBoxColumn MaxCpu;
        private System.Windows.Forms.DataGridViewTextBoxColumn AvgCpu;
        private System.Windows.Forms.DataGridViewTextBoxColumn Status;
    }
}
namespace FPTL_Auto_Statistic
{
    partial class Main
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.файлToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.выходToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.справкаToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.оПрограммеToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.buttonChooseExecutor = new System.Windows.Forms.Button();
            this.textBoxExecutorPath = new System.Windows.Forms.TextBox();
            this.buttonCooseProgramFiles = new System.Windows.Forms.Button();
            this.textBoxProgramFiles = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxNumberOfLaunches = new System.Windows.Forms.TextBox();
            this.textBoxMaxBackCPUusage = new System.Windows.Forms.TextBox();
            this.buttonBegin = new System.Windows.Forms.Button();
            this.buttonPause = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.dataGridViewLaunchParametrs = new System.Windows.Forms.DataGridView();
            this.LaunchParametrs = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripStatusLabelCPUmem = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelMem = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelTask = new System.Windows.Forms.ToolStripStatusLabel();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.buttonAddParametrs = new System.Windows.Forms.Button();
            this.buttonContinue = new System.Windows.Forms.Button();
            this.checkBoxMemControl = new System.Windows.Forms.CheckBox();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewLaunchParametrs)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.файлToolStripMenuItem,
            this.справкаToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(9, 3, 0, 3);
            this.menuStrip1.Size = new System.Drawing.Size(824, 25);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // файлToolStripMenuItem
            // 
            this.файлToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.выходToolStripMenuItem});
            this.файлToolStripMenuItem.Name = "файлToolStripMenuItem";
            this.файлToolStripMenuItem.Size = new System.Drawing.Size(48, 19);
            this.файлToolStripMenuItem.Text = "Файл";
            // 
            // выходToolStripMenuItem
            // 
            this.выходToolStripMenuItem.Name = "выходToolStripMenuItem";
            this.выходToolStripMenuItem.Size = new System.Drawing.Size(108, 22);
            this.выходToolStripMenuItem.Text = "Выход";
            this.выходToolStripMenuItem.Click += new System.EventHandler(this.выходToolStripMenuItem_Click);
            // 
            // справкаToolStripMenuItem
            // 
            this.справкаToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.оПрограммеToolStripMenuItem});
            this.справкаToolStripMenuItem.Name = "справкаToolStripMenuItem";
            this.справкаToolStripMenuItem.Size = new System.Drawing.Size(65, 19);
            this.справкаToolStripMenuItem.Text = "Справка";
            // 
            // оПрограммеToolStripMenuItem
            // 
            this.оПрограммеToolStripMenuItem.Name = "оПрограммеToolStripMenuItem";
            this.оПрограммеToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
            this.оПрограммеToolStripMenuItem.Text = "О программе";
            this.оПрограммеToolStripMenuItem.Click += new System.EventHandler(this.оПрограммеToolStripMenuItem_Click);
            // 
            // buttonChooseExecutor
            // 
            this.buttonChooseExecutor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonChooseExecutor.Location = new System.Drawing.Point(18, 35);
            this.buttonChooseExecutor.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.buttonChooseExecutor.Name = "buttonChooseExecutor";
            this.buttonChooseExecutor.Size = new System.Drawing.Size(794, 29);
            this.buttonChooseExecutor.TabIndex = 18;
            this.buttonChooseExecutor.Text = "Выбрать исполняемый файл интерпретатора FPTL";
            this.buttonChooseExecutor.UseVisualStyleBackColor = true;
            this.buttonChooseExecutor.Click += new System.EventHandler(this.buttonChooseExecutor_Click);
            // 
            // textBoxExecutorPath
            // 
            this.textBoxExecutorPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxExecutorPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBoxExecutorPath.Location = new System.Drawing.Point(18, 69);
            this.textBoxExecutorPath.Margin = new System.Windows.Forms.Padding(6, 8, 6, 8);
            this.textBoxExecutorPath.Name = "textBoxExecutorPath";
            this.textBoxExecutorPath.ReadOnly = true;
            this.textBoxExecutorPath.Size = new System.Drawing.Size(794, 26);
            this.textBoxExecutorPath.TabIndex = 17;
            // 
            // buttonCooseProgramFiles
            // 
            this.buttonCooseProgramFiles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCooseProgramFiles.Location = new System.Drawing.Point(18, 119);
            this.buttonCooseProgramFiles.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.buttonCooseProgramFiles.Name = "buttonCooseProgramFiles";
            this.buttonCooseProgramFiles.Size = new System.Drawing.Size(794, 29);
            this.buttonCooseProgramFiles.TabIndex = 16;
            this.buttonCooseProgramFiles.Text = "Выбрать файлы программ";
            this.buttonCooseProgramFiles.UseVisualStyleBackColor = true;
            this.buttonCooseProgramFiles.Click += new System.EventHandler(this.buttonCooseProgramFiles_Click);
            // 
            // textBoxProgramFiles
            // 
            this.textBoxProgramFiles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxProgramFiles.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBoxProgramFiles.Location = new System.Drawing.Point(18, 153);
            this.textBoxProgramFiles.Margin = new System.Windows.Forms.Padding(6, 8, 6, 8);
            this.textBoxProgramFiles.Multiline = true;
            this.textBoxProgramFiles.Name = "textBoxProgramFiles";
            this.textBoxProgramFiles.ReadOnly = true;
            this.textBoxProgramFiles.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxProgramFiles.Size = new System.Drawing.Size(794, 72);
            this.textBoxProgramFiles.TabIndex = 15;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(68, 446);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(629, 20);
            this.label2.TabIndex = 21;
            this.label2.Text = "Количество запусков каждой программы с каждым набором параметров запуска.";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(68, 478);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(748, 20);
            this.label3.TabIndex = 22;
            this.label3.Text = "Максимально допустимый процент использования процессорного времени другими процес" +
    "сами.";
            this.label3.Visible = false;
            // 
            // textBoxNumberOfLaunches
            // 
            this.textBoxNumberOfLaunches.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.textBoxNumberOfLaunches.Location = new System.Drawing.Point(18, 443);
            this.textBoxNumberOfLaunches.MaxLength = 4;
            this.textBoxNumberOfLaunches.Name = "textBoxNumberOfLaunches";
            this.textBoxNumberOfLaunches.Size = new System.Drawing.Size(44, 26);
            this.textBoxNumberOfLaunches.TabIndex = 23;
            this.textBoxNumberOfLaunches.Text = "10";
            this.textBoxNumberOfLaunches.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBoxNumberOfLaunches.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxNumberOfLaunches_KeyPress);
            // 
            // textBoxMaxBackCPUusage
            // 
            this.textBoxMaxBackCPUusage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.textBoxMaxBackCPUusage.Location = new System.Drawing.Point(18, 475);
            this.textBoxMaxBackCPUusage.MaxLength = 2;
            this.textBoxMaxBackCPUusage.Name = "textBoxMaxBackCPUusage";
            this.textBoxMaxBackCPUusage.Size = new System.Drawing.Size(44, 26);
            this.textBoxMaxBackCPUusage.TabIndex = 24;
            this.textBoxMaxBackCPUusage.Text = "10";
            this.textBoxMaxBackCPUusage.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBoxMaxBackCPUusage.Visible = false;
            this.textBoxMaxBackCPUusage.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxMaxBackCPUusage_KeyPress);
            // 
            // buttonBegin
            // 
            this.buttonBegin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonBegin.Location = new System.Drawing.Point(633, 507);
            this.buttonBegin.Name = "buttonBegin";
            this.buttonBegin.Size = new System.Drawing.Size(179, 29);
            this.buttonBegin.TabIndex = 25;
            this.buttonBegin.Text = "Начать выполнение";
            this.buttonBegin.UseVisualStyleBackColor = true;
            this.buttonBegin.Click += new System.EventHandler(this.buttonBegin_Click);
            // 
            // buttonPause
            // 
            this.buttonPause.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonPause.Enabled = false;
            this.buttonPause.Location = new System.Drawing.Point(216, 507);
            this.buttonPause.Name = "buttonPause";
            this.buttonPause.Size = new System.Drawing.Size(179, 29);
            this.buttonPause.TabIndex = 26;
            this.buttonPause.Text = "Приостановить";
            this.buttonPause.UseVisualStyleBackColor = true;
            this.buttonPause.Visible = false;
            this.buttonPause.Click += new System.EventHandler(this.buttonPause_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonCancel.Enabled = false;
            this.buttonCancel.Location = new System.Drawing.Point(17, 507);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(179, 29);
            this.buttonCancel.TabIndex = 27;
            this.buttonCancel.Text = "Отменить";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // dataGridViewLaunchParametrs
            // 
            this.dataGridViewLaunchParametrs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewLaunchParametrs.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewLaunchParametrs.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.LaunchParametrs});
            this.dataGridViewLaunchParametrs.Location = new System.Drawing.Point(18, 284);
            this.dataGridViewLaunchParametrs.Name = "dataGridViewLaunchParametrs";
            this.dataGridViewLaunchParametrs.RowHeadersWidth = 60;
            this.dataGridViewLaunchParametrs.Size = new System.Drawing.Size(794, 125);
            this.dataGridViewLaunchParametrs.TabIndex = 28;
            this.dataGridViewLaunchParametrs.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.dataGridViewLaunchParametrs_RowsAdded);
            this.dataGridViewLaunchParametrs.RowsRemoved += new System.Windows.Forms.DataGridViewRowsRemovedEventHandler(this.dataGridViewLaunchParametrs_RowsRemoved);
            // 
            // LaunchParametrs
            // 
            this.LaunchParametrs.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.LaunchParametrs.HeaderText = "Параметры запуска";
            this.LaunchParametrs.Name = "LaunchParametrs";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripProgressBar1,
            this.toolStripStatusLabelCPUmem,
            this.toolStripStatusLabelMem,
            this.toolStripStatusLabelTask});
            this.statusStrip1.Location = new System.Drawing.Point(0, 539);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(824, 22);
            this.statusStrip1.TabIndex = 29;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(100, 16);
            this.toolStripProgressBar1.Step = 1;
            // 
            // toolStripStatusLabelCPUmem
            // 
            this.toolStripStatusLabelCPUmem.AutoSize = false;
            this.toolStripStatusLabelCPUmem.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.toolStripStatusLabelCPUmem.Name = "toolStripStatusLabelCPUmem";
            this.toolStripStatusLabelCPUmem.Size = new System.Drawing.Size(65, 17);
            this.toolStripStatusLabelCPUmem.Text = "CPU: 0%";
            // 
            // toolStripStatusLabelMem
            // 
            this.toolStripStatusLabelMem.AutoSize = false;
            this.toolStripStatusLabelMem.Name = "toolStripStatusLabelMem";
            this.toolStripStatusLabelMem.Size = new System.Drawing.Size(100, 17);
            this.toolStripStatusLabelMem.Text = "Mem: 0MB";
            this.toolStripStatusLabelMem.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripStatusLabelTask
            // 
            this.toolStripStatusLabelTask.Name = "toolStripStatusLabelTask";
            this.toolStripStatusLabelTask.Size = new System.Drawing.Size(542, 17);
            this.toolStripStatusLabelTask.Spring = true;
            this.toolStripStatusLabelTask.Text = " ";
            this.toolStripStatusLabelTask.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.WorkerSupportsCancellation = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // buttonAddParametrs
            // 
            this.buttonAddParametrs.Location = new System.Drawing.Point(18, 249);
            this.buttonAddParametrs.Name = "buttonAddParametrs";
            this.buttonAddParametrs.Size = new System.Drawing.Size(794, 29);
            this.buttonAddParametrs.TabIndex = 30;
            this.buttonAddParametrs.Text = "Добавить набор параметров запуска";
            this.buttonAddParametrs.UseVisualStyleBackColor = true;
            this.buttonAddParametrs.Visible = false;
            // 
            // buttonContinue
            // 
            this.buttonContinue.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonContinue.Enabled = false;
            this.buttonContinue.Location = new System.Drawing.Point(435, 507);
            this.buttonContinue.Name = "buttonContinue";
            this.buttonContinue.Size = new System.Drawing.Size(179, 29);
            this.buttonContinue.TabIndex = 31;
            this.buttonContinue.Text = "Продолжить";
            this.buttonContinue.UseVisualStyleBackColor = true;
            this.buttonContinue.Visible = false;
            this.buttonContinue.Click += new System.EventHandler(this.buttonContinue_Click);
            // 
            // checkBoxMemControl
            // 
            this.checkBoxMemControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxMemControl.AutoSize = true;
            this.checkBoxMemControl.Location = new System.Drawing.Point(49, 415);
            this.checkBoxMemControl.Name = "checkBoxMemControl";
            this.checkBoxMemControl.Size = new System.Drawing.Size(699, 24);
            this.checkBoxMemControl.TabIndex = 32;
            this.checkBoxMemControl.Text = " Завершать процесс выполнения программы, если заканчивается оперативная память.";
            this.checkBoxMemControl.UseVisualStyleBackColor = true;
            this.checkBoxMemControl.Visible = false;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(824, 561);
            this.Controls.Add(this.checkBoxMemControl);
            this.Controls.Add(this.buttonContinue);
            this.Controls.Add(this.buttonAddParametrs);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.dataGridViewLaunchParametrs);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonPause);
            this.Controls.Add(this.buttonBegin);
            this.Controls.Add(this.textBoxMaxBackCPUusage);
            this.Controls.Add(this.textBoxNumberOfLaunches);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.buttonChooseExecutor);
            this.Controls.Add(this.textBoxExecutorPath);
            this.Controls.Add(this.buttonCooseProgramFiles);
            this.Controls.Add(this.textBoxProgramFiles);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MinimumSize = new System.Drawing.Size(840, 600);
            this.Name = "Main";
            this.Text = "Программа автоматического сбора статистики выполнения FPTL программ";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewLaunchParametrs)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem файлToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem выходToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem справкаToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem оПрограммеToolStripMenuItem;
        private System.Windows.Forms.Button buttonChooseExecutor;
        private System.Windows.Forms.TextBox textBoxExecutorPath;
        private System.Windows.Forms.Button buttonCooseProgramFiles;
        private System.Windows.Forms.TextBox textBoxProgramFiles;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxNumberOfLaunches;
        private System.Windows.Forms.TextBox textBoxMaxBackCPUusage;
        private System.Windows.Forms.Button buttonBegin;
        private System.Windows.Forms.Button buttonPause;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.DataGridView dataGridViewLaunchParametrs;
        private System.Windows.Forms.DataGridViewTextBoxColumn LaunchParametrs;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelTask;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelCPUmem;
        public System.ComponentModel.BackgroundWorker backgroundWorker1;
        public System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelMem;
        private System.Windows.Forms.Button buttonAddParametrs;
        private System.Windows.Forms.Button buttonContinue;
        private System.Windows.Forms.CheckBox checkBoxMemControl;
    }
}


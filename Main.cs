using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Windows.Forms;

namespace Auto_Statistic
{
    public partial class Main: Form
    {
        private const string settingsPath = @".\settings.dat";
        public static Executor.ExecutionParameters windowVars = new Executor.ExecutionParameters();
        private Executor executor;
        public static CheckAlg checkAlgorithm;

        public Main()
        {
            InitializeComponent();
            //DataBase.Create();
            windowVars = ReadSettingsFile(settingsPath);
            InitFields();
        }

        private void InitFields()
        {
            checkBoxInterpr.Checked = windowVars.Interpr;
            checkBoxMemControl.Checked = windowVars.prohibitUsePageFile;
            textBoxTimeLimit.Text = windowVars.timeLimit.ToString();
            textBoxNumberOfLaunches.Text = windowVars.launchNum.ToString();
            textBox_variance.Text = windowVars.variance.ToString(CultureInfo.InvariantCulture);
            textBoxMaxBackCPUusage.Text = windowVars.backProcLimit.ToString();

            textBoxExecutorPath.Text = "";
            foreach (string executionFile in windowVars.executionFilesPaths)
            {
                textBoxExecutorPath.Text += executionFile + Environment.NewLine;
            }

            textBoxProgramFiles.Text = "";
            foreach (string programPath in windowVars.textProgramFilesPaths)
            {
                textBoxProgramFiles.Text += programPath + Environment.NewLine;
            }

            while (windowVars.referenceResults.Count < windowVars.startParams.Count)
            {
                windowVars.referenceResults.Add("");
            }

            while (windowVars.interprParams.Count < windowVars.startParams.Count)
            {
                windowVars.interprParams.Add("");
            }

            dataGridViewLaunchParametrs.RowCount = windowVars.startParams.Count+1;
            dataGridViewLaunchParametrs.Rows[0].Cells[0].Value = "";
            dataGridViewLaunchParametrs.Rows[0].Cells[1].Value = "";
            dataGridViewLaunchParametrs.Rows[0].Cells[2].Value = "";
            for (int i=0; i < windowVars.startParams.Count; ++i)
            {
                dataGridViewLaunchParametrs.Rows[i].HeaderCell.Value = (i + 1).ToString();
                dataGridViewLaunchParametrs.Rows[i].Cells[0].Value = windowVars.interprParams[i];
                dataGridViewLaunchParametrs.Rows[i].Cells[1].Value = windowVars.startParams[i];
                dataGridViewLaunchParametrs.Rows[i].Cells[2].Value = windowVars.referenceResults[i];
            }
        }

        private Executor.ExecutionParameters ReadSettingsFile(string path)
        {
            if (!File.Exists(path)) return new Executor.ExecutionParameters();

            try
            {
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    Executor.ExecutionParameters state = (Executor.ExecutionParameters)formatter.Deserialize(fs);
                    
                    if (state.launchNum <= 0) state.launchNum = 1;
                    if (state.variance < 0) state.variance = 0.0005f;
                    if (state.backProcLimit <= 0 || state.backProcLimit > 100) state.backProcLimit = 100;

                    if (state.executionFilesPaths == null) state.executionFilesPaths = new List<string>();
                    List<string> removePathList = new List<string>();
                    foreach (string executionFile in state.executionFilesPaths)
                    {
                        if (String.IsNullOrEmpty(executionFile) || !File.Exists(executionFile))
                            removePathList.Add(executionFile);
                    }
                    foreach (string executionFile in removePathList)
                    {
                        state.executionFilesPaths.Remove(executionFile);
                    }

                    if (state.textProgramFilesPaths == null) state.textProgramFilesPaths = new List<string>();
                    removePathList = new List<string>();
                    foreach (string programPath in state.textProgramFilesPaths)
                    {
                        if (String.IsNullOrEmpty(programPath) || !File.Exists(programPath))
                            removePathList.Add(programPath);
                    }
                    foreach (string programPath in removePathList)
                    {
                        state.textProgramFilesPaths.Remove(programPath);
                    }

                    if (state.interprParams == null) state.interprParams = new List<string>();
                    if (state.startParams == null) state.startParams = new List<string>();
                    if (state.referenceResults == null) state.referenceResults = new List<string>();

                    while (state.referenceResults.Count < state.startParams.Count)
                        state.referenceResults.Add("");

                    while (state.interprParams.Count < state.startParams.Count)
                        state.interprParams.Add("");

                    List<int> removeIndexList = new List<int>();
                    for (var i = 0; i < state.startParams.Count; ++i)
                    {
                        if (String.IsNullOrEmpty(state.startParams[i]))
                        {
                            removeIndexList.Add(i);
                        }
                        else {
                            if (String.IsNullOrEmpty(state.referenceResults[i]))
                                state.referenceResults[i] = "";
                            if (String.IsNullOrEmpty(state.interprParams[i]))
                                state.interprParams[i] = "";
                        }
                    }
                    for (var j = removeIndexList.Count-1; j>=0; --j)
                    {
                        state.startParams.RemoveAt(removeIndexList[j]);
                        state.referenceResults.RemoveAt(removeIndexList[j]);
                        state.interprParams.RemoveAt(removeIndexList[j]);
                    }

                    if (String.IsNullOrEmpty(state.checkAlgorithmText))
                    {
                        state.checkAlgorithmText = CheckAlg.defaultAlg;
                        state.checkAlgorithmText = "";
                        state.checkAlgorithmText = "";
                    }

                    try
                    {
                        checkAlgorithm = new CheckAlg(state.checkAlgorithmUsingsText, state.checkAlgorithmClassesText, state.checkAlgorithmText);
                    }
                    catch (Exception)
                    {
                        state.checkAlgorithmText = CheckAlg.defaultAlg;
                        state.checkAlgorithmText = "";
                        state.checkAlgorithmText = "";
                        checkAlgorithm = new CheckAlg();
                    }

                    return state;
                }
            }
            catch (Exception exc)
            {
                ShowError("Файл настроек повреждён.\n", exc);
            }
            return new Executor.ExecutionParameters();
        }

        private void SaveSettingsFile(string path)
        {
            try
            {
                if (File.Exists(path)) File.SetAttributes(path, FileAttributes.Normal);
                using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(fs, windowVars);
                }
            }
            catch (Exception exc)
            {
                ShowError("Не удалось сохранить настройки.\n", exc);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (backgroundWorker1.IsBusy) buttonCancel_Click(sender, e);
            ReadFormFields();
            SaveSettingsFile(settingsPath);
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void buttonAddExecutors_Click(object sender, EventArgs e)
        {
            OpenFileDialog OPF = new OpenFileDialog
            {
                Multiselect = true,
                Title = "Выберите исполняемые файлы.",
                Filter = "Executable|*.exe|All files|*.*",
                CheckFileExists = true
            };
            if (OPF.ShowDialog() != DialogResult.OK || OPF.FileName == String.Empty) return;
            windowVars.executionFilesPaths.AddRange(OPF.FileNames.ToList());
            foreach (string file in OPF.FileNames)
            {
                textBoxExecutorPath.Text += file + Environment.NewLine;
            }
        }

        private void buttonAddProgramFiles_Click(object sender, EventArgs e)
        {
            OpenFileDialog OPF = new OpenFileDialog
            {
                Multiselect = true,
                Title = "Выберите файлы программ.",
                CheckFileExists = true
            };
            if (OPF.ShowDialog() != DialogResult.OK || OPF.FileName == String.Empty) return;
            windowVars.textProgramFilesPaths.AddRange(OPF.FileNames.ToList());
            foreach (string file in OPF.FileNames)
            {
                textBoxProgramFiles.Text += file + Environment.NewLine;
            }
        }

        private void textBoxNumberOfLaunches_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Delete && e.KeyChar != (char)Keys.Back && (e.KeyChar < '0' || e.KeyChar > '9'))
                e.KeyChar = '\0';
        }

        private void textBoxMaxBackCPUusage_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Delete && e.KeyChar != (char)Keys.Back && (e.KeyChar < '0' || e.KeyChar > '9'))
                e.KeyChar = '\0';
        }

        private void textBoxTimeLimit_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Delete && e.KeyChar != (char)Keys.Back && (e.KeyChar < '0' || e.KeyChar > '9'))
                e.KeyChar = '\0';
        }

        private void textBox_leftVal_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Delete && e.KeyChar != (char)Keys.Back && (e.KeyChar < '0' || e.KeyChar > '9'))
                e.KeyChar = '\0';
        }

        private void textBox_rightVal_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Delete && e.KeyChar != (char)Keys.Back && (e.KeyChar < '0' || e.KeyChar > '9'))
                e.KeyChar = '\0';
        }

        private void dataGridViewLaunchParameters_RowsAdded(object sender, 
            DataGridViewRowsAddedEventArgs e)
        {
            dataGridViewLaunchParametrs.Rows[e.RowIndex].HeaderCell.Value = 
                (e.RowIndex + 1).ToString();
            if (e.RowIndex != 0)
                dataGridViewLaunchParametrs.Rows[e.RowIndex - 1].HeaderCell.Value = 
                    (e.RowIndex).ToString();
        }

        private void dataGridViewLaunchParameters_RowsRemoved(object sender, 
            DataGridViewRowsRemovedEventArgs e)
        {
            for (var i = e.RowIndex; i < dataGridViewLaunchParametrs.RowCount; ++i)
            {
                dataGridViewLaunchParametrs.Rows[i].HeaderCell.Value = (i+1).ToString();
            }
        }

        private void buttonBegin_Click(object sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy || !ValidatedReadInputData()) return;
            buttonCancel.Enabled = true;
            buttonPause.Enabled = true;
            buttonContinue.Enabled = false;
            buttonBegin.Enabled = false;
            executor = new Executor(windowVars, checkAlgorithm);
            toolStripProgressBar1.Maximum = executor.GetTasksNumber();
            backgroundWorker1.RunWorkerAsync();
        }

        public bool ValidatedReadInputData()
        {
            ReadFormFields();

            if (windowVars.executionFilesPaths == null || windowVars.executionFilesPaths[0] == String.Empty)
            {
                MessageBox.Show("Не выбран ни один исполняемый файл!", "Ошибка");
                return false;
            }
            foreach (string file in windowVars.executionFilesPaths)
            {
                if (!File.Exists(file))
                {
                    MessageBox.Show("Не найден исполняемый файл по заданному пути: " + file, "Ошибка");
                    return false;
                }
            }

            if (windowVars.Interpr)
            {
                if (windowVars.textProgramFilesPaths == null || windowVars.textProgramFilesPaths[0] == String.Empty)
                {
                    MessageBox.Show("Не выбраны файлы программ!", "Ошибка");
                    return false;
                }

                foreach (string file in windowVars.textProgramFilesPaths)
                {
                    if (!File.Exists(file))
                    {
                        MessageBox.Show("Не найден файл программы по заданному пути: " + file, "Ошибка");
                        return false;
                    }
                }
            }
            else
            {
                windowVars.textProgramFilesPaths = new List<string>()
                {
                    ""
                };
            }

            return true;
        }

        private void ReadFormFields()
        {
            windowVars.prohibitUsePageFile = checkBoxMemControl.Checked;
            windowVars.Interpr = checkBoxInterpr.Checked;

            if (!UInt16.TryParse(textBoxNumberOfLaunches.Text, out windowVars.launchNum))
            {
                windowVars.launchNum = 10;
                textBoxNumberOfLaunches.Text = windowVars.launchNum.ToString();
            }

            textBox_variance.Text = textBox_variance.Text.Replace(',', '.');
            if (!float.TryParse(textBox_variance.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out windowVars.variance) || windowVars.variance < 0)
            {
                windowVars.variance = 0.0005f;
                textBox_variance.Text = windowVars.variance.ToString(CultureInfo.InvariantCulture);
            }

            if (!Byte.TryParse(textBoxMaxBackCPUusage.Text, out windowVars.backProcLimit))
            {
                windowVars.backProcLimit = 10;
                textBoxMaxBackCPUusage.Text = windowVars.backProcLimit.ToString();
            }

            if (!int.TryParse(textBoxTimeLimit.Text, out windowVars.timeLimit) || windowVars.timeLimit < 0)
            {
                windowVars.timeLimit = 0;
                textBoxTimeLimit.Text = windowVars.timeLimit.ToString();
            }

            windowVars.startParams.Clear();
            windowVars.referenceResults.Clear();
            windowVars.interprParams.Clear();
            for (var i = 0; i < dataGridViewLaunchParametrs.Rows.Count - 1; ++i)
            {
                if (dataGridViewLaunchParametrs.Rows[i].Cells[1].Value == null) continue;

                var startParam = dataGridViewLaunchParametrs.Rows[i].Cells[1].Value.ToString();
                if (String.IsNullOrEmpty(startParam)) continue;
                windowVars.startParams.Add(startParam);

                string interprParams = "";
                if (dataGridViewLaunchParametrs.Rows[i].Cells[0].Value != null)
                    interprParams = dataGridViewLaunchParametrs.Rows[i].Cells[0].Value.ToString();
                windowVars.interprParams.Add(interprParams);

                string result = "";
                if (dataGridViewLaunchParametrs.Rows[i].Cells[2].Value != null)
                    result = dataGridViewLaunchParametrs.Rows[i].Cells[2].Value.ToString();
                windowVars.referenceResults.Add(result);
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            executor.Cancel();
            while (backgroundWorker1.IsBusy)
            {
                Application.DoEvents();
                Thread.Sleep(100);
            }
            buttonCancel.Enabled = false;
            buttonPause.Enabled = false;
            buttonContinue.Enabled = false;
            buttonBegin.Enabled = true;
        }

        private void buttonPause_Click(object sender, EventArgs e)
        {
            buttonCancel.Enabled = true;
            buttonPause.Enabled = false;
            buttonContinue.Enabled = true;
            buttonBegin.Enabled = false;
        }

        private void buttonContinue_Click(object sender, EventArgs e)
        {
            buttonCancel.Enabled = true;
            buttonPause.Enabled = true;
            buttonContinue.Enabled = false;
            buttonBegin.Enabled = false;
        }

        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox aboutBox = new AboutBox();
            aboutBox.ShowDialog();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
               executor.Run();
            }
            catch (Exception exc)
            {
                ShowError("", exc);
            }
        }

        public static void ShowError(String mess, Exception exc)
        {
            String errorTrace = mess;
            while (exc != null)
            {
                errorTrace += exc.Message + "\n" + exc.StackTrace + "\n\n";
                exc = exc.InnerException;
            }
            MessageBox.Show(errorTrace, "Ошибка");
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            buttonCancel.Enabled = false;
            buttonPause.Enabled = false;
            buttonContinue.Enabled = false;
            buttonBegin.Enabled = true;
            Process.Start(executor.GetResultsPath());
            executor = null;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (executor != null)
            {
                string strCurProcMem;
                float curProcMem = executor.GetCurProcMemUsage();
                if (curProcMem >= 1024) strCurProcMem = (curProcMem / 1024).ToString("F") + "GB";
                else strCurProcMem = curProcMem.ToString("F") + "MB";

                toolStripStatusLabelCPUmem.Text = "CPU: " + executor.GetCurProcCpuUsage().ToString("P0");
                toolStripStatusLabelMem.Text = "Mem: " + strCurProcMem;

                toolStripProgressBar1.Value = executor.GetCompletedTasksNum();
                toolStripStatusLabelTask.Text = executor.GetProgressStatus();
            }
            else
            {
                toolStripStatusLabelCPUmem.Text = "CPU: 0%";
                toolStripStatusLabelMem.Text = "Mem: 0MB";

                toolStripProgressBar1.Value = 0;
                toolStripStatusLabelTask.Text = "";
            }
        }

        private void сохранитьКонфигурациюToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog SFD = new SaveFileDialog
            {
                Title = "Сохранить конфигурацию.",
                Filter = "Конфигурация|*.ASconfig"
            };
            if (SFD.ShowDialog() != DialogResult.OK || SFD.FileName == String.Empty) return;
            ReadFormFields();
            SaveSettingsFile(SFD.FileName);
        }

        private void загрузитьКонфигурациюToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog OFD = new OpenFileDialog
            {
                Title = "Выберите файл с конфигурацией.",
                CheckFileExists = true,
                Filter = "Конфигурация|*.ASconfig"
            };
            if (OFD.ShowDialog() != DialogResult.OK || OFD.FileName == String.Empty) return;
            windowVars = ReadSettingsFile(OFD.FileName);
            InitFields();
        }

        private void задатьУсловиеПроверкиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CheckInput checkInput = new CheckInput();
            checkInput.ShowDialog();
        }

        private void checkBoxInterpr_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxInterpr.Checked)
            {
                windowVars.Interpr = true;
                buttonCooseProgramFiles.Visible = true;
                buttonClearProgramFiles.Visible = true;
                textBoxProgramFiles.Visible = true;
                textBoxExecutorPath.Size = new Size(textBoxExecutorPath.Width, buttonCooseProgramFiles.Top - textBoxExecutorPath.Top - 8);
            }
            else
            {
                windowVars.Interpr = false;
                buttonCooseProgramFiles.Visible = false;
                buttonClearProgramFiles.Visible = false;
                textBoxProgramFiles.Visible = false;
                textBoxExecutorPath.Size = new Size(textBoxExecutorPath.Width, textBox_varWord.Top - textBoxExecutorPath.Top - 8);
            }
        }

        private void button_change_Click(object sender, EventArgs e)
        {
            var varWord = textBox_varWord.Text;
            if (String.IsNullOrEmpty(varWord)) return;

            Int32.TryParse(textBox_leftVal.Text, out var a);
            Int32.TryParse(textBox_rightVal.Text, out var b);
            if (a>b) return;

            ReadFormFields();
            List<string> newParams = new List<string>();
            List<string> newResults = new List<string>();
            List<string> newInterprParams = new List<string>();

            while (windowVars.referenceResults.Count < windowVars.startParams.Count)
                windowVars.referenceResults.Add("");

            while (windowVars.interprParams.Count < windowVars.startParams.Count)
                windowVars.interprParams.Add("");

            for (var p = 0; p < windowVars.startParams.Count; ++p)
            {
                if (windowVars.startParams[p].Contains(varWord))
                {
                    for (var i = a; i <= b; ++i)
                    {
                        newParams.Add(windowVars.startParams[p].Replace(varWord, i.ToString()));
                        newResults.Add(windowVars.referenceResults[p]);
                        newInterprParams.Add(windowVars.interprParams[p]);
                    }
                }
                else
                {
                    newParams.Add(windowVars.startParams[p]);
                    newResults.Add(windowVars.referenceResults[p]);
                    newInterprParams.Add(windowVars.interprParams[p]);
                }
            }

            windowVars.startParams = newParams;
            windowVars.referenceResults = newResults;
            windowVars.interprParams = newInterprParams;
            InitFields();
        }

        private void button_erase_Click(object sender, EventArgs e)
        {
            windowVars.startParams = new List<string>();
            InitFields();
        }

        private void buttonClearExecutions_Click(object sender, EventArgs e)
        {
            windowVars.executionFilesPaths.Clear();
            textBoxExecutorPath.Text = "";
        }

        private void buttonClearProgramFiles_Click(object sender, EventArgs e)
        {
            windowVars.textProgramFilesPaths.Clear();
            textBoxProgramFiles.Text = "";
        }
    }
}

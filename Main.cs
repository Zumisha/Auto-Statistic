using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FPTL_Auto_Statistic
{
    public partial class Main : Form
    {
        [Serializable]
        private class WindowState
        {
            public string fptlPath = "";
            public List<string> programsPaths = new List<string>();
            public ushort launchNum = 10;
            public byte backProcLimit = 10;
            public List<string> startParams = new List<string>();
            public bool prohibitUsePageFile = true;
        }

        private readonly string[] coreParams = {
            "--num-cores ",
            "-n "
        };

        private const string settingsPath = @".\settings.dat";
        private WindowState windowVars;
        private Profiler profiler;
        private string curPath;

        [Serializable]
        private struct Results
        {
            public string programName;
            public string arguments;
            public int threadsCount;
            public double avgExecTime;
            public double variance;
            public float maxMemUsage;
            public float maxCpuUsage;
            public float avgCpuUsage;
            public string execStatus;

            private static string CsvStringBuilder(string programName, string arguments, string threadsCount, string avgExecTime, string variance, string maxMemUsage, string maxCpuUsage, string avgCpuUsage, string execStatus)
            {
                return "\"" + programName + "\";\"" + arguments + "\";\"" + threadsCount + "\";\"" + avgExecTime + "\";\"" + variance + "\";\"" +
                       maxMemUsage + "\";\"" + maxCpuUsage + "\";\"" + avgCpuUsage + "\";\""+ execStatus + "\"";
            }
            public static string CsvNames()
            {
                return CsvStringBuilder("Program", "Arguments", "Threads", "Time, s", "Variance", "Max RAM, MB","Max CPU, %", "Avg CPU, %", "Status");
            }
            public static string CsvEmpty()
            {
                return CsvStringBuilder("", "", "", "", "", "", "", "", "");
            }
            public string CsvResults()
            {
                return CsvStringBuilder(programName, arguments, threadsCount.ToString(), avgExecTime.ToString("F3"),
                    variance.ToString("F"), maxMemUsage.ToString("F"), maxCpuUsage.ToString("P2"), avgCpuUsage.ToString("P2"), execStatus);
            }
        }

        public Main()
        {
            InitializeComponent();
            windowVars = ReadSettingsFile(settingsPath);
            InitFields();
        }

        private void InitFields()
        {
            textBoxExecutorPath.Text = windowVars.fptlPath;
            foreach (string programPath in windowVars.programsPaths)
            {
                textBoxProgramFiles.Text += programPath + Environment.NewLine;
            }

            dataGridViewLaunchParametrs.RowCount = windowVars.startParams.Count+1;
            for (int i=0; i < windowVars.startParams.Count; ++i)
            {
                dataGridViewLaunchParametrs.Rows[i].HeaderCell.Value = (i + 1).ToString();
                dataGridViewLaunchParametrs.Rows[i].Cells[0].Value = windowVars.startParams[i];
            }

            checkBoxMemControl.Checked = windowVars.prohibitUsePageFile;
            textBoxNumberOfLaunches.Text = windowVars.launchNum.ToString();
            textBoxMaxBackCPUusage.Text = windowVars.backProcLimit.ToString();
        }

        private WindowState ReadSettingsFile(string path)
        {
            if (File.Exists(path))
            {
                try
                {
                    using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        WindowState state = (WindowState)formatter.Deserialize(fs);

                        if (!String.IsNullOrEmpty(state.fptlPath) && !File.Exists(state.fptlPath))
                            state.fptlPath = "";
                        
                        foreach (string programPath in state.programsPaths)
                        {
                            if (String.IsNullOrEmpty(programPath) || !File.Exists(programPath))
                                state.programsPaths.Remove(programPath);
                        }

                        foreach (string startParam in state.startParams)
                        {
                            if (String.IsNullOrEmpty(startParam))
                                state.startParams.Remove(startParam);
                        }

                        if (state.launchNum <= 0) state.launchNum = 10;
                        if (state.launchNum < 0 || state.backProcLimit > 100) state.backProcLimit = 100;

                        return state;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Файл настроек повреждён\nнастройки будут сброшены.", "Ошибка");
                }
            }
            return new WindowState();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (backgroundWorker1.IsBusy)
                buttonCancel_Click(sender, e);
            ReadFormFields();
            try
            {
                if (File.Exists(settingsPath))
                    File.SetAttributes(settingsPath, FileAttributes.Normal);
                using (FileStream fs = new FileStream(settingsPath, FileMode.Create, FileAccess.Write))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(fs, windowVars);
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show("Не удалось сохранить настройки.\n" + exc.Message, "Ошибка");
            }
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void buttonChooseExecutor_Click(object sender, EventArgs e)
        {
            OpenFileDialog OPF = new OpenFileDialog { Title = "Выберите исполняемый файл интерпретатора.", CheckFileExists = true };
            if (OPF.ShowDialog() != DialogResult.OK || OPF.FileName == String.Empty) return;
            windowVars.fptlPath = OPF.FileName;
            textBoxExecutorPath.Text = windowVars.fptlPath;
        }

        private void buttonChooseProgramFiles_Click(object sender, EventArgs e)
        {
            OpenFileDialog OPF = new OpenFileDialog { Multiselect = true, Title = "Выберите файлы FPTL программ.", CheckFileExists = true };
            if (OPF.ShowDialog() != DialogResult.OK || OPF.FileName == String.Empty) return;
            windowVars.programsPaths = OPF.FileNames.ToList();
            textBoxProgramFiles.Text = "";
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

        private void dataGridViewLaunchParametrs_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            dataGridViewLaunchParametrs.Rows[e.RowIndex].HeaderCell.Value = (e.RowIndex + 1).ToString();
            if (e.RowIndex != 0)
                dataGridViewLaunchParametrs.Rows[e.RowIndex - 1].HeaderCell.Value = (e.RowIndex).ToString();
        }

        private void dataGridViewLaunchParametrs_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            for (int i = e.RowIndex; i < dataGridViewLaunchParametrs.RowCount; ++i)
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
            toolStripProgressBar1.Maximum =
                windowVars.programsPaths.Count * (dataGridViewLaunchParametrs.Rows.Count - 1) * windowVars.launchNum;
            backgroundWorker1.RunWorkerAsync();
        }

        public bool ValidatedReadInputData()
        {
            if (windowVars.fptlPath == String.Empty)
            {
                MessageBox.Show("Не выбран исполняемый файл интерпретатора!", "Ошибка");
                return false;
            }
            if (!File.Exists(windowVars.fptlPath))
            {
                MessageBox.Show("Не найден исполняемый файл интерпретатора по заданному пути!", "Ошибка");
                return false;
            }
            if (windowVars.programsPaths == null || windowVars.programsPaths[0] == String.Empty)
            {
                MessageBox.Show("Не выбраны файлы FPTL программ!", "Ошибка");
                return false;
            }
            foreach (string file in windowVars.programsPaths)
            {
                if (!File.Exists(file))
                {
                    MessageBox.Show("Не найден файл FPTL программы по заданному пути: " + file, "Ошибка");
                    return false;
                }
            }

            ReadFormFields();
            return true;
        }

        private void ReadFormFields()
        {
            if (!UInt16.TryParse(textBoxNumberOfLaunches.Text, out windowVars.launchNum))
            {
                windowVars.launchNum = 10;
                textBoxNumberOfLaunches.Text = windowVars.launchNum.ToString();
            }

            if (!Byte.TryParse(textBoxMaxBackCPUusage.Text, out windowVars.backProcLimit))
            {
                windowVars.backProcLimit = 10;
                textBoxMaxBackCPUusage.Text = windowVars.backProcLimit.ToString();
            }

            windowVars.prohibitUsePageFile = checkBoxMemControl.Checked;

            windowVars.startParams.Clear();
            for (int i = 0; i < dataGridViewLaunchParametrs.Rows.Count - 1; ++i)
            {
                string startParam = dataGridViewLaunchParametrs.Rows[i].Cells[0].Value.ToString();
                if (!String.IsNullOrEmpty(startParam))
                    windowVars.startParams.Add(startParam);
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            backgroundWorker1.CancelAsync();
            profiler.CancelProcess();
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

        private void writeFullSystemInfo(string path)
        {
            if (File.Exists(path)) File.SetAttributes(path, FileAttributes.Normal);
            using (StreamWriter sysInfoFS = new StreamWriter(File.Create(path), Encoding.GetEncoding(1251)))
                foreach (var infoType in FullSystemInfo.Get())
                {
                    sysInfoFS.WriteLine(infoType.Key + ";;");
                    foreach (var info in infoType.Value)
                        sysInfoFS.WriteLine(";" + info.Key + ";" + info.Value);
                }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                string resultsFolderPath = @".\results\";
                if (!Directory.Exists(resultsFolderPath)) Directory.CreateDirectory(resultsFolderPath);

                string curTime = $"{DateTime.Now:yyyy'-'MM'-'dd'-T'HH'-'mm'-'ss}";
                curPath = resultsFolderPath + @"\" + curTime + @"\";
                if (!Directory.Exists(curPath)) Directory.CreateDirectory(curPath);

                backgroundWorker1.ReportProgress(0, "Сбор информации о системе.");
                string sysInfoPath = curPath + @"\System-Info_" + curTime + ".csv";
                writeFullSystemInfo(sysInfoPath);

                string profilerFolderPath = curPath + @"\Profiler\";
                if (!Directory.Exists(profilerFolderPath)) Directory.CreateDirectory(profilerFolderPath);

                string logsFolderPath = curPath + @"\Logs\";
                if (!Directory.Exists(logsFolderPath)) Directory.CreateDirectory(logsFolderPath);

                string resultsPath = curPath + @"\Results_" + curTime + ".csv";
                if (File.Exists(resultsPath)) File.SetAttributes(resultsPath, FileAttributes.Normal);

                int tasksCompleted = 0;
                using (StreamWriter resFS = new StreamWriter(File.Create(resultsPath), Encoding.GetEncoding(1251)))
                {
                    resFS.WriteLine(Results.CsvNames());

                    foreach (string program in windowVars.programsPaths)
                    {
                        resFS.WriteLine(Results.CsvEmpty());

                        string programPath = Path.GetFileNameWithoutExtension(program);
                        string logPath = logsFolderPath + @"\Log_" + programPath + "_" + curTime + ".txt";
                        if (File.Exists(logPath)) File.SetAttributes(logPath, FileAttributes.Normal);

                        using (StreamWriter outLogFS = new StreamWriter(File.Create(logPath), Encoding.GetEncoding(1251)))
                            for (int par = 0; par < windowVars.startParams.Count; ++par)
                            {
                                Results results = new Results
                                {
                                    programName = programPath,
                                    arguments = windowVars.startParams[par],
                                    avgExecTime = 0,
                                    variance = 0,
                                    maxCpuUsage = 0,
                                    maxMemUsage = 0,
                                    avgCpuUsage = 0,
                                    execStatus = "Success"
                                };

                                outLogFS.WriteLine(results.arguments);
                                outLogFS.WriteLine("------------------------------------------------------------");

                                int n = 0;
                                for (int iteration = 1; iteration <= windowVars.launchNum; ++iteration)
                                {
                                    if (backgroundWorker1.CancellationPending)
                                    {
                                        profiler = null;
                                        return;
                                    }

                                    backgroundWorker1.ReportProgress(tasksCompleted, 
                                        "Набор №" + (par + 1) + ". Проход №" + iteration + ". " + results.programName);

                                    outLogFS.WriteLine("\n____________________________________________________________\n");
                                    outLogFS.WriteLine("Iteration: " + iteration + "\n");

                                    string profilerPath = profilerFolderPath + @"\" + results.programName + "_" +
                                                          results.arguments + "_" + iteration + "_" + curTime + ".csv";
                                    if (File.Exists(profilerPath)) File.SetAttributes(profilerPath, FileAttributes.Normal);
                                    profiler = new Profiler(windowVars.fptlPath,
                                        "--source-file \"" + program + "\" " + results.arguments, profilerPath, windowVars.prohibitUsePageFile);

                                    Profiler.ProfilerStatistic stats = profiler.StartProcess();
                                    int successCheck = stats.programResult.IndexOf("Time", StringComparison.Ordinal);
                                    if (profiler.IsExceededMemory())
                                    {
                                        results.execStatus = "Exceeded Memory";
                                        outLogFS.WriteLine("Exceeded memory!\n");
                                        break;
                                    }
                                    if (profiler.IsProcessCanceled())
                                    {
                                        results.execStatus = "Canceled";
                                        outLogFS.WriteLine("Canceled!\n");
                                    }
                                    else if (successCheck == -1)
                                    {
                                        results.execStatus = "Errored";
                                        outLogFS.WriteLine("Errored!\n");
                                    }
                                    else
                                    {
                                        ++n;
                                        if (n > 1)
                                        {
                                            results.variance = (results.variance + Math.Pow(stats.execTime - results.avgExecTime, 2) / n -
                                                                results.variance / (n - 1));
                                        }
                                        results.avgExecTime = results.avgExecTime + (stats.execTime - results.avgExecTime) / n;
                                        results.avgCpuUsage = results.avgCpuUsage + (stats.avgCpuUsage - results.avgCpuUsage) / n;
                                        if (results.maxMemUsage < stats.maxMemUsage) results.maxMemUsage = stats.maxMemUsage;
                                        if (results.maxCpuUsage < stats.maxCpuUsage) results.maxCpuUsage = stats.maxCpuUsage;
                                    }

                                    profiler = null;
                                    outLogFS.WriteLine(stats.programResult);
                                    tasksCompleted++;
                                }

                                if (windowVars.launchNum == 1) results.variance = Double.NaN;                             
                                results.threadsCount = parseCoreNum(results.arguments);
                                resFS.WriteLine(results.CsvResults());

                                outLogFS.WriteLine("\n------------------------------------------------------------");
                            }
                    }
                }

            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Ошибка");
            }
        }

        private int parseCoreNum(string arguments)
        {
            int coreNum, corePos = -1, i = 0;
            while (corePos == -1 && i < coreParams.Length)
            {
                corePos = arguments.IndexOf(coreParams[i], StringComparison.Ordinal);
                ++i;
            }
            if (corePos == -1)
            {
                coreNum = 1;
            }
            else
            {
                string arg = arguments.Substring(corePos + coreParams.Length);
                int splitPos = arg.IndexOf(" ", StringComparison.Ordinal);
                if (splitPos != -1)
                    arg = arg.Substring(0, splitPos);
                if (!Int32.TryParse(arguments, out coreNum))
                    coreNum = 0;
            }
            return coreNum;
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            toolStripStatusLabelTask.Text = e.UserState.ToString();
            toolStripProgressBar1.Value = e.ProgressPercentage;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            toolStripStatusLabelTask.Text = "";
            toolStripProgressBar1.Value = 0;
            toolStripStatusLabelCPUmem.Text = "CPU: 0%";
            toolStripStatusLabelMem.Text = "Mem: 0MB";
            buttonCancel.Enabled = false;
            buttonPause.Enabled = false;
            buttonContinue.Enabled = false;
            buttonBegin.Enabled = true;
            Process.Start(curPath);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (profiler != null)
            {
                string strCurProcMem;
                float curProcMem = profiler.GetCurProcMemUsage();
                if (curProcMem >= 1024) strCurProcMem = (curProcMem / 1024).ToString("F") + "GB";
                else strCurProcMem = curProcMem.ToString("F") + "MB";

                toolStripStatusLabelCPUmem.Text = "CPU: " + profiler.GetCurProcCpuUsage().ToString("P0");
                toolStripStatusLabelMem.Text = "Mem: " + strCurProcMem;
            }
            else
            {
                toolStripStatusLabelCPUmem.Text = "CPU: 0%";
                toolStripStatusLabelMem.Text = "Mem: 0MB";
            }
        }
    }
}

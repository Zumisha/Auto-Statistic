using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Collections.Concurrent;

namespace Auto_Statistic
{
    partial class Profiler
    {
        private readonly Process curProcess;
        private const int startInterval = 10;
        private const int interval = 100;
        private readonly int timeLimit = 0;
        private readonly bool prohibitUsePageFile;
        private readonly string historySavePath;
        private static readonly double criticalAvailableMemSizeMB = 0.05 * FullSystemInfo.GeneralRamInfo.totalRamSizeMB;
        private const int maxMemHistorySize = 1024 * 32; // по 32Б
        private readonly ConcurrentQueue<StatisticUnit> History = new ConcurrentQueue<StatisticUnit>();
        private ProfilerResultStatistic evalStat;
        private bool processRunning = false;
        private long statCount;
        private bool processCanceled = false;
        private bool exceededMemory = false;
        private float curProcMem = 0;
        private float curProcCPU = 0;
        private double lastProc = 0;
        private double lastTotal = 0;

        public Profiler(string path, string args, string historySavePath, bool prohibitUsePageFile = true, int timeLimit = 0)
        {
            curProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = path,
                    Arguments = args,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                },
                EnableRaisingEvents = true
            };
            curProcess.ErrorDataReceived += ErrorReceived;
            curProcess.OutputDataReceived += DataReceived;
            this.historySavePath = historySavePath;
            this.prohibitUsePageFile = prohibitUsePageFile;
            if (timeLimit == 0) this.timeLimit = -1;
            else this.timeLimit = 1000 * timeLimit;
        }

        public ProfilerResultStatistic StartProcess()
        {
            processCanceled = false;
            processRunning = true;
            exceededMemory = false;
            statCount = 0;
            evalStat = new ProfilerResultStatistic();

            if (File.Exists(historySavePath)) File.SetAttributes(historySavePath, FileAttributes.Normal);
            using (StreamWriter fs = new StreamWriter(File.Create(historySavePath), Encoding.GetEncoding(1251)))
            {
                fs.WriteLine("Time;CPU usage;RAM usage");
            }

            Timer statTimer = new Timer(GetCurStat, null, 0, startInterval);

            curProcess.Start();
            curProcess.BeginErrorReadLine();
            curProcess.BeginOutputReadLine();
            curProcess.PriorityClass = ProcessPriorityClass.BelowNormal;

            curProcess.WaitForExit(500);
            statTimer.Change(0, interval);

            curProcess.WaitForExit(timeLimit);
            if (!curProcess.HasExited)
            {
                CancelProcess();
            }

            evalStat.execTime = (curProcess.ExitTime - curProcess.StartTime).TotalSeconds;
            statTimer.Dispose();
            SaveStoredStats();

            processRunning = false;
            return evalStat;
        }

        void DataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
                evalStat.programResult.AppendLine(e.Data);
        }

        void ErrorReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
                evalStat.programResult.AppendLine(e.Data);
        }

        public bool IsProcessRunning()
        {
            return processRunning;
        }

        public bool IsProcessCanceled()
        {
            return processCanceled;
        }

        public bool IsExceededMemory()
        {
            return exceededMemory;
        }

        public void CancelProcess()
        {
            try
            {
                curProcess.Kill();
                processCanceled = true;
            }
            catch (Exception exc)
            {
                // Process already stop.
                Debug.WriteLine(exc.ToString());
            }
        }

        private void GetCurStat(object obj = null)
        {
            try
            {
                curProcess.Refresh();
                var newProcessTime = curProcess.TotalProcessorTime.TotalSeconds;
                var newTotalTime = (DateTime.Now - curProcess.StartTime).TotalSeconds;

                if (prohibitUsePageFile && SystemStateInfo.AvailableRamSize() < criticalAvailableMemSizeMB)
                {
                    CancelProcess();
                    exceededMemory = true;
                    return;
                }

                curProcMem = (float)((double)curProcess.WorkingSet64 / 1024 / 1024);
                curProcCPU = (float)((newProcessTime - lastProc) / Environment.ProcessorCount / (newTotalTime - lastTotal));
                lastTotal = newTotalTime;
                lastProc = newProcessTime;

                if (curProcCPU < 0 || curProcCPU > 1 || double.IsNaN(curProcCPU)) curProcCPU = evalStat.avgCpuUsage;
                if (curProcCPU > evalStat.maxCpuUsage) evalStat.maxCpuUsage = curProcCPU;

                statCount++;
                evalStat.avgCpuUsage += (curProcCPU - evalStat.avgCpuUsage) / statCount;
                if (curProcMem > evalStat.maxMemUsage) evalStat.maxMemUsage = curProcMem;

                History.Enqueue(new StatisticUnit(curProcCPU, curProcMem, newTotalTime));
                if (History.Count >= maxMemHistorySize) SaveStoredStats();
            }
            catch (Exception exc)
            {
                // Process not running.
                Debug.WriteLine(exc.ToString());
            }
        }
        
        private void SaveStoredStats()
        {
            using (StreamWriter fs = new StreamWriter(File.Open(historySavePath, FileMode.Append), Encoding.GetEncoding(1251)))
            {
                while (History.TryDequeue(out var stat))
                {
                    fs.WriteLine(stat.time.ToString("F") +  ";" + stat.cpuUsage.ToString("P2") + ";" + stat.ramUsage.ToString("F"));
                }
            }
        }

        public float GetCurProcCpuUsage()
        {
            return curProcCPU;
        }

        public float GetCurProcMemUsage()
        {
            return curProcMem;
        }
    }
}

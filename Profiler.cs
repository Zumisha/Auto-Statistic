using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace FPTL_Auto_Statistic
{
    class Profiler
    {
        private readonly Process curProcess;
        private readonly int interval;
        private readonly bool prohibitUsePageFile;
        private readonly string historySavePath;
        private PerformanceCounter processCpuUsage;
        private PerformanceCounter processRamUsage;
        private static readonly double criticalAvailableMemSizeMB = 0.05 * FullSystemInfo.GeneralRamInfo.totalRamSizeMB;
        private const int maxMemHistorySize = 1024 * 32;
        private DateTime startTime;
        private List<StatUnit> History = new List<StatUnit>(maxMemHistorySize);
        private ProfilerStatistic evalStat;
        private bool processRunning = false;
        private long statCount;
        private bool processCanceled = false;
        private bool exceededMemory = false;
        private float curProcMem = 0;
        private float curProcCPU = 0;


        private struct StatUnit
        {
            public readonly float cpuUsage;
            public readonly float ramUsage;
            public readonly double time;

            public StatUnit(float cpuUsage, float ramUsage, double time)
            {
                this.cpuUsage = cpuUsage;
                this.ramUsage = ramUsage;
                this.time = time;
            }

        }
        public class ProfilerStatistic
        {
            public float maxMemUsage = 0;
            public float maxCpuUsage = 0;
            public float avgCpuUsage = 0;
            public double execTime = 0;
            public string programResult = "";
        }

        public Profiler(string path, string args, string historySavePath, bool prohibitUsePageFile = true, int interval = 100)
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
            this.interval = interval;
            this.prohibitUsePageFile = prohibitUsePageFile;
        }

        public ProfilerStatistic StartProcess()
        {
            Mutex startLock = new Mutex();
            startLock.WaitOne();
            processCanceled = false;
            processRunning = true;
            exceededMemory = false;
            statCount = 0;
            evalStat = new ProfilerStatistic();
            startTime = DateTime.Now;

            if (File.Exists(historySavePath)) File.SetAttributes(historySavePath, FileAttributes.Normal);
            using (StreamWriter fs = new StreamWriter(File.Create(historySavePath), Encoding.GetEncoding(1251)))
            {
                fs.WriteLine("Time;CPU usage;RAM usage");
            }

            curProcess.Start();
            curProcess.BeginOutputReadLine();
            curProcess.BeginErrorReadLine();
            curProcess.PriorityClass = ProcessPriorityClass.BelowNormal;
            processCpuUsage = new PerformanceCounter("Process", "% Processor Time", curProcess.ProcessName);
            processRamUsage = new PerformanceCounter("Process", "Working Set", curProcess.ProcessName);
            Timer statTimer = new Timer(GetCurStat, null, 0, interval);
            Timer saveTimer = new Timer(SaveStoredStats, null, 0, interval * (maxMemHistorySize - 100));

            curProcess.WaitForExit();
            evalStat.execTime = (DateTime.Now - startTime).TotalSeconds;
            statTimer.Dispose();
            saveTimer.Dispose();
            SaveStoredStats();

            processRunning = false;
            return evalStat;
        }

        void DataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
                evalStat.programResult += Environment.NewLine + e.Data.ToString();
        }

        void ErrorReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
                evalStat.programResult += Environment.NewLine + e.Data.ToString();
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
            catch (Exception)
            {
            }
        }

        private float CurCpuUsage()
        {
            try
            {
                return processCpuUsage.NextValue();
            }
            catch (Exception)
            {
                return 0;
            }
        }

        private float CurRamUsage()
        {
            try
            {
                return processRamUsage.NextValue();
            }
            catch (Exception)
            {
                return 0;
            }
        }

        private void GetCurStat(object obj = null)
        {
            if (prohibitUsePageFile && SystemStateInfo.AvailableRamSize() < criticalAvailableMemSizeMB)
            {
                CancelProcess();
                exceededMemory = true;
                return;
            }

            statCount++;
            curProcMem = CurRamUsage() / 1024 / 1024;
            curProcCPU = CurCpuUsage() / SystemStateInfo.TotalCpuUsage();
            if (curProcCPU < 0 || curProcCPU > 1 || Double.IsNaN(curProcCPU)) curProcCPU = evalStat.avgCpuUsage;
            if (curProcCPU > evalStat.maxCpuUsage) evalStat.maxCpuUsage = curProcCPU;
            evalStat.avgCpuUsage += (curProcCPU - evalStat.avgCpuUsage) / statCount;
            if (curProcMem > evalStat.maxMemUsage) evalStat.maxMemUsage = curProcMem;

            lock (History)
            {
                History.Add(new StatUnit(curProcCPU, curProcMem, (DateTime.Now - startTime).TotalSeconds));
            }
        }

        private static Mutex mtx = new Mutex();
        private void SaveStoredStats(object obj = null)
        {
            mtx.WaitOne();
            List<StatUnit> histCopy;
            lock (History)
            {
                histCopy = History;
                History = new List<StatUnit>(maxMemHistorySize);
            }
            using (StreamWriter fs = new StreamWriter(File.Open(historySavePath, FileMode.Append), Encoding.GetEncoding(1251)))
            {
                foreach (StatUnit stat in histCopy)
                {
                    fs.WriteLine(stat.time.ToString("F") +  ";" + stat.cpuUsage.ToString("P2") + ";" + stat.ramUsage.ToString("F"));
                }
            }
            mtx.ReleaseMutex();
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

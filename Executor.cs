using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Auto_Statistic.Storage;

namespace Auto_Statistic
{
    public partial class Executor
    {
        private const string resultsFolderPath = @".\results\";

        private readonly ExecutionParameters executionParameters;
        private readonly CheckAlg checkAlgorithm;
        private Profiler profiler;
        private readonly Dictionary<string, object> storage = new Dictionary<string, object>();

        private string curPath;
        private readonly int tasksNumber;
        private int tasksCompleted;
        private string progressStatus = "";
        private bool cancel;

        public Executor(ExecutionParameters executionParameters, CheckAlg checkAlgorithm)
        {
            if (executionParameters.executionFilesPaths.Count <= 0)
                throw new Exception("Не указано ни одного исполняемого файла!");
            if (executionParameters.startParams.Count <= 0)
                throw new Exception("Не указано ни одного параметра запуска!");
            if (executionParameters.launchNum <= 0) throw new Exception("Количество запусков должно быть больше нуля!");
            if (executionParameters.variance < 0) throw new Exception("Среднеквадратичное отклонение должно быть больше нуля!");

            this.executionParameters = executionParameters;

            tasksNumber = executionParameters.executionFilesPaths.Count *
                          executionParameters.startParams.Count *
                          executionParameters.launchNum;
            if (executionParameters.textProgramFilesPaths.Count != 0)
            {
                tasksNumber *= executionParameters.textProgramFilesPaths.Count;
            }

            this.checkAlgorithm = checkAlgorithm;
        }

        public void Run()
        {
            cancel = false;
            tasksCompleted = 0;
            // Время необходимо указывать во всех именах файлов, тк эксель не может открыть 2 файла с одинаковым названием.
            string curTime = $"{DateTime.Now:yyyy'-'MM'-'dd'-T'HH'-'mm'-'ss}";

            if (!Directory.Exists(resultsFolderPath)) Directory.CreateDirectory(resultsFolderPath);

            curPath = resultsFolderPath + @"\" + curTime + @"\";
            if (!Directory.Exists(curPath)) Directory.CreateDirectory(curPath);

            string sysInfoPath = curPath + @"\System-Info_" + curTime + ".csv";
            if (!File.Exists(sysInfoPath))
            {
                progressStatus = "Сбор информации о системе.";
                FullSystemInfo.WriteFullSystemInfo(sysInfoPath);
            }

            string profilerFolderPath = curPath + @"\Profiler\";
            if (!Directory.Exists(profilerFolderPath)) Directory.CreateDirectory(profilerFolderPath);

            string logsFolderPath = curPath + @"\Logs\";
            if (!Directory.Exists(logsFolderPath)) Directory.CreateDirectory(logsFolderPath);

            string resultsPath = curPath + @"\Results_" + curTime + ".csv";
            if (File.Exists(resultsPath)) File.SetAttributes(resultsPath, FileAttributes.Normal);

            using (StreamWriter resFS = new StreamWriter(File.Create(resultsPath), Encoding.GetEncoding(1251)))
            {
                resFS.WriteLine(ExecutionResult.CsvNames());
                foreach (string executable in executionParameters.executionFilesPaths)
                {
                    foreach (string programTextFile in executionParameters.textProgramFilesPaths)
                    {
                        resFS.WriteLine(ExecutionResult.CsvEmpty());

                        string executableName = SaveName(string.IsNullOrEmpty(programTextFile)
                            ? Path.GetFileNameWithoutExtension(executable)
                            : $"{Path.GetFileNameWithoutExtension(executable)}_{Path.GetFileNameWithoutExtension(programTextFile)}");

                        string logPath = $@"{logsFolderPath}\Log_{executableName}_{curTime}.txt";
                        if (File.Exists(logPath)) File.SetAttributes(logPath, FileAttributes.Normal);

                        using (StreamWriter outLogFS =
                            new StreamWriter(File.Create(logPath), Encoding.GetEncoding(1251)))
                            for (int par = 0; par < executionParameters.startParams.Count; ++par)
                            {
                                ExecutionResult executionResult = new ExecutionResult
                                {
                                    programName = executableName,
                                    interpArgs = executionParameters.interprParams[par],
                                    arguments = executionParameters.startParams[par],
                                    avgExecTime = 0,
                                    variance = double.NaN,
                                    maxCpuUsage = 0,
                                    maxMemUsage = 0,
                                    avgCpuUsage = 0,
                                    execStatus = ExecutionStatus.Untested
                                };

                                outLogFS.WriteLine("\n------------------------------------------------------------");
                                outLogFS.WriteLine(executionResult.interpArgs + " " + executionResult.arguments);
                                outLogFS.WriteLine("------------------------------------------------------------");

                                int n = 0;
                                bool success = false;
                                for (var iteration = 1; iteration <= executionParameters.launchNum; ++iteration)
                                {
                                    if (cancel) return;

                                    progressStatus =
                                        $"Набор №{par + 1}. Проход №{iteration}. {executionResult.programName}";

                                    if (executionResult.execStatus != ExecutionStatus.Success &&
                                        executionResult.execStatus != ExecutionStatus.Untested ||
                                        executionResult.variance < executionParameters.variance)
                                    {
                                        tasksCompleted++;
                                        continue;
                                    }

                                    outLogFS.WriteLine(
                                        "\n____________________________________________________________\n");
                                    outLogFS.WriteLine($"Iteration: {iteration}\n");

                                    StringBuilder profilerPathBuilder = new StringBuilder();
                                    profilerPathBuilder.Append($@"{profilerFolderPath}\{executionResult.programName}_");
                                    profilerPathBuilder.Append(SaveName(executionResult.interpArgs + " " + executionResult.arguments));
                                    profilerPathBuilder.Append($"_{iteration}_{curTime}.csv");
                                    string profilerPath = profilerPathBuilder.ToString();

                                    if (File.Exists(profilerPath))
                                        File.SetAttributes(profilerPath, FileAttributes.Normal);
                                    if (string.IsNullOrEmpty(programTextFile))
                                    {
                                        profiler = new Profiler(executable,
                                            executionResult.arguments,
                                            profilerPath,
                                            executionParameters.prohibitUsePageFile,
                                            executionParameters.timeLimit);
                                    }
                                    else
                                    {
                                        profiler = new Profiler(executable,
                                            $"{executionResult.interpArgs} \"{programTextFile}\" {executionResult.arguments}",
                                            profilerPath,
                                            executionParameters.prohibitUsePageFile,
                                            executionParameters.timeLimit);
                                    }

                                    Profiler.ProfilerResultStatistic stats = profiler.StartProcess();

                                    if (profiler.IsExceededMemory())
                                    {
                                        executionResult.execStatus = ExecutionStatus.ExceededMemory;
                                        outLogFS.WriteLine("Exceeded memory!\n");
                                        break;
                                    }
                                    else if (profiler.IsProcessCanceled())
                                    {
                                        executionResult.execStatus = ExecutionStatus.Cancel;
                                        outLogFS.WriteLine("Canceled!\n");
                                    }
                                    else if (executionResult.execStatus == ExecutionStatus.Untested)
                                    {
                                        string refRes = "";
                                        if (!String.IsNullOrEmpty(executionParameters.referenceResults[par]))
                                            refRes = executionParameters.referenceResults[par];

                                        try
                                        {
                                            success = checkAlgorithm.Check(refRes, stats.programResult.ToString(),
                                                storage);
                                        }
                                        catch (Exception exc)
                                        {
                                            outLogFS.WriteLine($"Errored!\n{exc.InnerException ?? exc}\n");
                                            //Main.ShowError("Check error.\n\n", exc.InnerException ?? exc);
                                            success = false;
                                        }

                                        if (success) executionResult.execStatus = ExecutionStatus.Success;
                                        else executionResult.execStatus = ExecutionStatus.Error;
                                    }

                                    ++n;
                                    if (n == 2)
                                    {
                                        executionResult.variance =
                                            Math.Pow(stats.execTime - executionResult.avgExecTime, 2) / 2;

                                    }
                                    else if (n > 2)
                                    {
                                        executionResult.variance =
                                            (executionResult.variance +
                                             Math.Pow(stats.execTime - executionResult.avgExecTime, 2) / n -
                                             executionResult.variance / (n - 1));
                                    }

                                    executionResult.avgExecTime =
                                        executionResult.avgExecTime +
                                        (stats.execTime - executionResult.avgExecTime) / n;
                                    executionResult.avgCpuUsage =
                                        executionResult.avgCpuUsage +
                                        (stats.avgCpuUsage - executionResult.avgCpuUsage) / n;
                                    if (executionResult.maxMemUsage < stats.maxMemUsage)
                                        executionResult.maxMemUsage = stats.maxMemUsage;
                                    if (executionResult.maxCpuUsage < stats.maxCpuUsage)
                                        executionResult.maxCpuUsage = stats.maxCpuUsage;

                                    profiler = null;

                                    stats.LogToFile(outLogFS);

                                    tasksCompleted++;
                                }

                                executionResult.threadsCount = CoreNumParser.parseCoreNum(executionResult.interpArgs);
                                resFS.WriteLine(executionResult.CsvResults());
                            }
                    }
                }
            }
        }

        private string SaveName(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var ch in str)
            {
                if (!Path.GetInvalidFileNameChars().Contains(ch))
                {
                    sb.Append(ch);
                }
                else
                {
                    sb.Append("_");
                }
            }

            return sb.ToString();
        }

        public string GetResultsPath()
        {
            return curPath;
        }

        public int GetTasksNumber()
        {
            return tasksNumber;
        }

        public int GetCompletedTasksNum()
        {
            return tasksCompleted;
        }

        public string GetProgressStatus()
        {
            return progressStatus;
        }

        public void Cancel()
        {
            cancel = true;
            profiler.CancelProcess();
        }

        public float GetCurProcCpuUsage()
        {
            if (profiler != null) return profiler.GetCurProcCpuUsage();
            return 0;
        }

        public float GetCurProcMemUsage()
        {
            if (profiler != null) return profiler.GetCurProcMemUsage();
            return 0;
        }
    }
}

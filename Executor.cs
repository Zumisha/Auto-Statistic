using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Auto_Statistic
{
    public partial class Executor
    {
        private const string resultsFolderPath = @".\results\";

        private readonly ExecutionParameters executionParameters;
        private readonly CheckAlg checkAlgorithm;
        private Profiler profiler;

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

                        string executableName = (programTextFile == "")
                            ? Path.GetFileNameWithoutExtension(executable)
                            : $"{Path.GetFileNameWithoutExtension(executable)}_{Path.GetFileNameWithoutExtension(programTextFile)}";

                        string logPath = $@"{logsFolderPath}\Log_{executableName}_{curTime}.txt";
                        if (File.Exists(logPath)) File.SetAttributes(logPath, FileAttributes.Normal);

                        using (StreamWriter outLogFS =
                            new StreamWriter(File.Create(logPath), Encoding.GetEncoding(1251)))
                            for (int par = 0; par < executionParameters.startParams.Count; ++par)
                            {
                                ExecutionResult executionResult = new ExecutionResult
                                {
                                    programName = executableName,
                                    arguments = executionParameters.startParams[par],
                                    avgExecTime = 0,
                                    variance = double.NaN,
                                    maxCpuUsage = 0,
                                    maxMemUsage = 0,
                                    avgCpuUsage = 0,
                                    execStatus = "Success"
                                };

                                outLogFS.WriteLine("\n------------------------------------------------------------");
                                outLogFS.WriteLine(executionResult.arguments);
                                outLogFS.WriteLine("------------------------------------------------------------");

                                int n = 0;
                                for (var iteration = 1; iteration <= executionParameters.launchNum; ++iteration)
                                {
                                    if (cancel) return;

                                    progressStatus =
                                        $"Набор №{par + 1}. Проход №{iteration}. {executionResult.programName}";

                                    if (executionResult.execStatus != "Success" ||
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
                                    foreach (var ch in executionResult.arguments)
                                    {
                                        if (!Path.GetInvalidFileNameChars().Contains(ch))
                                        {
                                            profilerPathBuilder.Append(ch);
                                        }
                                    }

                                    profilerPathBuilder.Append($"_{iteration}_{curTime}.csv");
                                    string profilerPath = profilerPathBuilder.ToString();

                                    if (File.Exists(profilerPath))
                                        File.SetAttributes(profilerPath, FileAttributes.Normal);
                                    if (String.IsNullOrEmpty(programTextFile))
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
                                            $"\"{programTextFile}\" {executionResult.arguments}",
                                            profilerPath,
                                            executionParameters.prohibitUsePageFile,
                                            executionParameters.timeLimit);
                                    }

                                    Profiler.ProfilerResultStatistic stats = profiler.StartProcess();

                                    string refRes = "";
                                    if (!String.IsNullOrEmpty(executionParameters.referenceResults[par]))
                                        refRes = executionParameters.referenceResults[par];
                                    bool success;
                                    try
                                    {
                                        success = checkAlgorithm.Check(refRes, stats.programResult.ToString());
                                    }
                                    catch (Exception exc)
                                    {
                                        Main.ShowError("Check error.\n\n", exc.InnerException ?? exc);
                                        success = false;
                                    }

                                    if (profiler.IsExceededMemory())
                                    {
                                        executionResult.execStatus = "Exceeded Memory";
                                        outLogFS.WriteLine("Exceeded memory!\n");
                                        break;
                                    }
                                    else if(profiler.IsProcessCanceled())
                                    {
                                        executionResult.execStatus = "Canceled";
                                        outLogFS.WriteLine("Canceled!\n");
                                    }
                                    else if (!success)
                                    {
                                        executionResult.execStatus = "Errored";
                                        outLogFS.WriteLine("Errored!\n");
                                    }
                                    else
                                    {
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
                                    }

                                    profiler = null;

                                    stats.LogToFile(outLogFS);

                                    tasksCompleted++;
                                }

                                executionResult.threadsCount = CoreNumParser.parseCoreNum(executionResult.arguments);
                                resFS.WriteLine(executionResult.CsvResults());
                            }
                    }
                }
            }

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

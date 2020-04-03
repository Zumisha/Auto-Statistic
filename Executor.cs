using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.CSharp;

namespace Auto_Statistic
{
    public class Executor
    {
        private const string resultsFolderPath = @".\results\";

        private ExecutionParameters executionParameters= new ExecutionParameters();
        private Profiler profiler;
        private object checkAlgorithm;

        private string curPath;
        private readonly int tasksNumber;
        private int tasksCompleted;
        private string progressStatus = "";
        private bool cancel;

        private static readonly string[] coreParams =
        {
            "--num-cores ",
            "-n "
        };

        private static readonly Dictionary<string, string> providerOptions = new Dictionary<string, string>
        {
            {"CompilerVersion", "v4.0"}
        };

        private static readonly CSharpCodeProvider provider = new CSharpCodeProvider(providerOptions);

        private static readonly CompilerParameters compilerParams = new CompilerParameters
        {
            GenerateInMemory = true,
            GenerateExecutable = false
        };

        public static CompilerResults compileAlg(string text)
        {
            string program =
                "namespace Checker { public class Checker { public static bool Check(string output, string expected) {" +
                text + "} } }";
            CompilerResults checkRes = provider.CompileAssemblyFromSource(compilerParams, program);

            /*string[] outFile = System.IO.File.ReadAllText("out.txt").Replace("\n"," ").Trim().Split(' ');
            string[] refFile = System.IO.File.ReadAllText("reference.txt").Replace("\n", " ").Trim().Split(' ');
            for (var i = 0; i < System.Math.Min(outFile.Length,refFile.Length); ++i)
            {
                if (System.Math.Abs(double.Parse(outFile[i], CultureInfo.InvariantCulture) - double.Parse(refFile[i], CultureInfo.InvariantCulture)) > double.Epsilon)
                {
                    return false;
                }
            }
            return true;
            */

            return checkRes;
        }

        [Serializable]
        public class ExecutionParameters
        {
            public List<string> executionFilesPaths = new List<string>();
            public List<string> textProgramFilesPaths = new List<string>();
            public List<string> startParams = new List<string>();
            public List<string> referenceResults = new List<string>();
            public string checkAlgorithmText = defaultAlg;
            public ushort launchNum = 10;
            public float variance = 0;
            public byte backProcLimit = 10;
            public bool prohibitUsePageFile = true;
            public bool Interpr = true;
            public int timeLimit = 0;

            public static readonly string defaultAlg =
                @"return (output.IndexOf(""Error"") == -1) && 
            (output.IndexOf(expected) != -1);";
        }

        [Serializable]
        public struct Result
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

            private static string CsvStringBuilder(string programName, string arguments, string threadsCount,
                string avgExecTime, string variance, string maxMemUsage, string maxCpuUsage, string avgCpuUsage,
                string execStatus)
            {
                return "\"" + programName + "\";\"" + arguments + "\";\"" + threadsCount + "\";\"" + avgExecTime +
                       "\";\"" + variance + "\";\"" +
                       maxMemUsage + "\";\"" + maxCpuUsage + "\";\"" + avgCpuUsage + "\";\"" + execStatus + "\"";
            }

            public static string CsvNames()
            {
                return CsvStringBuilder("Program", "Arguments", "Threads", "Time, s", "Variance", "Max RAM, MB",
                    "Max CPU, %", "Avg CPU, %", "Status");
            }

            public static string CsvEmpty()
            {
                return CsvStringBuilder("", "", "", "", "", "", "", "", "");
            }

            public string CsvResults()
            {
                return CsvStringBuilder(programName, arguments, threadsCount.ToString(), avgExecTime.ToString("F3"),
                    variance.ToString("F4"), maxMemUsage.ToString("F"), maxCpuUsage.ToString("P2"),
                    avgCpuUsage.ToString("P2"), execStatus);
            }
        }

        public Executor(ExecutionParameters executionParameters, object checkAlgorithm)
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
                resFS.WriteLine(Result.CsvNames());
                foreach (string executable in executionParameters.executionFilesPaths)
                {
                    foreach (string programTextFile in executionParameters.textProgramFilesPaths)
                    {
                        resFS.WriteLine(Result.CsvEmpty());

                        string executableName = (programTextFile == "") ? 
                            Path.GetFileNameWithoutExtension(executable) : 
                            $"{Path.GetFileNameWithoutExtension(executable)}_{Path.GetFileNameWithoutExtension(programTextFile)}";

                        string logPath = $@"{logsFolderPath}\Log_{executableName}_{curTime}.txt";
                        if (File.Exists(logPath)) File.SetAttributes(logPath, FileAttributes.Normal);

                        using (StreamWriter outLogFS =
                            new StreamWriter(File.Create(logPath), Encoding.GetEncoding(1251)))
                            for (int par = 0; par < executionParameters.startParams.Count; ++par)
                            {
                                Result result = new Result
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

                                outLogFS.WriteLine(result.arguments);
                                outLogFS.WriteLine("------------------------------------------------------------");

                                int n = 0;
                                for (var iteration = 1; iteration <= executionParameters.launchNum; ++iteration)
                                {
                                    if (cancel) return;

                                    progressStatus = $"Набор №{par + 1}. Проход №{iteration}. {result.programName}";

                                    if (result.execStatus != "Success" || result.variance < executionParameters.variance)
                                    {
                                        tasksCompleted++;
                                        continue;
                                    }

                                    outLogFS.WriteLine(
                                        "\n____________________________________________________________\n");
                                    outLogFS.WriteLine($"Iteration: {iteration}\n");

                                    StringBuilder profilerPathBuilder = new StringBuilder();
                                    profilerPathBuilder.Append($@"{profilerFolderPath}\{result.programName}_");
                                    foreach (var ch in result.arguments)
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
                                            result.arguments, 
                                            profilerPath,
                                            executionParameters.prohibitUsePageFile, 
                                            executionParameters.timeLimit);
                                    }
                                    else
                                    {
                                        profiler = new Profiler(executable,
                                            $"\"{programTextFile}\" {result.arguments}",
                                            profilerPath,
                                            executionParameters.prohibitUsePageFile,
                                            executionParameters.timeLimit);
                                    }

                                    Profiler.ProfilerStatistic stats = profiler.StartProcess();
                                    string refRes = "";
                                    if (!String.IsNullOrEmpty(executionParameters.referenceResults[par]))
                                        refRes = executionParameters.referenceResults[par];

                                    bool success = false;
                                    MethodInfo check = checkAlgorithm.GetType().GetMethod("Check");
                                    try
                                    {
                                        success = (bool) check.Invoke(null, new object[] {stats.programResult, refRes});
                                    }
                                    catch (Exception exc)
                                    {
                                        Main.ShowError("Check error.\n\n", exc.InnerException ?? exc);
                                        success = false;
                                    }

                                    if (profiler.IsExceededMemory())
                                    {
                                        result.execStatus = "Exceeded Memory";
                                        outLogFS.WriteLine("Exceeded memory!\n");
                                        break;
                                    }

                                    if (profiler.IsProcessCanceled())
                                    {
                                        result.execStatus = "Canceled";
                                        outLogFS.WriteLine("Canceled!\n");
                                    }
                                    else if (!success)
                                    {
                                        result.execStatus = "Errored";
                                        outLogFS.WriteLine("Errored!\n");
                                    }
                                    else
                                    {
                                        ++n;
                                        if (n == 2)
                                        {
                                            result.variance = Math.Pow(stats.execTime - result.avgExecTime, 2) / 2;

                                        }
                                        else if (n > 2)
                                        {
                                            result.variance =
                                                (result.variance +
                                                 Math.Pow(stats.execTime - result.avgExecTime, 2) / n -
                                                 result.variance / (n - 1));
                                        }

                                        result.avgExecTime =
                                            result.avgExecTime + (stats.execTime - result.avgExecTime) / n;
                                        result.avgCpuUsage =
                                            result.avgCpuUsage + (stats.avgCpuUsage - result.avgCpuUsage) / n;
                                        if (result.maxMemUsage < stats.maxMemUsage)
                                            result.maxMemUsage = stats.maxMemUsage;
                                        if (result.maxCpuUsage < stats.maxCpuUsage)
                                            result.maxCpuUsage = stats.maxCpuUsage;
                                    }

                                    profiler = null;
                                    outLogFS.WriteLine(stats.programResult);
                                    tasksCompleted++;
                                }
                                
                                result.threadsCount = parseCoreNum(result.arguments);
                                resFS.WriteLine(result.CsvResults());

                                outLogFS.WriteLine("\n------------------------------------------------------------");
                            }
                    }
                }
            }

        }

        private int parseCoreNum(string arguments)
        {
            int coreNum, corePos = -1, i = -1;
            while (corePos == -1 && i < coreParams.Length - 1)
            {
                ++i;
                corePos = arguments.IndexOf(coreParams[i]);
            }

            if (corePos == -1)
            {
                coreNum = 0;
            }
            else
            {
                string arg = arguments.Substring(corePos + coreParams[i].Length);
                int splitPos = arg.IndexOf(" ", StringComparison.Ordinal);
                if (splitPos != -1)
                    arg = arg.Substring(0, splitPos);
                if (!Int32.TryParse(arg, out coreNum))
                    coreNum = 0;
            }

            return coreNum;
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
